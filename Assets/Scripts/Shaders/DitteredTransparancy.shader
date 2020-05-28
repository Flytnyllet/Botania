Shader "Botania/DitteredTrancsparncy"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		[Toggle(ALPHA_CUTOUT)]
		_Cutout("Alpha Cutout", float) = 0
		_CutoutValue("Cutout Value",range(0,1)) = 0.4
		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmissionMult("Emission Multiplier", float) = 1
		_SpecularMap("Metallic", 2D) = "white" {}
		_RoughMap("Roghness", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_PhysicsMap("Physics", 2D) = "black"{}
		_floatX("Float Range X", float) = 0.01
		_floatY("Float Range Y", float) = 0.01
		_floatTimeX("Float Time Mult X", float) = 0.01
		_floatTimeY("Float Time Mult Y", float) = 0.01
	}
		SubShader{
			Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			LOD 150
			cull off


		CGPROGRAM
		#pragma surface surf Standard noforwardadd addshadow vertex:vert alpha:fade 
		#pragma shader_feature _EMISSION
		#pragma shader_feature _METALLICGLOSSMAP
		#pragma target 3.0
		#pragma shader_feature ALPHA_CUTOUT

		sampler2D _MainTex;
		sampler2D _EmissionMap;
		sampler2D _Alpha;
		sampler2D _SpecularMap;
		sampler2D _RoughMap;
		sampler2D _PhysicsMap;
		float4 _Color;
		float _CutoutValue;
		float _EmissionMult;
		float gEmissionMult;
		float _floatX;
		float _floatY;
		float _floatTimeY;
		float _floatTimeX;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			fixed facing : VFACE;
		};

		void vert(inout appdata_full v) {
			float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

			float physMask = tex2Dlod(_PhysicsMap, float4(v.texcoord.xy, 0, 0)).r;

			float sinW = sin(worldPos.x + worldPos.z + _Time.y*_floatTimeY);
			float cosW = sin(worldPos.x + worldPos.z + _Time.y*_floatTimeX);

			v.vertex.x += cosW * physMask*_floatX;
			v.vertex.y += sinW * physMask*_floatY;

			////Rotation Matrix
			//float2x2 m = float2x2(_CosTime.x, -_SinTime.x, _SinTime.x, _CosTime.x);
			//float2 rotation = mul(m, v.vertex.xy);
			//v.vertex.xy= lerp(v.vertex.xy, rotation, physMask);

		}
		void surf(Input IN, inout SurfaceOutputStandard  o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb*_Color / gEmissionMult / gEmissionMult;
			o.Alpha = tex2D(_Alpha, IN.uv_MainTex);
			//float2 pos = IN.screenPos.xy / IN.screenPos.w;
			//pos *= _ScreenParams.xy; // pixel position
			//float4x4 thresholdMatrix =
			//{
			//1.0 / 17.0,   9.0 / 17.0,   3.0 / 17.0,   11.0 / 17.0,
			//13.0 / 17.0,  5.0 / 17.0,   15.0 / 17.0,  7.0 / 17.0,
			//4.0 / 17.0,   12.0 / 17.0,  2.0 / 17.0,   10.0 / 17.0,
			//16.0 / 17.0,  8.0 / 17.0,   14.0 / 17.0,  6.0 / 17.0
			//};
//#ifdef ALPHA_CUTOUT 
//			clip(o.Alpha - _CutoutValue);
//#else
//			clip(o.Alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
//#endif
			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex)*_EmissionMult*gEmissionMult;
			o.Metallic = tex2D(_SpecularMap, IN.uv_MainTex).r*tex2D(_RoughMap, IN.uv_MainTex).r;

			if (IN.facing < 0.5)
				o.Normal *= -1.0;

		}
		ENDCG
		}
			Fallback "Differed"
}