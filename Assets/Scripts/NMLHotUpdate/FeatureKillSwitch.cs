using UnityEngine;

public class FeatureKillSwitch : MonoBehaviour
{
	[SerializeField]
	[Header("Behaviours to disable when the feature is off")]
	private Behaviour[] behavioursToDisable;

	[HideInInspector]
	[SerializeField]
	public string feature;

	private void OnEnable()
	{
		if (GameManager.Instance.gameEconomyData.GetFeature(feature).Enabled)
		{
			Behaviour[] array = behavioursToDisable;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}
	}
}
