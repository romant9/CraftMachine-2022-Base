using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class RecycleWeaponDefinition
	{
		public int Identifier;

		public int Type;

		public string Object;

		public int Limit;

		public int Reward;

		public List<string> Pic;

		public List<string> RewardPic;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		[JsonIgnore]
		private long _StartTimeMilliseconds;

		[JsonIgnore]
		private long _EndTimeMilliseconds;

		[NonSerialized]
		[JsonIgnore]
		public List<RewardPicEntry> RewardPicEntries;

		[JsonIgnore]
		public long StartTimeMilliseconds
		{
			get
			{
				if (_StartTimeMilliseconds > 0)
				{
					return _StartTimeMilliseconds;
				}
				if (string.IsNullOrEmpty(StartTimeUTC))
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				_StartTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - dateTime).TotalSeconds * 1000;
				return _StartTimeMilliseconds;
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
				if (string.IsNullOrEmpty(EndTimeUTC))
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				_EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - dateTime).TotalSeconds * 1000;
				return _EndTimeMilliseconds;
			}
		}

		public void InitRewardPic()
		{
			RewardPicEntries = new List<RewardPicEntry>();
			if (RewardPic == null)
			{
				return;
			}
			foreach (string item in RewardPic)
			{
				if (!string.IsNullOrEmpty(item))
				{
					string[] array = item.Split(':');
					if (array.Length >= 2 && int.TryParse(array[0], out var result))
					{
						RewardPicEntries.Add(new RewardPicEntry(result, array[1]));
					}
				}
			}
		}

		public bool IsActive(long currentUtcTime)
		{
			if (StartTimeMilliseconds == 0L || EndTimeMilliseconds == 0L)
			{
				return true;
			}
			if (StartTimeMilliseconds <= currentUtcTime)
			{
				return EndTimeMilliseconds > currentUtcTime;
			}
			return false;
		}
	}
}
