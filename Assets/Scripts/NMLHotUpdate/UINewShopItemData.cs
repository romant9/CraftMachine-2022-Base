using TWDModel;

public class UINewShopItemData
{
	public BundleStoreDefinition storeDefinition;

	public TradefairBundleStoreDefinition tradefairDefinition;

	public GoldShopDefinition goldShopDefinition;

	public UINewShopLineData.NewShopItemType GetShopType()
	{
		UINewShopLineData.NewShopItemType result = UINewShopLineData.NewShopItemType.Max;
		if (storeDefinition != null)
		{
			result = UINewShopLineData.NewShopItemType.BundleStore;
		}
		if (tradefairDefinition != null)
		{
			result = UINewShopLineData.NewShopItemType.Tradefair;
		}
		if (goldShopDefinition != null)
		{
			result = UINewShopLineData.NewShopItemType.Gold;
		}
		return result;
	}

	public bool IsEmptyData()
	{
		bool result = false;
		if (storeDefinition == null && tradefairDefinition == null && goldShopDefinition == null)
		{
			result = true;
		}
		return result;
	}

	public string GetDataID()
	{
		string result = "";
		switch (GetShopType())
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
			result = storeDefinition.BundleIdentifier;
			break;
		case UINewShopLineData.NewShopItemType.Tradefair:
			result = tradefairDefinition.BundleIdentifier;
			break;
		case UINewShopLineData.NewShopItemType.Gold:
			result = goldShopDefinition.ItemId;
			break;
		}
		return result;
	}
}
