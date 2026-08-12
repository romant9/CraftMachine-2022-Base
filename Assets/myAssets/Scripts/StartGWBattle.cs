using System;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class StartGWBattle : MonoBehaviour
{
	public static StartGWBattle Instance { get; private set; }
	public GuildModel guildModel => GWTeamUtils.Instance.IsOpponentGuild ?
		(GWTeamUtils.Instance.IsOpponentMaps ? GWTeamUtils.Instance.GuildModel : GWTeamUtils.Instance.OpponentGuilModel ?? GWTeamUtils.Instance.GuildModel)
		: (GWTeamUtils.Instance.IsOpponentMaps ? GWTeamUtils.Instance.OpponentGuilModel ?? GWTeamUtils.Instance.GuildModel : GWTeamUtils.Instance.GuildModel);

	//запуск из редактора
	public bool IsStartBattle;

	public bool IsStarted { get; set; }

	//сохраняем CurrentBattle перед изменениями. Затем откатываемся
	public string CurrentBattleSerialized { get; set; }

	public int OverrideHours = 1;

	public string OverrideTime;

	public bool IsAIForSurvivors;

	public bool IsSurvivorsPassTurns;

	public bool IsWaitKeyEveryTurn;

	[Range(0f, 1f)]
	public float IsWaitTimeForNextTurn = 0;

	public bool IsFakeBattle;

	public bool IsTryOpenPVPTeamsListPerSectorFromFile;

    public List<SurvivorModel> SurvivorsFromMission { get; set; }

	private void Awake()
	{
		if (Instance != null)
		{
			DebugTWD.LogError("Multiple StartGWBattle!");
			return;
		}
		Instance = this;
	}

	void Start()
	{
	}

	void Update()
	{
		if (IsStartBattle)
		{
			IsStartBattle = false;
			StartBattle(guildModel);
		}
	}

	//для расчета своих защитников на карте врага :
	//guildModel - OpponentGuild
	public void StartBattle(GuildModel guildModel)
	{
		//var opponentMatchmakingEntry = guildModel.GuildWarModel.NextBattlesOpponentMatchmakingInfo.First();// GetNextGuildBattleOpponentMatchmakingEntry();
		//var matchmakingInfoDeserialized = guildModel.GuildWarModel.NextBattlesOpponentMatchmakingInfo;

		var enemyData = GWTeamsManager.Instance.GwMatchMakingInfo;// guildModel.GuildWarModel.CurrentBattle.EnemyGuildData; // GWTeamsManager.instance.GwMatchMakingInfo;
		var TimeSlot = guildModel.GuildWarModel.CurrentBattle.TimeSlot;
		var RandomSeed = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.RandomSeed;

		IsStarted = guildModel.GuildWarModel.StartNewBattle(RandomSeed, guildModel, enemyData, DataManager.Instance.ModelManager, TimeSlot, isFakeBattle : IsFakeBattle); //стояло true // opponentMatchmakingEntry.IsFakeBattle);

		if (IsStarted)
		{
			DebugTWD.Log("Battle started for " + guildModel.Name);
			IsStarted = false;

			//var PVPTeamsListPerSector = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PVPTeamsListPerSector;
			//string data1 = jsonSerializer.Serialize(PVPTeamsListPerSector);
			//var path1 = GWTeamUtils.globalPath + GWTeamUtils.playerFolder + "1.PlayerInfoSnapshot_new" + ".json";
			//GWTeamUtils.SaveToFile(data1, path1, append: false);
			//Debug.Log("1.PlayerInfoSnapshot_new saved");
		}
	}

	public void SaveCurrentGWBattle()
	{
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		CurrentBattleSerialized = OfflineManager.JsonSerializer.Serialize(currentBattle);

		IsFakeBattle = true;
		//guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.Start();

		StartBattle(guildModel);
	}

	public void RestoreGWBattle()
	{
		IsFakeBattle = false;

		if (!string.IsNullOrEmpty(CurrentBattleSerialized))
		{
			guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle = OfflineManager.JsonSerializer.Deserialize<GuildBattleModel>(CurrentBattleSerialized);
			guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.Start();
		}
	}


	#region temp Methods
	public void SetupMissionAndPvpPlacement(GuildBattleMapModel mapModel)
	{
		for (int i = 0; i < mapModel.Sectors.Count; i++)
		{
			//mapModel.Sectors[i].SetupArea();
			SetupArea(mapModel, i);
		}
	}
	public void SetupArea(GuildBattleMapSectorModel sector)
	{
		var missionSectorDefinition = DataManager.Instance.GameData.FindMissionSectorDefinition(sector.SectorId);
		if (sector.StartIndexPerArea == null || missionSectorDefinition == null)
		{
			return;
		}
		sector.StartIndexPerArea.Clear();
		if (sector.StartIndexPerArea.Count == 0)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num = (i != 0) ? (i * missionSectorDefinition.MissionAmountPerPVPEnemy) : 0;
				sector.StartIndexPerArea.Add(new List<int>());
				for (int j = num; j < num + missionSectorDefinition.MissionAmountPerPVPEnemy; j++)
				{
					sector.MissionPoolName = missionSectorDefinition.MissionPoolName;
					GuildBattleMapMissionModel missionModel = GetMissionModel(sector, GuildBattleMapMissionModel.GenerateId(sector.MissionPoolName, sector.SectorId, j));
					missionModel.AreaIndex = i;
					TryAssignPvpTeamForMission(missionModel);
				}
				sector.StartIndexPerArea[i].Add(num);
			}
			sector.CurrentBatchIndex = num;
		}
		//sector.UpdateAreaMissionsLists();
	}
	public void SetupArea(GuildBattleMapModel mapModel, int index)
	{
		GuildBattleMapSectorModel sector = mapModel.Sectors[index];
		SetupArea(sector);
	}
	public GuildBattleMapMissionModel GetMissionModel(GuildBattleMapSectorModel sector, string uniqueMissionId)
	{
		for (int i = 0; i < sector.RandomizedMissions.Count; i++)
		{
			GuildBattleMapMissionModel guildBattleMapMissionModel = sector.RandomizedMissions[i];
			if (guildBattleMapMissionModel.Id == uniqueMissionId)
			{
				return guildBattleMapMissionModel;
			}
		}
		return null;
	}
	public void TryAssignPvpTeamForMission(GuildBattleMapMissionModel mission)
	{
		var PvpTeamsListPerSector = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PVPTeamsListPerSector;

		if (PvpTeamsListPerSector.Count == 0)
		{
			DebugTWD.LogError("PvpTeamsListPerSector count is 0. Attention: ");

			var players = GWTeamUtils.Instance.GetAllTeamsSorted(guildModel.GuildWarModel.CurrentBattle.EnemyGuildData.PlayerInfoSnapshot);
			PvpTeamsListPerSector = GWTeamUtils.Instance.PVPTeamsListPerSector(guildModel, players);
			if (PvpTeamsListPerSector != null && PvpTeamsListPerSector.Count > 0) guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PVPTeamsListPerSector = PvpTeamsListPerSector;
		}
		var PvpTeamsIndexPerMission = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission;

		if (mission == null || PvpTeamsListPerSector == null || mission.Type != 0)
		{
			return;
		}

		if (PvpTeamsIndexPerMission.ContainsKey(mission.Id))
		{
			DebugTWD.LogError("Mission already has PvP assigned: " + mission.Id);
			return;
		}
		if (PvpTeamsListPerSector.TryGetValue(mission.SectorIdOwner, out var value))
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
				FixedPoint fixedPoint;
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
					GeneratePvpTeamIndexId(mission.SectorIdOwner, num, out string id);
					PvpTeamsIndexPerMission.Add(mission.Id, id);
					value[num].MissionId = mission.Id;
					return;
				}
			}
		}
		DebugTWD.LogError("Could not find Enemy for mission : " + mission.Id);
	}

	public static void GeneratePvpTeamIndexId(int sectorId, int index, out string id)
	{
		id = sectorId + "_" + index;
	}

	public List<string> GetCSectorsID()
	{
		var CSectorsList = new List<string>();
		string file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
		Dictionary<string, string> PvpTeamsIndexPerMission = OfflineManager.JsonSerializer.Deserialize<Dictionary<string, string>>(file);

		foreach (var mission in PvpTeamsIndexPerMission)
		{
			var sectorContainMapName = mission.Value.Substring(0, mission.Value.Length - 1);
			//int maxIndex = int.Parse(mission.Key.Split('_')[2]);
			bool isC = sectorContainMapName.ToLower().Contains("c");
			if (isC) CSectorsList.Add(mission.Key);
		}
		return CSectorsList;
	}

	public void ReAssignTeamsCSectors()
	{
		List<string> CSectorsList = GetCSectorsID();

		for (int i = 0; i < CSectorsList.Count; i++)
		{
			var mapModel = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel;

			int sectorID = int.Parse(CSectorsList[i].Split('_')[1]);
			var sectorModel = mapModel.GetSectorModel(sectorID);
			//mapModel.Sectors[i].SetupArea();
			SetupArea(sectorModel);
		}
	}
	#endregion
}
