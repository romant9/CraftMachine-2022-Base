using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : SingularityMonoBehaviour<ObjectPoolManager>
{
	private class CacheRepopulationDefinition
	{
		public int LowLimit;

		public int HighLimit = -1;

		public int RepopulateAmount;

		public int InitialAmount = 1;
	}

	private const string CLONE_SUFFIX = "(Clone)";

	private GameObject containerObject;

	private Dictionary<string, Stack<GameObject>> objectsAvailableToUse = new Dictionary<string, Stack<GameObject>>();

	private Dictionary<string, CacheRepopulationDefinition> RepopulateDefinitions = new Dictionary<string, CacheRepopulationDefinition>();

	protected void Start()
	{
		containerObject = new GameObject("__MEMORY_POOL_CONTAINER__");
		containerObject.transform.parent = base.gameObject.transform;
		Helpers.GameObjectSetActive(containerObject, value: false);
	}

	public void SetupCacheForObject(GameObject prefab, int initialAmount, int lowLimit = 0, int highLimit = -1, int repopulateAmount = 1)
	{
		string key = prefab.name + "(Clone)";
		if (RepopulateDefinitions.ContainsKey(key))
		{
			RepopulateDefinitions.Remove(key);
		}
		CacheRepopulationDefinition cacheRepopulationDefinition = new CacheRepopulationDefinition();
		cacheRepopulationDefinition.LowLimit = lowLimit;
		cacheRepopulationDefinition.HighLimit = highLimit;
		cacheRepopulationDefinition.RepopulateAmount = repopulateAmount;
		cacheRepopulationDefinition.InitialAmount = initialAmount;
		RepopulateDefinitions.Add(key, cacheRepopulationDefinition);
		PopulateCache(prefab, initialAmount);
	}

	[ContextMenu("DestroyAllObjects")]
	public void DestroyAllObjects()
	{
		foreach (KeyValuePair<string, Stack<GameObject>> item in objectsAvailableToUse)
		{
			foreach (GameObject item2 in item.Value)
			{
				Object.Destroy(item2);
			}
		}
		objectsAvailableToUse.Clear();
		RepopulateDefinitions.Clear();
	}

	public GameObject FetchObject(GameObject prefab, Transform parent = null)
	{
		string key = prefab.name + "(Clone)";
		if (!RepopulateDefinitions.ContainsKey(key))
		{
			SetupCacheForObject(prefab, 1);
		}
		CacheRepopulationDefinition cacheRepopulationDefinition = RepopulateDefinitions[key];
		Stack<GameObject> stack = objectsAvailableToUse[key];
		if (stack.Count == 0)
		{
			PopulateCache(prefab, cacheRepopulationDefinition.RepopulateAmount);
		}
		GameObject gameObject = null;
		if (stack.Count > 0)
		{
			gameObject = stack.Pop();
		}
		if (stack.Count <= cacheRepopulationDefinition.LowLimit && cacheRepopulationDefinition.RepopulateAmount > 0)
		{
			PopulateCache(prefab, cacheRepopulationDefinition.RepopulateAmount);
		}
		if ((bool)gameObject)
		{
			gameObject.transform.SetParent(parent);
			Helpers.GameObjectSetActive(gameObject, value: true);
			gameObject.SendMessage("OnPoolRetrieve", SendMessageOptions.DontRequireReceiver);
		}
		return gameObject;
	}

	public void ReturnObjectToPool(GameObject obj)
	{
		if (obj == null || string.IsNullOrEmpty(obj.name))
		{
			return;
		}
		obj.SendMessage("OnPoolReturn", SendMessageOptions.DontRequireReceiver);
		if (RepopulateDefinitions == null)
		{
			RepopulateDefinitions = new Dictionary<string, CacheRepopulationDefinition>();
		}
		if (objectsAvailableToUse == null)
		{
			objectsAvailableToUse = new Dictionary<string, Stack<GameObject>>();
		}
		string key = obj.name;
		if (RepopulateDefinitions.ContainsKey(key) && objectsAvailableToUse.ContainsKey(key))
		{
			CacheRepopulationDefinition cacheRepopulationDefinition = RepopulateDefinitions[key];
			Stack<GameObject> stack = objectsAvailableToUse[key];
			if (cacheRepopulationDefinition == null || stack == null || containerObject == null)
			{
				Object.Destroy(obj);
				return;
			}
			if (cacheRepopulationDefinition.HighLimit > -1 && stack.Count >= cacheRepopulationDefinition.HighLimit)
			{
				Object.Destroy(obj);
				return;
			}
			if (!stack.Contains(obj))
			{
				stack.Push(obj);
			}
			obj.transform.parent = containerObject.transform;
			Helpers.GameObjectSetActive(obj, value: false);
			return;
		}
		if (objectsAvailableToUse.ContainsKey(key))
		{
			objectsAvailableToUse.Remove(key);
		}
		if (RepopulateDefinitions.ContainsKey(key))
		{
			RepopulateDefinitions.Remove(key);
		}
		CacheRepopulationDefinition cacheRepopulationDefinition2 = new CacheRepopulationDefinition();
		cacheRepopulationDefinition2.LowLimit = 0;
		cacheRepopulationDefinition2.HighLimit = -1;
		cacheRepopulationDefinition2.RepopulateAmount = 1;
		cacheRepopulationDefinition2.InitialAmount = 1;
		if (!RepopulateDefinitions.ContainsKey(key))
		{
			RepopulateDefinitions.Add(key, cacheRepopulationDefinition2);
		}
		Stack<GameObject> stack2 = new Stack<GameObject>();
		objectsAvailableToUse.Add(key, stack2);
		stack2.Push(obj);
		obj.transform.parent = null;
		Helpers.GameObjectSetActive(obj, value: false);
	}

	private void PopulateCache(GameObject prefab, int amount)
	{
		int num = amount;
		string key = prefab.name + "(Clone)";
		if (RepopulateDefinitions.ContainsKey(key) && objectsAvailableToUse.ContainsKey(key))
		{
			CacheRepopulationDefinition cacheRepopulationDefinition = RepopulateDefinitions[key];
			Stack<GameObject> stack = objectsAvailableToUse[key];
			if (cacheRepopulationDefinition.HighLimit > -1)
			{
				num = Mathf.Min(amount, cacheRepopulationDefinition.HighLimit - stack.Count);
			}
		}
		if (!objectsAvailableToUse.ContainsKey(key))
		{
			Stack<GameObject> value = new Stack<GameObject>();
			objectsAvailableToUse.Add(key, value);
		}
		for (int i = 0; i < num; i++)
		{
			if (!prefab.GetComponent<CacheableObject>())
			{
				throw new UnityException();
			}
			GameObject gameObject = Object.Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			gameObject.transform.parent = containerObject.transform;
			Helpers.GameObjectSetActive(gameObject, value: false);
			objectsAvailableToUse[key].Push(gameObject);
		}
	}
}
