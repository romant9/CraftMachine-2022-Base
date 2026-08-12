Shader "Shader Forge/FX_Fire Dual" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_AlphaTex ("Inverse Alpha", 2D) = "white" {}
		_FlowMap ("FlowMap", 2D) = "white" {}
		_Flow_strengh ("Flow_strengh", Float) = 0.2
		_U_FlowSpeed ("U_FlowSpeed", Float) = 0.5
		_V_FlowSpeed ("V_FlowSpeed", Float) = 0.5
		_EmissiveStrenght ("EmissiveStrenght", Float) = 1.5
		_Mask ("Mask", 2D) = "white" {}
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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
	//CustomEditor "DualAlphaMaterialInspector"
}