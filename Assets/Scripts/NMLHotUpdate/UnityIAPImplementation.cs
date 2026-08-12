using System;
using System.Globalization;
using BaseModel;
using Client.Connectivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TWDModel;
using UnityEngine;
using UnityEngine.Purchasing;

public class UnityIAPImplementation : IStoreListener
{
	public delegate void PurchaseFail(string id, PurchaseConfirmationResult purchaseConfirmationResult, string purchaseFailureReason);

	private const string DebugTag = "UnityIAP - ";

	public Action OnInitializationCompleted;

	public PurchaseFail OnPurchaseFail;

	public Action<StorePurchaseInfo> OnPurchaseSuccessful;

	public Action<PurchaseValidationResult, StorePurchaseInfo> OnPurchaseValidationFail;

	private IStoreController unityController;

	private float timeLastPurchase;

	private int seqID = -2;

	public UnityIAPImplementation()
	{
		SignalRClient.Instance.OnCommandCompletedMessage += CommandCompleteMessageHandler;
	}

	~UnityIAPImplementation()
	{
		SignalRClient.Instance.OnCommandCompletedMessage -= CommandCompleteMessageHandler;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.LogWarning("UnityIAP - Initialization failed " + error);
	}

	public void OnInitializeFailed(InitializationFailureReason error, string message)
	{
		Debug.LogWarning("UnityIAP - Initialization failed " + error.ToString() + " " + message);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		string id = e.purchasedProduct.definition.id;
		Debug.LogWarning("UnityIAP - Received request to process purchase: " + id);
		StorePurchaseInfo currentIAP = GameManager.Instance.playerModel.CurrentIAP;
		if (currentIAP == null)
		{
			Debug.LogError("UnityIAP - Received request to process a purchase when the current IAP is null.\n ProductId: " + id);
			Helpers.ExecuteCommand(new AddUnhandledPurchaseCommand(CreateUnhandledPurchase(e)));
			return PurchaseProcessingResult.Complete;
		}
		if (currentIAP.Product.ProductIdentifier != id)
		{
			Debug.LogError("UnityIAP - Received request to process a purchase different than the current one. \n CurrentPurchaseID: " + currentIAP.Product.ProductIdentifier + "\n ProductID received: " + id);
			Helpers.ExecuteCommand(new AddUnhandledPurchaseCommand(CreateUnhandledPurchase(e)));
			return PurchaseProcessingResult.Complete;
		}
		Helpers.ExecuteCommand(new AddTransactionToCurrentPurchaseCommand(new IAPTransaction
		{
			Base64EncodedTransactionReceipt = GetReceiptInCorrectFormat(e.purchasedProduct.receipt),
			ProductIdentifier = id,
			Quantity = 1,
			TransactionIdentifier = GetTransactionIDInCorrectFormat(e.purchasedProduct),
			TrackingId = Guid.NewGuid().ToString()
		}));
		Helpers.ExecuteCommand(new ReportPurchaseStatusCommand(id, null, GameManager.Instance.BundleSource, PurchaseConfirmationResult.OK, PurchaseValidationResult.None, ""));
		Debug.LogWarning("UnityIAP - Requesting Validation.");
		RequestValidation(currentIAP);
		return PurchaseProcessingResult.Pending;
	}

	private StorePurchaseInfo CreateUnhandledPurchase(PurchaseEventArgs e)
	{
		return new StorePurchaseInfo
		{
			Created = DateTime.UtcNow,
			Store = GetStore(e.purchasedProduct.definition.type),
			BundleId = "",
			Product = new IAPProduct
			{
				ProductIdentifier = e.purchasedProduct.definition.id,
				Title = e.purchasedProduct.metadata.localizedTitle,
				Description = e.purchasedProduct.metadata.localizedDescription,
				Price = e.purchasedProduct.metadata.localizedPrice.ToString(CultureInfo.InvariantCulture),
				CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
				FormattedPrice = e.purchasedProduct.metadata.localizedPriceString
			},
			Transaction = new IAPTransaction
			{
				Base64EncodedTransactionReceipt = GetReceiptInCorrectFormat(e.purchasedProduct.receipt),
				ProductIdentifier = e.purchasedProduct.definition.id,
				Quantity = 1,
				TransactionIdentifier = GetTransactionIDInCorrectFormat(e.purchasedProduct),
				TrackingId = Guid.NewGuid().ToString()
			},
			IosMarketType = (GameConfiguration.Instance.Config.LowViolence ? IosMarketType.korea : IosMarketType.global)
		};
	}

	private string GetReceiptInCorrectFormat(string receipt)
	{
		string text = (string?)JObject.Parse(receipt)["Payload"];
		if (GetStorePlatform() == IAPStore.GooglePlayStore)
		{
			text = (string?)JObject.Parse(text)["json"];
		}
		return text;
	}

	private string GetTransactionIDInCorrectFormat(Product purchasedProduct)
	{
		string result = purchasedProduct.transactionID;
		if (GetStorePlatform() == IAPStore.GooglePlayStore)
		{
			result = (string?)JObject.Parse(purchasedProduct.receipt)["Payload"];
			result = (string?)JObject.Parse(result)["signature"];
		}
		return result;
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.LogWarning("UnityIAP - Purchase failed: \nProductID: " + i.definition.id + "Reason: " + p);
		Helpers.ExecuteCommand(new ReportPurchaseStatusCommand(i.definition.id, null, GameManager.Instance.BundleSource, ResolveConfirmationResult(p), PurchaseValidationResult.None, p.ToString()));
		Notify_OnPurchaseFail(i.definition.id, ResolveConfirmationResult(p), p.ToString());
		ConfirmPendingPurchase();
	}

	private void Notify_OnPurchaseFail(string productId, PurchaseConfirmationResult confirmationResult, string purchaseFailureReason)
	{
		if (OnPurchaseFail != null)
		{
			OnPurchaseFail(productId, confirmationResult, purchaseFailureReason);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		unityController = controller;
		if (OnInitializationCompleted != null)
		{
			OnInitializationCompleted();
		}
	}

	private void RequestValidation(StorePurchaseInfo currentPurchase)
	{
		SignalRClient.Instance.RequestCommand("ValidateAndApplyReceipt", JsonConvert.SerializeObject(currentPurchase), OnValidateReceipt, waitForResponse: true);
	}

	public bool Buy(string productId, string bundleContentId)
	{
		if (Time.realtimeSinceStartup - timeLastPurchase < 1f)
		{
			return false;
		}
		timeLastPurchase = Time.realtimeSinceStartup;
		Product product = unityController.products.WithID(productId);
		if (product == null)
		{
			Debug.LogError("UnityIAP - Product not found. Canceling buy." + productId);
			Notify_OnPurchaseFail(productId, PurchaseConfirmationResult.Error, "Product not found");
			return false;
		}
		if (GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(PlatformIdConverter.ToMainProductId(productId)) == null)
		{
			Debug.LogError("UnityIAP - FillProduct: unknown product " + productId);
			return false;
		}
		if (GameManager.Instance.playerModel.CurrentIAP != null)
		{
			Debug.LogWarning("Overriding current purchase");
		}
		StartIAPPurchaseCommand startIAPPurchaseCommand = new StartIAPPurchaseCommand(new StorePurchaseInfo
		{
			Created = DateTime.UtcNow,
			Store = GetStore(product.definition.type),
			BundleId = bundleContentId,
			Product = new IAPProduct
			{
				ProductIdentifier = productId,
				Title = product.metadata.localizedTitle,
				Description = product.metadata.localizedDescription,
				Price = product.metadata.localizedPrice.ToString(),
				CurrencyCode = product.metadata.isoCurrencyCode,
				FormattedPrice = product.metadata.localizedPriceString
			},
			IosMarketType = (GameConfiguration.Instance.Config.LowViolence ? IosMarketType.korea : IosMarketType.global)
		}, GameManager.Instance.BundleSource);
		if (Helpers.ExecuteCommand(startIAPPurchaseCommand) == TWDModelResult.OK)
		{
			seqID = startIAPPurchaseCommand.SequenceId;
			return true;
		}
		return false;
	}

	private void CommandCompleteMessageHandler(int responsecode, int sequenceid)
	{
		if (IsInitialized() && sequenceid == seqID)
		{
			if (responsecode == 0)
			{
				Debug.LogWarning("StartIAP command completed on server. Starting IAP: " + GameManager.Instance.playerModel.CurrentIAP.Product.ProductIdentifier + "====" + GameManager.Instance.playerModel.HashedId + "#" + GameManager.Instance.playerModel.CurrentIAP.BundleId);
				unityController.InitiatePurchase(GameManager.Instance.playerModel.CurrentIAP.Product.ProductIdentifier, GameManager.Instance.playerModel.HashedId + "#" + GameManager.Instance.playerModel.CurrentIAP.BundleId);
			}
			seqID = -2;
		}
	}

	public string GetFormattedPrice(string productId)
	{
		if (IsInitialized())
		{
			return unityController.products.WithID(productId).metadata.localizedPriceString;
		}
		Debug.LogError("UnityIAP - IAPs not yet initialized.");
		return "";
	}

	private void OnValidateReceipt(string message)
	{
		Debug.LogWarning("UnityIAP - Server responded with receipt validation : " + message);
		ValidateReceiptResponse validateReceiptResponse = GameManager.Instance.jsonSerializer.Deserialize<ValidateReceiptResponse>(message);
		if (validateReceiptResponse == null)
		{
			Debug.LogError("UnityIAP - Can not parse validation result: " + message);
			return;
		}
		if (unityController == null)
		{
			Debug.LogError("UnityIAP - Received server response when IAPS are not initialized.");
			return;
		}
		if (GameManager.Instance.playerModel.CurrentIAP == null)
		{
			Debug.LogError("UnityIAP - Received receipt validation when the current purchase is null");
			NotifyOnPurchaseValidationFail(PurchaseValidationResult.ClientValidationNullPurchase, GameManager.Instance.playerModel.CurrentIAP);
			return;
		}
		switch (validateReceiptResponse.NextAction)
		{
		case ValidateReceiptNextAction.Stop:
			if (validateReceiptResponse.State == InAppPurchaseState.Applied)
			{
				NotifyOnPurchaseValidationFail(PurchaseValidationResult.ClientValidationDuplicate, GameManager.Instance.playerModel.CurrentIAP);
			}
			else
			{
				NotifyOnPurchaseValidationFail(PurchaseValidationResult.ClientValidationFailed, GameManager.Instance.playerModel.CurrentIAP);
			}
			ConfirmPendingPurchase();
			break;
		case ValidateReceiptNextAction.Retry:
		case ValidateReceiptNextAction.ReloadAndRetry:
			RequestValidation(GameManager.Instance.playerModel.CurrentIAP);
			break;
		case ValidateReceiptNextAction.Proceed:
		{
			ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
			int shopTabIndex = -1;
			int shopPosition = -1;
			if (shopPopup != null)
			{
				shopTabIndex = shopPopup.GetCurrentTabIndex();
				shopPosition = shopPopup.IndexOfLastItemClicked;
			}
			if (Helpers.ExecuteCommand(new BuyCurrentIAPCommand(GameManager.Instance.BundleSource, shopTabIndex, shopPosition)) == TWDModelResult.OK)
			{
				OnPurchaseSuccessful?.Invoke(GameManager.Instance.playerModel.CurrentIAP);
			}
			else
			{
				NotifyOnPurchaseValidationFail(PurchaseValidationResult.ClientCommandFailed, GameManager.Instance.playerModel.CurrentIAP);
			}
			ConfirmPendingPurchase();
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void ConfirmPendingPurchase()
	{
		Product product = unityController.products.WithID(GameManager.Instance.playerModel.CurrentIAP.Product.ProductIdentifier);
		unityController.ConfirmPendingPurchase(product);
	}

	private void NotifyOnPurchaseValidationFail(PurchaseValidationResult result, StorePurchaseInfo currentPurchase)
	{
		if (OnPurchaseValidationFail != null)
		{
			OnPurchaseValidationFail(result, currentPurchase);
		}
	}

	public void PopulateProductList(string[] products)
	{
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		foreach (string text in products)
		{
			string id = ConvertToGooglePlayProductId(text);
			if (text.Contains("SUBSCRIPTION"))
			{
				configurationBuilder.AddProduct(text, ProductType.Subscription, new IDs
				{
					{ text, "MacAppStore" },
					{ id, "GooglePlay" }
				});
			}
			else
			{
				configurationBuilder.AddProduct(text, ProductType.Consumable, new IDs
				{
					{ text, "MacAppStore" },
					{ id, "GooglePlay" }
				});
			}
		}
		UnityPurchasing.Initialize(this, configurationBuilder);
	}

	public IAPStore GetStorePlatform()
	{
		return IAPStore.AppleAppStore;
	}

	public IAPStore GetStore(ProductType productType = ProductType.Consumable)
	{
		if (productType == ProductType.Subscription)
		{
			return IAPStore.AppleAppStoreSubScription;
		}
		return IAPStore.AppleAppStore;
	}

	public bool IsInitialized()
	{
		return unityController != null;
	}

	public Product[] GetAllProducts()
	{
		return unityController.products.all;
	}

	private static PurchaseConfirmationResult ResolveConfirmationResult(PurchaseFailureReason purchaseFailureReason)
	{
		if (purchaseFailureReason == PurchaseFailureReason.UserCancelled)
		{
			return PurchaseConfirmationResult.Canceled;
		}
		return PurchaseConfirmationResult.Error;
	}

	private static string ConvertToGooglePlayProductId(string iosId)
	{
		return iosId.Replace("TWD_NML_", "").Replace("TWD_BUNDLE_", "").Replace("TWD_OFFER_", "")
			.ToLower();
	}
}
