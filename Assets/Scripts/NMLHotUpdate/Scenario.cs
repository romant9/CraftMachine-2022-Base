using UnityEngine;

[ExecuteInEditMode]
public class Scenario : MonoBehaviour
{
	[HideInInspector]
	public string BackgroundScene;

	public string Description;

	public bool IncludedInBuild = true;

	[ReadOnly]
	public string ExportHash;

	[ReadOnly]
	public string ExportVersion;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		if (!(ScenarioManager.Instance != null))
		{
			return;
		}
		foreach (GameObject scenario in ScenarioManager.Instance.GetScenarios())
		{
			if (scenario != base.gameObject)
			{
				scenario.SetActive(value: false);
			}
		}
	}
}
