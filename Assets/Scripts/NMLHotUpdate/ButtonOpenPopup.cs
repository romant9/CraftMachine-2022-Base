using UnityEngine;

[RequireComponent(typeof(UIButton), typeof(BoxCollider))]
public class ButtonOpenPopup : MonoBehaviour
{
	[SerializeField]
	private UIType popupType;

	private void OnClick()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(popupType, HUDElement.GetParent(this.gameObject));
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
	}
}
