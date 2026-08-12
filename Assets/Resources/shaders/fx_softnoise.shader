Shader "TWD/FX/Noise/SoftNoise" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_Noise ("Noise", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW ("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Noisescale ("Noise scale", Float) = 1000
		_Noisepower ("Noise power", Float) = 1
		_Noiselerp ("Noise lerp", Float) = 1
		_Color ("Color", Vector) = (1,1,1,1)
		_Emissionpower ("Emission power", Float) = 1
		_Emission ("Emission", Float) = 2
		_OpacityTex ("OpacityTex", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		_Maskpower ("Mask power", Float) = 1
		_Maskmultiplayer ("Mask multiplayer", Float) = 3
		[Toggle] _Softedges ("Soft edges", Float) = 0
		[Toggle] _Usedepth ("Use depth", Float) = 1
		_Depthpower ("Depth power", Float) = 1
		_OpacityTexspeedXY ("OpacityTex speed XY", Vector) = (0,-0.5,0,0)
		_Sideopacitymult ("Side opacity mult", Float) = 1.5
		[Toggle] _Upopacity ("Up opacity", Float) = 1
		[Enum(Cull Off,0,Cull Front,1,Cull Back,2)] _CullMode2 ("Culling", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
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
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}