using UnityEngine;

public class BundleRewardCardsResource : ScriptableObject
{
	public BundleCardEntry[] resources;

	public GameObject GetBundleRewardCardPrefab(string identifier)
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
