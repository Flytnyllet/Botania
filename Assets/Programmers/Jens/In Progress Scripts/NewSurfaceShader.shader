
Shader "Tree" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
	}
		SubShader{
		  Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		  CGPROGRAM
		  #pragma surface surf Lambert vertex:vert alpha
		 
		struct Input {
			  float2 uv_MainTex;
			  float2 leafuv;
		  };

		struct appdata {
			float4 vertex : POSITION;
			float4 uv : TEXCOORD0;	//Texture
			float4 uv2 : TEXCOORD1;	//Physics
			float4 uv3 : TEXCOORD2;
		};

		sampler2D _MainTex;
		sampler2D _LeafTex;
		sampler2D _Noise;


		  void vert(inout appdata_full v, out Input l) {
			  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			  UNITY_INITIALIZE_OUTPUT(Input, l);
			  l.leafuv = v.texcoord3;
			  appdata o;
			  o.uv2 = float4(v.texcoord1.xy, 0, 0);
			  float f = tex2Dlod(_Noise, float4(abs(worldPos.xy % 1), 0,0)).r * 100;

			  v.vertex.x += cos(_Time.z*.3)*o.uv2.y*0.3f;
			  v.vertex.y += sin(_Time.z*.5)*o.uv2.y*0.1f;
		  }


		  void surf(Input IN, inout SurfaceOutput o) {
			  fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			  o.Albedo = (c.rgb  * c.a);
			  o.Emission = c.rgb.rgb;
			  o.Alpha = 1;
			  if (IN.leafuv.x > 0) {
				  o.Alpha = 0;
			  }
		  }
		  ENDCG
	  }
		  Fallback "Diffuse"
}