Shader "TWD/Reflective_Water_FOAM Dual" {
	Properties {
		_Normal1 ("Normal 1", 2D) = "bump" {}
		_WaterColor ("WaterColor", Vector) = (0.5060554,0.5882353,0.567832,1)
		_SpecularColor ("Specular Color", Vector) = (1,1,1,1)
		_Normal2 ("Normal 2", 2D) = "bump" {}
		_Cubemapreflection ("Cubemap reflection", Cube) = "_Skybox" {}
		_N1Uspeed ("N1 U speed", Float) = 0.01
		_N1Vspeed ("N1 V speed", Float) = 0.01
		_N2Uspeed ("N2 U speed", Float) = 0.01
		_N2Vspeed ("N2 V speed", Float) = 0.01
		_FresnelExp ("Fresnel Exp", Range(0, 0.9)) = 0.3654139
		_Gloss ("Gloss", Range(0, 1)) = 0.7368426
		_ReflectionPower ("Reflection Power", Range(0, 20)) = 1.747542
		_Foamnormal ("Foam normal", 2D) = "bump" {}
		_Foamtexturea ("Foam texture (RGBA)", 2D) = "white" {}
		_AlphaTex ("Foam Extra Transparency (R)", 2D) = "black" {}
		_ReflectionStrength ("Reflection Strength", Range(0, 2)) = 0
		_AmbientLight ("Ambient Light", Float) = 0.5
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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
	Fallback "TWD/Diffuse Tint"
	//CustomEditor "DualAlphaMaterialInspector"
}