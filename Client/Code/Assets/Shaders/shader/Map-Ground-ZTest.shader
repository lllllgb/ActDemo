Shader "Shader/Map-Ground-ZTest"
{
	Properties
	{
		_Color("Color", Color) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags{ "Queue" = "Background+500" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		LOD 100
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}
