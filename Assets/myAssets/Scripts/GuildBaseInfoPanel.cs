using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class GuildBaseInfoPanel : MonoBehaviour
{
	private GuildModel guildModel => GWTeamUtils.Instance.CurrentGuildModel;
	private string allText = string.Empty;
	private List<string> allTextList = new List<string>();

	public UITextList InfoList;

	public bool IsGetData = false;

	private GuildBattleMatchmakingInfo matchmakingInfoDeserialized; //new opponent

	private string opponentOldGuildId;

	public UILabel ErrorText;

	private long GetDefendersCooldown()
	{
		return DataManager.Instance.Player.UtcTimestampLastGvgDefendersUpdate + DataManager.Instance.GameData.ConfigData.GvGDefendersCooldown;// - DataManager.Instance.Player.UtcTimeStamp;
	}

	void Start()
	{

	}

	private void OnEnable()
	{
		if (guildModel != null)
		{
			SetWarDataUI();
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(WaitForGuild());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator WaitForGuild()
	{
		yield return new WaitUntil(() => guildModel != null);
		yield return new WaitForEndOfFrame();
		SetWarDataUI();
	}

	public void SetWarDataUI()
	{
		InfoList.Clear();
		allTextList.Clear();
		allText = string.Empty;

		bool isNewEnemy = false;

		var origin = guildModel.GuildWarModel.NextBattlesOpponentMatchmakingInfo;
		if (origin != null && origin.Count > 0)
		{
			var OpponentMatchmakingInfoData = origin[0].OpponentMatchmakingInfo;
			if (!string.IsNullOrEmpty(OpponentMatchmakingInfoData))
			{
				isNewEnemy = true;
				matchmakingInfoDeserialized = OfflineManager.JsonSerializer.DeserializeObject<GuildBattleMatchmakingInfo>(OpponentMatchmakingInfoData);
				//var enemyInfo = matchmakingInfoDeserialized.PlayerInfoSnapshot;
			}
		}
		else matchmakingInfoDeserialized = null;

		var beginString = "Статистика Войны Гильдий" + "\n-----------------";
		allTextList.Add(beginString);
		var seasonNumber = guildModel.GvGSeasonModel.SeasonDefinitionId;
		var battleStateText = "Сезон : " + seasonNumber.ToString();
		allTextList.Add(battleStateText);
		var warNumber = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.WarId;
		var warNumberText = "Война : " + warNumber.ToString();
		allTextList.Add(warNumberText);
		var enemyName = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildName;
		var newEnemyText = "";
		string enemyNameText = "Текущий Соперник : " + enemyName;
		allTextList.Add(enemyNameText);

		if (isNewEnemy && matchmakingInfoDeserialized.GuildName != enemyName)
		{
			newEnemyText = "Определен новый соперник";
			allTextList.Add(newEnemyText);
			enemyNameText = "Новый Соперник : " + matchmakingInfoDeserialized.GuildName;
			allTextList.Add(enemyNameText);
		}

		var battleState = "Статус : " + guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.CurrentState.ToString();
		allTextList.Add(battleState);
		var battleResult = "Результат : " + guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.BattleResult.ToString();
		allTextList.Add(battleResult);
		var gvgDefendersCount = "Число записей о защитниках : " + guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot.Count.ToString() + " / " + guildModel.GuildMembers.Count;
		allTextList.Add(gvgDefendersCount);
		var gwCurrentStartBattleTimeSlotTime = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.TimeSlot;
		var gwCurrentStartBattleTimeSlotTimeString = "Начало текущего боя : " + MyTools.LongToTime(gwCurrentStartBattleTimeSlotTime);
		allTextList.Add(gwCurrentStartBattleTimeSlotTimeString);
		var gwCurrentEndBattleTimeSlotTime = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EndBattleTimestamp;
		var gwCurrentEndBattleTimeSlotTimeString = "Конец текущего боя : " + MyTools.LongToTime(gwCurrentEndBattleTimeSlotTime);
		allTextList.Add(gwCurrentEndBattleTimeSlotTimeString);
		bool isBattleActive = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.IsBattleActiveForTimeSlot(gwCurrentStartBattleTimeSlotTime, DataManager.Instance.Player.UtcTimeStamp);
		var currentBattleStatus = "Текущий бой " + (isBattleActive ? "не" : "") + " закончен";
		allTextList.Add(currentBattleStatus);
		var utcTimestampLastGvgDefendersUpdate = MyTools.LongToTime(DataManager.Instance.Player.UtcTimestampLastGvgDefendersUpdate);
		var utcTimestampLastGvgDefendersUpdateText = "Обновление ваших защитников : " + utcTimestampLastGvgDefendersUpdate;
		allTextList.Add(utcTimestampLastGvgDefendersUpdateText);
		var getDefendersCooldownDate = MyTools.LongToTime(GetDefendersCooldown());
		var getDefendersCooldownDateText = "Заморозка ваших защитников : " + getDefendersCooldownDate;
		allTextList.Add(getDefendersCooldownDateText);
		var timeNextUpdateForGvgBattleEntries = MyTools.LongToTime(guildModel.GvGSeasonModel.GuildWarModel.timeNextUpdateForGvgBattleEntries);
		var timeNextUpdateForGvgBattleEntriesText = "Последнее обновление записей битвы : " + timeNextUpdateForGvgBattleEntries;
		allTextList.Add(timeNextUpdateForGvgBattleEntriesText);
		var middleString = "-----------------\nСтатистика Гильдии" + "\n-----------------";
		allTextList.Add(middleString);
		var membersCount = "Число участников : " + guildModel.GuildMembers.Count;
		allTextList.Add(membersCount);
		var guildCreated = guildModel.Created.ToLocalTime().ToString(UserPrefsKeys.TimeFormat);
		var guildCreatedText = "Гильдия создана : " + guildCreated;
		allTextList.Add(guildCreatedText);
		var guildLifeTime = MyTools.ToReadableString(DateTime.Now.ToLocalTime() - guildModel.Created.ToLocalTime());
		var guildLifeTimeText = "Прошло времени с основания : " + guildLifeTime;
		allTextList.Add(guildLifeTimeText);
		var utcTimeStamp = MyTools.LongToTime(DataManager.Instance.Player.UtcTimeStamp);
		var utcTimeStampText = "Последняя активность в профиле : " + utcTimeStamp;
		allTextList.Add(utcTimeStampText);
		allText = string.Join("\n", allTextList);
		InfoList.Add(allText);
	}

	public void CopyToClipboardGWData()
	{
		MyTools.CopyToClipboard(allText);
	}

	void Update()
	{
		if (IsGetData)
		{
			IsGetData = false;
			SetWarDataUI();
		}
	}

	public void EnableNewOpponent()
	{
		SwitchOpponent(true);
	}

	public void EnableOldOpponent()
	{
		SwitchOpponent(false);
	}

	public void SwitchOpponent(bool isNew)
	{
		string textRu;
		if (matchmakingInfoDeserialized == null)
		{
			textRu = "Новый соперник не определился";
			MyTools.OpenAlert(textRu);
			//ErrorText.text = textRu;
			//ErrorText.GetComponent<TweenAlpha>().PlayForward();
			return;
		}

		if (string.IsNullOrEmpty(GWTeamUtils.Instance.OpponentGuildID) || matchmakingInfoDeserialized.GroupId == GWTeamUtils.Instance.OpponentGuildID)
		{
			textRu = "Соперник уже применен";
			MyTools.OpenAlert(textRu);
			//ErrorText.text = textRu;
			//ErrorText.GetComponent<TweenAlpha>().PlayForward();
			return;
		}

		if (isNew)
		{
			if (!string.IsNullOrEmpty(GWTeamUtils.Instance.OpponentGuildID))
			{
				opponentOldGuildId = GWTeamUtils.Instance.OpponentGuildID;
			}
			GWTeamUtils.Instance.OpponentGuildID = matchmakingInfoDeserialized.GroupId;
			GWTeamUtils.Instance.LoadGuildData(isOpponent: true);
		}
		else
		{
			if (!string.IsNullOrEmpty(opponentOldGuildId))
			{
				GWTeamUtils.Instance.OpponentGuildID = opponentOldGuildId;
				GWTeamUtils.Instance.LoadGuildData(isOpponent: true);
				opponentOldGuildId = null;
			}
		}
	}
}
