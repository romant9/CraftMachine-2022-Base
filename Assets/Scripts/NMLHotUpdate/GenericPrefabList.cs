using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenericPrefabList
{
	public List<PrefabResource> prefabResources;

	public GenericPrefabList()
	{
		prefabResources = new List<PrefabResource>();
	}

	public GameObject GetFirstElement()
	{
		if (prefabResources != null)
		{
			return prefabResources[0].GetPrefab();
		}
		return null;
	}

	public GameObject GetLastElement()
	{
		if (prefabResources != null)
		{
			return prefabResources[prefabResources.Count].GetPrefab();
		}
		return null;
	}

	public GameObject GetElement(int index)
	{
		if (prefabResources != null)
		{
			return prefabResources[index].GetPrefab();
		}
		return null;
	}

	public GameObject GetRandomElement()
	{
		if (prefabResources != null)
		{
			return prefabResources[UnityEngine.Random.Range(0, prefabResources.Count)].GetPrefab();
		}
		return null;
	}

	public GameObject FindElement(string name)
	{
		foreach (PrefabResource prefabResource in prefabResources)
		{
			if (prefabResource.PrefabName == name)
			{
				return prefabResource.GetPrefab();
			}
		}
		return null;
	}
}
