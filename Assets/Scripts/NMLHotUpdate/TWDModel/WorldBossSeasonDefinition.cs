using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WorldBossSeasonDefinition
	{
		public int Season;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public string Coin;

		public int StartDifficulty;

		public int Shop;

		public string SeasonTitle;

		public string SeasonPic;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public bool IsOpen(long utcTimeStamp)
		{
			if (utcTimeStamp >= startTime)
			{
				return utcTimeStamp < endTime;
			}
			return false;
		}

		public long TimeUntilStartMilliseconds(long utcTimeStamp)
		{
			return startTime - utcTimeStamp;
		}

		public long TimeUntilEndMilliseconds(long utcTimeStamp)
		{
			return endTime - utcTimeStamp;
		}

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}

		public override string ToString()
		{
			return $"[WorldBossSeasonDefinition: Season={Season}, StartTimeMilliseconds={startTime}, EndTimeMilliseconds={endTime}]";
		}
	}
}
