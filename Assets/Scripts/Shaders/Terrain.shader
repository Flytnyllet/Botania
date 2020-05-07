Shader "Botania/Terrain"
{
	Properties
	{
		[PerRendererData]_MainTex("Texture", 2D) = "white" {}
		[PerRendererData]_AltTex("Texture", 2D) = "white" {}
		[PerRendererData]_Emission("Texture", 2D) = "white" {}
		[PerRendererData]_NoiseTextures("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Geometry" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		 #pragma surface surf Lambert  addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static float EPSILON = 1E-4;

		sampler2D _MainTex;
		sampler2D _AltTex;
		sampler2D _Emission;
		sampler2D _NoiseTextures;

		float3 baseColor;
		float baseTextureScale;
		float mainTexStart;
		float mainTexStop;

		//float minHeight;
		//float maxHeight;

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

		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_MainTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

			//triplanar mapping
			float3 GetTriplanarMap(sampler2D tex, float3 worldPos, float scale) {
				float3 scaledWorldPos = worldPos / scale;
				float3 xProjection = tex2D(tex, scaledWorldPos.zy);
				float3 yProjection = tex2D(tex, scaledWorldPos.xz);
				float3 zProjection = tex2D(tex, scaledWorldPos.xy);
				return xProjection + yProjection + zProjection;
			}
			float3 triplanar(sampler2D tex,float3 worldPos, float scale, float3 blendAxes) {
				float3 scaledWorldPos = worldPos / scale;
				float3 xProjection = tex2D(tex, float2(scaledWorldPos.y, scaledWorldPos.z))* blendAxes.x;
				float3 yProjection = tex2D(tex, float2(scaledWorldPos.x, scaledWorldPos.z)) * blendAxes.y;
				float3 zProjection = tex2D(tex, float2(scaledWorldPos.x, scaledWorldPos.y)) * blendAxes.z;
				return xProjection + yProjection + zProjection;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				float3 blendAxes = abs(IN.worldNormal);
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
				float3 scalesPos = IN.worldPos / 2;
				float  noiseVal =1-smoothstep(0.01,0.2,noise(IN.worldPos.xz/4));
				//float noiseStrenght = tex2D(_NoiseTextures, IN.uv_MainTex).x*baseTextureStrenght[0];
				float noiseStrenght = tex2D(_NoiseTextures, IN.uv_MainTex).x;
				//int2 pixelCoord = IN.uv_MainTex*_NoiseSize;
				//float noiseStrenght = smoothstep(0.01 ,0.05 , _NoiseArray[pixelCoord.x, pixelCoord.y].x);
				float3 altCol = tex2D(_AltTex, scalesPos.xz / 4);
				//float3 altCol = triplanar(_AltTex, IN.worldPos, 2, blendAxes);
				float3 colour = tex2D(_MainTex, scalesPos.xz / 4);
				//float3 colour = triplanar(_MainTex, IN.worldPos, 2, blendAxes);
				float3 emissions = tex2D(_Emission, scalesPos.xz / 4);
				//for (int i = 0; i < layerCount; i++) {
				//	//float drawStrength = inverseLerp();

				//	//float3 baseColor = baseColors[i] * baseColorStrength[i];
				//	//float3 textureColor = triplanar(float3(IN.worldPos), baseTextureScales[i], blendAxes, i)*noiseStrenght;
				//	float3 textureColor = triplanar(float3(IN.worldPos), baseTextureScales[i], blendAxes, i);

				//	o.Albedo = colour;
				//}
				o.Albedo = lerp(altCol, colour, smoothstep(mainTexStart, mainTexStop, noiseStrenght));
				o.Emission = lerp( float3(0,0,0), emissions, smoothstep(0.815, 0.825, noiseStrenght))*noiseVal;
				//o.Albedo = altCol+colour;
				//o.Albedo = float4(noiseStrenght, 0,0,1);
				//o.Albedo = float4(IN.uv_MainTex,0,1);
			}
			ENDCG
	}
		FallBack "Diffuse"
}
