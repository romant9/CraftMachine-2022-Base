public class MissionHubGvGStateLocked : MissionHubGvGStateBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.Locked);
	}

	public override void Enter()
	{
		CheckLockedState();
		Helpers.GameObjectSetActive(BattleActiveEffect, value: false);
		Helpers.GameObjectSetActive(ProgressBar, value: false);
		Helpers.GameObjectSetActive(BattleTimerLabel, value: false);
		Helpers.GameObjectSetActive(TimerGameobject, value: false);
		Helpers.GameObjectSetActive(GuildBattleParticipantsContainer, value: false);
		Helpers.GameObjectSetActive(BattleDescription, value: false);
		base.Enter();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		CheckLockedState();
	}

	private void CheckLockedState()
	{
		if (GuildWarHelper.IsLockedByCouncilLevel())
		{
			HelpersUI.SetContentToLabel(LockedLabel, LocalizationManager.GetText(MissionHubGvGStateLocalizations.GuildWarUnlockAtLevel, GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtCouncilLevel));
		}
		else if (GuildWarHelper.IsLockedByTutorial())
		{
			HelpersUI.SetContentToLabel(LockedLabel, LocalizationManager.GetText(MissionHubGvGStateLocalizations.GuildWarUnlockAfterTutorial));
		}
		else if (!GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled)
		{
			HelpersUI.SetContentToLabel(LockedLabel, LocalizationManager.GetText(MissionHubGvGStateLocalizations.NotAvailable));
		}
		else if (!GameManager.Instance.playerModel.IsGuildMember)
		{
			HelpersUI.SetContentToLabel(LockedLabel, LocalizationManager.GetText(MissionHubGvGStateLocalizations.GuildWarJoinGuild));
		}
		Helpers.GameObjectSetActive(BattleActiveEffect, value: false);
	}
}
