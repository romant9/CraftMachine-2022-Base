using UnityEngine;

public class ReturnLoginPrivilegePopupItem : MonoBehaviour
{
	[SerializeField]
	private UIButton[] iconButton;

	private void Awake()
	{
		for (int i = 0; i < iconButton?.Length; i++)
		{
			iconButton[i].gameObject.AddComponent<BoxCollider>();
		}
	}

	public void SetActive(bool active)
	{
		for (int i = 0; i < iconButton?.Length; i++)
		{
			SetButtonActive(iconButton[i], active);
		}
	}

	private void SetButtonActive(UIButton button, bool active)
	{
		if (!(button == null))
		{
			button.isEnabled = active;
		}
	}
}
