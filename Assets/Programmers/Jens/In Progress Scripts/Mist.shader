// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Mist"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		[NoScaleOffset] _SkyCubemap("-", Cube) = "" {}
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
			#pragma multi_compile _ USE_SKYBOX

			#include "UnityCG.cginc"

			sampler2D  _CameraDepthTexture;
	// Fog/skybox information
	half4 _FogColor;
	samplerCUBE _SkyCubemap;
	half4 _SkyCubemap_HDR;
	half4 _SkyTint;
	half _SkyExposure;
	float _SkyRotation;



			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uv_depth : TEXCOORD1;
				float3 ray : TEXCOORD2;
			};
			float3 RotateAroundYAxis(float3 v, float deg)
			{
				float alpha = deg * UNITY_PI / 180.0;
				float sina, cosa;
				sincos(alpha, sina, cosa);
				float2x2 m = float2x2(cosa, -sina, sina, cosa);
				return float3(mul(m, v.xz), v.y).xzy;
			}
			v2f vert(appdata_full  v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.ray = RotateAroundYAxis(v.texcoord1.xyz, -_SkyRotation);
				return o;
			}
			
			
			sampler2D _MainTex;
			sampler2D _Noise;

			fixed4 frag(v2f i) : SV_Target
			{
				half d = half(Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r));
				float fog = d * 5 ;
				half3 skyColor = DecodeHDR(texCUBE(_SkyCubemap, i.ray), _SkyCubemap_HDR);
				skyColor *= _SkyTint.rgb * _SkyExposure * unity_ColorSpaceDouble;
				// Lerp between source color to skybox color with fog amount.
				//return lerp(half4(skyColor, 1), half4(1,1,1,1), fog);
				return half4(skyColor, 1);

				//float4 base = tex2D(_MainTex, i.uv);
				//
				//d = (length(d) / 0.3);
				//return base+d;
			}
			ENDCG
		}
	 }
}
