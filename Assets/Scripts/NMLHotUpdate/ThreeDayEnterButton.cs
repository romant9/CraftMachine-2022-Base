using UnityEngine;

public class ThreeDayEnterButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	public void Start()
	{
	}

	private void LateUpdate()
	{
		_ = button.activeInHierarchy;
	}

	private void SpawnLoginCalendar()
	{
	}

	private void OnDestroy()
	{
	}
}
