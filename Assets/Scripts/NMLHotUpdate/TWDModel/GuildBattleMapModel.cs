using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GuildBattleMapModel : TWDGroupModelChild
	{
		public const int ColumnCount = 4;

		public const string GuildBattleNonPvpCompletionAdded = "GuildBattleNonPvpCompletionAdded";

		public const string GuildBattlePvpCompletionAdded = "GuildBattlePvpCompletionAdded";

		public const string GuildBattleMissionPvPEnemiesUpdated = "GuildBattleMissionPvPEnemiesUpdated";

		[JsonIgnore]
		public Dictionary<int, List<GuildBattlePvpTeam>> PVPTeamsListPerSector;

		private Dictionary<string, GuildBattleMapMissionModel> _missionsLookupTable;

		private Dictionary<int, GuildBattleMapSectorModel> _sectorLookupTable;

		private PlayerModel player;

		public int WarDefinitionId { get; set; }

		public int RandomSeed { get; set; }

		public TWDGroupChildModelList<GuildBattleMapSectorModel> Sectors { get; set; }

		public Dictionary<string, string> PvpTeamsIndexPerMission { get; set; }

		private Dictionary<string, GuildBattleMapMissionModel> missionsLookupTable
		{
			get
			{
				if (_missionsLookupTable == null)
				{
					_missionsLookupTable = new Dictionary<string, GuildBattleMapMissionModel>();
					for (int i = 0; i < Sectors.Count; i++)
					{
						GuildBattleMapSectorModel guildBattleMapSectorModel = Sectors[i];
						if (guildBattleMapSectorModel != null)
						{
							for (int j = 0; j < guildBattleMapSectorModel.RandomizedMissions.Count; j++)
							{
								GuildBattleMapMissionModel guildBattleMapMissionModel = guildBattleMapSectorModel.RandomizedMissions[j];
								_missionsLookupTable.Add(guildBattleMapMissionModel.Id, guildBattleMapMissionModel);
							}
						}
					}
				}
				return _missionsLookupTable;
			}
		}

		private Dictionary<int, GuildBattleMapSectorModel> sectorLookupTable
		{
			get
			{
				if (_sectorLookupTable == null || Sectors.Count != _sectorLookupTable.Count)
				{
					_sectorLookupTable = new Dictionary<int, GuildBattleMapSectorModel>();
					for (int i = 0; i < Sectors.Count; i++)
					{
						GuildBattleMapSectorModel guildBattleMapSectorModel = Sectors[i];
						if (guildBattleMapSectorModel != null)
						{
							_sectorLookupTable.Add(guildBattleMapSectorModel.SectorId, guildBattleMapSectorModel);
						}
					}
				}
				return _sectorLookupTable;
			}
		}

		public GuildBattleMapModel()
		{
			Sectors = new TWDGroupChildModelList<GuildBattleMapSectorModel>();
			PvpTeamsIndexPerMission = new Dictionary<string, string>();
			PVPTeamsListPerSector = new Dictionary<int, List<GuildBattlePvpTeam>>();
			WarDefinitionId = -1;
			RandomSeed = -1;
		}

		public override void Start()
		{
			base.Start();
			SetupMap(RandomSeed, WarDefinitionId);
		}

		public override void SetPlayerOwnerAndGameEconomyData(GameEconomyData ged, TWDGroupModelChild root, PlayerModel player = null)
		{
			base.SetPlayerOwnerAndGameEconomyData(ged, root, player);
			this.player = player;
		}

		public bool AddCompletionToMissionModel(string uniqueMissionId, string playerId, out bool wasPvpCompletion)
		{
			wasPvpCompletion = false;
			GuildBattleMapMissionModel missionModel = GetMissionModel(uniqueMissionId);
			if (missionModel != null)
			{
				bool num = missionModel.IsPvpComplete();
				missionModel.AddMissionCompletions();
				wasPvpCompletion = missionModel.IsPvpComplete();
				if (num != wasPvpCompletion)
				{
					missionModel.PvpPlayerHashedId = playerId;
					missionModel.PvpParticipants.Remove(playerId);
					NotifyChange("GuildBattlePvpCompletionAdded", missionModel);
				}
				else
				{
					NotifyChange("GuildBattleNonPvpCompletionAdded", missionModel);
				}
				return true;
			}
			return false;
		}

		public GuildBattleMapMissionModel GetMissionModel(string uniqueMissionId)
		{
			GuildBattleMapMissionModel value = null;
			if (missionsLookupTable.TryGetValue(uniqueMissionId, out value))
			{
				return value;
			}
			return null;
		}

		public GuildBattleMapSectorModel GetSectorModel(int sectorId)
		{
			GuildBattleMapSectorModel value = null;
			if (sectorLookupTable.TryGetValue(sectorId, out value))
			{
				return value;
			}
			return null;
		}

		public void SetupMap(int randomSeed, int warDefinitionId)
		{
			WarDefinitionId = warDefinitionId;
			if (base.gameEconomyData.FindGuildWarWithId(WarDefinitionId) != null)
			{
				RandomSeed = randomSeed;
				SetupMap();
			}
		}

		public void SetupMissionAndPvpPlacement()
		{
			for (int i = 0; i < Sectors.Count; i++)
			{
				Sectors[i].SetupArea();
			}
		}

		private void SetupMap()
		{
			if (RandomSeed == -1 || base.gameEconomyData.GuildBattleMissionPoolDefinitionGrouped == null)
			{
				return;
			}
			GuildWarDefinition guildWarDefinition = base.gameEconomyData.FindGuildWarWithId(WarDefinitionId);
			base.Debug.Log("SetupMap : " + RandomSeed);
			int[] sectorsIds = guildWarDefinition.SectorsIds;
			GuildBattleSectorDefinition[] guildBattleSectorDefinitions = base.gameEconomyData.GuildBattleSectorDefinitions;
			int num = guildBattleSectorDefinitions.Length;
			for (int i = 0; i < num; i++)
			{
				GuildBattleSectorDefinition guildBattleSectorDefinition = guildBattleSectorDefinitions[i];
				if (!sectorsIds.Contains(guildBattleSectorDefinition.Id))
				{
					continue;
				}
				GuildBattleMapSectorModel sectorModel = GetSectorModel(guildBattleSectorDefinition.Id);
				if (sectorModel == null)
				{
					sectorModel = CreateSector(guildBattleSectorDefinition.Id);
					AddSectorToSectors(ref sectorModel);
				}
				sectorModel.MissionConfigPoolName = guildBattleSectorDefinition.MissionConfigPoolName;
				sectorModel.MissionPoolName = guildBattleSectorDefinition.MissionPoolName;
				List<int> PVPMissionsPlacement = null;
				SetPVPEnemyPlacement(guildBattleSectorDefinition, ref PVPMissionsPlacement);
				List<Tuple<int, int, int>> selectedMissions = new List<Tuple<int, int, int>>();
				RandomizeMissions(guildBattleSectorDefinition, sectorModel, ref selectedMissions);
				int num2 = 0;
				int count = selectedMissions.Count;
				for (int j = 0; j < count; j++)
				{
					Tuple<int, int, int> tuple = selectedMissions[j];
					GuildBattleMapMissionModel missionModel = GetMissionModel(GuildBattleMapMissionModel.GenerateId(guildBattleSectorDefinition.MissionPoolName, guildBattleSectorDefinition.Id, j));
					if (missionModel == null)
					{
						missionModel = CreateMapMission(guildBattleSectorDefinition.MissionPoolName, guildBattleSectorDefinition.Id, j);
						AddMissionToSector(ref sectorModel, ref missionModel);
					}
					missionModel.OrderNumberInPool = tuple.First;
					missionModel.SectorIdOwner = guildBattleSectorDefinition.Id;
					if (num2 < PVPMissionsPlacement.Count && PVPMissionsPlacement[num2] == j)
					{
						num2++;
						missionModel.Type = GuildBattleMapMissionModel.MissionType.PVP;
					}
					else
					{
						missionModel.Type = GuildBattleMapMissionModel.MissionType.PVE;
					}
					missionModel.MissionConfigIndexObjective = tuple.Second;
					missionModel.MissionConfigIndexEnemies = tuple.Third;
					missionModel.CostIndex = guildWarDefinition.CostIndex;
				}
				sectorModel.UpdateAreaMissionsLists();
			}
			Sectors.UpdateModelObjects();
		}

		private List<GuildBattlePvpTeam> GetAllTeamsSorted(Dictionary<string, GuildBattleParticipantInfo> players)
		{
			List<GuildBattlePvpTeam> list = new List<GuildBattlePvpTeam>();
			foreach (GuildBattleParticipantInfo value in players.Values)
			{
				List<SurvivorMockData> selectedSurvivors = value.SelectedSurvivors;
				if (selectedSurvivors.Count >= 9)
				{
					GuildBattlePvpTeam item = new GuildBattlePvpTeam(selectedSurvivors.GetRange(0, 3));
					GuildBattlePvpTeam item2 = new GuildBattlePvpTeam(selectedSurvivors.GetRange(3, 3));
					GuildBattlePvpTeam item3 = new GuildBattlePvpTeam(selectedSurvivors.GetRange(6, 3));
					list.Add(item);
					list.Add(item2);
					list.Add(item3);
				}
			}
			list.StableSort((GuildBattlePvpTeam a, GuildBattlePvpTeam b) => a.AverageAdjustedLevel.CompareTo(b.AverageAdjustedLevel));
			return list;
		}

		public void AssingPVPTeams(Dictionary<string, GuildBattleParticipantInfo> players, TWDModelManager debugManager)
		{
			GuildWarDefinition warDefinition = base.gameEconomyData.FindGuildWarWithId(WarDefinitionId);
			if (warDefinition == null)
			{
				base.Debug.LogError("No war definition was found for WarDefinitionId " + WarDefinitionId);
				return;
			}
			List<GuildBattlePvpTeam> allTeamsSorted = GetAllTeamsSorted(players);
			List<GuildBattleSectorDefinition> list = base.gameEconomyData.GuildBattleSectorDefinitions.Where((GuildBattleSectorDefinition x) => warDefinition.SectorsIds.Contains(x.Id)).ToList();
			int minVp = list.Min((GuildBattleSectorDefinition x) => x.SectorVP);
			int maxVp = list.Max((GuildBattleSectorDefinition x) => x.SectorVP);
			foreach (GuildBattleSectorDefinition item in list)
			{
				if (!PVPTeamsListPerSector.TryGetValue(item.Id, out var value))
				{
					value = new List<GuildBattlePvpTeam>();
					PVPTeamsListPerSector.Add(item.Id, value);
				}
				GuildBattleMapSectorModel sectorModel = GetSectorModel(item.Id);
				if (sectorModel == null)
				{
					base.Debug.LogWarning("No sector was found for sector id " + item.Id);
					continue;
				}
				int amountOfTeamsNeeded = sectorModel.RandomizedMissions.Count((GuildBattleMapMissionModel x) => x.Type == GuildBattleMapMissionModel.MissionType.PVP);
				List<GuildBattlePvpTeam> pvpTeamsForSector = GetPvpTeamsForSector(item.SectorVP, minVp, maxVp, allTeamsSorted, amountOfTeamsNeeded);
				value.AddRange(pvpTeamsForSector);
				value?.StableSort((GuildBattlePvpTeam a, GuildBattlePvpTeam b) => a.AverageAdjustedLevel.CompareTo(b.AverageAdjustedLevel));
			}
			debugManager?.Debug.Log("#### Serialized GuildBattleParticipantInfo - " + debugManager.GetMessageSerializer().Serialize(players));
		}

		private List<GuildBattlePvpTeam> GetPvpTeamsForSector(int sectorVp, int minVp, int maxVp, List<GuildBattlePvpTeam> allTeams, int amountOfTeamsNeeded)
		{
			if (amountOfTeamsNeeded == 0)
			{
				return new List<GuildBattlePvpTeam>();
			}
			if (allTeams.Count == 0)
			{
				base.Debug.LogError($"{allTeams} has a size of 0. Probably PlayerSnapshot has incomplete data.");
				return new List<GuildBattlePvpTeam>();
			}
			List<GuildBattlePvpTeam> list = new List<GuildBattlePvpTeam>();
			if (amountOfTeamsNeeded >= allTeams.Count)
			{
				int num = 0;
				while (list.Count != amountOfTeamsNeeded)
				{
					list.Add(CreateCopy(allTeams[num % allTeams.Count]));
					num++;
				}
				return list;
			}
			FixedPoint fixedPoint = sectorVp - minVp;
			FixedPoint fixedPoint2 = maxVp - minVp;
			FixedPoint fixedPoint3 = fixedPoint / fixedPoint2;
			int num2 = (int)Math.Floor((double)(allTeams.Count - 1) * (double)fixedPoint3);
			int num3 = num2 + amountOfTeamsNeeded;
			int num4 = num3 - allTeams.Count;
			if (num4 > 0)
			{
				num2 -= num4;
				num3 -= num4;
			}
			if (num2 < 0)
			{
				base.Debug.LogError("Start index for assigning teams was negative");
				num2 = 0;
				num3 = allTeams.Count;
			}
			if (num3 > allTeams.Count)
			{
				base.Debug.LogError("End index for assigning teams was over the amount of teams in the array");
				num3 = allTeams.Count;
			}
			for (int i = num2; i < num3; i++)
			{
				list.Add(CreateCopy(allTeams[i]));
			}
			return list;
		}

		private GuildBattlePvpTeam CreateCopy(GuildBattlePvpTeam team)
		{
			return new GuildBattlePvpTeam(team.Survivors);
		}

		public GuildBattlePvpTeam GetPvpTeamForMission(string missionId)
		{
			string value = "";
			int sectorId = -1;
			int index = -1;
			if (PvpTeamsIndexPerMission == null)
			{
				return null;
			}
			if (!PvpTeamsIndexPerMission.TryGetValue(missionId, out value))
			{
				return null;
			}
			ParsePvpTeamIndexId(value, out sectorId, out index);
			if (!PVPTeamsListPerSector.ContainsKey(sectorId) || PVPTeamsListPerSector[sectorId].Count <= index)
			{
				return null;
			}
			return PVPTeamsListPerSector[sectorId][index];
		}

		private void RandomizeMissions(GuildBattleSectorDefinition sectorDefinition, GuildBattleMapSectorModel sector, ref List<Tuple<int, int, int>> selectedMissions)
		{
			int num = sectorDefinition.PVPEnemyAmount * sectorDefinition.MissionAmountPerPVPEnemy;
			ModelRandom modelRandom = new ModelRandom(RandomSeed + sectorDefinition.Id);
			string text = "";
			text = GuildBattleMissionConfig.GetGroupKey("Objectives", sector.MissionPoolName);
			FixedPoint[] array = null;
			if (base.gameEconomyData.GuildBattleMissionConfigsWeights.TryGetValue(text, out var value))
			{
				array = value.ToArray();
			}
			FixedPoint[] array2 = null;
			text = GuildBattleMissionConfig.GetGroupKey("Enemies", sector.MissionPoolName);
			if (base.gameEconomyData.GuildBattleMissionConfigsWeights.TryGetValue(text, out value))
			{
				array2 = value.ToArray();
			}
			if (array == null || array2 == null)
			{
				base.Debug.LogError("Missing weights for " + sector.MissionPoolName + ", check GED!");
				return;
			}
			List<string> value2 = null;
			base.gameEconomyData.GuildBattleMissionPoolDefinitionGrouped.TryGetValue(sector.MissionPoolName, out value2);
			if (value2 == null)
			{
				base.Debug.LogError("Missing mission pool for " + sector.MissionPoolName + ", check GED!");
				return;
			}
			List<string> list = null;
			for (int i = 0; i < num; i++)
			{
				int second = modelRandom.WeightedRandom(array);
				int third = modelRandom.WeightedRandom(array2);
				if (i % sectorDefinition.MissionAmountPerPVPEnemy == 0)
				{
					list = new List<string>(value2);
				}
				int randomInRange = modelRandom.GetRandomInRange(0, list.Count - 1);
				list.RemoveAt(randomInRange);
				Tuple<int, int, int> item = new Tuple<int, int, int>(randomInRange, second, third);
				selectedMissions.Add(item);
			}
		}

		private void SetPVPEnemyPlacement(GuildBattleSectorDefinition sectorDefinition, ref List<int> PVPMissionsPlacement)
		{
			int num = 1;
			PVPMissionsPlacement = new List<int>();
			while (PVPMissionsPlacement.Count < sectorDefinition.PVPEnemyAmount)
			{
				PVPMissionsPlacement.Add(num * sectorDefinition.MissionAmountPerPVPEnemy - 1);
				num++;
			}
		}

		public void Reset()
		{
			Sectors.Clear();
			PvpTeamsIndexPerMission.Clear();
		}

		public void TryAssignPvpTeamForMission(GuildBattleMapMissionModel mission)
		{
			if (mission == null || PVPTeamsListPerSector == null || mission.Type != GuildBattleMapMissionModel.MissionType.PVP)
			{
				return;
			}
			if (PvpTeamsIndexPerMission.ContainsKey(mission.Id))
			{
				base.Debug.LogError("Mission already has PvP assigned: " + mission.Id);
				return;
			}
			if (PVPTeamsListPerSector.TryGetValue(mission.SectorIdOwner, out var value))
			{
				if (value.Count == 0)
				{
					return;
				}
				int num = 0;
				int num2 = -1;
				if (mission.AreaIndex == 0)
				{
					num2 = -1;
				}
				else if (mission.AreaIndex == 1 || mission.AreaIndex == 2)
				{
					FixedPoint fixedPoint = 0L;
					if (value.Count % 2 == 0)
					{
						fixedPoint = (FixedPoint)value.Count / (FixedPoint)2L;
						fixedPoint = FixedPoint.Ceiling(fixedPoint) - 1L;
					}
					else
					{
						fixedPoint = (FixedPoint)value.Count / (FixedPoint)2L;
						fixedPoint = FixedPoint.Floor(fixedPoint);
					}
					num2 = (int)fixedPoint;
					num2 = Math.Min(num2, value.Count - 1);
					num2 = Math.Max(num2, 0);
				}
				else if (mission.AreaIndex == 3)
				{
					num2 = value.Count;
				}
				string id = "";
				for (int i = 0; i < value.Count; i++)
				{
					if (i == 0)
					{
						num = num2;
					}
					if (mission.AreaIndex == 0 || mission.AreaIndex == 2)
					{
						num++;
					}
					else if (mission.AreaIndex == 3 || (mission.AreaIndex == 1 && i > 0))
					{
						num--;
					}
					num = ((num < 0) ? (value.Count - 1) : num);
					num = ((num <= value.Count - 1) ? num : 0);
					if (string.IsNullOrEmpty(value[num].MissionId))
					{
						GeneratePvpTeamIndexId(mission.SectorIdOwner, num, out id);
						PvpTeamsIndexPerMission.Add(mission.Id, id);
						value[num].MissionId = mission.Id;
						return;
					}
				}
			}
			base.Debug.LogError("Could not find Enemy for mission : " + mission.Id);
		}

		public static void GeneratePvpTeamIndexId(int sectorId, int index, out string id)
		{
			id = sectorId + "_" + index;
		}

		public static void ParsePvpTeamIndexId(string indexIdString, out int sectorId, out int index)
		{
			sectorId = -1;
			index = -1;
			if (!string.IsNullOrEmpty(indexIdString))
			{
				string[] array = indexIdString.Split('_');
				if (array.Length > 1)
				{
					int.TryParse(array[0], out sectorId);
					int.TryParse(array[1], out index);
				}
			}
		}

		private GuildBattleMapSectorModel CreateSector(int sectorId)
		{
			GuildBattleMapSectorModel guildBattleMapSectorModel = new GuildBattleMapSectorModel();
			guildBattleMapSectorModel.SetPlayerOwnerAndGameEconomyData(base.gameEconomyData, base.root, player);
			guildBattleMapSectorModel.SectorId = sectorId;
			return guildBattleMapSectorModel;
		}

		private GuildBattleMapMissionModel CreateMapMission(string missionPool, int sectorId, int orderNumberInPool)
		{
			GuildBattleMapMissionModel guildBattleMapMissionModel = new GuildBattleMapMissionModel();
			guildBattleMapMissionModel.SetPlayerOwnerAndGameEconomyData(base.gameEconomyData, base.root, player);
			guildBattleMapMissionModel.Id = GuildBattleMapMissionModel.GenerateId(missionPool, sectorId, orderNumberInPool);
			return guildBattleMapMissionModel;
		}

		private void AddMissionToSector(ref GuildBattleMapSectorModel sectorModel, ref GuildBattleMapMissionModel missionModel)
		{
			if (sectorModel != null && missionModel != null)
			{
				if (!missionsLookupTable.ContainsKey(missionModel.Id))
				{
					missionsLookupTable.Add(missionModel.Id, missionModel);
				}
				sectorModel.RandomizedMissions.Add(missionModel);
			}
		}

		private void AddSectorToSectors(ref GuildBattleMapSectorModel sectorModel)
		{
			if (Sectors != null && sectorModel != null)
			{
				if (!sectorLookupTable.ContainsKey(sectorModel.SectorId))
				{
					sectorLookupTable.Add(sectorModel.SectorId, sectorModel);
				}
				Sectors.Add(sectorModel);
			}
		}
	}
}
