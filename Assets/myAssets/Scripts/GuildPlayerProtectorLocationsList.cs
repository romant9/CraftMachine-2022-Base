using BaseModel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using static GuildPlayerListCardBase;

public partial class GuildPlayerProtectorLocationsList : ScrollableListPanel<ScoreDataEntry>
{
	[SerializeField]
	[Tooltip("Tabs for contributions, rank.")]
	protected GameObject cardPrefabDetail;
	private UITabs InfoTypesTabs;

	public Color selectedBtColor;

	public List<SectorsDataEntry> SectorList { get; private set; }

	private GuildBattleMatchmakingInfo GwMatchMakingInfo;
	public List<GuildBattleTeamInfo> TeamsForMaps { get; set; }

	private bool IsOpponentMaps;
	private bool IsOpponentGuild;
	public bool IsIndexChanged { get; set; }

	public GameObject PlayerLocationsHat;
	public GameObject PlayerDetailHat;
	public UILabel gwPlayersCount;
	public UILabel gwTeamsCount;

	public GuildModel guildModel { get; private set; }

	[HideInInspector]
	public GuildPlayerListCardType cardType = GuildPlayerListCardType.PlayerLocations;
	public float currentCardPosition {  get; set; }

	private MessageSerializer jsonSerializer => OfflineManager.JsonSerializer;

	public Dictionary<string, string> TeamsForMapsOpponenets { get; set; }


	protected override void Awake()
	{
		base.Awake();
		TeamsForMapsOpponenets = new Dictionary<string, string>();
	}
	private void OnEnable()
	{
		if (InfoTypesTabs != null)
		{
			InfoTypesTabs.OnNewTabSelectedEvent += OnNewTabSelected;
		}
		UIEvent.OnUIEvent += OnUiEvent;
		Setup();
	}

	private void OnDisable()
	{
		if (InfoTypesTabs != null)
		{
			InfoTypesTabs.OnNewTabSelectedEvent -= OnNewTabSelected;
		}
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SocialGuildPlayerChanged")
		{
			Setup();
		}
	}

	//guildModel - GuildModel
	public void OnClickOurMaps()
	{
		GWTeamUtils.Instance.IsOpponentMaps = false;
		GWTeamsManager.Instance.InitData();

		StartStopBattle();

		DebugTWD.Log("On Click Our Maps for " + guildModel.Name + " with protectors of " + GwMatchMakingInfo.GuildName + " " + GwMatchMakingInfo.PlayerInfoSnapshot.Count);

		Setup();
	}

	public void OnClickOpponentMaps()
	{
		//Debug.Log("OnClickOpponentMaps exit");

		GWTeamUtils.Instance.IsOpponentMaps = true;
		GWTeamsManager.Instance.InitData();

		StartStopBattle();

		DebugTWD.Log("On Click Our Maps for " + guildModel.Name + " with protectors of " + GwMatchMakingInfo.GuildName + " " + GwMatchMakingInfo.PlayerInfoSnapshot.Count);

		Setup();

		//var guildModel = GWTeamUtils.Instance.IsOpponentGuild ? GWTeamUtils.Instance.GuildModel : GWTeamUtils.Instance.OpponentGuilModel;
		//GWTeamUtils.Instance.IsOpponentMaps = true;
		//TeamsForMaps = GWTeamUtils.Instance.gwManager.TeamsForEnemyMaps(guildModel);
		//GwMatchMakingInfo = guildModel.GuildWarModel.CurrentBattle.EnemyGuildData;
		//GWTeamsManager.instance.InitData();
		//Setup();
	}

	public void StartStopBattle()
	{
		DebugTWD.Log("Start Battle");

		guildModel = GWTeamsManager.Instance.CurrentGuildModel;

		GuildBattleModel currentBattle = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle;
		string currentBattleSer = jsonSerializer.Serialize(currentBattle);
		//string GwMatchMakingInfoSer = jsonSerializer.Serialize(guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData.PlayerInfoSnapshot);
		//var path = GWTeamUtils.globalPath + GWTeamUtils.playerFolder + "CurrentGuildModel_" + ".txt";
		//GWTeamUtils.SaveToFile(currentBattleSer, path, append: false);
		//GWTeamsManager.instance.SaveAllTeamsSortedList(isSave: false);

		StartGWBattle.Instance.StartBattle(guildModel);
		TeamsForMaps = GWTeamsManager.Instance.TeamsForOurMaps(guildModel);

		TeamsForMapsOpponenets[guildModel.Id] = jsonSerializer.Serialize(TeamsForMaps);

		if (GWTeamUtils.Instance.IsSaveTeams)
		{
			string data = TeamsForMapsOpponenets[guildModel.Id];
			var path = CommandHelper.GlobalPath + "TeamsForMaps.json";
			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log("TeamsForMaps saved: " + path);
		}

		guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle = jsonSerializer.Deserialize<GuildBattleModel>(currentBattleSer);
		guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.SetPlayerOwnerAndGameEconomyData(GameManager.Instance.gameEconomyData, guildModel.GvGSeasonModel, null);

		//var playerInfoSnapshot = jsonSerializer.Deserialize<Dictionary<string, GuildBattleParticipantInfo>>(GwMatchMakingInfoSer);
		//guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData.SetSnapshot(playerInfoSnapshot);

		GwMatchMakingInfo = GWTeamsManager.Instance.GwMatchMakingInfo;
	}

	private void Setup()
	{
		if (GWTeamUtils.Instance.GuildModel != null)
		{
			scrollView.GetComponent<UIPanel>().alpha = 0;

			SortOnOff(true);
			DebugTWD.Log("Setup GuildPlayer Protectors List", DebugType.Wars);

			if (guildModel == null || guildModel != GWTeamsManager.Instance.CurrentGuildModel || TeamsForMaps == null)
			{
				DebugTWD.Log("Clear all", DebugType.Wars);
				ClearCards();
				if (GWTeamUtils.Instance.IsOpponentMaps)
					OnClickOpponentMaps();
				else
					OnClickOurMaps();
				return;
			}

			if (cardType == GuildPlayerListCardType.PlayerLocations)
			{
				DebugTWD.Log("setup GuildPlayer Protectors LocationsList " + GwMatchMakingInfo.PlayerInfoSnapshot.Count, DebugType.Wars);

				SectorList = new List<SectorsDataEntry>();

				List<SectorItem> sectorItems = new List<SectorItem>();
				List<SectorItem> sectorItemsError = new List<SectorItem>();

				foreach (var player in GwMatchMakingInfo.PlayerInfoSnapshot)
				{
					string id = player.Key;
					SectorsDataEntry dataEntry = new SectorsDataEntry(id, player.Value.Name);
					var survivors = TeamsForMaps.Where(x => x.HashID == id);
					//Debug.Log("Add sectors for " + player.Value.Name);
					var lowestLevel = GWTeamUtils.Instance.gwManager.lowest_location_level;
					var a = survivors.FirstOrDefault(x => x.TeamIndex == "A")?.SectorNames?.Where(x => GWTeamUtils.TeamLevel(x) >= lowestLevel)?.ToList() ?? new List<string> { string.Empty };
					SectorItem.AddItems(a, player.Value.Name, "A", ref sectorItems, ref sectorItemsError);
					var b = survivors.FirstOrDefault(x => x.TeamIndex == "B")?.SectorNames?.Where(x => GWTeamUtils.TeamLevel(x) >= lowestLevel)?.ToList() ?? new List<string> { string.Empty };
					SectorItem.AddItems(b, player.Value.Name, "B", ref sectorItems, ref sectorItemsError);
					var c = survivors.FirstOrDefault(x => x.TeamIndex == "C")?.SectorNames?.Where(x => GWTeamUtils.TeamLevel(x) >= lowestLevel)?.ToList() ?? new List<string> { string.Empty };
					SectorItem.AddItems(c, player.Value.Name, "C", ref sectorItems, ref sectorItemsError);
					dataEntry.SectorNamesAll = new List<List<string>> { a, b, c };
					dataEntry.SectorLevel = TeamsForMaps.FirstOrDefault(x => x.HashID == player.Key)?.SectorLevel ?? 0;

					var originSurvivorsA_Level = GWTeamUtils.GetAverageAdjustedLevel(player.Value.SelectedSurvivors.GetRange(0, 3));
					var originSurvivorsB_Level = GWTeamUtils.GetAverageAdjustedLevel(player.Value.SelectedSurvivors.GetRange(3, 3));
					var originSurvivorsC_Level = GWTeamUtils.GetAverageAdjustedLevel(player.Value.SelectedSurvivors.GetRange(6, 3));

					dataEntry.AdjustedLevels = new List<int> { originSurvivorsA_Level, originSurvivorsB_Level, originSurvivorsC_Level };
                    //dataEntry.BasePlayerIndex = GwMatchMakingInfo.PlayerInfoSnapshot.Keys.ToList().IndexOf(id);
                    dataEntry.BasePlayerIndex = GwMatchMakingInfo.PlayerInfoSnapshot.Keys.ToList().IndexOf(id);

                    SectorList.Add(dataEntry);
				}

				SetCards(SectorList, true);
				for (int j = 0; j < cards.Count; j++)
				{
					GuildProtSectorsListCard card = (GuildProtSectorsListCard)cards[j];

					card.Type = cardType;
					card.UpdateUI();
				}

				gwPlayersCount.text = GwMatchMakingInfo.PlayerInfoSnapshot.Count + "/" + guildModel.GuildMembers.Count;
			}
			else
			{
				DebugTWD.Log("setup GuildPlayer Detail LocationsList " + TeamsForMaps.Count);

				//string data = jsonSerializer.Serialize(TeamsForMaps);
				//var path = GWTeamUtils.globalPath + GWTeamUtils.playerFolder + "1.TeamsForMaps" + ".json";
				//GWTeamUtils.SaveToFile(data, path, append: false);
				//Debug.Log("1.TeamsForMaps saved");

				SectorList = new List<SectorsDataEntry>();
				foreach (var team in TeamsForMaps)
				{
					string id = team.Name;
					var lowestLevel = GWTeamUtils.Instance.gwManager.lowest_location_level;
					var sectors = team.SectorNames.Where(x => GWTeamUtils.TeamLevel(x) >= lowestLevel)?.ToList() ?? new List<string> { string.Empty };
					team.SectorNames = sectors;

                    //Искусственная сортировка индексов 1..60 //3f6993e320264b97b76e89d4860e7ba1
					//if (team.HashID == "3f6993e320264b97b76e89d4860e7ba1")
					//{
					//	Debug.Log("dd");
					//}
     //               var teamFromSorted = GWTeamUtils.Instance.gwManager.AllTeamsSortedList.First(x => x.OwnerHashedPlayerId == team.HashID && x.Survivors.First().ActorDefinitionId == team.Team.First().ActorDefinitionId);
					//if (teamFromSorted != null) team.TeamSortedPlayerIndex = GWTeamUtils.Instance.gwManager.AllTeamsSortedList.IndexOf(teamFromSorted);
                    //
					SectorsDataEntry dataEntry = new SectorsDataEntry(team);
					SectorList.Add(dataEntry);
				}

				SetCards(SectorList, true);
				for (int j = 0; j < cards.Count; j++)
				{
					GuildProtDetailsListCard card = (GuildProtDetailsListCard)cards[j];
					card.Type = cardType;
					card.UpdateUI();
					//card.SetRank(cards.Count - j);
				}

				gwTeamsCount.text = TeamsForMaps.Count + "/" + guildModel.GuildMembers.Count * 3;
			}

			var tgIndex = GWTeamsManager.Instance.pvpTeamsToggleSet.GetSelectedIndex();
			if (tgIndex == -1) { tgIndex = 0; }
			var tg = GWTeamsManager.Instance.pvpTeamsToggleSet.GetButton(tgIndex);

			GWTeamsManager.Instance.SetTeamDataFromToggle(tg, true);

			StartCoroutine(ResetPosition(true));
		}
		else
		{
			SetCards(null);
		}
	}


	public void ResetPositionTo(float y)
	{
		if (y == 0)
			scrollView.ResetPosition();
		else
			scrollView.ResetPositionTo(y);
	}

	private IEnumerator ResetPosition(bool isFocus)
	{
		yield return new WaitUntil(() => SectorList != null && SectorList.Count > 0 && cardsContainer.activeInHierarchy && cardsContainer.transform.childCount > 1);
		DebugTWD.Log("reset position");
		PositionCards(true);
		yield return null;
		scrollView.ResetPosition();

		if (isFocus)
		{
			GWTeamsManager.Instance.ResetPositionTo();
		}
		scrollView.GetComponent<UIPanel>().alpha = 1;
	}

	public void SortByBaseIndex(UIInput baseIndexUI)
	{
		int newIndex = -1;
		if (int.TryParse(baseIndexUI.value, out int index))
		{
			newIndex = index - 1;
		}
		else return;

		Dictionary<string, GuildBattleParticipantInfo> snapshot = GwMatchMakingInfo.PlayerInfoSnapshot;
		if (newIndex < 0 || newIndex > snapshot.Count - 1) return;
		var cardThis = cards.FirstOrDefault(x => ((GuildProtSectorsListCard)x).baseIndexAddInput == baseIndexUI);
		if (cardThis == null)
		{
			DebugTWD.Log("не могу найти карточку " + newIndex);
			return;
		}
		var cardThisId = cardThis.Item.Id;

		var cardThisData = SectorList.FirstOrDefault(x => x.Id == cardThisId);
		var thisIndex = cardThisData.BasePlayerIndex;
		DebugTWD.Log("new index is " + newIndex + " " + cardThis.Item.Name + "  old index : " + cardThisData.BasePlayerIndex);
		var list1 = SectorList;

		if (newIndex == thisIndex) return;

		IsIndexChanged = true;

		if (newIndex > thisIndex)
		{
			for (int i = 0; i < list1.Count; i++)
			{
				var survivorData = list1[i];
				if (i < thisIndex || i > newIndex)
				{
					DebugTWD.Log("не двигаем " + i);
				}
				else if (i > thisIndex && i <= newIndex)
				{
					DebugTWD.Log("уменьшаем " + i);

					survivorData.BasePlayerIndex--;

				}
				else if (i == thisIndex && survivorData.BasePlayerIndex != newIndex)
				{
					DebugTWD.Log("меняем " + i + " на " + newIndex);

					survivorData.BasePlayerIndex = newIndex;
				}
			}
		}
		else
		{
			DebugTWD.Log("другой случай ");

			for (int i = 0; i < list1.Count; i++)
			{
				var survivorData = list1[i];
				if (i < newIndex || i > thisIndex)
				{
					DebugTWD.Log("не двигаем " + i);
				}
				else if (i >= newIndex && i < thisIndex)
				{
					DebugTWD.Log("увеличиваем " + i);

					survivorData.BasePlayerIndex++;

				}
				else if (i == thisIndex && survivorData.BasePlayerIndex != newIndex)
				{
					DebugTWD.Log("меняем " + i + " на " + newIndex);

					survivorData.BasePlayerIndex = newIndex;
				}
			}
		}

		foreach (var player in snapshot)
		{
			var reIndex = list1.FirstOrDefault(x => x.Id == player.Key).BasePlayerIndex;
			player.Value.PlayerBaseIndex = reIndex;
		}

		Dictionary<string, GuildBattleParticipantInfo> snapshot2 = new Dictionary<string, GuildBattleParticipantInfo>();
		var list2 = list1.OrderBy(x => x.BasePlayerIndex).ToList();
		foreach (var player2 in list2)
		{
			var key = player2.Id;
			var val = snapshot[key];
			snapshot2.Add(key, val);
		}

		GwMatchMakingInfo.SetSnapshot(snapshot2);
	}

	protected override void SetCard(UIListCard<ScoreDataEntry> card)
	{
		base.SetCard(card);
	}

	private void OnNewTabSelected(int tabindex)
	{
		Setup();
	}

	public void ChangeDetailListType(UIToggle tg)
	{
		GuildPlayerListCardType oldType = cardType;
		if (tg.value)
		{
			cardType = GuildPlayerListCardType.TeamLocations;
			PlayerLocationsHat.SetActive(false);
			PlayerDetailHat.SetActive(true);
			ChangeCardType(cardPrefabDetail);
		}
		else
		{
			cardType = GuildPlayerListCardType.PlayerLocations;
			PlayerLocationsHat.SetActive(true);
			PlayerDetailHat.SetActive(false);
			ChangeCardType(null);
		}

		if (oldType != cardType)
		{
			TeamsForMaps = null;
			Setup();
		}
	}
	public void UpdateUIImmediate()
	{
		foreach (var card in cards)
		{
			card.UpdateUI();
		}
	}

	public void OpenAlert()
	{
		AlertPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
		if (confirmationPopup != null)
		{
			string text = "Защита в бою - Защитники, сохраненные перед началом боя.\n" +
				"   - Состояние защиты не меняется до начала следующего боя.\n" +
				"   - Актуально использовать в день битвы.\n" +
				"Защита сейчас - Защитники на текущий момент.\n" +
				"   Состояние защиты меняется после:\n" +
				"   - Изменения/сохранения команд защиты участниками гильдии\n" +
				"   - Ухода или вступления в гильдию\n" +
				"   - окончания регистрации на следующий бой\n" +
				"   С началом следующего боя 'Защита сейчас' становится 'Защитой в бою'";

			MyTools.CopyToClipboard(text);
			confirmationPopup.SetTransform(new Vector2(550, 260), 30, NGUIText.Alignment.Left);
			confirmationPopup.SetContent("", text);
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			confirmationPopup.SetCallbacks(delegate
			{
				confirmationPopup.Close();
			});
			confirmationPopup.Open();
		}
	}

	private class SectorItem
	{
		public string SectorName;
		public string PlayerName;
		public string TeamSign;

		public SectorItem(string sname, string pname, string team, ref List<SectorItem> sectorItems, ref List<SectorItem> sectorItemsError)
		{
			SectorName = sname;
			PlayerName = pname;
			TeamSign = team;

			if (!Add(ref sectorItems))
			{
				Add(ref sectorItemsError);
			}
		}

		public static void AddItems(List<string> snames, string pname, string team, ref List<SectorItem> sectorItems, ref List<SectorItem> sectorItemsError)
		{
			if (snames != null && snames.Count > 0)
			{
				foreach (var s in snames)
				{
					new SectorItem(s, pname, team, ref sectorItems, ref sectorItemsError);
				}
			}
		}

		public bool Add(ref List<SectorItem> sectorItems)
		{
			if (!sectorItems.Contains(this))
			{
				sectorItems.Add(this);
				return true;
			}
			else
			{
				DebugTWD.LogWarning($"Дублирование локаций: {SectorName},  {PlayerName}, {TeamSign}", DebugType.Wars);
				return false;
			}
		}
	}
}
