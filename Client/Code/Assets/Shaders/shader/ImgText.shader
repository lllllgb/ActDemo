
Shader "Shader/ImgText" 
{
	Properties	
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags
		{ 
			"Queue"="Transparent+500" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
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
				float4 color : COLOR0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				fixed4 color : COLOR0;
			};

			v2f vert(appdata_t IN)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.texcoord = IN.texcoord;
				o.color = IN.color;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				return tex2D(_MainTex, IN.texcoord) * IN.color;
			}
		ENDCG
		}
	}
	Fallback "Shader/Obj-Texture"
}