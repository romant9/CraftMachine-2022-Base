using System;

namespace TWDModel
{
	[Serializable]
	public class HeroSkinDefinition
	{
		public string ID;

		public string HeroID;

		public string LocalizationKey;

		public string SeasonLocalizationKey;

		public bool AvailableOnHeroPurchased;
	}
}
