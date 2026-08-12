using UnityEngine;

public class BundleCardsResource : ScriptableObject
{
	public static string DefaultBundleCard = "Shop_Item_Card";

	public BundleCardEntry[] resources;

	public GameObject GetBundleCardPrefab(string identifier)
	{
		for (int i = 0; i < resources.Length; i++)
		{
			BundleCardEntry bundleCardEntry = resources[i];
			if (bundleCardEntry.Identifier == identifier)
			{
				return bundleCardEntry.PrefabResource.GetPrefab();
			}
		}
		return null;
	}
}
