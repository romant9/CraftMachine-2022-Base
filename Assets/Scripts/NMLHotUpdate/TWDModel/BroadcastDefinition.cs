using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BroadcastDefinition
	{
		public string EventID;

		public int EventBroadcastOrder;

		public string EventBroadcastTitle;

		public bool IsCanSameEvent;

		public bool HaveBanner;

		public string TabTitle;

		public string Icon;

		public string BannerImage;

		public string BannerDesc;

		public bool Available;

		public int Params;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		[JsonIgnore]
		private long _EndTimeMilliseconds;

		[JsonIgnore]
		public long EndTimeMilliseconds
		{
			get
			{
				if (_EndTimeMilliseconds > 0)
				{
					return _EndTimeMilliseconds;
				}
				if (string.IsNullOrEmpty(EndTimeUtc))
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}
	}
}
