using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class PhoneCallVisual
	{
		public const string FALLBACK_NAME = "FallbackVisual";

		public string Name;

		public string BgImgPath;

		public string OverlayImgPath;

		public string TitleColor;

		public string Title2Color;

		public string LocalisationKey;

		public string HeroUp;

		public List<string> AmountEffect;
	}
}
