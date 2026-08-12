using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class OutfitDefinition
	{
		public string ID;

		public int Cost;

		public string LocalizationKey;

		public string BundleSprite;

		public string ShopAvailableStartTimestamp;

		public string ShopAvailableEndTimestamp;

		private long shopAvailableStartTime;

		private long shopAvailableEndTime;

		[JsonIgnore]
		public string TitleLocalizationKey
		{
			get
			{
				if (LocalizationKey == null)
				{
					return null;
				}
				return LocalizationKey + ".Title";
			}
		}

		[JsonIgnore]
		public string SeasonLocalizationKey
		{
			get
			{
				if (LocalizationKey == null)
				{
					return null;
				}
				return LocalizationKey + ".Season";
			}
		}

		[JsonIgnore]
		public long ShopAvailableStartTimeMilliseconds => shopAvailableStartTime;

		[JsonIgnore]
		public long ShopAvailableEndTimeMilliseconds => shopAvailableEndTime;

		public void SetShopAvailabilityTimes(DateTime origin)
		{
			if (!string.IsNullOrEmpty(ShopAvailableStartTimestamp))
			{
				shopAvailableStartTime = (long)(GameEconomyData.ParseDateTime(ShopAvailableStartTimestamp) - origin).TotalSeconds * 1000;
			}
			if (!string.IsNullOrEmpty(ShopAvailableEndTimestamp))
			{
				shopAvailableEndTime = (long)(GameEconomyData.ParseDateTime(ShopAvailableEndTimestamp) - origin).TotalSeconds * 1000;
			}
		}

		public bool IsAvailableOnShop(long currentUTCTime)
		{
			bool flag = true;
			if (ShopAvailableStartTimeMilliseconds > 0)
			{
				flag = currentUTCTime > ShopAvailableStartTimeMilliseconds;
			}
			if (ShopAvailableEndTimeMilliseconds > 0)
			{
				flag = flag && currentUTCTime < ShopAvailableEndTimeMilliseconds;
			}
			return flag;
		}
	}
}
