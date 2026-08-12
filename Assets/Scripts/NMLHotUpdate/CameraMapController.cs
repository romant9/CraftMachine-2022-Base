using System;
using UnityEngine;

public class CameraMapController : MonoBehaviour
{
	[Tooltip("The plane/ground that the camera is looking at.")]
	public BoxCollider GroundCollider;

	[SerializeField]
	[Tooltip("How far away the camera looks at the GroundCollider.")]
	private float Distance = 50f;

	[SerializeField]
	[Tooltip("camera pitch.")]
	private float Pitch = 25f;

	[SerializeField]
	[Tooltip("How quickly the camera stops after the player lifts his finger.")]
	private float damping = 0.1f;

	[SerializeField]
	[Tooltip("How much the user has to move the finger for the camera to move.")]
	private int scrollTreshold = 5;

	[SerializeField]
	[Tooltip("Setting this to 0 or 100000 does not make any noticeable difference.")]
	private float lerpSpeed = 0.2f;

	[SerializeField]
	[Tooltip("How far left the camera can move")]
	private float boundsLeft = -320f;

	[SerializeField]
	[Tooltip("How far right the camera can move")]
	private float boundsRight = 320f;

	[SerializeField]
	[Tooltip("How far north can the camera move")]
	private float boundsTop = 320f;

	[SerializeField]
	[Tooltip("How far south the camera can move")]
	private float boundsBottom = -320f;

	[SerializeField]
	[Tooltip("Should automatically calculate bounds (boundsLeft, boundsRight).")]
	private bool autoCalculateBounds;

	public bool constrainHorizontal = true;

	private Vector3 target;

	private Vector3 offset;

	private int scrollingId;

	private Vector2 prevPosition0;

	private Vector2 prevPosition1;

	private float prevScreenDistance;

	private Vector2 scrollFactors;

	private Vector2 delta;

	private Vector3 velocity;

	private bool scrollStarted;

	private Vector3 lerpTarget;

	private float shakeAmplitude;

	private float shakeDuration;

	private float shakeTime;

	private float panT;

	private float panSpeed;

	private Vector3 panStartPosition;

	private float panStartDistance;

	private Vector3 panEndPosition;

	private float panEndDistance;

	private float fov;

	public bool Scrolled { get; private set; }

	public bool ScrollEnabled { get; set; }

	public bool Lerping { get; private set; }

	public bool Panning { get; private set; }

	public Vector3 TargetPosition => target;

	private void Awake()
	{
		ScrollEnabled = true;
	}

	private void Start()
	{
		if (Distance < 0f)
		{
			Distance = 50f;
		}
		fov = GetComponent<Camera>().fieldOfView;
		offset = new Vector3(0f, 0f, 0f);
		if (GroundCollider.Raycast(new Ray(base.transform.position, base.transform.forward), out var hitInfo, 9999f))
		{
			target = hitInfo.point;
		}
		UpdateOrientation();
		scrollingId = -1;
	}

	private void Update()
	{
		if (GameManager.Instance != null && GameManager.Instance.AllowCameraMove())
		{
			if (Input.touchCount > 0)
			{
				UpdateTouchGestures();
			}
			else
			{
				UpdateMouseGestures();
			}
		}
		if (Panning)
		{
			panT = Mathf.Min(panT + panSpeed * Time.deltaTime, 1f);
			float t = 0.5f * (1f - Mathf.Cos(panT * MathF.PI));
			target = Vector3.Lerp(panStartPosition, panEndPosition, t);
			Distance = Mathf.Lerp(panStartDistance, panEndDistance, t);
			if (panT == 1f)
			{
				Panning = false;
			}
		}
		else if (Lerping)
		{
			target = Vector3.Lerp(target, lerpTarget, lerpSpeed * 60f * Time.deltaTime);
			if (Vector3.Distance(target, lerpTarget) < 1f)
			{
				Lerping = false;
			}
		}
		else
		{
			target += velocity * Time.deltaTime;
			velocity = Vector3.Lerp(velocity, Vector3.zero, damping * 60f * Time.deltaTime);
		}
		UpdateOrientation();
	}

	private void OnEnable()
	{
		Scrolled = false;
		scrollStarted = false;
		scrollingId = -1;
	}

	public void StartPan(Vector3 endPosition, float endDistance, float time)
	{
		panT = 0f;
		panSpeed = 1f / time;
		panStartPosition = target;
		panStartDistance = Distance;
		panEndPosition = endPosition;
		panEndDistance = endDistance;
		Panning = true;
		Scrolled = true;
	}

	public void Reset(Vector3 position, float distance)
	{
		target = position;
		Distance = distance;
		Scrolled = false;
		Panning = false;
		Lerping = false;
		UpdateOrientation();
	}

	public void MoveTo(Vector3 target)
	{
		Scrolled = true;
		Lerping = true;
		lerpTarget = target;
		velocity = Vector3.zero;
	}

	public void MoveTo(Vector3 target, float velocity)
	{
		Scrolled = true;
		Lerping = true;
		lerpTarget = target;
		lerpSpeed = velocity;
	}

	public void UpdateBounds(float left, float right)
	{
		if (autoCalculateBounds)
		{
			boundsLeft = left;
			boundsRight = right;
		}
	}

	private void CheckBounds()
	{
		Vector2 screenPosition = default(Vector2);
		Vector3 worldPosition = default(Vector3);
		Vector3 vector = target;
		screenPosition.x = 0f;
		screenPosition.y = (float)Screen.height * 0.5f;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		vector.x -= Mathf.Min(worldPosition.x - boundsLeft, 0f);
		screenPosition.x = Screen.width;
		screenPosition.y = (float)Screen.height * 0.5f;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		vector.x -= Mathf.Max(worldPosition.x - boundsRight, 0f);
		screenPosition.x = (float)Screen.width * 0.5f;
		screenPosition.y = 0f;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		vector.z -= Mathf.Min(worldPosition.z - boundsBottom, 0f);
		screenPosition.x = (float)Screen.width * 0.5f;
		screenPosition.y = Screen.height;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		vector.z -= Mathf.Max(worldPosition.z - boundsTop, 0f);
		if (target != vector)
		{
			target = vector;
			base.transform.localPosition = target + offset;
			Lerping = false;
		}
	}

	private void GetWorldPosition(ref Vector2 screenPosition, ref Vector3 worldPosition)
	{
		Ray ray = GetComponent<Camera>().ScreenPointToRay(screenPosition);
		if (!GroundCollider.Raycast(ray, out var hitInfo, 4000f))
		{
			Ray ray2 = ray;
			Debug.LogError("GetWorldPosition failed" + ray2.ToString());
		}
		worldPosition = hitInfo.point;
	}

	private void UpdateOrientation()
	{
		UpdateCamera();
		CheckBounds();
	}

	protected void UpdateCamera()
	{
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(Pitch, 0f, 0f);
		base.transform.localRotation = identity;
		float f = Pitch * (MathF.PI / 180f);
		offset.y = Distance * Mathf.Sin(f);
		offset.z = (0f - Distance) * Mathf.Cos(f);
		base.transform.localPosition = target + offset;
		Vector3 vector = base.transform.parent.TransformPoint(target);
		Vector3 vector2 = GetComponent<Camera>().WorldToScreenPoint(vector + new Vector3(1f, 0f, 0f));
		scrollFactors.x = 1f / (vector2.x - (float)Screen.width * 0.5f);
		Vector3 vector3 = GetComponent<Camera>().WorldToScreenPoint(vector + new Vector3(0f, 0f, 1f));
		scrollFactors.y = 1f / (vector3.y - (float)Screen.height * 0.5f);
		float num = fov;
		if (GetComponent<Camera>().fieldOfView != num)
		{
			GetComponent<Camera>().fieldOfView = num;
		}
	}

	private void UpdateTouchGestures()
	{
		Touch touch = Input.GetTouch(0);
		if (scrollingId == -1 && touch.phase == TouchPhase.Began)
		{
			StartScrolling(touch.fingerId, touch.position);
		}
		if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			EndScrolling();
		}
		if (touch.fingerId == scrollingId)
		{
			UpdateScrolling(touch.position);
		}
	}

	private void UpdateMouseGestures()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StartScrolling(0, Input.mousePosition);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			EndScrolling();
		}
		else if (scrollingId != -1)
		{
			UpdateScrolling(Input.mousePosition);
		}
	}

	private void StartScrolling(int id, Vector2 position)
	{
		scrollingId = id;
		prevPosition0 = position;
		scrollStarted = false;
		Scrolled = false;
	}

	private void EndScrolling()
	{
		scrollingId = -1;
		scrollStarted = false;
		Scrolled = false;
	}

	private void UpdateScrolling(Vector2 position)
	{
		delta.x = position.x - prevPosition0.x;
		delta.y = (constrainHorizontal ? 0f : (position.y - prevPosition0.y));
		if (scrollStarted)
		{
			if (delta.x != 0f || delta.y != 0f)
			{
				Scrolled = true;
			}
			velocity.x = (0f - delta.x) * scrollFactors.x / Time.deltaTime;
			velocity.z = (0f - delta.y) * scrollFactors.y / Time.deltaTime;
			prevPosition0 = position;
		}
		if (ScrollEnabled && !scrollStarted && delta.magnitude >= (float)scrollTreshold)
		{
			scrollStarted = true;
			prevPosition0 = position;
		}
	}
}
