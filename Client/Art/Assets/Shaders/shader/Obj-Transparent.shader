
Shader "Shader/Obj-Transparent" 
{
	Properties	
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaFactor ("Alpha Factor", Range(0.0, 1.0)) = 1.0
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

		Cull Back
		Lighting Off
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

		Pass 
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.texcoord = IN.texcoord;
				return o;
			}

			sampler2D _MainTex;
			float _AlphaFactor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, IN.texcoord);
				color.a *= _AlphaFactor;
				return color;
			}
		ENDCG
		}
	}
	Fallback "Shader/Obj-Texture"
}