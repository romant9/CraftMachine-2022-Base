using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WorldBossBattlegroundDefinition
	{
		public int ID;

		public int BgLevelGroup;

		public List<int> BgLevel;

		public string CapturePointType;

		public string CapturePoint;

		public string MapIds;

		public int EnemyLevel;

		public string EnemyActorId;

		public string After;

		public string BuildingName;

		public string BuildingDesc;

		public string BuildingLockedDesc;

		public string BuildingDoneDesc;

		public string BuildingEffDesc;

		public List<string> DeBuff;

		private List<DifficultyIncrementalDebuff> worldBossDebuffs;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> WorldBossDebuffs => worldBossDebuffs;

		public bool IsPVECapturePointType()
		{
			return CapturePointType.Equals("PVE", StringComparison.OrdinalIgnoreCase);
		}

		public bool IsBOSSCapturePointType()
		{
			return CapturePointType.Equals("BOSS", StringComparison.OrdinalIgnoreCase);
		}

		public bool WithinBgLevel(int level)
		{
			if (BgLevel == null || BgLevel.Count == 0)
			{
				return false;
			}
			if (BgLevel.Count == 1)
			{
				return BgLevel[0] == level;
			}
			if (BgLevel[0] <= level)
			{
				return level <= BgLevel[1];
			}
			return false;
		}

		public void SetDebuffss(List<DifficultyIncrementalDebuff> debuffs)
		{
			worldBossDebuffs = debuffs;
		}
	}
}
