public class SurvivorInfoStateUpgradeDone : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorUpgradeDone;
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(base.SurvivorStatistics, value: true);
		Helpers.GameObjectSetActive(base.SurvivorTraitsList.gameObject, value: true);
		UpdateUpgradePanel(base.TraitUpgradePanel);
	}

	public override void Enter()
	{
		base.Enter();
		if (!OfflineManager.IsLoadDataManager) SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestShowUpgradeAnim();
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Hide);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Hide);
	}

	protected virtual void UpdateUpgradePanel(SurvivorUpgradeView upgradeView)
	{
		if (Helpers.GameObjectSetActive(upgradeView, value: true))
		{
			upgradeView.UpdateWith(base.SurvivorModel, CurrentState, OnOkClicked);
		}
	}

	protected virtual void OnOkClicked(UIButtonExtended button)
	{
		if (button != null && TutorialView.Allowed("Ok"))
		{
			button.Clear();
			SetState(States.SurvivorOverview);
			EventManager.NotifyClick("Ok");
		}
	}
}
