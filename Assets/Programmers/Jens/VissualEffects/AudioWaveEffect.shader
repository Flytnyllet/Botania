Shader "Botania/ScreenEffects/AudioWaves"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		[PerRenderData] _Lerp("Effect Lerp Amount",range(0,1)) = 0
		_Str("Effect Ammount",range(0,1)) = 0
		_speed("Effect speed",float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_Thickness("Line Thickness", Float) = 0.01
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
				static const float pi = 3.141592653589793238462;
				static const float pi2 = 6.283185307179586476924;

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
				sampler2D _Noise;
				sampler2D _CameraDepthTexture;
				float4 _Color;
				float _Thickness;
				float _Lerp;
				float _Str;
				float _speed;

				fixed4 frag(v2f i) : SV_Target
				{
					float depth = 1 - tex2D(_CameraDepthTexture, i.uv).x * 1;
					depth += sin(i.uv.x*pi)*-.001;
					float waves =((depth + _Time.w*_speed*0.0001) % 0.05) * 1000;
					float wavesA = smoothstep(0.5- _Thickness, 0.5, waves);
					float wavesB = smoothstep(0.5, 0.5+ _Thickness, waves);
					waves = (wavesA - wavesB)*clamp(0, 1, tex2D(_CameraDepthTexture, i.uv).x * 10000);
					fixed4 col = tex2D(_MainTex, i.uv);
					float2 uv = i.uv - 0.5;
					//float tanVal = atan2(uv.y, uv.x) + pi;
					//float centerDist = distance(uv, 0);
					//float sinVal = sin((sin(depth) - _Time.w * 0.00001)*0.5 + 0.5);
					//float noiseVal = tex2D(_Noise, float2(0, sinVal));
					float4 effectCol = col - (1 - (wavesA - wavesB)*clamp(0,1,tex2D(_CameraDepthTexture, i.uv).x * 10000))*_Str;
					//return col*(1 - waves)+ waves* _Color;
					return lerp(col, effectCol, _Lerp) + waves * _Color*_Color.a;
				}
				ENDCG
			}
		}
}
