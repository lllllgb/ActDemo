Shader "Shader/Map-Transparent" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "black" {}
	}

		SubShader{
			Tags {"Queue" = "Background" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			LOD 100

			ZWrite Off
			Blend One Zero

			Pass {
				Lighting Off
				SetTexture[_MainTex] { combine texture }
			}
	}
}