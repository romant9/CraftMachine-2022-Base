Shader "TWD/FX/UI/AnimatedSpriteOutlineMask 1" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_MaskTexture1 ("_MaskTexture1", 2D) = "white" {}
		_MaskTexture2 ("_MaskTexture2", 2D) = "white" {}
		_MaskUVScale1 ("_MaskUVScale1", Range(0, 2)) = 0
		_MaskUVScale2 ("_MaskUVScale2", Range(0, 2)) = 0
		_OutlineColor1 ("_OutlineColor1", Vector) = (1,1,1,1)
		_OutlineColor2 ("_OutlineColor2", Vector) = (1,1,1,1)
		_OutlineWidth1 ("_OutlineWidth1", Range(0, 1)) = 0
		_OutlineWidth2 ("_OutlineWidth2", Range(0, 1)) = 0
		_OutlineWeight1 ("_OutlineWeight1", Range(0, 4)) = 0
		_OutlineWeight2 ("_OutlineWeight2", Range(0, 4)) = 0
		_OutlineFlowSpeed1 ("_OutlineFlowSpeed1", Range(0, 3)) = 0
		_OutlineFlowSpeed2 ("_OutlineFlowSpeed2", Range(0, 3)) = 0
		_OutlineAccuracy ("_OutlineAccuracy", Range(1, 16)) = 8
		_OutlineMaskAlpha ("_OutlineMaskAlpha", Range(0, 1)) = 0
		_BloomFallOff ("_BloomFallOff", Range(0, 1)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}