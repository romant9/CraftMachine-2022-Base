using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class MapContainerModel : TWDModelObject
	{
		public const string MapMissionAddedChanged = "MapMissionAdded";

		public const string MapMissionRemovedChanged = "MapMissionRemoved";

		public const string MapMissionGroupAddedChanged = "MapMissionGroupAdded";

		public const string MapMissionsCleared = "MapMissionsCleared";

		public const string OutpostTutorialMapId = "OutpostTutorial";

		public ModelList<MapMissionGroupModel> MapMissionGroups { get; set; }

		public MapMissionModel CurrentGrindMissionModel { get; set; }

		[IgnoreModelProperty]
		public MapMissionModel AttackTargetMissionModel { get; private set; }

		[IgnoreModelProperty]
		public MapMissionGroupModel AttackTargetMissionGroupModel { get; private set; }

		[JsonIgnore]
		public MapMissionModel LastPlayedMissionModel { get; set; }

		public void ClearMissionModelReferences()
		{
			AttackTargetMissionModel = null;
			AttackTargetMissionGroupModel = null;
			LastPlayedMissionModel = null;
			if (base.manager != null && base.manager.Player != null)
			{
				base.manager.Player.ResetIAttackTargetMapMission();
			}
		}

		public void ReturnFromCombat()
		{
			if (AttackTargetMissionModel != null && !AttackTargetMissionModel.IsGrindMission)
			{
				LastPlayedMissionModel = AttackTargetMissionModel;
			}
			else
			{
				LastPlayedMissionModel = null;
			}
			ClearAttackTargetMissionData();
		}

		public void ClearAttackTargetMissionData()
		{
			AttackTargetMissionModel = null;
			AttackTargetMissionGroupModel = null;
			if (base.manager != null && base.manager.Player != null)
			{
				base.manager.Player.ResetIAttackTargetMapMission();
			}
		}

		public bool MissionCompleted()
		{
			if (AttackTargetMissionModel != null)
			{
				return CompleteMission(AttackTargetMissionModel);
			}
			return true;
		}

		public void Clear()
		{
			MapMissionGroups = new ModelList<MapMissionGroupModel>();
			NotifyChange("MapMissionsCleared");
		}

		public override void Initialize()
		{
			base.Initialize();
			MapMissionGroups = new ModelList<MapMissionGroupModel>();
		}

		public MapMissionModel GetMissionModelForSpawnPoint(MissionSpawnPoint spawnPoint)
		{
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = GetMissionGroupModelForSpawnPointGroup(spawnPoint.OwningGroup);
			if (missionGroupModelForSpawnPointGroup != null)
			{
				int count = missionGroupModelForSpawnPointGroup.Missions.Count;
				for (int i = 0; i < count; i++)
				{
					MapMissionModel mapMissionModel = missionGroupModelForSpawnPointGroup.Missions[i];
					if (mapMissionModel.MissionId == spawnPoint.MissionId)
					{
						return mapMissionModel;
					}
				}
			}
			return null;
		}

		public bool IsMissionCompleted(MissionSpawnPoint spawnPoint)
		{
			MapMissionModel mapMissionModel = ((spawnPoint != null) ? base.manager.Player.MapContainerModel.GetMissionModelForSpawnPoint(spawnPoint) : null);
			if (mapMissionModel != null && (mapMissionModel.State == MapMissionState.Completed || mapMissionModel.State == MapMissionState.Respawning))
			{
				return true;
			}
			return false;
		}

		public void GetSeasonContentUnlocked(out List<string> mapIds, SeasonDefinition season = null)
		{
			MissionSpawnPointGroup missionSpawnPointGroup = null;
			mapIds = new List<string>();
			if (base.manager.Player.gameEconomyData.MapDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < base.manager.Player.gameEconomyData.MapDefinitions.Count; i++)
			{
				if (base.manager.Player.gameEconomyData.MapDefinitions[i] != null)
				{
					missionSpawnPointGroup = base.manager.Player.gameEconomyData.MapDefinitions[i];
					if (missionSpawnPointGroup.Category == MapCategory.Season && (season == null || missionSpawnPointGroup.Subcategory == season.Id) && base.manager.Player.UtcTimeStamp >= missionSpawnPointGroup.UnlockTimeMilliseconds)
					{
						mapIds.Add(missionSpawnPointGroup.MapId);
					}
				}
			}
		}

		public bool HasUnseenContent(SeasonDefinition season = null)
		{
			string text = "";
			List<string> mapIds = new List<string>();
			base.manager.Player.MapContainerModel.GetSeasonContentUnlocked(out mapIds, season);
			for (int i = 0; i < mapIds.Count; i++)
			{
				if (mapIds[i] != null)
				{
					text = mapIds[i];
					if (!base.manager.Player.Blackboard.IsToggleOn("Toggle.Episode." + text + ".Seen"))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool DoesSeasonTrialsNeedUpdate()
		{
			MapMissionGroupModel mapMissionGroupModel = null;
			if (MapMissionGroups != null)
			{
				for (int i = 0; i < MapMissionGroups.Count; i++)
				{
					mapMissionGroupModel = MapMissionGroups[i];
					if (mapMissionGroupModel != null && mapMissionGroupModel.MissionSpawnPointGroup == null)
					{
						base.Debug.LogWarning("SpawnPointGroup " + mapMissionGroupModel.MissionSpawnPointGroupId + " not found!");
					}
					if (mapMissionGroupModel != null && mapMissionGroupModel.MissionSpawnPointGroup != null && mapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Season && mapMissionGroupModel.NewerFeaturedDataExist() != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public MapMissionGroupModel GetMissionGroupModelForSpawnPointGroup(MissionSpawnPointGroup spawnPointGroup)
		{
			if (spawnPointGroup == null)
			{
				return null;
			}
			return GetMissionGroupModelForSpawnPointGroup(spawnPointGroup.Id);
		}

		public MapMissionGroupModel GetMissionGroupModelForSpawnPointGroup(int spawnPointGroupId)
		{
			int count = MapMissionGroups.Count;
			for (int i = 0; i < count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = MapMissionGroups[i];
				if (mapMissionGroupModel.MissionSpawnPointGroupId == spawnPointGroupId)
				{
					return mapMissionGroupModel;
				}
			}
			return null;
		}

		public MapMissionGroupModel GetMissionGroupModelForSpawnPointGroup(string spawnPointGroupDisplayName)
		{
			int count = MapMissionGroups.Count;
			for (int i = 0; i < count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = MapMissionGroups[i];
				MissionSpawnPointGroup missionSpawnPointGroup = mapMissionGroupModel.MissionSpawnPointGroup;
				if (missionSpawnPointGroup != null && missionSpawnPointGroup.DisplayName == spawnPointGroupDisplayName)
				{
					return mapMissionGroupModel;
				}
			}
			return null;
		}

		public List<MapMissionModel> GetUnlockingMissionModels(MapMissionModel mapMissionModel)
		{
			List<MapMissionModel> list = new List<MapMissionModel>();
			MissionSpawnPoint missionSpawnPoint = mapMissionModel.MissionSpawnPoint;
			if (missionSpawnPoint != null)
			{
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = GetMissionGroupModelForSpawnPointGroup(missionSpawnPoint.OwningGroup);
				int count = missionGroupModelForSpawnPointGroup.Missions.Count;
				for (int i = 0; i < count; i++)
				{
					MapMissionModel mapMissionModel2 = missionGroupModelForSpawnPointGroup.Missions[i];
					if (mapMissionModel2.MissionSpawnPoint != null && mapMissionModel2.MissionSpawnPoint.SpawnPointsToUnlock != null && mapMissionModel2.MissionSpawnPoint.SpawnPointsToUnlock.Contains(missionSpawnPoint))
					{
						list.Add(mapMissionModel2);
					}
				}
			}
			return list;
		}

		public List<MissionSpawnPoint> GetUnlockingMissionSpawnPoints(MissionSpawnPoint missionSpawnPoint)
		{
			List<MissionSpawnPoint> list = new List<MissionSpawnPoint>();
			foreach (MissionSpawnPoint missionSpawnPoint2 in missionSpawnPoint.OwningGroup.MissionSpawnPoints)
			{
				if (missionSpawnPoint2.SpawnPointsToUnlock != null && missionSpawnPoint2.SpawnPointsToUnlock.Contains(missionSpawnPoint))
				{
					list.Add(missionSpawnPoint2);
				}
			}
			return list;
		}

		public MapMissionGroupModel SpawnMissionGroup(MissionSpawnPointGroup group)
		{
			MapMissionGroupModel mapMissionGroupModel = GetMissionGroupModelForSpawnPointGroup(group);
			if (mapMissionGroupModel == null)
			{
				mapMissionGroupModel = new MapMissionGroupModel();
				mapMissionGroupModel.Initialize();
				mapMissionGroupModel.SetManager(base.manager);
				if (base.manager.IsStarted)
				{
					mapMissionGroupModel.Start();
				}
				mapMissionGroupModel.MissionSpawnPointGroupId = group.Id;
				MapMissionGroups.Add(mapMissionGroupModel);
				NotifyChange("MapMissionGroupAdded", mapMissionGroupModel);
			}
			CheckForModifications(group);
			return mapMissionGroupModel;
		}

		public int GetEpisodeOrder(MissionSpawnPointGroup spawnPointGroup)
		{
			if (spawnPointGroup.EpisodeDifficultyLevel != 1)
			{
				spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupForDifficultyLevel1(spawnPointGroup);
			}
			if (spawnPointGroup == null)
			{
				return -1;
			}
			QuestDefinition[] questDefinitions = base.manager.Player.gameEconomyData.QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				if (questDefinition.IsAvailable && questDefinition.GetMissionQuestMapId() == spawnPointGroup.MapId)
				{
					return questDefinition.Order;
				}
			}
			return -1;
		}

		public int GetHighestAvailableEpisodeOrder()
		{
			int num = -1;
			QuestDefinition[] questDefinitions = base.manager.Player.gameEconomyData.QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				if (!questDefinition.IsAvailable)
				{
					continue;
				}
				MissionSpawnPointGroup spawnPointGroupByMapId = base.manager.Player.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupByMapId(questDefinition.GetMissionQuestMapId());
				if (spawnPointGroupByMapId != null)
				{
					MapMissionGroupModel missionGroupModelForSpawnPointGroup = GetMissionGroupModelForSpawnPointGroup(spawnPointGroupByMapId);
					if (missionGroupModelForSpawnPointGroup != null && !missionGroupModelForSpawnPointGroup.IsLocked)
					{
						num = UtilsMath.Max(num, questDefinition.Order);
					}
				}
			}
			return num;
		}

		public bool CanSpawnGrind(MapMissionGroupModel groupModel)
		{
			if (groupModel.IsLocked)
			{
				return false;
			}
			int highestAvailableEpisodeOrder = GetHighestAvailableEpisodeOrder();
			int episodeOrder = GetEpisodeOrder(groupModel.MissionSpawnPointGroup);
			return highestAvailableEpisodeOrder - episodeOrder < base.manager.Player.gameEconomyData.ConfigData.NumEpisodesContainingGrindMissions;
		}

		public void CheckOutpostTutorialGroupInstances()
		{
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			MissionSpawnPoint missionSpawnPoint;
			try
			{
				missionSpawnPoint = gameEconomyData.MissionSpawnPointData.FindFirstSpawnPointByMissionId(gameEconomyData.ConfigData.OutpostTutorialMissionId);
			}
			catch { return; }
			if (missionSpawnPoint == null)
			{
				return;
			}
			MapMissionModel missionModelForSpawnPoint = GetMissionModelForSpawnPoint(missionSpawnPoint);
			if (missionModelForSpawnPoint != null)
			{
				return;
			}
			missionModelForSpawnPoint = CreateMissionModel(missionSpawnPoint);
			if (missionModelForSpawnPoint != null)
			{
				MissionSpawnPointGroup spawnPointGroupByMapId = gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupByMapId("OutpostTutorial");
				if (spawnPointGroupByMapId != null)
				{
					GetMissionGroupModelForSpawnPointGroup(spawnPointGroupByMapId)?.AddMission(missionModelForSpawnPoint);
				}
			}
		}

		public void ValidateEpisodes()
		{
			int highestAvailableEpisodeOrder = GetHighestAvailableEpisodeOrder();
			List<MapMissionGroupModel> list = new List<MapMissionGroupModel>();
			for (int i = 0; i < MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = MapMissionGroups[i];
				int episodeOrder = GetEpisodeOrder(mapMissionGroupModel.MissionSpawnPointGroup);
				if (mapMissionGroupModel.IsWeeklyChallenge || mapMissionGroupModel.IsInApocalyptiWeeklyChallenge || episodeOrder < 0)
				{
					continue;
				}
				if (mapMissionGroupModel.IsLocked)
				{
					list.Add(mapMissionGroupModel);
				}
				else
				{
					if (episodeOrder >= highestAvailableEpisodeOrder)
					{
						continue;
					}
					for (int j = 0; j < mapMissionGroupModel.Missions.Models.Count; j++)
					{
						MapMissionModel mapMissionModel = mapMissionGroupModel.Missions.Models[i];
						if (mapMissionModel.MissionSpawnPoint != null && mapMissionModel.MissionSpawnPoint.IsExplicit)
						{
							mapMissionModel.State = MapMissionState.Completed;
						}
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				MapMissionGroups.Remove(list[k]);
			}
		}

		private void CheckForModifications(MissionSpawnPointGroup group)
		{
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = GetMissionGroupModelForSpawnPointGroup(group);
			if (missionGroupModelForSpawnPointGroup == null || missionGroupModelForSpawnPointGroup.IsLocked || missionGroupModelForSpawnPointGroup.IsDisabledOnGED || group.Category == MapCategory.Grind || group.Category == MapCategory.GuildBattle || group.Category == MapCategory.GuildBoss || group.Category == MapCategory.GuildBossPVE || group.Category == MapCategory.GuildBossPVP)
			{
				return;
			}
			List<MapMissionModel> list = new List<MapMissionModel>();
			for (int i = 0; i < missionGroupModelForSpawnPointGroup.Missions.Count; i++)
			{
				MapMissionModel mapMissionModel = missionGroupModelForSpawnPointGroup.Missions[i];
				bool num = mapMissionModel.MissionSpawnPoint == null;
				bool flag = mapMissionModel.MissionSpawnPointGroup == null || mapMissionModel.MissionSpawnPointGroup.Id != missionGroupModelForSpawnPointGroup.MissionSpawnPointGroupId;
				if (num || flag)
				{
					list.Add(mapMissionModel);
				}
				else if (mapMissionModel.MissionSpawnPoint.IsExplicit && mapMissionModel.MissionId != mapMissionModel.MissionSpawnPoint.MissionId)
				{
					mapMissionModel.MissionId = mapMissionModel.MissionSpawnPoint.MissionId;
				}
			}
			while (list.Count > 0)
			{
				MapMissionModel mapMissionModel2 = list[0];
				list.RemoveAt(0);
				missionGroupModelForSpawnPointGroup.RemoveMission(mapMissionModel2);
			}
			for (int j = 0; j < group.MissionSpawnPoints.Count; j++)
			{
				MissionSpawnPoint missionSpawnPoint = group.MissionSpawnPoints[j];
				if (missionSpawnPoint != null && GetMissionModelForSpawnPoint(missionSpawnPoint) == null)
				{
					MapMissionModel mapMissionModel3 = SpawnMission(missionSpawnPoint);
					if (mapMissionModel3 == null)
					{
						continue;
					}
					if (!HasUncompletedParents(mapMissionModel3))
					{
						mapMissionModel3.ForceState(MapMissionState.Unlocked);
					}
					if (mapMissionModel3.MissionSpawnPoint.SpawnPointsToUnlock == null)
					{
						continue;
					}
					foreach (MissionSpawnPoint item in mapMissionModel3.MissionSpawnPoint.SpawnPointsToUnlock)
					{
						MapMissionModel missionModelForSpawnPoint = GetMissionModelForSpawnPoint(item);
						if (missionModelForSpawnPoint != null && missionModelForSpawnPoint.State == MapMissionState.Completed)
						{
							mapMissionModel3.ForceState(MapMissionState.Completed);
							break;
						}
					}
				}
				else if (missionSpawnPoint != null)
				{
					MapMissionModel missionModelForSpawnPoint2 = GetMissionModelForSpawnPoint(missionSpawnPoint);
					if (missionModelForSpawnPoint2 != null && missionModelForSpawnPoint2.State == MapMissionState.Locked && !HasUncompletedParents(missionModelForSpawnPoint2) && (missionModelForSpawnPoint2.MissionSpawnPointGroup == null || missionModelForSpawnPoint2.MissionSpawnPointGroup.Category != MapCategory.Survival))
					{
						missionModelForSpawnPoint2.ForceState(MapMissionState.Unlocked);
					}
				}
			}
		}

		public bool SpawnGrindMission(int grindButtonDefinitionId)
		{
			MissionSpawnPoint missionSpawnPoint = SelectRandomGrindMission();
			if (missionSpawnPoint == null)
			{
				return false;
			}
			GrindButtonDefinition grindButtonDefinition = base.gameEconomyData.GetGrindButtonDefinition(grindButtonDefinitionId);
			if (grindButtonDefinition == null)
			{
				return false;
			}
			CurrentGrindMissionModel = SpawnMission(missionSpawnPoint);
			CurrentGrindMissionModel.MissionLevel = grindButtonDefinition.GetMissionLevel(base.manager.Player);
			CurrentGrindMissionModel.LootTag = grindButtonDefinition.LootTag;
			CurrentGrindMissionModel.DropContext = grindButtonDefinition.DropContext;
			CurrentGrindMissionModel.GrindButtonDefinitionId = grindButtonDefinitionId;
			return true;
		}

		public void SpawnSeasonEpisodes()
		{
			for (int i = 0; i < MapMissionGroups.Count; i++)
			{
				if (MapMissionGroups[i].MissionSpawnPointGroup.Category == MapCategory.Season)
				{
					SpawnMissionsForGroup(MapMissionGroups[i].MissionSpawnPointGroup);
				}
			}
		}

		public void SpawnEndlessModeMissions()
		{
			for (int i = 0; i < MapMissionGroups.Count; i++)
			{
				if (MapMissionGroups[i].MissionSpawnPointGroup.Category == MapCategory.Endless)
				{
					SpawnMissionsForGroup(MapMissionGroups[i].MissionSpawnPointGroup);
				}
			}
		}

		public bool SpawnMissionsForGroup(MissionSpawnPointGroup spawnPointGroup)
		{
			if (spawnPointGroup.Category == MapCategory.Grind)
			{
				return false;
			}
			if (GetMissionGroupModelForSpawnPointGroup(spawnPointGroup) == null)
			{
				base.manager.Debug.LogError("No group '" + spawnPointGroup?.ToString() + "' cannot spawn missions!");
				return false;
			}
			for (int i = 0; i < spawnPointGroup.MissionSpawnPoints.Count; i++)
			{
				MissionSpawnPoint spawnPoint = spawnPointGroup.MissionSpawnPoints[i];
				SpawnMission(spawnPoint);
			}
			return true;
		}

		public MapMissionGroupModel GetMissionGroupModelThatContains(MapMissionModel mission)
		{
			int count = MapMissionGroups.Count;
			for (int i = 0; i < count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = MapMissionGroups[i];
				if (mapMissionGroupModel.Missions != null && mapMissionGroupModel.Missions.Contains(mission))
				{
					return mapMissionGroupModel;
				}
			}
			return null;
		}

		public MapMissionModel GetMission(string name)
		{
			for (int i = 0; i < MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = MapMissionGroups[i];
				for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
				{
					if (mapMissionGroupModel.Missions[j].MissionData != null && mapMissionGroupModel.Missions[j].MissionData.DisplayTextID == name)
					{
						return mapMissionGroupModel.Missions[j];
					}
				}
			}
			return null;
		}

		public int GetMissionIndex(MapMissionModel mission)
		{
			MapMissionGroupModel missionGroupModelThatContains = GetMissionGroupModelThatContains(mission);
			if (missionGroupModelThatContains != null)
			{
				for (int i = 0; i < missionGroupModelThatContains.Missions.Count; i++)
				{
					if (missionGroupModelThatContains.Missions[i] == mission)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int GetGroupIndex(MapMissionGroupModel group)
		{
			for (int i = 0; i < MapMissionGroups.Count; i++)
			{
				if (MapMissionGroups[i] == group)
				{
					return i;
				}
			}
			return -1;
		}

		public MapMissionModel CreateMissionModel(MissionSpawnPoint spawnPoint)
		{
			MapMissionModel mapMissionModel = CreateMissionModelInternal(spawnPoint);
			mapMissionModel.InitExplicitMission(spawnPoint);
			return mapMissionModel;
		}

		private MapMissionModel CreateMissionModelInternal(MissionSpawnPoint spawnPoint)
		{
			MapMissionModel mapMissionModel = new MapMissionModel();
			mapMissionModel.MissionSpawnPointGroupId = spawnPoint.OwningGroup.Id;
			mapMissionModel.IsDeadly = spawnPoint.IsDeadly;
			mapMissionModel.CostIndex = spawnPoint.OwningGroup.CostIndex;
			List<MissionSpawnPoint> unlockingMissionSpawnPoints = GetUnlockingMissionSpawnPoints(spawnPoint);
			mapMissionModel.State = ((unlockingMissionSpawnPoints.Count == 0) ? MapMissionState.Unlocked : MapMissionState.Locked);
			mapMissionModel.MissionLevel = spawnPoint.MissionLevel;
			mapMissionModel.Initialize();
			mapMissionModel.SetManager(base.manager);
			mapMissionModel.RecalculateWeeklyChallengeMissionLevel();
			if (base.manager.IsStarted)
			{
				mapMissionModel.Start();
			}
			return mapMissionModel;
		}

		private MissionSpawnPoint SelectRandomGrindMission()
		{
			ConfigData configData = base.gameEconomyData.ConfigData;
			int num = 0;
			List<MissionSpawnPointGroup> list = new List<MissionSpawnPointGroup>();
			for (int i = 0; i < configData.GrindMissionMaps.Count; i++)
			{
				MissionSpawnPointGroup spawnPointGroupByMapId = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupByMapId(configData.GrindMissionMaps[i]);
				if (spawnPointGroupByMapId != null && configData.GrindMissionMinPlayerLevels.Count > i && configData.GrindMissionMinPlayerLevels[i] <= base.manager.Player.Level && configData.GrindMissionMaxPlayerLevels.Count > i && configData.GrindMissionMaxPlayerLevels[i] >= base.manager.Player.Level)
				{
					list.Add(spawnPointGroupByMapId);
					num += spawnPointGroupByMapId.MissionSpawnPoints.Count;
				}
			}
			if (num == 0)
			{
				return null;
			}
			int num2 = base.manager.Player.PlayerRandom.GetRandomInRange(0, num - 1);
			for (int j = 0; j < list.Count; j++)
			{
				if (num2 < list[j].MissionSpawnPoints.Count)
				{
					return list[j].MissionSpawnPoints[num2];
				}
				num2 -= list[j].MissionSpawnPoints.Count;
			}
			base.Debug.LogError("Failed to find grind mission on player level " + base.manager.Player.Level);
			return null;
		}

		public MapMissionModel SpawnMission(MissionSpawnPoint spawnPoint)
		{
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = GetMissionGroupModelForSpawnPointGroup(spawnPoint.OwningGroup);
			if (missionGroupModelForSpawnPointGroup == null)
			{
				base.manager.Debug.LogError("No group '" + spawnPoint.OwningGroup?.ToString() + "' for mission '" + spawnPoint?.ToString() + "'!");
				return null;
			}
			MapMissionModel mapMissionModel = GetMissionModelForSpawnPoint(spawnPoint);
			if (mapMissionModel == null)
			{
				mapMissionModel = CreateMissionModel(spawnPoint);
				if (spawnPoint.OwningGroup.Category != MapCategory.Grind)
				{
					missionGroupModelForSpawnPointGroup.AddMission(mapMissionModel);
					NotifyChange("MapMissionAdded", mapMissionModel);
				}
			}
			return mapMissionModel;
		}

		public void RemoveMissionsForGroup(MapMissionGroupModel group)
		{
			group.RemoveMissions();
		}

		public bool HasLockedParents(MapMissionModel mapMissionModel)
		{
			List<MapMissionModel> unlockingMissionModels = GetUnlockingMissionModels(mapMissionModel);
			if (unlockingMissionModels != null && unlockingMissionModels.Count > 0)
			{
				foreach (MapMissionModel item in unlockingMissionModels)
				{
					if (item.State == MapMissionState.Locked)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasUncompletedParents(MapMissionModel mapMissionModel)
		{
			List<MapMissionModel> unlockingMissionModels = GetUnlockingMissionModels(mapMissionModel);
			if (unlockingMissionModels != null && unlockingMissionModels.Count > 0)
			{
				foreach (MapMissionModel item in unlockingMissionModels)
				{
					if (item.State == MapMissionState.Locked || item.State == MapMissionState.Unlocked || item.State == MapMissionState.Respawning)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CompleteMission(MapMissionModel mapMissionModel)
		{
			bool flag = false;
			if (mapMissionModel != null && mapMissionModel.manager != null && mapMissionModel.MissionSpawnPoint != null)
			{
				if (mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Survival)
				{
					if (base.manager.Player.WeeklySurvival != null)
					{
						if (mapMissionModel.SolveOrderNumberInGroup() == base.manager.Player.WeeklySurvival.NextMissionOrderNumber)
						{
							base.manager.Player.WeeklySurvival.MoveToNextMission();
							MapMissionGroupModel currentOrNextMapMissionGroupModel = base.manager.Player.WeeklySurvival.GetCurrentOrNextMapMissionGroupModel();
							if (currentOrNextMapMissionGroupModel != null)
							{
								foreach (MapMissionModel mission in currentOrNextMapMissionGroupModel.Missions)
								{
									if (mission.UpdateSurvivalMapState())
									{
										mission.NotifyChange("StateChanged");
									}
								}
							}
						}
						else
						{
							flag = true;
						}
					}
				}
				else if (mapMissionModel.MissionSpawnPointGroup.Category != MapCategory.Season && mapMissionModel.MissionSpawnPoint.SpawnPointsToUnlock != null)
				{
					foreach (MissionSpawnPoint item in mapMissionModel.MissionSpawnPoint.SpawnPointsToUnlock)
					{
						MapMissionModel missionModelForSpawnPoint = GetMissionModelForSpawnPoint(item);
						if (missionModelForSpawnPoint != null && !HasUncompletedParents(missionModelForSpawnPoint))
						{
							missionModelForSpawnPoint.Unlock();
						}
					}
				}
			}
			return !flag;
		}

		public TWDModelResult AttackMission(MapMissionModel mapMissionModel, MapMissionGroupModel mapMissionGroupModel)
		{
			bool num = mapMissionGroupModel?.IsWeeklySurvival ?? false;
			bool flag = mapMissionGroupModel != null && mapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Endless;
			bool flag2 = false;
			if (flag && base.manager.Player.EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				flag2 = true;
			}
			if (num)
			{
				if (base.manager.Player.SurvivorContainer.CombatSurvivors.Count < 1)
				{
					return TWDModelResult.NotEnoughSurvivors;
				}
			}
			else
			{
				if (base.manager.Player.SurvivorContainer.CombatSurvivors.Count < mapMissionModel.MissionData.MaxTeamSize)
				{
					return TWDModelResult.NotEnoughSurvivors;
				}
				if (mapMissionGroupModel != null && mapMissionGroupModel.IsWeeklyChallenge && mapMissionModel.IsMasterMission && base.manager.GameEconomyData.ConfigData.ChallangeMasterMissionCouncilLevelUnlock > base.manager.Player.CouncilLevel)
				{
					return TWDModelResult.ChallengeMasterMissionNotUnlocked;
				}
				if (flag)
				{
					string text = "";
					text = (flag2 ? base.manager.Player.EndlessModeManager.CurrentEndlessModeCalendarDefinition.ExpertMapID : base.manager.Player.EndlessModeManager.CurrentEndlessModeCalendarDefinition.MapID);
					EndlessModeManagerModel endlessModeManager = base.manager.Player.EndlessModeManager;
					if (text != mapMissionModel.MissionId)
					{
						return TWDModelResult.Error;
					}
					if (!flag2 && endlessModeManager.CurrentGoldAttemptCount >= endlessModeManager.EndlessModeConfig.DailyGoldAttemptCount && endlessModeManager.EndlessModeConfig.DailyGoldAttemptCount > 0 && base.manager.Player.GetCurrency(CurrencyType.EndlessPassToken).Value < endlessModeManager.EndlessModeConfig.MissionBaseCost)
					{
						return TWDModelResult.Error;
					}
					if (flag2 && endlessModeManager.CurrentExpertGoldAttemptCount >= endlessModeManager.EndlessModeConfig.DailyGoldExpertAttemptCount && endlessModeManager.EndlessModeConfig.DailyGoldExpertAttemptCount > 0 && base.manager.Player.GetCurrency(CurrencyType.EndlessPassExpertToken).Value < endlessModeManager.EndlessModeConfig.MissionBaseCost)
					{
						return TWDModelResult.Error;
					}
					if (endlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert && !endlessModeManager.HasValidCombatActorsForExpertMode())
					{
						return TWDModelResult.Error;
					}
				}
			}
			Cashier cashier = null;
			cashier = ((!flag2) ? mapMissionModel.GetStartMissionCashier() : mapMissionModel.GetStartMissionExpertModeCashier());
			Cashier cashier2 = ((mapMissionModel != null) ? cashier : null);
			if (cashier2 != null && !cashier2.CanAffordWithDiamonds())
			{
				return TWDModelResult.NotEnoughCurrency;
			}
			if (mapMissionModel != null)
			{
				AttackTargetMissionModel = mapMissionModel;
				AttackTargetMissionGroupModel = mapMissionGroupModel;
				if (AttackTargetMissionModel == null)
				{
					return TWDModelResult.Error;
				}
				if (!CheckSurvivorsForAttack())
				{
					return TWDModelResult.Error;
				}
				if (AttackTargetMissionGroupModel.IsLocked)
				{
					return TWDModelResult.Error;
				}
				bool flag3 = false;
				if (AttackTargetMissionModel.MissionSpawnPointGroup != null)
				{
					flag3 = AttackTargetMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && AttackTargetMissionModel.IsLastInGroup && AttackTargetMissionModel.State == MapMissionState.Completed;
				}
				if (flag3)
				{
					return TWDModelResult.Error;
				}
				base.manager.Player.ShouldConsumeMissionCurrency = true;
				base.manager.Player.GuildBattlePlayer.RetryMission = false;
				return TWDModelResult.OK;
			}
			return TWDModelResult.InvalidPosition;
		}

		private bool CheckSurvivorsForAttack()
		{
			if (AttackTargetMissionModel.IsInWeeklySurvival)
			{
				bool flag = true;
				SurvivorContainerModel survivorContainer = base.manager.Player.SurvivorContainer;
				for (int i = 0; i < survivorContainer.CombatSurvivors.Count; i++)
				{
					SurvivorModel survivorModel = survivorContainer.CombatSurvivors[i];
					SurvivalCharacterStateModel survivorStateInSurvivalMode = survivorContainer.SurvivalCharacters.GetSurvivorStateInSurvivalMode(survivorModel);
					if (survivorStateInSurvivalMode == null)
					{
						return false;
					}
					flag &= !survivorStateInSurvivalMode.OutOfAction;
					flag &= survivorContainer.Survivors.Contains(survivorModel);
					flag &= !survivorModel.IsUpgrading();
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			bool flag2 = true;
			bool disableOutpostHeroLimits = base.manager.GameEconomyData.ConfigData.DisableOutpostHeroLimits;
			SurvivorContainerModel survivorContainer2 = base.manager.Player.SurvivorContainer;
			for (int j = 0; j < survivorContainer2.CombatSurvivors.Count; j++)
			{
				SurvivorModel survivorModel2 = survivorContainer2.CombatSurvivors[j];
				flag2 &= disableOutpostHeroLimits || !survivorContainer2.IsOutpostDefending(survivorModel2);
				flag2 &= survivorModel2.InjuryType == InjuryType.None;
				flag2 &= survivorContainer2.Survivors.Contains(survivorModel2);
				flag2 &= !survivorModel2.IsUpgrading();
				if (!flag2)
				{
					return false;
				}
			}
			return true;
		}

		public void SetAttackedMissionForHotfix(MapMissionModel missionModel)
		{
			AttackTargetMissionModel = missionModel;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void AddSelectedMissionAnalyticsProperties(ref Dictionary<string, string> properties)
		{
			if (AttackTargetMissionModel == null || AttackTargetMissionModel.manager == null || base.manager == null || base.manager.Player == null || base.manager.Player.MapContainerModel == null || base.manager.GameEconomyData == null)
			{
				return;
			}
			MissionData missionData = AttackTargetMissionModel.MissionData;
			if (missionData == null)
			{
				return;
			}
			string text = "mission_id";
			string key = "season_number";
			string text2 = "episode_number";
			string text3 = "mission_number";
			string key2 = "mission_iteration";
			string item = "mission_difficulty";
			string text4 = "mission_flavour";
			string text5 = "mission_is_deadly";
			string text6 = "mission_kind";
			string text7 = "episode_difficulty";
			string text8 = "episode_name";
			string item2 = "weekly_challenge_stars";
			string text9 = "mission_level";
			string key3 = "mission_code";
			string key4 = "grind_difficulty";
			foreach (string item3 in new List<string>
			{
				text, text2, text3, item, text4, text5, text6, text7, text8, item2,
				text9
			})
			{
				if (properties.ContainsKey(item3))
				{
					properties.Remove(item3);
				}
			}
			properties.Add(text, missionData.Id);
			MapCategory mapCategory = ((AttackTargetMissionModel.MissionSpawnPointGroup != null) ? AttackTargetMissionModel.MissionSpawnPointGroup.Category : MapCategory.None);
			MapMissionGroupModel missionGroupModelThatContains = base.manager.Player.MapContainerModel.GetMissionGroupModelThatContains(AttackTargetMissionModel);
			if (AttackTargetMissionModel.MissionSpawnPointGroup == null || AttackTargetMissionModel.MissionSpawnPointGroup.MapId == null || missionGroupModelThatContains == null)
			{
				return;
			}
			int result = -1;
			if (mapCategory == MapCategory.Season && AttackTargetMissionModel.MissionSpawnPointGroup.MapId.Length > 1)
			{
				int.TryParse(AttackTargetMissionModel.MissionSpawnPointGroup.MapId.Substring(1, 1), out result);
			}
			int num = GetEpisodeIndex(AttackTargetMissionModel) + 1;
			int num2 = base.manager.Player.MapContainerModel.GetMissionIndex(AttackTargetMissionModel) + 1;
			if (mapCategory == MapCategory.Story || mapCategory == MapCategory.Season)
			{
				properties.Add(text2, num.ToString());
				properties.Add(text3, num2.ToString());
				properties.Add(text7, missionGroupModelThatContains.MissionSpawnPointGroup.EpisodeDifficultyLevel.ToString());
				properties.Add(text8, missionGroupModelThatContains.MissionSpawnPointGroup.MapId);
				if (mapCategory == MapCategory.Season)
				{
					properties.Add(key, result.ToString());
					properties.Add(key2, AttackTargetMissionModel.CompletionTimes.ToString());
					properties.Add(key3, "S" + result.ToString("D2") + "E" + num.ToString("D2") + "M" + num2.ToString("D2"));
				}
				if (mapCategory == MapCategory.Story)
				{
					properties.Add(key3, "E" + num.ToString("D2") + "M" + num2.ToString("D2"));
				}
			}
			if (mapCategory == MapCategory.Challenge || mapCategory == MapCategory.ApocalypticChallenge)
			{
				properties.Add(text3, num2.ToString());
				properties.Add(key3, missionGroupModelThatContains.MissionSpawnPointGroup.MapId + num2.ToString("D2"));
			}
			if (mapCategory == MapCategory.Grind)
			{
				properties.Add(key4, base.manager.GameEconomyData.GetGrindButtonDefinition(AttackTargetMissionModel.GrindButtonDefinitionId).GrindDifficulty.ToString());
				properties.Add(key3, "G" + AttackTargetMissionModel.MissionLevel.ToString("D2"));
			}
			string value = (AttackTargetMissionModel.IsDeadly ? "1" : "0");
			properties.Add(text5, value);
			string missionKind = GetMissionKind();
			properties.Add(text6, missionKind);
			if (base.manager.Player != null && !string.IsNullOrEmpty(base.manager.Player.SelectedMissionFlavor))
			{
				MissionFlavorData missionFlavorData = base.manager.GameEconomyData.GetMissionFlavorData(base.manager.Player.SelectedMissionFlavor);
				if (missionFlavorData != null)
				{
					properties.Add(text4, missionFlavorData.Name);
				}
			}
			properties.Add(text9, AttackTargetMissionModel.MissionLevel.ToString());
		}

		public string GetMissionKind()
		{
			if (AttackTargetMissionModel != null && AttackTargetMissionModel.MissionSpawnPointGroup != null)
			{
				switch (AttackTargetMissionModel.MissionSpawnPointGroup.Category)
				{
				case MapCategory.Story:
					return "story";
				case MapCategory.Grind:
					return "grind";
				case MapCategory.ApocalypticChallenge:
					return "weekly_apocalyptic_challenge";
				case MapCategory.Challenge:
					return "weekly_challenge";
				case MapCategory.Season:
					return "season";
				case MapCategory.Survival:
					return "survival";
				case MapCategory.None:
					return "none";
				}
			}
			return "";
		}

		public MapMissionGroupModel GetHarderVersion(MapMissionGroupModel groupModel)
		{
			int spawnPointGroupId = groupModel.MissionSpawnPointGroupId + 1;
			return GetMissionGroupModelForSpawnPointGroup(spawnPointGroupId);
		}

		public bool HasCompletedHarderEpisodeMission()
		{
			for (int i = 0; i < MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = MapMissionGroups[i];
				MissionSpawnPointGroup missionSpawnPointGroup = mapMissionGroupModel.MissionSpawnPointGroup;
				if (missionSpawnPointGroup == null || missionSpawnPointGroup.EpisodeDifficultyLevel <= 1)
				{
					continue;
				}
				for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
				{
					if (mapMissionGroupModel.Missions[j].IsCompleted)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetEpisodeIndex(MapMissionModel missionModel)
		{
			int num = -1;
			if (missionModel != null && missionModel.MissionSpawnPointGroup != null)
			{
				for (int i = 0; i < base.manager.GameEconomyData.MapDefinitions.Count; i++)
				{
					MissionSpawnPointGroup missionSpawnPointGroup = base.manager.GameEconomyData.MapDefinitions[i];
					if (missionSpawnPointGroup.Category == missionModel.MissionSpawnPointGroup.Category)
					{
						num++;
					}
					if (missionSpawnPointGroup.MapId == missionModel.MissionSpawnPointGroup.MapId)
					{
						return num;
					}
				}
			}
			return num;
		}
	}
}
