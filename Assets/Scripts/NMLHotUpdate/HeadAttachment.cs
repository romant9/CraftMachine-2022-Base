using System;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

[Serializable]
public class HeadAttachment
{
	public string PrefabName;

	public Color Color = Color.white;

	public string ReplacementMaterialName;

	private GameObject prefab;

	private Material replacementMaterial;

	public GameObject GetPrefab()
	{
		if (prefab == null)
		{
			prefab = AssetBundleManager.Instance.LoadAsset<GameObject>(PrefabName, ModularCharacter.BundleName);
		}
		return prefab;
	}

	public Material GetReplacementMaterial()
	{
		if (replacementMaterial == null)
		{
			replacementMaterial = AssetBundleManager.Instance.LoadAsset<Material>(ReplacementMaterialName, ModularCharacter.BundleName);
		}
		return replacementMaterial;
	}
}
