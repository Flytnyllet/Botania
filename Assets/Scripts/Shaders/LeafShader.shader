Shader "Botania/leaves"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		[Toggle(ALPHA_CUTOUT)]
		_Cutout("Alpha Cutout", float) = 0
		_CutoutValue("Alpha Cutout Value", float) = 0

		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmissionMult("Emission Multiplier", range(0,1)) = 1.0
			//_Metal("Metallness Map", 2D) = "black" {}
	//		_Rough("Roughness Map", 2D) = "black" {}
			_Metallic("Metallic", Range(0,1)) = 0.0
			//[HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
			_Speed("WindSpeed", float) = 1.0
			_StrenghtX("WindStrenghtHorizontal", float) = 0.5
			_StrenghtY("WindStrenghtVertical", float) = 0.5
			_StrenghtShake("Shake Strength", float) = 0.5
	}
		SubShader{
			Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			LOD 250
			cull off


		CGPROGRAM
			#pragma shader_feature ALPHA_CUTOUT 
			#pragma surface surf Lambert vertex:vert dithercrossfade addshadow
			#pragma target 3.0

			float gWindSpeed;

			sampler2D _MainTex;
			sampler2D _EmissionMap;
			sampler2D _Alpha;
			sampler2D _Metal;
			sampler2D _Rough;

			float _Metallic;
			float _EmissionMult;
			float _Speed;
			float _StrenghtX;
			float _StrenghtY;
			float _StrenghtShake;
			float _CutoutValue;
			float gEmissionMult;


			float random(float2 st) {
				return frac(sin(dot(st.xy,
					float2(12.9898, 78.233)))
					* 43758.5453123);
			}
			float Noise(float2 xy) {
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

			float Noise(float x, float y) {
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
#define NUM_OCTAVES 5
			float fBm(float2 pos) {
				float v = 0.0;
				float a = 0.5;
				float2 shift = float2(100.0, 100.0);
				// Rotate to reduce axial bias
				float2x2 rot = float2x2(cos(0.5), sin(0.5), -sin(0.5), cos(0.50));
				for (int i = 0; i < NUM_OCTAVES; ++i) {
					v += a * Noise(pos);
					pos = mul(rot, pos) * 2.0 + shift;
					a *= 0.5;
				}
				return v;
			}

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
				fixed facing : VFACE;
			};

			void vert(inout appdata_full v) {
				float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				float height = lerp(0,1, v.vertex.y);
				//float sinW = sin((unity_ObjectToWorld[1].x + unity_ObjectToWorld[1].z) + _Time.y*_Speed)*0.1;
				//float cosW = cos((unity_ObjectToWorld[1].x + unity_ObjectToWorld[1].z) + _Time.y*_Speed*2)*0.1;
				float sinW = sin((worldPos.x + worldPos.z) + _Time.y*_Speed)*0.1;
				float cosW = cos((worldPos.x + worldPos.z) + _Time.y*_Speed*2)*0.1;
				//float noise = sin(unity_ObjectToWorld[1].x+ unity_ObjectToWorld[1].z + _Time.y*_Speed);
				//float noise = fBm(unity_ObjectToWorld[1].xz+ _Time.y*_Speed);
				//float noise = sin((worldPos.x + worldPos.x) + _Time.y*_Speed * 10);
				float noise = fBm(worldPos.xz + _Time.w*_Speed )*0.1*_StrenghtShake;
				v.vertex.x += height * (sinW + noise * 0.2) *_StrenghtX*gWindSpeed;
				v.vertex.y += height * (cosW + noise * 0.2) *_StrenghtY*gWindSpeed;
			}

			void surf(Input IN, inout SurfaceOutput o) {
				float alpha = tex2D(_Alpha, IN.uv_MainTex).r;
				float2 pos = IN.screenPos.xy / IN.screenPos.w;
				pos *= _ScreenParams.xy; // pixel position
				float4x4 thresholdMatrix =
				{
				1.0 / 17.0,   9.0 / 17.0,   3.0 / 17.0,   11.0 / 17.0,
				13.0 / 17.0,  5.0 / 17.0,   15.0 / 17.0,  7.0 / 17.0,
				4.0 / 17.0,   12.0 / 17.0,  2.0 / 17.0,   10.0 / 17.0,
				16.0 / 17.0,  8.0 / 17.0,   14.0 / 17.0,  6.0 / 17.0
				};

#ifdef ALPHA_CUTOUT 
				clip(alpha - _CutoutValue);
#else
				clip(alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
#endif
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb/ gEmissionMult;
				o.Emission = tex2D(_EmissionMap, IN.uv_MainTex)*_EmissionMult*gEmissionMult;

				if (IN.facing < 0.5)
					o.Normal *= -1.0;

			}
			ENDCG
		}
			Fallback "Differed"
}