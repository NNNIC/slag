Shader "Sample/AlphaTex" {
	Properties {
		_MainTex ("Base (RGB)", 2D)   = "white" {}
		_Alpha1 ("Alpha1",Range(0,1)) = 1
	}
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Ztest Off
		Fog { Color (0,0,0,0) } 
	 
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
	
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					constantColor(0,0,0,[_Alpha1])
					combine texture * primary, texture * constant
				}
			}
		}
	}
}
