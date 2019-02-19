// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Shader/Realtime-Shadow"
{
	Properties
	{
		_Color ("Color", Color) = (0.3, 0.3, 0.3, 1)
	}
	SubShader
	{
		Tags { 
			"Queue" = "Geometry"
			"RenderType"="Opaque" 
		}
		LOD 100

		Pass
		{
			Cull Front
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
			Stencil{
				Ref 1
				Comp Greater
				Pass replace
				Fail Keep
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			fixed4 _Color;
			
			fixed4 vert (float4 vertex:POSITION) : SV_POSITION
			{
				fixed4 worldPos = mul(unity_ObjectToWorld, vertex);
				fixed3 lightDir = normalize(WorldSpaceLightDir(worldPos));
				worldPos.xz = worldPos.xz - (worldPos.y / lightDir.y) * lightDir.xz;
				worldPos.y = 0;
				return mul(UNITY_MATRIX_VP, worldPos);
			}
			
			fixed4 frag () : COLOR
			{
				return _Color;
			}
			ENDCG
		}
	}
}
