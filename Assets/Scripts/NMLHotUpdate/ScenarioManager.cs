using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScenarioManager : MonoBehaviour
{
	private static ScenarioManager instance;

	public static ScenarioManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<ScenarioManager>();
			}
			return instance;
		}
	}

	private void Update()
	{
		if (base.transform.position != Vector3.zero || base.transform.rotation != Quaternion.identity || base.transform.localScale != Vector3.one)
		{
			base.transform.Reset();
		}
	}

	public GameObject GetActiveScenario()
	{
		GameObject gameObject = null;
		foreach (GameObject scenario in GetScenarios())
		{
			if (scenario.activeSelf)
			{
				if (gameObject != null)
				{
					Debug.LogError("Multiple scenarios active!");
					return null;
				}
				gameObject = scenario;
			}
		}
		return gameObject;
	}

	public List<GameObject> GetScenarios()
	{
		return new List<GameObject>();
	}

	public GameObject FindScenario(string name)
	{
		foreach (GameObject scenario in GetScenarios())
		{
			if (scenario.name.Equals(name))
			{
				return scenario.gameObject;
			}
		}
		return null;
	}

	public bool CheckDuplicateNames()
	{
		List<GameObject> scenarios = GetScenarios();
		HashSet<string> hashSet = new HashSet<string>();
		bool result = true;
		foreach (GameObject item in scenarios)
		{
			if (hashSet.Contains(item.name))
			{
				Debug.LogError("Duplicate name " + item.name);
				result = false;
			}
			else
			{
				hashSet.Add(item.name);
			}
		}
		return result;
	}
}
