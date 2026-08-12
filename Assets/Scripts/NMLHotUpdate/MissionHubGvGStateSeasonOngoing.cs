using TWDModel;

public class MissionHubGvGStateSeasonOngoing : MissionHubGvGStateBase
{
	private GuildWarDefinition nextWar;

	public override void Init()
	{
		base.Init();
		SetState(States.SeasonOngoing);
	}

	public override void Enter()
	{
		nextWar = GuildWarHelper.GetGuildWarModel().FindNextGuildWar(GameManager.Instance.playerModel.UtcTimeStamp);
		if (nextWar != null)
		{
			if (!nextWar.IsOpen(GameManager.Instance.playerModel.UtcTimeStamp))
			{
				HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.PrepareForNextWar));
				LocationTexture.material = PrepareForWarMaterial;
				labelLocalization = LocalizationManager.GetText(MissionHubGvGStateLocalizations.WarStartsIn);
			}
			else
			{
				HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.NoWarActive));
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText(MissionHubGvGStateLocalizations.NoWarActive));
		}
		Helpers.GameObjectSetActive(GuildBattleParticipantsContainer, value: false);
		Helpers.GameObjectSetActive(ProgressBar, value: false);
		Helpers.GameObjectSetActive(BattleTimerLabel, value: false);
		base.Enter();
	}

	protected override void UpdateUIDynamic()
	{
		if (nextWar != null && !nextWar.IsOpen(GameManager.Instance.playerModel.UtcTimeStamp) && TimerLabel != null)
		{
			HelpersUI.SetContentToLabel(TimerLabel, labelLocalization + " " + MissionHubGvGStateBase.FormatTimeLeft(GuildWarHelper.GetTimeLeftToNextWar()));
		}
	}
}
