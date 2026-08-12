using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GuildBattleMapSectorModel : TWDGroupModelChild
	{
		public enum Status
		{
			Available = 0,
			Completed = 1,
			Exhausted = 2
		}

		public const int AreaAmountInSector = 4;

		public const string GuildBattleSectorStatusChange = "GuildBattleSectorStatus";

		public const string GuildBattleAddCompletionToArea = "GuildBattleAddCompletionToArea";

		[JsonIgnore]
		public List<GuildBattleMissionQueueData>[] AreaMissions;

		[JsonIgnore]
		public string MissionConfigPoolName;

		[JsonIgnore]
		public string MissionPoolName;

		[JsonIgnore]
		private GuildBattleSectorDefinition[] prerequisitesInternal;

		[JsonIgnore]
		private GuildBattleSectorDefinition missionSectorDefinitionInternal;

		public int SectorId { get; set; }

		public TWDGroupChildModelList<GuildBattleMapMissionModel> RandomizedMissions { get; set; }

		public List<List<int>> StartIndexPerArea { get; set; }

		public int CurrentBatchIndex { get; set; }

		public Status SectorStatus { get; set; }

		[JsonIgnore]
		public GuildBattleSectorDefinition[] Prerequisites
		{
			get
			{
				if (prerequisitesInternal == null && base.gameEconomyData != null && MissionSectorDefinition != null && MissionSectorDefinition.PrerequisitesSectorIds != null)
				{
					prerequisitesInternal = new GuildBattleSectorDefinition[MissionSectorDefinition.PrerequisitesSectorIds.Length];
					for (int i = 0; i < MissionSectorDefinition.PrerequisitesSectorIds.Length; i++)
					{
						prerequisitesInternal[i] = base.gameEconomyData.FindMissionSectorDefinition(MissionSectorDefinition.PrerequisitesSectorIds[i]);
					}
				}
				return prerequisitesInternal;
			}
		}

		[JsonIgnore]
		public GuildBattleSectorDefinition MissionSectorDefinition
		{
			get
			{
				if (missionSectorDefinitionInternal == null && base.gameEconomyData != null)
				{
					missionSectorDefinitionInternal = base.gameEconomyData.FindMissionSectorDefinition(SectorId);
				}
				return missionSectorDefinitionInternal;
			}
		}

		public GuildBattleMapSectorModel()
		{
			SectorStatus = Status.Available;
			RandomizedMissions = new TWDGroupChildModelList<GuildBattleMapMissionModel>();
			StartIndexPerArea = new List<List<int>>();
		}

		public override void Start()
		{
			base.Start();
			AreaMissions = new List<GuildBattleMissionQueueData>[4];
		}

		public bool IsCompleted()
		{
			if (SectorStatus == Status.Completed)
			{
				if (HelpersModel.IsUnlockAllSectors) return false;
				return true;
			}
			int num = 0;
			if (RandomizedMissions != null)
			{
				for (int i = 0; i < RandomizedMissions.Count; i++)
				{
					if (RandomizedMissions[i].IsPvpComplete())
					{
						num++;
					}
				}
			}
			return num >= MissionSectorDefinition.PVPEnemyAmount;
		}

		public bool IsStartedButNotComplete()
		{
			if (IsCompleted())
			{
				return false;
			}
			return RandomizedMissions.Count((GuildBattleMapMissionModel x) => x.IsCompleted()) > 0;
		}

		public int EnemiesDefeatedCount(out int totalDefeatedCount)
		{
			if (SectorStatus == Status.Completed)
			{
				totalDefeatedCount = MissionSectorDefinition.PVPEnemyAmount;
				return totalDefeatedCount;
			}
			totalDefeatedCount = 0;
			if (RandomizedMissions != null)
			{
				for (int i = 0; i < RandomizedMissions.Count; i++)
				{
					if (RandomizedMissions[i].IsPvpComplete())
					{
						totalDefeatedCount++;
					}
				}
			}
			return MissionSectorDefinition.PVPEnemyAmount;
		}

		public bool IsExhausted()
		{
			if (SectorStatus == Status.Exhausted)
			{
				return true;
			}
			if (RandomizedMissions != null)
			{
				for (int i = 0; i < RandomizedMissions.Count; i++)
				{
					if (!RandomizedMissions[i].IsCompleted())
					{
						return false;
					}
				}
			}
			return true;
		}

		public void SetSectorStatus(Status val)
		{
			SectorStatus = val;
			NotifyChange("GuildBattleSectorStatus");
		}

		public bool CanBeUnlocked(GuildBattleMapModel mapModel)
		{
			if (Prerequisites != null && mapModel != null)
			{
				for (int i = 0; i < Prerequisites.Length; i++)
				{
					if (Prerequisites[i] == null)
					{
						continue;
					}
					int id = Prerequisites[i].Id;
					GuildBattleMapSectorModel sectorModel = mapModel.GetSectorModel(id);
					if (sectorModel != null && (!sectorModel.CanBeUnlocked(mapModel) || !sectorModel.IsCompleted()))
					{
						if (i + 1 >= Prerequisites.Length || MissionSectorDefinition.AllPrerequisitesMustBeCompleted)
						{
							return false;
						}
					}
					else if (!MissionSectorDefinition.AllPrerequisitesMustBeCompleted)
					{
						break;
					}
				}
			}
			return true;
		}

		public GuildBattleMapMissionModel GetMissionModel(string uniqueMissionId)
		{
			for (int i = 0; i < RandomizedMissions.Count; i++)
			{
				GuildBattleMapMissionModel guildBattleMapMissionModel = RandomizedMissions[i];
				if (guildBattleMapMissionModel.Id == uniqueMissionId)
				{
					return guildBattleMapMissionModel;
				}
			}
			return null;
		}

		public int SolveMissionOrderNumber(string uniqueMissionId)
		{
			for (int i = 0; i < RandomizedMissions.Count; i++)
			{
				if (RandomizedMissions[i].Id == uniqueMissionId)
				{
					return i;
				}
			}
			return -1;
		}

		public void AddCompletionToArea(int areaIndex)
		{
			GvGSeasonModel gvGSeasonModel = base.root as GvGSeasonModel;
			if (areaIndex == -1 || StartIndexPerArea.Count <= areaIndex || StartIndexPerArea[areaIndex] == null || StartIndexPerArea[areaIndex].Count == 0)
			{
				base.Debug.LogError("Cannot find or empty, area index: " + areaIndex);
				return;
			}
			List<int> list = StartIndexPerArea[areaIndex];
			int num = CurrentBatchIndex + MissionSectorDefinition.MissionAmountPerPVPEnemy;
			if (list.Contains(num))
			{
				base.Debug.Log("New index already exists in area, newId: " + num);
				return;
			}
			int runningNumber = num + MissionSectorDefinition.MissionAmountPerPVPEnemy - 1;
			string uniqueMissionId = GuildBattleMapMissionModel.GenerateId(MissionPoolName, SectorId, runningNumber);
			if (GetMissionModel(uniqueMissionId) == null)
			{
				NotifyChange("GuildBattleAddCompletionToArea");
				return;
			}
			CurrentBatchIndex = num;
			list.Add(CurrentBatchIndex);
			if (OfflineManager.IsLoadDataManager)
			{
				global::DebugTWD.LogWarning("AddCompletionToArea. Проверить метод TryAssignPvpTeamForMission", DebugType.Warning);
				//StartGWBattle.Instance.guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.TryAssignPvpTeamForMission(missionModel);
			}
			for (int i = CurrentBatchIndex; i < CurrentBatchIndex + MissionSectorDefinition.MissionAmountPerPVPEnemy; i++)
			{
				GuildBattleMapMissionModel missionModel = GetMissionModel(GuildBattleMapMissionModel.GenerateId(MissionPoolName, SectorId, i));
				if (missionModel != null)
				{
					missionModel.AreaIndex = areaIndex;
					gvGSeasonModel.GuildWarModel.CurrentBattle.CurrentMapModel.TryAssignPvpTeamForMission(missionModel);
				}
			}
			UpdateAreaMissionsLists();
			NotifyChange("GuildBattleAddCompletionToArea");
		}

		public bool PvEMissionsInAreaCompleted(int areaIndex)
		{
			for (int i = StartIndexPerArea[areaIndex][StartIndexPerArea[areaIndex].Count - 1]; i < RandomizedMissions.Count && RandomizedMissions[i].AreaIndex == areaIndex; i++)
			{
				if (!RandomizedMissions[i].IsMissionPveComplete())
				{
					return false;
				}
			}
			return true;
		}

		public void SetupArea()
		{
			if (OfflineManager.IsLoadDataManager)
			{
				global::DebugTWD.LogWarning("SetupArea. Проверить метод TryAssignPvpTeamForMission", DebugType.Warning);
				//gvGSeasonModel.GuildWarModel.CurrentBattle.CurrentMapModel.TryAssignPvpTeamForMission(missionModel);
			}
			if (StartIndexPerArea == null || RandomizedMissions == null)
			{
				base.Debug.LogError("Main mission lists are NULL!");
				return;
			}
			if (MissionSectorDefinition == null)
			{
				base.Debug.LogError("Sector definition is NULL!");
				return;
			}
			GvGSeasonModel gvGSeasonModel = base.root as GvGSeasonModel;
			if (StartIndexPerArea.Count == 0)
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					num = ((i != 0) ? (i * MissionSectorDefinition.MissionAmountPerPVPEnemy) : 0);
					StartIndexPerArea.Add(new List<int>());
					for (int j = num; j < num + MissionSectorDefinition.MissionAmountPerPVPEnemy; j++)
					{
						GuildBattleMapMissionModel missionModel = GetMissionModel(GuildBattleMapMissionModel.GenerateId(MissionPoolName, SectorId, j));
						missionModel.AreaIndex = i;
						gvGSeasonModel.GuildWarModel.CurrentBattle.CurrentMapModel.TryAssignPvpTeamForMission(missionModel);
					}
					StartIndexPerArea[i].Add(num);
				}
				CurrentBatchIndex = num;
			}
			UpdateAreaMissionsLists();
		}

		public void UpdateAreaMissionsLists()
		{
			for (int i = 0; i < StartIndexPerArea.Count; i++)
			{
				if (StartIndexPerArea[i] == null || AreaMissions.Length != StartIndexPerArea.Count)
				{
					base.Debug.LogError("StartIndexPerArea or AreaMissions have incorrect length or NULL values!");
					break;
				}
				if (AreaMissions[i] == null)
				{
					AreaMissions[i] = new List<GuildBattleMissionQueueData>();
				}
				List<GuildBattleMissionQueueData> list = AreaMissions[i];
				GuildBattleMissionQueueData guildBattleMissionQueueData = null;
				for (int j = 0; j < StartIndexPerArea[i].Count; j++)
				{
					if (list.Count <= j || list[j] == null)
					{
						guildBattleMissionQueueData = new GuildBattleMissionQueueData(MissionSectorDefinition.MissionAmountPerPVPEnemy);
						list.Add(guildBattleMissionQueueData);
					}
					else
					{
						guildBattleMissionQueueData = list[j];
						guildBattleMissionQueueData.Clear();
						guildBattleMissionQueueData.SetMax(MissionSectorDefinition.MissionAmountPerPVPEnemy);
					}
					guildBattleMissionQueueData.Last = j + 1 >= StartIndexPerArea[i].Count;
					int num = StartIndexPerArea[i][j];
					for (int k = num; k < num + MissionSectorDefinition.MissionAmountPerPVPEnemy; k++)
					{
						string text = GuildBattleMapMissionModel.GenerateId(MissionPoolName, SectorId, k);
						GuildBattleMapMissionModel missionModel = GetMissionModel(text);
						if (missionModel != null)
						{
							guildBattleMissionQueueData.Add(missionModel);
							missionModel.MissionQueueIndex = j;
						}
						else
						{
							GvGSeasonModel gvGSeasonModel = base.root as GvGSeasonModel;
							base.Debug.LogError($"No mission with id {text} was found in the generated randomized missions with war id {gvGSeasonModel.GuildWarModel?.WarDefinitionId}");
						}
					}
				}
			}
		}

		public GuildBattleMapMissionModel GetPvpMissionFromAreaAndQueue(int areaIndex, int missionQueueIndex)
		{
			if (AreaMissions == null || areaIndex < 0 || areaIndex >= AreaMissions.Length || AreaMissions[areaIndex] == null)
			{
				return null;
			}
			List<GuildBattleMissionQueueData> list = AreaMissions[areaIndex];
			if (missionQueueIndex < 0 || missionQueueIndex >= list.Count || list[missionQueueIndex] == null)
			{
				return null;
			}
			return list[missionQueueIndex].EnemyMission;
		}
	}
}
