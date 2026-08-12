using System;

[Serializable]
public class AssetBundlesResourceEntry : ResourceEntry
{
	public bool ShouldLoadOnStart;

	public bool ContainsAssetsToBeInstantiated;

	public bool UnloadInCombat;
}
