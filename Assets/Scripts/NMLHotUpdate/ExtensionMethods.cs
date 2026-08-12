using UnityEngine;

public static class ExtensionMethods
{
	public static void Reset(this Transform trans)
	{
		trans.localRotation = Quaternion.identity;
		trans.localPosition = Vector3.zero;
		trans.localScale = Vector3.one;
	}

	public static GameObject FindInParents<T>(this GameObject obj) where T : Component
	{
		if (obj.GetComponent<T>() != null)
		{
			return obj;
		}
		if (obj.transform.parent != null)
		{
			return obj.transform.parent.gameObject.FindInParents<T>();
		}
		return null;
	}

	public static T FindComponentInParents<T>(this GameObject obj) where T : Component
	{
		T component = obj.GetComponent<T>();
		if (component != null)
		{
			return component;
		}
		if (obj.transform.parent != null)
		{
			return obj.transform.parent.gameObject.FindComponentInParents<T>();
		}
		return null;
	}

	public static void RemoveAllChildren(this GameObject gameObject)
	{
		while (gameObject.transform.childCount != 0)
		{
			Transform child = gameObject.transform.GetChild(0);
			child.parent = null;
			if (child.TryGetComponent<CacheableObject>(out var component))
			{
				component.Destroy();
			}
			else
			{
				Object.Destroy(child.gameObject);
			}
		}
	}

	public static void SetActiveAllChildren(this GameObject gameObject, bool active)
	{
		foreach (Transform item in gameObject.transform)
		{
			item.gameObject.SetActive(active);
		}
	}

	public static void SetLayerRecursively(this GameObject go, int layerNumber)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layerNumber;
		}
	}

	public static Transform FindInChildren(this Transform self, string name)
	{
		int childCount = self.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = self.GetChild(i);
			if (child.name == name)
			{
				return child;
			}
			Transform transform = child.FindInChildren(name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static GameObject FindInChildren(this GameObject self, string name)
	{
		Transform transform = self.transform.FindInChildren(name);
		if (!(transform != null))
		{
			return null;
		}
		return transform.gameObject;
	}

	public static void ConditionalSetPosition(this Transform trans, bool localCoord, Vector3 newPos, bool condition)
	{
		if (condition)
		{
			if (localCoord)
			{
				trans.localPosition = newPos;
			}
			else
			{
				trans.position = newPos;
			}
		}
	}

	public static void ConditionalSetRotation(this Transform trans, bool localCoord, Vector3 newRot, bool condition)
	{
		if (condition)
		{
			if (localCoord)
			{
				trans.localEulerAngles = newRot;
			}
			else
			{
				trans.eulerAngles = newRot;
			}
		}
	}

	public static void ConditionalSetScale(this Transform trans, bool localCoord, Vector3 newScale, bool condition)
	{
		if (condition)
		{
			if (localCoord)
			{
				trans.localScale = newScale;
			}
			else
			{
				trans.localScale = newScale;
			}
		}
	}

	public static GameObject FindDeactiveParent(this GameObject gameObject)
	{
		if (gameObject == null || gameObject.transform == null || gameObject.activeInHierarchy || gameObject.transform.parent == null || gameObject.transform.parent.gameObject == null)
		{
			return null;
		}
		if (!gameObject.transform.parent.gameObject.activeSelf)
		{
			return gameObject.transform.parent.gameObject;
		}
		return gameObject.transform.parent.gameObject.FindDeactiveParent();
	}

	public static bool IsPrefab(this Transform transform)
	{
		if (transform == null)
		{
			return false;
		}
		if (transform.root == transform)
		{
			return transform.parent == null;
		}
		return false;
	}
}
