// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader/Particles-Distortion-Additive" {
    Properties {
        _Main_Texture ("Main_Texture", 2D) = "white" {}
		_Tint("Tint_Color", Color) = (1,1,1,1)
        _Distortion_Tex_01 ("Distortion_Tex_01", 2D) = "black" {}
        _ColorDistort_Inten ("Color&Distort_Inten", Vector) = (1.5,3,0.1,0.2)
        _Mask_Texture ("Mask_Texture", 2D) = "white" {}
        _Distortion_U_Speed ("Distortion_U_Speed", Float ) = 0
        _Distortion_V_Speed ("Distortion_V_Speed", Float ) = 0

		//Beam
		[Enum(Off,0,On,1)] _Beam_Mode ("Beam Mode", Float) = 0
		_Planner_G_Speed ("Planner_Mask_G", Range(-2,2)) = 0
		_Planner_B_Speed ("Planner_Mask_B", Range(-2,2)) = 0
		_Beam_edge_Alpha ("Beam Edge Alpha", Range(0,4)) = 0

		//FallOff
		[Enum(Off,0,On,1)] _FallOff_Mode("FallOff Mode", Float) = 0
		_FallOff ("FallOff", Range(0,3)) = 0

		[MaterialToggle] _tex_bleach("_tex_bleach", Float) = 0.3215686
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		[Enum(Off,0,Front,1,Back,2)]_Cull("Cull Mode", Float) = 2
    }
    SubShader {
        Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha One
            ZWrite Off
			Cull [_Cull]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
			#pragma multi_compile_particles

			#pragma shader_feature _BEAM
			#pragma shader_feature _FALLOFF
			#pragma target 2.0
			fixed _tex_bleach;
            sampler2D _Main_Texture; half4 _Main_Texture_ST;
            sampler2D _Distortion_Tex_01; half4 _Distortion_Tex_01_ST;
            half4 _ColorDistort_Inten;
            sampler2D _Mask_Texture; half4 _Mask_Texture_ST;
            half _Distortion_U_Speed;
            half _Distortion_V_Speed;
			half4 _Tint;

			//Beam
			#if _BEAM
			half _Planner_G_Speed;
			half _Planner_B_Speed;
			half _Beam_edge_Alpha;
			#endif

			#if _FALLOFF
			half _FallOff;
			#endif

            struct VertexInput {
                half4 vertex : POSITION;
				fixed4 color : COLOR;
				//lack of precision
                float2 texcoord0 : TEXCOORD0;
				half3 normal : NORMAL;
            };
            struct VertexOutput {
                half4 pos : SV_POSITION;
				fixed4 color : COLOR;
				//lack of precision
                float2 uv0 : TEXCOORD0;
				#if _BEAM
				half3 normalDir : TEXCOORD2;
				half3 eyeVec : TEXCOORD3;
				#endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				o.color = half4(v.color.xyz * _Tint.xyz * _ColorDistort_Inten.r, v.color.a * _ColorDistort_Inten.g * _Tint.a);
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
				#if _BEAM
				o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
				half3 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.eyeVec = normalize( _WorldSpaceCameraPos - posWorld);
				#endif
                return o;
            }
            half4 frag(VertexOutput i) : SV_Target {
////// Emissive:
				float2 uv = i.uv0 + float2((_Time.g*_Distortion_U_Speed), (_Time.g*_Distortion_V_Speed));
                float4 _Distortion_Tex_01_var = tex2D(_Distortion_Tex_01,TRANSFORM_TEX(uv, _Distortion_Tex_01));
                float2 Distort_uv = ((_Distortion_Tex_01_var.r*_ColorDistort_Inten.b)+i.uv0);
                half4 _Main_Texture_var = tex2D(_Main_Texture,TRANSFORM_TEX(Distort_uv, _Main_Texture));
                half3 finalColor = _Main_Texture_var.rgb;
				finalColor = lerp(finalColor.rgb, dot(finalColor.rgb, half3(0.3, 0.59, 0.11)), _tex_bleach);
                half4 _Mask_Texture_var = tex2D(_Mask_Texture,TRANSFORM_TEX(i.uv0, _Mask_Texture));
				fixed4 finalRGBA = fixed4(finalColor, _Main_Texture_var.a *_Mask_Texture_var.r) * i.color;

				#if _BEAM
				half4 _Mask_Planner_G = tex2D(_Mask_Texture, TRANSFORM_TEX((i.uv0+fixed2(_Planner_G_Speed * _Time.g,0)), _Mask_Texture));
				half4 _Mask_Planner_B = tex2D(_Mask_Texture, TRANSFORM_TEX((i.uv0+fixed2(_Planner_B_Speed * _Time.g,0)), _Mask_Texture));
				finalRGBA.a *= _Mask_Planner_G.g * _Mask_Planner_B.b * pow(saturate(dot(i.normalDir, i.eyeVec)), _Beam_edge_Alpha);
				#endif

				#if _FALLOFF
				finalRGBA.a -= _FallOff;
				#endif
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Shader/Obj-Texture" 
}
