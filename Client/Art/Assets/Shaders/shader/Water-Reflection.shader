Shader "Shader/Water-Reflection" {
	Properties {
		_Color("Color1", Color) = (0.0824, 0.219, 0.116, 1)
		_RefMap("Reflection Map", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "white" {}
		_RippleAmount("Ripple Amount", Range(0.0, 2.0)) = 0.5
		_Speed("Speed", Vector) = (0.02, 0.02, 0.02, 0.02)
	}

	SubShader 
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		LOD 100
		Cull Back
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass 
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			sampler2D _NormalMap;
			sampler2D _RefMap;
			float4 _RefMap_ST;

			float4 _Color;
			float2 _Speed;
			float _RippleAmount;

			struct appdata 
			{
			   float4 vertex : POSITION0;
			   float3 normal : NORMAL;
			   float2 texcoord : TEXCOORD0;
			   float4 color : COLOR0;
			};
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float2 texcoord : TEXCOORD3;
				fixed4 color : COLOR0;
			};

			float3 CalcNormal(float3 worldNormal, float2 texcoord)
			{
				float2 p1 = texcoord + float2(0.484, 0.867);
				float2 p2 = texcoord + float2(0.685, 0.447);
   
				p1.x -= _Speed.x * _Time.y;
				p2.y += _Speed.y * _Time.y;
   
				p1 *= 2;
				p2 *= 2;

				float3 n1 = tex2D(_NormalMap, p1);
				float3 n2 = tex2D(_NormalMap, p2);
   
				float3 n = n1 + n2;
				n = n * 2 - 2;
				n = lerp(n, float3(0, 1, 0), _RippleAmount);

				return n;
			}

			float3 Reflect(float3 eye, float3 norm)
			{
				return eye - 2 * norm * dot(eye, norm);
			}
 
			v2f vert(appdata v)
			{ 
				v2f Output;
				Output.pos = UnityObjectToClipPos(v.vertex);
				Output.worldPos = mul(unity_ObjectToWorld, v.vertex);
				Output.worldNormal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)));
				Output.viewDir = normalize(Output.worldPos.xyz - _WorldSpaceCameraPos.xyz);
				Output.texcoord = TRANSFORM_TEX(v.texcoord, _RefMap);
				Output.color = v.color;
				return Output;
			} 
 
			fixed4 frag(v2f i) : COLOR
			{
				float3 norm = CalcNormal(i.worldNormal, i.texcoord);
				float3 ref = normalize(Reflect(i.viewDir, norm));
				float2 texCoord = ref.xz * 0.5 + 0.5;
				fixed4 c = tex2D(_RefMap, texCoord) * i.color * _Color;
				return c;
			}
			ENDCG 
		}
	} 
}
