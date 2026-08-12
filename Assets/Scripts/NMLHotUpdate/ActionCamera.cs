using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ActionCamera : MonoBehaviour
{
	private ActionCameraData actionCameraData;

	private Vector3 startPosition;

	private Quaternion startRotation;

	private float startFOV;

	private Vector3 targetPosition;

	private Quaternion targetRotation;

	private float targetFOV;

	private float interpolationStartTime;

	private int targetCameraParamIndex;

	private List<Vector3> targets;

	private Vector3 originalPosition;

	private Quaternion originalRotation;

	private float originalFOV;

	private bool hasOriginalProperties;

	private bool ActionRequested;

	private ActionCameraState state;

	private Camera CameraComponent;

	private int currentParamsIndex;

	private int currentTargetIndex;

	private float endDelayTimer;

	private int nextSignalCount;

	[Tooltip("Index of the Target Camera Params entry to be previewed.")]
	public int InterpolationParamsIndex;

	private float previewWaitTimer;

	[Tooltip("Should trigger always during gameplay.")]
	public bool AlwaysTrigger;

	public static ActionCamera Instance { get; private set; }

	public ActionCameraData ActionCameraData
	{
		get
		{
			if (actionCameraData == null)
			{
				string text = "ActionCameraData";
				if (GameManager.Instance.playerModel.Combat.IsSurvivalMission)
				{
					text = "ActionCameraData_Survival";
				}
				actionCameraData = UnityUtils.LoadFromAssetBundle<ActionCameraData>(text, "scriptableobjects");
			}
			return actionCameraData;
		}
	}

	public bool AllowedToActivate { get; set; }

	public int LastInstigatorId { get; private set; }

	public float LastTimeActivated { get; private set; }

	public bool IsCooldownActive
	{
		get
		{
			if (!AlwaysTrigger)
			{
				if (ActionCameraData != null && LastTimeActivated > 0f)
				{
					return Time.time - LastTimeActivated < ActionCameraData.CooldownTime;
				}
				return false;
			}
			return false;
		}
	}

	private TargetCameraParams CurrentTargetCameraParams
	{
		get
		{
			if (ActionCameraData.TargetCameraParams == null || currentParamsIndex < 0 || currentParamsIndex >= ActionCameraData.TargetCameraParams.Count)
			{
				return null;
			}
			return ActionCameraData.TargetCameraParams[currentParamsIndex];
		}
	}

	public bool HasCurrentTarget
	{
		get
		{
			if (targets != null && currentTargetIndex >= 0)
			{
				return currentTargetIndex < targets.Count;
			}
			return false;
		}
	}

	public Vector3 CurrentTarget
	{
		get
		{
			if (targets == null || currentTargetIndex < 0 || currentTargetIndex >= targets.Count)
			{
				return new Vector3(0f, 0f, 0f);
			}
			return targets[currentTargetIndex];
		}
	}

	public bool IsActive => state != ActionCameraState.Idle;

	public bool IsAtTarget => state == ActionCameraState.AtTarget;

	public event Action OnCameraReady;

	private void Awake()
	{
		Instance = this;
		LastTimeActivated = -1000f;
		AllowedToActivate = true;
		CameraComponent = GetComponent<Camera>();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void SetState(ActionCameraState newState)
	{
		if (state != newState)
		{
			state = newState;
		}
	}

	private void StoreCameraProperties()
	{
		if (!hasOriginalProperties)
		{
			originalPosition = base.transform.position;
			originalRotation = base.transform.localRotation;
			originalFOV = CameraComponent.fieldOfView;
			hasOriginalProperties = true;
		}
	}

	private void RestoreCameraProperties()
	{
		if (hasOriginalProperties)
		{
			hasOriginalProperties = false;
			base.transform.position = originalPosition;
			base.transform.localRotation = originalRotation;
			CameraComponent.fieldOfView = originalFOV;
		}
	}

	private int GetTargetCameraParamsIndex(ActionCameraType actionCameraType)
	{
		for (int i = 0; i < ActionCameraData.TargetCameraParams.Count; i++)
		{
			if (ActionCameraData.TargetCameraParams[i].actionCameraType == actionCameraType)
			{
				return i;
			}
		}
		return -1;
	}

	private void StartInterpolation(List<Vector3> interpolationTargets, int targetParamsIndex)
	{
		if (state == ActionCameraState.Idle)
		{
			StoreCameraProperties();
			targets = interpolationTargets;
			currentTargetIndex = -1;
			currentParamsIndex = targetParamsIndex;
			if (!NextTarget())
			{
				SetState(ActionCameraState.Idle);
				RestoreCameraProperties();
			}
		}
	}

	private Quaternion GetTargetRotation(TargetCameraParams cameraParams)
	{
		return Quaternion.Euler(0f, cameraParams.yaw, 0f) * Quaternion.Euler(cameraParams.pitch, 0f, 0f);
	}

	private Vector3 GetTargetDirection(TargetCameraParams cameraParams)
	{
		return Vector3.Normalize(Quaternion.Euler(0f, cameraParams.yaw, 0f) * Quaternion.Euler(cameraParams.pitch, 0f, 0f) * new Vector3(0f, 0f, 1f));
	}

	private Vector3 GetTargetPosition(Vector3 targetBasePosition, TargetCameraParams cameraParams)
	{
		Vector3 vector = Quaternion.Euler(0f, cameraParams.yaw, 0f) * Quaternion.Euler(cameraParams.pitch, 0f, 0f) * new Vector3(0f, 0f, 1f);
		return targetBasePosition + new Vector3(0f, cameraParams.heightOffset, 0f) - vector * cameraParams.distance;
	}

	private bool NextTarget()
	{
		if (state == ActionCameraState.Idle || state == ActionCameraState.AtTarget)
		{
			currentTargetIndex++;
			if (HasCurrentTarget)
			{
				SetState(ActionCameraState.InterpolatingToTarget);
				endDelayTimer = 0f;
				interpolationStartTime = Time.time;
				startPosition = base.transform.position;
				startRotation = base.transform.localRotation;
				startFOV = CameraComponent.fieldOfView;
				targetRotation = GetTargetRotation(CurrentTargetCameraParams);
				targetPosition = GetTargetPosition(CurrentTarget, CurrentTargetCameraParams);
				targetFOV = CurrentTargetCameraParams.fov;
				return true;
			}
		}
		return false;
	}

	private void StopInterpolation()
	{
		if (state != ActionCameraState.Idle && state != ActionCameraState.InterpolatingToOriginal)
		{
			SetState(ActionCameraState.InterpolatingToOriginal);
			interpolationStartTime = Time.time;
			startPosition = base.transform.position;
			startRotation = base.transform.localRotation;
			startFOV = CameraComponent.fieldOfView;
			targetPosition = originalPosition;
			targetRotation = originalRotation;
			targetFOV = originalFOV;
		}
	}

	private void UpdateInterpolation()
	{
		if (CurrentTargetCameraParams == null)
		{
			RestoreCameraProperties();
			SetState(ActionCameraState.Idle);
			return;
		}
		float num = Mathf.Clamp((Time.time - interpolationStartTime) / CurrentTargetCameraParams.interpolationTime, 0f, 1f);
		float t = CurrentTargetCameraParams.interpolationCurve.Evaluate(num);
		base.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
		base.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
		CameraComponent.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
		if (!(num >= 1f))
		{
			return;
		}
		if (state == ActionCameraState.InterpolatingToTarget)
		{
			SetState(ActionCameraState.AtTarget);
		}
		else if (state == ActionCameraState.InterpolatingToOriginal)
		{
			SetState(ActionCameraState.Idle);
			RestoreCameraProperties();
			if (this.OnCameraReady != null)
			{
				this.OnCameraReady();
			}
		}
	}

	private bool UseCooldownForType(ActionCameraType actionCameraType)
	{
		if (actionCameraType != ActionCameraType.CombatExitLocation)
		{
			return actionCameraType != ActionCameraType.SurvivorDeath;
		}
		return false;
	}

	public bool RequestActionCamera(Vector3 targetPosition, ActionCameraType actionCameraType, int instigatorId = -1)
	{
		if (!AllowedToActivate || (IsCooldownActive && UseCooldownForType(actionCameraType)) || PlayerInputManager.Instance.IsButtonDown)
		{
			ActionRequested = false;
			return false;
		}
		LastInstigatorId = instigatorId;
		currentParamsIndex = -1;
		for (int i = 0; i < ActionCameraData.TargetCameraParams.Count; i++)
		{
			TargetCameraParams targetCameraParams = ActionCameraData.TargetCameraParams[i];
			Vector3 targetDirection = GetTargetDirection(targetCameraParams);
			Vector3 vector = GetTargetPosition(targetPosition, targetCameraParams);
			Vector3 vector2 = targetPosition + new Vector3(0f, targetCameraParams.heightOffset, 0f);
			float num = Vector3.Distance(vector, vector2);
			int mask = LayerMask.GetMask("LVL", "Static");
			if (targetCameraParams.actionCameraType != actionCameraType)
			{
				continue;
			}
			bool flag = false;
			RaycastHit[] array = Physics.RaycastAll(new Ray(vector, targetDirection), Mathf.Clamp(num - 0.75f, 0f, 100f));
			for (int j = 0; j < array.Length; j++)
			{
				int num2 = 1 << array[j].collider.gameObject.layer;
				if (!array[j].collider.isTrigger && (num2 & mask) != 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				array = Physics.RaycastAll(new Ray(vector2, -targetDirection), Mathf.Clamp(num, 0f, 100f));
				for (int k = 0; k < array.Length; k++)
				{
					int num3 = 1 << array[k].collider.gameObject.layer;
					if (!array[k].collider.isTrigger && (num3 & mask) != 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				currentParamsIndex = i;
				break;
			}
		}
		if (currentParamsIndex != -1)
		{
			LastTimeActivated = Time.time;
			ActionRequested = true;
			StartInterpolation(new List<Vector3> { targetPosition }, currentParamsIndex);
		}
		else
		{
			ActionRequested = false;
		}
		return ActionRequested;
	}

	public void SignalNextTarget()
	{
		nextSignalCount++;
	}

	public void StopActionCamera()
	{
		ActionRequested = false;
		StopInterpolation();
	}

	private void LateUpdate()
	{
		if (state != ActionCameraState.Idle)
		{
			UpdateInterpolation();
		}
		if (state != ActionCameraState.AtTarget)
		{
			if (!IsFixInit)
			{
				if (!LoadingScreenCombat.Active)
				{
					IsFixInit = true;
					StartCoroutine( FixCameraSettings());
				}
			}
			return;
		}
		endDelayTimer += Time.deltaTime;
		if ((CurrentTargetCameraParams.endDelay >= 0f && endDelayTimer >= CurrentTargetCameraParams.endDelay) || (CurrentTargetCameraParams.endDelay < 0f && nextSignalCount > 0))
		{
			if (CurrentTargetCameraParams.endDelay < 0f && nextSignalCount > 0)
			{
				nextSignalCount--;
			}
			if (!NextTarget())
			{
				StopInterpolation();
			}
		}
	}


	#region mycode
	private bool IsFixInit = false;
	[ContextMenu("FixCameraSettings")]
	public void FixCameraSettingsDebug()
	{
		StartCoroutine(FixCameraSettings());
	}

	private IEnumerator FixCameraSettings()
	{
		//LayerM |= 1 << LayerMask.NameToLayer("Background_VFX");

		var amplyfy = GetComponent<AmplifyColorEffect>();
		amplyfy.enabled = false;

		yield return new WaitForSeconds(1);

		MyTools.ModifyPProfile(gameObject);
	}

	private int zoomingId = -1;
	private int scrollingId;
	private float prevScreenDistance;
	public bool Zoomed { get; private set; }
	[SerializeField]
	[Tooltip("How far away the camera looks at the GroundCollider.")]
	private float distance = 35;

	[SerializeField]
	[Tooltip("How close the camera can get to the GroundCollider.")]
	private float minDistance = 15;

	[SerializeField]
	[Tooltip("How far away the camera can get from the GroundCollider.")]
	private float maxDistance = 50;
	public float mult = .1f;

	public void Update()
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

	private void UpdateMouseGestures()
	{
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

	private void UpdateTouchGestures()
	{
		Touch touch = Input.GetTouch(0);

		if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			if (zoomingId != -1)
			{
				EndZooming(zoomingId != touch.fingerId);
			}
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
			if (scrollingId != touch2.fingerId)
			{
				EndZooming(switchTouch: false);
			}
		}
		if (zoomingId != -1)
		{
			UpdateZooming(touch.position, touch2.position);
		}
	}

	private void StartZooming(int id, Vector2 position0, Vector2 position1)
	{
		zoomingId = id;
		prevScreenDistance = (position1 - position0).magnitude;
		Zoomed = false;
	}

	private void EndZooming(bool switchTouch)
	{
		if (switchTouch)
		{
			scrollingId = zoomingId;
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
		distance += (prevScreenDistance - magnitude) * mult;
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
		prevScreenDistance = magnitude;
		CameraComponent.fieldOfView = distance;
	}
	#endregion
}
