public class GvgStartBattleStateBattleActive : GvgStartBattleStateBattleStartBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.BattleActive);
	}

	public override void Enter()
	{
		base.Enter();
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText(MissionHubGvGStateBase.MissionHubGvGStateLocalizations.GuildBattleOnGoing));
		TweenManager.FinishTweenGroup(PopupContainter, 10, includeInactive: true);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		RewardsStatsToggle.SetToggleState(toggleState: false);
	}
}
