using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GvGSeasonDefinition
	{
		public int Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

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

		public override string ToString()
		{
			return $"[GvGSeasonDefinition: StartTimeMilliseconds={StartTimeMilliseconds}, EndTimeMilliseconds={EndTimeMilliseconds}]";
		}
	}
}
