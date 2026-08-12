using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
	public static WeatherSystem Instance;

	public WeatherResourcesData Data;

	private Dictionary<string, GameObject> currentEffects;

	public static List<string> ListOfEffects
	{
		get
		{
			if (Instance == null || Instance.Data == null)
			{
				return null;
			}
			return Instance.Data.ListOfEffects;
		}
	}

	public static WeatherSystem AttachToParent(GameObject parent)
	{
		Instance = Helpers.AddComponent<WeatherSystem>(parent);
		if (Instance != null)
		{
			Instance.LoadData();
		}
		return Instance;
	}

	public static GameObject InstantiateEffect(string name)
	{
		if (Instance == null || Instance.gameObject == null)
		{
			return null;
		}
		if (Instance.currentEffects == null)
		{
			Instance.currentEffects = new Dictionary<string, GameObject>();
		}
		if (Instance.currentEffects.ContainsKey(name))
		{
			return Instance.currentEffects[name];
		}
		GameObject gameObject = Helpers.InstantiateFromResourcesToParent("WeatherSystem/" + name, Instance.gameObject);
		if (gameObject != null)
		{
			Instance.currentEffects.Add(name, gameObject);
		}
		return gameObject;
	}

	public static void RemoveEffect(string name)
	{
		if (!(Instance == null) && !(Instance.gameObject == null) && Instance.currentEffects != null && Instance.currentEffects.TryGetValue(name, out var value))
		{
			Helpers.DestroyOrCache(value);
			Instance.currentEffects.Remove(name);
		}
	}

	public static void RemoveAllEffects()
	{
		if (Instance == null || Instance.currentEffects == null)
		{
			return;
		}
		foreach (KeyValuePair<string, GameObject> currentEffect in Instance.currentEffects)
		{
			if (currentEffect.Value != null)
			{
				Helpers.DestroyOrCache(currentEffect.Value);
			}
		}
		Instance.currentEffects.Clear();
	}

	private void LoadData()
	{
		string text = "WeatherSystem/WeatherResourcesData";
		if (Data == null)
		{
			Data = UnityUtils.LoadFromAssetBundle<WeatherResourcesData>(text, "scriptableobjects");
			if (Data == null)
			{
				Debug.LogError("Could not load src: Resources/" + text);
			}
		}
	}
}
