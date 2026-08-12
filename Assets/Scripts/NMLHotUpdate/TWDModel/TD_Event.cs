using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TD_Event
	{
		public int Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string CloseTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public string TDMapId;

		public List<string> Debuff;

		public List<string> RollBuff;

		public List<string> DebuffImage;

		private long startTime;

		private long closeTime;

		private long endTime;

		[NonSerialized]
		[JsonIgnore]
		public Dictionary<int, List<TD_Reward>> Rewards;

		[NonSerialized]
		[JsonIgnore]
		public Dictionary<int, TD_MapConfig> MapConfigs;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long CloseTimeMilliseconds => closeTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetCloseTime(DateTime origin)
		{
			closeTime = (long)(GameEconomyData.ParseDateTime(CloseTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}
	}
}
