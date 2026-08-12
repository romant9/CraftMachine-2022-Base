using System;

namespace TWDModel
{
	[Serializable]
	public class BlackMarketDefinition : IWeightedItem
	{
		public int UniqueId;

		public string ActorDefinitionID;

		public string ItemCategory;

		public string Reward;

		public int Weight;

		public string Price;

		public int Quantity;

		public int MinCouncilLevel;

		public int MaxCouncilLevel;

		public int MinStars;

		public int MaxStars;

		public bool NeedHeroUnlocked;

		public bool CanBePurchasedAgain;

		public int BlackMarketToken;

		public int GetWeight()
		{
			return Weight;
		}

		public int GetPrice(TWDModelManager modelManager)
		{
			int num = HelpersModel.ParsePrice(Price).priceAmount;
			if (new Rewards(Reward).RewardsList[0] is RewardMissingTokens rewardMissingTokens)
			{
				num = rewardMissingTokens.GetTokenAmount(modelManager) * num;
			}
			return num;
		}

		public CurrencyType GetCurrencyType()
		{
			return HelpersModel.ParsePrice(Price).priceCurrency;
		}
	}
}
