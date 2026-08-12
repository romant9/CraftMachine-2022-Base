using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using Epic.OnlineServices.Ecom;
using Newtonsoft.Json;
using TWDModel;
using UnityEngine;

public class EpicIAPImplementation
{
	public delegate void PurchaseFail(string id, PurchaseConfirmationResult purchaseConfirmationResult, string purchaseFailureReason);

	private const string DebugTag = "EpicIAP - ";

	public Action OnInitializationCompleted;

	public PurchaseFail OnPurchaseFail;

	public Action<StorePurchaseInfo> OnPurchaseSuccessful;

	public Action<PurchaseValidationResult, StorePurchaseInfo> OnPurchaseValidationFail;

	private bool initialized;

	private float timeLastPurchase;

	private int seqID = -2;

	private int retryCount = 3;

	public EpicIAPImplementation()
	{
		SignalRClient.Instance.OnCommandCompletedMessage += CommandCompleteMessageHandler;
	}

	~EpicIAPImplementation()
	{
		SignalRClient.Instance.OnCommandCompletedMessage -= CommandCompleteMessageHandler;
	}

	private void CommandCompleteMessageHandler(int responsecode, int sequenceid)
	{
		if (sequenceid != seqID)
		{
			return;
		}
		if (responsecode == 0)
		{
			Debug.LogWarning("StartIAP command completed on server. Starting IAP: " + GameManager.Instance.playerModel.CurrentIAP.Product.ProductIdentifier);
			StorePurchaseInfo currentPurchase = GameManager.Instance.playerModel.CurrentIAP;
			EOSStore.CheckOutOverlayByOfferId(currentPurchase.Product.ProductIdentifier, delegate(string offerId, string transactionId)
			{
				if (transactionId == null)
				{
					OnPurchaseFailed(offerId, "Unknown");
				}
				else
				{
					Dictionary<string, string> value = new Dictionary<string, string>
					{
						["orderId"] = transactionId.ToString(),
						["packageName"] = "",
						["productId"] = offerId,
						["purchaseTime"] = DateTime.Now.Ticks.ToString(),
						["purchaseState"] = "-1",
						["purchaseToken"] = ""
					};
					Helpers.ExecuteCommand(new AddTransactionToCurrentPurchaseCommand(new IAPTransaction
					{
						Base64EncodedTransactionReceipt = JsonConvert.SerializeObject(value),
						ProductIdentifier = offerId,
						Quantity = 1,
						TransactionIdentifier = transactionId,
						TrackingId = Guid.NewGuid().ToString(),
						CatalogItemId = EOSStore.GetCatalogItemIdByOfferId(offerId),
						EpicAccessToken = EOSLogin.GetAccessToken()
					}));
					Helpers.ExecuteCommand(new ReportPurchaseStatusCommand(offerId, null, GameManager.Instance.BundleSource, PurchaseConfirmationResult.OK, PurchaseValidationResult.None, ""));
					Debug.LogWarning("EpicIAP - Requesting Validation.");
					RequestValidation(currentPurchase);
				}
			});
		}
		seqID = -2;
	}

	public void OnPurchaseFailed(string productId, string reason)
	{
		Debug.LogWarning("EpicIAP - Purchase failed: \nProductID: " + productId + "Reason: " + reason);
		Helpers.ExecuteCommand(new ReportPurchaseStatusCommand(productId, null, GameManager.Instance.BundleSource, PurchaseConfirmationResult.Error, PurchaseValidationResult.None, reason));
		Notify_OnPurchaseFail(productId, PurchaseConfirmationResult.Error, reason);
	}

	private void RequestValidation(StorePurchaseInfo currentPurchase)
	{
		SignalRClient.Instance.RequestCommand("ValidateAndApplyReceipt", JsonConvert.SerializeObject(currentPurchase), OnValidateReceipt, waitForResponse: true);
	}

	private void OnValidateReceipt(string message)
	{
		Debug.LogWarning("EpicIAP - Server responded with receipt validation : " + message);
		ValidateReceiptResponse validateReceiptResponse = GameManager.Instance.jsonSerializer.Deserialize<ValidateReceiptResponse>(message);
		if (validateReceiptResponse == null)
		{
			Debug.LogError("EpicIAP - Can not parse validation result: " + message);
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
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
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
		if (retryCount == 0)
		{
			initialized = true;
			if (OnInitializationCompleted != null)
			{
				OnInitializationCompleted();
			}
			return;
		}
		retryCount--;
		EOSStore.QueryOffers(delegate(bool success)
		{
			if (success)
			{
				initialized = true;
				if (OnInitializationCompleted != null)
				{
					OnInitializationCompleted();
				}
			}
			else
			{
				Debug.LogWarning("EpicIAP - Initialization failed");
				initialized = false;
				PopulateProductList(products);
			}
		});
	}

	public bool IsInitialized()
	{
		return initialized;
	}

	public bool Buy(string productId, string bundleContentId)
	{
		if (Time.realtimeSinceStartup - timeLastPurchase < 1f)
		{
			return false;
		}
		timeLastPurchase = Time.realtimeSinceStartup;
		EOSStore.GetCatalogByOfferId(productId, out var outOffer);
		if (!outOffer.HasValue)
		{
			Debug.LogError("EpicIAP - Product not found. Canceling buy." + productId);
			Notify_OnPurchaseFail(productId, PurchaseConfirmationResult.Error, "Product not found");
			return false;
		}
		CatalogOffer value = outOffer.Value;
		if (GameManager.Instance.playerModel.CurrentIAP != null)
		{
			Debug.LogWarning("Overriding current purchase");
		}
		float num = (float)value.CurrentPrice64 / (float)Math.Pow(10.0, value.DecimalPoint);
		StartIAPPurchaseCommand startIAPPurchaseCommand = new StartIAPPurchaseCommand(new StorePurchaseInfo
		{
			Created = DateTime.UtcNow,
			Store = GetStorePlatform(),
			BundleId = bundleContentId,
			Product = new IAPProduct
			{
				ProductIdentifier = productId,
				Title = value.TitleText,
				Description = value.DescriptionText,
				Price = num.ToString(),
				CurrencyCode = value.CurrencyCode,
				FormattedPrice = value.CurrencyCode + " " + num.ToString()
			}
		}, GameManager.Instance.BundleSource);
		if (Helpers.ExecuteCommand(startIAPPurchaseCommand) == TWDModelResult.OK)
		{
			seqID = startIAPPurchaseCommand.SequenceId;
			return true;
		}
		return false;
	}

	private void Notify_OnPurchaseFail(string productId, PurchaseConfirmationResult confirmationResult, string purchaseFailureReason)
	{
		if (OnPurchaseFail != null)
		{
			OnPurchaseFail(productId, confirmationResult, purchaseFailureReason);
		}
	}

	public IAPStore GetStorePlatform()
	{
		return IAPStore.EpicStore;
	}

	public string GetFormattedPrice(string productId)
	{
		if (IsInitialized())
		{
			BundleContentDefinition bundleContentDefinitionWithIAPProduct = GameManager.Instance.gameEconomyData.GetBundleContentDefinitionWithIAPProduct(productId);
			productId = ((bundleContentDefinitionWithIAPProduct != null) ? bundleContentDefinitionWithIAPProduct.EpicOfferID : "");
			EOSStore.GetCatalogByOfferId(productId, out var outOffer);
			if (!outOffer.HasValue)
			{
				Debug.LogError("EpicIAP - Product not found : " + productId);
				return "Product not found";
			}
			CatalogOffer value = outOffer.Value;
			float num = (float)value.CurrentPrice64 / (float)Math.Pow(10.0, value.DecimalPoint);
			return value.CurrencyCode + " " + num.ToString();
		}
		return LocalizationManager.GetText("Generic.Connecting");
	}

	public void UnlinkAccount(Action successCallback = null, Action failureCallback = null)
	{
		SignalRClient.Instance.RequestCommand("UnlinkAccountAsync", EOSLogin.GetAccountUserId().ToString(), AccountType.WindowsEditor.ToString(), delegate(string message)
		{
			if (SignalRClient.Instance.HasError)
			{
				Debug.LogError("UnlinkAccountAsync failed: " + message);
				SignalRClient.Instance.ClearError();
				if (failureCallback != null)
				{
					failureCallback();
				}
			}
			else if (successCallback != null)
			{
				successCallback();
			}
		}, null, waitForResponse: true);
	}
}
