Shader "WGame/Glass"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
    }
    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque" 
            }

        Pass
        {
            Tags {"LightMode"="UniversalForward"}
            
            HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma shader_feature _AdditionalLights
            
            #pragma vertex vert
            #pragma fragment frag

            CBUFFER_START(UnityPerMaterial)
            half4 _Specular;
            half4 _Diffuse;
            float _Gloss;
            CBUFFER_END

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }

            half4 frag (v2f IN) : SV_Target
            {
                Light mainLight = GetMainLight();
                half3 worldLightDir = normalize(TransformObjectToWorldDir(mainLight.direction));
                half3 diffuse = mainLight.color * _Diffuse.rgb * max(0, dot(IN.worldNormal, worldLightDir));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos);
                half3 halfDir = normalize(worldLightDir + viewDir);
                half3 specular = mainLight.color * _Specular.rgb * pow(max(0, dot(IN.worldNormal, halfDir)), _Gloss);
                half3 ambient = half3(unity_SHAr.w, unity_SHAr.w, unity_SHAr.w);
                half atten = mainLight.distanceAttenuation;

                half3 color = ambient + (diffuse + specular) * atten;

                #ifdef _AdditionalLights
                int lightCount = GetAdditionalLightsCount();
                for(int index = 0; index < lightCount; index++)
                {
                    Light light = GetAdditionalLight(index, IN.worldPos);
                    half3 diffuseAdd = light.color * _Diffuse.rgb * max(0, dot(IN.worldNormal, light.direction));
                    half3 halfDir = normalize(light.direction + viewDir);
                    half3 specularAdd = light.color * _Specular.rgb * pow(max(0, dot(IN.worldNormal, halfDir)), _Gloss);
                    color += (diffuseAdd + specularAdd) * light.distanceAttenuation;
                }
                #endif
                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
	FallBack "Packages/com.unity.render-pipelines.universal/FallbackError"
}
