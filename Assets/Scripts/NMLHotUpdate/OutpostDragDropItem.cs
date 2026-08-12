using UnityEngine;

public class OutpostDragDropItem : UIDragDropItem
{
	[SerializeField]
	private UISprite canPlaceSprite;

	[SerializeField]
	private UISprite cannotPlaceSprite;

	public OutpostSliceHotspot originalHotspot;

	protected override void Awake()
	{
		base.Awake();
		HideIndicators();
	}

	public void HideIndicators()
	{
		if (canPlaceSprite != null && cannotPlaceSprite != null)
		{
			canPlaceSprite.gameObject.SetActive(value: false);
			cannotPlaceSprite.gameObject.SetActive(value: false);
		}
	}

	protected override void OnDragDropRelease(GameObject surface)
	{
		if (CanPlace() && originalHotspot != null)
		{
			base.OnDragDropRelease(surface);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drop");
		}
		else
		{
			base.OnDragDropRelease(null);
		}
	}

	protected override void OnDragDropStart()
	{
		originalHotspot = base.transform.parent.GetComponent<OutpostSliceHotspot>();
		base.OnDragDropStart();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drag");
	}

	protected override void OnDragDropMove(Vector2 delta)
	{
		base.OnDragDropMove(delta);
		if (canPlaceSprite != null && cannotPlaceSprite != null)
		{
			bool flag = CanPlace();
			SetActiveGameObject(canPlaceSprite.gameObject, flag);
			SetActiveGameObject(cannotPlaceSprite.gameObject, !flag);
		}
	}

	private void SetActiveGameObject(GameObject obj, bool value)
	{
		if (obj != null && obj.activeSelf != value)
		{
			obj.SetActive(value);
		}
	}

	protected virtual bool CanPlace()
	{
		return false;
	}

	protected UIDragDropContainer GetDestinationDropContainer()
	{
		GameObject touchedUIObject = HelpersUI.GetTouchedUIObject();
		if (!touchedUIObject)
		{
			return null;
		}
		return NGUITools.FindInParents<UIDragDropContainer>(touchedUIObject);
	}

	protected OutpostSliceHotspot GetDestinationHotspot()
	{
		GameObject touchedUIObject = HelpersUI.GetTouchedUIObject();
		UIDragDropContainer uIDragDropContainer = (touchedUIObject ? NGUITools.FindInParents<UIDragDropContainer>(touchedUIObject) : null);
		if (uIDragDropContainer != null)
		{
			return uIDragDropContainer.GetComponent<OutpostSliceHotspot>();
		}
		return null;
	}
}
