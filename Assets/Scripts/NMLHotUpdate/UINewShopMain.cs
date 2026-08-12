using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class UINewShopMain : MonoBehaviour
{
	private const int NewShopItemNums = 3;

	[SerializeField]
	private UINewShopList UINewShopList;

	[Header("MainStore")]
	[SerializeField]
	private ShopItemCardBundleNewVersion rightStoreBundle;

	[SerializeField]
	private ShopItemCardBundleTradeFairNewVersion rightFairBundle;

	[SerializeField]
	private ShopItemExtra rightExtra;

	[Header("Component")]
	[SerializeField]
	private ShopGoldDetail shopGoldDetail;

	private UINewShopLineData.NewShopItemType shopItemType = UINewShopLineData.NewShopItemType.Max;

	private UINewShopItemData itemSelectedInfo;

	private UINewShopItemData FirstItemSelectedInfo;

	public UINewShopItemData ItemSelectedInfo => itemSelectedInfo;

	private void SetShopItemType(List<TradefairBundleStoreDefinition> tradefairDefinitions, List<BundleStoreDefinition> storeDefinitions, List<GoldShopDefinition> GoldShopDefinitions)
	{
		if (storeDefinitions != null && storeDefinitions.Count > 0)
		{
			shopItemType = UINewShopLineData.NewShopItemType.BundleStore;
		}
		if (tradefairDefinitions != null && tradefairDefinitions.Count > 0)
		{
			shopItemType = UINewShopLineData.NewShopItemType.Tradefair;
		}
		if (GoldShopDefinitions != null && GoldShopDefinitions.Count > 0)
		{
			shopItemType = UINewShopLineData.NewShopItemType.Gold;
		}
	}

	public void ResetItemSelectedInfo()
	{
		itemSelectedInfo = null;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "NewShopSelectedEvent":
			if (parameter is UINewShopItemData newSelect)
			{
				itemSelectedInfo = newSelect;
				UpdateRight(newSelect);
			}
			break;
		case "NewShopSelectedFirstEvent":
			UIEvent.Send("NewShopSelectedEvent", FirstItemSelectedInfo);
			break;
		case "NewShopTabChanagedEvent":
		case "NewShopToggleChanagedEvent":
			ResetItemSelectedInfo();
			SetRightVisible(visible: true);
			break;
		case "NewShopFilterChanagedEvent":
		{
			object[] obj = (object[])parameter;
			_ = (BundleClassification)obj[0];
			BundleClassification num = (BundleClassification)obj[1];
			ResetItemSelectedInfo();
			if (num == BundleClassification.All)
			{
				SetRightVisible(visible: true);
			}
			else
			{
				SetRightVisible(visible: false);
			}
			break;
		}
		case "NewShopRemovedFromStoreEvent":
			SetRightVisible(visible: false);
			break;
		case "NewShopSetRightVisibleEvent":
		{
			bool rightVisible = (bool)parameter;
			SetRightVisible(rightVisible);
			break;
		}
		}
	}

	public void UpdateUI(BundleClassification newFilter, List<TradefairBundleStoreDefinition> tradefairDefinitions, List<BundleStoreDefinition> storeDefinitions, List<GoldShopDefinition> GoldShopDefinitions)
	{
		if ((tradefairDefinitions == null || tradefairDefinitions.Count <= 0) && (storeDefinitions == null || storeDefinitions.Count <= 0) && (GoldShopDefinitions == null || GoldShopDefinitions.Count <= 0))
		{
			UINewShopList.UpdateListWithData(null, resetScrollPosition: true);
			return;
		}
		SetShopItemType(tradefairDefinitions, storeDefinitions, GoldShopDefinitions);
		UINewShopItemData uINewShopItemData = new UINewShopItemData();
		switch (shopItemType)
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
			uINewShopItemData.storeDefinition = storeDefinitions[0];
			break;
		case UINewShopLineData.NewShopItemType.Gold:
			uINewShopItemData.goldShopDefinition = GoldShopDefinitions[0];
			break;
		case UINewShopLineData.NewShopItemType.Tradefair:
			uINewShopItemData.tradefairDefinition = tradefairDefinitions[0];
			break;
		}
		FirstItemSelectedInfo = uINewShopItemData;
		List<UINewShopLineData> items = GetItems(tradefairDefinitions, storeDefinitions, GoldShopDefinitions);
		if (itemSelectedInfo == null)
		{
			UINewShopList.UpdateListWithData(items, resetScrollPosition: true);
			if (newFilter == BundleClassification.All)
			{
				UIEvent.Send("NewShopSelectedEvent", uINewShopItemData);
			}
		}
		else
		{
			UINewShopList.UpdateListWithData(items, resetScrollPosition: false);
			UIEvent.Send("NewShopSelectedEvent", itemSelectedInfo);
		}
	}

	private List<UINewShopLineData> GetItems(List<TradefairBundleStoreDefinition> tradefairBundles, List<BundleStoreDefinition> bundles, List<GoldShopDefinition> GoldShopDefinitions)
	{
		List<UINewShopLineData> list = new List<UINewShopLineData>();
		bool flag = false;
		int num = 0;
		bool flag2 = false;
		UINewShopLineData uINewShopLineData = new UINewShopLineData();
		switch (shopItemType)
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
		{
			flag = Helpers.GetBundleBannerSwitch();
			num = bundles.Count / 3;
			for (int l = 0; l < num; l++)
			{
				UINewShopLineData uINewShopLineData3 = new UINewShopLineData();
				for (int m = 0; m < 3; m++)
				{
					uINewShopLineData3.AddStore(bundles[l * 3 + m]);
				}
				list.Add(uINewShopLineData3);
			}
			uINewShopLineData = new UINewShopLineData();
			flag2 = false;
			for (int n = 0; n < bundles.Count; n++)
			{
				if (n >= num * 3)
				{
					flag2 = true;
					uINewShopLineData.AddStore(bundles[n]);
				}
			}
			if (flag2)
			{
				list.Add(uINewShopLineData);
			}
			break;
		}
		case UINewShopLineData.NewShopItemType.Gold:
		{
			flag = false;
			num = GoldShopDefinitions.Count / 3;
			for (int num2 = 0; num2 < num; num2++)
			{
				UINewShopLineData uINewShopLineData4 = new UINewShopLineData();
				for (int num3 = 0; num3 < 3; num3++)
				{
					uINewShopLineData4.AddComponent(GoldShopDefinitions[num2 * 3 + num3]);
				}
				list.Add(uINewShopLineData4);
			}
			UINewShopLineData uINewShopLineData5 = new UINewShopLineData();
			flag2 = false;
			for (int num4 = 0; num4 < GoldShopDefinitions.Count; num4++)
			{
				if (num4 >= num * 3)
				{
					flag2 = true;
					uINewShopLineData5.AddComponent(GoldShopDefinitions[num4]);
				}
			}
			if (flag2)
			{
				list.Add(uINewShopLineData5);
			}
			break;
		}
		case UINewShopLineData.NewShopItemType.Tradefair:
		{
			flag = Helpers.GetFairBannerSwitch();
			num = tradefairBundles.Count / 3;
			for (int i = 0; i < num; i++)
			{
				UINewShopLineData uINewShopLineData2 = new UINewShopLineData();
				for (int j = 0; j < 3; j++)
				{
					uINewShopLineData2.AddTrade(tradefairBundles[i * 3 + j]);
				}
				list.Add(uINewShopLineData2);
			}
			uINewShopLineData = new UINewShopLineData();
			flag2 = false;
			for (int k = 0; k < tradefairBundles.Count; k++)
			{
				if (k >= num * 3)
				{
					flag2 = true;
					uINewShopLineData.AddTrade(tradefairBundles[k]);
				}
			}
			if (flag2)
			{
				list.Add(uINewShopLineData);
			}
			break;
		}
		}
		if (flag)
		{
			UINewShopLineData uINewShopLineData6 = new UINewShopLineData();
			uINewShopLineData6.BannerType = shopItemType;
			list.Insert(0, uINewShopLineData6);
		}
		return list;
	}

	private void UpdateRight(UINewShopItemData newSelect)
	{
		rightExtra.UpdateUI(newSelect);
		rightFairBundle.enabled = false;
		rightStoreBundle.enabled = false;
		shopGoldDetail.enabled = false;
		switch (newSelect.GetShopType())
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
			rightStoreBundle.enabled = true;
			rightStoreBundle.SetData(newSelect.storeDefinition);
			break;
		case UINewShopLineData.NewShopItemType.Tradefair:
			rightFairBundle.enabled = true;
			rightFairBundle.SetData(newSelect.tradefairDefinition);
			break;
		case UINewShopLineData.NewShopItemType.Gold:
			shopGoldDetail.enabled = true;
			shopGoldDetail.UpdateUI(newSelect.goldShopDefinition);
			break;
		}
		SetRightVisible(visible: true);
	}

	public void ClearRight()
	{
		rightStoreBundle?.Clear();
		rightFairBundle?.Clear();
	}

	private void SetRightVisible(bool visible)
	{
		rightExtra.SetMainContentShow(show: false);
		Helpers.GameObjectSetActive(shopGoldDetail, value: false);
		if (itemSelectedInfo != null && visible)
		{
			switch (itemSelectedInfo.GetShopType())
			{
			case UINewShopLineData.NewShopItemType.BundleStore:
				rightExtra.SetMainContentShow(show: true);
				break;
			case UINewShopLineData.NewShopItemType.Tradefair:
				rightExtra.SetMainContentShow(show: true);
				break;
			case UINewShopLineData.NewShopItemType.Gold:
				Helpers.GameObjectSetActive(shopGoldDetail, value: true);
				break;
			}
		}
	}
}
