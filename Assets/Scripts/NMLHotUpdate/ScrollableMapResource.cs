using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

public class ScrollableMapResource : ScriptableObject
{
	public string PrefabName;

	public string TextureName;

	public int Width;

	public int Height;

	private GameObject prefab;

	private Texture texture;

	public static string BundleName => "scrollablemapresources";

	public Texture GetTexture()
	{
		if (texture == null)
		{
			texture = AssetBundleManager.Instance.LoadAsset<Texture>(TextureName, BundleName);
		}
		return texture;
	}

	public GameObject GetPrefab()
	{
		if (prefab == null)
		{
			prefab = AssetBundleManager.Instance.LoadAsset<GameObject>(PrefabName, BundleName);
		}
		return prefab;
	}
}
