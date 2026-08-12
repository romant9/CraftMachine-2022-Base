using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActorResourceEntry : ResourceEntry
{
	public string IconSprite;

	public List<string> PrefabResourceList;

	public string PortraitTexture;

	public string CharacterScreenPrefab;

	internal GameObject GetRandomPrefab()
	{
		return UnityUtils.LoadFromAssetBundle<PrefabResource>(PrefabResourceList[UnityEngine.Random.Range(0, PrefabResourceList.Count)], "scriptableobjects").GetPrefab();
	}

	internal void GetRandomPrefabAsync(Action<GameObject> callback)
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(PrefabResourceList[UnityEngine.Random.Range(0, PrefabResourceList.Count)], "scriptableobjects");
		if (prefabResource == null)
		{
			callback(null);
		}
		else
		{
			prefabResource.GetPrefabAsync(callback);
		}
	}

	internal GameObject GetFirstPrefab()
	{
		return UnityUtils.LoadFromAssetBundle<PrefabResource>(PrefabResourceList[0], "scriptableobjects").GetPrefab();
	}

	internal GameObject GetPrefab(int index)
	{
		return UnityUtils.LoadFromAssetBundle<PrefabResource>(PrefabResourceList[index], "scriptableobjects").GetPrefab();
	}

	internal GameObject GetCharacterScreenPrefab()
	{
		return UnityUtils.LoadFromAssetBundle<GameObject>(CharacterScreenPrefab, "scriptableobjects");
	}
}
