Shader "TWD FX/FX Deform Cloth" {
	Properties {
		_Direction ("Displacement Direction", Vector) = (0,1,0,0)
		_Amount ("Displacement Amount", Float) = 0.5
		_WaveScale ("Wave Scale", Float) = 1.5
		_WaveSpeed ("Wave Speed", Float) = 1.5
		_Color ("Base Color", Vector) = (1,1,1,1)
		_MainTex ("Base + Mask (RGBA)", 2D) = "white" {}
		_AlphaTex ("Android Alpha (RGB)", 2D) = "white" {}
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
}