using System;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

public class PrefabResource : ScriptableObject
{
	public string PrefabName;

	private GameObject assetBundlePrefab;

	public static string BundleName => "prefabresources";

	public GameObject GetPrefab()
	{
		if (assetBundlePrefab == null)
		{
			assetBundlePrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(PrefabName, BundleName, IsCustom);
		}
		return assetBundlePrefab;
	}

	public void GetPrefabAsync(Action<GameObject> callback)
	{
		if (assetBundlePrefab != null)
		{
			callback(assetBundlePrefab);
			return;
		}
		AssetBundleManager.Instance.LoadAssetAsync(PrefabName, BundleName, delegate(GameObject asset)
		{
			assetBundlePrefab = asset;
			callback(assetBundlePrefab);
		});
	}


	#region myparams
	public bool IsCustom;
	#endregion
}
