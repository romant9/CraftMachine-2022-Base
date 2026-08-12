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
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	//CustomEditor "ASEMaterialInspector"
}