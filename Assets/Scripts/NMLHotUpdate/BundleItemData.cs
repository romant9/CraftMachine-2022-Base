using TWDModel;

public class BundleItemData
{
	public BundleStoreDefinition bundleStoreDefinition;

	public BundleContentDefinition bundleContentDefinition;

	public BundleItemData()
	{
	}

	public BundleItemData(string bundleId, GameEconomyData gameEconomyData)
	{
		if (gameEconomyData != null)
		{
			bundleStoreDefinition = gameEconomyData.GetBundleStoreDefinition(bundleId);
			bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(bundleId);
		}
		if (!HasData())
		{
			Debug.LogError("Error when updating BundleItemData with bundleId: " + bundleId);
		}
	}

	public bool HasData()
	{
		if (bundleStoreDefinition != null)
		{
			return bundleContentDefinition != null;
		}
		return false;
	}
}
