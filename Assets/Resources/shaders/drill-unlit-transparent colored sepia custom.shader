Shader "Drill/Transparent Colored Sepia Custom" {
	Properties {
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_ColorMul ("Sepia Color White (mul)", Vector) = (0.8,0.75,0.55,1)
		_ColorAdd ("Sepia Color Black (add)", Vector) = (0.3,0.2,0.18,1)
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