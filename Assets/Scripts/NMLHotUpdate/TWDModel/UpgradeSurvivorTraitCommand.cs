using BaseModel;

namespace TWDModel
{
	public class UpgradeSurvivorTraitCommand : ConsumeCurrencyCommand
	{
		public UpgradeSurvivorTraitCommand()
		{
		}

		public UpgradeSurvivorTraitCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			SurvivorModel survivorModel = (SurvivorModel)manager.GetModel(base.ModelId);
			if (survivorModel != null)
			{
				Cashier upgradeTraitCashier = survivorModel.GetUpgradeTraitCashier();
				if (upgradeTraitCashier != null)
				{
					tWDModelResult = upgradeTraitCashier.Pay(survivorModel);
					if (tWDModelResult == TWDModelResult.OK)
					{
						TdMetrics tdMetrics = (manager as TWDModelManager).TdMetrics.SetEventType("upgrade_hero_trait");
						if (survivorModel.CanUpgradeSurvivorRarity())
						{
							tdMetrics.AddProperty("is_upgrade_rarity", true);
							tWDModelResult = ((!survivorModel.UpgradeSurvivorRarity(doNotInstantiateTrait: false, tdMetrics)) ? TWDModelResult.Error : TWDModelResult.OK);
						}
						else
						{
							tdMetrics.AddProperty("is_upgrade_rarity", false);
							tWDModelResult = ((!survivorModel.UpgradeLowestLevelTrait(doNotInstantiateTrait: false, tdMetrics)) ? TWDModelResult.Error : TWDModelResult.OK);
						}
						if (tWDModelResult == TWDModelResult.OK && survivorModel.IsHero)
						{
							tdMetrics.AddProperty("hero_id", survivorModel.ActorDefinitionID).Send();
						}
						else
						{
							tdMetrics.Reset();
						}
						CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivorModel);
						survivorModel.TokensSpent += upgradeTraitCashier.GetTotalCost(survivorTraitUpgradeCurrencyType);
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
