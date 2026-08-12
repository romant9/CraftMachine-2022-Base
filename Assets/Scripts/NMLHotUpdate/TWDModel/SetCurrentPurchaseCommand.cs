using BaseModel;

namespace TWDModel
{
	public class SetCurrentPurchaseCommand : ModelCommand
	{
		public StorePurchaseInfo purchaseInfo { get; set; }

		public SetCurrentPurchaseCommand()
		{
		}

		public SetCurrentPurchaseCommand(StorePurchaseInfo purchaseInfo)
		{
			this.purchaseInfo = purchaseInfo;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Player.CurrentIAP = purchaseInfo;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
