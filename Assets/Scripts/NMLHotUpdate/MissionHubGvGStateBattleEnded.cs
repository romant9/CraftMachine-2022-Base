public class MissionHubGvGStateBattleEnded : MissionHubGvGStateBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.BattleEnded);
	}

	public override void Enter()
	{
		labelLocalization = LocalizationManager.GetText(MissionHubGvGStateLocalizations.WaitingForBattleToStart);
		SetNumberOfParticipants();
		Helpers.GameObjectSetActive(TimerGameobject, value: false);
		LocationTexture.material = GvGDefaultMaterial;
		Helpers.GameObjectSetActive(BattleActiveEffect, value: false);
		Helpers.GameObjectSetActive(ProgressBar, value: false);
		if (GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle())
		{
			BattleTimerLabel.color = SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NormalTimerColor;
			HelpersUI.SetContentToLabel(BattleTimerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(MissionHubGvGStateLocalizations.BattleEndedCollectYourRewards));
		}
		else
		{
			Helpers.GameObjectSetActive(BattleTimerLabel, value: false);
		}
		HelpersUI.SetContentToLabel(BattleDescription, LocalizationManager.GetText("GvG.Alert.BattleEnded.Title"));
		base.Enter();
	}

	public override void UpdateUI()
	{
		SetNumberOfParticipants();
	}
}
