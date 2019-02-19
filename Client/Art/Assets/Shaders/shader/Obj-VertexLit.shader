Shader "Shader/Obj-VertexLit" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
		Tags
		{ 
			"Queue"="Geometry" 
			"IgnoreProjector"="True" 
			"RenderType"="Opaque"
		}
		LOD 200

		Cull Back
		Lighting On
		ZWrite On
		ZTest LEqual
		Blend Off
		
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				UNITY_FOG_COORDS(2)
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 lambert = 0.5 * max(0.0, dot(worldNormal, worldLightDir)) + 0.5;
				fixed3 diffuse = lambert * _LightColor0.xyz;

				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb = col.rgb * diffuse;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
            }
            ENDCG
        }
    }
	
	Fallback "Shader/Obj-Texture"
}