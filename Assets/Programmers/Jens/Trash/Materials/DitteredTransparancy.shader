Shader "Botania/StippleTrancsparncy"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		_Transparency("Transparency", Range(0,1)) = 1.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 150
			cull off
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd
		sampler2D _MainTex;
		//float4 _MainTex_TexelSize;
		sampler2D _Alpha;
		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};
		half _Transparency;
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.w;
			//o.Alpha = tex2D(_Alpha, IN.uv_MainTex).x;
			float2 pos = IN.screenPos.xy / IN.screenPos.w;
			pos *= _ScreenParams.xy; // pixel position
			//pos = _MainTex_TexelSize.zw*IN.uv_MainTex;
			float4x4 thresholdMatrix =
			{
			1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
			13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
			4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
			16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
			};
			//clip(fmod(pos.x, _MainTex_TexelSize.x))
			clip(o.Alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
		}
		ENDCG
		}
			Fallback "Differed"
}