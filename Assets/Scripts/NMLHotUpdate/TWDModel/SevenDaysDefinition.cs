using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SevenDaysDefinition
	{
		public int Id;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public string TitleColor1;

		public string TitleColor2;

		public string DescColor;

		public string BundleIdentifier;

		public string Background;

		public string BackBox;

		[JsonIgnore]
		public long StartTimestamp
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - dateTime).TotalSeconds;
			}
		}

		[JsonIgnore]
		public long EndTimestamp
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - dateTime).TotalSeconds;
			}
		}

		[JsonIgnore]
		public DateTime StartDateTime
		{
			get
			{
				new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return GameEconomyData.ParseDateTime(StartTimeUtc);
			}
		}

		[JsonIgnore]
		public DateTime EndDateTime
		{
			get
			{
				new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return GameEconomyData.ParseDateTime(EndTimeUtc);
			}
		}
	}
}
