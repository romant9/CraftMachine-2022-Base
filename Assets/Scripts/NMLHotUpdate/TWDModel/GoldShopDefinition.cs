using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GoldShopDefinition
	{
		public string ItemId;

		public string ItemSpriteName;

		public List<string> GuaranteedComponents;

		public int Price;

		public int RandomComponentCount;

		public string EventControl;

		public int DisplayOrder;

		public string OverrideTitleLocalization;

		public string GreenTagLocalisation;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long MinTimeFromLastCategoryBought;

		public bool ShowTimerInCard;

		public string CardImageContentPathItem;

		public string LocalImageName;

		public int CardImageRatio;

		public int MaxPurchases;

		public string Reward;

		public bool ShowMaxPurchases;

		public string ValueBadgeLocalisation;

		public string DescriptionLocalization;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		private long startTime;

		private long endTime;

		[NonSerialized]
		[JsonIgnore]
		public List<ComponentCrateItem> SubItems;

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

		[JsonIgnore]
		public bool IsNewVersion => !string.IsNullOrEmpty(Reward);

		[JsonIgnore]
		public bool IsSingleReward
		{
			get
			{
				bool result = false;
				if (RewardEntries != null && RewardEntries.RewardsList != null && RewardEntries.RewardsList.Count == 1)
				{
					result = true;
				}
				return result;
			}
		}

		public void SetTimeLimits(DateTime origin)
		{
			if (StartTimeUTC != null && EndTimeUTC != null)
			{
				startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
				endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
			}
		}

		private CurrencyType ParseComponentCurrency(string s)
		{
			try
			{
				CurrencyType currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), s);
				if (ComponentHelper.IsComponentCurrency(currencyType))
				{
					return currencyType;
				}
			}
			catch (ArgumentException)
			{
			}
			return CurrencyType.None;
		}

		private void AddSubItem(string s, int count)
		{
			int result = 0;
			if (int.TryParse(s, out result))
			{
				SubItems.Add(new ComponentCrateItem(null, result, count));
				return;
			}
			CurrencyType currencyType = ParseComponentCurrency(s);
			if (currencyType != CurrencyType.None)
			{
				SubItems.Add(new ComponentCrateItem(ComponentHelper.GetComponentTypeName(currencyType), ComponentHelper.GetComponentRarityLevel(currencyType), count));
			}
			else if (ParseComponentCurrency(s + "0") != CurrencyType.None)
			{
				SubItems.Add(new ComponentCrateItem(s, -1, count));
			}
		}

		public void InitializeSubItems()
		{
			SubItems = new List<ComponentCrateItem>();
			string text = null;
			int num = 0;
			for (int i = 0; i < ((GuaranteedComponents != null) ? GuaranteedComponents.Count : 0); i++)
			{
				string text2 = GuaranteedComponents[i].Trim();
				if (text != null && text2 == text)
				{
					num++;
				}
				else
				{
					if (text != null)
					{
						AddSubItem(text, num);
					}
					num = 1;
					text = text2;
				}
				if (i == GuaranteedComponents.Count - 1)
				{
					AddSubItem(text, num);
				}
			}
			if (RandomComponentCount > 0)
			{
				SubItems.Add(new ComponentCrateItem(null, -1, RandomComponentCount));
			}
		}
	}
}
