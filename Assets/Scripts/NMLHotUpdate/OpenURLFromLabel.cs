using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class OpenURLFromLabel : MonoBehaviour
{
	public void OnClick()
	{
		UILabel component = GetComponent<UILabel>();
		if (component != null)
		{
			string urlAtPosition = component.GetUrlAtPosition(UICamera.lastHit.point);
			if (!string.IsNullOrEmpty(urlAtPosition))
			{
				Application.OpenURL(urlAtPosition);
			}
		}
	}
}
