Shader "Botania/Water"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Alpha("Alpha", Range(0,1)) = 0.0
		_Delta("Delta", Range(0,1)) = 0.01
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1

	}
		SubShader
		{
			Tags  { "RenderType" = "Geometry" "Queue" = "Transparent" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard 

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			float _Delta;
			float _Alpha;
			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float _DepthMaxDistance;
			sampler2D _CameraDepthTexture;

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

			float2 sobel(float2 uv) {
				float2 pos = uv;

				float BL = noise(pos + float2(-1.0, -1.0) * _Delta);
				float B = noise(pos + float2(0.0, -1.0) * _Delta) *  2.0;
				float BR = noise(pos + float2(1.0, -1.0) * _Delta);
				float L = noise(pos + float2(-1.0, 0.0) * _Delta) *  2.0;
				float R = noise(pos + float2(1.0, 0.0) * _Delta) * 2.0;
				float TL = noise(pos + float2(-1.0, 1.0) * _Delta);
				float T = noise(pos + float2(0.0, 1.0) * _Delta) *  2.0;
				float TR = noise(pos + float2(1.0, 1.0) * _Delta);
				float dX = (TR + R + BR) - (TL + L + BL);
				float dY = (BL + B + BR) - (TL + T + TR);
				return float2(dX, dY);
			}

			struct Input
			{
				float2 uv_MainTex;
				float3 worldPos;
				float3 worldNormal; INTERNAL_DATA
				float4 screenPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard  o)
			{
				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - IN.screenPos.w;
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
				
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				// Metallic and smoothness come from slider variables
				float3 trest = IN.worldNormal* float3(1,1,1);
				float2 waterNormal = sobel(IN.worldPos.xz + _Time.w*0.5);
				o.Normal = UnpackNormal(half4(waterNormal.x*0.01, -waterNormal.y*0.01,-1, 0))*0.5 + 0.5;
				o.Albedo = waterColor;
				o.Metallic = _Metallic* waterColor.a;
				o.Smoothness = _Glossiness* waterColor.a;
				o.Alpha = waterColor.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
