using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildShopDefinition
	{
		public int ID;

		public int Season;

		public int TierRequirement;

		public bool VIPRequired;

		public string Content;

		[NonSerialized]
		[JsonIgnore]
		public Rewards ContentRewards;

		[NonSerialized]
		[JsonIgnore]
		public CurrencyType PriceCurrency;

		[NonSerialized]
		[JsonIgnore]
		public int PriceAmount;

		public string PriceString;

		public int InitialAmount;

		public int RestockOnNewWar;

		public int RestockOnNewTier;

		[JsonIgnore]
		public bool LimitedPurchases => InitialAmount != -1;

		[JsonIgnore]
		public bool UniquePurchase => InitialAmount + RestockOnNewWar + RestockOnNewTier == 1;

		public GuildShopDefinition()
		{
		}

		public GuildShopDefinition(GuildShopDefinition other)
		{
			ID = other.ID;
			Season = other.Season;
			TierRequirement = other.TierRequirement;
			VIPRequired = other.VIPRequired;
			Content = other.Content;
			PriceString = other.PriceString;
			PriceCurrency = other.PriceCurrency;
			PriceAmount = other.PriceAmount;
			InitialAmount = other.InitialAmount;
			RestockOnNewWar = other.RestockOnNewWar;
			RestockOnNewTier = other.RestockOnNewTier;
		}

		public void Setup()
		{
			if (!string.IsNullOrEmpty(PriceString))
			{
				(PriceAmount, PriceCurrency) = HelpersModel.ParsePrice(PriceString);
			}
			if (ContentRewards == null)
			{
				try
				{
					ContentRewards = new Rewards(Content);
				}
				catch (Exception)
				{
					ContentRewards = new Rewards();
				}
			}
		}

		public override string ToString()
		{
			return "Guild Shop item: ID=" + ID + " Season=" + Season + " VIP Required=" + VIPRequired + "TierRequirement=" + TierRequirement + " Content=" + Content + " Price=" + PriceAmount + " InitialAmount=" + InitialAmount + " RestockOnNewWar=" + RestockOnNewWar + " RestockOnNewTier=" + RestockOnNewTier;
		}
	}
}
