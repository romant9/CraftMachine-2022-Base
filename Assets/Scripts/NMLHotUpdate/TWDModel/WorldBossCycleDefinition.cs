using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WorldBossCycleDefinition
	{
		public int ID;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public int Cycle;

		public int Season;

		private const long DefaultSignUpDeadlineOffsetMs = 7200000L;

		private const long DefaultDifficultyDeadlineOffsetMs = 3600000L;

		private long signUpDeadlineOffsetMs = 7200000L;

		private long difficultyDeadlineOffsetMs = 3600000L;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		[JsonIgnore]
		public long SignUpDeadlineMilliseconds => startTime - signUpDeadlineOffsetMs;

		[JsonIgnore]
		public long DifficultyDeadlineMilliseconds => startTime - difficultyDeadlineOffsetMs;

		public bool IsOpen(long utcTimeStamp)
		{
			if (utcTimeStamp >= startTime)
			{
				return utcTimeStamp < endTime;
			}
			return false;
		}

		public bool IsSignUpOpen(long utcTimeStamp)
		{
			return utcTimeStamp < SignUpDeadlineMilliseconds;
		}

		public bool IsDifficultySelectionOpen(long utcTimeStamp)
		{
			return utcTimeStamp < DifficultyDeadlineMilliseconds;
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

		public void SetSignUpCloseMinutes(int minutes)
		{
			signUpDeadlineOffsetMs = ((minutes > 0) ? ((long)minutes * 60000L) : 7200000);
		}

		public void SetDifficultyCloseMinutes(int minutes)
		{
			difficultyDeadlineOffsetMs = ((minutes > 0) ? ((long)minutes * 60000L) : 3600000);
		}

		public override string ToString()
		{
			return $"[WorldBossCycleDefinition: ID={ID}, Cycle={Cycle}, Season={Season}, StartTimeMilliseconds={startTime}, EndTimeMilliseconds={endTime}]";
		}
	}
}
