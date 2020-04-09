Shader "Botania/Deferred"
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

			clip(o.Alpha -.05);
		}
		ENDCG
		}
			Fallback "Differed"
}