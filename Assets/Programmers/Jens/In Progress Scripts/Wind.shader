Shader "__Lab/WindMovement" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Colour", Color) = (0,0,0,1)
		_Noise("Noise", 2D) = "white" {}
		_Speed("Rotation Speed", float) = 1.0
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  CGPROGRAM
		  #pragma surface surf Lambert vertex:vert addshadow
		  #pragma target 3.0

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
		sampler2D _Noise;
		float4 _Color;
		half _Speed;



		//void vert(inout appdata_full v, out Input l) {
		   // float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
		   // UNITY_INITIALIZE_OUTPUT(Input, l);
		   // l.leafuv = v.texcoord3;
		   // appdata o;
		   // o.uv2 = float4( v.texcoord1.xy, 0, 0);
		   // float f = tex2Dlod(_Noise, float4(abs(worldPos.xy % 1), 0,0)).r * 100;

		   // if (o.uv2.y > .95f) {

			  //  v.vertex.x += cos(_Time.z*.3)*o.uv2.y*0.3f;
			  //  v.vertex.y += sin(_Time.z*.5)*o.uv2.y*0.1f;

			  //  v.vertex.x += cos(f + _Time.z*.3)*o.uv2.y*0.1f;
			  //  v.vertex.y += sin(f + _Time.z*.5)*o.uv2.y*0.05f;
		   // }
		//}

	  void vert(inout appdata_full v) {

		  float sin, cos;
		  sincos(_Time.y*_Speed, sin, cos);

		  v.vertex.x += cos * v.vertex.y*0.03f;
		  v.vertex.y += sin * v.vertex.y*0.01f;
	  }


	  void surf(Input IN, inout SurfaceOutput o) {
		  fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		  o.Albedo = c.rgb;
		  o.Alpha = c.a;
	  }
	ENDCG
	}
			Fallback "Diffuse"
}