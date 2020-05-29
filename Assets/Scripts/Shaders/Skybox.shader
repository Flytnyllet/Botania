// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Botania/Skybox" {
	Properties{
		_Tint("Tint Color", Color) = (.5, .5, .5, .5)
		[Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
		_Rotation("Rotation", Range(0, 360)) = 0
		_FogColor("-", Color) = (0, 0, 0, 0)
		_FogHeight("Fog Height", Range(0, 1)) = 0.5
		_FogOffset("Fog Height Offset", Range(0, 1)) = 0.1
		[NoScaleOffset] _FrontTex("Front [+Z]   (HDR)", 2D) = "grey" {}
		[NoScaleOffset] _BackTex("Back [-Z]   (HDR)", 2D) = "grey" {}
		[NoScaleOffset] _LeftTex("Left [+X]   (HDR)", 2D) = "grey" {}
		[NoScaleOffset] _RightTex("Right [-X]   (HDR)", 2D) = "grey" {}
		[NoScaleOffset] _UpTex("Up [+Y]   (HDR)", 2D) = "grey" {}
		[NoScaleOffset] _DownTex("Down [-Y]   (HDR)", 2D) = "grey" {}
	}

		SubShader{
			Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
			Cull Off ZWrite Off

			CGINCLUDE
			#include "UnityCG.cginc"
			#pragma multi_compile FOG_LINEAR FOG_EXP FOG_EXP2

			half4 _FogColor;
			half _FogHeight;
			half _FogOffset;
			half4 _Tint;
			half _Exposure;
			float _Rotation;
			float gEmissionMult;


			float random(float2 st) {
				return frac(sin(dot(st.xy,
					float2(12.9898, 78.233)))
					* 43758.5453123);
			}
			float noise(float2 xy) {
				float2 i = floor(xy);
				float2 f = frac(xy);

				float a = random(i);
				float b = random(i + float2(1.0, 0.0));
				float c = random(i + float2(0.0, 1.0));
				float d = random(i + float2(1.0, 1.0));

				float2 u = f * f*(3.0 - 2.0*f);

				return lerp(a, b, u.x) +
					(c - a)* u.y * (1.0 - u.x) +
					(d - b) * u.x * u.y;
			}
			float noise(float x, float y) {
				float2 i = floor(float2(x, y));
				float2 f = frac(float2(x, y));

				float a = random(i);
				float b = random(i + float2(1.0, 0.0));
				float c = random(i + float2(0.0, 1.0));
				float d = random(i + float2(1.0, 1.0));

				float2 u = f * f*(3.0 - 2.0*f);

				return lerp(a, b, u.x) +
					(c - a)* u.y * (1.0 - u.x) +
					(d - b) * u.x * u.y;
			}

#define NUM_OCTAVES 5
			float fbm(in float2 _st) {
				float v = 0.0;
				float a = 0.5;
				float2 shift = float2(100.0,100.0);
				// Rotate to reduce axial bias
				float2x2 rot = float2x2(cos(0.5), sin(0.5),-sin(0.5), cos(0.50));
				for (int i = 0; i < NUM_OCTAVES; ++i) {
					v += a * noise(_st);
					_st = mul(rot, _st) * 2.0 + shift;
					a *= 0.5;
				}
				return smoothstep(0.5, 1, v);
			}



	float3 RotateAroundYInDegrees(float3 vertex, float degrees)
	{
		float alpha = degrees * UNITY_PI / 180.0;
		float sina, cosa;
		sincos(alpha, sina, cosa);
		float2x2 m = float2x2(cosa, -sina, sina, cosa);
		return float3(mul(m, vertex.xz), vertex.y).xzy;
	}

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};
	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};
	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
		o.vertex = UnityObjectToClipPos(rotated);
		o.texcoord = v.texcoord;
		return o;
	}
	/////////////
	half4 skybox_frag(v2f i, sampler2D smp, half4 smpDecode)
	{
		half4 tex = tex2D(smp, i.texcoord);
		half3 c = DecodeHDR(tex, smpDecode);
		c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
		c *= _Exposure;
		half4 skyCol = half4(c, 1);
			return lerp(skyCol, _FogColor, 1 - smoothstep(_FogHeight, _FogHeight + _FogOffset, i.texcoord.y))/ gEmissionMult/ gEmissionMult;
	}
	half4 skybox_frag_Top(v2f i, sampler2D smp, half4 smpDecode)
	{
		half4 tex = tex2D(smp, i.texcoord);
		half3 c = DecodeHDR(tex, smpDecode);
		c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
		c *= _Exposure;
		return half4(c, 1) / gEmissionMult / gEmissionMult;
	}
	half4 skybox_frag_Down(v2f i, sampler2D smp, half4 smpDecode)
	{
		half4 tex = tex2D(smp, i.texcoord);
		half3 c = DecodeHDR(tex, smpDecode);
		c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
		c *= _Exposure;
		return  _FogColor / gEmissionMult / gEmissionMult;
	}
	ENDCG

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		sampler2D _FrontTex;
		half4 _FrontTex_HDR;
		half4 frag(v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex_HDR); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		sampler2D _BackTex;
		half4 _BackTex_HDR;
		half4 frag(v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex_HDR); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		sampler2D _LeftTex;
		half4 _LeftTex_HDR;
		half4 frag(v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex_HDR); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		sampler2D _RightTex;
		half4 _RightTex_HDR;
		half4 frag(v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex_HDR); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		sampler2D _UpTex;
		half4 _UpTex_HDR;
		half4 frag(v2f i) : SV_Target { return skybox_frag_Top(i,_UpTex, _UpTex_HDR); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		sampler2D _DownTex;
		half4 _DownTex_HDR;
		half4 frag(v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex_HDR); }
		ENDCG
	}
	}
}
