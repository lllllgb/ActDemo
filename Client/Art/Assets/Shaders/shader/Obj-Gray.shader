Shader "Shader/Obj-Gray"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}

		LOD 100

		Pass
		{
			Cull Back
			Lighting Off
			ZWrite On
			ZTest LEqual
			Blend Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed gray = dot(col.rgb, fixed3(0.3, 0.59, 0.11));
				col.rgb = (gray, gray, gray);
				return col;
			}
			ENDCG
		}
	}
	Fallback "Shader/Obj-Texture"
}