Shader "LuisShadersUnlit/BolaDeFuego"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _AudioTex ("Audio Texture", 2D) = "white" {}
        _TimeScale ("Time Scale", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            sampler2D _AudioTex;
            float4 _MainTex_ST;
            float _TimeScale;
            //float4 _ScreenParams; // xy = resolution

            //HELPERS
            float fract(float x) { return x - floor(x); }
            float3 fract(float3 x) { return x - floor(x); }
            float mix(float a, float b, float t) { return lerp(a, b, t); }

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
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //before
                //fixed4 col = tex2D(_MainTex, i.uv);
                //return col;
                //after
                float2 fragCoord = i.uv * _ScreenParams.xy;
                float2 resolution = _ScreenParams.xy;
                float time = _Time.y * _TimeScale;

                // === Adaptación del mainImage ===
                float freqs[4];
                freqs[0] = tex2D(_AudioTex, float2(0.01, 0.25)).x;
                freqs[1] = tex2D(_AudioTex, float2(0.07, 0.25)).x;
                freqs[2] = tex2D(_AudioTex, float2(0.15, 0.25)).x;
                freqs[3] = tex2D(_AudioTex, float2(0.30, 0.25)).x;

                float brightness = freqs[1] * 0.25 + freqs[2] * 0.25;
                float radius = 0.24 + brightness * 0.2;
                float invRadius = 1.0 / radius;

                float aspect = resolution.x / resolution.y;
                float2 uv = fragCoord / resolution;
                float2 p = uv - 0.5;
                p.x *= aspect;

                float fade = pow(length(2.0 * p), 0.5);
                float fVal1 = 1.0 - fade;
                float fVal2 = 1.0 - fade;

                float angle = atan2(p.x, p.y) / 6.2832;
                float dist = length(p);
                float3 coord = float3(angle, dist, time * 0.1);

                float newTime1 = abs(snoise(coord + float3(0, -time * (0.35 + brightness * 0.001), time * 0.015), 15.0));
                float newTime2 = abs(snoise(coord + float3(0, -time * (0.15 + brightness * 0.001), time * 0.015), 45.0));

                for (int ii = 1; ii <= 7; ii++)
                {
                    float power = pow(2.0, ii + 1.0);
                    fVal1 += (0.5 / power) * snoise(coord + float3(0, -time, time * 0.2), (power * 10.0) * (newTime1 + 1.0));
                    fVal2 += (0.5 / power) * snoise(coord + float3(0, -time, time * 0.2), (power * 25.0) * (newTime2 + 1.0));
                }

                float corona = pow(fVal1 * max(1.1 - fade, 0.0), 2.0) * 50.0;
                corona += pow(fVal2 * max(1.1 - fade, 0.0), 2.0) * 50.0;
                corona *= 1.2 - newTime1;

                float2 sp = -1.0 + 2.0 * uv;
                sp.x *= aspect;
                sp *= (2.0 - brightness);
                float r = dot(sp, sp);
                float f = (1.0 - sqrt(abs(1.0 - r))) / r + brightness * 0.5;

                float3 starSphere = 0;
                if (dist < radius)
                {
                    corona *= pow(dist * invRadius, 24.0);
                    float2 newUv = sp * f + float2(time, 0.0);

                    float3 texSample = tex2D(_MainTex, newUv).rgb;
                    float uOff = (texSample.g * brightness * 4.5 + time);
                    float2 starUV = newUv + float2(uOff, 0.0);
                    starSphere = tex2D(_MainTex, starUV).rgb;
                }

                float3 orange = float3(0.8, 0.65, 0.3);
                float3 orangeRed = float3(0.8, 0.35, 0.1);
                float starGlow = saturate(1.0 - dist * (1.0 - brightness));

                float3 col = f * (0.75 + brightness * 0.3) * orange + starSphere + corona * orange + starGlow * orangeRed;
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
