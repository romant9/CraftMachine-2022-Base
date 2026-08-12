using UnityEngine;

public class OutpostHelpPopup : MonoBehaviour
{
	public void Update()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetKeyUp(KeyCode.Escape))
		{
			HideClicked();
		}
	}

	public void HideClicked()
	{
		base.gameObject.SetActive(value: false);
	}
}
