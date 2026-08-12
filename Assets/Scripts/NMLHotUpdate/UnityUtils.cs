using System.Collections.Generic;
using System.Linq;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

public class UnityUtils
{
	private static Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>(128);

	public static Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();

	private static char[] pathSeparators = new char[2] { '/', '\\' };

	private static float previousReachabilityUpdateTime = -1f;

	private static NetworkReachability reachability;

	private const float REACHABILITY_UPDATE_INTERVAL = 1f;

	public static NetworkReachability InternetReachability
	{
		get
		{
			if (Time.realtimeSinceStartup - previousReachabilityUpdateTime > 1f)
			{
				previousReachabilityUpdateTime = Time.realtimeSinceStartup;
				reachability = Application.internetReachability;
			}
			return reachability;
		}
	}

	public static Transform FindChild(Transform current, string name)
	{
		if (current.name == name)
		{
			return current;
		}
		for (int i = 0; i < current.childCount; i++)
		{
			Transform transform = FindChild(current.GetChild(i), name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static void AlignItemsInsideContainerGrid(List<GameObject> objects, GameObject container, float offset, bool addToContainer = false, float maxItemScale = -1f, int columnsCount = -1, int rowsCount = -1, bool startsFromRight = true)
	{
		int num = ((columnsCount > -1) ? columnsCount : objects.Count);
		int num2 = (int)Mathf.Ceil((float)objects.Count / (float)num);
		int num3 = num2;
		if (rowsCount > -1)
		{
			num3 = Mathf.Max(rowsCount, num2);
		}
		BoxCollider boxCollider = container.GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = container.GetComponentInChildren<BoxCollider>();
		}
		float x = boxCollider.size.x;
		float y = boxCollider.size.y;
		float num4 = x / (float)num;
		float num5 = y / (float)num3;
		float num6 = y / (float)num3 - offset;
		float num7 = x / (float)num - offset;
		for (int i = 0; i < num2; i++)
		{
			int num8 = i * num;
			int num9 = Mathf.Min(objects.Count - 1, num8 + num - 1);
			for (int j = num8; j <= num9; j++)
			{
				GameObject gameObject = objects[j];
				BoxCollider boxCollider2 = gameObject.GetComponent<BoxCollider>();
				if (boxCollider2 == null)
				{
					boxCollider2 = gameObject.GetComponentInChildren<BoxCollider>();
				}
				float x2 = boxCollider2.size.x;
				float y2 = boxCollider2.size.y;
				float num10 = Mathf.Min(num7 / x2, num6 / y2);
				if (maxItemScale > -1f)
				{
					num10 = Mathf.Min(num10, maxItemScale);
				}
				int num11 = j / num;
				int num12 = j % num;
				float num13 = (startsFromRight ? (x * 0.5f - ((float)num12 * num4 + num4 * 0.5f)) : ((0f - x) * 0.5f + ((float)num12 * num4 + num4 * 0.5f)));
				float num14 = y * 0.5f - (float)num11 * num5 - num5 * 0.5f;
				if (addToContainer)
				{
					gameObject.transform.parent = container.transform;
					gameObject.transform.localPosition = new Vector3(num13 + boxCollider2.center.x * num10, num14 - boxCollider2.center.y * num10, 0f);
				}
				else
				{
					Vector3 localPosition = container.transform.localPosition;
					gameObject.transform.localPosition = new Vector3(localPosition.x + num13 + boxCollider2.center.x * num10, localPosition.y + num14 - boxCollider2.center.y * num10, 0f);
				}
				gameObject.transform.localScale = new Vector3(num10, num10, num10);
			}
		}
	}

	public static void AlignItemsInsideContainerLine(List<GameObject> objects, GameObject container, float offset, bool addToContainer = false, float maxScale = -1f, bool fillHorizontally = true)
	{
		BoxCollider boxCollider = container.GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = container.GetComponentInChildren<BoxCollider>();
		}
		float x = boxCollider.size.x;
		float y = boxCollider.size.y;
		if (objects.Count <= 0)
		{
			return;
		}
		float num = 0f;
		foreach (GameObject @object in objects)
		{
			BoxCollider boxCollider2 = @object.GetComponent<BoxCollider>();
			if (boxCollider2 == null)
			{
				boxCollider2 = @object.GetComponentInChildren<BoxCollider>();
			}
			float x2 = boxCollider2.size.x;
			float y2 = boxCollider2.size.y;
			float num2 = y / y2;
			if (!fillHorizontally)
			{
				num2 = x / x2;
			}
			if (maxScale > -1f)
			{
				num2 = Mathf.Min(num2, maxScale);
			}
			num = ((!fillHorizontally) ? (num + y2 * num2) : (num + x2 * num2));
		}
		num += offset * (float)(objects.Count - 1);
		float num3 = num * -0.5f;
		foreach (GameObject object2 in objects)
		{
			BoxCollider boxCollider3 = object2.GetComponent<BoxCollider>();
			if (boxCollider3 == null)
			{
				boxCollider3 = object2.GetComponentInChildren<BoxCollider>();
			}
			float x3 = boxCollider3.size.x;
			float y3 = boxCollider3.size.y;
			float num4 = y / y3;
			if (!fillHorizontally)
			{
				num4 = x / x3;
			}
			if (maxScale > -1f)
			{
				num4 = Mathf.Min(num4, maxScale);
			}
			float num5 = (fillHorizontally ? x3 : y3);
			if (addToContainer)
			{
				object2.transform.parent = container.transform;
				if (fillHorizontally)
				{
					object2.transform.localPosition = new Vector3(num3 + num5 * 0.5f * num4 + boxCollider3.center.x * num4, (0f - boxCollider3.center.y) * num4, 0f);
				}
				else
				{
					object2.transform.localPosition = new Vector3(boxCollider3.center.x * num4, num3 + num5 * 0.5f * num4 + boxCollider3.center.y * num4, 0f);
				}
			}
			else
			{
				Vector3 localPosition = container.transform.localPosition;
				if (fillHorizontally)
				{
					object2.transform.localPosition = new Vector3(localPosition.x + num3 + num5 * 0.5f * num4 + boxCollider3.center.x * num4, localPosition.y - boxCollider3.center.y * num4, 0f);
				}
				else
				{
					object2.transform.localPosition = new Vector3(localPosition.x + boxCollider3.center.x * num4, localPosition.y + num3 + num5 * 0.5f * num4 - boxCollider3.center.y * num4, 0f);
				}
			}
			object2.transform.localScale = new Vector3(num4, num4, num4);
			num3 += num5 * num4 + offset;
		}
	}

	public static Object LoadAsset(string assetPath)
	{
		Object value = null;
		if (loadedAssets.TryGetValue(assetPath, out value))
		{
			return value;
		}
		return Resources.Load(assetPath);
	}

	public static Object LoadFromAssetBundle(string name, string bundleName)
	{
		Object value = null;
		if (loadedAssets.TryGetValue(name, out value))
		{
			return value;
		}
		if (name.Contains("/"))
		{
			name = name.Substring(name.LastIndexOf('/') + 1);
		}
		return AssetBundleManager.Instance.LoadAsset<Object>(name, bundleName);
	}

	public static T LoadFromAssetBundle<T>(string name, string bundleName) where T : Object
	{
		if (loadedAssets.TryGetValue(name, out Object value))
		{
			return (T)value;
		}
		if (name.Contains("HudElementsConfig"))
		{
			name = name.Substring(name.LastIndexOf('/') + 1);
		}
		return AssetBundleManager.Instance.LoadAsset<T>(name, bundleName);
	}

	public static Object PreloadAsset(string assetPath, string bundleName)
	{
		if (string.IsNullOrEmpty(assetPath))
		{
			return null;
		}
		Object obj = null;
		if (!string.IsNullOrEmpty(assetPath.Trim()))
		{
			if (assetPath.Contains("/"))
			{
				assetPath = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
			}
			if (loadedAssets.ContainsKey(assetPath))
			{
				return loadedAssets[assetPath];
			}
			obj = AssetBundleManager.Instance.LoadAsset<Object>(assetPath, bundleName);
			_ = (bool)obj;
			loadedAssets.Add(assetPath, obj);
		}
		return obj;
	}

	public static void ReleasePreloadedAssets()
	{
		loadedAssets.Clear();
	}

	public static T LoadAsset<T>(string assetPath) where T : Object
	{
		return LoadAsset(assetPath) as T;
	}

	public static string DumpDictionary<T1, T2>(Dictionary<T1, T2> dictionary)
	{
		string text = "";
		foreach (KeyValuePair<T1, T2> item in dictionary)
		{
			if (item.Key != null && item.Value != null)
			{
				text = text + item.Key.ToString() + ": " + item.Value.ToString() + ", ";
			}
		}
		return text;
	}

	public static bool IsPowerOfTwo(uint value)
	{
		if (value != 0)
		{
			return (value & (~value + 1)) == value;
		}
		return false;
	}

	public static string MoveUpInDirPath(string path)
	{
		int num = path.LastIndexOfAny(pathSeparators);
		if (num == path.Length - 1)
		{
			num = path.LastIndexOfAny(pathSeparators, path.Length - 2);
		}
		if (num >= 0)
		{
			return path.Substring(0, num);
		}
		return path;
	}

	public static void StripPhysicsFromHierarchy(GameObject gameObject, bool stripColliders = true, bool stripRigidbodies = true, bool stripJoints = true)
	{
		if (stripColliders)
		{
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i]);
			}
		}
		if (stripJoints)
		{
			Joint[] componentsInChildren2 = gameObject.GetComponentsInChildren<Joint>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Object.Destroy(componentsInChildren2[j]);
			}
		}
		if (stripRigidbodies)
		{
			Rigidbody[] componentsInChildren3 = gameObject.GetComponentsInChildren<Rigidbody>();
			for (int k = 0; k < componentsInChildren3.Length; k++)
			{
				Object.Destroy(componentsInChildren3[k]);
			}
		}
	}

	public static void CombineInHierarchy(GameObject[] gameObjects)
	{
		List<MeshRenderer> list = new List<MeshRenderer>();
		for (int i = 0; i < gameObjects.Length; i++)
		{
			if (gameObjects[i] != null)
			{
				list.AddRange(gameObjects[i].GetComponentsInChildren<MeshRenderer>());
			}
		}
		Dictionary<Material, List<GameObject>> dictionary = new Dictionary<Material, List<GameObject>>();
		for (int j = 0; j < list.Count; j++)
		{
			Material sharedMaterial = list[j].sharedMaterial;
			GameObject gameObject = list[j].gameObject;
			if (dictionary.ContainsKey(sharedMaterial))
			{
				dictionary[sharedMaterial].Add(gameObject);
				continue;
			}
			List<GameObject> value = new List<GameObject> { gameObject };
			dictionary.Add(sharedMaterial, value);
		}
		for (int k = 0; k < dictionary.Count; k++)
		{
			if (dictionary != null && dictionary.ElementAt(k).Value != null && gameObjects[0] != null)
			{
				StaticBatchingUtility.Combine(dictionary.ElementAt(k).Value.ToArray(), gameObjects[0]);
			}
		}
	}

	public static void CombineInHierarchy(GameObject gameObject)
	{
		CombineInHierarchy(new GameObject[1] { gameObject });
	}

	public static void ClearInternetReachabilityCache()
	{
		previousReachabilityUpdateTime = -1f;
	}

	public static void UnloadUsedTextures(Queue<Object> usedTextures)
	{
		while (usedTextures.Count > 0)
		{
			Resources.UnloadAsset(usedTextures.Dequeue());
		}
		usedTextures.Clear();
	}

	public static void CollectTexture(Material material, int property, ref Queue<Object> usedTextures, ref Queue<Object> usedTexturesOutfit)
	{
		if (!material.HasProperty(property))
		{
			return;
		}
		Texture texture = material.GetTexture(property);
		if (!(texture == null))
		{
			if (texture.name.Contains("Outfit"))
			{
				usedTexturesOutfit.Enqueue(texture);
			}
			else
			{
				usedTextures.Enqueue(texture);
			}
		}
	}
}
