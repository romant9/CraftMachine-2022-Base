public class SurvivorInfoBadgesOverview : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivosBadgesOverview;
	}

	public override void Enter()
	{
		base.Enter();
		Helpers.GameObjectSetActive(base.SurvivorBadgesPanel, value: true);
		Helpers.GameObjectSetActive(base.SurvivorRightSidePanel, value: true);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Extend);
		base.SurvivorRightSidePanel.SetSelectedIndex(1);
		base.SurvivorBadgesPanel.CheckIfFirstTime();
	}

	protected override void UpdateAndShowTraits()
	{
	}
}
