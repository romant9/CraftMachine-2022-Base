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

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, float2(input.uv.x, input.uv.y));
			}

			ENDHLSL
		}
	}
}