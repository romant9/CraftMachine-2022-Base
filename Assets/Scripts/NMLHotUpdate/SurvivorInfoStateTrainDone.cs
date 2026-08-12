public class SurvivorInfoStateTrainDone : SurvivorInfoStateUpgradeDone
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorTrainDone;
	}

	public override void UpdateUI()
	{
		if (!TutorialView.Allowed("Ok"))
		{
			base.LevelUpPanel.DisableOkButton();
		}
		else
		{
			base.LevelUpPanel.EnableOkButton();
		}
		Helpers.GameObjectSetActive(base.SurvivorStatistics, value: true);
		Helpers.GameObjectSetActive(base.SurvivorTraitsList.gameObject, value: true);
		UpdateUpgradePanel(base.LevelUpPanel);
	}

	protected override void OnOkClicked(UIButtonExtended button)
	{
		SurvivorInfoPopup.HandleSurvivorUpgradeViewed(base.SurvivorModel);
		base.OnOkClicked(button);
	}
}
