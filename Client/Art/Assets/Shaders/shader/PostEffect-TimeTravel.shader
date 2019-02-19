Shader "Shader/PostEffect-TimeTravel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DistortFactor("Distort Factor", Range(0, 1)) = 0
		_DistortCenter("Distort Center", Vector) = (0.5, 0.5, 0, 0)
	}
	SubShader
	{
		Cull Off 
		ZWrite Off 
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float _DistortFactor;
			float4 _DistortCenter;

			fixed4 frag (v2f_img i) : COLOR
			{
				fixed2 dir = i.uv - _DistortCenter.xy;
				return tex2D(_MainTex, i.uv + _DistortFactor * normalize(dir) * (1 - length(dir)));
			}
			ENDCG
		}
	}
}
