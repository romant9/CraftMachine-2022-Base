using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GvGCalendarWarDay : MonoBehaviour
{
	public UIButtonExtended button;

	[Header("Time of the week states")]
	[SerializeField]
	private UILabel weekdayText;

	[SerializeField]
	private GameObject todayIndicator;

	[SerializeField]
	private GameObject signedUpIndicator;

	[SerializeField]
	private GameObject selectedIndicator;

	[SerializeField]
	private GameObject pastIndicator;

	[SerializeField]
	private GvGCalendarPlayerLight[] calendarPlayerLights;

	[Header("Guild Battle states")]
	[SerializeField]
	private UIGuildBattleLockdownTimer lockdownTimer;

	[SerializeField]
	private UIGuildBattleTimerFixed battleStartTimer;

	[SerializeField]
	private GameObject battleOnGoing;

	[SerializeField]
	private GameObject battleWon;

	[SerializeField]
	private GameObject battleLost;

	[SerializeField]
	private GameObject battleDraw;

	[SerializeField]
	private GameObject battleNotHappened;

	private const string weekdayLocalization = "Generic.Time.WeekdaySmall.";

	public void FillWarDay(KeyValuePair<long, List<string>> registeredPlayersForTimeslot)
	{
		ClearState();
		if (registeredPlayersForTimeslot.Value.Count >= GameManager.Instance.playerModel.gameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
		{
			for (int i = 0; i < registeredPlayersForTimeslot.Value.Count; i++)
			{
				calendarPlayerLights[i].SetState(GvGCalendarPlayerLight.CalendarPlayerLightState.FullSlot);
			}
		}
		else
		{
			for (int j = 0; j < registeredPlayersForTimeslot.Value.Count; j++)
			{
				calendarPlayerLights[j].SetState(GvGCalendarPlayerLight.CalendarPlayerLightState.SignedUp);
			}
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		Helpers.GameObjectSetActive(signedUpIndicator, registeredPlayersForTimeslot.Value.Contains(playerModel.HashedId));
		if ((GuildWarHelper.GetCurrentBattle()?.TimeSlot == registeredPlayersForTimeslot.Key && !GuildWarHelper.GetCurrentBattle().HasEnded()) || (!GuildWarHelper.IsBattleOnGoing() && DateTimeOffset.FromUnixTimeMilliseconds(registeredPlayersForTimeslot.Key).UtcDateTime.Date == DateTime.UtcNow.Date))
		{
			Helpers.GameObjectSetActive(todayIndicator, value: true);
		}
		else if (registeredPlayersForTimeslot.Key < playerModel.UtcTimeStamp)
		{
			Helpers.GameObjectSetActive(pastIndicator, value: true);
		}
		DateTime utcDateTime = DateTimeOffset.FromUnixTimeMilliseconds(registeredPlayersForTimeslot.Key).UtcDateTime;
		HelpersUI.SetContentToLabel(weekdayText, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Time.WeekdaySmall." + utcDateTime.DayOfWeek));
		SetGuildBattleState(registeredPlayersForTimeslot.Key);
	}

	private void SetGuildBattleState(long timeSlot)
	{
		GuildWarModel guildWarModel = GameManager.Instance.playerModel.GuildWarModel;
		if (guildWarModel != null)
		{
			GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
			if (currentBattle != null && currentBattle.TimeSlot == timeSlot && !GuildWarHelper.GetCurrentBattle().HasEnded())
			{
				Helpers.GameObjectSetActive(battleOnGoing, value: true);
			}
			else if (guildWarModel.GuildBattleResults.ContainsKey(timeSlot))
			{
				GuildBattleResultInfo guildBattleResultInfo = guildWarModel.GuildBattleResults[timeSlot];
				Helpers.GameObjectSetActive(battleWon, guildBattleResultInfo.BattleResult == GuildBattleModel.GuildBattleResult.Victory);
				Helpers.GameObjectSetActive(battleLost, guildBattleResultInfo.BattleResult == GuildBattleModel.GuildBattleResult.Defeat);
				Helpers.GameObjectSetActive(battleDraw, guildBattleResultInfo.BattleResult == GuildBattleModel.GuildBattleResult.Draw);
			}
			else if ((timeSlot <= GameManager.Instance.playerModel.UtcTimeStamp && !GuildWarHelper.HasMatchmakingEntryForTimeSlot(timeSlot)) || (GuildWarHelper.IsLockDownTimeForTimeSlotClientSide(timeSlot) && !guildWarModel.HasEnoughRegisteredPlayersToStartBattleForTimeSlot(timeSlot)))
			{
				Helpers.GameObjectSetActive(battleNotHappened, value: true);
			}
			else if (GuildWarHelper.IsLockDownTimeForTimeSlotClientSide(timeSlot))
			{
				battleStartTimer?.SetBattleTimeSlotForTimer(timeSlot);
			}
			else
			{
				lockdownTimer?.SetBattleTimeSlotForTimer(timeSlot);
			}
		}
	}

	public void SelectGuildWarDay(bool selected)
	{
		Helpers.GameObjectSetActive(selectedIndicator, selected);
	}

	private void ClearState()
	{
		SetStateToAllLights(GvGCalendarPlayerLight.CalendarPlayerLightState.Empty);
		Helpers.GameObjectSetActive(todayIndicator, value: false);
		Helpers.GameObjectSetActive(signedUpIndicator, value: false);
		Helpers.GameObjectSetActive(pastIndicator, value: false);
		Helpers.GameObjectSetActive(battleStartTimer, value: false);
		Helpers.GameObjectSetActive(battleOnGoing, value: false);
		Helpers.GameObjectSetActive(battleWon, value: false);
		Helpers.GameObjectSetActive(battleLost, value: false);
		Helpers.GameObjectSetActive(battleDraw, value: false);
		Helpers.GameObjectSetActive(battleNotHappened, value: false);
		Helpers.GameObjectSetActive(lockdownTimer, value: false);
	}

	private void SetStateToAllLights(GvGCalendarPlayerLight.CalendarPlayerLightState state)
	{
		for (int i = 0; i < calendarPlayerLights.Length; i++)
		{
			calendarPlayerLights[i].SetState(state);
		}
	}
}
