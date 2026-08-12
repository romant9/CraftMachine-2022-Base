using UnityEngine;

public class OutpostPlaceItemDragDrop : UIDragDropItem
{
	protected override void OnDragDropStart()
	{
		base.OnDragDropStart();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drag");
	}

	protected override void OnDragDropRelease(GameObject surface)
	{
		base.OnDragDropRelease(null);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drop");
	}
}
