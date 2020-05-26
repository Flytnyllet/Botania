Shader "Botania/WindMovement_Fabric_PhysicsMapSupport" {
	Properties{
		_Color("Colour", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmissionMult("Emision Multiplier", float) = 1
		_PhysicsMap("Physics Map", 2D) = "black" {}
		_Alpha("Alpha Map", 2D) = "white" {}
		[Toggle(ALPHA_CUTOUT)]
		_Cutout("Alpha Cutout", float) = 0
		_CutoutValue("Cutout Value",range(0,1)) = 0.4
		_noiseTex("Noise", 2D) = "white" {}
		_WindSize("Wind Size", float) = 1.0
		_Speed("Speed", float) = 1.0
		_Strenght("Stremght", float) = 0.5
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  cull off
		  CGPROGRAM
		  #pragma surface surf Lambert vertex:vert addshadow
		  #pragma target 3.0 
		  #pragma shader_feature ALPHA_CUTOUT

		struct Input {
			  float2 uv_MainTex;
			  float4 screenPos;
		  };

		struct appdata {
			float4 vertex : POSITION;
			float4 uv : TEXCOORD0;	//Texture
			//float4 uv2 : TEXCOORD1;	//Physics
			//float4 uv3 : TEXCOORD2;
		};


		sampler2D _MainTex;
		sampler2D _EmissionMap;
		sampler2D _PhysicsMap;
		sampler2D _Alpha;
		sampler2D _noiseTex;
		float4 _Color;
		float _Speed;
		float _WindSize;
		float _Strenght;
		float _CutoutValue;
		float _EmissionMult;
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

	  void vert(inout appdata_full v) {
		  float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

		  float tex = tex2Dlod(_PhysicsMap, float4(v.texcoord.xy, 0, 0)).r;
		  float sinW = sin((worldPos.x + worldPos.z + worldPos.y + _Time.y*_Speed) * 4);
		  float noise = fBm(worldPos.xz + _Time.w*_Speed*0.5);

		  v.vertex.x += tex * (noise)*_Strenght;
		  v.vertex.y += tex * (sinW)*_Strenght;
	  }
	  static float thresholArray[] = {
		  0,48,12,60, 3,51,15,63,
		  32,16,44,28,35,19,47,31,
		  8,56, 4,52,11,59, 7,55,
		  40,24,36,20,43,27,39,23,
		  2,50,14,62, 1,49,13,61,
		  34,18,46,30,33,17,45,29,
		  10,58, 6,54, 9,57, 5,53,
		  42,26,38,22,41,25,37,21
	  };

	  void surf(Input IN, inout SurfaceOutput o) {
		  fixed4 c = tex2D(_MainTex, IN.uv_MainTex)*_Color;
		  o.Albedo = c.rgb / gEmissionMult;
		  o.Alpha = tex2D(_Alpha, IN.uv_MainTex)*_Color;
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
		  clip(o.Alpha - _CutoutValue);
#else
		  clip(o.Alpha -0.01- (thresholArray[pos.x%8*8+pos.y%8]) / 64);
		  //clip(o.Alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
#endif
		  o.Emission = tex2D(_EmissionMap, IN.uv_MainTex)*_EmissionMult*gEmissionMult;
	  }
	ENDCG
		}
			Fallback "Diffuse"
}