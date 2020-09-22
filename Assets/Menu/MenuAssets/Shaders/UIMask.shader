Shader "IndieYP/UIMask" 
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0
         
         // required for UI.Mask
         _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
         _ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags{
			"Queue" = "Transparent+1"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}
         
		// required for UI.Mask
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		ColorMask [_ColorMask]

		Offset -1, -1
		Zwrite On
		Pass
		{
			AlphaTest Greater[_Cutoff]
			Blend SrcAlpha SrcAlpha
			SetTexture[_MainTex] {
				combine texture * primary, texture
			}
		}
	}
}
