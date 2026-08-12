using BaseModel;

namespace TWDModel
{
	public class UpgradeSurvivorCommand : ConsumeCurrencyCommand
	{
		public bool Instant { get; set; }

		public UpgradeSurvivorCommand()
		{
		}

		public UpgradeSurvivorCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel survivorModel = (SurvivorModel)manager.GetModel(base.ModelId);
			TWDModelResult tWDModelResult;
			if (Instant && base.Cashier.useTokensForPayment)
			{
				tWDModelResult = survivorModel.UpgradeInstant(base.Cashier);
			}
			else if (Instant)
			{
				tWDModelResult = survivorModel.UpgradeInstant();
			}
			else
			{
				tWDModelResult = survivorModel.StartUpgrade(base.UseDiamondsAmount);
				if (tWDModelResult == TWDModelResult.OK)
				{
					if ((manager.GetPlayer() as PlayerModel).Camp.GetBuilding("TrainingGround") is TrainingGroundBuildingModel trainingGroundBuildingModel)
					{
						trainingGroundBuildingModel.SetUpgradingModel(survivorModel);
					}
					else
					{
						((TWDModelManager)manager).Debug.LogError("Training ground not found!");
						tWDModelResult = TWDModelResult.Error;
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
