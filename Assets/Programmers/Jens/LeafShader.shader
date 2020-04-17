Shader "Botania/leaves"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		_EmissionMap("Emission Map", 2D) = "black" {}
		//[HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
	}
		SubShader{
			Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			LOD 250
			cull off


		CGPROGRAM
		#pragma surface surf Lambert noforwardadd addshadow
		#pragma shader_feature _METALLICGLOSSMAP
		#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _EmissionMap;
		sampler2D _Alpha;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			fixed facing : VFACE;
		};


		void surf(Input IN, inout SurfaceOutput o) {
			//o.Alpha = 1.0;
			float alpha = tex2D(_Alpha, IN.uv_MainTex).r;
			float2 pos = IN.screenPos.xy / IN.screenPos.w;
			pos *= _ScreenParams.xy; // pixel position
			float4x4 thresholdMatrix =
			{
			1.0 / 17.0,   9.0 / 17.0,   3.0 / 17.0,   11.0 / 17.0,
			13.0 / 17.0,  5.0 / 17.0,   15.0 / 17.0,  7.0 / 17.0,
			4.0 / 17.0,   12.0 / 17.0,  2.0 / 17.0,   10.0 / 17.0,
			16.0 / 17.0,  8.0 / 17.0,   14.0 / 17.0,  6.0 / 17.0
			};
			clip(alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex);

			if (IN.facing < 0.5)
				o.Normal *= -1.0;

		}
		ENDCG
	}
		Fallback "Differed"
}