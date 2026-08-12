using BaseModel;

namespace TWDModel
{
	public class UnlockCageWalkerCommand : ConsumeCurrencyCommand
	{
		public UnlockCageWalkerCommand()
		{
		}

		public UnlockCageWalkerCommand(OutpostWalkerModel outpostWalkerModel)
			: base(outpostWalkerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			OutpostWalkerModel outpostWalkerModel = (OutpostWalkerModel)manager.GetModel(base.ModelId);
			if (outpostWalkerModel.HasUnlockedWithMoney)
			{
				result = TWDModelResult.Error;
			}
			else
			{
				Cashier unlockCashier = outpostWalkerModel.GetUnlockCashier();
				if (unlockCashier.CanAfford() && unlockCashier.Pay() == TWDModelResult.OK)
				{
					outpostWalkerModel.Unlock();
				}
				else
				{
					result = TWDModelResult.Error;
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
