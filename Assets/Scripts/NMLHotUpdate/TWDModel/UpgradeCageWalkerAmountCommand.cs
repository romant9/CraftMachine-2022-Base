using BaseModel;

namespace TWDModel
{
	public class UpgradeCageWalkerAmountCommand : ConsumeCurrencyCommand
	{
		public UpgradeCageWalkerAmountCommand()
		{
		}

		public UpgradeCageWalkerAmountCommand(OutpostWalkerModel outpostWalkerModel)
			: base(outpostWalkerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			tWDModelResult = ((OutpostWalkerModel)manager.GetModel(base.ModelId)).UpgradeAmount(base.UseDiamondsAmount);
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
