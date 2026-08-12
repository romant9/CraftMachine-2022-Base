using UnityEngine;

public class CombatMainContainer : MonoBehaviour
{
	[SerializeField]
	private GameObject BottomLeft;

	[SerializeField]
	private GameObject TopRight;

	[SerializeField]
	private GameObject HUD_Elements;

	[SerializeField]
	private GameObject BottomCenter;

	[SerializeField]
	private GameObject BottomRight;

	[SerializeField]
	private GameObject TopCenter;

	[SerializeField]
	private GameObject RightCenter;

	[SerializeField]
	private GameObject HUD_Elements_SP;

	private void SetOnly_SP(bool active)
	{
		Helpers.GameObjectSetActive(BottomLeft, !active);
		Helpers.GameObjectSetActive(TopRight, !active);
		Helpers.GameObjectSetActive(HUD_Elements, !active);
		Helpers.GameObjectSetActive(BottomCenter, !active);
		Helpers.GameObjectSetActive(BottomRight, !active);
		Helpers.GameObjectSetActive(TopCenter, !active);
		Helpers.GameObjectSetActive(RightCenter, !active);
		Helpers.GameObjectSetActive(HUD_Elements_SP, value: true);
	}

	public void SetOnlyNotifi(bool active)
	{
		SetOnly_SP(active);
	}
}
