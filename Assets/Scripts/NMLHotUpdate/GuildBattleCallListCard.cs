using UnityEngine;

public class GuildBattleCallListCard : GuildBattleLogListCard
{
	[Header("Current/Next Battle")]
	[SerializeField]
	private UILabel battleTimeLabel;

	[SerializeField]
	private UILabel participantsLabel;

	[SerializeField]
	private UILabel joinButtonLabel;

	[SerializeField]
	private float refreshInterval = 1f;

	private float refreshTimer;

	public override void UpdateUI()
	{
		int registeredPlayersCountForBattleTimeSlot = GuildWarHelper.GetRegisteredPlayersCountForBattleTimeSlot();
		int maxPlayerCountInBattle = GameManager.Instance.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		HelpersUI.SetContentToLabel(participantsLabel, $"{registeredPlayersCountForBattleTimeSlot}/{maxPlayerCountInBattle}");
		if (GuildWarHelper.IsBattleOnGoing())
		{
			HelpersUI.SetContentToLabel(battleTimeLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.GuildBattleOngoing"));
			HelpersUI.SetContentToLabel(joinButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.JoinBattle"));
			return;
		}
		HelpersUI.SetContentToLabel(battleTimeLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.WaitingForBattleToStart") + " " + GuildWarHelper.GetFormatedTimeLeftToNextAvailableBattleStart());
		if (GuildWarHelper.IsPlayerRegisteredForBattle() || registeredPlayersCountForBattleTimeSlot >= maxPlayerCountInBattle)
		{
			HelpersUI.SetContentToLabel(joinButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.JoinBattle"));
		}
		else
		{
			HelpersUI.SetContentToLabel(joinButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.SignUpForBattle"));
		}
	}

	private void Update()
	{
		refreshTimer -= Time.deltaTime;
		if (refreshTimer < 0f)
		{
			UpdateUI();
			refreshTimer = refreshInterval;
		}
	}

	public void OnClickBattle()
	{
		CampManager.Instance.GoToGuildBattleMap();
	}
}
