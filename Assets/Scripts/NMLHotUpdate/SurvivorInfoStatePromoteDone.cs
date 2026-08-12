using TWDModel;

public class SurvivorInfoStatePromoteDone : SurvivorInfoStateUpgradeDone
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorPromoteDone;
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(base.SurvivorStatistics, value: true);
		Helpers.GameObjectSetActive(base.SurvivorTraitsList.gameObject, value: true);
		if (base.SurvivorModel.SurvivorRarityLevel < SurvivorModel.MAX_UPGRADEABLE_TRAITS)
		{
			UpdateUpgradePanel(base.PromotedPanel);
		}
		else
		{
			UpdateUpgradePanel(base.PromotedPanelNoTrait);
		}
	}
}
