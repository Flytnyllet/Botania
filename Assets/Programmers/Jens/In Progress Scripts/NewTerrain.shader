Shader "Botania/Terrain"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int MAX_COLOR_COUNT = 8;
		const static float EPSILON = 1E-4;

		int layerCount;
		float3 baseColors[MAX_COLOR_COUNT];
		float baseTextureScales[MAX_COLOR_COUNT];
		//float baseStartHeights[MAX_COLOR_COUNT];
		//float baseBlends[MAX_COLOR_COUNT];

		//float minHeight;
		//float maxHeight;


		UNITY_DECLARE_TEX2DARRAY(baseTextures);

        struct Input
        {
			float3 worldPos;
			float3 worldNormal;
        };


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float inverseLerp(float a, float b, float value)
		{
			return saturate((value - a) / (b - a));
		}

		//triplanar mapping
		float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			float3 colour = float3(0, 0, 0);

			for (int i = 0; i < layerCount; i++) {
				//float drawStrength = inverseLerp(-baseBlends[i] / 2 - EPSILON, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

				//float3 baseColor = baseColors[i] * baseColorStrength[i];
				float3 textureColor = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i);

				//o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
			}
        }
        ENDCG
    }
    FallBack "Diffuse"
}
