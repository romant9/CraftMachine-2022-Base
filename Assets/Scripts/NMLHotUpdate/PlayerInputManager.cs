using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	private static PlayerInputManager instance;

	private GridField<CellStatus> validTargets;

	[Tooltip("How many pixels to move to begin dragging")]
	public float DragThresholdInPixels = 15f;

	[HideInInspector]
	public GridCoordinate StartDragCoordinate = GridCoordinate.Invalid;

	[HideInInspector]
	public Vector3 PreviousMousePosition;

	[HideInInspector]
	public Vector3 MouseDelta;

	[HideInInspector]
	public bool PinchActive;

	[HideInInspector]
	public float PinchDelta;

	private float MousePinchStartDelta;

	private bool MousePinchStartSet;

	[HideInInspector]
	public Vector3 MouseDragStart;

	[HideInInspector]
	public Vector3 MouseDragDelta;

	[HideInInspector]
	public bool MouseDrag;

	private List<PlayerInputHandler> inputHandlers;

	private PlayerInputHandler activeInputHandler;

	private List<Vector2> MouseDragHistory = new List<Vector2>();

	private int MouseDragHistorySize = 60;

	private bool abTestingEnabledForActorSelection;

	public ValidTargetsChangedHandler ValidTargetsChanged;

	public ActorSelectionChangedHandler ActorSelectionChanged;

	public UserInteractionStartedHandler UserInteractionStarted;

	public UserInteractionStoppedHandler UserInteractionStopped;

	private ActorModel controlledActor;

	public static PlayerInputManager Instance => instance;

	public bool IsEnabled { get; set; }

	public bool IsAbilityInteracting { get; set; }

	public bool PlayerSelectionEnabled { get; set; }

	public float MouseDownTime { get; private set; }

	public bool IsReconnecting { get; set; }

	public int CurrentTapIndex { get; private set; }

	private GridView GridView => GridView.Instance;

	private GridModel Grid => GridView.Grid;

	private CombatModel Combat
	{
		get
		{
			if (!GameManager.IsInitialized)
			{
				return null;
			}
			return GameManager.Instance.playerModel.Combat;
		}
	}

	private BoxCollider GridCollider => GridView.GridCollider;

	public ActorModel ControlledActor
	{
		get
		{
			return controlledActor;
		}
		private set
		{
			if (controlledActor != value)
			{
				if (controlledActor != null)
				{
					controlledActor.Changed -= OnControlledActorChanged;
				}
				controlledActor = value;
				if (controlledActor != null)
				{
					controlledActor.Changed += OnControlledActorChanged;
				}
			}
		}
	}

	public bool IsDragging
	{
		get
		{
			if (MouseDrag)
			{
				return MouseDragDelta.magnitude >= DragThresholdInPixels;
			}
			return false;
		}
	}

	public bool HasBeenDragged { get; private set; }

	public bool IsButtonDown
	{
		get
		{
			if (!UICamera.isOverUI)
			{
				return Input.GetMouseButton(0);
			}
			return false;
		}
	}

	public PlayerInputManager()
	{
		IsEnabled = true;
		PlayerSelectionEnabled = true;
		inputHandlers = new List<PlayerInputHandler>();
		inputHandlers.Add(new AbilityInputHandler());
		inputHandlers.Add(new AbilityTargetActorsInputHandler());
		inputHandlers.Add(new AbilityTargetGridInputHandler());
		inputHandlers.Add(new ActorMoveInputHandler());
		inputHandlers.Add(new ActorSelectionInputHandler());
		inputHandlers.Add(new CameraInputHandler());
		inputHandlers.Add(new MagazineAreaInputHandler());
		inputHandlers.Add(new DelayedActionGrenadeAreaInputHandler());
		inputHandlers.Add(new ObjectInfoInputHandler());
		inputHandlers.Add(new SupportInputHandler());
		inputHandlers.Add(new CommandSkillGridSelectInputHandler());
		inputHandlers.Add(new ActorSelectionSkillSelectInputHandler());
		inputHandlers.Sort();
	}

	public void Awake()
	{
		instance = this;
		if (GameManager.Instance != null && GameManager.Instance.gameEconomyData != null)
		{
			Feature feature = GameManager.Instance.gameEconomyData.GetFeature("NML_4988_MovingSurvivorsAB");
			if (feature != null)
			{
				abTestingEnabledForActorSelection = feature.Enabled;
			}
		}
	}

	private void OnActorChanged(ActorModel actor)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		ControlledActor = actor;
		foreach (PlayerInputHandler inputHandler in inputHandlers)
		{
			inputHandler.OnControlledActorChanged(actor);
		}
		if (Mathf.RoundToInt((Time.realtimeSinceStartup - realtimeSinceStartup) * 1000f) > 10)
		{
			_ = Application.isEditor;
		}
	}

	private void OnControlledActorChanged(ModelObject model, string changed, object args)
	{
		foreach (PlayerInputHandler inputHandler in inputHandlers)
		{
			inputHandler.OnControlledActorPropertiesChanged(changed, args);
		}
	}

	public GridCoordinate GetGridCoordinateFromScreenPosition(Vector3 screenPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		GridCoordinate result = GridCoordinate.Invalid;
		if (GridCollider != null && GridCollider.Raycast(ray, out var hitInfo, 100f))
		{
			FixedVec3 position = new FixedVec3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
			result = Grid.GetCoordinate(position);
		}
		return result;
	}

	public GridCoordinate GetPreviousDragCoordinate(float secondsAgo)
	{
		if (MouseDragHistory != null && MouseDragHistory.Count > 0)
		{
			int num = Mathf.RoundToInt(secondsAgo * (float)Application.targetFrameRate);
			int index = Math.Max(MouseDragHistory.Count - 1 - num, 0);
			return GetGridCoordinateFromScreenPosition(MouseDragHistory[index]);
		}
		return GridCoordinate.Invalid;
	}

	public GridCoordinate GetMouseGridCoordinate()
	{
		return GetGridCoordinateFromScreenPosition(Input.mousePosition);
	}

	public ActorModel GetSurvivorAtMouseCoordinate()
	{
		GridCoordinate mouseGridCoordinate = GetMouseGridCoordinate();
		ActorModel actorModel = (Grid.IsCoordinateValid(mouseGridCoordinate) ? Combat.Occupiers[mouseGridCoordinate] : null);
		if (actorModel != null && actorModel.Faction == Faction.Survivor)
		{
			return actorModel;
		}
		if (abTestingEnabledForActorSelection)
		{
			RaycastHit[] array = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100f);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].collider.GetComponent<ActorView>() != null)
				{
					FixedVec3 position = new FixedVec3(array[i].point.x, array[i].point.y, array[i].point.z);
					mouseGridCoordinate = Grid.GetCoordinate(position);
					actorModel = Combat.Occupiers[mouseGridCoordinate];
					if (actorModel != null && actorModel.Faction == Faction.Survivor && Grid.IsCoordinateValid(mouseGridCoordinate))
					{
						return actorModel;
					}
				}
			}
		}
		return null;
	}

	public ActorModel GetActorAtMouseCoordinate()
	{
		GridCoordinate mouseGridCoordinate = GetMouseGridCoordinate();
		if (!Grid.IsCoordinateValid(mouseGridCoordinate))
		{
			return null;
		}
		return Combat.Occupiers[mouseGridCoordinate];
	}

	public InteractiveObjectModel GetInteractiveObjectAtMouseCoordinate()
	{
		GridCoordinate mouseGridCoordinate = GetMouseGridCoordinate();
		foreach (InteractiveObjectModel model in Combat.GetModels<InteractiveObjectModel>())
		{
			if (model == null || model.Completed || model.Disabled || model.HasInteractionStarted)
			{
				continue;
			}
			if (model.Placement == Placement.Cell)
			{
				foreach (GridCoordinate coordinate in model.Location.Coordinates)
				{
					if (coordinate == mouseGridCoordinate)
					{
						return model;
					}
				}
				continue;
			}
			foreach (int edge in model.Location.Edges)
			{
				Grid.GetCoordinatesFromEdge(edge, out var a, out var b);
				if (a == mouseGridCoordinate || b == mouseGridCoordinate)
				{
					return model;
				}
			}
		}
		return null;
	}

	public MagazineArea GetMagazineAreaAtMouseCoordinate()
	{
		GridCoordinate mouseGridCoordinate = GetMouseGridCoordinate();
		MagazineAreasManager model = Combat.GetModel<MagazineAreasManager>();
		if (model?.ExistedMagazineAreas == null)
		{
			return null;
		}
		foreach (MagazineArea existedMagazineArea in model.ExistedMagazineAreas)
		{
			if (existedMagazineArea != null && existedMagazineArea.EffectiveAreaGridCoordinate == mouseGridCoordinate)
			{
				return existedMagazineArea;
			}
		}
		return null;
	}

	public DelayedActionGrenadeArea GetDelayedActionGrenadeAreaAtMouseCoordinate()
	{
		GridCoordinate mouseGridCoordinate = GetMouseGridCoordinate();
		if (!mouseGridCoordinate.IsValid || Combat == null)
		{
			return null;
		}
		foreach (DelayedActionGrenadeArea item in Combat.Models.OfType<DelayedActionGrenadeArea>())
		{
			if (item != null && item.EffectiveAreaGridCoordinate == mouseGridCoordinate)
			{
				return item;
			}
		}
		return null;
	}

	public CombatExitModel GetExitLocationAtMouse()
	{
		GridCoordinate mouseGridCoordinate = GetMouseGridCoordinate();
		foreach (CombatExitModel model in Combat.GetModels<CombatExitModel>())
		{
			for (int i = 0; i < model.GridCoordinates.Count; i++)
			{
				if (mouseGridCoordinate == model.GridCoordinates[i])
				{
					return model;
				}
			}
		}
		return null;
	}

	public void SetValidTargets(GridField<CellStatus> inValidTargets)
	{
		validTargets = inValidTargets;
		NotifyValidTargetsChanged();
	}

	private void NotifyValidTargetsChanged()
	{
		ValidTargetsChanged?.Invoke(validTargets);
	}

	private void NotifyActorSelected(ActorModel actorSelected)
	{
		ActorSelectionChanged?.Invoke(actorSelected);
	}

	private void NotifyUserInteractionStarted()
	{
		UserInteractionStarted?.Invoke();
	}

	private void NotifyUserInteractionStopped()
	{
		UserInteractionStopped?.Invoke();
	}

	private void Start()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnLoadCompleted += OnLoadCompleted;
		}
		PreviousMousePosition = new Vector3(0f, 0f, 0f);
		MouseDelta = new Vector3(0f, 0f, 0f);
		MouseDragStart = new Vector3(0f, 0f, 0f);
		MouseDragDelta = new Vector3(0f, 0f, 0f);
		MouseDrag = false;
		PinchActive = false;
		PinchDelta = 0f;
		MousePinchStartDelta = 0f;
		MousePinchStartSet = false;
		validTargets = null;
		IsAbilityInteracting = false;
	}

	private void OnDestroy()
	{
		instance = null;
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnLoadCompleted -= OnLoadCompleted;
		}
		if (Combat != null && Combat.TurnManager != null)
		{
			Combat.TurnManager.ActorChanged -= OnActorChanged;
		}
		SetActiveInputHandler(null);
		ControlledActor = null;
		inputHandlers = null;
	}

	private void OnLoadCompleted()
	{
		foreach (PlayerInputHandler inputHandler in inputHandlers)
		{
			inputHandler.Initialize();
		}
		Combat.TurnManager.ActorChanged += OnActorChanged;
		OnActorChanged(Combat.TurnManager.ActiveActor);
	}

	private void UpdateDrag()
	{
		if (!UICamera.isOverUI && Input.GetMouseButtonDown(0))
		{
			MouseDragStart = Input.mousePosition;
			StartDragCoordinate = GetMouseGridCoordinate();
			MouseDrag = true;
			HasBeenDragged = false;
			MouseDragHistory.Clear();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			MouseDrag = false;
			HasBeenDragged = false;
			MouseDragDelta = new Vector3(0f, 0f, 0f);
		}
		if (MouseDrag)
		{
			MouseDragDelta = Input.mousePosition - MouseDragStart;
			if (IsDragging)
			{
				HasBeenDragged = true;
			}
			if (MouseDragHistory.Count >= MouseDragHistorySize)
			{
				MouseDragHistory.RemoveAt(0);
			}
			MouseDragHistory.Add(Input.mousePosition);
		}
	}

	private void SetActiveInputHandler(PlayerInputHandler inputHandler)
	{
		if (activeInputHandler != null)
		{
			activeInputHandler.InteractionStopped();
		}
		activeInputHandler = inputHandler;
		if (activeInputHandler == null)
		{
			return;
		}
		if (activeInputHandler.ResetOtherHandlers)
		{
			foreach (PlayerInputHandler inputHandler2 in inputHandlers)
			{
				if (inputHandler2 != activeInputHandler)
				{
					inputHandler2.Reset();
				}
			}
		}
		activeInputHandler.InteractionStarted();
		activeInputHandler.ProcessedTapIndex = CurrentTapIndex;
	}

	public T GetHandler<T>() where T : PlayerInputHandler
	{
		foreach (PlayerInputHandler inputHandler in inputHandlers)
		{
			if (inputHandler as T != null)
			{
				return inputHandler as T;
			}
		}
		return null;
	}

	private void UpdatePinch()
	{
		PinchActive = false;
		if (Input.touchCount == 2)
		{
			float num = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			if (!MousePinchStartSet)
			{
				MousePinchStartSet = true;
				MousePinchStartDelta = num;
			}
			PinchDelta = (num - MousePinchStartDelta) * 0.5f;
			PinchActive = true;
		}
		else
		{
			MousePinchStartSet = false;
			MousePinchStartDelta = 0f;
		}
	}

	public void Stop()
	{
		IsEnabled = false;
		MouseDragHistory.Clear();
		foreach (PlayerInputHandler inputHandler in inputHandlers)
		{
			inputHandler.Reset();
		}
	}

	private void Update()
	{
		if (!IsEnabled || Combat == null || Grid == null || (Combat != null && !Combat.MissionStarted))
		{
			return;
		}
		MouseDelta = Input.mousePosition - PreviousMousePosition;
		UpdateDrag();
		UpdatePinch();
		if (Input.GetMouseButton(0))
		{
			MouseDownTime += Time.deltaTime;
		}
		else
		{
			MouseDownTime = 0f;
		}
		if (Input.GetMouseButtonUp(0))
		{
			SetActiveInputHandler(null);
		}
		if (!UICamera.isOverUI)
		{
			if (Input.GetMouseButtonDown(0))
			{
				CurrentTapIndex++;
				if (ActionCamera.Instance != null && ActionCamera.Instance.IsActive)
				{
					ActionCamera.Instance.StopActionCamera();
				}
			}
			if ((!(ActionCamera.Instance != null) || !ActionCamera.Instance.IsActive) && ((Input.GetMouseButton(0) && activeInputHandler == null) || Input.GetMouseButtonDown(0)))
			{
				foreach (PlayerInputHandler inputHandler in inputHandlers)
				{
					_ = inputHandler is ActorSelectionSkillSelectInputHandler;
					if ((!inputHandler.RequiresPlayerInputEnabled || CombatView.Instance.IsPlayerInputEnabled) && (!inputHandler.TapOnly || (inputHandler.ProcessedTapIndex != CurrentTapIndex && Input.GetMouseButtonDown(0))) && inputHandler.CanHandleInteraction())
					{
						SetActiveInputHandler(inputHandler);
						if (!inputHandler.ClickThrough)
						{
							break;
						}
					}
				}
			}
		}
		if (activeInputHandler != null && (!activeInputHandler.RequiresPlayerInputEnabled || CombatView.Instance.IsPlayerInputEnabled))
		{
			if (activeInputHandler.TapOnly)
			{
				if (IsDragging)
				{
					SetActiveInputHandler(null);
				}
			}
			else if (!activeInputHandler.UpdateInteraction(Time.deltaTime))
			{
				SetActiveInputHandler(null);
			}
		}
		foreach (PlayerInputHandler inputHandler2 in inputHandlers)
		{
			inputHandler2.Update(Time.deltaTime);
		}
		PreviousMousePosition = Input.mousePosition;
	}

	public void ResetAllHandlers()
	{
		for (int i = 0; i < inputHandlers.Count; i++)
		{
			inputHandlers[i].Reset();
		}
	}
}
