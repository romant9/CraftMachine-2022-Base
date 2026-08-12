using System;
using TWDModel;
using UnityEngine;

public class GvGCalendarWarDayDetails : MonoBehaviour
{
	[SerializeField]
	private GvGCalendarPlayerDetails[] gvgCalendarPlayerDetails;

	[SerializeField]
	private UIButton registerButton;

	[SerializeField]
	private UIButton resignButton;

	[SerializeField]
	private UILabel dayLabel;

	[SerializeField]
	private UILabel startTimeUTC;

	[SerializeField]
	private UILabel requiredPlayersLabel;

	[SerializeField]
	private UIGuildBattleLockdownTimer signUpTimer;

	[SerializeField]
	private UIGuildBattleTimerFixed battleStartTimer;

	[SerializeField]
	private UISprite playersRequiredBG;

	private long selectedTimeslot = -1L;

	private GuildWarModel guildWar;

	private const string battleStartsLocalizationString = "GvG.Hub.Calendar.BattleStarts{time}";

	private const string lockdownTimeEndsLocalizationString = "GvG.Hub.Calendar.SignUpEnds{time}";

	private const string guildBattleSelectedDayLocalizationString = "GvG.Hub.Calendar.SelectedDay{day}";

	private const string requiredPlayersLocalizationString = "GvG.Hub.Calendar.RequiredPlayers{registered}{required}";

	private const string weekdayLocalization = "Generic.Time.WeekdaySmall.";

	private void OnEnable()
	{
		guildWar = GuildWarHelper.GetGuildWarModel();
		if (guildWar != null)
		{
			guildWar.Changed += OnGuildWarModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.GuildBattleLockdownTimeEvent)
		{
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		if (guildWar != null)
		{
			guildWar.Changed -= OnGuildWarModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
	}

	private void OnGuildWarModelChanged(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattlePlayerRegistered" || changed == "GuildBattlePlayerResigned")
		{
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		int count = guildWar.RegisteredPlayersForBattleSlot[selectedTimeslot].Count;
		int minPlayersToStartBattle = GameManager.Instance.gameEconomyData.GuildWarConfig.MinPlayersToStartBattle;
		bool flag = count >= minPlayersToStartBattle;
		for (int i = 0; i < gvgCalendarPlayerDetails.Length; i++)
		{
			if (i < count)
			{
				gvgCalendarPlayerDetails[i].SetPlayerInfo(guildWar.RegisteredPlayersForBattleSlot[selectedTimeslot][i], selectedTimeslot);
			}
			else
			{
				gvgCalendarPlayerDetails[i].SetFreeSlot();
			}
		}
		HelpersUI.SetContentToLabel(requiredPlayersLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.Calendar.RequiredPlayers{registered}{required}", count, Mathf.Max(count, minPlayersToStartBattle)));
		playersRequiredBG.gradientBottom = (flag ? SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.ValidColorGradientBottom : SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.InvalidColorGradientBottom);
		playersRequiredBG.gradientTop = (flag ? SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.ValidColorGradientTop : SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.InvalidColorGradientTop);
		signUpTimer?.SetBattleTimeSlotForTimer(selectedTimeslot, "GvG.Hub.Calendar.SignUpEnds{time}", deactivateWhenTimerAtZero: true);
		battleStartTimer?.SetBattleTimeSlotForTimer(selectedTimeslot, "GvG.Hub.Calendar.BattleStarts{time}", deactivateWhenTimerAtZero: true);
		if (GuildWarHelper.IsPlayerRegisteredForBattle(selectedTimeslot))
		{
			Helpers.GameObjectSetActive(registerButton, value: false);
			Helpers.GameObjectSetActive(resignButton, value: true);
			PlayerModel playerModel = GameManager.Instance.playerModel;
			bool flag2 = !GuildWarHelper.IsLockDownTimeForTimeSlotClientSide(selectedTimeslot) && guildWar.CanPlayerResignFromBattleSlot(selectedTimeslot, playerModel.HashedId, playerModel.UtcTimeStamp);
			HelpersUI.SetButtonState(resignButton, (!flag2) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		}
		else if (guildWar.RegisteredPlayersForBattleSlot[selectedTimeslot].Count < GameManager.Instance.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle)
		{
			PlayerModel playerModel2 = GameManager.Instance.playerModel;
			Helpers.GameObjectSetActive(registerButton, value: true);
			Helpers.GameObjectSetActive(resignButton, value: false);
			int num = guildWar.GetAllValidRegisteredDaysForPlayer(playerModel2.HashedId, playerModel2.UtcTimeStamp) + playerModel2.GvGSeasonModelPlayer.GuildWarModelPlayer.GetBattleParticipationsOnPreviousGuilds();
			bool flag3 = !GuildWarHelper.IsLimitRegisted() && guildWar.CanPlayerRegisterForBattleSlot(selectedTimeslot, playerModel2.HashedId, playerModel2.UtcTimeStamp) && !GuildWarHelper.IsLockDownTimeForTimeSlotClientSide(selectedTimeslot) && num < playerModel2.GetCurrency(CurrencyType.BattlePass).Value;
			HelpersUI.SetButtonState(registerButton, (!flag3) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		}
		else
		{
			Helpers.GameObjectSetActive(registerButton, value: false);
			Helpers.GameObjectSetActive(resignButton, value: false);
		}
	}

	public void SetupDayRegisteredPlayers(long timeslot, int day)
	{
		if (guildWar != null && timeslot != -1)
		{
			HelpersUI.SetContentToLabel(dayLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.Calendar.SelectedDay{day}", day));
			DateTime utcDateTime = DateTimeOffset.FromUnixTimeMilliseconds(timeslot).UtcDateTime;
			string content = string.Format("{0} {1} UTC", SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Time.WeekdaySmall." + utcDateTime.DayOfWeek), utcDateTime.ToString("HH:mm"));
			HelpersUI.SetContentToLabel(startTimeUTC, content);
			selectedTimeslot = timeslot;
			UpdateUI();
		}
	}

	public void Register()
	{
		SingularityMonoBehaviour<GuildWarManager>.Instance.RegisterToGuildBattle(selectedTimeslot);
	}

	public void Withdraw()
	{
		SingularityMonoBehaviour<GuildWarManager>.Instance.ResignFromGuildBattle(selectedTimeslot);
	}
}
