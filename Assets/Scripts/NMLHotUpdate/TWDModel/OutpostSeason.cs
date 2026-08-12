using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class OutpostSeason
	{
		public int Id;

		public int TierSetId;

		public string LocalizationKey;

		public string StartTimeUtc;

		public string EndTimeUtc;

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
	}
}
