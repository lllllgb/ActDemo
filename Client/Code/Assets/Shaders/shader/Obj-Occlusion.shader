Shader "Shader/Obj-Occlusion" 
{
    Properties
    {
		_OccColor("Occlusion Color", Color) = (0, 1, 1, 1)
    }

    SubShader
    {
		Tags
		{ 
			"Queue"="Geometry" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}

		LOD 100

		Pass
		{
			Cull Front
			Lighting Off
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 _OccColor;

			fixed4 frag(v2f i) : SV_Target
			{
				return _OccColor;
			}
			ENDCG
		}
    }
}