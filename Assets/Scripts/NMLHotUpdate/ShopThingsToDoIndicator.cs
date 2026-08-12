using BaseModel;
using TWDModel;
using UnityEngine;

public class ShopThingsToDoIndicator : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	[Header("GED shopTabIndex. -1 is all.")]
	[SerializeField]
	private int shopTabIndex = -1;

	[Header("Optional")]
	[SerializeField]
	private GameObject labelParent;

	private BundleStoreDefinition storeDefinition;

	public virtual void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			UpdateUI();
			UIEvent.OnUIEvent -= OnUIEvent;
			UIEvent.OnUIEvent += OnUIEvent;
			GameManager.Instance.playerModel.Changed -= OnPlayerModelChanged;
			GameManager.Instance.playerModel.Changed += OnPlayerModelChanged;
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	public virtual void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			UIEvent.OnUIEvent -= OnUIEvent;
			GameManager.Instance.playerModel.Changed -= OnPlayerModelChanged;
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
		storeDefinition = null;
	}

	public virtual void UpdateUI()
	{
		if (GameManager.Instance != null)
		{
			string text = "";
			if (shopTabIndex == -1)
			{
				text = (ShopPopupHelper.ContainsAnyFreeItems() ? LocalizationManager.GetText("Generic.Free") : "");
			}
			else if (shopTabIndex == 3)
			{
				text = (ShopPopupHelper.ContainsFreeTradeShopItems() ? LocalizationManager.GetText("Generic.Free") : "");
			}
			else if (shopTabIndex == 1)
			{
				text = ((ShopPopupHelper.GetFirstFreeIapItemTradeFair() != null) ? LocalizationManager.GetText("Generic.Free") : "");
			}
			else
			{
				storeDefinition = ShopPopupHelper.GetFirstFreeIapItem(shopTabIndex);
				if (storeDefinition != null)
				{
					text = (ShopPopupHelper.ContainsAnyFreeItems() ? LocalizationManager.GetText("Generic.Free") : "");
				}
			}
			bool flag = text != "";
			HelpersUI.SetContentToLabel(label, text, flag);
			Helpers.GameObjectSetActive(labelParent, flag);
		}
		else
		{
			Helpers.GameObjectSetActive(label, value: false);
			Helpers.GameObjectSetActive(labelParent, value: false);
		}
	}

	protected virtual void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnTradeCrateSlotPurchased":
		case "OnTradeCratePurchased":
		case "OnTradeEquipmentPurchased":
		case "OnBundleBought":
			UpdateUI();
			break;
		}
	}

	protected void OnPlayerModelChanged(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "TradeShopRefreshed":
		case "TradeShopSlotBought":
		case "TradeShopItemBought":
			UpdateUI();
			break;
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI();
	}
}
