using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SpenderTierDefinition
	{
		public string TierIdentifier;

		public double MinMoneySpent;

		public double MaxMoneySpent;

		public int MinDaysPlayed;

		public int MaxDaysPlayed;

		public int MinPurchases;

		public int MaxPurchases;

		public int MinCouncilLevel;

		public int MaxCouncilLevel;

		public int MinAveragePurchasePrice;

		public int MaxAveragePurchasePrice;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long MinTimeFromLastPurchase;

		public string MinCreationTimeStamp;

		public string MaxCreationTimeStamp;

		public string GeoSegment;

		private long minCreationTime;

		private long maxCreationTime;

		[JsonIgnore]
		public long MinCreationTimeMilliseconds => minCreationTime;

		[JsonIgnore]
		public long MaxCreationTimeMilliseconds => maxCreationTime;

		public List<string> GeoSegments => GeoSegment.Split(';').ToList();

		public void SetTimeLimits(DateTime origin)
		{
			minCreationTime = (string.IsNullOrEmpty(MinCreationTimeStamp) ? 0 : ((long)(GameEconomyData.ParseDateTime(MinCreationTimeStamp) - origin).TotalSeconds * 1000));
			maxCreationTime = (string.IsNullOrEmpty(MaxCreationTimeStamp) ? 0 : ((long)(GameEconomyData.ParseDateTime(MaxCreationTimeStamp) - origin).TotalSeconds * 1000));
		}
	}
}
