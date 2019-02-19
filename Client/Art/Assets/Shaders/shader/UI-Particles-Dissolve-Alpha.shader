// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Shader/UI-Particles-Dissolve-Alpha" {
    Properties {
		_MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Texture_Strength ("Texture_Strength", Range(-10, 10)) = 1

        _Dissolve ("Dissolve", Range(0, 1)) = 0.4994549
        _Dis_Color ("Dis_Color", Color) = (1,1,1,1)
        _Dis_Color2 ("Dis_Color2", Color) = (1,1,1,1)
        _Dis_Side ("Dis_Side", Range(0, 1)) = 0
        _Dis_Side2 ("Dis_Side2", Range(-1, 0)) = 0
        _Dis_Strength ("Dis_Strength", Range(0, 2)) = 2
        _Dis_Strength2 ("Dis_Strength2", Range(0, 10)) = 10
        _Dis_Texture ("Dis_Texture", 2D) = "white" {}

		_Distortion("Distortion", Range(0, 2)) = 0
		_DistortionX("DistortionX", Range(0, 1)) = 0.2
		_DistortionY("DistortionY", Range(0, 1)) = 0.2

		// UI Mask
		_MinX("Min X", Float) = -10
		_MaxX("Max X", Float) = 10
		_MinY("Min Y", Float) = -10
		_MaxY("Max Y", Float) = 10
    }
    SubShader 
	{
		Tags 
		{
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
			"RenderType"="Transparent"
		}
		Cull Off
        
		Pass 
		{
			Tags 
			{
				"LightMode"="ForwardBase"
            }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase

			uniform sampler2D _MainTex;
			uniform float4 _Color;
			uniform float _Texture_Strength;

			uniform float _Dissolve;
			uniform sampler2D _Dis_Texture; 
			uniform float _Dis_Side;
			uniform float _Dis_Side2;
			uniform float4 _Dis_Color;
			uniform float4 _Dis_Color2;
			uniform float _Dis_Strength;
			uniform float _Dis_Strength2;

			uniform float _Distortion;
			uniform float _DistortionX;
			uniform float _DistortionY;
			
			// UI Mask
			float _MinX;
			float _MaxX;
			float _MinY;
			float _MaxY;

			float4 _MainTex_ST;
			float4 _Dis_Texture_ST;

            struct VertexInput 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord0 : TEXCOORD0;
				float4 vertexColor : COLOR;
            };
			
			struct VertexOutput 
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float3 vpos : TEXCOORD2;
				float4 vertexColor : COLOR;
            };
			
			VertexOutput vert (VertexInput v) 
			{
				VertexOutput o;
				o.uv0 = v.texcoord0;
				o.vertexColor = v.vertexColor;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

            float4 frag(VertexOutput i) : COLOR 
			{
				i.normalDir = normalize(i.normalDir);

				float2 distortUV = (i.uv0 + float2(_DistortionX * _Time.y, _DistortionY * _Time.y) * _Distortion);
				float4 mainTexColor = tex2D(_MainTex, TRANSFORM_TEX(distortUV, _MainTex));
				float3 mainColor = mainTexColor.rgb * _Color.rgb * i.vertexColor.rgb * _Texture_Strength;

				float4 disTexColor = tex2D(_Dis_Texture, TRANSFORM_TEX(distortUV, _Dis_Texture));
				float disValue = _Dissolve + _Dissolve * disTexColor.r * 4.0;
				float disPowValue = pow(disValue, 80.0);
				clip((1.0 - disPowValue) - 0.5);

				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseColor = mainColor * max(0, dot(i.normalDir, lightDirection)) * _LightColor0.rgb;

				float node_9302 = (1.0 - _Dis_Side);
				float node_6725_if_leA = step(node_9302, disValue);
				float node_6725_if_leB = step(disValue, node_9302);
				float node_6725 = lerp(node_6725_if_leB, 1.0, node_6725_if_leA * node_6725_if_leB);
				float node_2110_if_leA = step(_Dis_Side2 * 0.2 + node_9302, disValue);
				float node_2110_if_leB = step(disValue, _Dis_Side2 * 0.2 + node_9302);
				float3 emissive = lerp(mainColor, lerp(_Dis_Color.rgb, float3(0.0, 0.0, 0.0), disPowValue), (_Dis_Strength * (1.0 - node_6725))) 
						+ (((node_6725 - lerp(node_2110_if_leB, 1.0, node_2110_if_leA * node_2110_if_leB)) * _Dis_Strength2) * _Dis_Color2.rgb);
				float3 finalColor = emissive + diffuseColor;
				
                fixed4 finalRGBA = fixed4(lerp(0, finalColor,(mainTexColor.a * i.vertexColor.a)),1) * mainTexColor.a;
				finalRGBA.a *= ((i.vpos.x >= _MinX) * (i.vpos.x <= _MaxX) * (i.vpos.y >= _MinY) * (i.vpos.y <= _MaxY));
				return finalRGBA;
			}
            ENDCG
        }
    }
}
