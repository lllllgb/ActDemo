Shader "Shader/MiniMap-Opacity"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "black" {}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Background-1"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}
		LOD 200

		Cull Back
		Lighting Off
		ZWrite Off
		Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
