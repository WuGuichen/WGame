Shader "WGame/Tone Based Shading"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _WarmColor ("Warm Color", Color) = (1,1,1,1)
        _CoolColor ("Cool Color", Color) = (1,1,1,1)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
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
                float4 positionOS   : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
            half3 _BaseColor;
            half4 _WarmColor;
            half4 _CoolColor;
            half4 _SpecularColor;
            half _Smoothness;
            CBUFFER_END


            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewWS = GetWorldSpaceViewDir(positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                Light light = GetMainLight();
                half3 lightColor = light.color * light.distanceAttenuation;
                half3 normalWS = normalize(IN.normalWS);
                half3 viewWS =  SafeNormalize(IN.viewWS);
                half smoothness = exp2(10 * _Smoothness + 1);

                //这里灯光方向反过来方便后面带入公式
                half NdotL = dot(normalWS, -light.direction);
                float3 halfVec = SafeNormalize(float3(light.direction) + float3(viewWS));
                half NdotH = saturate(dot(normalWS, halfVec));

                //https://users.cs.northwestern.edu/~ago820/thesis/node26.html
                half coolAlpha = _CoolColor.a;
                half warmBeta = _WarmColor.a;
                //kd为黑色到自身颜色的渐变，既漫反射的颜色
                half3 kd = _BaseColor * ( 1 - NdotL) + half3(0,0,0) * NdotL;
                //根据参考文档kBlue对应的是冷色调，kYellow对应的是暖色调 kBlue = (0,0,b) kYellow =(y,y,0)
                //我这里直接用2个color表示了，kBlue = _CoolColor.rgb, kYellow = _WarmColor.rgb;
                //可以看到这个公式等于将一个冷色调到暖色调的ramp和一个物体背光面到向光面颜色的Ramp相加的结果(Ramp)
                half3 kCool = _CoolColor.rgb + coolAlpha * kd;
                half3 kWarm = _WarmColor.rgb + warmBeta * kd;
                half3 diffuseColor = ((1 + NdotL) / 2) * kCool + (1 - (1 + NdotL) / 2) * kWarm;
                //常规的Blinn-Phone高光
                half3 specularColor = LightingSpecular(lightColor, light.direction, normalWS,viewWS, _SpecularColor, smoothness);
                //常规的环境光
                half3 ambientColor = unity_AmbientSky.rgb * _BaseColor;
                half4 totalColor = half4(diffuseColor + specularColor + ambientColor ,1);

                return totalColor;
            }

            ENDHLSL
        }
//        Pass
//        {
//            Name "ShadowCaster"
//            Tags{"LightMode" = "ShadowCaster"}
//            
//            HLSLPROGRAM
//
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
//
//            struct Attributes
//            {
//                float4 positionOS   : POSITION;
//                float3 normalOS     : NORMAL;
//            };
//
//            struct Varyings
//            {
//                float4 positionCS   : SV_POSITION;
//            };
//
//
//            Varyings vert(Attributes input)
//            {
//                Varyings output;
//                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
//                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
//
//                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, float3(0,0,0)));
//              #if UNITY_REVERSED_Z
//                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
//              #else
//                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
//              #endif
//                output.positionCS = positionCS;
//                return output;
//            }
//
//            half4 frag(Varyings input) : SV_TARGET
//            {
//                return 0;
//            }
//            
//            ENDHLSL
//        }
    }
}
