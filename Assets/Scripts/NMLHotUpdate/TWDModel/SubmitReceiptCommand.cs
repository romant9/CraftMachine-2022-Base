using System;
using BaseModel;

namespace TWDModel
{
	public class SubmitReceiptCommand : ModelCommand
	{
		public string BundleId;

		public Metrics.BundleSource BundleSource;

		public IAPStore Store;

		public IAPProduct Product;

		public IAPTransaction Transaction;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			if (playerModel.GetPendingPurchase(Transaction.TransactionIdentifier) != null)
			{
				manager.Debug.Log("Duplicate receipt received, sequence id = " + base.SequenceId);
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			PendingPurchaseInfo pendingPurchaseInfo = new PendingPurchaseInfo();
			pendingPurchaseInfo.Created = DateTime.UtcNow;
			pendingPurchaseInfo.BundleId = BundleId;
			pendingPurchaseInfo.BundleSource = BundleSource;
			pendingPurchaseInfo.Store = Store;
			pendingPurchaseInfo.Transaction = Transaction;
			VerifyBundleId(pendingPurchaseInfo, manager as TWDModelManager);
			FillProduct(pendingPurchaseInfo, Product, manager as TWDModelManager);
			playerModel.PendingIAPs.Add(pendingPurchaseInfo);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		private static string GetProductIdForBundle(TWDModelManager manager, string bundleId)
		{
			if (bundleId != null)
			{
				BundleContentDefinition bundleContentDefinition = manager.GameEconomyData.GetBundleContentDefinition(bundleId);
				if (bundleContentDefinition != null)
				{
					return bundleContentDefinition.IAPProduct;
				}
			}
			return null;
		}

		public static void VerifyBundleId(PendingPurchaseInfo purchase, TWDModelManager manager)
		{
			string productIdForBundle = GetProductIdForBundle(manager, purchase.BundleId);
			if (productIdForBundle == null || productIdForBundle != purchase.Transaction.ProductIdentifier)
			{
				BundleStoreDefinition bundleStoreDefinitionFromProductID = manager.GameEconomyData.GetBundleStoreDefinitionFromProductID(purchase.Transaction.ProductIdentifier, manager.Player.UtcTimeStamp);
				if (bundleStoreDefinitionFromProductID == null)
				{
					manager.Debug.LogError("VerifyBundleId: could not recover purchase " + purchase.Transaction.ProductIdentifier);
					return;
				}
				purchase.BundleId = bundleStoreDefinitionFromProductID.BundleIdentifier;
				manager.Debug.LogWarning("VerifyBundleId: recovered " + purchase.Transaction.ProductIdentifier + " to bundle " + bundleStoreDefinitionFromProductID.BundleIdentifier);
			}
		}

		public static void FillProduct(PendingPurchaseInfo purchase, IAPProduct product, TWDModelManager manager)
		{
			InAppPurchaseProductApple inAppPurchaseProduct = manager.GameEconomyData.GetInAppPurchaseProduct(purchase.Transaction.ProductIdentifier);
			if (inAppPurchaseProduct == null)
			{
				manager.Debug.LogError("FillProduct: unknown product " + purchase.Transaction.ProductIdentifier);
				return;
			}
			if (product != null)
			{
				purchase.Product = new IAPProduct
				{
					CurrencyCode = product.CurrencyCode,
					CurrencySymbol = product.CurrencySymbol,
					Description = product.Description,
					Price = product.Price,
					FormattedPrice = product.FormattedPrice,
					Title = product.Title,
					ProductIdentifier = product.ProductIdentifier
				};
			}
			else
			{
				purchase.Product = new IAPProduct
				{
					CurrencyCode = "USD",
					CurrencySymbol = "$",
					Description = "",
					Price = inAppPurchaseProduct.PriceUSD.ToString(),
					FormattedPrice = "$ " + inAppPurchaseProduct.PriceUSD,
					Title = inAppPurchaseProduct.Id,
					ProductIdentifier = inAppPurchaseProduct.Id
				};
				manager.Debug.LogWarning("FillProduct: recovered " + purchase.Transaction.ProductIdentifier);
			}
			purchase.Product.PriceTier = inAppPurchaseProduct.PriceTier;
			purchase.Product.PriceUSD = inAppPurchaseProduct.PriceUSD;
		}
	}
}
