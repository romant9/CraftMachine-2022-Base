Shader "TWD/FX/UI/Radial Progress bar" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_Radius ("Radius", Range(0, 1)) = 0.3
		_Arcrange ("Arc range", Range(0, 360)) = 360
		_Fillpercentage ("Fill percentage", Range(0, 1)) = 0.25
		_Globalopacity ("Global opacity", Range(0, 1)) = 1
		_Barmincolor ("Bar min color", Vector) = (1,0,0,1)
		_Barmaxcolor ("Bar max color", Vector) = (0,1,0.08965516,1)
		_Rotation ("Rotation", Range(0, 360)) = 0
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