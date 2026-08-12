public class SurvivorInfoStateReject : SurvivorInfoStateOverviewLimited
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivoreRejectOnly;
	}

	public override void Enter()
	{
		base.Enter();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.RetireButton != null && base.SurvivorModel != null)
		{
			Helpers.GameObjectSetActive(base.RetireButton, !base.SurvivorModel.IsHero && !base.SurvivorModel.IsUpgrading() && !base.SurvivorModel.IsFavourite);
		}
	}

	protected override void UpdateAndShowBadges()
	{
		if (base.SurvivorRightSidePanel != null)
		{
			base.SurvivorRightSidePanel.SetActiveButtons(value: false);
		}
	}
}
