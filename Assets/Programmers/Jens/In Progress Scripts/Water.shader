Shader "Unlit/Water"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	/*_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
	_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
	_DepthMaxDistance("Depth Maximum Distance", Float) = 1*/
	}
		SubShader
	{
		Tags { "RenderType" = "Geometry" "Queue" = "Transparent" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			/*float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float _DepthMaxDistance;
			sampler2D _CameraDepthTexture;*/

			float random(in float2 st) {
				return frac(sin(dot(st.xy,
									float2(12.9898,78.233)))
									* 43758.5453123);
			}
			float noise(float2 st) {
					float2 i = floor(st);
					float2 f = frac(st);

					float a = random(i);
					float b = random(i + float2(1.0, 0.0));
					float c = random(i + float2(0.0, 1.0));
					float d = random(i + float2(1.0, 1.0));


					float2 u = f * f*(3.0 - 2.0*f);


					return lerp(a, b, u.x) +
							(c - a)* u.y * (1.0 - u.x) +
							(d - b) * u.x * u.y;
			}
			float noise(float x,float y) {
					float2 i = floor(float2(x,y));
					float2 f = frac(float2(x,y));

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
				float3 worldPos : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				//float4 screenPosition : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				//o.screenPosition = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				/*float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
				*/
				float noiseVal = noise(i.worldPos.xz);
				fixed4 col = half4(noiseVal, noiseVal, noiseVal,1);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}
}
