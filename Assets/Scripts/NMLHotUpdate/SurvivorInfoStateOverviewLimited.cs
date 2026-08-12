public class SurvivorInfoStateOverviewLimited : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorOverviewLimited;
	}

	public override void Enter()
	{
		base.Enter();
		SurvivorInfoPopup.AllowWeapons = false;
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Show);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Show);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.SurvivorNamePanel != null)
		{
			base.SurvivorNamePanel.EnableNameInput(value: false);
		}
		UpdateAndShowSurvivorNavigationButtons();
	}

	protected override void UpdateAndShowBadges()
	{
		if (base.SurvivorRightSidePanel != null)
		{
			base.SurvivorRightSidePanel.SetActiveButtons(value: false);
		}
	}

	protected override void UpdateAndShowStats()
	{
		if (base.SurvivorStatistics != null)
		{
			base.SurvivorStatistics.SetInfo(base.SurvivorModel, SurvivorInfoPopup.AllowWeapons, showEquipmentLockedState: false);
			Helpers.GameObjectSetActive(base.SurvivorStatistics, value: true);
		}
		if (base.RarityAndClass != null)
		{
			base.RarityAndClass.UpdateWithSurvivor(base.SurvivorModel);
			Helpers.GameObjectSetActive(base.RarityAndClass, value: true);
		}
	}
}
