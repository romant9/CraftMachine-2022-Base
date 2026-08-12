using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class OpenLinkFromLabel : MonoBehaviour
{
	public void OnClick()
	{
		UILabel component = GetComponent<UILabel>();
		if (!(component != null))
		{
			return;
		}
		string urlAtPosition = component.GetUrlAtPosition(UICamera.lastHit.point);
		if (!string.IsNullOrEmpty(urlAtPosition))
		{
			LinkPopup linkPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LinkPopup) as LinkPopup;
			if (linkPopup != null)
			{
				linkPopup.ShowPopup(urlAtPosition);
			}
		}
	}
}
