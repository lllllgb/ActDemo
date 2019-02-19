Shader "Shader/Obj-Soft-Specular"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_LightDir("Light Dir", Vector) = (0, 0, 1, 0)
		_SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8, 256)) = 20
	}
	SubShader
	{
		Tags{ 
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata
			{
				fixed3 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed3 uv : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 worldNormal : TEXCOORD1;
				fixed3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _LightDir;
			fixed4 _SpecularColor;
			fixed _Gloss;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 worldLightDir = normalize(i.worldPos - _LightDir);
				fixed3 reflectDir = normalize(reflect(-worldLightDir, i.worldNormal));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));
				fixed3 specular = _SpecularColor.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);
				col.rgb += specular;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Shader/Obj-Texture"
}
