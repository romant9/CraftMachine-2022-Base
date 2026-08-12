namespace NextGames.Sdk.AssetBundleManager
{
	public enum AssetBundleState
	{
		Empty = 0,
		Downloading = 1,
		Downloaded = 2,
		Aborting = 3,
		Unloading = 4,
		Error = 5
	}
}