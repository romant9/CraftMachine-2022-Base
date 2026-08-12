using System;
using BaseModel;

namespace TWDModel
{
	public class RerollSurvivorTraitCommand : ConsumeCurrencyCommand
	{
		public string TraitToBeRerolled { get; set; }

		public RerollSurvivorTraitCommand()
		{
		}

		public RerollSurvivorTraitCommand(SurvivorModel survivor, string traitToBeRerolled)
			: base(survivor)
		{
			TraitToBeRerolled = traitToBeRerolled;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			SurvivorModel survivorModel = (SurvivorModel)manager.GetModel(base.ModelId);
			TraitDefinition traitDefinition = tWDModelManager.GameEconomyData.GetTraitDefinition(TraitToBeRerolled);
			if (survivorModel != null && survivorModel.CanRerollTrait && survivorModel.HasUpgradeTrait(TraitToBeRerolled) && traitDefinition != null && !traitDefinition.HasTag("FactionBuffTrait") && !traitDefinition.Identifier.Equals("Overwatch", StringComparison.Ordinal))
			{
				Cashier traitRerollCashier = survivorModel.GetTraitRerollCashier(TraitToBeRerolled);
				if (traitRerollCashier != null && traitRerollCashier.CanAfford())
				{
					tWDModelResult = traitRerollCashier.Pay(survivorModel);
					if (tWDModelResult == TWDModelResult.OK)
					{
						tWDModelResult = ((!survivorModel.RerollTrait(TraitToBeRerolled)) ? TWDModelResult.Error : TWDModelResult.OK);
					}
					if (tWDModelResult == TWDModelResult.OK)
					{
						tWDModelManager.Metrics.AddSpend().AddResources(traitRerollCashier).AddTraitReroll(survivorModel)
							.AddSurvivor(survivorModel)
							.AddLevel()
							.Send();
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
