using UnityEngine;

public class ItemListEnterButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	private void LateUpdate()
	{
		if (Helpers.CanEnterItemList())
		{
			Helpers.GameObjectSetActive(button, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(button, value: false);
		}
	}
}
