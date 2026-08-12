using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MissionSpawnPointGroup
	{
		public int Id;

		public int EpisodeDifficultyLevel;

		public string MapId;

		public MapCategory Category;

		public string Subcategory;

		public string DisplayName;

		public string BackgroundId;

		public int CostIndex;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string UnlockUTC;

		public string VideoURL;

		public List<MissionSpawnPoint> MissionSpawnPoints = new List<MissionSpawnPoint>();

		[JsonIgnore]
		public Dictionary<string, MissionSpawnPoint> MissionSpawnPointsById = new Dictionary<string, MissionSpawnPoint>();

		[JsonIgnore]
		public long UnlockTimeMilliseconds
		{
			get
			{
				if (string.IsNullOrEmpty(UnlockUTC))
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(UnlockUTC) - dateTime).TotalSeconds * 1000;
			}
		}

		public MissionSpawnPointGroup()
		{
			EpisodeDifficultyLevel = 1;
		}

		public MissionSpawnPoint GetSpawnPointByMissionId(string missionId)
		{
			if (MissionSpawnPointsById.TryGetValue(missionId, out var value))
			{
				return value;
			}
			return null;
		}
	}
}
