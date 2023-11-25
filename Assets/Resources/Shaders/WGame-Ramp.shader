Shader "WGame/Ramp"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _BaseMap("Base Map", 2D) = "white" {}
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        [NoScaleOffset] _Ramp("Ramp", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque" 
			"Queue"="Geometry"
            }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct a2v
            {
                float4 positonOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_Ramp);
            SAMPLER(sampler_Ramp);

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _SpecularColor;
            half _Smoothness;
            float4 _BaseMap_ST;
            CBUFFER_END

            v2f vert(a2v IN)
            {
                v2f OUT;

                float3 positionWS = TransformObjectToWorld(IN.positonOS.xyz);
                OUT.positionWS = positionWS;
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half LightingHalfLambert(half3 lightColor, half3 lightDirWS, half3 normalWS)
            {
                half NdotL = saturate(dot(normalWS, lightDirWS));
                // half halfLambert = pow(NdotL * 0.5 + 0.5, 2.0);
                half halfLambert = NdotL * 0.5 + 0.5;
                half4 ramp = SAMPLE_TEXTURE2D(_Ramp, sampler_Ramp, float2(halfLambert, halfLambert));
                return lightColor * ramp;
            }

            half4 frag(v2f IN) : SV_Target
            {
                Light light = GetMainLight();
                half3 lightColor = light.color * light.distanceAttenuation;
                half smoothness = exp2(10 * _Smoothness + 1);
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = SafeNormalize(IN.viewDirWS);

                half3 diffuseColor = LightingHalfLambert(lightColor, light.direction, normalWS);
                half3 specularColor = LightingSpecular(lightColor, light.direction, normalWS, viewDirWS, _SpecularColor, smoothness);

                int additionalLightCount = GetAdditionalLightsCount();
                for(int index = 0; index < additionalLightCount; index++)
                {
                    light = GetAdditionalLight(index, IN.positionWS);
                    half3 attenuatedLightColor = light.color + light.distanceAttenuation;
                    diffuseColor += LightingHalfLambert(attenuatedLightColor, light.direction, normalWS);
                    specularColor += LightingSpecular(attenuatedLightColor, light.direction, normalWS, viewDirWS, _SpecularColor, smoothness);
                }

                half3 ambientColor = unity_AmbientSky.rgb * _BaseColor;
                half4 mapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return half4(diffuseColor * _BaseColor * mapColor + specularColor + ambientColor, 1);
            }

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
            };


            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, float3(0,0,0)));
              #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
              #else
                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
              #endif
                output.positionCS = positionCS;
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET
            {
                return 0;
            }
            
            ENDHLSL
        }
    }
}
