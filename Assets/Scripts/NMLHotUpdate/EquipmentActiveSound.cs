using UnityEngine;

public class EquipmentActiveSound : MonoBehaviour
{
	[SerializeField]
	private string eventName;

	public void SetEquipmentActive(bool active)
	{
		if (active)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName, base.gameObject);
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent(eventName, base.gameObject);
		}
	}
}
