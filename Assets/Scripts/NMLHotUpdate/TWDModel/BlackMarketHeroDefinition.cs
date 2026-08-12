using System;

namespace TWDModel
{
	[Serializable]
	public class BlackMarketHeroDefinition : IWeightedItem
	{
		public string ActorDefinitionID;

		public string HeroSeasonIDArt;

		public int HeroWeight;

		public int GetWeight()
		{
			return HeroWeight;
		}
	}
}
