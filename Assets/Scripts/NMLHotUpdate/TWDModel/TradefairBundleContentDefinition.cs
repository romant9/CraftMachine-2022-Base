using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TradefairBundleContentDefinition
	{
		public string Identifier;

		public int IAPProduct;

		public string EpicOfferID;

		public ThirdPartyName IsThirdParty;

		public string Category;

		public string Rewards;

		public List<string> RewardsPrefabsData;

		public List<string> RewardsExtraLocalization;

		public List<string> RewardsImageURL;

		public int StrikePricePercentage;

		public BundleType BundleType;

		public BundleClassification Classification;

		public bool PayBanana;

		public string BananaBonus;

		public double ShowPrice;

		public bool HideCoinPurchase;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		[NonSerialized]
		[JsonIgnore]
		public Rewards ExtraRewardEntries;

		public bool IsNormalBundle()
		{
			return BundleType == BundleType.Bundle;
		}
	}
}
