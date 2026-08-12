using System;

namespace TWDModel
{
	[Serializable]
	public class MissionHubContent
	{
		[Serializable]
		public enum ListPlacement
		{
			None = 0,
			Top = 1,
			Bottom = 2
		}

		public int Id;

		public string PrefabName;

		public ListPlacement Placement;

		public int SortInt;

		public string TitleLocalizationKey;

		public string CharacterMaterialOverride;

		public string BackgroundMaterialOverride;
	}
}
