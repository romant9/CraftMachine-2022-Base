using System.Collections.Generic;
using TWDModel;

public class UINewShopLineData
{
	public enum NewShopItemType
	{
		BundleStore = 0,
		Tradefair = 1,
		Gold = 2,
		Max = 3
	}

	public NewShopItemType BannerType = NewShopItemType.Max;

	private List<BundleStoreDefinition> storeDefinitions = new List<BundleStoreDefinition>();

	private List<TradefairBundleStoreDefinition> tradeDefinitions = new List<TradefairBundleStoreDefinition>();

	private List<GoldShopDefinition> goldShopDefinitions = new List<GoldShopDefinition>();

	public bool IsBanner()
	{
		if (BannerType != NewShopItemType.Max)
		{
			return true;
		}
		return false;
	}

	public NewShopItemType GetShopType()
	{
		if (IsBanner())
		{
			return BannerType;
		}
		NewShopItemType result = NewShopItemType.Max;
		if (storeDefinitions != null && storeDefinitions.Count > 0)
		{
			result = NewShopItemType.BundleStore;
		}
		if (tradeDefinitions != null && tradeDefinitions.Count > 0)
		{
			result = NewShopItemType.Tradefair;
		}
		if (goldShopDefinitions != null && goldShopDefinitions.Count > 0)
		{
			result = NewShopItemType.Gold;
		}
		return result;
	}

	public List<BundleStoreDefinition> GetBundleStores()
	{
		return storeDefinitions;
	}

	public List<TradefairBundleStoreDefinition> GetTradefairs()
	{
		return tradeDefinitions;
	}

	public List<GoldShopDefinition> GetComponentItems()
	{
		return goldShopDefinitions;
	}

	public void AddStore(BundleStoreDefinition bundle)
	{
		storeDefinitions.Add(bundle);
	}

	public void AddTrade(TradefairBundleStoreDefinition bundle)
	{
		tradeDefinitions.Add(bundle);
	}

	public void AddComponent(GoldShopDefinition bundle)
	{
		goldShopDefinitions.Add(bundle);
	}
}
