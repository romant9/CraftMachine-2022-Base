using BaseModel;
using UnityEngine;

public class ActiveFoundationPopup : MonoBehaviour
{
	public void Start()
	{
		GameManager.Instance.playerModel.ActiveFoundationManager.Changed += activeFoundationOnChanged;
	}

	private void OnDestroy()
	{
		GameManager.Instance.playerModel.ActiveFoundationManager.Changed -= activeFoundationOnChanged;
	}

	private void activeFoundationOnChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "PeriodEndEvent" && base.gameObject.activeInHierarchy)
		{
			Close();
		}
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	public void OnInfoClicked()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActiveFoundationInfoPopup, HUDElement.GetParent(this.gameObject)).Open();
	}
}
