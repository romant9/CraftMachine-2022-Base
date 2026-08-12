using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallenge
	{
		public int Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public int DetailMapId;

		public int ApocalypticMapId;

		public int RoundsToSkipToken;

		public string FeaturedStarHero;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> DebuffConfigs => null;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public WeeklyChallenge()
		{
			RoundsToSkipToken = 2;
		}

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}
	}
}
