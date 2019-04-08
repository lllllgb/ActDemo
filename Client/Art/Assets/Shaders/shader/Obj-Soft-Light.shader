Shader "Shader/Obj-Soft-Light"

{
	//属性
	Properties{
	    _Color ("Color Tint", Color) = (0, 0, 0, 1)
		_MainTex("Base 2D", 2D) = "white"{}
		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0.0, 1.0)) = 1.0
		[Toggle]_RimToggle("RimToggle", Range(0.0, 1.0)) = 1.0
		_LightDir("Light Dir", Vector) = (-0.08, 0.5, 1, 0)
		_LightColor("Light Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_LightIntensity("LightIntensity", Range(0.0, 1.0)) = 0.5

	}
 
	
	SubShader
	{
		Pass
		{    
	
		Tags
		{ 
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque" 
		}
             

			CGPROGRAM
	
			#include "Lighting.cginc"
		    #pragma fragmentoption ARB_precision_hint_fastest
			//#pragma shader_feature RIMPOWER_OFF //RIMPOWER_ON 
	       #pragma shader_feature RIMPOWER_OFF
			
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _RimColor;
			float _RimPower;
			float _RimToggle;
	        fixed4 _Color;
            float4 _LightDir;
			float  _LightIntensity;
			float4 _LightColor;  
	
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 uv : TEXCOORD1;
			    float3 lightir : TEXCOORD3;
				float3 worldViewDir : TEXCOORD2;
			};
 
		
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
			
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				o.worldViewDir = _WorldSpaceCameraPos.xyz - worldPos;
			
				return o;
			}
 
		
			fixed4 frag(v2f i) : SV_Target
			{
												
				fixed3 worldNormal = normalize(i.worldNormal);
		       
			    float3 worldViewDir = normalize(i.worldViewDir);
				
			    float3 lightDir = normalize(_LightDir.xyz);
			
			    fixed3 halfLambert  = _LightColor.xyz * max(0.0, dot(worldNormal, lightDir)*0.5+0.5) * _LightIntensity ;
							
				fixed4 texColor = tex2D(_MainTex, i.uv)*_Color;
								
		        float rim = 1 - max(0, dot(worldViewDir, worldNormal));

				fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower);
			  

				fixed3 finalColor =texColor+halfLambert;
				
				if(_RimToggle > 0.5)
				{
					return fixed4(finalColor+rimColor, 1);
				}
				else
				{
					return fixed4(finalColor, 1);
				}
			}
 
			
			#pragma vertex vert
			#pragma fragment frag	
 
			ENDCG
		}
	}
	
	FallBack "Diffuse"
	
}