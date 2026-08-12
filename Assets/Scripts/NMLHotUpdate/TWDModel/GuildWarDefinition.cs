using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildWarDefinition
	{
		public int Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string FirstBattleTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		private long startTime;

		private long endTime;

		private long firstBattleTime;

		public string SectorString;

		public string RewardSetName;

		public int CostIndex;

		[JsonIgnore]
		public int[] SectorsIds;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public long FirstBattleTimeMilliseconds => firstBattleTime;

		public bool IsOpen(long utcTimeStamp)
		{
			if (utcTimeStamp >= StartTimeMilliseconds)
			{
				return utcTimeStamp < EndTimeMilliseconds;
			}
			return false;
		}

		public long TimeUntilStartMilliseconds(long utcTimeStamp)
		{
			return StartTimeMilliseconds - utcTimeStamp;
		}

		public long TimeUntilEndMilliseconds(long utcTimeStamp)
		{
			return EndTimeMilliseconds - utcTimeStamp;
		}

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetFirstBattleStartTime(DateTime origin)
		{
			firstBattleTime = (long)(GameEconomyData.ParseDateTime(FirstBattleTimeUTC) - origin).TotalSeconds * 1000;
		}

		public override string ToString()
		{
			return $"[GuildWarDefinition: StartTimeMilliseconds={StartTimeMilliseconds}, EndTimeMilliseconds={EndTimeMilliseconds}]";
		}
	}
}
