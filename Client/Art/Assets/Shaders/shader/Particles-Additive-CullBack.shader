Shader "Shader/Particles-Additive-CullBack"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}

	Category
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		LOD 100

		Blend SrcAlpha One
		ColorMask RGB
		Cull Back
		Lighting Off
		ZWrite Off

		SubShader
	{
		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_particles

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _TintColor;

		struct appdata_t
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		float4 _MainTex_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}

		sampler2D_float _CameraDepthTexture;
		float _InvFade;

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
			return col;
		}
		ENDCG
	}
	}
	}
}
