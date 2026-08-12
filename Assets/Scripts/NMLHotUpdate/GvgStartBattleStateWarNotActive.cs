public class GvgStartBattleStateWarNotActive : GvgStartBattleStateBase
{
	public override void Init()
	{
		base.Init();
		SetState(States.WarNotActive);
	}

	public override void Enter()
	{
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText("GuildBattleStartPopup.WarNotActive"));
		base.Enter();
		Helpers.GameObjectSetActive(CloseButton, value: false);
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(GuildEmblem, value: false);
		Helpers.GameObjectSetActive(GuildEmblemEmpty, value: true);
		Helpers.GameObjectSetActive(EnemyGuildEmblemEmpty, value: true);
		Helpers.GameObjectSetActive(EnemyGuildEmblem, value: false);
		Helpers.GameObjectSetActive(FakeBattleStartContainer, value: false);
		Helpers.GameObjectSetActive(FakeEnemyGuildEmblem, value: false);
		Helpers.GameObjectSetActive(MatchTimeLabel, value: false);
	}

	public override bool AllowExit()
	{
		return GuildWarHelper.IsWarOngoing();
	}
}
