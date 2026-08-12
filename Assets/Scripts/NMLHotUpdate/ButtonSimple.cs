using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class ButtonSimple : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	public void SetLabel(string text)
	{
		base.gameObject.SetActive(text != "");
		if (label != null)
		{
			label.text = text;
		}
	}

	public void SetCallback(EventDelegate.Callback callback)
	{
		UIButton component = GetComponent<UIButton>();
		component.onClick.Clear();
		component.onClick.Add(new EventDelegate(callback));
	}
}
