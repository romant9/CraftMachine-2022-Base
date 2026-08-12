using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BundleCardPopup : HUDElement
{
	public const string PREFS_BUNDLE_LAST_SHOWN_DATE = "BUNDLE_LAST_SHOWN_DATE";

	public const string PREFS_LIMITED_OFFER_LAST_SHOWN_DATE = "LIMITED_OFFER_LAST_SHOWN_DATE";

	public const string PREFS_BUNDLE_SHOWN_LIST = "BUNDLE_SHOWN_LIST";

	private const string LOG_NAME = "BundlePopup: ";

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject closeButton;

	[Tooltip("Container for both timer visualization and ok button when popup is blocked by time")]
	[SerializeField]
	private GameObject timerContainer;

	[Tooltip("Progressbar to visualize the timer")]
	[SerializeField]
	private UIProgressBar timerProgressBar;

	[Tooltip("Ok button inside the timer continer used when popup is blocked by time")]
	[SerializeField]
	private UIButton okButton;

	[Tooltip("Time used when the popup timer is enabled")]
	[SerializeField]
	private float cantCloseBeforeSeconds = 3f;

	private ShopItemCard currentCard;

	private bool timerEnabled;

	private bool canClose = true;

	private float remainingTimeToClose;

	private bool _isInitialized;

	public BundleItemData bundleData { get; set; }

	public static void OpenBundle(string bundleId, bool saveTimestamp = false, bool addTimer = false)
	{
		BundleItemData bundleItemData = new BundleItemData(bundleId, GameManager.Instance.gameEconomyData);
		BundleCardPopup bundleCardPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BundleCardPopup) as BundleCardPopup;
		if (!bundleCardPopup.IsOpen && bundleItemData != null && bundleItemData.HasData())
		{
			if (saveTimestamp)
			{
				saveOpenForBundleStoreDefinition(bundleItemData.bundleStoreDefinition);
				makeTimestamp();
			}
			bundleCardPopup.bundleData = bundleItemData;
			bundleCardPopup.Open();
			if (addTimer)
			{
				bundleCardPopup.EnableTimer();
			}
		}
	}

	public static bool TryOpenSuitableBundle(bool saveTimestamp = true, bool addTimer = false)
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.gameEconomyData != null && !TutorialView.Instance.Running)
		{
			if (addTimer)
			{
				addTimer = GameManager.Instance.gameEconomyData.GetFeature("TimerOnBundlePopup").Enabled;
			}
			if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen != 0)
			{
				return false;
			}
			if (!OfflineManager.IsLoadDataManager)
			{
				if (CampView.Instance == null || !CampView.Instance.IsShown)
				{
					return false;
				}
			}
			double secondsSinceLastOpen = getSecondsSinceLastOpen();
			BundleStoreDefinition bundleStoreDefinitionToShowInPromo = GameManager.Instance.playerModel.BundleManager.GetBundleStoreDefinitionToShowInPromo(secondsSinceLastOpen);
			if (bundleStoreDefinitionToShowInPromo != null)
			{
				GameManager.Instance.BundleSource = Metrics.BundleSource.Auto;
				OpenBundle(bundleStoreDefinitionToShowInPromo.BundleIdentifier, saveTimestamp, addTimer);
			}
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Update()
	{
		base.Update();
		if (timerEnabled)
		{
			UpdateTimer();
		}
		if (OfflineManager.Instance.ConnectSourceCurrent == OfflineManager.ConnectSource.Epic) UpdatePriceLabel();
	}

	private void UpdatePriceLabel()
	{
		if (!_isInitialized && GameManager.Instance.IAPManager != null && GameManager.Instance.IAPManager.IsInitialized())
		{
			_isInitialized = true;
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (bundleData != null && bundleData.HasData() && InstantiateCurrentCardIfNeeded() != null)
		{
			InstantiateCurrentCardIfNeeded().SetData(bundleData.bundleStoreDefinition);
		}
	}

	public static void ClearBundleShownTimes()
	{
		TWDPlayerPrefs.DeleteKey("BUNDLE_LAST_SHOWN_DATE");
		TWDPlayerPrefs.DeleteKey("LIMITED_OFFER_LAST_SHOWN_DATE");
	}

	public override void Close()
	{
		if (canClose)
		{
			base.Close();
		}
	}

	public void Clear()
	{
		bundleData = null;
		if (currentCard != null)
		{
			currentCard.Clear();
			Helpers.DestroyOrCache(currentCard);
		}
		currentCard = null;
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBundleBought":
		case "OnPurchaseInterrupted":
			if (timerEnabled)
			{
				StopTimer();
			}
			OnClickClose();
			return;
		case "OnPopUpClose":
			if (parameter != null && parameter is BundleCardPopup && parameter as BundleCardPopup == this)
			{
				if (bundleData != null)
				{
					ShopPopupHelper.SendEndShopVisitAnalytics(new List<BundleStoreDefinition> { bundleData.bundleStoreDefinition }, this);
				}
				Clear();
				return;
			}
			break;
		}
		if (type == "SendEndShopVisitAnalytics" && parameter != null && parameter is ShopItemCard && bundleData != null && bundleData.HasData() && (parameter as ShopItemCard).GetData() == bundleData.bundleStoreDefinition)
		{
			ShopPopupHelper.SendEndShopVisitAnalytics(new List<BundleStoreDefinition> { bundleData.bundleStoreDefinition }, this);
		}
	}

	private ShopItemCard InstantiateCurrentCardIfNeeded()
	{
		if (currentCard == null && bundleData != null)
		{
			string cardPrefab = bundleData.bundleStoreDefinition.CardPrefab;
			NUIListItemBase nUIListItemBase = InstantiateWithSrc(cardPrefab, "uilistitems");
			currentCard = nUIListItemBase as ShopItemCard;
		}
		return currentCard;
	}

	private NUIListItemBase InstantiateWithSrc(string assetName, string bundleName)
	{
		if (!string.IsNullOrEmpty(assetName) && container != null)
		{
			GameObject gameObject = UnityUtils.LoadFromAssetBundle<GameObject>(assetName, bundleName);
			if (gameObject != null)
			{
				return Helpers.InstantiateWithComponent<NUIListItemBase>(gameObject, container);
			}
			Debug.LogWarning("BundlePopup: Could not LoadAsset from src: " + assetName);
		}
		else
		{
			Debug.LogWarning("BundlePopup: Cannot LoadAsset with NULL or Empty assetPath");
		}
		return null;
	}

	protected static double getSecondsSinceLastOpen(bool isOffer = true)
	{
		string key = (isOffer ? "LIMITED_OFFER_LAST_SHOWN_DATE" : "BUNDLE_LAST_SHOWN_DATE");
		if (TWDPlayerPrefs.HasKey(key))
		{
			long dateData = Convert.ToInt64(TWDPlayerPrefs.GetString(key));
			return DateTime.Now.Subtract(DateTime.FromBinary(dateData)).TotalSeconds;
		}
		return -1.0;
	}

	protected static void saveOpenForBundleStoreDefinition(BundleStoreDefinition definition)
	{
		Dictionary<string, int> dictionary = JsonUtility.FromJson<Dictionary<string, int>>(TWDPlayerPrefs.GetString("BUNDLE_SHOWN_LIST"));
		if (dictionary == null)
		{
			dictionary = new Dictionary<string, int>();
		}
		string text = "";

		if (!dictionary.ContainsKey(definition.BundleIdentifier))
		{
			dictionary.Add(definition.BundleIdentifier, 0);
			text = $"Новый бандл ({definition.BundleIdentifier}). Откройте игру, чтобы закрыть его";
			DebugTWD.LogWarning(text, DebugType.System);
		}
		else
		{
			text = $"Этот Бандл  ({definition.BundleIdentifier}) уже открывался";
			DebugTWD.LogWarning(text);
		}
		if (OfflineManager.IsLoadDataManager)
		{
			MyTools.UpdateLogPanel(text);
		}
		dictionary[definition.BundleIdentifier]++;
		string value = JsonUtility.ToJson(dictionary);
		TWDPlayerPrefs.SetString("BUNDLE_SHOWN_LIST", value);
	}

	protected static void makeTimestamp(bool isOffer = true)
	{
		if (isOffer)
		{
			TWDPlayerPrefs.SetString("LIMITED_OFFER_LAST_SHOWN_DATE", DateTime.Now.ToBinary().ToString());
		}
		else
		{
			TWDPlayerPrefs.SetString("BUNDLE_LAST_SHOWN_DATE", DateTime.Now.ToBinary().ToString());
		}
	}

	private void SetOkButtonState()
	{
		if (canClose)
		{
			okButton.enabled = true;
			okButton.SetState(UIButtonColor.State.Normal, true);
		}
		else
		{
			okButton.enabled = false;
			okButton.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	protected void EnableTimer()
	{
		timerEnabled = true;
		canClose = false;
		remainingTimeToClose = cantCloseBeforeSeconds;
		Helpers.GameObjectSetActive(closeButton, value: false);
		Helpers.GameObjectSetActive(timerContainer, value: true);
		SetOkButtonState();
	}

	protected void StopTimer()
	{
		remainingTimeToClose = 0f;
		canClose = true;
		Helpers.GameObjectSetActive(timerProgressBar.gameObject, value: false);
		SetOkButtonState();
	}

	protected void UpdateTimer()
	{
		if (remainingTimeToClose > 0f)
		{
			remainingTimeToClose -= Time.deltaTime;
			timerProgressBar.value = remainingTimeToClose / cantCloseBeforeSeconds;
			if (remainingTimeToClose <= 0f)
			{
				StopTimer();
			}
		}
	}
}
