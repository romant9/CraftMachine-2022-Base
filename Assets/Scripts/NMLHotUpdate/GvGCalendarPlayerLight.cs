using UnityEngine;

public class GvGCalendarPlayerLight : MonoBehaviour
{
	public enum CalendarPlayerLightState
	{
		Empty = 0,
		SignedUp = 1,
		FullSlot = 2
	}

	[SerializeField]
	private GameObject emptySlot;

	[SerializeField]
	private GameObject signedUpSlot;

	[SerializeField]
	private GameObject fullSlot;

	public void SetState(CalendarPlayerLightState state)
	{
		Helpers.GameObjectSetActive(emptySlot, state == CalendarPlayerLightState.Empty);
		Helpers.GameObjectSetActive(signedUpSlot, state == CalendarPlayerLightState.SignedUp);
		Helpers.GameObjectSetActive(fullSlot, state == CalendarPlayerLightState.FullSlot);
	}
}
