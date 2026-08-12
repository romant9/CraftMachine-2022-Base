using System;
using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class CameraInputHandler : PlayerInputHandler
{
	private Vector3 cameraDragHitStartWorldCenterPosition;

	private Vector3 cameraTargetPosition;

	private static float cameraDistanceEpsilon = 0.001f;

	private CameraState currentState;

	private Vector3 cameraSoftLimitMin;

	private Vector3 cameraSoftLimitMax;

	private Vector3 cameraHardLimitMin;

	private Vector3 cameraHardLimitMax;

	private float visibleBottomRowLength;

	private Vector3 velocity;

	private float startZoomFactor;

	private float currentZoomFactor;

	private float referenceAreaHeight;

	private float cameraZMoveLimit;

	private float currentCameraElevationAngle;

	private float currentCameraSpeed;

	private bool draggingEnabled;

	private Plane dragArea = new Plane(new Vector3(0f, 1f, 0f), 0f);

	private ActorModel pendingActorToFocus;

	public override bool RequiresPlayerInputEnabled => false;

	public Camera Camera => Camera.main;

	public CombatCameraData CombatCameraData { get; private set; }

	public CombatCameraProfile CombatCameraProfile { get; private set; }

	public override int Priority => 0;

	public override bool ResetOtherHandlers => false;

	public bool DraggingEnabled
	{
		get
		{
			return draggingEnabled;
		}
		set
		{
			if (draggingEnabled != value)
			{
				if ((value && currentState != CameraState.MovingToTargetLocation) || !value)
				{
					SwitchState(CameraState.Idle);
				}
				draggingEnabled = value;
			}
		}
	}

	private Vector3 PlaneCenter => (PlaneMin + PlaneMax) * 0.5f;

	private Vector3 PlaneNormal => new Vector3(0f, 1f, 0f);

	private Vector3 PlaneMin
	{
		get
		{
			BoxCollider cameraOverrideBounds = GridView.Instance.CameraOverrideBounds;
			if (cameraOverrideBounds != null)
			{
				return cameraOverrideBounds.bounds.min;
			}
			Vector3 position = GridView.Instance.transform.position;
			Vector3 rhs = position + new Vector3((float)GridView.Instance.Model.Width * (float)GridView.Instance.Model.CellSize.X, 0f, (float)(-GridView.Instance.Model.Height) * (float)GridView.Instance.Model.CellSize.Y);
			return Vector3.Min(position, rhs);
		}
	}

	private Vector3 PlaneMax
	{
		get
		{
			BoxCollider cameraOverrideBounds = GridView.Instance.CameraOverrideBounds;
			if (cameraOverrideBounds != null)
			{
				return new Vector3(cameraOverrideBounds.bounds.max.x, cameraOverrideBounds.bounds.min.y, cameraOverrideBounds.bounds.max.z);
			}
			Vector3 position = GridView.Instance.transform.position;
			Vector3 rhs = position + new Vector3((float)GridView.Instance.Model.Width * (float)GridView.Instance.Model.CellSize.X, 0f, (float)(-GridView.Instance.Model.Height) * (float)GridView.Instance.Model.CellSize.Y);
			return Vector3.Max(position, rhs);
		}
	}

	public override bool CanHandleInteraction()
	{
		GridCoordinate gridCoordinateFromScreenPosition = PlayerInputManager.Instance.GetGridCoordinateFromScreenPosition(PlayerInputManager.Instance.MouseDragStart);
		ActorModel actorModel = (base.Grid.IsCoordinateValid(gridCoordinateFromScreenPosition) ? base.Combat.Occupiers[gridCoordinateFromScreenPosition] : null);
		bool flag = actorModel != null && actorModel.Faction == Faction.Survivor;
		if (PlayerInputManager.Instance.IsDragging)
		{
			return !flag;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		if (CombatCameraData != null)
		{
			currentCameraSpeed = CombatCameraData.CameraDragSpeed;
			SwitchState(CameraState.Dragging);
		}
	}

	public override void InteractionStopped()
	{
		if (!(Camera != null))
		{
			return;
		}
		float cameraSoftLimitRatio = GetCameraSoftLimitRatio(Camera.transform.position);
		if (cameraSoftLimitRatio >= 0f && cameraSoftLimitRatio <= 1f)
		{
			SwitchState(CameraState.Idle);
			return;
		}
		if (cameraSoftLimitRatio < 0f)
		{
			cameraTargetPosition = cameraSoftLimitMin;
		}
		else
		{
			cameraTargetPosition = cameraSoftLimitMax;
		}
		SwitchState(CameraState.MovingToTargetLocation);
	}

	public void DisableCameraDragIfOccurring()
	{
		SwitchState(CameraState.MovingToTargetLocation);
	}

	public void MoveCameraToGridWorldPosition(Vector3 target, float speed = -1f)
	{
		if (speed > -1f)
		{
			currentCameraSpeed = speed;
		}
		else
		{
			currentCameraSpeed = CombatCameraData.CameraMoveSpeed;
		}
		cameraTargetPosition = GetTargetPositionOnRail(target);
		SwitchState(CameraState.MovingToTargetLocation);
	}

	private Vector3 GetTargetPositionOnRail(Vector3 targetPosition, bool useSoftLimits = true)
	{
		Vector3 vector = (useSoftLimits ? cameraSoftLimitMin : cameraHardLimitMin);
		Vector3 vector2 = (useSoftLimits ? cameraSoftLimitMax : cameraHardLimitMax) - vector;
		Vector3 rhs = targetPosition - vector;
		float num = Vector3.Dot(vector2.normalized, rhs);
		float magnitude = vector2.magnitude;
		float num2 = Mathf.Clamp(num / magnitude, 0f, 1f);
		Vector3 result = vector + vector2 * num2;
		result.z = Mathf.Clamp(targetPosition.z, cameraHardLimitMin.z - cameraZMoveLimit, cameraHardLimitMin.z + cameraZMoveLimit);
		return result;
	}

	public void FrameActorToView(ActorModel actor, bool immediateMove = true)
	{
		if (!immediateMove)
		{
			pendingActorToFocus = actor;
			return;
		}
		if (cameraSoftLimitMin == Vector3.zero || cameraSoftLimitMax == Vector3.zero)
		{
			pendingActorToFocus = actor;
			return;
		}
		Vector3 position = Camera.transform.position;
		float num = position.x - visibleBottomRowLength * 0.2f;
		float num2 = position.x + visibleBottomRowLength * 0.2f;
		Vector3 vector = GridView.Instance.GetPosition(actor.GridCoordinate).ToVector3();
		Vector3 vector2 = position;
		if (vector.x < num)
		{
			float num3 = num - vector.x;
			vector2.x -= num3;
		}
		else if (vector.x > num2)
		{
			float num4 = vector.x - num2;
			vector2.x += num4;
		}
		if ((position - vector2).sqrMagnitude > 0.1f)
		{
			cameraTargetPosition = GetTargetPositionOnRail(vector2);
			SwitchState(CameraState.MovingToTargetLocation);
		}
	}

	public void MoveCameraToCentralPointOfActors(List<GameObject> actorsList)
	{
		if (actorsList.Count > 0)
		{
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			for (int i = 0; i < actorsList.Count; i++)
			{
				Vector3 position = actorsList[i].transform.position;
				vector.x = Mathf.Min(position.x, vector.x);
				vector.y = Mathf.Min(position.y, vector.y);
				vector.z = Mathf.Min(position.z, vector.z);
				vector2.x = Mathf.Max(position.x, vector2.x);
				vector2.y = Mathf.Max(position.y, vector2.y);
				vector2.z = Mathf.Max(position.z, vector2.z);
			}
			MoveCameraToGridWorldPosition(vector + (vector2 - vector) * 0.5f);
		}
	}

	private void SetupCamera()
	{
		if (Camera == null)
		{
			return;
		}
		Vector3 position = Camera.transform.position;
		CombatCameraData = UnityUtils.LoadFromAssetBundle<CombatCameraData>("CombatCameraData", "scriptableobjects");
		if (CombatCameraData != null)
		{
			CombatCameraProfile = CombatCameraData.GetCurrentProfile();
			if (CombatCameraProfile != null)
			{
				Camera.fieldOfView = CombatCameraData.FieldOfView;
				FramePlaneToCamera(Camera, CombatCameraProfile.ElevationAngle, PlaneMin, PlaneMax, PlaneNormal, new Margin(CombatCameraProfile.TopMargin, CombatCameraProfile.SideMargin, CombatCameraProfile.BottomMargin, CombatCameraProfile.SideMargin));
				SetCameraZoom(currentZoomFactor);
			}
		}
		Camera.transform.position = GetTargetPositionOnRail(position);
	}

	public void SetCameraZoom(float zoomAlpha)
	{
		Camera.fieldOfView = Mathf.Lerp(CombatCameraData.FieldOfView, CombatCameraData.FieldOfView * 0.5f, zoomAlpha);
		Vector3[] worldCorners = Camera.GetWorldCorners();
		Vector3 vector = worldCorners[0];
		Vector3 vector2 = worldCorners[3];
		Plane plane = new Plane(PlaneNormal, PlaneCenter);
		Vector3 vector3 = (worldCorners[1] + worldCorners[2]) * 0.5f;
		Vector3 vector4 = (worldCorners[0] + worldCorners[3]) * 0.5f;
		Ray ray = new Ray(Camera.transform.position, Vector3.Normalize(vector3 - Camera.transform.position));
		Ray ray2 = new Ray(Camera.transform.position, Vector3.Normalize(vector4 - Camera.transform.position));
		float enter = 0f;
		float enter2 = 0f;
		plane.Raycast(ray, out enter);
		plane.Raycast(ray2, out enter2);
		float num = Vector3.Distance(ray.GetPoint(enter), ray2.GetPoint(enter2));
		if (zoomAlpha == 0f)
		{
			referenceAreaHeight = num;
		}
		Ray ray3 = new Ray(Camera.transform.position, Vector3.Normalize(vector - Camera.transform.position));
		Ray ray4 = new Ray(Camera.transform.position, Vector3.Normalize(vector2 - Camera.transform.position));
		float enter3 = 0f;
		float enter4 = 0f;
		plane.Raycast(ray3, out enter3);
		plane.Raycast(ray4, out enter4);
		visibleBottomRowLength = Vector3.Distance(ray3.GetPoint(enter3), ray4.GetPoint(enter4));
		Vector3 vector5 = new Vector3(PlaneMin.x + visibleBottomRowLength * (0.5f - CombatCameraProfile.SideMargin), Camera.transform.position.y, Camera.transform.position.z);
		Vector3 vector6 = new Vector3(PlaneMax.x - visibleBottomRowLength * (0.5f - CombatCameraProfile.SideMargin), Camera.transform.position.y, Camera.transform.position.z);
		cameraZMoveLimit = Mathf.Abs(referenceAreaHeight - num) * 0.5f;
		cameraSoftLimitMin = vector5 + new Vector3(-8f, 0, 0);
		cameraSoftLimitMax = vector6 + new Vector3(8f,0,0);
		cameraHardLimitMin = vector5 + new Vector3(-8f, 0, 0);
		cameraHardLimitMax = vector6 + new Vector3(8f, 0, 0);
	}

	public override void Initialize()
	{
		DraggingEnabled = true;
		currentState = CameraState.Idle;
		SetupCamera();
		cameraTargetPosition = GetTargetPositionOnRail(Camera.transform.position);
		if (pendingActorToFocus != null)
		{
			FrameActorToView(pendingActorToFocus);
		}
		pendingActorToFocus = null;
		currentZoomFactor = 0f;
		startZoomFactor = -1f;
	}

	private void SwitchState(CameraState newState)
	{
		if (currentState == newState || Camera == null)
		{
			return;
		}
		switch (newState)
		{
		case CameraState.Dragging:
		{
			pendingActorToFocus = null;
			Ray ray = Camera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			if (dragArea.Raycast(ray, out var _))
			{
				cameraDragHitStartWorldCenterPosition = Camera.transform.position;
				break;
			}
			SwitchState(CameraState.Idle);
			return;
		}
		case CameraState.MovingToTargetLocation:
			currentCameraSpeed = CombatCameraData.CameraMoveSpeed;
			cameraTargetPosition = GetTargetPositionOnRail(cameraTargetPosition);
			break;
		}
		currentState = newState;
	}

	public override bool UpdateInteraction(float timeDelta)
	{
		Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
		Vector3 mouseDelta = base.PlayerInputManager.MouseDelta;
		float sqrMagnitude = mouseDelta.sqrMagnitude;
		if (currentState == CameraState.Dragging && sqrMagnitude > 0f && dragArea.Raycast(ray, out var enter))
		{
			ray = Camera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			dragArea.Raycast(ray, out enter);
			Vector3 point = ray.GetPoint(enter);
			float num = 1f / (Camera.WorldToScreenPoint(point + new Vector3(1f, 0f, 0f)).x - (float)Screen.width * 0.5f);
			float num2 = 1f / (Camera.WorldToScreenPoint(point + new Vector3(0f, 0f, 1f)).y - (float)Screen.height * 0.5f);
			velocity.x = (0f - mouseDelta.x) * num / timeDelta;
			velocity.z = (0f - mouseDelta.y) * num2 / timeDelta;
			Vector3 vector = new Vector3((0f - base.PlayerInputManager.MouseDragDelta.x) * num, 0f, (0f - base.PlayerInputManager.MouseDragDelta.y) * num2);
			cameraTargetPosition = GetTargetPositionOnRail(cameraDragHitStartWorldCenterPosition + vector, useSoftLimits: false);
			float num3 = CombatCameraProfile.ElevationAngle + CombatCameraData.ElevationAngleVariationSoft;
			float num4 = CombatCameraProfile.ElevationAngle - CombatCameraData.ElevationAngleVariationSoft;
			float num5 = mouseDelta.y * num2;
			float num6 = ((currentCameraElevationAngle < num3 && currentCameraElevationAngle > num4) ? 1f : CombatCameraData.SoftLimitsFriction);
			currentCameraElevationAngle += num5 * num6;
			currentCameraElevationAngle = Math.Max(num4 - CombatCameraData.ElevationAngleVariationHard, Math.Min(num3 + CombatCameraData.ElevationAngleVariationHard, currentCameraElevationAngle));
		}
		return DraggingEnabled;
	}

	public static void FramePlaneToCamera(Camera camera, float pitchInDegrees, Vector3 planeMin, Vector3 planeMax, Vector3 planeNormal, Margin relativeMargins)
	{
		float num = relativeMargins.top + relativeMargins.bottom;
		Vector3 vector = new Vector3(planeMax.x - planeMin.x, planeMax.y - planeMin.y, planeMax.z - planeMin.z);
		Vector3 vector2 = planeMin + vector * 0.5f;
		camera.transform.rotation = Quaternion.Euler(new Vector3(pitchInDegrees, 0f, 0f));
		float num2 = Vector3.Dot(camera.transform.forward, -planeNormal) * vector.z;
		float num3 = 1f / Mathf.Clamp(1f - num, 0.001f, 1f);
		float num4 = Mathf.Clamp(camera.fieldOfView / num3, 0.1f, 180f);
		float num5 = num2 * 0.5f / Mathf.Tan(num4 * 0.5f * (MathF.PI / 180f));
		Vector3 vector3 = -camera.transform.forward * num5;
		camera.transform.position = vector2 + vector3;
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
		Vector3 origin = new Vector3((planeMax.x - planeMin.x) * 0.5f + planeMin.x, planeMax.y, planeMax.z);
		Vector3 origin2 = new Vector3((planeMax.x - planeMin.x) * 0.5f + planeMin.x, planeMin.y, planeMin.z);
		float enter = 0f;
		array[3].Raycast(new Ray(origin, -camera.transform.up), out enter);
		float enter2 = 0f;
		array[2].Raycast(new Ray(origin2, camera.transform.up), out enter2);
		Vector3 vector4 = camera.transform.up * enter;
		Vector3 vector5 = -camera.transform.up * enter2;
		float num6 = ((num > 0f) ? (1f - relativeMargins.bottom / num) : 0.5f);
		float num7 = ((num > 0f) ? (1f - relativeMargins.top / num) : 0.5f);
		Vector3 vector6 = vector5 * num6 + vector4 * num7;
		camera.transform.position = camera.transform.position + vector6;
	}

	public override void Update(float timeDelta)
	{
		if (CombatCameraData == null)
		{
			return;
		}
		if (base.PlayerInputManager.PinchActive)
		{
			if (startZoomFactor < 0f)
			{
				startZoomFactor = currentZoomFactor;
			}
			currentZoomFactor = Mathf.Clamp(startZoomFactor + base.PlayerInputManager.PinchDelta * 0.005f, 0f, 1f);
			SetupCamera();
		}
		else
		{
			startZoomFactor = -1f;
		}
		if (currentZoomFactor > 0f && !base.PlayerInputManager.PinchActive && base.PlayerInputManager.IsDragging)
		{
			float num = currentZoomFactor;
			Vector3 vector = new Vector3(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height, 0f);
			if (vector.x < 0.1f && base.PlayerInputManager.MouseDragDelta.x < 0f)
			{
				currentZoomFactor -= timeDelta * 0.5f;
			}
			else if (vector.x > 0.9f && base.PlayerInputManager.MouseDragDelta.x > 0f)
			{
				currentZoomFactor -= timeDelta * 0.5f;
			}
			else if (vector.y < 0.1f && base.PlayerInputManager.MouseDragDelta.y < 0f)
			{
				currentZoomFactor -= timeDelta * 0.5f;
			}
			else if (vector.y > 0.9f && base.PlayerInputManager.MouseDragDelta.y > 0f)
			{
				currentZoomFactor -= timeDelta * 0.5f;
			}
			currentZoomFactor = Mathf.Max(0f, currentZoomFactor);
			if (num != currentZoomFactor)
			{
				SetupCamera();
			}
		}
		if (pendingActorToFocus != null && VisualizationQueue.Instance.TotalTaskCount < 1)
		{
			currentCameraSpeed = CombatCameraData.CameraMoveSpeed;
			FixedVec3 position = GridView.Instance.GetPosition(pendingActorToFocus.GridCoordinate);
			cameraTargetPosition = GetTargetPositionOnRail(position.ToVector3());
			SwitchState(CameraState.MovingToTargetLocation);
			pendingActorToFocus = null;
			return;
		}
		if (currentState != CameraState.Dragging)
		{
			cameraTargetPosition = GetTargetPositionOnRail(cameraTargetPosition + velocity * timeDelta);
			velocity = Vector3.Lerp(velocity, Vector3.zero, CombatCameraData.Damping * 60f * timeDelta);
			currentCameraElevationAngle = Mathf.LerpAngle(currentCameraElevationAngle, CombatCameraProfile.ElevationAngle, CombatCameraData.Damping * 60f * timeDelta);
		}
		if (currentState == CameraState.MovingToTargetLocation)
		{
			if (PlayerInputManager.Instance.MouseDrag)
			{
				SwitchState(CameraState.Idle);
				return;
			}
			if ((cameraTargetPosition - Camera.transform.position).sqrMagnitude < cameraDistanceEpsilon)
			{
				cameraTargetPosition = GetTargetPositionOnRail(cameraTargetPosition);
				Camera.transform.position = cameraTargetPosition;
				SwitchState(CameraState.Idle);
			}
		}
		InterpolateCameraTowardsTarget();
		if (ActionCamera.Instance != null && !ActionCamera.Instance.IsActive)
		{
			Camera.transform.rotation = Quaternion.Euler(new Vector3(currentCameraElevationAngle, 0f, 0f));
		}
	}

	private float GetCameraSoftLimitRatio(Vector3 target)
	{
		Vector3 vector = cameraSoftLimitMax - cameraSoftLimitMin;
		Vector3 rhs = target - cameraSoftLimitMin;
		float num = Vector3.Dot(vector.normalized, rhs);
		float magnitude = vector.magnitude;
		return num / magnitude;
	}

	private void InterpolateCameraTowardsTarget()
	{
		Vector3 vector = Camera.transform.position - cameraTargetPosition;
		if (Time.deltaTime > 0f && vector.sqrMagnitude > cameraDistanceEpsilon)
		{
			Camera.transform.position = Vector3.Lerp(Camera.transform.position, cameraTargetPosition, Mathf.Clamp(currentCameraSpeed * Time.deltaTime, 0f, 1f));
		}
	}

	public void FocusCameraOnTargetIfFarFromCenter(Vector3 worldPosition)
	{
		if (IsTargetFarFromCameraCenter(worldPosition))
		{
			MoveCameraToGridWorldPosition(worldPosition);
		}
	}

	public bool IsTargetFarFromCameraCenter(Vector3 worldPosition)
	{
		Vector2 vector = Camera.main.WorldToScreenPoint(worldPosition);
		float num = (float)Camera.main.pixelWidth * CombatCameraData.CameraFocusTriggerRatio;
		float num2 = (float)Camera.main.pixelWidth * (1f - CombatCameraData.CameraFocusTriggerRatio);
		if (!(vector.x > num2))
		{
			return vector.x < num;
		}
		return true;
	}
}
