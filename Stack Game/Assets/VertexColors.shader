Shader "Custom/VertexColors" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Pass{
			Lighting On
			ColorMaterial AmbientAndDiffuse
			SetTexture [_Maintex]{
				combine texture * primary DOUBLE
			}
		}

	}
}