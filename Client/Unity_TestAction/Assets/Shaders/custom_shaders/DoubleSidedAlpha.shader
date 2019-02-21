Shader "GOD/DoubleSidedAlpha" {
	Properties
	{
		_Color ("Main Color",Color) = (0,0,0,1)
		_MainTex ("Base (RGB)",2D) = ""{}
		_Emission ("Emmisive Color", Color) = (0,0,0,0)
		_AlphaCutoff ("AlphaCutoff", Range (0, 1)) = 0.5
	}
	
	SubShader {
		Tags {"RenderType"="Opaque" "Queue"="Geometry+501"}
		LOD 100
		Pass {
			Material {
				Diffuse [_Color]
				Ambient [_Color]
				Emission [_Emission]
			}
			Lighting On
			Cull Off
			AlphaTest Greater [_AlphaCutoff]
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * primary + constant, texture * constant
			}
		}
	}
	
	Fallback "VertexLit"
}
