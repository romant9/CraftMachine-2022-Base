using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CustomBundleDefinition
	{
		public string Identifier;

		public string OverrideTitleLocalization;

		public int Order;

		[GEDType(GEDSpecialType.InAppProductId)]
		public string IAPProduct;

		public string EpicOfferID;

		public string SteamOfferID;

		public int TradefairPrice;

		public string Rewards;

		public List<int> StorageID;

		public bool ExcludeSameItem;

		public int MaxPurchases;

		public bool ShowMaxPurchases;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long RefreshTime;

		public List<string> SpenderTier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimestamp;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimestamp;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long MinTimeFromLastCategoryBought;

		public int GoldPrice;

		public int FragmentPrice;

		public bool ShowAvailabilityTime;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		private long startTime;

		private long endTime;

		private CustomizedBundleType customizedBundleType;

		[JsonIgnore]
		public CustomizedBundleType CustomBundleType => customizedBundleType;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		[JsonIgnore]
		public bool HasDateLimit
		{
			get
			{
				if (StartTimeMilliseconds > 0)
				{
					return EndTimeMilliseconds > 0;
				}
				return false;
			}
		}

		public void SetTimeLimits(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimestamp) - origin).TotalSeconds * 1000;
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimestamp) - origin).TotalSeconds * 1000;
		}

		public void SetCustomizedBundleType(CustomizedBundleType type)
		{
			customizedBundleType = type;
		}
	}
}
