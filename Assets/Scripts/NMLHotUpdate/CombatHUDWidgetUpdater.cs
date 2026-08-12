using UnityEngine;

public class CombatHUDWidgetUpdater : MonoBehaviour
{
	private UIWidget widget;

	private int lastUpdateFrame;

	private Vector3 lastCameraPosition = Vector3.zero;

	private Quaternion lastCameraRotation = Quaternion.identity;

	private Vector3 lastTargetPosition = Vector3.zero;

	private const int forceUpdateInterval = 32;

	private Camera _cachedCamera;

	private Camera cachedCamera
	{
		get
		{
			if (_cachedCamera == null)
			{
				_cachedCamera = Camera.main;
			}
			return _cachedCamera;
		}
	}

	public void Awake()
	{
		widget = base.gameObject.GetComponent<UIWidget>();
		if (widget == null)
		{
			Debug.LogError("CombatHUDWidgetUpdater failed to get UIWidget component.");
		}
	}

	public void Update()
	{
		if (widget == null)
		{
			return;
		}
		int frameCount = Time.frameCount;
		if (lastUpdateFrame == frameCount)
		{
			return;
		}
		lastUpdateFrame = frameCount;
		if (widget.updateAnchors != UIRect.AnchorUpdate.OnUpdate)
		{
			bool num = (frameCount & 0x1F) == 0;
			Transform target = widget.leftAnchor.target;
			Camera camera = cachedCamera;
			if (num || (camera != null && (camera.transform.position != lastCameraPosition || camera.transform.rotation != lastCameraRotation)) || (target != null && target.position != lastTargetPosition))
			{
				lastCameraPosition = ((camera != null) ? camera.transform.position : Vector3.zero);
				lastCameraRotation = ((camera != null) ? camera.transform.rotation : Quaternion.identity);
				lastTargetPosition = ((target != null) ? target.position : Vector3.zero);
				UIRect.AnchorUpdate updateAnchors = widget.updateAnchors;
				widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
				widget.Update();
				widget.updateAnchors = updateAnchors;
			}
		}
	}
}
