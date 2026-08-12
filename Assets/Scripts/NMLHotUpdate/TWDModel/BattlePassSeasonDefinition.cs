using System;

namespace TWDModel
{
	[Serializable]
	public class BattlePassSeasonDefinition
	{
		public int Id;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public string HighlightedRewardTexture;

		public string NameLocKey;

		public string BundleIdentifier;

		public string TitleColor;

		public string BackgroundColor;

		public string PopupIcon;
	}
}
