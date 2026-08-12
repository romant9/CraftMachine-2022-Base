Shader "TWD/FX/UI/UIRays" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[NoScaleOffset] _Texture ("Texture", 2D) = "white" {}
		_RadialTiling ("Radial Tiling", Range(0, 10)) = 1
		_RotationSpeed ("Rotation Speed", Range(-1, 1)) = 0
		_HorizontalScale ("Horizontal Scale", Range(0, 2)) = 1
		_VerticalScale ("Vertical Scale", Range(0, 2)) = 1
		_HorizontalOffset ("Horizontal Offset", Range(-1, 1)) = 0
		_VerticalOffset ("Vertical Offset", Range(-1, 1)) = 0
		_Intensity ("Intensity", Range(0, 5)) = 1
		_Falloff ("Falloff", Range(0, 5)) = 3
		_InsideColor ("Inside Color", Vector) = (0,0,0,0)
		_OutsideColor ("Outside Color", Vector) = (0,0,0,0)
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