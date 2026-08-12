public class GvgStartBattleStateSpectating : GvgStartBattleStateBattleStartBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.Spectating);
	}

	public override void Enter()
	{
		base.Enter();
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText(MissionHubGvGStateBase.MissionHubGvGStateLocalizations.GuildBattleOnGoing));
		TweenManager.FinishTweenGroup(PopupContainter, 10, includeInactive: true);
		TweenManager.PlayTweenGroup(PopupContainter, 11);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		RewardsStatsToggle.SetToggleState(toggleState: false);
		Helpers.GameObjectSetActive(GoButton, value: false);
	}

	protected override void UpdateUIDynamic()
	{
		HelpersUI.SetContentToLabel(MatchTimeLabel, GuildWarHelper.GetFormatedTimeLeftToCurrentBattleEnd());
		bool flag = GuildWarHelper.IsBattleOnGoing();
		if ((flag && GuildWarHelper.IsLastMinuteForBattleEnd()) || (!flag && GuildWarHelper.IsLastMinuteBeforeBattleStart()))
		{
			HelpersUI.SetColor(MatchTimeLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.LastMinuteWarningLabelColor);
		}
		else
		{
			HelpersUI.SetColor(MatchTimeLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NormalTimerColor);
		}
	}

	public override bool AllowExit()
	{
		return GuildWarHelper.IsPlayerRegisteredForBattle();
	}
}
