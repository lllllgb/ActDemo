Shader "Shader/PostEffect-RadialBlur"
{
	Properties
	{
		_MainTex ("Texture", RECT) = "white" {}
		_BlurStrength("Blur Strength", Range(0, 1)) = 0.5
		_BlurWidth("Blur Width", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Pass
		{
			Cull Off
			ZWrite Off
			ZTest Always
			Fog{ Mode Off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed _BlurStrength;
			fixed _BlurWidth;

			fixed4 frag (v2f_img i) : COLOR
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				fixed samples[10];
				samples[0] = -0.08;
				samples[1] = -0.05;
				samples[2] = -0.03;
				samples[3] = -0.02;
				samples[4] = -0.01;
				samples[5] = 0.01;
				samples[6] = 0.02;
				samples[7] = 0.03;
				samples[8] = 0.05;
				samples[9] = 0.08;

				fixed2 dir = 0.5 - i.uv;
				fixed dist = sqrt(dir.x * dir.x + dir.y * dir.y);
				dir = dir / dist;
				fixed4 sum = color;
				for (int index = 0; index < 10; index++)
				{
					sum += tex2D(_MainTex, i.uv + dir * samples[index] * _BlurWidth);
				}
				sum *= 1.0 / 11.0;
				fixed t = saturate(dist * _BlurStrength);
				return lerp(color, sum, t);
			}
			ENDCG
		}
	}
}
