Shader "Shader/Obj-VertexLit-Specular" 
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_Specular("Specular Tex", 2D) = "white" {}
		_SpecularRange("Specular Range", Range(0.0, 256.0)) = 255
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
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 diffuse : TEXCOORD1;
				float3 specular : TEXCOORD2;
            };

			sampler2D _MainTex;
			fixed4 _Color;
			sampler2D _Specular;
			half _SpecularRange;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

				fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				o.diffuse = (0.5 * max(0.0, dot(worldNormal, worldLightDir)) + 0.5) * _LightColor0.xyz;
                o.specular = _LightColor0.rgb * pow(max(0, dot(worldNormal, halfDir)), _SpecularRange);
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 color = (1.0, 1.0, 1.0, 1.0);
				color.rgb = tex2D(_MainTex, i.uv).rgb * i.diffuse + tex2D(_Specular, i.uv).a * i.specular + _Color.rgb;
				return color;
            }
            ENDCG
        }
    }
	
	Fallback "Shader/Obj-Texture"
}