using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleHighscoresEndBattle : HUDElement
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

	public void SetScores(GuildBattleResultInfo guildBattleResult)
	{
		if (playersLeaderboardList.uiScrollView.isPressed)
		{
			return;
		}
		int maxPlayerCountInBattle = GameManager.Instance.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		List<ScoreDataEntry> playerScores = guildBattleResult.PlayerScores;
		string guildId = GameManager.Instance.playerModel.GuildId;
		List<GuildBattleHighscoresEntry> highScoresEntries = new List<GuildBattleHighscoresEntry>();
		List<string> list = new List<string>(guildBattleResult.RegisteredPlayers);
		List<string> list2 = new List<string>(guildBattleResult.EnemyRegisteredPlayers);
		int entryIndex = 0;
		int entryIndex2 = 0;
		string value = ((!guildBattleResult.isFakeBattle && guildId.CompareTo(guildBattleResult.EnemyGroupId) != 1) ? (guildBattleResult.EnemyGroupId + "_" + guildId) : (guildId + "_" + (guildBattleResult.isFakeBattle ? "Fake" : guildBattleResult.EnemyGroupId)));
		HelpersUI.SetContentToLabel(sectorScoreOwn, "0");
		HelpersUI.SetContentToLabel(sectorScoreEnemy, "0");
		for (int i = 0; i < (playerScores?.Count ?? 0); i++)
		{
			GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry = playerScores[i] as GuildBattlePlayersScoreDataEntry;
			bool flag = guildBattlePlayersScoreDataEntry.GuildId == guildId;
			if (guildBattlePlayersScoreDataEntry.Name.Contains(value))
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
			list.Remove(guildBattlePlayersScoreDataEntry.Id);
			list2.Remove(guildBattlePlayersScoreDataEntry.Id);
		}
		Dictionary<string, GuildBattleParticipantInfo> playerInfoSnapshot = GameManager.Instance.playerModel.GuildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;
		for (int j = 0; j < list.Count; j++)
		{
			string text = list[j];
			if (playerInfoSnapshot.ContainsKey(text))
			{
				GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry2 = new GuildBattlePlayersScoreDataEntry();
				guildBattlePlayersScoreDataEntry2.Name = playerInfoSnapshot[text].Name;
				guildBattlePlayersScoreDataEntry2.Id = text;
				guildBattlePlayersScoreDataEntry2.PlayerEmblem = playerInfoSnapshot[text].PlayerEmblem;
				AddPlayerEntry(guildBattlePlayersScoreDataEntry2, toLeftSide: true, ref entryIndex, ref highScoresEntries);
			}
		}
		if (!guildBattleResult.isFakeBattle)
		{
			for (int k = 0; k < list2.Count; k++)
			{
				string text = list2[k];
				if (guildBattleResult.EnemyLeaderboardInfo.ContainsKey(text))
				{
					GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry3 = new GuildBattlePlayersScoreDataEntry();
					guildBattlePlayersScoreDataEntry3.Name = guildBattleResult.EnemyLeaderboardInfo[text].Key;
					guildBattlePlayersScoreDataEntry3.Id = text;
					guildBattlePlayersScoreDataEntry3.PlayerEmblem = guildBattleResult.EnemyLeaderboardInfo[text].Value;
					AddPlayerEntry(guildBattlePlayersScoreDataEntry3, toLeftSide: false, ref entryIndex2, ref highScoresEntries);
				}
			}
		}
		playersLeaderboardList.UpdateWithList(highScoresEntries, DoubleLeaderBoardPrefabName, null, callUpdateUI: true);
		playersLeaderboardList.SortAndRepositionItems();
		playersLeaderboardList.ResetScrollPosition();
		int num = entryIndex;
		int num2 = entryIndex2;
		HelpersUI.SetContentToLabel(participantsAmountLabel, $"{num}/{maxPlayerCountInBattle}");
		HelpersUI.SetContentToLabel(enemyParticipantsAmountLabel, $"{num2}/{maxPlayerCountInBattle}");
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
}
