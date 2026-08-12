using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BundleDefinition
	{
		public string Identifier;

		public string EpisodeIdentifier;

		public int DiamondsAmount;

		public int SuppliesAmount;

		public int ReplayTokenAmount;

		public int PhoneAmount;

		public int SurvivorSlots;

		public List<string> Outfits;

		public string SpecificEquipment;

		public EquipmentCategory EquipmentCategory;

		public int EquipmentLevelOffset;

		public SurvivorClass EquipmentSurvivorClass;

		public Rarity EquipmentRarity;

		public SurvivorClass SurvivorClass;

		public Rarity SurvivorRarity;

		public int SurvivorLevelOffset;

		public string SpriteName;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long OfferClientRemindTime;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long OfferAvailabilityTime;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long OfferMinTimeFromLastBought;

		public float OfferMinRequiredUSDSpent;

		public int OfferMissionEpisodeIndexRequired;

		public int OfferMissionIndexRequired;

		public string OfferStartTimestamp;

		public string OfferEndTimestamp;

		private long offerStartTime;

		private long offerEndTime;

		[JsonIgnore]
		public long OfferStartTimeMilliseconds => offerStartTime;

		[JsonIgnore]
		public long OfferEndTimeMilliseconds => offerEndTime;

		[JsonIgnore]
		public bool HasDateLimit
		{
			get
			{
				if (OfferStartTimeMilliseconds > 0)
				{
					return OfferEndTimeMilliseconds > 0;
				}
				return false;
			}
		}

		public void SetOfferTimes(DateTime origin)
		{
			offerStartTime = (long)(GameEconomyData.ParseDateTime(OfferStartTimestamp) - origin).TotalSeconds * 1000;
			offerEndTime = (long)(GameEconomyData.ParseDateTime(OfferEndTimestamp) - origin).TotalSeconds * 1000;
		}
	}
}
