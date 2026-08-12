using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklySurvival
	{
		public int Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public int DetailMapId;

		public string RewardSetName;

		public string SurvivalMissionConfig1;

		public string SurvivalMissionConfig2;

		public string SurvivalMissionConfig3;

		public int SectionMissionCount1;

		public int SectionMissionCount2;

		public int SectionMissionCount3;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public int TotalMissionCount => SectionMissionCount1 + SectionMissionCount2 + SectionMissionCount3;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}
	}
}
