using TWDModel;
using UnityEngine;

public class ShopItemExtra : MonoBehaviour
{
	[SerializeField]
	private UILabel LimitLabel;

	[SerializeField]
	private UISprite priceIconSprite;

	[SerializeField]
	private GameObject MainContent;

	public void UpdateUI(UINewShopItemData newSelect)
	{
		if (newSelect == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(priceIconSprite, value: false);
		int num = 0;
		bool flag = false;
		int num2 = 0;
		string text = "";
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool value = false;
		switch (newSelect.GetShopType())
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
		{
			num = newSelect.storeDefinition.MaxPurchases;
			flag = newSelect.storeDefinition.ShowMaxPurchases;
			text = newSelect.storeDefinition.BundleIdentifier;
			value = playerModel.BundleManager.CanBuyBundle(newSelect.storeDefinition);
			BundleManagerModel bundleManager = playerModel.BundleManager;
			if (bundleManager.BoughtBundlesAmount != null && bundleManager.BoughtBundlesAmount.ContainsKey(text))
			{
				num2 = bundleManager.BoughtBundlesAmount[text];
			}
			break;
		}
		case UINewShopLineData.NewShopItemType.Tradefair:
		{
			num = newSelect.tradefairDefinition.MaxPurchases;
			flag = newSelect.tradefairDefinition.ShowMaxPurchases;
			text = newSelect.tradefairDefinition.BundleIdentifier;
			value = playerModel.TradefairManager.CanBuyBundle(newSelect.tradefairDefinition);
			TradefairManagerModel tradefairManager = playerModel.TradefairManager;
			if (tradefairManager.BoughtBundlesAmount != null && tradefairManager.BoughtBundlesAmount.ContainsKey(text))
			{
				num2 = tradefairManager.BoughtBundlesAmount[text];
			}
			break;
		}
		}
		Helpers.GameObjectSetActive(LimitLabel, value: false);
		if (flag)
		{
			Helpers.GameObjectSetActive(LimitLabel, value: true);
			LimitLabel.text = LocalizationManager.GetText("ShopUI.DetailPage.PurchaseLimit", num - num2, num);
		}
		Helpers.GameObjectSetActive(this, value);
	}

	public void SetMainContentShow(bool show)
	{
		Helpers.GameObjectSetActive(MainContent, show);
	}
}
