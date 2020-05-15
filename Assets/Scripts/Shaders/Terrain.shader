Shader "Botania/Terrain"
{
	Properties
	{
		[PerRendererData]_MainTex("Texture", 2D) = "white" {}
		[PerRendererData]_AltTex0("Texture", 2D) = "white" {}
		[PerRendererData]_AltTex1("Texture", 2D) = "white" {}
		[PerRendererData]_AltTex2("Texture", 2D) = "white" {}
		[PerRendererData]_AltTex3("Texture", 2D) = "white" {}
		[PerRendererData]_Emission("Texture", 2D) = "white" {}
		[PerRendererData]_NoiseTexture("Texture", 2D) = "white" {}
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

		#define EPSILON = 1E-4;
		const static  int LAYER_COUNT = 4;

		sampler2D _MainTex;
		float baseTextureScale;
		float altTextureScale[LAYER_COUNT];
		float startStep[LAYER_COUNT];
		float endStep[LAYER_COUNT];
		float4 altTextureColour[LAYER_COUNT];
		sampler2D _Emission;
		sampler2D _AltTex0;
		sampler2D _AltTex1;
		sampler2D _AltTex2;
		sampler2D _AltTex3;
		sampler2D _NoiseTexture;

		float mainTexStart;
		float mainTexStop;
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
				//float3 blendAxes = abs(IN.worldNormal);
				//blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
				float  noiseVal = 1 - smoothstep(0.01, 0.2, Noise(IN.worldPos.xz / 4));

				float3 mainTex = tex2D(_MainTex, IN.worldPos.xz / baseTextureScale);
				float4 noise = tex2D(_NoiseTexture, IN.uv_MainTex);
				//noise.r = smoothstep(startStep[0], endStep[0], noise.r);
				//noise.g = smoothstep(startStep[1], endStep[1], noise.g)*(1 - noise.r);
				//noise.b = smoothstep(startStep[2], endStep[2], noise.b)*(1 - noise.r)*(1 - noise.g);
				//noise.a = smoothstep(startStep[3], endStep[3], noise.a)*(1 - noise.r)*(1 - noise.g)*(1 - noise.b);
				noise.r = smoothstep(startStep[0], endStep[0], noise.r);
				noise.g = smoothstep(startStep[1], endStep[1], noise.g)*(1 - noise.r);
				noise.b = smoothstep(startStep[2], endStep[2], noise.b)*(1 - noise.r)*(1 - noise.g);
				noise.a = smoothstep(startStep[3], endStep[3], noise.a)*(1 - noise.r)*(1 - noise.g)*(1 - noise.b);
				float mainTexVal = 1 * (1 - noise.r)*(1 - noise.g)*(1 - noise.b)*(1 - noise.a);
				mainTex *= mainTexVal;
				float3 emissions = tex2D(_Emission, IN.worldPos.xz / baseTextureScale);
				o.Emission = lerp(float3(0,0,0), emissions, mainTexVal)*noiseVal;
				//o.Albedo = altCol+colour;

				float4 altCol0 = tex2D(_AltTex0, IN.worldPos.xz / altTextureScale[0])*altTextureColour[0] * noise.r;
				float4 altCol1 = tex2D(_AltTex1, IN.worldPos.xz / altTextureScale[1])*altTextureColour[1] * noise.g;
				float4 altCol2 = tex2D(_AltTex2, IN.worldPos.xz / altTextureScale[2])*altTextureColour[2] * noise.b;
				float4 altCol3 = tex2D(_AltTex3, IN.worldPos.xz / altTextureScale[3])*altTextureColour[3] * noise.a;
				o.Albedo = (mainTex + altCol0 + altCol1 + altCol2 + altCol3) / gEmissionMult / gEmissionMult;
				//o.Albedo = o.Normal;
				//o.Albedo = noise;
			}
			ENDCG
	}
		FallBack "Diffuse"
}