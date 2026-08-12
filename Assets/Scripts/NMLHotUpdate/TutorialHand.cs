using UnityEngine;

public class TutorialHand : HUDElementFollowTarget
{
	[Tooltip("How many seconds should it take to interpolate the drag action from start to end.")]
	public float DragTime = 1f;

	[Tooltip("How many seconds should the drag indicator stay at end position until it starts from beginning again.")]
	public float DragEndDelay = 0.5f;

	[Tooltip("GameObject for showing drag actions.")]
	public GameObject Drag;

	[Tooltip("GameObject for showing tap actions.")]
	public GameObject Tap;

	[Tooltip("GameObject for showing dragged trajectory.")]
	public GameObject Trajectory;

	private GameObject anchorProxy;

	private Vector3 dragStart;

	private Vector3 dragEnd;

	private float dragCurrentTime;

	private bool dragActive;

	private UISprite trajectorySprite;

	private float trajectorySpriteAlpha;

	private void Start()
	{
		if (Trajectory != null)
		{
			trajectorySprite = Trajectory.GetComponent<UISprite>();
			trajectorySpriteAlpha = trajectorySprite.alpha;
		}
	}

	private void SetAnchorProxy(Vector3 position)
	{
		if (anchorProxy == null)
		{
			anchorProxy = new GameObject("TutorialHand_AnchorProxy");
		}
		anchorProxy.transform.position = position;
	}

	public void ShowClick(GameObject clickTarget)
	{
		base.gameObject.SetActive(value: true);
		Drag.SetActive(value: false);
		Tap.SetActive(value: true);
		FollowTarget(clickTarget);
		dragActive = false;
	}

	public void ShowClick(Vector3 clickTarget)
	{
		base.gameObject.SetActive(value: true);
		Drag.SetActive(value: false);
		Tap.SetActive(value: true);
		SetAnchorProxy(clickTarget);
		FollowTarget(anchorProxy);
		dragActive = false;
	}

	public void ShowDrag(Vector3 startDrag, Vector3 endDrag)
	{
		base.gameObject.SetActive(value: true);
		Drag.SetActive(value: true);
		Tap.SetActive(value: false);
		dragStart = startDrag;
		dragEnd = endDrag;
		dragActive = true;
		dragCurrentTime = 0f;
		if (trajectorySprite != null)
		{
			trajectorySprite.width = 0;
			trajectorySprite.alpha = 0f;
		}
		SetAnchorProxy(dragStart);
		FollowTarget(anchorProxy);
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
		if (!active)
		{
			dragActive = false;
		}
	}

	public void Update()
	{
		if (dragActive)
		{
			dragCurrentTime += Time.unscaledDeltaTime;
			if (dragCurrentTime > DragTime + DragEndDelay)
			{
				dragCurrentTime -= DragTime + DragEndDelay;
			}
			float num = Mathf.Clamp((DragTime > 0f) ? (dragCurrentTime / DragTime) : dragCurrentTime, 0f, 1f);
			float num2 = 1f - (1f - num) * (1f - num);
			Vector3 position = Vector3.Lerp(dragStart, dragEnd, num2);
			SetAnchorProxy(position);
			if (trajectorySprite != null)
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(dragStart);
				Vector3 vector2 = new Vector3(vector.y, 0f, vector.x);
				Vector3 vector3 = Camera.main.WorldToScreenPoint(position);
				Vector3 vector4 = new Vector3(vector3.y, 0f, vector3.x);
				float num3 = Vector3.Distance(vector2, vector4);
				float pixelSizeAdjustment = UIRoot.GetPixelSizeAdjustment(base.gameObject);
				trajectorySprite.width = (int)(num3 * pixelSizeAdjustment);
				Quaternion quaternion = Quaternion.LookRotation((vector4 - vector2).normalized);
				trajectorySprite.alpha = trajectorySpriteAlpha * num2;
				trajectorySprite.transform.eulerAngles = new Vector3(0f, 0f, quaternion.eulerAngles.y);
			}
		}
	}
}
