Shader "Unlit/FX Jumping Indicator" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_SinScale ("JumpSpeed/JumpOffset/AlphaSpeed/AlphaOffset", Vector) = (4,0,4,0)
		_Jump ("Jump Scale/Offset/Power/Amount", Vector) = (0.5,0.5,2,1)
		_Alpha ("Alpha Scale/Offset/Power/Amount", Vector) = (0.5,0.5,2,1)
		//[Toggle]_ColorToOpacity("Color Mult Opacity", Float) = 0

	}
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			//uniform float _ColorToOpacity;
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f IN) : SV_Target
			{
				//half4 col = (_ColorToOpacity ? fixed4(1,1,1,1) : tex2D(_MainTex, IN.texcoord)) * IN.color * _Color;
								half4 col = tex2D(_MainTex, IN.texcoord) * IN.color * _Color;

				//half4 col = IN.color * _Color;
				//col.a = tex2D(_MainTex, IN.texcoord).a * tex2D(_MainTex, IN.texcoord1).r;
				//col.a = tex2D(_MainTex, IN.texcoord1).a * (_ColorToOpacity ? tex2D(_MainTex, IN.texcoord1).r : 1) * _Color.a;// * (col.r + col.b + col.b)/3;
				col.a = 1;
				//return fixed4(col.rgb * _Color.rgb, col.a);
				return col;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}