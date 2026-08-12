using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EndlessModeCalendarDefinition
	{
		public int Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public string MapID;

		public string ExpertMapID;

		public string SpawnSetupID;

		public string SpawnSetIDExpert;

		public string NormalWaveRewardSetID;

		public string ExpertModeWaveRewardSetID;

		public string LeaderBoardRewardSetID1;

		public string LeaderBoardRewardSetID2;

		public string LeaderBoardRewardSetID;

		public string ExpertModeHeroSetID;

		public int MaxWalkerAmount;

		public int MaxWalkerAmountExpert;

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
				return _StartTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - dateTime).TotalSeconds * 1000;
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
				return _EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - dateTime).TotalSeconds * 1000;
			}
		}

		public List<string> GetNormalWaveRewardSetIDs => NormalWaveRewardSetID.Split(';').ToList();

		public List<string> GetExpertModeWaveRewardSetIDs => ExpertModeWaveRewardSetID.Split(';').ToList();
	}
}
