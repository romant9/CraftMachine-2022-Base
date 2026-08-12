using BaseModel;

namespace TWDModel
{
	public class SpeedUpUpgradeOutpostWalkerCommand : ConsumeCurrencyCommand
	{
		public SpeedUpUpgradeOutpostWalkerCommand()
		{
		}

		public SpeedUpUpgradeOutpostWalkerCommand(OutpostWalkerModel outpostWalkerModel)
			: base(outpostWalkerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			OutpostWalkerModel outpostWalkerModel = (OutpostWalkerModel)manager.GetModel(base.ModelId);
			TWDModelResult result = outpostWalkerModel.TimedActionModel.SpeedUpAction(outpostWalkerModel);
			return new NGModelCommandRespond(this, result);
		}
	}
}
