Shader "Botania/Water"
{
	Properties
	{
		_Color("Color Division", Color) = (1,1,1,1)
		_ColDivStr("Color Division Strenght", range(0,1)) = 1.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_FresnelLow("Fresnel Low", Range(-1,0.9)) = 0.0
		_FresnelHigh("Fresnel High", Range(0,1)) = 0.0
		_Alpha("Alpha (THIS DOES NOTHING!)", Range(0,1)) = 0.0
		_Delta("Delta", Range(0,1)) = 0.01
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
		_WaveDirection("Wave DiVector XY", Vector) = (1,1,0,0)
		_WaveSpeed("Wave Speed Multipier", float) = 1.0
		_Radius("Rain Drop Radius", float) = 2
		_RainSpeed("Rain Drop Speed", float) = 2
			//_RainWave("Rain To Wave Ratio", range(0,1)) = 0.9

	}
		SubShader
		{
			Tags  { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard exclude_path:deferred exclude_path:prepass alpha:fade

			#pragma target 3.0

			sampler2D _MainTex;
			float _Delta;
			float _Alpha;
			float _ColDivStr;
			float4 _Color;
			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float2 _WaveDirection;
			float _WaveSpeed;
			float _DepthMaxDistance;
			float _Radius;
			float _RainSpeed;
			float gRainWave;
			float _FresnelLow;
			float _FresnelHigh;
			float gEmissionMult;
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
				float3 viewDir;
				float3 vertexNormal;
			};

			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.vertexNormal = v.normal;
			}
			half _Glossiness;
			half _Metallic;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard  o)
			{
				int radius = 2;
				float EPSILON = 1e-3;
				//Dela upp i en 10*10 grid
				float2 origin = floor(IN.uv_MainTex * 10 / _Radius);
				float2 circles = float2(0, 0);
				for (int j = -radius; j <= radius; ++j)
				{
					for (int i = -radius; i <= radius; ++i)
					{
						//För varje grid
						float2 pi = origin + float2(i, j);

						float3 p3 = frac(float3(pi.xyx) * .103);
						p3 += dot(p3, p3.yzx + 19.19);
						float samplePoin = frac((p3.x + p3.y) * p3.z);

						float2 p = pi + samplePoin;

						float t = frac(0.3*_Time.w*_RainSpeed + samplePoin);
						float2 v = p - IN.uv_MainTex * 10 / _Radius;
						float dist = length(v) - (float(radius) + 1.)*t;
						float dist1 = dist - EPSILON;
						float dist2 = dist + EPSILON;
						float p1 = sin(31.*dist1) * smoothstep(-0.6, -0.3, dist1) * smoothstep(0., -0.3, dist1);
						float p2 = sin(31.*dist2) * smoothstep(-0.6, -0.3, dist2) * smoothstep(0., -0.3, dist2);
						circles += 0.5 * normalize(v) * ((p2 - p1) / (2. * EPSILON) * (1. - t) * (1. - t));
					}
				}
				circles /= float((radius * 2 + 1)*(radius * 2 + 1));

				float intensity = lerp(0.01, 0.15, smoothstep(0.1, 0.6, abs(frac(0.05*_Time.w*_RainSpeed + 0.5)*2. - 1.)));
				float3 rainNormal = float3(circles, sqrt(1. - dot(circles, circles)));

				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - IN.screenPos.w;
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 ambientCol = lerp(1, _Color, _ColDivStr);
				float4 waterColor = lerp(_DepthGradientShallow / ambientCol, _DepthGradientDeep / ambientCol, waterDepthDifference01);

				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				// Metallic and smoothness come from slider variables
				float3 trest = IN.worldNormal* float3(1,1,1);
				float2 waterNormal = sobel(IN.worldPos.xz + _WaveDirection * _Time.w*_WaveSpeed);
				float fresnelfactor = dot(float3(0, 1, 0),1 - normalize(UnityWorldSpaceViewDir(IN.worldPos)));

				float VissionPotionEffectMultiplier = 1 / gEmissionMult / gEmissionMult;

				fresnelfactor = smoothstep(_FresnelLow, _FresnelHigh, fresnelfactor);
				o.Normal += lerp(rainNormal ,UnpackNormal(half4(waterNormal*0.02,1, 0))*0.5 + 0.5, gRainWave);
				o.Albedo = waterColor * VissionPotionEffectMultiplier;
				o.Metallic = 1 * VissionPotionEffectMultiplier;
				o.Smoothness = _Glossiness * waterColor.a*fresnelfactor*VissionPotionEffectMultiplier;
				o.Alpha = fresnelfactor;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
