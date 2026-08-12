public class MissionHubGvGStateWarOngoing : MissionHubGvGStateBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.WarOnGoing);
	}

	public override void Enter()
	{
		labelLocalization = LocalizationManager.GetText(MissionHubGvGStateLocalizations.WaitingForBattleToStart);
		SetNumberOfParticipants();
		Helpers.GameObjectSetActive(TimerGameobject, value: false);
		Helpers.GameObjectSetActive(GuildBattleParticipantsContainer, value: true);
		LocationTexture.material = GvGDefaultMaterial;
		Helpers.GameObjectSetActive(BattleActiveEffect, value: false);
		Helpers.GameObjectSetActive(ProgressBar, value: false);
		Helpers.GameObjectSetActive(BattleTimerLabel, value: false);
		base.Enter();
	}

	protected override void UpdateUIDynamic()
	{
		if (GuildWarHelper.CheckIfNextBattleExists())
		{
			if (GuildWarHelper.IsLastMinuteBeforeBattleStart())
			{
				HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.BattleStarting));
			}
			else
			{
				HelpersUI.SetContentToLabel(BattleDescription, labelLocalization + " " + MissionHubGvGStateBase.FormatTimeLeft(GuildWarHelper.GetTimeLeftToNextAvailableBattleStart()));
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.WarEnds) + " " + MissionHubGvGStateBase.FormatTimeLeft(GuildWarHelper.GetTimeLeftToCurrentWarEnd()));
		}
	}

	public override void UpdateUI()
	{
		SetNumberOfParticipants();
	}
}
