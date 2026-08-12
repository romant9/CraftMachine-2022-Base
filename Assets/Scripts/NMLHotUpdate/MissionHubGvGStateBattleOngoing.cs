public class MissionHubGvGStateBattleOngoing : MissionHubGvGStateBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.BattleOnGoing);
	}

	public override void Enter()
	{
		HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.GuildBattleOnGoing));
		Helpers.GameObjectSetActive(BattleTimerLabel, value: true);
		Helpers.GameObjectSetActive(BattleActiveEffect, value: true);
		Helpers.GameObjectSetActive(GuildBattleParticipantsContainer, value: true);
		Helpers.GameObjectSetActive(ProgressBar, value: true);
		UpdateUI();
		base.Enter();
	}

	public override void UpdateUI()
	{
		SetNumberOfParticipants();
		ProgressBar.UpdateUI();
	}

	protected override void UpdateUIDynamic()
	{
		HelpersUI.SetContentToLabel(BattleTimerLabel, GuildWarHelper.GetFormatedTimeLeftToCurrentBattleEnd(roundLastMinute: true, lastMinuteWarning: false));
		if (GuildWarHelper.IsLastMinuteForBattleEnd())
		{
			HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.BattleEnding));
			HelpersUI.SetColor(BattleTimerLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.LastMinuteWarningLabelColor);
		}
		else
		{
			HelpersUI.SetColor(BattleTimerLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NormalTimerColor);
		}
	}
}
