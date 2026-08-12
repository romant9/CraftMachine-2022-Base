Shader "TWD/FX/Animated-Mask" {
	Properties {
		_Albedo ("Albedo", 2D) = "white" {}
		_MatCap ("MatCap", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		_TileableMask ("TileableMask", 2D) = "white" {}
		_Intensity ("Intensity", Range(0, 10)) = 0
		_BlendFactor ("BlendFactor", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	//CustomEditor "ASEMaterialInspector"
}