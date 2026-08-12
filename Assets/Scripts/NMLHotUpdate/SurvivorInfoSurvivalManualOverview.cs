public class SurvivorInfoSurvivalManualOverview : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivalManual;
	}

	public override void Enter()
	{
		base.Enter();
		Helpers.GameObjectSetActive(base.SurvivalManualPanel, value: true);
		Helpers.GameObjectSetActive(base.SurvivorTraitsList, value: false);
		Helpers.GameObjectSetActive(base.SurvivorRightSidePanel, value: true);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Show);
		base.SurvivorRightSidePanel.SetSelectedIndex(2);
	}

	protected override void UpdateAndShowTraits()
	{
	}
}
