Shader "Drill/Card Teaser Dual (SoftClip)" {
	Properties {
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_AlphaTex ("Extra transparency (R)", 2D) = "black" {}
		_Color ("Main Color", Vector) = (0.3,0.12,0.01,1)
		_Cutout_Low ("", Range(0, 1)) = 0.2
		_Cutout_High ("", Range(0, 1)) = 0.4
		_Smoothness ("", Range(0, 1)) = 0.4
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
	//CustomEditor "DualAlphaMaterialInspector"
}