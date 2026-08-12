using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class GuildBattleHighscoresPanel : HUDElement
{
	[SerializeField]
	private UILabel sectorScoreOwn;

	[SerializeField]
	private UILabel sectorScoreEnemy;

	[SerializeField]
	private UILabel participantsAmountLabel;

	[SerializeField]
	private UILabel enemyParticipantsAmountLabel;

	[SerializeField]
	private NUIScrollableList playersLeaderboardList;

	[SerializeField]
	private string DoubleLeaderBoardPrefabName;

	[SerializeField]
	private float refreshInterval = 1.5f;

	private float refreshTimer;

	private void OnEnable()
	{
		UpdateScores();
		SubscribeForEvents();
	}

	private void OnDisable()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.CurrentBattle.Changed -= OnBattleChanged;
		}
		EventManager.OnEvent -= OnEvent;
	}

	public override void Update()
	{
		base.Update();
		refreshTimer -= Time.deltaTime;
		if (refreshTimer <= 0f)
		{
			SingularityMonoBehaviour<GuildWarManager>.Instance.RequestBattleHighscoresUpdate();
			refreshTimer = refreshInterval;
		}
	}

	private void SubscribeForEvents()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.CurrentBattle.Changed -= OnBattleChanged;
			guildWarModel.CurrentBattle.Changed += OnBattleChanged;
		}
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
	}

	private void UpdateScores()
	{
		if (playersLeaderboardList.uiScrollView.isPressed || !GuildWarHelper.IsGuildMember())
		{
			return;
		}
		Vp = 0;
		EnemyVp = 0;
		int maxPlayerCountInBattle = GameManager.Instance.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		GuildBattleModel currentBattle = GuildWarHelper.GetGuildWarModel().CurrentBattle;
		List<ScoreDataEntry> list;
		if (currentBattle.HasStarted())
		{
			list = currentBattle.PlayerHighscores;

			if (IsLoadDataManager && (list == null || list.Count == 0))
			{
				list = GuildWarHelper.GetGuildWarModel().GuildBattleResults.FirstOrDefault().Value?.PlayerScores ?? null;
			}
		}
		else
		{
			GuildWarHelper.GetGuildWarModel().GuildBattleResults.TryGetValue(currentBattle.TimeSlot, out var value);
			list = ((value == null) ? currentBattle.PlayerHighscores : value.PlayerScores);
		}
		string guildId = GameManager.Instance.playerModel.GuildId;
		List<GuildBattleHighscoresEntry> highScoresEntries = new List<GuildBattleHighscoresEntry>();
		List<string> list2 = new List<string>(GuildWarHelper.GetCurrentBattle().RegisteredPlayers);
		List<string> list3 = new List<string>(GuildWarHelper.GetCurrentBattle().EnemyGuildData.RegisteredPlayersList);
		int entryIndex = 0;
		int entryIndex2 = 0;
		HelpersUI.SetContentToLabel(sectorScoreOwn, "0");
		HelpersUI.SetContentToLabel(sectorScoreEnemy, "0");

		if (!IsLoadDataManager)
		{
			for (int i = 0; i < (list?.Count ?? 0); i++)
			{
				GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry = list[i] as GuildBattlePlayersScoreDataEntry;
				bool flag = guildBattlePlayersScoreDataEntry.GuildId == guildId;
				if (guildBattlePlayersScoreDataEntry.Name == currentBattle.BattleId)
				{
					if (flag)
					{
						HelpersUI.SetContentToLabel(sectorScoreOwn, guildBattlePlayersScoreDataEntry.Score.ToString());
					}
					else
					{
						HelpersUI.SetContentToLabel(sectorScoreEnemy, guildBattlePlayersScoreDataEntry.Score.ToString());
					}
					continue;
				}
				if (flag)
				{
					AddPlayerEntry(guildBattlePlayersScoreDataEntry, toLeftSide: true, ref entryIndex, ref highScoresEntries);
				}
				else
				{
					AddPlayerEntry(guildBattlePlayersScoreDataEntry, toLeftSide: false, ref entryIndex2, ref highScoresEntries);
				}
				list2.Remove(guildBattlePlayersScoreDataEntry.Id);
				list3.Remove(guildBattlePlayersScoreDataEntry.Id);
			}
			Dictionary<string, GuildBattleParticipantInfo> playerInfoSnapshot = GameManager.Instance.playerModel.GuildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;
			for (int j = 0; j < list2.Count; j++)
			{
				string text = list2[j];
				if (playerInfoSnapshot.ContainsKey(text))
				{
					GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry2 = new GuildBattlePlayersScoreDataEntry();
					guildBattlePlayersScoreDataEntry2.Name = playerInfoSnapshot[text].Name;
					guildBattlePlayersScoreDataEntry2.Id = text;
					guildBattlePlayersScoreDataEntry2.PlayerEmblem = playerInfoSnapshot[text].PlayerEmblem;
					AddPlayerEntry(guildBattlePlayersScoreDataEntry2, toLeftSide: true, ref entryIndex, ref highScoresEntries);
				}
			}
			if (!GuildWarHelper.GetCurrentBattle().IsFakeBattle)
			{
				playerInfoSnapshot = GuildWarHelper.GetCurrentBattle().EnemyGuildData.PlayerInfoSnapshot;
				for (int k = 0; k < list3.Count; k++)
				{
					string text = list3[k];
					if (playerInfoSnapshot.ContainsKey(text))
					{
						GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry3 = new GuildBattlePlayersScoreDataEntry();
						guildBattlePlayersScoreDataEntry3.Name = playerInfoSnapshot[text].Name;
						guildBattlePlayersScoreDataEntry3.Id = text;
						guildBattlePlayersScoreDataEntry3.PlayerEmblem = playerInfoSnapshot[text].PlayerEmblem;
						AddPlayerEntry(guildBattlePlayersScoreDataEntry3, toLeftSide: false, ref entryIndex2, ref highScoresEntries);
					}
				}
			}
		}
		else
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("Расчет VP дл составов");

			List<GuildBattlePlayersScoreDataEntry> scoreList = null;
			var guildOwn = GWTeamUtils.Instance.GuildModel;
			currentBattle = guildOwn.GuildWarModel.CurrentBattle;
			var guildOpponent = GWTeamUtils.Instance.OpponentGuilModel;
			GuildBattleModel currentBattleEnemy = guildOpponent.GuildWarModel.CurrentBattle;
			List<GuildBattlePlayersScoreDataEntry> scoreListOwn = currentBattle.GetPlayerScoresFromGuildDataCustom(guildOwn);
			List<GuildBattlePlayersScoreDataEntry> scoreListOpponent = currentBattleEnemy.GetPlayerScoresFromGuildDataCustom(guildOpponent);

			if (scoreListOwn != null && scoreListOpponent != null)
			{
				scoreListOwn.AddRange(scoreListOpponent);
				scoreList = scoreListOwn;
			}

			if (IsRealTimeResults)
			{
				Dictionary<string, GuildBattleParticipantInfo> playerInfoSnapshot = GameManager.Instance.playerModel.GuildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;
				Dictionary<string, GuildBattleParticipantInfo> playerEnemySnapshot2 = GameManager.Instance.playerModel.GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.PlayerInfoSnapshot;

				for (int i = 0; i < (list?.Count ?? 0); i++)
				{
					if (i == 0)
					{
						Vp += (int)list[i].Score;
						HelpersUI.SetContentToLabel(sectorScoreOwn, list[i].Score.ToString());
					}
					else if (i == 1)
					{
						EnemyVp += (int)list[i].Score;
						HelpersUI.SetContentToLabel(sectorScoreEnemy, list[i].Score.ToString());
					}
					else
					{
						string text = list[i].Id;
						if (playerInfoSnapshot.ContainsKey(text))
						{
							Vp += (int)list[i].Score;

							GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry2 = new GuildBattlePlayersScoreDataEntry();
							guildBattlePlayersScoreDataEntry2.Score = list[i].Score;
							guildBattlePlayersScoreDataEntry2.Name = playerInfoSnapshot[text].Name;
							guildBattlePlayersScoreDataEntry2.Id = text;
							guildBattlePlayersScoreDataEntry2.PlayerEmblem = playerInfoSnapshot[text].PlayerEmblem;
							AddPlayerEntry(guildBattlePlayersScoreDataEntry2, toLeftSide: true, ref entryIndex, ref highScoresEntries);
						}
						else if (playerEnemySnapshot2.ContainsKey(text))
						{
							EnemyVp += (int)list[i].Score;

							GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry3 = new GuildBattlePlayersScoreDataEntry();
							guildBattlePlayersScoreDataEntry3.Score = list[i].Score;
							guildBattlePlayersScoreDataEntry3.Name = playerEnemySnapshot2[text].Name;
							guildBattlePlayersScoreDataEntry3.Id = text;
							guildBattlePlayersScoreDataEntry3.PlayerEmblem = playerEnemySnapshot2[text].PlayerEmblem;
							AddPlayerEntry(guildBattlePlayersScoreDataEntry3, toLeftSide: false, ref entryIndex2, ref highScoresEntries);
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < (scoreList?.Count ?? 0); i++)
				{
					GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry = scoreList[i];
					bool flag = guildBattlePlayersScoreDataEntry.GuildId == guildId;

					if (flag)
					{
						if (guildBattlePlayersScoreDataEntry.Name == currentBattle.BattleId)
						{
							HelpersUI.SetContentToLabel(sectorScoreOwn, guildBattlePlayersScoreDataEntry.Score.ToString());
						}
						else
						{
							if (guildBattlePlayersScoreDataEntry.Score < 1036) AddPlayerEntry(guildBattlePlayersScoreDataEntry, toLeftSide: true, ref entryIndex, ref highScoresEntries);
						}
						Vp += (int)scoreList[i].Score;
					}
					else
					{
						if (guildBattlePlayersScoreDataEntry.Name == currentBattleEnemy.BattleId || guildBattlePlayersScoreDataEntry.Name == currentBattle.BattleId)
						{
							HelpersUI.SetContentToLabel(sectorScoreEnemy, guildBattlePlayersScoreDataEntry.Score.ToString());
						}
						else
						{
							if (guildBattlePlayersScoreDataEntry.Score < 1036) AddPlayerEntry(guildBattlePlayersScoreDataEntry, toLeftSide: false, ref entryIndex2, ref highScoresEntries);
						}
						EnemyVp += (int)scoreList[i].Score;
					}
				}
			}
		}

		bool num = playersLeaderboardList.currentItemsCount == 0;
		playersLeaderboardList.UpdateWithList(highScoresEntries, DoubleLeaderBoardPrefabName, null, callUpdateUI: true);
		playersLeaderboardList.SortAndRepositionItems();
		if (num)
		{
			playersLeaderboardList.ResetScrollPosition();
		}
		int num2 = entryIndex;
		int num3 = entryIndex2;
		HelpersUI.SetContentToLabel(participantsAmountLabel, $"{num2}/{maxPlayerCountInBattle}");
		HelpersUI.SetContentToLabel(enemyParticipantsAmountLabel, $"{num3}/{maxPlayerCountInBattle}");
	}

	private void AddPlayerEntry(GuildBattlePlayersScoreDataEntry entry, bool toLeftSide, ref int entryIndex, ref List<GuildBattleHighscoresEntry> highScoresEntries)
	{
		if (highScoresEntries.Count <= entryIndex)
		{
			highScoresEntries.Add(new GuildBattleHighscoresEntry());
		}
		if (toLeftSide)
		{
			highScoresEntries[entryIndex].playerA = entry;
		}
		else
		{
			highScoresEntries[entryIndex].playerB = entry;
		}
		entryIndex++;
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.GroupModelLoaded)
		{
			SubscribeForEvents();
		}
	}

	private void OnBattleChanged(TWDGroupModelChild modelObject, string changed, object args)
	{
		if (changed == "GuildBattleScoresUpdated")
		{
			UpdateScores();
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private bool IsRealTimeResults;
	int Vp = 0;
	int EnemyVp = 0;
	#endregion

	#region mycode
	public void UpdateScoreExt(bool isReal, out int vp, out int enemyvp)
	{
		IsRealTimeResults = isReal;
		UpdateScores();
		vp = Vp;
		enemyvp = EnemyVp;
	}
	#endregion
}
