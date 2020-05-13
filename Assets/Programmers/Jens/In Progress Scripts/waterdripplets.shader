Shader "Hidden/NewImageEffectShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Radius("Radius", int) = 2
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"


				float random(float2 st) {
					return frac(sin(dot(st.xy,
						float2(12.9898, 78.233)))
						* 43758.5453123);
				}
				float noise(float2 xy) {
					float2 i = floor(xy);
					float2 f = frac(xy);

					float a = random(i);
					float b = random(i + float2(1.0, 0.0));
					float c = random(i + float2(0.0, 1.0));
					float d = random(i + float2(1.0, 1.0));

					float2 u = f * f*(3.0 - 2.0*f);

					return lerp(a, b, u.x) +
						(c - a)* u.y * (1.0 - u.x) +
						(d - b) * u.x * u.y;
				}
				float noise(float x, float y) {
					float2 i = floor(float2(x, y));
					float2 f = frac(float2(x, y));

					float a = random(i);
					float b = random(i + float2(1.0, 0.0));
					float c = random(i + float2(0.0, 1.0));
					float d = random(i + float2(1.0, 1.0));

					float2 u = f * f*(3.0 - 2.0*f);

					return lerp(a, b, u.x) +
						(c - a)* u.y * (1.0 - u.x) +
						(d - b) * u.x * u.y;
				}


				float samplePoint(float2 p)
				{
					float3 p3 = frac(float3(p.xyx) * .103);
					p3 += dot(p3, p3.yzx + 19.19);
					return frac((p3.x + p3.y) * p3.z);
				}

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

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				int _Radius;

				fixed4 frag(v2f IN) : SV_Target
				{
					float EPSILON = 1e-3;
				//Dela upp i en 10*10 grid
					float2 origin = floor(IN.uv*10);
					float2 circles = float2(0,0);
					for (int j = -_Radius; j <= _Radius; ++j)
					{
						for (int i = -_Radius; i <= _Radius; ++i)
						{
							//För varje grid
							float2 pi = origin + float2(i, j);

							float3 p3 = frac(float3(pi.xyx) * .103);
							p3 += dot(p3, p3.yzx + 19.19);
							float samplePoin = frac((p3.x + p3.y) * p3.z);
							
							float2 p = pi + samplePoin;

							float t = frac(0.3*_Time.w*0.5 + samplePoin);
							float2 v = p - IN.uv * 10;
							float dist = length(v) - (float(_Radius) + 1.)*t;
							float dist1 = dist - EPSILON;
							float dist2 = dist + EPSILON;
							float p1 = sin(31.*dist1) * smoothstep(-0.6, -0.3, dist1) * smoothstep(0., -0.3, dist1);
							float p2 = sin(31.*dist2) * smoothstep(-0.6, -0.3, dist2) * smoothstep(0., -0.3, dist2);
							circles += 0.5 * normalize(v) * ((p2 - p1) / (2. * EPSILON) * (1. - t) * (1. - t));
						}
					}
					circles /= float((_Radius * 2 + 1)*(_Radius * 2 + 1));

					float intensity = lerp(0.01, 0.15, smoothstep(0.1, 0.6, abs(frac(0.05*_Time.w*0.5+ 0.5)*2. - 1.)));
					float3 normal = float3(circles, sqrt(1. - dot(circles, circles)));


					//float2 uv = IN.uv*10;
					//float4 o = _Time;

					//uv = mul(uv, float2x2(7, -5, 5, 7)*.1);
					//o = min(o, length(frac(uv) - .5) / .6);
					//uv = mul(uv, float2x2(7, -5, 5, 7)*.1);
					//o = min(o, length(frac(uv) - .5) / .6);
					//uv = mul(uv, float2x2(7, -5, 5, 7)*.1);
					//o = min(o, length(frac(uv) - .5) / .6);





/*

							fixed4 col = tex2D(_MainTex, IN.uv);
							float2 uv = IN.uv - 0.5;
							float dist = step(distance(uv, 0), 0.55);*/

							return float4(normal,1);
						}
						ENDCG
					}
		}
}
