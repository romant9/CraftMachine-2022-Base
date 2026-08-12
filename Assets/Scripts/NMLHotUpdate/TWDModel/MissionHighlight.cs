using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MissionHighlight
	{
		public string MapId;

		public int Version;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndUTC;

		public string CompletionReward;

		public string DescriptionKey;

		public string BundleId;

		public string BundleSpriteName;

		[NonSerialized]
		[JsonIgnore]
		public Rewards CompletionRewards;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public void Setup()
		{
			if (CompletionRewards == null)
			{
				try
				{
					CompletionRewards = new Rewards(CompletionReward, null, 0, EquipmentSource.MissionLoot);
				}
				catch (Exception)
				{
					CompletionRewards = new Rewards();
				}
			}
			if (!string.IsNullOrEmpty(StartUTC))
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				startTime = (long)(GameEconomyData.ParseDateTime(StartUTC) - dateTime).TotalSeconds * 1000;
			}
			if (!string.IsNullOrEmpty(EndUTC))
			{
				DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				endTime = (long)(GameEconomyData.ParseDateTime(EndUTC) - dateTime2).TotalSeconds * 1000;
			}
		}

		public bool IsActive(long timeUtc)
		{
			if (string.IsNullOrEmpty(StartUTC) || string.IsNullOrEmpty(EndUTC))
			{
				return false;
			}
			if (StartTimeMilliseconds <= timeUtc && EndTimeMilliseconds >= timeUtc)
			{
				return true;
			}
			return false;
		}

		public bool WasActiveBefore(long timeUtc)
		{
			if (string.IsNullOrEmpty(StartUTC) || string.IsNullOrEmpty(EndUTC))
			{
				return false;
			}
			if (StartTimeMilliseconds < timeUtc && EndTimeMilliseconds < timeUtc)
			{
				return true;
			}
			return false;
		}

		public long GetTimeLeft(long utcTime)
		{
			return Math.Max(EndTimeMilliseconds - utcTime, 0L);
		}

		public long GetTimeUntilStart(long utcTime)
		{
			return Math.Max(StartTimeMilliseconds - utcTime, 0L);
		}
	}
}
