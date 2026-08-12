using System.Collections.Generic;

public class ContentPackManifest
{
	public string AssetVersion;

	public List<ContentPack> ContentPacks;

	public ContentPackManifest(string assetVersion)
	{
		AssetVersion = assetVersion;
		ContentPacks = new List<ContentPack>();
	}
}
