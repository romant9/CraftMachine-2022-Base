public class GvgStartBattleStateBattleStarted : GvgStartBattleStateBattleStartBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.BattleStarted);
	}

	public override void Enter()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.MapTeamSelection);
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText("GuildBattleStartPopup.BattleStarted"));
		base.Enter();
		TweenManager.PlayTweenGroup(PopupContainter, 10, forward: true, OnTweenComplete);
	}

	public override void Exit()
	{
		base.Exit();
		GuildWarHelper.SendHasSeenGuildBattleStartFlagCommand();
	}

	private void OnTweenComplete()
	{
		TweenManager.FinishTweenGroup(PopupContainter, 10, includeInactive: true);
	}
}
