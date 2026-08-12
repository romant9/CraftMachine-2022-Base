using TWDModel;

public class GvgStartBattleStateFakeBattleStarted : GvgStartBattleStateBattleStartBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.FakeBattleStarted);
	}

	public override void Enter()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.MapTeamSelection);
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText("GuildBattleStartPopup.BattleStarted"));
		Helpers.GameObjectSetActive(StartBattleContainer, value: false);
		Helpers.GameObjectSetActive(FakeBattleStartContainer, value: true);
		FakeBattleStartContainer.Setup();
		TweenManager.PlayTweenGroup(PopupContainter, 10, forward: true, OnTweenComplete);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(GuildEmblem, value: false);
		Helpers.GameObjectSetActive(EnemyGuildEmblemEmpty, value: false);
		Helpers.GameObjectSetActive(EnemyGuildEmblem, value: false);
		Helpers.GameObjectSetActive(FakeEnemyGuildEmblem, value: false);
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		HelpersUI.SetContentToLabel(FakeBattleRewardPointsRewardLabel, ("x " + (currentBattle.GetGuildBattleVictoryRewardPointsMultiplier() + 1f)).ToString());
		HelpersUI.SetContentToLabel(FakeBattleVpRewardLabel, ("x " + (currentBattle.GetGuildBattleVictoryPointsMultiplier() + 1f)).ToString());
		HelpersUI.SetContentToLabel(DrawFakeBattleRewardPointsRewardLabel, "x " + (currentBattle.GetGuildBattleDrawRewardPointsMultiplier() + 1f));
		HelpersUI.SetContentToLabel(DrawFakeBattleVpRewardLabel, "x " + (currentBattle.GetGuildBattleDrawPointsMultiplier() + 1f));
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText("GuildBattleStartPopup.FakeBattleStarted"));
		Helpers.GameObjectSetActive(GoButton, value: true);
	}

	protected override void UpdateUIDynamic()
	{
		HelpersUI.SetContentToLabel(MatchTimeLabel, GuildWarHelper.GetFormatedTimeLeftToCurrentBattleEnd());
		if (GuildWarHelper.IsLastMinuteForBattleEnd())
		{
			HelpersUI.SetColor(MatchTimeLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.LastMinuteWarningLabelColor);
		}
		else
		{
			HelpersUI.SetColor(MatchTimeLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NormalTimerColor);
		}
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
