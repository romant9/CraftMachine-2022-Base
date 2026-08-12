using System;

namespace BaseModel
{
	public class StorePurchaseInfo
	{
		public DateTime Created;

		public string BundleId;

		public IAPStore Store;

		public IAPProduct Product;

		public IAPTransaction Transaction;

		public IosMarketType IosMarketType;
	}
}
