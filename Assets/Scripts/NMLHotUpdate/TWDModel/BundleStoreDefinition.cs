using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BundleStoreDefinition
	{
		public string BundleIdentifier;

		public int DisplayOrder;

		public int ShopTabIndex;

		public List<string> SpenderTiers;

		public int MaxPurchases;

		public string PreviousBundle;

		public string EquivalentPreviousRotation;

		public string OverrideTitleLocalization;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long PopupCooldownTimer;

		public bool ShowTimer;

		public bool ShowTimerInCard;

		public bool IsTaggedAsFreeItem;

		public bool HidePrice;

		public bool IsPartOfRotation;

		public CurrencyType CardCurrencyToShow;

		public bool ShowOfferPopup;

		public string CardMainSpriteName;

		public string OfferButtonSpriteName;

		public string CardPrefab;

		public bool Shiny;

		public string CardImageURL;

		public string CardImageContentPathItem;

		public string CardImageContentPathHero;

		public string ValueBadgeLocalisation;

		public string SalesBadgeLocalisation;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long AvailabilityTime;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long MinTimeFromLastCategoryBought;

		public SurvivorClass SurvivorClassRequired;

		public int MissionSpawnPointIndexRequired;

		public string MapIdRequired;

		public int MissionIndexRequired;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimestamp;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimestamp;

		public bool NoPopUpOfferTimer;

		public string OverallImagePath;

		public string LocalImageName;

		public int CardImageRatio;

		public bool ShowMaxPurchases;

		public string OverrideTitleLocalizationDetail;

		public string FrontPageLabelLocalization;

		private long startTime;

		private long endTime;

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
	}
}
