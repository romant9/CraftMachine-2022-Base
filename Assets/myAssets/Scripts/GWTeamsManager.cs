using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class GWTeamsManager : MonoBehaviour
{
	public static GWTeamsManager Instance { get; private set; }
	private GuildModel GuildModel => GWTeamUtils.Instance.GuildModel;
	private GuildModel OpponentGuilModel => GWTeamUtils.Instance.OpponentGuilModel;
	public GuildModel CurrentGuildModel { get; private set; }

	[Serializable]
	public class GWTeamData
	{
		public GWTeamPanel teamPanel;
		public GuildBattlePvpTeam pvpTeam { get; set; }
		public UISlider indexSlider;
		public int index { get; set; }
	}

	public List<GWTeamData> gWTeamsData;
	private List<SurvivorMockData> pvpTeamCurrent;

	public Dictionary<string, GuildBattleParticipantInfo> GwProtectorsInfo { get; private set; }
	public GuildBattleMatchmakingInfo GwMatchMakingInfo { get; private set; }
	public List<GuildBattlePvpTeam> AllTeamsSortedList { get; set; }

	public Dictionary<string, List<SurvivorMockData>> survivorTeamsOrigin = new Dictionary<string, List<SurvivorMockData>>();

	//GuildBattlePvpTeam guildBattlePvpTeam_Top; //top, last item
	//GuildBattlePvpTeam guildBattlePvpTeam_preTop; //top middle
	//GuildBattlePvpTeam guildBattlePvpTeam_Middle; //low, first item
	//GuildBattlePvpTeam guildBattlePvpTeam_Low; //low, second item

	public int count_Top_base = 20;
	public int count_preTop_base = 0;
	public int count_middle_base = 0;

	public UISlider topCountSlider;
	public UILabel topCountLabel;

	public UISlider pretopCountSlider;
	public UILabel pretopCountLabel;

	public UISlider middleCountSlider;
	public UILabel middleCountLabel;

	//фильтр для шаблона
	public UISlider lowestLocSlider;
	//фильтр секторов
	public UISlider lowestLocSlider2;

	public UILabel lowestLocLabel;
	public UILabel lowestLocLabel2;

	public List<UILabel> teamSectorNameLabels;
	public UIScrollView uIScrollViewSectors;

	public int lowest_location_level = 0;
	public bool middleIsPretop = false;

	private bool isInitDone = false;
	public string guildID { get; private set; }

	public UIButtonToggleSet pvpTeamsToggleSet;
	public GWTeamPanel GWTeamPanelCurrent;
	public GuildPlayerProtectorLocationsList GWProtList;
	//public GuildBattleTeamInfo teamDataCurrent { get; set; }
	public string currentPlayerID { get; set; }
	public int currentPlayerBaseIndex { get; set; }

	public int teamSortedPlayerIndex {  get; set; }

	private string CustomMapsCopy;

	public List<LevelChangeItem> btLevelChangeList;

	Dictionary<int, List<GuildBattlePvpTeam>> PvpTeamsListPerSector;
	Dictionary<string, string> PvpTeamsIndexPerMission;


	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		if (GuildModel == null) return;

		//InitData();

		SetCountSliders();

		isInitDone = true;
	}

	void SetCountSliders()
	{
		topCountSlider.value = 1f;
		pretopCountSlider.value = .5f;
		middleCountSlider.value = 0f;
		lowestLocSlider.value = 1f;
		lowestLocSlider2.value = 1f;
	}

	public void SaveAllTeamsSortedList(bool isSave)
	{
		AllTeamsSortedList = GWTeamUtils.Instance.GetAllTeamsSorted(GwProtectorsInfo);
		//убрать для старого варианта
		PvpTeamsIndexPerMission = GWTeamUtils.Instance.IsOpponentGuild && OpponentGuilModel != null ? OpponentGuilModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission : GuildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission;
		//
		if (isSave)
		{
			string data = OfflineManager.JsonSerializer.Serialize(AllTeamsSortedList);
			var path = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "1.AllTeamsSortedList.json";
			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log("1.AllTeamsSortedList_original saved");
		}
	}

	public void InitData(bool resetSliders = true)
	{
		string guildName;
		CurrentGuildModel = null;

		if (GWTeamUtils.Instance.IsOpponentGuild)
		{
			//поменять для старого варианта
			if (GWTeamUtils.Instance.IsOpponentMaps)
			{
				GwMatchMakingInfo = OpponentGuilModel?.GuildBattleMatchmakingInfo ?? GuildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData;
			}
			else
			{
				GwMatchMakingInfo = GuildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData;
			}
			CurrentGuildModel = GuildModel;
			//
		}
		else
		{
			//
			if (GWTeamUtils.Instance.IsOpponentMaps)
			{
				GwMatchMakingInfo = GuildModel.GuildBattleMatchmakingInfo;
			}
			else
			{
				GwMatchMakingInfo = OpponentGuilModel?.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData ?? GuildModel.GuildBattleMatchmakingInfo;
			}
			CurrentGuildModel = OpponentGuilModel ?? GuildModel;
			//
		}

		GwProtectorsInfo = GwMatchMakingInfo.PlayerInfoSnapshot;
		guildID = CurrentGuildModel.Id;
		guildName = CurrentGuildModel.Name;

		//if (AllTeamsSortedList == null)
		//{
			SaveAllTeamsSortedList(isSave: false);
		//}

		int count = AllTeamsSortedList.Count;
		DebugTWD.Log("Init GWTeamsManager for " + guildName + ", Count is " + count);

		if (resetSliders)
		{
			gWTeamsData[0].indexSlider.value = (count - 1f) / count;
			gWTeamsData[1].indexSlider.value = (count - 10f) / count;
			gWTeamsData[2].indexSlider.value = middleIsPretop ? (count - 10f) / count : 10f / count;
			gWTeamsData[3].indexSlider.value = 0f;
		}
	}

	public void SetTeamCount(UISlider sl)
	{
		if (AllTeamsSortedList == null) return;
		int count = AllTeamsSortedList.Count;

		if (sl == topCountSlider)
		{
			var label = Mathf.RoundToInt(topCountSlider.value * count / 3f);
			count_Top_base = label;
			topCountLabel.text = label.ToString();
		}
		else if (sl == pretopCountSlider)
		{
			var label = Mathf.RoundToInt(pretopCountSlider.value * count / 3f);
			count_preTop_base = label;
			pretopCountLabel.text = label.ToString();
		}
		else if (sl == middleCountSlider)
		{
			var label = Mathf.RoundToInt(middleCountSlider.value * count / 3f);
			count_middle_base = label;
			middleCountLabel.text = label.ToString();
		}
		else if (sl == lowestLocSlider)
		{
			var label = Mathf.RoundToInt(lowestLocSlider.value * 15f);
			lowest_location_level = label;
			lowestLocLabel.text = label.ToString();
		}
		else
		{
			var label = Mathf.RoundToInt(lowestLocSlider2.value * 15f);
			lowest_location_level = label;
			lowestLocLabel2.text = label.ToString();
		}
	}

	void Update()
	{
	}

	private void OnEnable()
	{
		if (string.IsNullOrEmpty(guildID) || (GuildModel != null && guildID != GuildModel.Id)) isInitDone = false;
		SetBtLevelsListeners(true);

		if (isInitDone || GuildModel == null) return;
		InitData();
	}
	private void OnDisable()
	{
		SetBtLevelsListeners(false);
	}

	public void SetDataToSectorNameLabels(List<GuildBattleTeamInfo> CustomTeams)
	{
		foreach (var label in teamSectorNameLabels)
		{
			label.text = string.Empty;
		}
		if (CustomTeams == null & CustomTeams.Count == 0)
		{
			return;
		}

		var maxCount = 0;
		for (int i = 0; i < CustomTeams.Count; i++)
		{
			if (i > teamSectorNameLabels.Count) break;
			List<string> sectors = CustomTeams[i].SectorNamesAll;
			if (sectors.Count > maxCount) maxCount = sectors.Count;
			var result = $"({sectors.Count})\nlev.{CustomTeams[i].AdjustedLevel}\n{ string.Join("\n", sectors)}";
			teamSectorNameLabels[i].text = result;
		}

		string result2 = string.Empty;
		for (int j = 0; j < maxCount; j++)
		{
			var str0 = string.Empty;
			for (int i = 0; i < CustomTeams.Count; i++)
			{
				string str1;
				if (CustomTeams[i].SectorNamesAll.Count > j)
				str1 = CustomTeams[i].SectorNamesAll[j];
				else str1 = string.Empty;
				str0 += str1 + '\t';
			}
			result2 += str0 + '\n';
		}
		CustomMapsCopy = result2;

		uIScrollViewSectors.ResetPosition();
	}

	public void CopyToClipboardCustomTeams()
	{
		if (string.IsNullOrEmpty(CustomMapsCopy)) return;
		MyTools.CopyToClipboard(CustomMapsCopy);
	}
	public void CopyToClipboardSectorProtectors()
	{
		if (GWProtList.SectorList == null || GWProtList.SectorList.Count == 0) return;

		string guildName = CurrentGuildModel.GuildWarModel.CurrentBattle.EnemyGuildName;
		string result = guildName + '\n' + DateTime.Now.ToLocalTime().ToString(UserPrefsKeys.TimeFormat) + '\n';

		if (GWProtList.cardType == GuildPlayerListCardBase.GuildPlayerListCardType.PlayerLocations)
		{
			for (int i = 0; i < GWProtList.SectorList.Count; i++)
			{
				var card = GWProtList.SectorList[i];
				var sectors = card.SectorNamesAll.ToArray();
				string cardData = card.Name + '\n' + "A:" + '\t' + $"({card.AdjustedLevels[0]})" + '\t' + (sectors[0] != null ? string.Join("\t", sectors[0]) : string.Empty) + '\n'
												   + "B:" + '\t' + $"({card.AdjustedLevels[1]})" + '\t' + (sectors[1] != null ? string.Join("\t", sectors[1]) : string.Empty) + '\n'
												   + "C:" + '\t' + $"({card.AdjustedLevels[2]})" + '\t' + (sectors[2] != null ? string.Join("\t", sectors[2]) : string.Empty) + '\n' + '\n';
				result += cardData;
			}
		}
		else
		{
			//var gwPlayersCount = CurrentGuildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot.Count;
			var gwTeamsCount = GWProtList.SectorList.Count;
			var gwPlayersCount = gwTeamsCount / 3;

			for (int i = 0; i < GWProtList.SectorList.Count; i++)
			{
				var card = GWProtList.SectorList[i];
				var sectors = card.SectorNames;
				var teamSurvivors = string.Join("\t", card.TeamInfo.Team.Select(x => x.ActorDefinitionId));
				var teamLevels = string.Join("\t", card.TeamInfo.Team.Select(x => x.AdjustedLevel));
				string cardData = card.Name + '\n'
					+ "Уровень ГВ:" + '\t' + card.AdjustedLevel + "\t" + teamLevels + "\n"
					+ "Команда :" + '\t' + card.TeamIndex + "\t" + teamSurvivors + "\n"
					+ "Индекс участника :" + '\t' + (card.BasePlayerIndex + 1).ToString() + "/" + gwPlayersCount + "\n"
					+ "Индекс команды :" + '\t' + (card.TeamSortedPlayerIndex + 1).ToString() + "/" + gwTeamsCount + "\n"
					+ "Вступление в гильдию :" + '\t' + card.GuildJoinedDate + "\n"
					+ "Локации :" + '\t' + (sectors != null ? string.Join("\t", sectors) : string.Empty) + '\n'
					+ "\n";

				result += cardData;
			}
		}
		MyTools.CopyToClipboard(result);
	}

	public void SetTeamDataFromSlider(UISlider sl)
	{
		if (CurrentGuildModel == null) return;
		var slider = gWTeamsData.First(x => x.indexSlider == sl);
		int indexInList = gWTeamsData.IndexOf(slider);
		var team = gWTeamsData[indexInList];
		team.index = Mathf.RoundToInt(sl.value * AllTeamsSortedList.Count);
		int count = AllTeamsSortedList.Count;
		if (team.index > count - 1) team.index = count - 1;
		team.pvpTeam = AllTeamsSortedList[team.index];
		SetTeamData(team.teamPanel, AllTeamsSortedList[team.index], team.index);
		DebugTWD.Log("setup team " + indexInList);
	}

	public void SetTeamDataFromToggle(UIButtonToggle tg, bool notFocus = true)
	{
		if (CurrentGuildModel == null) return;

		if (currentPlayerID == null || !GwProtectorsInfo.ContainsKey(currentPlayerID))
		{
			if (GWProtList.SectorList != null && GWProtList.SectorList.Count > 0)
			{
				if (GWProtList.cardType == GuildPlayerListCardBase.GuildPlayerListCardType.PlayerLocations)
				{
					var currentPlayer = GWProtList.SectorList.First(x => x.BasePlayerIndex == 0);
					currentPlayerID = currentPlayer.Id;
					currentPlayerBaseIndex = currentPlayer.BasePlayerIndex;
				}
				else
				{
					var teamData = GWProtList.SectorList.First(x => x.TeamSortedPlayerIndex == 0).TeamInfo;
					currentPlayerID = teamData.HashID;
					teamSortedPlayerIndex = GWProtList.SectorList.First(x => x.TeamSortedPlayerIndex == 0).TeamSortedPlayerIndex;
				}
			}
			else return;
		}

		int index = pvpTeamsToggleSet.GetUIButtonToggleList.ToList().IndexOf(tg);
		GuildBattleParticipantInfo teamInfo = GwProtectorsInfo[currentPlayerID];
		List<SurvivorMockData> survivors = teamInfo.SelectedSurvivors;
		string firstSurvivor = survivors.First().ActorDefinitionId;
		string name = teamInfo.Name;
		pvpTeamCurrent = GWTeamUtils.GetPvpTeam(survivors, index);
		string teamIndex = GWTeamUtils.TeamIndexFromTg(index);
		int rankIndex = -1;

		if (GWProtList.cardType == GuildPlayerListCardBase.GuildPlayerListCardType.PlayerLocations)
		{
			var currentPlayer = GWProtList.SectorList.First(x => x.Id == currentPlayerID);
			if (currentPlayer != null)
			{
				currentPlayerBaseIndex = currentPlayer.BasePlayerIndex;
				rankIndex = currentPlayerBaseIndex;
				DebugTWD.Log("Find BasePlayerIndex" + currentPlayerBaseIndex);
			}
			//Debug.Log("select Location card for " + guildMembersInfo[currentPlayerID].Name + " " + currentPlayerBaseIndex + " " + index + " " + survivors.Count);
		}
		else
		{
			var teamData = GWProtList.TeamsForMaps.FirstOrDefault(x => x.HashID == currentPlayerID && x.TeamIndex == teamIndex && x.Team.First().ActorDefinitionId == firstSurvivor);
			if (teamData != null)
			{
				teamSortedPlayerIndex = teamData.TeamSortedPlayerIndex;
				rankIndex = teamSortedPlayerIndex;
				DebugTWD.Log("Find teamSortedPlayerIndex" + teamSortedPlayerIndex);
			}
			//Debug.Log("select team card for " + guildMembersInfo[currentPlayerID].Name + " " + teamSortedPlayerIndex + " " + notFocus + " " + GWProtList.sectorList.Count + " " + index + " " + teamIndex);
		}

		GWTeamPanelCurrent.RefreshSurvivorSlots(pvpTeamCurrent, teamIndex, name, rankIndex);

		if (!notFocus)
			ResetPositionTo();
	}

	public void ResetPositionTo()
	{
		float y;
		if (GWProtList.cardType == GuildPlayerListCardBase.GuildPlayerListCardType.TeamLocations)
		{
			y = (float)teamSortedPlayerIndex / ((float)GWProtList.SectorList.Count - 1);
		}
		else
		{
			y = (float)currentPlayerBaseIndex / ((float)GWProtList.SectorList.Count - 1);
		}
		GWProtList.ResetPositionTo(y);
	}

	public void SetBtLevelsListeners(bool set)
	{
		for (int i = 0; i < btLevelChangeList.Count; i++)
		{
			if (set)
			{
				foreach (var bt in btLevelChangeList)
				{
					bt.levelUp.SetClickCallback(SetLevelUp);
					bt.levelDown.SetClickCallback(SetLevelUp);
				}
			}
			else
			{
				foreach (var bt in btLevelChangeList)
				{
					bt.levelUp.SetClickCallback(SetLevelUp);
					bt.levelDown.SetClickCallback(SetLevelUp);
				}
			}
		}
	}

	[Serializable]
	public class LevelChangeItem
	{
		public UIButtonExtended levelDown;
		public UIButtonExtended levelUp;
		public UILabel signPlus;
		public UILabel signCount;
	}

	public void SetLevelUp(UIButtonExtended bt)
	{
		int index = 0;
		bool isUp = false;
		var itemUp = btLevelChangeList.FirstOrDefault(x => x.levelUp == bt);
		var itemDown = btLevelChangeList.FirstOrDefault(x => x.levelDown == bt);

		if (itemUp == null && itemDown == null) return;

		isUp = itemUp != null;
		LevelChangeItem item = itemUp ?? itemDown;

		switch (btLevelChangeList.IndexOf(item))
		{
			case 0:
				index = 0; break;
			case 1:
				index = 1; break;
			case 2:
				index = 2; break;
			default:
				break;
		}

		pvpTeamCurrent[index].AdjustedLevel += (isUp ? 1 : -1);
		pvpTeamCurrent[index].AdjustedLevelAdd += (isUp ? 1 : -1);

		var actor = pvpTeamCurrent[index].ActorDefinitionId;
		var playerId = pvpTeamCurrent[index].OwnerHashedPlayerId;
		var survivorData = CurrentGuildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData.PlayerInfoSnapshot.FirstOrDefault(x => x.Key == playerId).Value ?? null;

		if (survivorData != null)
		{
			var survivor = survivorData.SelectedSurvivors.FirstOrDefault(x => x.ActorDefinitionId == actor);
			survivor.AdjustedLevel = pvpTeamCurrent[index].AdjustedLevel;
			survivor.AdjustedLevelAdd = pvpTeamCurrent[index].AdjustedLevelAdd;
		}

		DebugTWD.Log(pvpTeamCurrent[index].ActorDefinitionId + " is change level" + (isUp ? " Up" : " down"));

		GWTeamPanelCurrent.RefreshSurvivorSlots();
	}

	private void SetTeamData(GWTeamPanel panel, GuildBattlePvpTeam team, int index)
	{
		var teamInfo = GwProtectorsInfo[team.OwnerHashedPlayerId];
		var survivors = teamInfo.SelectedSurvivors;
		var teamIndex = GWTeamUtils.TeamIndex(survivors.IndexOf(team.Survivors.First()));
		var name = teamInfo.Name;

		panel.RefreshSurvivorSlots(team, teamIndex, name, index);
	}

	//распределение врагов на нашей карте
	public List<GuildBattleTeamInfo> TeamsForOurMaps(GuildModel guildModel)
	{
		DebugTWD.Log("TeamsForOurMaps for " + guildModel.Name);
		GuildBattleModel battleModel = guildModel.GuildWarModel.CurrentBattle;

        string file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
        Dictionary<string, string> PvpTeamsIndexPerMissionCustom = OfflineManager.JsonSerializer.Deserialize<Dictionary<string, string>>(file);

        //команды врагов
        //GwProtectorsInfo = GWTeamUtils.Instance.IsOpponentMaps ? guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot : battleModel.EnemyGuildData.PlayerInfoSnapshot;
        //Dictionary<string, GuildBattleParticipantInfo> enemyInfo = GwProtectorsInfo;
        Dictionary<string, GuildBattleParticipantInfo> enemyInfo = GwProtectorsInfo;

		var dicBbInfoIndex = new Dictionary<string, int>();
		int BbIndex = 0;
		foreach (var b in enemyInfo.Keys)
		{
			dicBbInfoIndex.Add(b, BbIndex);
			BbIndex++;
		}

		string path;
		string data;
        //AllTeamsSortedList = GetAllTeamsSorted(enemyInfo);
        if (StartGWBattle.Instance.IsTryOpenPVPTeamsListPerSectorFromFile && GWTeamUtils.Instance.IsOpponentGuild)
		{
            path = @"e:\Unity Projects\TWD\Projects\Resources\JSON\PvpTeamsListPerSector_origin.json";
			data = File.ReadAllText(path);
            PvpTeamsListPerSector = OfflineManager.JsonSerializer.Deserialize<Dictionary<int, List<GuildBattlePvpTeam>>>(data);
        }
		else
		{
            PvpTeamsListPerSector = battleModel.CurrentMapModel.PVPTeamsListPerSector;// GetPVPTeamsListPerSector(guildModel, AllTeamsSortedList);
        }

  //      data = OfflineManager.JsonSerializer.Serialize(PvpTeamsListPerSector);
	 //   path = @"e:\Unity Projects\TWD\Projects\Resources\JSON\PvpTeamsListPerSector_mod.json";
  //      MyTools.SaveToFile(data, path, append: false);
		//Debug.Log("1.AllTeamsSortedList saved");

		PvpTeamsIndexPerMission = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission;
        var _PvpTeamsIndexPerMission = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission;
        if (_PvpTeamsIndexPerMission != null && _PvpTeamsIndexPerMission.Count > 0)
            PvpTeamsIndexPerMission = _PvpTeamsIndexPerMission;

        List<GuildBattleTeamInfo> GuildBattlePlayerInfoList = new List<GuildBattleTeamInfo>();

		//string data = jsonSerializer.Serialize(AllTeamsSortedList);
		//var path = GWTeamUtils.globalPath + GWTeamUtils.playerFolder + "1.AllTeamsSortedList" + ".json";
		//GWTeamUtils.SaveToFile(data, path, append: false);
		//Debug.Log("1.AllTeamsSortedList saved");

		foreach (var pair in PvpTeamsIndexPerMission)
		{
			GuildBattleMapModel.ParsePvpTeamIndexId(pair.Value, out int sectorId, out int index);
			GuildBattlePvpTeam pvpTeamForMission = PvpTeamsListPerSector[sectorId][index];
			var survivors = enemyInfo[pvpTeamForMission.OwnerHashedPlayerId].SelectedSurvivors;
			SurvivorMockData survivor = null;
			foreach (var s in survivors)
			{
				foreach (var s2 in pvpTeamForMission.Survivors)
				{
					if (s.ActorDefinitionId == s2.ActorDefinitionId)
					{
						survivor = s;
						break;
					}
				}
			}
			if (survivor == null) continue;

			GuildBattleTeamInfo item = GuildBattlePlayerInfoList.FirstOrDefault(x => x.Team == pvpTeamForMission.Survivors);
			PvpTeamsIndexPerMissionCustom.TryGetValue(pair.Key, out string mapID);
			var id = pvpTeamForMission.OwnerHashedPlayerId;

			if (item == null)
			{
				item = new GuildBattleTeamInfo();
				item.Name = enemyInfo[id].Name;
				item.HashID = id;
				item.AdjustedLevel = pvpTeamForMission.AverageAdjustedLevel;
				item.Team = pvpTeamForMission.Survivors;
				item.TeamIndex = GWTeamUtils.TeamIndex(survivors.IndexOf(item.Team.First()));

				item.SectorNames = new List<string> { mapID };
				item.BasePlayerIndex = dicBbInfoIndex[id];

				var opponentGuild = guildModel.Id == GWTeamUtils.Instance.GuildID && OpponentGuilModel != null ? OpponentGuilModel : GuildModel;
				var date = opponentGuild.GetMemberInfo(id)?.GuildJoinedDate ?? 0;
				if (date != 0)
					item.GuildJoinedDate = MyTools.LongToTime(opponentGuild.GetMemberInfo(id).GuildJoinedDate);
				GuildBattlePlayerInfoList.Add(item);
			}
			else
			{
				item.SectorNames.Add(mapID);
			}
		}

		foreach (var team in GuildBattlePlayerInfoList)
		{
			List<List<string>> list;
			if (GWTeamUtils.Instance.IsGenerateCustomTeams)
			{
				list = GuildBattlePlayerInfoList.Where(x => x.AdjustedLevel == team.AdjustedLevel).Select(x => x.SectorNames).ToList();
			}
			else
			{
				list = GuildBattlePlayerInfoList.Where(x => x.HashID == team.HashID).Select(x => x.SectorNames).ToList();
			}

			var sectorsAll = new List<string>();
			var sectorLevels = new List<int>();
			int SectorLevel = 0;

			foreach (var l in list)
			{
				foreach (var s in l)
				{
					var teamLevel = GWTeamUtils.TeamLevel(s);
					SectorLevel += teamLevel;
					sectorLevels.Add(teamLevel);
				}
				sectorsAll.AddRange(l);
			}

			var temp = new List<string>();
			foreach (var s in sectorsAll)
			{
				temp.Add(s);
			}

			sectorsAll.StableSort((string a, string b) => sectorLevels[temp.IndexOf(a)].CompareTo(sectorLevels[temp.IndexOf(b)]));
			team.SectorNamesAll = sectorsAll;
			team.SectorLevel = SectorLevel;
		}

		//GuildBattlePlayerInfoList.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.AdjustedLevel.CompareTo(b.AdjustedLevel));
		//string data2 = jsonSerializer.Serialize(GuildBattlePlayerInfoList);
		//var path2 = GWTeamUtils.globalPath + GWTeamUtils.playerFolder + "1.GuildBattlePlayerInfoList" + ".json";
		//GWTeamUtils.SaveToFile(data2, path2, append: false);
		//Debug.Log("1.GuildBattlePlayerInfoList saved");

		if (GWTeamUtils.Instance.IsGenerateCustomTeams)
		{
			List<GuildBattleTeamInfo> GuildBattlePlayerInfoClean = new List<GuildBattleTeamInfo>();

			foreach (var team in GuildBattlePlayerInfoList)
			{
				if (team.AdjustedLevel == 0) continue;
				var item = GuildBattlePlayerInfoClean.FirstOrDefault(x => x.AdjustedLevel == team.AdjustedLevel);
				if (item == null) GuildBattlePlayerInfoClean.Add(team);
			}

			for (int i = 0; i < GuildBattlePlayerInfoClean.Count; i++)
			{
				var locations = GuildBattlePlayerInfoClean[i].SectorNamesAll.Where(x => GWTeamUtils.TeamLevel(x) >= lowest_location_level).ToList();
				GuildBattlePlayerInfoClean[i].SectorNamesAll = locations;
				DebugTWD.Log("SectorNames " + locations.Count + " for " + GuildBattlePlayerInfoClean[i].AdjustedLevel);
			}

			GuildBattlePlayerInfoClean.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.AdjustedLevel.CompareTo(b.AdjustedLevel));
			return GuildBattlePlayerInfoClean;
		}
		else
		{
			for (int i = 0; i < GuildBattlePlayerInfoList.Count; i++)
			{
				var teamInAllTeamsSortedList = AllTeamsSortedList.First(x => x.OwnerHashedPlayerId == GuildBattlePlayerInfoList[i].HashID
					&& x.Survivors[0].ActorDefinitionId == GuildBattlePlayerInfoList[i].Team[0].ActorDefinitionId);
				GuildBattlePlayerInfoList[i].TeamSortedPlayerIndex = AllTeamsSortedList.IndexOf(teamInAllTeamsSortedList);
			}

			GuildBattlePlayerInfoList.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.TeamSortedPlayerIndex.CompareTo(b.TeamSortedPlayerIndex));
			return GuildBattlePlayerInfoList;
		}
	}

	public Dictionary<int, List<GuildBattlePvpTeam>> GetPVPTeamsListPerSector(GuildModel guildModel, List<GuildBattlePvpTeam> allTeamsSorted)
	{
		var CurrentMapModel = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel;
		var _PVPTeamsListPerSector = new Dictionary<int, List<GuildBattlePvpTeam>>();
		GuildWarDefinition warDefinition = DataManager.Instance.GameData.FindGuildWarWithId(CurrentMapModel.WarDefinitionId);
		if (warDefinition == null) return null;

		List<GuildBattleSectorDefinition> list = DataManager.Instance.GameData.GuildBattleSectorDefinitions.Where((GuildBattleSectorDefinition x) => warDefinition.SectorsIds.Contains(x.Id)).ToList();
		int minVp = list.Min((GuildBattleSectorDefinition x) => x.SectorVP);
		int maxVp = list.Max((GuildBattleSectorDefinition x) => x.SectorVP);
		foreach (GuildBattleSectorDefinition item in list)
		{
			if (!_PVPTeamsListPerSector.TryGetValue(item.Id, out var value))
			{
				value = new List<GuildBattlePvpTeam>();
				GuildBattleMapSectorModel sectorModel = CurrentMapModel.GetSectorModel(item.Id);
				if (sectorModel == null) continue;

				int amountOfTeamsNeeded = sectorModel.RandomizedMissions.Count((GuildBattleMapMissionModel x) => x.Type == GuildBattleMapMissionModel.MissionType.PVP);
				List<GuildBattlePvpTeam> pvpTeamsForSector = GetPvpTeamsForSector(item.SectorVP, minVp, maxVp, allTeamsSorted, amountOfTeamsNeeded);
				value.AddRange(pvpTeamsForSector);
				value?.StableSort((GuildBattlePvpTeam a, GuildBattlePvpTeam b) => a.AverageAdjustedLevel.CompareTo(b.AverageAdjustedLevel));

				_PVPTeamsListPerSector.Add(item.Id, value);
			}
		}

		foreach (KeyValuePair<string, string> item in CurrentMapModel.PvpTeamsIndexPerMission)
		{
			GuildBattleMapModel.ParsePvpTeamIndexId(item.Value, out var sectorId, out var index);
			if (_PVPTeamsListPerSector.TryGetValue(sectorId, out var value) && value.Count > index)
			{
				value[index].MissionId = item.Key;
			}
		}
		return _PVPTeamsListPerSector;
	}

	private List<GuildBattlePvpTeam> GetPvpTeamsForSector(int sectorVp, int minVp, int maxVp, List<GuildBattlePvpTeam> allTeams, int amountOfTeamsNeeded)
	{
		if (amountOfTeamsNeeded == 0)
		{
			return new List<GuildBattlePvpTeam>();
		}

		if (allTeams.Count == 0)
		{
			return new List<GuildBattlePvpTeam>();
		}

		List<GuildBattlePvpTeam> list = new List<GuildBattlePvpTeam>();
		if (amountOfTeamsNeeded >= allTeams.Count)
		{
			int num = 0;
			while (list.Count != amountOfTeamsNeeded)
			{
				list.Add(new GuildBattlePvpTeam(allTeams[num % allTeams.Count].Survivors));
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
			num2 = 0;
			num3 = allTeams.Count;
		}

		if (num3 > allTeams.Count)
		{
			num3 = allTeams.Count;
		}

		for (int i = num2; i < num3; i++)
		{
			list.Add(new GuildBattlePvpTeam(allTeams[i].Survivors));
		}

		return list;
	}
	public List<GuildBattlePvpTeam> GetAllTeamsSorted(Dictionary<string, GuildBattleParticipantInfo> players)
	{
		List<GuildBattlePvpTeam> list = new List<GuildBattlePvpTeam>();
		foreach (var key in players.Keys)
		{
			var value = players[key];
			List<SurvivorMockData> selectedSurvivors = value.SelectedSurvivors;
			if (selectedSurvivors.Count >= 9)
			{
				foreach (var surv in selectedSurvivors)
				{
					surv.OwnerHashedPlayerId = key;
				}

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

	public Dictionary<string, GuildBattleParticipantInfo> GeneratedCustomTeams(Dictionary<string, GuildBattleParticipantInfo> bbInfo)
	{
		if (bbInfo == null) return null;

		//survivorTeamsOrigin = new Dictionary<string, List<SurvivorMockData>>();

		int count_Top_current = 0;
		int count_preTop_current = 0;
		int count_middle_current = 0;
		int count_Low_current = 0;

		foreach (var survivor in bbInfo)
		{
			List<SurvivorMockData> selectedSurvivors = survivor.Value.SelectedSurvivors;
			if (selectedSurvivors.Count >= 9)
			{
				List<SurvivorMockData> team_A;
				List<SurvivorMockData> team_B;
				List<SurvivorMockData> team_C;

				if (count_Top_current < count_Top_base)
				{
					count_Top_current++;
					team_A = gWTeamsData[0].pvpTeam.Survivors;
				}
				else
				{
					if (count_preTop_current < count_preTop_base)
					{
						count_preTop_current++;
						team_A = gWTeamsData[1].pvpTeam.Survivors;
					}
					else
					{
						if (count_middle_current < count_middle_base)
						{
							count_middle_current++;
							team_A = gWTeamsData[2].pvpTeam.Survivors;
						}
						else
						{
							count_Low_current++;
							team_A = gWTeamsData[3].pvpTeam.Survivors;
						}
					}
				}

				if (count_preTop_current < count_preTop_base)
				{
					count_preTop_current++;
					team_B = gWTeamsData[1].pvpTeam.Survivors;
				}
				else
				{
					if (count_middle_current < count_middle_base)
					{
						count_middle_current++;
						team_B = gWTeamsData[2].pvpTeam.Survivors;
					}
					else
					{
						count_Low_current++;
						team_B = gWTeamsData[3].pvpTeam.Survivors;
					}
				}

				if (count_middle_current < count_middle_base)
				{
					count_middle_current++;
					team_C = gWTeamsData[2].pvpTeam.Survivors;
				}
				else
				{
					count_Low_current++;
					team_C = gWTeamsData[3].pvpTeam.Survivors;
				}

				var selectedSurvivorsCustom = new List<SurvivorMockData>
				{
					team_A[0], team_A[1], team_A[2],
					team_B[0], team_B[1], team_B[2],
					team_C[0], team_C[1], team_C[2]
				};

				//survivorTeamsOrigin.Add(survivor.Key, survivor.Value.SelectedSurvivors);
				survivor.Value.SelectedSurvivors = selectedSurvivorsCustom;
			}
		}
		DebugTWD.Log("teams count is :\n" + "Top : " + count_Top_current + "\n"
			+ "PreTop : " + count_preTop_current + "\n"
			+ "Middle : " + count_middle_current + "\n"
			+ "Low : " + count_Low_current + "\n"
			+ "Summ : " + (count_Top_current + count_preTop_current + count_middle_current + count_Low_current).ToString());
		return bbInfo;
	}

	public void BackToOriginTeams()
	{
		if (survivorTeamsOrigin.Count > 0 && survivorTeamsOrigin.Count == GwProtectorsInfo.Count)
		{
			foreach (var survivor in survivorTeamsOrigin)
			{
				GwProtectorsInfo[survivor.Key].SelectedSurvivors = survivor.Value;
			}
			survivorTeamsOrigin.Clear();
			InitData(false);
		}
	}

	#region oldcode
	//-----------------------
	//распределение союзников на вражеской карте (не используем)
	//public List<GuildBattleTeamInfo> TeamsForEnemyMaps(GuildModel guildModel)
	//{
	//    Debug.Log("TeamsForEnemyMaps please exit");
	//    var file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
	//    Dictionary<string, string> PvpTeamsIndexPerMission = jsonSerializer.Deserialize<Dictionary<string, string>>(file);

	//    GwProtectorsInfo = guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;
	//    Dictionary<string, GuildBattleParticipantInfo> bbInfo = GwProtectorsInfo;

	//    var dicBbInfoIndex = new Dictionary<string, int>();
	//    int BbIndex = 0;
	//    foreach (var b in bbInfo.Keys)
	//    {
	//        dicBbInfoIndex.Add(b, BbIndex);
	//        BbIndex++;
	//    }

	//    if (GWTeamUtils.Instance.IsGenerateCustomTeams)
	//    {
	//        bbInfo = GeneratedCustomTeams(bbInfo);
	//    }

	//    AllTeamsSortedList = GetAllTeamsSorted(bbInfo);

	//    //StartGWBattle.Instance.StartBattle();

	//    PvpTeamsListPerSector = GetPVPTeamsListPerSector(guildModel, AllTeamsSortedList);
	//    List<GuildBattleTeamInfo> GuildBattlePlayerInfoList = new List<GuildBattleTeamInfo>();
	//    Dictionary<string, string> dic = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission;

	//    foreach (var pair in dic)
	//    {
	//        //ParsePvpTeamIndexId(pair.Key, out int sectorId, out int index);
	//        GuildBattleMapModel.ParsePvpTeamIndexId(pair.Value, out int sectorId, out int index);
	//        GuildBattlePvpTeam pvpTeamForMission = PvpTeamsListPerSector[sectorId][index];
	//        var survivors = bbInfo[pvpTeamForMission.OwnerHashedPlayerId].SelectedSurvivors;
	//        SurvivorMockData survivor = null;
	//        foreach (var s in survivors)
	//        {
	//            foreach (var s2 in pvpTeamForMission.Survivors)
	//            {
	//                if (s.Name == s2.Name)
	//                {
	//                    survivor = s;
	//                    break;
	//                }
	//            }
	//        }
	//        //if (survivor == null) continue;

	//        GuildBattleTeamInfo item = GuildBattlePlayerInfoList.FirstOrDefault(x => x.Team == pvpTeamForMission.Survivors);
	//        PvpTeamsIndexPerMission.TryGetValue(pair.Key, out string mapID);
	//        var id = pvpTeamForMission.OwnerHashedPlayerId;

	//        if (item == null)
	//        {
	//            item = new GuildBattleTeamInfo();
	//            item.Name = bbInfo[id].Name;
	//            item.HashID = id;
	//            item.AdjustedLevel = pvpTeamForMission.AverageAdjustedLevel;
	//            item.Team = pvpTeamForMission.Survivors;
	//            item.TeamIndex = GWTeamUtils.TeamIndex(survivors.IndexOf(survivor));
	//            item.SectorNames = new List<string> { mapID };
	//            item.BasePlayerIndex = dicBbInfoIndex[id];
	//            item.TeamSortedPlayerIndex = AllTeamsSortedList.IndexOf(AllTeamsSortedList.FirstOrDefault(x => x.OwnerHashedPlayerId ==
	//                id && x.Survivors == pvpTeamForMission.Survivors));
	//            //item.num2 = new List<int> { pvpTeamForMission.num2 };
	//            //item.num3 = new List<int> { pvpTeamForMission.num3 };
	//            var date = guildModel.GetMemberInfo(id)?.GuildJoinedDate ?? 0;
	//            if (date != 0)
	//                item.GuildJoinedDate = CraftSettings.LongToTime(guildModel.GetMemberInfo(id).GuildJoinedDate);
	//            GuildBattlePlayerInfoList.Add(item);
	//        }
	//        else
	//        {
	//            item.SectorNames.Add(mapID);
	//            //item.num2.Add(pvpTeamForMission.num2);
	//            //item.num3.Add(pvpTeamForMission.num3);
	//        }
	//    }

	//    if (GuildBattlePlayerInfoList.Count < AllTeamsSortedList.Count)
	//    {
	//        List<GuildBattleTeamInfo> GuildBattlePlayerInfoListEmpty = new List<GuildBattleTeamInfo>();
	//        foreach (var team in AllTeamsSortedList)
	//        {
	//            var id = team.OwnerHashedPlayerId;

	//            if (GuildBattlePlayerInfoList.FirstOrDefault(x => x.Team == team.Survivors) == null)
	//            {
	//                var itemEmpty = new GuildBattleTeamInfo();
	//                itemEmpty.Name = bbInfo[id].Name;
	//                itemEmpty.HashID = id;
	//                itemEmpty.AdjustedLevel = team.AverageAdjustedLevel;
	//                itemEmpty.Team = team.Survivors;
	//                var survivors = bbInfo[id].SelectedSurvivors;

	//                itemEmpty.TeamIndex = GWTeamUtils.TeamIndex(survivors.IndexOf(team.Survivors.First()));
	//                itemEmpty.SectorNames = new List<string> { };
	//                itemEmpty.BasePlayerIndex = dicBbInfoIndex[id];
	//                itemEmpty.TeamSortedPlayerIndex = AllTeamsSortedList.IndexOf(AllTeamsSortedList.FirstOrDefault(x => x.OwnerHashedPlayerId ==
	//                        id && x.Survivors == team.Survivors));
	//                //itemEmpty.num2 = new List<int> { 0 };
	//                //itemEmpty.num3 = new List<int> { 0 };
	//                itemEmpty.GuildJoinedDate = CraftSettings.LongToTime(guildModel.GetMemberInfo(id).GuildJoinedDate);

	//                GuildBattlePlayerInfoListEmpty.Add(itemEmpty);
	//            }
	//        }
	//        GuildBattlePlayerInfoList.AddRange(GuildBattlePlayerInfoListEmpty);
	//    }

	//    foreach (var team in GuildBattlePlayerInfoList)
	//    {
	//        List<List<string>> list;
	//        if (GWTeamUtils.Instance.IsGenerateCustomTeams)
	//        {
	//            list = GuildBattlePlayerInfoList.Where(x => x.AdjustedLevel == team.AdjustedLevel).Select(x => x.SectorNames).ToList();
	//        }
	//        else
	//        {
	//            list = GuildBattlePlayerInfoList.Where(x => x.HashID == team.HashID).Select(x => x.SectorNames).ToList();
	//        }

	//        var sectorsAll = new List<string>();
	//        var sectorLevels = new List<int>();
	//        int SectorLevel = 0;

	//        foreach (var l in list)
	//        {
	//            foreach (var s in l)
	//            {
	//                var teamLevel = GWTeamUtils.TeamLevel(s);
	//                SectorLevel += teamLevel;
	//                sectorLevels.Add(teamLevel);
	//            }
	//            sectorsAll.AddRange(l);
	//        }

	//        var temp = new List<string>();
	//        foreach (var s in sectorsAll)
	//        {
	//            temp.Add(s);
	//        }

	//        sectorsAll.StableSort((string a, string b) => sectorLevels[temp.IndexOf(a)].CompareTo(sectorLevels[temp.IndexOf(b)]));
	//        team.SectorNamesAll = sectorsAll;
	//        team.SectorLevel = SectorLevel;
	//    }

	//    List<GuildBattleTeamInfo> GuildBattlePlayerInfoClean = null;
	//    if (GWTeamUtils.Instance.IsGenerateCustomTeams)
	//    {
	//        GuildBattlePlayerInfoClean = new List<GuildBattleTeamInfo>();

	//        foreach (var team in GuildBattlePlayerInfoList)
	//        {
	//            if (team.AdjustedLevel == 0) continue;
	//            var item = GuildBattlePlayerInfoClean.FirstOrDefault(x => x.AdjustedLevel == team.AdjustedLevel);
	//            if (item == null) GuildBattlePlayerInfoClean.Add(team);
	//        }

	//        for (int i = 0; i < GuildBattlePlayerInfoClean.Count; i++)
	//        {
	//            var locations = GuildBattlePlayerInfoClean[i].SectorNamesAll.Where(x => GWTeamUtils.TeamLevel(x) >= lowest_location_level).ToList();
	//            GuildBattlePlayerInfoClean[i].SectorNamesAll = locations;
	//            Debug.Log("SectorNames " + locations.Count + " for " + GuildBattlePlayerInfoClean[i].AdjustedLevel);
	//        }

	//        GuildBattlePlayerInfoClean.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.AdjustedLevel.CompareTo(b.AdjustedLevel));
	//    }
	//    else
	//    {
	//        GuildBattlePlayerInfoList.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.TeamSortedPlayerIndex.CompareTo(b.TeamSortedPlayerIndex));
	//        GuildBattlePlayerInfoClean = GuildBattlePlayerInfoList;
	//    }
	//    return GuildBattlePlayerInfoClean;
	//}
	//-----------------------
	#endregion
}
