Shader "Shader/PostEffect-Bloom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BloomTex("Bloom Texture", 2D) = "black" {}
	}
	SubShader
	{
		ZTest Off
		ZWrite Off
		Cull Off
		Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vertBloom
			#pragma fragment fragBloom
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vertMax
			#pragma fragment fragMax
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vertBlur
			#pragma fragment fragBlur
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _BloomTex;
	uniform fixed4 _Parameter;
	uniform half4 _OffsetA;
	uniform half4 _OffsetB;
	#define THRESHHOLD _Parameter.z
	#define ONE_MINUS_THRESHHOLD_TIMES_INTENSITY _Parameter.w

	struct VertexInput
	{
		float4 vertex : POSITION;
		half2 uv : TEXCOORD0;
	};
	struct v2fBloom
	{
		half4 position : SV_POSITION;
		half2 uv : TEXCOORD0;
	};
	struct v2fMax
	{
		half4 position : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 uv1 : TEXCOORD1;
		half2 uv2 : TEXCOORD2;
		half2 uv3 : TEXCOORD3;
		half2 uv4 : TEXCOORD4;
	};
	struct v2fBlur
	{
		half4 position : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 uv1 : TEXCOORD1;
		half2 uv2 : TEXCOORD2;
		half2 uv3 : TEXCOORD3;
	};

	v2fBloom vertBloom(VertexInput i)
	{
		v2fBloom o;
		o.position = UnityObjectToClipPos(i.vertex);
		o.uv = i.uv;
		return o;
	}
	fixed4 fragBloom(v2fBloom i) : COLOR
	{
		return tex2D(_MainTex, i.uv) + tex2D(_BloomTex, i.uv);
	}

	v2fMax vertMax(VertexInput i)
	{
		v2fMax o;
		o.position = UnityObjectToClipPos(i.vertex);
		o.uv = i.uv;
		o.uv1 = i.uv + _OffsetA.xy;
		o.uv2 = i.uv + _OffsetA.zw;
		o.uv3 = i.uv + _OffsetB.xy;
		o.uv4 = i.uv + _OffsetB.zw;
		return o;
	}
	fixed4 fragMax(v2fMax i) : COLOR
	{
		fixed4 color = tex2D(_MainTex, i.uv);
		color = max(color, tex2D(_MainTex, i.uv1));
		color = max(color, tex2D(_MainTex, i.uv2));
		color = max(color, tex2D(_MainTex, i.uv3));
		color = max(color, tex2D(_MainTex, i.uv4));
		return saturate(color - THRESHHOLD) * ONE_MINUS_THRESHHOLD_TIMES_INTENSITY;
	}

	v2fBlur vertBlur(VertexInput i)
	{
		v2fBlur o;
		o.position = UnityObjectToClipPos(i.vertex);
		o.uv = i.uv + _OffsetA.xy;
		o.uv1 = i.uv + _OffsetA.zw;
		o.uv2 = i.uv + _OffsetB.xy;
		o.uv3 = i.uv + _OffsetB.zw;
		return o;
	}
	fixed4 fragBlur(v2fBlur i) : COLOR
	{
		fixed4 color = tex2D(_MainTex, i.uv) + tex2D(_MainTex, i.uv1) + tex2D(_MainTex, i.uv2) + tex2D(_MainTex, i.uv3);
		return color * 0.25;
	}
	ENDCG

	FallBack Off
}
