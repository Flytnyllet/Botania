Shader "Botania/ScreenEffects/Underwater"
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


			float Random(float2 st) {
				return frac(sin(dot(st.xy,
					float2(12.9898, 78.233)))
					* 43758.5453123);
			}
			float Noise(float2 xy) {
				float2 i = floor(xy);
				float2 f = frac(xy);

				float a = Random(i);
				float b = Random(i + float2(1.0, 0.0));
				float c = Random(i + float2(0.0, 1.0));
				float d = Random(i + float2(1.0, 1.0));

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
					v += a * Noise(_st);
					_st = mul(rot, _st) * 2.0 + shift;
					a *= 0.5;
				}
				return v;
			}
			float fBm3D(float3 coords) {
				float xy = fbm(coords.xy);
				float xz = fbm(coords.xz);
				float yz = fbm(coords.yz);
				float yx = fbm(coords.yz);
				float zx = fbm(coords.zx);
				float zy = fbm(coords.zy);
				return (xy + xz + yz + yx + zx + zy) / 6;
			}
			/**
 * Return the normalized direction to march in from the eye point for a single pixel.
 *
 * fieldOfView: vertical field of view in degrees
 * size: resolution of the output image
 * fragCoord: the x,y coordinate of the pixel in the output image
 */
			float3 rayDirection(float fieldOfView, float2 size, float2 fragCoord) {
				float2 xy = fragCoord - size / 2.0;
				float z = size.y / tan(radians(fieldOfView) / 2.0);
				return normalize(float3(xy, -z));
			}

			float3 Direction(float2 fragCoord)
			{
				fragCoord -= 0.5;
				float2 horizontal = lerp(float2(0, 0), float2(0.34202059099, 0.93969245784), fragCoord.x);
				float2 vertical = lerp(float2(0, 0), float2(0.68199836006, 0.73135370161), fragCoord.y);
				return normalize(float3(horizontal, 0) + float3(0, vertical));
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 interpolatedRay : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.interpolatedRay = v.ray;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			fixed4 frag(v2f i) : SV_Target
			{
				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv));
				float depth = Linear01Depth(rawDepth) * 1850;
				//float4 wsDir = depth * i.interpolatedRay*0.99;
				//float3 position = _WorldSpaceCameraPos + wsDir;
				//position.x += _Time.y * 2;

				return smoothstep(0, 50, depth);


			}
			ENDCG
		}
	}
}
