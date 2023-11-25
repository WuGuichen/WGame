Shader "WGame/Stylized Highlight Spread"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        [Space(10)]
        _SpecularScale("Specular Scale", Range(0,1)) = 0.01
        _SpecularSmooth("Specular Smooth", Range(0,1)) = 0.001
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        
        [Space(10)]
        _SpreadSmooth("Spread Smooth", Range(0,1)) = 0.3
        _SpreadColor("Spread Color", Color) = (1,0,0,1)
        _SpreadScale("Spread Scale", Range(0,1)) = 0.3
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

            struct Attributes
            {
                float4 positonOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half _SpecularScale;
            half _SpecularSmooth;
            half4 _SpecularColor;
            half _SpreadSmooth;
            half4 _SpreadColor;
            half _SpreadScale;
            CBUFFER_END

            half StylizedHighlightScale(half3 lightDirWS, half3 normalWS, half3 viewDirWS, half highlightScale)
            {
                half3 halfDirWS = normalize(viewDirWS + lightDirWS);
                half NdotH = saturate(dot(normalWS, halfDirWS));
                half modifier = NdotH * (highlightScale + 0.5);
                return modifier;
            }

            half3 StylizedHighlightSmoothLerp(half3 startColor, half3 endColor, half smooth, half lerpDelta)
            {
                half colorLerpDelta = smoothstep(0.5 - smooth * 0.5, 0.5 + smooth * 0.5, lerpDelta);
                return lerp(startColor, endColor, colorLerpDelta);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positonOS.xyz);
                OUT.positionHCS = TransformObjectToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                
                return OUT;
            }


            half4 frag(Varyings IN) : SV_Target
            {
                Light light = GetMainLight();
                half3 lightColor = light.color * light.distanceAttenuation;

                //将扩散的高光和基础颜色插值在一起
                half spreadDelta = StylizedHighlightScale(light.direction,normalize(IN.normalWS), SafeNormalize(IN.viewDirWS),_SpreadScale);
                half3 spreadColor = StylizedHighlightSmoothLerp(_BaseColor.rgb,_SpreadColor.rgb * lightColor,_SpreadSmooth,spreadDelta);

                //将正常的高光和上一步的结果插值在一起
                half specularDelta = StylizedHighlightScale(light.direction,normalize(IN.normalWS), SafeNormalize(IN.viewDirWS),_SpecularScale);
                half3 finalSpecularColor = StylizedHighlightSmoothLerp(spreadColor.rgb,_SpecularColor.rgb * lightColor,_SpecularSmooth,specularDelta);
                
                half4 totalColor = half4(finalSpecularColor.rgb,1);
                return totalColor;
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
