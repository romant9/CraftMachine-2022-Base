using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ThreeDayDefinition
	{
		public int Id;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public string reward1;

		public string reward2;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long refresh;

		public string spendetier1;

		public string spendetier2;

		public string BundleIdentifier;

		[NonSerialized]
		[JsonIgnore]
		public List<Rewards> RewardEntries1;

		[NonSerialized]
		[JsonIgnore]
		public List<Rewards> RewardEntries2;

		[JsonIgnore]
		private long _StartTimeMilliseconds;

		[JsonIgnore]
		private long _EndTimeMilliseconds;

		[JsonIgnore]
		public long StartTimeMilliseconds
		{
			get
			{
				if (_StartTimeMilliseconds > 0)
				{
					return _StartTimeMilliseconds;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _StartTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public long EndTimeMilliseconds
		{
			get
			{
				if (_EndTimeMilliseconds > 0)
				{
					return _EndTimeMilliseconds;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}

		public void CalcReward()
		{
			if (!string.IsNullOrEmpty(reward1))
			{
				RewardEntries1 = (from x in reward1.Split(';')
					select new Rewards(x)).ToList();
			}
			if (!string.IsNullOrEmpty(reward2))
			{
				RewardEntries2 = (from x in reward2.Split(';')
					select new Rewards(x)).ToList();
			}
		}
	}
}
