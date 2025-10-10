Shader "LuisShadersUnlit/BolaDeFuego3D_Full_v2"
{
    Properties
    {
        _MainTex ("Main Texture (stars/noise)", 2D) = "white" {}
        _AudioTex ("Audio Texture (optional)", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.80,0.65,0.30,1)
        _HighlightColor ("Highlight Color", Color) = (0.80,0.35,0.10,1)
        _TimeScale ("Time Scale", Float) = 0.1
        _NoiseScale ("Noise Scale", Float) = 8.0
        _CoronaIntensity ("Corona Intensity", Float) = 0.9
        _StarIntensity ("Star Intensity", Float) = 0.6
        _BaseIntensity ("Base Intensity", Float) = 0.9
        _UseAudio ("Use Audio (0/1)", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _AudioTex;
            float4 _BaseColor;
            float4 _HighlightColor;
            float _TimeScale;
            float _NoiseScale;
            float _CoronaIntensity;
            float _StarIntensity;
            float _BaseIntensity;
            float _UseAudio;

            static const float PI = 3.14159265359;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            // --- snoise (mantengo tu implementación, devuelve aprox [-1,1]) ---
            float snoise(float3 uv, float res)
            {
                float3 s = float3(1.0, 100.0, 10000.0);
                uv *= res;

                float3 uv0 = floor(fmod(uv, res)) * s;
                float3 uv1 = floor(fmod(uv + 1.0, res)) * s;

                float3 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                float4 v = float4(
                    uv0.x + uv0.y + uv0.z,
                    uv1.x + uv0.y + uv0.z,
                    uv0.x + uv1.y + uv0.z,
                    uv1.x + uv1.y + uv0.z
                );

                float4 r = frac(sin(v * 1e-3) * 1e5);
                float r0 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

                r = frac(sin((v + uv1.z - uv0.z) * 1e-3) * 1e5);
                float r1 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

                return lerp(r0, r1, f.z) * 2.0 - 1.0;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz; // posición en espacio-objeto (espera esfera centrada en 0)
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // tiempo
                float time = _Time.y * _TimeScale;

                // dirección sobre la esfera (normalizamos el espacio-objeto)
                float3 p = normalize(i.objPos);

                // mapeo esférico -> uv en [0,1]
                float u = atan2(p.z, p.x) / (2.0 * PI) + 0.5;
                float v = asin(p.y) / PI + 0.5;
                float2 sphUV = float2(u, v);

                // brightness (opcional desde textura de audio)
                float brightness = 0.5;
                if (_UseAudio > 0.5)
                {
                    float fre1 = tex2D(_AudioTex, float2(0.07, 0.25)).x;
                    float fre2 = tex2D(_AudioTex, float2(0.15, 0.25)).x;
                    brightness = fre1 * 0.25 + fre2 * 0.25;
                }

                // coordenadas para ruido (controladas por _NoiseScale)
                float3 coord = float3(u * _NoiseScale, v * _NoiseScale, time * 0.1);

                // sumas de octavas (más moderadas)
                float n1 = 0.0;
                float n2 = 0.0;
                for (int ii = 1; ii <= 7; ii++)
                {
                    float power = pow(2.0, ii + 1.0);
                    n1 += (0.5 / power) * snoise(coord + float3(0.0, -time, time * 0.2), power * 10.0 * (1.0 + 0.5 * brightness));
                    n2 += (0.5 / power) * snoise(coord + float3(0.0, -time * 0.2, time * 0.2), power * 25.0 * (1.0 + 0.5 * brightness));
                }

                // normalizamos ruido a [0,1] de forma controlada
                float n1norm = saturate(n1 * 0.5 + 0.5);
                float n2norm = saturate(n2 * 0.5 + 0.5);

                // corona (multiplicadores reducidos)
                float corona = pow(n1norm, 2.0) * _CoronaIntensity;
                corona += pow(n2norm, 1.6) * (_CoronaIntensity * 0.6);
                corona *= saturate(0.9 + 0.2 * brightness); // pequeña dependencia al audio

                // detalle "stars"/texture
                float2 starUV = frac(sphUV + float2(time * 0.05, 0.0));
                float3 starSphere = tex2D(_MainTex, starUV).rgb * _StarIntensity;

                // composición de color
                float3 baseCol = _BaseColor.rgb * _BaseIntensity;
                float3 col = baseCol;
                col += starSphere * 0.6;
                col += corona * _BaseColor.rgb * 0.9;
                col += _HighlightColor.rgb * (pow(n1norm, 1.5) * 0.45);

                // Tonemapping simple (Reinhard) para evitar picos HDR brutales
                col = col / (1.0 + col);

                // normalizamos para seguridad
                col = saturate(col);

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
