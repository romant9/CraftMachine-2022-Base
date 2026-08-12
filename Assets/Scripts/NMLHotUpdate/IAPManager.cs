using BaseModel;
using System;
using System.Linq;
using TWDModel;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
	private EpicIAPImplementation implementation;
	private UnityIAPImplementation implementationGoogle;

	private bool IsConnectedToTheStore
	{
		get
		{
			if (OfflineManager.Instance.ConnectSourceCurrent == OfflineManager.ConnectSource.Epic) return true;

			if (GameManager.Instance != null && GameManager.Instance.GameCenterManager != null)
			{
				return !string.IsNullOrEmpty(GameManager.Instance.GameCenterManager.GetId());
			}
			return false;
		}
	}

	private void Awake()
	{
		if (OfflineManager.IsGoogleSource)
		{
			implementationGoogle = new UnityIAPImplementation();
			UnityIAPImplementation unityIAPImplementation = implementationGoogle;
			unityIAPImplementation.OnInitializationCompleted = (Action)Delegate.Combine(unityIAPImplementation.OnInitializationCompleted, new Action(OnInitializationCompleted));
			UnityIAPImplementation unityIAPImplementation2 = implementationGoogle;
			unityIAPImplementation2.OnPurchaseFail = (UnityIAPImplementation.PurchaseFail)Delegate.Combine(unityIAPImplementation2.OnPurchaseFail, new UnityIAPImplementation.PurchaseFail(OnPurchaseFailed));
			UnityIAPImplementation unityIAPImplementation3 = implementationGoogle;
			unityIAPImplementation3.OnPurchaseSuccessful = (Action<StorePurchaseInfo>)Delegate.Combine(unityIAPImplementation3.OnPurchaseSuccessful, new Action<StorePurchaseInfo>(OnPurchaseSuccessful));
			UnityIAPImplementation unityIAPImplementation4 = implementationGoogle;
			unityIAPImplementation4.OnPurchaseValidationFail = (Action<PurchaseValidationResult, StorePurchaseInfo>)Delegate.Combine(unityIAPImplementation4.OnPurchaseValidationFail, new Action<PurchaseValidationResult, StorePurchaseInfo>(OnPurchaseValidationFailed));
		}
		else
		{
			implementation = new EpicIAPImplementation();
			EpicIAPImplementation epicIAPImplementation = implementation;
			epicIAPImplementation.OnInitializationCompleted = (Action)Delegate.Combine(epicIAPImplementation.OnInitializationCompleted, new Action(OnInitializationCompleted));
			EpicIAPImplementation epicIAPImplementation2 = implementation;
			epicIAPImplementation2.OnPurchaseFail = (EpicIAPImplementation.PurchaseFail)Delegate.Combine(epicIAPImplementation2.OnPurchaseFail, new EpicIAPImplementation.PurchaseFail(OnPurchaseFailed));
			EpicIAPImplementation epicIAPImplementation3 = implementation;
			epicIAPImplementation3.OnPurchaseSuccessful = (Action<StorePurchaseInfo>)Delegate.Combine(epicIAPImplementation3.OnPurchaseSuccessful, new Action<StorePurchaseInfo>(OnPurchaseSuccessful));
			EpicIAPImplementation epicIAPImplementation4 = implementation;
			epicIAPImplementation4.OnPurchaseValidationFail = (Action<PurchaseValidationResult, StorePurchaseInfo>)Delegate.Combine(epicIAPImplementation4.OnPurchaseValidationFail, new Action<PurchaseValidationResult, StorePurchaseInfo>(OnPurchaseValidationFailed));
		}
	}

	private void OnDestroy()
	{
		if (OfflineManager.IsGoogleSource)
		{
			if (implementation == null) return;
			EpicIAPImplementation epicIAPImplementation = implementation;
			epicIAPImplementation.OnInitializationCompleted = (Action)Delegate.Remove(epicIAPImplementation.OnInitializationCompleted, new Action(OnInitializationCompleted));
			EpicIAPImplementation epicIAPImplementation2 = implementation;
			epicIAPImplementation2.OnPurchaseFail = (EpicIAPImplementation.PurchaseFail)Delegate.Remove(epicIAPImplementation2.OnPurchaseFail, new EpicIAPImplementation.PurchaseFail(OnPurchaseFailed));
			EpicIAPImplementation epicIAPImplementation3 = implementation;
			epicIAPImplementation3.OnPurchaseSuccessful = (Action<StorePurchaseInfo>)Delegate.Remove(epicIAPImplementation3.OnPurchaseSuccessful, new Action<StorePurchaseInfo>(OnPurchaseSuccessful));
			EpicIAPImplementation epicIAPImplementation4 = implementation;
			epicIAPImplementation4.OnPurchaseValidationFail = (Action<PurchaseValidationResult, StorePurchaseInfo>)Delegate.Remove(epicIAPImplementation4.OnPurchaseValidationFail, new Action<PurchaseValidationResult, StorePurchaseInfo>(OnPurchaseValidationFailed));
		}
		else
		{
			if (implementationGoogle == null) return;
			UnityIAPImplementation unityIAPImplementation = implementationGoogle;
			unityIAPImplementation.OnInitializationCompleted = (Action)Delegate.Remove(unityIAPImplementation.OnInitializationCompleted, new Action(OnInitializationCompleted));
			UnityIAPImplementation unityIAPImplementation2 = implementationGoogle;
			unityIAPImplementation2.OnPurchaseFail = (UnityIAPImplementation.PurchaseFail)Delegate.Remove(unityIAPImplementation2.OnPurchaseFail, new UnityIAPImplementation.PurchaseFail(OnPurchaseFailed));
			UnityIAPImplementation unityIAPImplementation3 = implementationGoogle;
			unityIAPImplementation3.OnPurchaseSuccessful = (Action<StorePurchaseInfo>)Delegate.Remove(unityIAPImplementation3.OnPurchaseSuccessful, new Action<StorePurchaseInfo>(OnPurchaseSuccessful));
			UnityIAPImplementation unityIAPImplementation4 = implementationGoogle;
			unityIAPImplementation4.OnPurchaseValidationFail = (Action<PurchaseValidationResult, StorePurchaseInfo>)Delegate.Remove(unityIAPImplementation4.OnPurchaseValidationFail, new Action<PurchaseValidationResult, StorePurchaseInfo>(OnPurchaseValidationFailed));
		}
	}

	public void PopulateProductList(string[] products)
	{
        if (OfflineManager.IsGoogleSource)
		{
            if (implementationGoogle != null)
            {
                string[] array = new string[products.Length];
                for (int i = 0; i < products.Length; i++)
                {
                    array[i] = PlatformIdConverter.ToPlatformProductId(products[i]);
                }
                implementationGoogle.PopulateProductList(array);
            }
        }
		else
		{
            if (implementation != null)
            {
                string[] array = new string[products.Length];
                for (int i = 0; i < products.Length; i++)
                {
                    array[i] = PlatformIdConverter.ToPlatformProductId(products[i]);
                }
                implementation.PopulateProductList(array);
            }
        }			
	}

	public bool IsInitialized()
	{
        return OfflineManager.IsGoogleSource ? implementationGoogle.IsInitialized() : implementation.IsInitialized();
	}

	public string GetFormattedPrice(string productId)
	{
		if (OfflineManager.IsGoogleSource)
		{
			if (implementationGoogle != null)
			{
				if (implementationGoogle.IsInitialized())
				{
					string productId2 = PlatformIdConverter.ToPlatformProductId(productId);
					return implementationGoogle.GetFormattedPrice(productId2);
				}
				else
				{
					return LocalizationManager.GetText("Popup.GameCenter.GooglePlayLoginTitle");
				}
			}
		}
		else
		{
			if (implementation != null && implementation.IsInitialized())
			{
				return implementation.GetFormattedPrice(productId);
			}
		}
		return LocalizationManager.GetText("Generic.Free");
	}

	public void Buy(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition)
	{
		if (bundleContentDefinition == null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
			Debug.LogWarning("Purchase Cannot Continue : bundleContentDefinition is NULL");
		}
		else if (string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
		{
			GiveProductToPlayer(bundleStoreDefinition, bundleContentDefinition);
		}
		else
		{
            if (OfflineManager.IsGoogleSource)
			{
                if (!IsConnectedToTheStore && implementationGoogle != null && implementationGoogle.GetStorePlatform() == IAPStore.GooglePlayStore)
                {
                    Debug.LogWarning("Purchase Cannot Continue : user not logged on store");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                    if (GameManager.Instance.GameCenterManager != null)
                    {
                        StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(connect: true, delegate
                        {
                        }));
                    }
                }
                else if (implementationGoogle == null || !implementationGoogle.IsInitialized())
                {
                    Debug.LogWarning("Purchase Cannot Continue : Not ready to make purchases");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                }
                else
                {
					string epicOfferID = PlatformIdConverter.ToPlatformProductId(bundleContentDefinition.IAPProduct);
                    if (implementationGoogle.Buy(epicOfferID, bundleContentDefinition.Identifier))
                    {
                        SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Open();
                        SingularityMonoBehaviour<SDKManager>.Instance.Recharge(GameManager.Instance.playerModel.CurrentIAP);
                    }
                }
            }
			else
			{
                if (!IsConnectedToTheStore && implementation != null && implementation.GetStorePlatform() == IAPStore.GooglePlayStore)
                {
                    Debug.LogWarning("Purchase Cannot Continue : user not logged on store");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                    if (GameManager.Instance.GameCenterManager != null)
                    {
                        StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(connect: true, delegate
                        {
                        }));
                    }
                }
                else if (implementation == null || !implementation.IsInitialized())
                {
                    Debug.LogWarning("Purchase Cannot Continue : Not ready to make purchases");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                }
                else
                {
                    string epicOfferID = bundleContentDefinition.EpicOfferID;
                    if (implementation.Buy(epicOfferID, bundleContentDefinition.Identifier))
                    {
                        SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Open();
                        SingularityMonoBehaviour<SDKManager>.Instance.Recharge(GameManager.Instance.playerModel.CurrentIAP);
                    }
                }
            }
        }		
	}

	public void BuyCustomBundle(CustomBundleDefinition customBundleDefinition)
	{
		if (customBundleDefinition == null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
			Debug.LogWarning("Purchase Cannot Continue : bundleContentDefinition is NULL");
		}
		else
		{
            if (string.IsNullOrEmpty(customBundleDefinition.IAPProduct))
            {
                return;
            }
            if (OfflineManager.IsGoogleSource)
			{
                if (!IsConnectedToTheStore && implementationGoogle != null && implementationGoogle.GetStorePlatform() == IAPStore.GooglePlayStore)
                {
                    Debug.LogWarning("Purchase Cannot Continue : user not logged on store");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                    if (GameManager.Instance.GameCenterManager != null)
                    {
                        StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(connect: true, delegate
                        {
                        }));
                    }
                }
                else if (implementationGoogle == null || !implementationGoogle.IsInitialized())
                {
                    Debug.LogWarning("Purchase Cannot Continue : Not ready to make purchases");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                }
                else
                {
                    string epicOfferID = PlatformIdConverter.ToPlatformProductId(customBundleDefinition.IAPProduct);
                    if (implementationGoogle.Buy(epicOfferID, customBundleDefinition.Identifier))
                    {
                        SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Open();
                    }
                }
            }
			else
			{
                if (!IsConnectedToTheStore && implementation != null && implementation.GetStorePlatform() == IAPStore.GooglePlayStore)
                {
                    Debug.LogWarning("Purchase Cannot Continue : user not logged on store");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                    if (GameManager.Instance.GameCenterManager != null)
                    {
                        StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(connect: true, delegate
                        {
                        }));
                    }
                }
                else if (implementation == null || !implementation.IsInitialized())
                {
                    Debug.LogWarning("Purchase Cannot Continue : Not ready to make purchases");
                    SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
                }
                else
                {
                    string epicOfferID = customBundleDefinition.EpicOfferID;
                    if (implementation.Buy(epicOfferID, customBundleDefinition.Identifier))
                    {
                        SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Open();
                    }
                }
            }
            
		}
	}

	private void GiveProductToPlayer(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
		if (Helpers.ExecuteCommand(new UnlockFreeBundleCommand(bundleStoreDefinition, GameManager.Instance.playerModel, GameManager.Instance.BundleSource)) == TWDModelResult.OK)
		{
			UIEvent.Send("OnBundleBought", bundleStoreDefinition);
			IAPConfirmPopupNew.OpenWithBundleContent(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			}
		}
	}

	private void OnPurchaseSuccessful(StorePurchaseInfo currentPurchase)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
		BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(currentPurchase.BundleId);
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(currentPurchase.BundleId);
		if (bundleContentDefinition != null)
		{
			IAPConfirmPopupNew.OpenWithBundleContent(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false);
		}
		UIEvent.Send("OnBundleBought", bundleStoreDefinition);
		CustomBundleDefinition customBundleDefinition = GameManager.Instance.gameEconomyData.GetCustomBundleDefinition(currentPurchase.BundleId);
		if (customBundleDefinition != null)
		{
			IAPConfirmPopupNew.OpenCustomBundleContent(customBundleDefinition);
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
		}
		SingularityMonoBehaviour<SDKManager>.Instance.ExternalAnalytics.OnPurchase(currentPurchase.Product.PriceUSD, currentPurchase.Product.ProductIdentifier, currentPurchase.Transaction.TransactionIdentifier, GameManager.Instance.playerModel.GetTotalPurchases());
		if (!OfflineManager.IsLoadDataManager) SingularityMonoBehaviour<SDKManager>.Instance.ReportPurchaseEvent(currentPurchase);
		GameManager.Instance.RequestPltv();
	}

	private void OnPurchaseFailed(string productId, PurchaseConfirmationResult confirmationResult, string failureReason)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
		string text = "";
		text = ((confirmationResult == PurchaseConfirmationResult.Canceled) ? "Error.PurchaseCanceled" : "Error.PurchaseFailed");
		string text2 = "";
		if (OfflineManager.IsGoogleSource)
		{
			Product product = implementationGoogle.GetAllProducts().First((Product x) => x.definition.id == productId);
			text2 = ((product == null) ? productId : product.metadata.localizedTitle);
		}
		else
		{
			EOSStore.GetCatalogByOfferId(productId, out var outOffer);
			text2 = (outOffer.HasValue ? ((string)outOffer.Value.TitleText) : productId);
		}

		AlertPopup.ShowPopup(LocalizationManager.GetText("Error.Error"), LocalizationManager.GetText(text, text2), LocalizationManager.GetText("Button.Ok"));
		UIEvent.Send("OnPurchaseInterrupted");
	}

	private void OnInitializationCompleted()
	{
		ShopPopupHelper.UpdateCurrentTabIfOpen();
	}

	private void OnPurchaseValidationFailed(PurchaseValidationResult result, StorePurchaseInfo currentPurchase)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup).Close();
		SendValidationResultToAnalytics(result, currentPurchase);
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
		}
		AlertPopup.ShowPopupGetText("Error.Error", "Error.PurchaseValidationFailed", "Button.Ok", null);
		UIEvent.Send("OnPurchaseInterrupted");
	}

	private void SendValidationResultToAnalytics(PurchaseValidationResult result, StorePurchaseInfo currentPurchase)
	{
		string bundleIdentifier = "<unknown>";
		if (currentPurchase != null && currentPurchase.Product != null)
		{
			bundleIdentifier = currentPurchase.Product.ProductIdentifier;
		}
		string trackingId = "<unknown>";
		if (currentPurchase != null && currentPurchase.Transaction != null)
		{
			trackingId = currentPurchase.Transaction.TrackingId;
		}
		Metrics.BundleSource bundleSource = GameManager.Instance.BundleSource;
		Helpers.ExecuteCommand(new ReportPurchaseStatusCommand(bundleIdentifier, trackingId, bundleSource, PurchaseConfirmationResult.OK, result, ""));
	}

	public void UnlinkAccount(Action successCallback = null, Action failureCallback = null)
	{
		implementation.UnlinkAccount(successCallback, failureCallback);
	}
}
