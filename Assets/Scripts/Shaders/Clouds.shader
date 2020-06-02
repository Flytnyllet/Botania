Shader "Unlit/Clouds"
{
	Properties
	{
		//_LowStep("Cloud Lower Cuttoff", range(0,1)) = 0.4
		_HighStep("Cloud Upper Cuttoff", range(0,2)) = 1.0
		_Directon("Cloud Velocity XY", Vector) = (1,1,0,0)
		_Offset("Cloud Offset XY", Vector) = (1,1,0,0)
		_CloudCol("Cloud Colour", Color) = (1,1,1,1)
		_FogColor("-", Color) = (0, 0, 0, 0)
		_FogHeight("Fog Height", float) = 0.5
		_FogOffset("Fog Height Offset", float) = 0.1
	}
		SubShader
	{
			Tags {"Queue" = "Background-1" "RenderType" = "Background" "PreviewType" = "Skybox"}

	 ZWrite Off
	 Cull Front
	 Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

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

#define NUM_OCTAVES 5
			float fbm(in float2 _st) {
				float v = 0.0;
				float a = 0.5;
				float2 shift = float2(100.0,100.0);
				// Rotate to reduce axial bias
				float2x2 rot = float2x2(cos(0.5), sin(0.5),-sin(0.5), cos(0.50));
				for (int i = 0; i < NUM_OCTAVES; ++i) {
					v += a * noise(_st);
					_st = mul(rot, _st) * 2.0 + shift;
					a *= 0.5;
				}
				return v;
			}



			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _FogColor;
			float4 _CloudCol;
			float _LowStep;
			float gCloudLowStep;
			float gEmissionMult;
			float _HighStep;
			float _FogHeight;
			float _FogOffset;
			float2 _Directon;
			float2 _Offset;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex)*0.1;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// apply fog
				float noise = fbm(i.worldPos.xz * 0.1 + _Offset + _Time.w*0.1*_Directon);
			//UNITY_APPLY_FOG(i.fogCoord, noise);
			//return float4(1, 1, 1, 1);
			float clouds = smoothstep(gCloudLowStep, _HighStep, noise)*(1 - step(_WorldSpaceCameraPos.y, 9.5))*smoothstep(-20, 1, i.worldPos.y);
			return clouds * _CloudCol / gEmissionMult;
			//return lerp(col, _FogColor, 1 - smoothstep(_FogHeight, _FogHeight + _FogOffset, i.worldPos.y));
		}
		ENDCG
	}
	}
}
