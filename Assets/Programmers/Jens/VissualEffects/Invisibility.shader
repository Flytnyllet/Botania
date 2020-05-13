Shader "Botania/ScreenEffects/Invisibility"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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

			float Noise3D(float3 pos) {
				float AB = Noise(pos.xy);
				float BC = Noise(pos.yz);
				float AC = Noise(pos.xz);

				float BA = Noise(pos.yx);
				float CB = Noise(pos.zy);
				float CA = Noise(pos.zx);

				return (AB + BC + AC + BA + CB + CA) / 6;

			}

#define NUM_OCTAVES 5
			float fBm(float2 pos) {
				float v = 0.0;
				float a = 0.5;
				float2 shift = float2(100.0,100.0);
				// Rotate to reduce axial bias
				float2x2 rot = float2x2(cos(0.5), sin(0.5),-sin(0.5), cos(0.50));
				for (int i = 0; i < NUM_OCTAVES; ++i) {
					v += a * Noise(pos);
					pos = mul(rot, pos) * 2.0 + shift;
					a *= 0.5;
				}
				return v;
			}

			float fBm3D(float3 pos) {
				float AB = fBm(pos.xy);
				float BC = fBm(pos.yz);
				float AC = fBm(pos.xz);

				float BA = fBm(pos.yx);
				float CB = fBm(pos.zy);
				float CA = fBm(pos.zx);

				return (AB + BC + AC + BA + CB + CA) / 6;
				return 1;
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

		/*	struct appdata_full {

			};*/

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldPos:TEXCOORD1;
			};

			

			float4x4 gScreenToWorld;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				float4 clipVert = o.vertex;
				o.worldPos = mul(gScreenToWorld, clipVert);
				o.worldPos /= o.worldPos.w;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			fixed4 frag(v2f i) : SV_Target
			{
				float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
				//return float4(viewDir, 1);
				float3  worldPos = -i.worldPos.xyz;
				//worldpos.z = -i.worldPos.z*0.25 + depth;
				float depth = (1- tex2D(_CameraDepthTexture, i.uv));
				//depth += depth*(sin(i.uv.x*3.14)*0.314);
				//depth = smoothstep(0.5, 0.7, 1-depth)*depth;
				float3 uv = float3(i.uv * 2 - 1, depth);
				float dist = step(distance(uv, 0), 0.55);
				worldPos += depth* viewDir * 1;
				float noise = fBm3D(worldPos.xyz);
				//noise = smoothstep(0.3, 0.5, noise)*depth;
				return lerp(float4(worldPos.xyz%1,1), tex2D(_MainTex, i.uv), 0.7)*step(depth, 0.99999);
				return noise;
			}
			ENDCG
		}
	}
}
