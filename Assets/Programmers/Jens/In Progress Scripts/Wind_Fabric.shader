Shader "__Lab/WindMovement_Fabric" {
	Properties{
		_Color("Colour", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_EmissionMap("Emission Map", 2D) = "black" {}
		_PhysicsMap("Physics Map", 2D) = "white" {}
		_Speed("Speed", float) = 1.0
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  cull off
		  CGPROGRAM
		  #pragma surface surf Lambert vertex:vert addshadow
		  #pragma target 3.0

		struct Input {
			  float2 uv_MainTex; 
			  float4 screenPos;
		  };

		struct appdata {
			float4 vertex : POSITION;
			float4 uv : TEXCOORD0;	//Texture
			//float4 uv2 : TEXCOORD1;	//Physics
			//float4 uv3 : TEXCOORD2;
		};


		sampler2D _MainTex;
		sampler2D _EmissionMap;
		sampler2D _PhysicsMap;
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
		  float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

		  float tex = tex2Dlod(_PhysicsMap, float4(v.texcoord.xy, 0, 0)).r;
		  
		  float largeWave = sin(worldPos.x + worldPos.z + worldPos.y + _Time.y*_Speed);
		  float smallWave = sin(worldPos.x + worldPos.z + worldPos.y + _Time.y*_Speed*0.25);

		  //v.vertex.x += cos * v.vertex.x*0.03f;
		  v.vertex.y += tex * (largeWave+smallWave)*0.1f;
	  }


	  void surf(Input IN, inout SurfaceOutput o) {
		  fixed4 c = tex2D(_MainTex, IN.uv_MainTex)*_Color;
		  o.Albedo = c.rgb;
		  o.Alpha = c.a; 
		  float2 pos = IN.screenPos.xy / IN.screenPos.w;
		  pos *= _ScreenParams.xy; // pixel position
		  float4x4 thresholdMatrix =
		  {
		  1.0 / 17.0,   9.0 / 17.0,   3.0 / 17.0,   11.0 / 17.0,
		  13.0 / 17.0,  5.0 / 17.0,   15.0 / 17.0,  7.0 / 17.0,
		  4.0 / 17.0,   12.0 / 17.0,  2.0 / 17.0,   10.0 / 17.0,
		  16.0 / 17.0,  8.0 / 17.0,   14.0 / 17.0,  6.0 / 17.0
		  };
		  clip(o.Alpha - thresholdMatrix[fmod(pos.x, 4)][pos.y % 4]);
		  o.Emission = tex2D(_EmissionMap, IN.uv_MainTex);
	  }
	ENDCG
		}
			Fallback "Diffuse"
}