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