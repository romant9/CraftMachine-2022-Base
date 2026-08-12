using TWDModel;

public class GvgStartBattleStateBattleStartBase : GvgStartBattleStateBase
{
	public override void Enter()
	{
		Helpers.GameObjectSetActive(StartBattleContainer, value: true);
		Helpers.GameObjectSetActive(FakeBattleStartContainer, value: false);
		Helpers.GameObjectSetActive(RewardsStatsToggle, value: true);
		RewardsStatsToggle.OnToggleValueChanged += RewardsStatsOnToggleValueChanged;
		base.Enter();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(EnemyGuildEmblemEmpty, value: false);
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null)
		{
			if (currentBattle.IsFakeBattle)
			{
				Helpers.GameObjectSetActive(EnemyGuildEmblem, value: false);
				Helpers.GameObjectSetActive(FakeEnemyGuildEmblem, value: true);
				FakeEnemyGuildEmblem.Setup();
			}
			else
			{
				Helpers.GameObjectSetActive(FakeEnemyGuildEmblem, value: false);
				Helpers.GameObjectSetActive(EnemyGuildEmblem, value: true);
				EnemyGuildEmblem.Setup();
			}
			HelpersUI.SetContentToLabel(RewardPointsRewardLabel, ("x " + (currentBattle.GetGuildBattleVictoryRewardPointsMultiplier() + 1f)).ToString());
			HelpersUI.SetContentToLabel(VpRewardLabel, ("x " + (currentBattle.GetGuildBattleVictoryPointsMultiplier() + 1f)).ToString());
			HelpersUI.SetContentToLabel(DrawRewardPointsRewardLabel, ("x " + (currentBattle.GetGuildBattleDrawRewardPointsMultiplier() + 1f)).ToString());
			HelpersUI.SetContentToLabel(VpDrawRewardLabel, ("x " + (currentBattle.GetGuildBattleDrawPointsMultiplier() + 1f)).ToString());
			Helpers.GameObjectSetActive(GoButton, value: true);
		}
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

	public override void Exit()
	{
		RewardsStatsToggle.OnToggleValueChanged -= RewardsStatsOnToggleValueChanged;
		base.Exit();
	}

	public override bool AllowExit()
	{
		return true;
	}

	private void RewardsStatsOnToggleValueChanged(bool ison)
	{
		if (ison)
		{
			ShowRewards();
		}
		else
		{
			ShowStats();
		}
	}

	protected virtual void ShowRewards()
	{
		Helpers.GameObjectSetActive(HighScoreContainer, value: false);
		Helpers.GameObjectSetActive(RewardsContainer, value: true);
	}

	protected virtual void ShowStats()
	{
		Helpers.GameObjectSetActive(HighScoreContainer, value: true);
		Helpers.GameObjectSetActive(RewardsContainer, value: false);
	}
}
