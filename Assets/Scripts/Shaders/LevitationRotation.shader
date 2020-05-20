Shader "Botania/RotationShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Speed("Rotation Speed", float) = 1.0
		_BobAmmount("Bobbing Ammount", float) = 0.1
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		[Toggle(ALPHA_CUTOUT)]
		_Cutout("Alpha Cutout", float) = 0
		_CutoutValue("Alpha Cutout Value", float) = 0

		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmissionMult("Emission Multiplier", float) = 1.0

	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			cull off
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _Alpha;
			sampler2D _EmissionMap;
			float _BobAmmount;
			float _EmissionMult;
			float gEmissionMult;

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
				fixed facing : VFACE;
			};

			float _Random;
			half _Speed;
			void vert(inout appdata_full v) {
				//Rotation Matrix
				float sin, cos;
				sincos(_Random + _Time.y*_Speed, sin, cos);
				float2x2 m = float2x2(cos, -sin, sin, cos);
				v.vertex.xz = mul(m, v.vertex.xz);
				v.vertex.y += sin* _BobAmmount;
			}

			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
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

	#ifdef ALPHA_CUTOUT 
				clip(alpha - _CutoutValue);
	#else
				clip(alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
	#endif
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb / gEmissionMult;
				o.Emission = tex2D(_EmissionMap, IN.uv_MainTex)*_EmissionMult*gEmissionMult;

				if (IN.facing < 0.5)
					o.Normal *= -1.0;

			}
			ENDCG
		}
			FallBack "Diffuse"
}
