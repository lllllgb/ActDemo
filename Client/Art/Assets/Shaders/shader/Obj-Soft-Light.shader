Shader "Shader/Obj-Soft-Light"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Origin("Origin", Range(0.0, 10.0)) = 1.0
		_LightDir("Light Dir", Vector) = (-0.08, 0.5, 1, 0)
		_LightColor("Light Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_LightIntensity("LightIntensity", Range(0.0, 10.0)) = 1.0
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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 diffuse : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _LightDir;
			float _Origin, _LightIntensity;
			float4 _LightColor;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				float3 lightDir = normalize(float3(_LightDir.x, _LightDir.y, _LightDir.z));
				o.diffuse = _LightColor.xyz * max(0.0, dot(worldNormal, lightDir)) * _LightIntensity + float3(_Origin, _Origin, _Origin);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb = col.rgb * i.diffuse;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Shader/Obj-Texture"
}