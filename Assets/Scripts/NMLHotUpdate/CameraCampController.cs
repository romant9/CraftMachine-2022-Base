using System;
using UnityEngine;

public class CameraCampController : MonoBehaviour
{
	[Serializable]
	public struct DeviceSettings
	{
		[SerializeField]
		[Tooltip("If this is true, the fixed width and height will be used to set specific bounds.")]
		public bool SetForSpecificResolution;

		[SerializeField]
		[Tooltip("How far left the camera can move, overwrites default.")]
		public float BoundsLeft;

		[SerializeField]
		[Tooltip("How far right the camera can move, overwrites default")]
		public float BoundsRight;

		[SerializeField]
		[Tooltip("Use this to specify the fixed resolution width.")]
		public int SpecifiedWidth;

		[SerializeField]
		[Tooltip("Use this to specify the fixed resolution height.")]
		public int SpecifiedHeight;
	}

	[Tooltip("Use this to create device or resolution specific settings for the camera.")]
	[SerializeField]
	public DeviceSettings[] DeviceSpecificSettings;

	[Tooltip("The plane/ground that the camera is looking at.")]
	public BoxCollider GroundCollider;

	[SerializeField]
	[Tooltip("How far away the camera looks at the GroundCollider.")]
	private float distance = 50f;

	[SerializeField]
	[Tooltip("How close the camera can get to the GroundCollider.")]
	private float minDistance = 30f;

	[SerializeField]
	[Tooltip("How far away the camera can get from the GroundCollider.")]
	private float maxDistance = 100f;

	[SerializeField]
	[Tooltip("The lower limit of camera pitch.")]
	private float minPitch = 25f;

	[SerializeField]
	[Tooltip("The upper limit of camera pitch")]
	private float maxPitch = 45f;

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
	[Tooltip("This value does not make any sense and is currently unused.")]
	private float doubleTapZoomIn = 0.2f;

	[SerializeField]
	[Tooltip("This value does not make any sense and is currently unused.")]
	private float doubleTapZoomOut = 0.7f;

	[SerializeField]
	[Tooltip("This value does not make any sense and is currently unused.")]
	private float doubleTapZoomSpeed = 0.15f;

	[SerializeField]
	[Tooltip("This value does not make any sense and is currently unused.")]
	private float doubleTapTreshold = 0.33f;

	[SerializeField]
	[Tooltip("How close it is possible to zoom.")]
	private float minZoom = 0.8f;

	[SerializeField]
	[Tooltip("How far away it is possible to zoom.")]
	private float maxZoom = 1f;

	private Vector3 target;

	private Vector3 offset;

	private int scrollingId;

	private int zoomingId;

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

	private bool pendingDoubleTap;

	private float doubleTapDistance;

	private Vector3 zoomInToStartLocation;

	private float zoomInToStartDistance;

	private float panT;

	private float panSpeed;

	private Vector3 panStartPosition;

	private float panStartDistance;

	private Vector3 panEndPosition;

	private float panEndDistance;

	private float fov;

	private Camera cachedCamera;

	public bool Scrolled { get; private set; }

	public bool ScrollEnabled { get; set; }

	public bool Zoomed { get; private set; }

	public bool Lerping { get; private set; }

	public bool Panning { get; private set; }

	public GameObject TargetGameObject { get; private set; }

	public float Distance => distance;

	public float MinDistance => minDistance;

	public float MaxDistance => maxDistance;

	public Vector3 TargetPosition => target;

	private void Start()
	{
		cachedCamera = GetComponent<Camera>();
		if (distance < 0f)
		{
			distance = 50f;
		}
		fov = cachedCamera.fieldOfView;
		offset = new Vector3(0f, 0f, 0f);
		UpdateOrientation();
		scrollingId = -1;
		zoomingId = -1;
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
		else
		{
			scrollingId = -1;
			zoomingId = -1;
		}
		if (TargetGameObject != null)
		{
			MoveTo(base.transform.parent.InverseTransformPoint(TargetGameObject.transform.position), 10f);
		}
		if (Panning)
		{
			panT = Mathf.Min(panT + panSpeed * Time.deltaTime, 1f);
			float t = 0.5f * (1f - Mathf.Cos(panT * MathF.PI));
			target = Vector3.Lerp(panStartPosition, panEndPosition, t);
			distance = Mathf.Lerp(panStartDistance, panEndDistance, t);
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
		UpdateShake();
	}

	public void SetEnabled(bool enabled)
	{
		if (base.gameObject.activeSelf != enabled)
		{
			base.gameObject.SetActive(enabled);
		}
	}

	private void OnEnable()
	{
		Scrolled = false;
		scrollStarted = false;
		scrollingId = -1;
		Zoomed = false;
		ScrollEnabled = true;
	}

	public void CheckDeviceSpecifics()
	{
		for (int i = 0; i < DeviceSpecificSettings.Length; i++)
		{
			DeviceSettings deviceSettings = DeviceSpecificSettings[i];
			if (Application.platform == RuntimePlatform.IPhonePlayer || !deviceSettings.SetForSpecificResolution || (Screen.width == deviceSettings.SpecifiedWidth && Screen.height == deviceSettings.SpecifiedHeight))
			{
				boundsLeft = deviceSettings.BoundsLeft;
				boundsRight = deviceSettings.BoundsRight;
			}
		}
	}

	public void FollowTargetGameObject(GameObject targetGameObject)
	{
		TargetGameObject = targetGameObject;
	}

	public void StartPan(Vector3 endPosition, float endDistance, float time)
	{
		panT = 0f;
		panSpeed = ((time > 0f) ? (1f / time) : 1f);
		panStartPosition = target;
		panStartDistance = distance;
		panEndPosition = endPosition;
		panEndDistance = endDistance;
		Panning = true;
		Scrolled = true;
	}

	public void Reset(Vector3 position, float distance)
	{
		target = position;
		this.distance = distance;
		Scrolled = false;
		Zoomed = false;
		Panning = false;
		Lerping = false;
		UpdateOrientation();
	}

	public float GetWorldZoom()
	{
		float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
		return Mathf.Lerp(minZoom, maxZoom, t);
	}

	public float GetDistance()
	{
		return distance;
	}

	public void ZoomInTo(Vector3 target, float distance, float time)
	{
		zoomInToStartLocation = this.target;
		zoomInToStartDistance = this.distance;
		StartPan(target, distance, time);
	}

	public void ZoomOutFrom(float time)
	{
		if (zoomInToStartDistance != 0f && zoomInToStartLocation != Vector3.zero)
		{
			if (time == 0f)
			{
				distance = zoomInToStartDistance;
			}
			else
			{
				StartPan(target, zoomInToStartDistance, time);
			}
			zoomInToStartDistance = 0f;
			zoomInToStartLocation = Vector3.zero;
		}
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

	public void Shake(float amplitude, float duration)
	{
		shakeAmplitude = amplitude;
		shakeDuration = duration;
		shakeTime = duration;
	}

	private void UpdateShake()
	{
		if (shakeTime > 0f)
		{
			float num = shakeTime / shakeDuration;
			shakeTime = Mathf.Max(shakeTime - Time.deltaTime, 0f);
			Vector3 vector = UnityEngine.Random.insideUnitSphere * shakeAmplitude * num;
			base.transform.localPosition += vector;
		}
	}

	private void UpdateDoubleTap()
	{
		if (Input.GetMouseButtonDown(0) && ScrollEnabled)
		{
			if (pendingDoubleTap)
			{
				CancelInvoke("ClearDoubleTap");
				ClearDoubleTap();
				if (distance - minDistance < (maxDistance - minDistance) * 0.25f)
				{
					doubleTapDistance = Mathf.Lerp(minDistance, maxDistance, doubleTapZoomOut);
				}
				else
				{
					doubleTapDistance = Mathf.Lerp(minDistance, maxDistance, doubleTapZoomIn);
				}
			}
			else
			{
				pendingDoubleTap = true;
				Invoke("ClearDoubleTap", doubleTapTreshold);
			}
		}
		if (doubleTapDistance != 0f)
		{
			distance = Mathf.Lerp(distance, doubleTapDistance, doubleTapZoomSpeed);
			if (Mathf.Abs(doubleTapDistance - distance) < 1f)
			{
				doubleTapDistance = 0f;
			}
		}
	}

	private void ClearDoubleTap()
	{
		pendingDoubleTap = false;
	}

	private void CheckBounds()
	{
		if (!(GroundCollider == null) && GroundCollider.gameObject.activeInHierarchy)
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
	}

	private void GetWorldPosition(ref Vector2 screenPosition, ref Vector3 worldPosition)
	{
		if (GroundCollider != null && GroundCollider.gameObject.activeInHierarchy)
		{
			Ray ray = cachedCamera.ScreenPointToRay(screenPosition);
			if (!GroundCollider.Raycast(ray, out var hitInfo, 4000f))
			{
				Ray ray2 = ray;
				Debug.LogError("GetWorldPosition failed" + ray2.ToString());
			}
			worldPosition = hitInfo.point;
		}
	}

	private void UpdateOrientation()
	{
		UpdateCamera();
		CheckBounds();
	}

	protected void UpdateCamera()
	{
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
		float num = maxDistance - minDistance;
		float t = (distance - minDistance) / ((num > 0f) ? num : 1f);
		float num2 = Mathf.Lerp(minPitch, maxPitch, t);
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(num2, 0f, 0f);
		base.transform.localRotation = identity;
		float f = num2 * (MathF.PI / 180f);
		offset.y = distance * Mathf.Sin(f);
		offset.z = (0f - distance) * Mathf.Cos(f);
		base.transform.localPosition = target + offset;
		Vector3 vector = base.transform.parent.TransformPoint(target);
		Vector3 vector2 = cachedCamera.WorldToScreenPoint(vector + new Vector3(1f, 0f, 0f));
		scrollFactors.x = 1f / (vector2.x - (float)Screen.width * 0.5f);
		Vector3 vector3 = cachedCamera.WorldToScreenPoint(vector + new Vector3(0f, 0f, 1f));
		scrollFactors.y = 1f / (vector3.y - (float)Screen.height * 0.5f);
		float num3 = fov;
		if (Screen.height > Screen.width)
		{
			num3 = fov * (float)Screen.height / (float)Screen.width;
		}
		if (cachedCamera.fieldOfView != num3)
		{
			cachedCamera.fieldOfView = num3;
		}
	}

	private void UpdateTouchGestures()
	{
		Touch touch = Input.GetTouch(0);
		if (scrollingId == -1 && touch.phase == TouchPhase.Began && HelpersUI.GetTouchedUIObject() == null)
		{
			StartScrolling(touch.fingerId, touch.position);
		}
		if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			if (zoomingId != -1)
			{
				EndZooming(zoomingId != touch.fingerId);
			}
			else
			{
				EndScrolling();
			}
		}
		if (touch.fingerId == scrollingId)
		{
			UpdateScrolling(touch.position);
		}
		if (Input.touchCount < 2)
		{
			return;
		}
		Touch touch2 = Input.GetTouch(1);
		Touch touch3 = ((touch.fingerId == scrollingId) ? touch2 : touch);
		if (touch3.phase == TouchPhase.Began)
		{
			StartZooming(touch3.fingerId, touch.position, touch2.position);
		}
		else if (touch2.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled)
		{
			if (scrollingId == touch2.fingerId)
			{
				EndScrolling();
			}
			else
			{
				EndZooming(switchTouch: false);
			}
		}
		if (touch2.fingerId == scrollingId)
		{
			UpdateScrolling(touch2.position);
		}
		if (zoomingId != -1)
		{
			UpdateZooming(touch.position, touch2.position);
		}
	}

	private void UpdateMouseGestures()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (HelpersUI.GetTouchedUIObject() == null)
			{
				StartScrolling(0, Input.mousePosition);
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			EndScrolling();
		}
		else if (scrollingId != -1)
		{
			UpdateScrolling(Input.mousePosition);
		}
		if (Input.GetMouseButtonDown(1))
		{
			StartZooming(1, Vector2.zero, Input.mousePosition);
		}
		else if (Input.GetMouseButtonUp(1))
		{
			EndZooming(switchTouch: false);
		}
		else if (zoomingId != -1)
		{
			UpdateZooming(Vector2.zero, Input.mousePosition);
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
		delta.y = position.y - prevPosition0.y;
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

	private void StartZooming(int id, Vector2 position0, Vector2 position1)
	{
		if (ScrollEnabled)
		{
			zoomingId = id;
			prevScreenDistance = (position1 - position0).magnitude;
			prevPosition1 = position1;
			Zoomed = false;
			doubleTapDistance = 0f;
		}
	}

	private void EndZooming(bool switchTouch)
	{
		if (switchTouch)
		{
			scrollingId = zoomingId;
			prevPosition0 = prevPosition1;
		}
		zoomingId = -1;
		Zoomed = false;
	}

	private void UpdateZooming(Vector2 position0, Vector2 position1)
	{
		float magnitude = (position1 - position0).magnitude;
		if (magnitude != prevScreenDistance)
		{
			Zoomed = true;
		}
		distance += (prevScreenDistance - magnitude) * scrollFactors.x * 10f;
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
		prevScreenDistance = magnitude;
		prevPosition1 = position1;
		UpdateOrientation();
	}

	public void SetMaxCameraMaxDistance(float distance)
	{
		maxDistance = distance;
	}

	public void FitToBounds(Bounds worldBounds)
	{
		float num = minDistance;
		float num2 = maxDistance;
		float fieldOfView = cachedCamera.fieldOfView;
		float num3 = fieldOfView * (float)Screen.width / (float)Screen.height;
		float x = worldBounds.extents.x;
		float num4 = worldBounds.extents.z * Mathf.Cos(minPitch * (MathF.PI / 180f));
		float a = x / Mathf.Tan(num3 * 0.5f * (MathF.PI / 180f));
		float b = num4 / Mathf.Tan(fieldOfView * 0.5f * (MathF.PI / 180f));
		maxDistance = Mathf.Max(a, b);
		minDistance = maxDistance * 0.4f;
		target = worldBounds.center;
		float num5 = distance;
		distance = maxDistance;
		UpdateCamera();
		Vector3 worldPosition = Vector3.zero;
		Vector2 screenPosition = default(Vector2);
		screenPosition.x = 0f;
		screenPosition.y = (float)Screen.height * 0.5f;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		boundsLeft = worldPosition.x;
		screenPosition.x = Screen.width;
		screenPosition.y = (float)Screen.height * 0.5f;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		boundsRight = worldPosition.x;
		screenPosition.x = (float)Screen.width * 0.5f;
		screenPosition.y = 0f;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		boundsBottom = worldPosition.z;
		screenPosition.x = (float)Screen.width * 0.5f;
		screenPosition.y = Screen.height;
		GetWorldPosition(ref screenPosition, ref worldPosition);
		boundsTop = worldPosition.z;
		distance = num5;
		minDistance = num;
		maxDistance = num2;
		UpdateCamera();
		CheckDeviceSpecifics();
	}
}
