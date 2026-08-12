using BaseModel;

namespace TWDModel
{
	public class RemoveReceiptCommand : ModelCommand
	{
		public string TransactionId;

		public RemoveReceiptCommand()
		{
		}

		public RemoveReceiptCommand(string transactionId)
		{
			TransactionId = transactionId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.Player.GetPendingPurchase(TransactionId) == null)
			{
				manager.Debug.LogError("Purchase not found");
			}
			else if (tWDModelManager.Player.RemovePendingPurchase(TransactionId))
			{
				result = TWDModelResult.OK;
			}
			else
			{
				manager.Debug.LogError("Failed to remove purchase");
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
