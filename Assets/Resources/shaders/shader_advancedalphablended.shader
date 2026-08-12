Shader "ParticleEffect_Shader/(Shader) Advanced Alpha Blended" {
	Properties {
		[HDR] _TintColor ("For Red Color", Vector) = (0.5,0.5,0.5,0.5)
		_TintColorFactor ("Red Color Factor", Float) = 1
		[HDR] _TintColor2 ("For Green Color", Vector) = (1,1,1,1)
		_TintColorFactor2 ("Green Color2 Factor", Float) = 1
		[HDR] _TintColor3 ("For Blue Color", Vector) = (1,1,1,1)
		_TintColorFactor3 ("Blue Color3 Factor", Float) = 1
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
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