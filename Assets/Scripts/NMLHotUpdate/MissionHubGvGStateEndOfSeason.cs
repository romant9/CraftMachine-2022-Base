using TWDModel;

public class MissionHubGvGStateEndOfSeason : MissionHubGvGStateBase
{
	private GvGSeasonDefinition nextSeason;

	public override void Init()
	{
		base.Init();
		SetState(States.EndOfSeason);
	}

	public override void Enter()
	{
		nextSeason = GuildWarHelper.GetGvGSeasonModel().FindNextSeason(GameManager.Instance.playerModel.UtcTimeStamp);
		if (nextSeason != null)
		{
			if (!nextSeason.IsOpen(GameManager.Instance.playerModel.UtcTimeStamp))
			{
				HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.EndOfSeason));
				labelLocalization = LocalizationManager.GetText(MissionHubGvGStateLocalizations.SeasonStartsIn);
			}
			else
			{
				HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.NoSeasonActive));
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.NoSeasonActive));
		}
		Helpers.GameObjectSetActive(GuildBattleParticipantsContainer, value: false);
		Helpers.GameObjectSetActive(BattleTimerLabel, value: false);
		Helpers.GameObjectSetActive(ProgressBar.gameObject, value: false);
		Helpers.GameObjectSetActive(BattleActiveEffect, value: false);
		base.Enter();
	}

	protected override void UpdateUIDynamic()
	{
		if (TimerLabel != null && nextSeason != null)
		{
			HelpersUI.SetContentToLabel(TimerLabel, labelLocalization + " " + MissionHubGvGStateBase.FormatTimeLeft(nextSeason.TimeUntilStartMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp)));
		}
	}
}
