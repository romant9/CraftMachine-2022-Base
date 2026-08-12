Shader "TWD/FX/Unlit/Fog" {
	Properties {
		[HDR] _Albedo ("Albedo", Vector) = (1,1,1,1)
		_SimpleNoiseScale ("Simple Noise Scale", Float) = 20
		_SimplexNoiseScale ("Simplex Noise Scale", Float) = 4
		_VoronoiScale ("Voronoi Scale", Float) = 5
		_SimpleNoiseAnimation ("Simple Noise Animation", Vector) = (0,0,0,0)
		_SimplexNoiseAnimation ("Simplex Noise Animation", Vector) = (0,0,0.02,0)
		_VoronoiNoiseAnimation ("Voronoi Noise Animation", Vector) = (0,0,0,0)
		_SimpleNoiseAmount ("Simple Noise Amount", Range(0, 1)) = 0.25
		_SimplexNoiseAmount ("Simplex Noise Amount", Range(0, 1)) = 0.25
		_VoronoiNoiseAmount ("Voronoi Noise Amount", Range(0, 1)) = 0.5
		_SimpleNoiseRemap ("Simple Noise Remap", Range(0, 1)) = 0
		_SimplexNoiseRemap ("Simplex Noise Remap", Range(0, 1)) = 0
		_VoronoiNoiseRemap ("Voronoi Noise Remap", Range(0, 1)) = 0
		_CombinedNoiseRemap ("Combined Noise Remap", Range(0, 1)) = 0
		_SurfaceDepthFade ("Surface Depth Fade", Float) = 0
		_CameraDepthFadeRange ("Camera Depth Fade Range", Float) = 0
		_CameraDepthFadeOffset ("Camera Depth Fade Offset", Float) = 0
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