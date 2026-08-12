using BaseModel;

namespace TWDModel
{
	public class ChooseRerolledTraitCommand : ModelCommand
	{
		public int Selection = -1;

		public ChooseRerolledTraitCommand()
		{
		}

		public ChooseRerolledTraitCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public ChooseRerolledTraitCommand(SurvivorModel survivor, int selection)
			: base(survivor)
		{
			Selection = selection;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			SurvivorModel survivorModel = (SurvivorModel)manager.GetModel(base.ModelId);
			if ((Selection == -1 || Selection == 0 || Selection == 1) && survivorModel != null && !string.IsNullOrEmpty(survivorModel.TraitToBeRerolledCandidate) && survivorModel.RandomTraitsFromReroll != null && survivorModel.RandomTraitsFromReroll.Count == 2)
			{
				tWDModelManager.Metrics.AddTraitReroll(survivorModel);
				string text = ((Selection == -1) ? survivorModel.TraitToBeRerolledCandidate : survivorModel.RandomTraitsFromReroll[Selection]);
				tWDModelResult = ((!survivorModel.ChooseRerolledTrait(Selection)) ? TWDModelResult.Error : TWDModelResult.OK);
				if (tWDModelResult == TWDModelResult.OK)
				{
					tWDModelManager.Metrics.AddTraitRerollOutcome(text);
					if (Selection == -1)
					{
						Cashier cashier = survivorModel.RefundTokens(text);
						tWDModelManager.Metrics.AddResources(cashier.LastRefundAmounts).AddTraitRerollTokenRefund();
					}
					tWDModelManager.Metrics.AddSurvivor(survivorModel).AddLevel().Send();
				}
				else
				{
					tWDModelManager.Metrics.Reset();
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
