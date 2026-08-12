using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleAttackTargetMissionData
	{
		public GuildBattleMapMissionModel MissionModel { get; set; }

		public string AttackMissionId { get; set; }

		public int MissionSectorId { get; private set; }

		public int MissionPositionWithinArea { get; private set; }

		public int MissionConfigIndexObjective { get; private set; }

		public int MissionConfigIndexEnemies { get; private set; }

		public int MissionQueueIndex { get; private set; }

		public GuildBattleMapMissionModel.MissionType MissionType { get; private set; }

		public bool IsPvPCombat { get; set; }

		public List<int> KilledPVPSurvivorsIndexes { get; set; }

		[JsonIgnore]
		public List<int> GuildSideKilledPVPSurvivorsIndexes { get; set; }

		public void AttackMission(GuildBattleMapMissionModel missionModel)
		{
			MissionModel = missionModel;
			AttackMissionId = missionModel.Id;
			MissionSectorId = MissionModel.SectorIdOwner;
			MissionPositionWithinArea = MissionModel.MissionPositionWithinArea;
			MissionConfigIndexObjective = MissionModel.MissionConfigIndexObjective;
			MissionConfigIndexEnemies = MissionModel.MissionConfigIndexEnemies;
			MissionType = MissionModel.Type;
			MissionQueueIndex = MissionModel.MissionQueueIndex;
			IsPvPCombat = missionModel.IsEnemyUnlocked();
			KilledPVPSurvivorsIndexes = new List<int>(missionModel.SavedData);
		}

		public void ReturnFromCombat()
		{
			if (MissionModel != null)
			{
				MissionModel.ClearMissionConfigOverride();
				if (GuildSideKilledPVPSurvivorsIndexes != null)
				{
					MissionModel.UpdateSaveData(GuildSideKilledPVPSurvivorsIndexes);
					GuildSideKilledPVPSurvivorsIndexes = null;
				}
				MissionModel = null;
			}
			KilledPVPSurvivorsIndexes = null;
		}

		public void Setup(TWDModelManager manager)
		{
			if (MissionModel != null)
			{
				GvGSeasonModel gvGSeasonModel = manager.Player.GvGSeasonModel;
				MissionModel.SetPlayerOwnerAndGameEconomyData(manager.GameEconomyData, gvGSeasonModel, manager.Player);
				MissionModel.Id = AttackMissionId;
				MissionModel.SectorIdOwner = MissionSectorId;
				MissionModel.MissionPositionWithinArea = MissionPositionWithinArea;
				MissionModel.SavedData = KilledPVPSurvivorsIndexes;
				MissionModel.MissionConfigIndexObjective = MissionConfigIndexObjective;
				MissionModel.MissionConfigIndexEnemies = MissionConfigIndexEnemies;
				MissionModel.Type = MissionType;
				MissionModel.MissionQueueIndex = MissionQueueIndex;
			}
		}

		public void Clear()
		{
			if (MissionModel != null)
			{
				MissionModel.ClearMissionConfigOverride();
				MissionModel = null;
			}
			AttackMissionId = null;
			MissionSectorId = 0;
			MissionPositionWithinArea = 0;
			KilledPVPSurvivorsIndexes = null;
			GuildSideKilledPVPSurvivorsIndexes = null;
			MissionConfigIndexEnemies = -1;
			MissionConfigIndexObjective = -1;
			MissionQueueIndex = -1;
		}
	}
}
