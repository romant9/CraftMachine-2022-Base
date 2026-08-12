Shader "Hidden/Amplify Color/BaseVignette" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_VignetteAspect ("Vignette Aspect", Float) = 1.7
		_VignetteScale("Vignette Scale", Float) = 1.1
		_VSoft("Vignette Softness", Range(0.0, 1.0)) = 0.5
		_VignetteColor("Vignette Color", Vector) = (0, 0, 0, 1)
		_lerpAmount ("Lerp Amount", Float) = 1
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #include "UnityCG.cginc"
			
			// Properties
			sampler2D _MainTex;
			float4 _VignetteColor;
			float _VignetteScale;
			float _VSoft;
			float _VignetteAspect;
			float _lerpAmount;

			float4 frag(v2f_img input) : COLOR
			{
                // sample texture for color
				float4 color = tex2D(_MainTex, input.uv);

				// add vignette
				float distFromCenter = distance(float2(input.uv.x * _VignetteAspect, input.uv.y) , float2(0.5 * _VignetteAspect, 0.5));
				float vignette = smoothstep(_VignetteScale, _VignetteScale - _VSoft, distFromCenter);
				vignette = smoothstep(vignette, 0, distFromCenter) * _lerpAmount;

				float4 blend = saturate(color * vignette) + (1 - vignette) * _VignetteColor * _VignetteColor.w;
				
				return lerp(color, blend, _lerpAmount);
			}

			ENDCG
		}
	}
}