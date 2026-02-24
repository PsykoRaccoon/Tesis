Shader "Custom/WallCutout_URP_Lit"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1,1,1,1)
        _Radius ("Cutout Radius", Float) = 2
        _Softness ("Edge Softness", Float) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }


            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float4 _BaseColor;

            float3 _PlayerPosition;
            float _Radius;
            float _Softness;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;


                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 albedoTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 albedo = albedoTex.rgb * _BaseColor.rgb;

                Light mainLight = GetMainLight();
                half3 normal = normalize(IN.normalWS);
                half NdotL = saturate(dot(normal, mainLight.direction));
                half3 diffuse = albedo * mainLight.color * NdotL;

                float2 fragScreenUV = IN.screenPos.xy / IN.screenPos.w;

                float4 playerHCS = TransformWorldToHClip(_PlayerPosition);
                float4 playerScreenPos = ComputeScreenPos(playerHCS);
                float2 playerScreenUV = playerScreenPos.xy / playerScreenPos.w;

                float2 delta = fragScreenUV - playerScreenUV;
                float distSqr = dot(delta, delta);

                float radius = _Radius * .2;
                float outer = radius + (_Softness * .2);

                float mask = smoothstep(radius * radius, outer * outer, distSqr);

                clip(mask - 0.01);

                return half4(diffuse, 1.0);
            }

            ENDHLSL
        }
    }
}