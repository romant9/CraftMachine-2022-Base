using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Camera")]
public class UIDragCamera : MonoBehaviour
{
	public UIDraggableCamera draggableCamera;

	[HideInInspector]
	public static float DragSpeed;

	public float DragSpeedOverride;

	private void Awake()
	{
		if (draggableCamera == null)
		{
			draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
		}
	}

	public void OnPress(bool isPressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null && draggableCamera.enabled)
		{
			draggableCamera.Press(isPressed);
		}
	}

	public void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null && draggableCamera.enabled)
		{
			if (DragSpeedOverride > 0) DragSpeed = DragSpeedOverride;

            draggableCamera.Drag(delta * DragSpeed);
		}
	}

	public void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null && draggableCamera.enabled)
		{
			draggableCamera.Scroll(delta);
		}
	}
}
