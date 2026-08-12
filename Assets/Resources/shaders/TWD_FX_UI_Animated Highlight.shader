Shader "TWD/FX/UI/Animated Highlight" {
	Properties {
		_BaseColor ("Base Color", Vector) = (0.5,0.5,0.5,1)
		_HiColor ("Hot Color", Vector) = (0.5,0.5,0.5,1)
		_Colorize ("Colorize", Vector) = (1,1,1,1)
		_ColorIntensity ("Color Intensity", Float) = 1
		_Intensity ("Intensity", Float) = 1
		_FlareTex ("Flare Texture", 2D) = "white" {}
		_FlareIntensity ("Flare Intensity", Float) = 1
		_VflareSpeed ("Flare V Speed", Float) = 1
		_UflareScale ("Flare U Scale", Float) = 1
		_VflareScale ("Flare V Scale", Float) = 1
		_MainTex ("Main Texture", 2D) = "black" {}
		_Seed ("_Seed", Float) = 0
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