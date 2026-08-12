using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class MapMissionGroupModel : TWDModelObject, IUserViewableObject, IAttackTargetModel
	{
		public const string MapMissionAddedChanged = "MapMissionAdded";

		public const string MapMissionRemovedChanged = "MapMissionRemoved";

		private bool hasBeenViewed;

		public int MissionSpawnPointGroupId { get; set; }

		public int MissionHighlightVersion { get; set; }

		public bool HasBeenViewed
		{
			get
			{
				return hasBeenViewed;
			}
			set
			{
				if (hasBeenViewed != value)
				{
					hasBeenViewed = value;
					NotifyChange("HasBeenViewed");
				}
			}
		}

		[JsonIgnore]
		public bool HasRequiredCarLevel => true;

		[JsonIgnore]
		public bool HasRequiredCouncilLevel => true;

		[JsonIgnore]
		public long IsNextToUnlockTime
		{
			get
			{
				if (base.manager.Player.gameEconomyData.MapDefinitions != null)
				{
					long num = long.MaxValue;
					string text = "";
					for (int i = 0; i < base.manager.Player.gameEconomyData.MapDefinitions.Count; i++)
					{
						MissionSpawnPointGroup missionSpawnPointGroup = base.manager.Player.gameEconomyData.MapDefinitions[i];
						if (missionSpawnPointGroup.UnlockTimeMilliseconds > base.manager.Player.UtcTimeStamp && missionSpawnPointGroup.UnlockTimeMilliseconds - base.manager.Player.UtcTimeStamp < num)
						{
							num = missionSpawnPointGroup.UnlockTimeMilliseconds - base.manager.Player.UtcTimeStamp;
							text = missionSpawnPointGroup.MapId;
						}
					}
					if (text == MissionSpawnPointGroup.MapId)
					{
						return num;
					}
				}
				return -1L;
			}
		}

		[JsonIgnore]
		public MissionHighlight IsFeaturedData
		{
			get
			{
				if (base.manager.Player.gameEconomyData.MissionHighlights != null)
				{
					for (int i = 0; i < base.manager.Player.gameEconomyData.MissionHighlights.Length; i++)
					{
						MissionHighlight missionHighlight = base.manager.Player.gameEconomyData.MissionHighlights[i];
						if (missionHighlight.MapId == MissionSpawnPointGroup.MapId && missionHighlight.IsActive(base.manager.Player.UtcTimeStamp))
						{
							return missionHighlight;
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public MissionHighlight LatestFeaturedData
		{
			get
			{
				MissionHighlight result = null;
				if (base.manager.Player.gameEconomyData.MissionHighlights != null)
				{
					for (int i = 0; i < base.manager.Player.gameEconomyData.MissionHighlights.Length; i++)
					{
						MissionHighlight missionHighlight = base.manager.Player.gameEconomyData.MissionHighlights[i];
						if (missionHighlight.MapId == MissionSpawnPointGroup.MapId && (missionHighlight.IsActive(base.manager.Player.UtcTimeStamp) || missionHighlight.WasActiveBefore(base.manager.Player.UtcTimeStamp)))
						{
							result = missionHighlight;
						}
					}
				}
				return result;
			}
		}

		[JsonIgnore]
		public MissionHighlight NextFeaturedData
		{
			get
			{
				if (base.manager.Player.gameEconomyData.MissionHighlights != null)
				{
					for (int i = 0; i < base.manager.Player.gameEconomyData.MissionHighlights.Length; i++)
					{
						MissionHighlight missionHighlight = base.manager.Player.gameEconomyData.MissionHighlights[i];
						if (missionHighlight.MapId == MissionSpawnPointGroup.MapId && missionHighlight.Version > MissionHighlightVersion)
						{
							return missionHighlight;
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public bool IsDisabledOnGED
		{
			get
			{
				if (base.manager.Player.gameEconomyData.ConfigData.DisabledEpisodes != null && MissionSpawnPointGroup != null)
				{
					return base.manager.Player.gameEconomyData.ConfigData.DisabledEpisodes.Contains(MissionSpawnPointGroup.DisplayName);
				}
				return false;
			}
		}

		[JsonIgnore]
		public int AttackTargetId => MissionSpawnPointGroupId;

		[JsonIgnore]
		public bool HasUnlockedMission
		{
			get
			{
				for (int i = 0; i < Missions.Count; i++)
				{
					if (!Missions[i].IsLocked)
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsLocked
		{
			get
			{
				if (MissionSpawnPointGroup != null && MissionSpawnPointGroup.Category == MapCategory.Grind)
				{
					return false;
				}
				if (IsWeeklyChallenge)
				{
					return !base.manager.Player.WeeklyChallenge.CanPlayWeeklyChallenge;
				}
				if (IsInApocalyptiWeeklyChallenge)
				{
					return !base.manager.Player.WeeklyChallenge.OpenedApocalypseWeeklyChallenge;
				}
				if (IsWeeklySurvival)
				{
					if (!base.manager.Player.Tutorial.StaticTutorialComplete)
					{
						return true;
					}
					if (base.manager.Player.WeeklySurvival != null)
					{
						return !base.manager.Player.WeeklySurvival.CanPlayWeeklySurvival;
					}
					return true;
				}
				if (MissionSpawnPointGroup != null && MissionSpawnPointGroup.UnlockTimeMilliseconds > base.manager.Player.UtcTimeStamp)
				{
					return true;
				}
				if (HasUnlockedMission && HasRequiredCarLevel)
				{
					return !HasRequiredCouncilLevel;
				}
				return true;
			}
		}

		[JsonIgnore]
		public MissionSpawnPointGroup MissionSpawnPointGroup
		{
			get
			{
				MissionSpawnPointGroup spawnPointGroup = base.manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(MissionSpawnPointGroupId);
				if (spawnPointGroup == null)
				{
					base.Debug.LogWarning("MissionSpawnPointGroup not found by Id " + MissionSpawnPointGroupId);
				}
				return spawnPointGroup;
			}
		}

		[JsonIgnore]
		public int NumberStarsCollected
		{
			get
			{
				int num = 0;
				for (int i = 0; i < Missions.Count; i++)
				{
					if (Missions[i].Stars != null)
					{
						num += Missions[i].Stars.NumberStars;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public int MaxNumberStars
		{
			get
			{
				int num = 0;
				for (int i = 0; i < Missions.Count; i++)
				{
					if (Missions[i].Stars != null)
					{
						num += 3;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public bool IsWeeklyChallenge
		{
			get
			{
				List<WeeklyChallenge> weeklyChallenges = base.gameEconomyData.WeeklyChallenges;
				for (int i = 0; i < weeklyChallenges.Count; i++)
				{
					if (weeklyChallenges[i] != null && weeklyChallenges[i].DetailMapId == MissionSpawnPointGroupId)
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsInApocalyptiWeeklyChallenge
		{
			get
			{
				List<WeeklyChallenge> weeklyChallenges = base.gameEconomyData.WeeklyChallenges;
				for (int i = 0; i < weeklyChallenges.Count; i++)
				{
					if (weeklyChallenges[i] != null && weeklyChallenges[i].ApocalypticMapId == MissionSpawnPointGroupId)
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsWeeklySurvival
		{
			get
			{
				List<WeeklySurvival> weeklySurvivals = base.gameEconomyData.WeeklySurvivals;
				for (int i = 0; i < weeklySurvivals.Count; i++)
				{
					if (weeklySurvivals[i] != null && weeklySurvivals[i].DetailMapId == MissionSpawnPointGroupId)
					{
						return true;
					}
				}
				return false;
			}
		}

		public ModelList<MapMissionModel> Missions { get; set; }

		public MissionHighlight NewerFeaturedDataExist()
		{
			MissionHighlight latestFeaturedData = LatestFeaturedData;
			if (latestFeaturedData != null && latestFeaturedData.Version > MissionHighlightVersion)
			{
				return latestFeaturedData;
			}
			return null;
		}

		public override string ToString()
		{
			return "MissionSpawnPointGroupId = '" + MissionSpawnPointGroupId + "', HasBeenViewed = " + HasBeenViewed + ", Mission Count = " + ((Missions != null) ? Missions.Count : 0);
		}

		public void AddMission(MapMissionModel mapMissionModel)
		{
			Missions.Add(mapMissionModel);
			NotifyChange("MapMissionAdded", mapMissionModel);
		}

		public void RemoveMissions()
		{
			Missions.Clear();
		}

		public void RemoveMission(MapMissionModel mapMissionModel)
		{
			Missions.Remove(mapMissionModel);
			NotifyChange("MapMissionRemoved", mapMissionModel);
		}

		public override void Initialize()
		{
			base.Initialize();
			Missions = new ModelList<MapMissionModel>();
			HasBeenViewed = false;
		}

		public void OnObjectViewedByUser()
		{
			HasBeenViewed = true;
		}

		public override bool IsValid()
		{
			return true;
		}

		public int GetNumberStoryMissions()
		{
			int num = 0;
			for (int i = 0; i < Missions.Count; i++)
			{
				MissionSpawnPoint missionSpawnPoint = Missions[i].MissionSpawnPoint;
				if (missionSpawnPoint != null && missionSpawnPoint.IsExplicit)
				{
					num++;
				}
			}
			return num;
		}

		public int GetNumberCompletedStoryMissions()
		{
			int num = 0;
			for (int i = 0; i < Missions.Count; i++)
			{
				MissionSpawnPoint missionSpawnPoint = Missions[i].MissionSpawnPoint;
				if (missionSpawnPoint != null && missionSpawnPoint.IsExplicit && Missions[i].State == MapMissionState.Completed)
				{
					num++;
				}
			}
			return num;
		}

		public bool AreAllStoryMissionsCompleted()
		{
			int numberStoryMissions = GetNumberStoryMissions();
			if (numberStoryMissions == 0)
			{
				return false;
			}
			return GetNumberCompletedStoryMissions() == numberStoryMissions;
		}

		public MapMissionModel GetFirstUnlockedMissionModel()
		{
			for (int i = 0; i < Missions.Count; i++)
			{
				if (Missions[i].MissionSpawnPoint != null && Missions[i].MissionSpawnPoint.IsExplicit && Missions[i].State == MapMissionState.Unlocked)
				{
					return Missions[i];
				}
			}
			return null;
		}

		public MapMissionModel GetMissionModel(MissionSpawnPoint spawnPoint)
		{
			for (int i = 0; i < Missions.Count; i++)
			{
				if (Missions[i].MissionSpawnPoint != null && Missions[i].MissionSpawnPoint.MissionId == spawnPoint.MissionId)
				{
					return Missions[i];
				}
			}
			return null;
		}

		public MapMissionModel GetNextMissionModel(MissionSpawnPoint spawnPoint)
		{
			if (spawnPoint == null)
			{
				return null;
			}
			bool flag = false;
			for (int i = 0; i < Missions.Count; i++)
			{
				if (flag)
				{
					return Missions[i];
				}
				if (Missions[i].MissionSpawnPoint != null && Missions[i].MissionSpawnPoint.MissionId == spawnPoint.MissionId)
				{
					flag = true;
				}
			}
			return null;
		}

		public int GetNonCompletedMissionsCount()
		{
			int num = 0;
			for (int i = 0; i < Missions.Count; i++)
			{
				if (Missions[i].State != MapMissionState.Completed)
				{
					num++;
				}
			}
			return num;
		}

		public Cashier GetExploreMissionCashier(bool deadly)
		{
			MissionCost missionCost = base.manager.GameEconomyData.GetMissionCost(MissionSpawnPointGroup.CostIndex);
			int cost = ((missionCost == null) ? 1 : (deadly ? missionCost.DeadlyExploreCost : missionCost.ExploreCost));
			return Cashier.CreateOneItemCashier(base.manager, PurchaseType.Explore, CurrencyType.Supplies, cost);
		}

		public MapMissionGroupModel GetCurrentEpisodeDifficultyGroupModel()
		{
			MapMissionGroupModel mapMissionGroupModel = this;
			int num = mapMissionGroupModel.MissionSpawnPointGroupId;
			if (base.manager.Player.SurvivorContainer.StoryTeller.CurrentQuest is MissionQuest missionQuest && missionQuest.GetUnlockedEpisode() == mapMissionGroupModel && missionQuest.HasCompleted)
			{
				return mapMissionGroupModel;
			}
			while (mapMissionGroupModel != null && mapMissionGroupModel.AreAllStoryMissionsCompleted())
			{
				if (mapMissionGroupModel.MissionSpawnPointGroup == null)
				{
					return mapMissionGroupModel;
				}
				num++;
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(num);
				if (missionGroupModelForSpawnPointGroup != null)
				{
					mapMissionGroupModel = missionGroupModelForSpawnPointGroup;
					continue;
				}
				return mapMissionGroupModel;
			}
			return mapMissionGroupModel;
		}

		public MapMissionGroupModel GetOriginalDifficultyMapMissionGroupModel()
		{
			MapMissionGroupModel mapMissionGroupModel = this;
			int num = MissionSpawnPointGroupId;
			while (mapMissionGroupModel.MissionSpawnPointGroup.EpisodeDifficultyLevel > 1)
			{
				num--;
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(num);
				if (missionGroupModelForSpawnPointGroup != null)
				{
					mapMissionGroupModel = missionGroupModelForSpawnPointGroup;
					continue;
				}
				return mapMissionGroupModel;
			}
			return mapMissionGroupModel;
		}

		public FixedPoint AverageRequiredSurvivorLevel()
		{
			FixedPoint fixedPoint = 0L;
			int num = 0;
			if (Missions != null && Missions.Count > 0)
			{
				for (int i = 0; i < Missions.Count; i++)
				{
					if (Missions[i] != null)
					{
						if (!Missions[i].IsMasterMission)
						{
							fixedPoint += (FixedPoint)Missions[i].RequiredSurvivorLevel;
						}
						else
						{
							num++;
						}
					}
				}
				if (fixedPoint > 0L)
				{
					fixedPoint /= (FixedPoint)(Missions.Count - num);
				}
			}
			return fixedPoint;
		}

		public FixedPoint AverageGroupGasCost(CurrencyType currencyType)
		{
			Cashier cashier = null;
			FixedPoint fixedPoint = 0L;
			int num = 0;
			if (Missions != null && Missions.Count > 0)
			{
				for (int i = 0; i < Missions.Count; i++)
				{
					if (Missions[i] == null)
					{
						continue;
					}
					if (!Missions[i].IsMasterMission)
					{
						cashier = Missions[i].GetStartMissionCashier();
						if (cashier != null)
						{
							fixedPoint += (FixedPoint)cashier.GetTotalCost(currencyType);
						}
					}
					else
					{
						num++;
					}
				}
				if (fixedPoint > 0L)
				{
					fixedPoint /= (FixedPoint)(Missions.Count - num);
				}
			}
			return fixedPoint;
		}
	}
}
