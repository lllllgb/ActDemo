Shader "Shader/Particles-UnlitTexture-ClipY" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
	}

	Category 
	{
		Tags { "Queue"="Transparent" "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		Lighting Off 
		Fog { Mode Off }
	
		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _TintColor;
			
				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};


				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					return o;
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
					return i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				}
				ENDCG 
			}
		}

		// ---- Dual texture cards
		SubShader 
		{
			Pass 
			{
				SetTexture [_MainTex] 
				{
					constantColor [_TintColor]
					combine constant * primary
				}
				SetTexture [_MainTex] 
				{
					combine texture * previous DOUBLE
				}
			}
		}
	
		// ---- Single texture cards (does not do color tint)
		SubShader 
		{
			Pass 
			{
				SetTexture [_MainTex] 
				{
					combine texture * primary
				}
			}
		}	
	}
}
