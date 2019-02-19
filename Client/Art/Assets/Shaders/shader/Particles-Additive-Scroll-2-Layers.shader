Shader "Shader/Particles-Additive-Scroll-2-Layers" 
{
	Properties 
	{
		_MainTex ("Base layer (RGB)", 2D) = "white" {}
		_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX ("1nd layer Scroll speed X", Float) = 1.0
		_ScrollY ("1nd layer Scroll speed Y", Float) = 0.0
		_Scroll2X ("2nd layer Scroll speed X", Float) = 1.0
		_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
		_Color("Color", Color) = (1,1,1,1)
		_MMultiplier ("Layer Multiplier", Float) = 1.0
	}

	SubShader 
	{
		Tags 
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		LOD 100

		Blend SrcAlpha One
		Cull Off 
		Lighting Off 
		ZWrite Off
	
		CGINCLUDE

		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _DetailTex;

		float4 _MainTex_ST;
		float4 _DetailTex_ST;
	
		float _ScrollX;
		float _ScrollY;
		float _Scroll2X;
		float _Scroll2Y;
		float _MMultiplier;
		float4 _Color;

		struct v2f {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			fixed4 color : TEXCOORD1;
		};

		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
			o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
			o.color = _MMultiplier * _Color * v.color;
			return o;
		}
		ENDCG


		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 tex = tex2D (_MainTex, i.uv.xy);
				fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
				return tex * tex2 * i.color;
			}
			ENDCG 
		}	
	}
}
