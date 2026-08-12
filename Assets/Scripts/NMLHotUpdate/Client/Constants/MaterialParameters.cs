using UnityEngine;

namespace Client.Constants
{
	public class MaterialParameters
	{
		public static readonly int TintColor = Shader.PropertyToID("_TintColor");

		public static readonly int AlphaTex = Shader.PropertyToID("_AlphaTex");

		public static readonly int Color = Shader.PropertyToID("_Color");

		public static readonly int DetailColor = Shader.PropertyToID("_DetailColor");

		public static readonly int WipeSpeed = Shader.PropertyToID("_WipeSpeed");

		public static readonly int SecondBlend = Shader.PropertyToID("_SecondBlend");

		public static readonly int ClipOffset = Shader.PropertyToID("_ClipOffset");

		public static readonly int FractionalFrame = Shader.PropertyToID("_FractionalFrame");

		public static readonly int RampOffset = Shader.PropertyToID("_RampOffset");

		public static readonly int Angle = Shader.PropertyToID("_Angle");

		public static readonly int AccumOrig = Shader.PropertyToID("_AccumOrig");

		public static readonly int Offset = Shader.PropertyToID("_Offset");

		public static readonly int MainTex = Shader.PropertyToID("_MainTex");

		public static readonly int NextTex = Shader.PropertyToID("_NextTex");

		public static readonly int SecondTex = Shader.PropertyToID("_SecondTex");

		public static readonly int MaskTex = Shader.PropertyToID("_MaskTex");

		public static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

		public static readonly int MainTexST = Shader.PropertyToID("_MainTex_ST");

		public static readonly int NextTexST = Shader.PropertyToID("_NextTex_ST");

		public static readonly int SecondTexST = Shader.PropertyToID("_SecondTex_ST");

		public static readonly int PrecisionTime = Shader.PropertyToID("_PrecisionTime");
	}
}
