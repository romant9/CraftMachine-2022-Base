Shader "TWD/FX/UI/RotatingOutline" 
{
	Properties 
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_ColorOverlay ("Color Overlay", Vector) = (1,1,1,1)
		_GlobalAlphaIntensity ("Global Alpha Intensity", Range(0, 5)) = 2
		_GlobalAlphaFalloff ("Global Alpha Falloff", Range(1, 5)) = 1
		_TrailLengthFalloff ("Trail Length Falloff", Range(1, 10)) = 1.5
		_HighLightIntensity ("High Light Intensity", Range(0, 3)) = 0.1
		_HighLightFalloff ("High Light Falloff", Range(1, 5)) = 1
		_TimeSpeed ("Time Speed", Range(0, 5)) = 1
		[IntRange] _TrailNumber ("Trail Number", Range(1, 4)) = 2
		[Toggle(_USECLOCKWISE_ON)] _UseClockwise ("Use Clockwise ?", Float) = 0
		[Toggle(_ENABLESECONDALPHASHADE_ON)] _EnableSecondAlphaShade ("Enable Second AlphaShade", Float) = 0
		[Toggle(_INVERTSECONDALPHASHADE_ON)] _InvertSecondAlphaShade ("Invert Second AlphaShade?", Float) = 0
		[Toggle(_USEDISTORTION_ON)] _UseDistortion ("Use Distortion?", Float) = 0
		_DistortionIntensity ("Distortion Intensity", Range(0, 0.25)) = 0
		_BaseTexture ("Base Texture", 2D) = "white" {}
		_RotatingTexture ("Rotating Texture", 2D) = "white" {}
		_SecondIconAlphaTexture ("Second Icon Alpha Texture", 2D) = "white" {}
		[HideInInspector] _T_CloudNoise ("T_CloudNoise", 2D) = "white" {}
		[HideInInspector] _texcoord ("", 2D) = "white" {}
	}
	SubShader
	{
		LOD 0

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
				
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
				
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_particles
				
			#include "UnityCG.cginc"

			struct appdata_t 
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID				
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO					
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _ColorOverlay;
			uniform float4 _Color;
			uniform sampler2D _BaseTexture;
			uniform float4 _BaseTexture_ST;
			uniform sampler2D _SecondIconAlphaTexture;
			uniform float4 _SecondIconAlphaTexture_ST;
			uniform float _GlobalAlphaIntensity;

			v2f vert ( appdata_t v  )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
					
				v.vertex.xyz +=  float3( 0, 0, 0 ) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag ( v2f i  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( i );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

				//fixed4 origin = tex2D(_MainTex, i.texcoord) * i.color;
				//float2 uv_BaseTexture = i.texcoord.xy * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
				//float2 uv_SecondIconAlphaTexture = i.texcoord.xy * _SecondIconAlphaTexture_ST.xy + _SecondIconAlphaTexture_ST.zw;
				//float4 appendResult14 = float4(( origin * _ColorOverlay * _Color ).rgb , tex2D( _BaseTexture, uv_BaseTexture ).r * ( 1.0 - tex2D( _SecondIconAlphaTexture, uv_SecondIconAlphaTexture ).r ) * origin.a * _GlobalAlphaIntensity );
				
				float2 uv_BaseTexture = i.texcoord.xy * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
				float2 uv_origin = (i.texcoord.xy - fixed2(.08,.08)) * 1.2 * _MainTex_ST.xy + _MainTex_ST.zw;
				fixed4 origin = tex2D(_MainTex, uv_origin) * i.color * _GlobalAlphaIntensity;
				float4 appendResult14 = float4((origin * _ColorOverlay * _Color ).rgb , tex2D( _BaseTexture, uv_BaseTexture).r * _GlobalAlphaIntensity );				
				fixed4 col = appendResult14;
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