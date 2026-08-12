using UnityEngine;

public abstract class ResourcesMap<T> : ScriptableObject where T : ResourceEntry
{
	public T[] resources;

	public T GetResources(string identifier)
	{
		for (int i = 0; i < resources.Length; i++)
		{
			T val = resources[i];
			if (val.Identifier == identifier)
			{
				return val;
			}
		}
		return null;
	}
}
