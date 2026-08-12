using System.Collections.Generic;
using UnityEngine;

public class PrefabList : ScriptableObject
{
	public List<GameObject> prefabs;

	public GameObject GetFirstElement()
	{
		if (prefabs != null)
		{
			return prefabs[0];
		}
		return null;
	}

	public GameObject GetLastElement()
	{
		if (prefabs != null)
		{
			return prefabs[prefabs.Count];
		}
		return null;
	}

	public GameObject GetElement(int index)
	{
		if (prefabs != null)
		{
			return prefabs[index];
		}
		return null;
	}

	public GameObject GetRandomElement()
	{
		if (prefabs != null)
		{
			return prefabs[Random.Range(0, prefabs.Count)];
		}
		return null;
	}
}
