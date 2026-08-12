using BaseModel;

namespace TWDModel
{
	public class AddTransactionToCurrentPurchaseCommand : ModelCommand
	{
		public IAPTransaction transaction { get; set; }

		public AddTransactionToCurrentPurchaseCommand()
		{
		}

		public AddTransactionToCurrentPurchaseCommand(IAPTransaction transaction)
		{
			this.transaction = transaction;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Player.CurrentIAP.Transaction = transaction;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
