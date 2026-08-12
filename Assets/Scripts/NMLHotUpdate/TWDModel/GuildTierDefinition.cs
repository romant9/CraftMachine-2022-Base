using System;

namespace TWDModel
{
	[Serializable]
	public class GuildTierDefinition
	{
		public int Tier;

		public string NameLocalizationKey;

		public string IconSprite;

		public int VictoryPointsRequired;

		public int Category;

		public float VictoryPointsMultiplier;

		public float RewardPointsMultiplier;

		public float DrawPointsMultiplier;

		public float RewardPointsDrawMultiplier;
	}
}
