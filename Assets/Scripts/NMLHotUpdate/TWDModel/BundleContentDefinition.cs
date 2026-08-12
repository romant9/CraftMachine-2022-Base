using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BundleContentDefinition
	{
		public string Identifier;

		[GEDType(GEDSpecialType.InAppProductId)]
		public string IAPProduct;

		public string EpicOfferID;

		public string SteamOfferID;

		[GEDType(GEDSpecialType.InAppProductId)]
		public string TradefairPrice;

		public bool IsAPP;

		public bool IsEpic;

		public bool IsSteam;

		public bool IsTradeFair;

		public ThirdPartyName IsThirdParty;

		public string Category;

		public string Rewards;

		public List<string> RewardsPrefabsData;

		public List<string> RewardsExtraLocalization;

		public List<string> RewardsImageURL;

		public int StrikePricePercentage;

		public int TradeFairPriceNew;

		public BundleType BundleType;

		public BundleClassification Classification;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		public static string CategoryGoldPack = "GoldPack";

		public static string CategoryBundle = "Bundle";

		public static string CategoryOffer = "Offer";

		public static string CategoryHidden = "Hidden";

		public bool IsNormalBundle()
		{
			return BundleType == BundleType.Bundle;
		}
	}
}
