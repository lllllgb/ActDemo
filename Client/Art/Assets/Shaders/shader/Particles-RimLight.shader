// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Shader/Particles-RimLight" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0.0, 0.0, 0.0, 0.0)
		_RimPower("Rim Power", float) = 3.0
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
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldView : TEXCOORD2;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldView = normalize(UnityWorldSpaceViewDir(worldPos));

                return o;
            }
            
            sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _RimColor;
			fixed _RimPower;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 worldView = normalize(i.worldView);

				fixed3 lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
				fixed3 diffuse = lambert * _LightColor0.xyz;
				
				float rim = 1 - max(0, dot(worldView, worldNormal));
				fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower);

				fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				col.rgb = col.rgb * diffuse + rimColor;
                return col;
            }
            ENDCG
        }
    }
	
	Fallback "Shader/Obj-Default"
}