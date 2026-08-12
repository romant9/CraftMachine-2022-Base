using BaseModel;

namespace TWDModel
{
	public class SpeedUpUpgradeSurvivorCommand : ConsumeCurrencyCommand
	{
		public SpeedUpUpgradeSurvivorCommand()
		{
		}

		public SpeedUpUpgradeSurvivorCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel survivorModel = (SurvivorModel)manager.GetModel(base.ModelId);
			TWDModelResult result = survivorModel.TimedActionModel.SpeedUpSurvivorUpgradeAction(survivorModel, base.Cashier);
			return new NGModelCommandRespond(this, result);
		}
	}
}
