using BaseModel;

namespace TWDModel
{
	public class IAPPurchase
	{
		public bool Completed;

		public IAPPurchaseFailure Failed;

		public string FailReason = "";

		public IAPProduct Product;

		public IAPTransaction Transaction;

		public string CustomData = "";
	}
}
