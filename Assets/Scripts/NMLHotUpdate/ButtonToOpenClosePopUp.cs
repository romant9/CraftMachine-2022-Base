using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class ButtonToOpenClosePopUp : MonoBehaviour
{
	[SerializeField]
	private List<UIType> toOpen = new List<UIType>();

	[SerializeField]
	private List<UIType> toClose = new List<UIType>();

	private UIButton uiButton;

	private EventDelegate clickHandler;

	private void Awake()
	{
		uiButton = GetComponent<UIButton>();
		clickHandler = new EventDelegate(OnClickEventHandler);
	}

	private void OnEnable()
	{
		uiButton.onClick.Add(clickHandler);
	}

	private void OnDisable()
	{
		uiButton.onClick.Remove(clickHandler);
	}

	private void OnClickEventHandler()
	{
		foreach (UIType item in toOpen)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(item, HUDElement.GetParent(this.gameObject))?.Open();
		}
		foreach (UIType item2 in toClose)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(item2, HUDElement.GetParent(this.gameObject))?.Close();
		}
	}
}
