using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class LabelWithURLHandler : MonoBehaviour
{
	public event OnURLClick OnUrlClicked;

	public void OnClick()
	{
		UILabel component = GetComponent<UILabel>();
		if (component != null)
		{
			string urlAtPosition = component.GetUrlAtPosition(UICamera.lastHit.point);
			if (!string.IsNullOrEmpty(urlAtPosition))
			{
				NotifyOnURLClicked(urlAtPosition);
			}
		}
	}

	private void NotifyOnURLClicked(string url)
	{
		this.OnUrlClicked?.Invoke(url);
	}
}
