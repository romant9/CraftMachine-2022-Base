using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SystemOpen
	{
		public string SystemID;

		public string SystemName;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTime;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTime;

		public string UnOpenedTips;

		public int OpenCampLv;

		public string ShowType;

		public int ShowCampLv;

		[JsonIgnore]
		public long StartTimeMilliseconds
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(StartTime) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public long EndTimeMilliseconds
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(EndTime) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public bool HasDateLimit
		{
			get
			{
				if (StartTimeMilliseconds > 0)
				{
					return EndTimeMilliseconds > 0;
				}
				return false;
			}
		}
	}
}
