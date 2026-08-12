Shader "TWD/FX/UI/SineGlow" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_GlowSpeed ("Glow Speed", Float) = 1
		_SineGlowFade ("Sine Glow: Fade", Range(0, 1)) = 1
		_SineGlowShaderMask ("Sine Glow: Shader Mask", 2D) = "white" {}
		_SineGlowColor ("Sine Glow: Color", Vector) = (0,0.556706,0.8307096,0)
		_SineGlowContrast ("Sine Glow: Contrast", Float) = 1
		_SineGlowFrequency ("Sine Glow: Frequency", Float) = 4
		_SineGlowMin ("Sine Glow: Min", Float) = 0
		_SineGlowMax ("Sine Glow: Max", Float) = 1
		[HideInInspector] _texcoord ("", 2D) = "white" {}
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
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, float2(input.uv.x, input.uv.y)) * _Color;
			}

			ENDHLSL
		}
	}
	//CustomEditor "ASEMaterialInspector"
}