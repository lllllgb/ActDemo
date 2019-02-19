Shader "Shader/PostEffect-Boom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Radio ("Radio", Range(0, 1)) = 0.5
	}
	SubShader
	{
		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Radio;

			fixed4 frag (v2f i) : COLOR
			{
				float v_half_w = 0.08;
				float v_half_w_1 = 12.5;
				float v_offset = 0.1;
				fixed2 v_center = fixed2(0.5, 0.5);
				float dis = distance(i.uv, v_center);
				float mask = step(_Radio - v_half_w, dis) * step(dis, v_half_w + _Radio);
				mask = mask * (1.0 - abs(dis - _Radio) * v_half_w_1);
				return tex2D(_MainTex, (i.uv + mask * v_offset * (i.uv - v_center)));
			}
			ENDCG
		}
	}
}
