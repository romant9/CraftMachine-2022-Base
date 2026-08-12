using System;
using System.Collections.Generic;
using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class GridView : ModelView<GridModel>
{
	private static GridView instance;

	public AbilityChangeHandler AbilityChanged;

	public Vector2Data ConfiguredCellSize;

	public int ConfiguredWidth;

	public int ConfiguredHeight;

	public BoxCollider CameraOverrideBounds;

	[SerializeField]
	[Tooltip("Grid background color.")]
	private Color gridBackground = new Color(1f, 1f, 1f, 0.25f);

	[SerializeField]
	[Tooltip("Grid blocked background color.")]
	private Color gridBlockedBackground = new Color(0f, 0f, 0f, 0.25f);

	private bool gridHighlightsInitialized;

	private int width;

	private int height;

	private FixedVec2 cellSize;

	private CombatModel combat;

	private BoxCollider gridCollider;

	private bool cameraInitialized;

	public GameObject GridHighLightPrefab;

	private MeshRenderer GridRenderer;

	private GameObject currentAbilitySelectionVisualization;

	private List<GridTargetHighlight> abilityHighlights = new List<GridTargetHighlight>();

	private AbilityActorTuple selectedAbilityToDisplayTargetCells;

	private GameObject abilityActivationContainer;

	public static GridView Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<GridView>();
				if (instance == null && DebugTWD.IsDebugBuild)
				{
					Debug.LogWarning("Performance warning: GridView instance not found");
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
			if (Instance != null)
			{
				DebugTWD.LogError("GridView Instance is not null. Returrn");
				return;
			}
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}
	public static GridView ActiveInstance => UnityEngine.Object.FindObjectOfType<GridView>();

	public FixedVec3 Position { get; private set; }

	public BoxCollider GridCollider => gridCollider;

	public GridModel Grid => base.Model;

	public AbilityActorTuple SelectedAbilityToDisplayTargetCells
	{
		get
		{
			return selectedAbilityToDisplayTargetCells;
		}
		set
		{
			if (value != selectedAbilityToDisplayTargetCells)
			{
				AbilityChangeHandler abilityChanged = AbilityChanged;
				if (abilityChanged != null)
				{
					if (value != null)
					{
						abilityChanged(value.Ability, value.Actor);
					}
					else
					{
						abilityChanged(null, null);
					}
				}
				selectedAbilityToDisplayTargetCells = value;
			}
			if (selectedAbilityToDisplayTargetCells == null)
			{
				CombatView.Instance.CombatHUD.DeselectAbilityButtons();
			}
		}
	}

	private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;

	public bool IsValidCoordinate(GridCoordinate c)
	{
		if (c.X >= 0 && c.Y >= 0 && c.X < ConfiguredWidth)
		{
			return c.Y < ConfiguredHeight;
		}
		return false;
	}

	private void Start()
	{
		base.gameObject.FindInChildren("GridVisualization")?.GetComponent<CombatGridVisualization>()?.UpdateVisibility();
	}

	public void OnDestroy()
	{
		instance = null;
		if (PlayerInputManager != null)
		{
			PlayerInputManager playerInputManager = PlayerInputManager;
			playerInputManager.ActorSelectionChanged = (ActorSelectionChangedHandler)Delegate.Remove(playerInputManager.ActorSelectionChanged, new ActorSelectionChangedHandler(OnActorSelectionChanged));
			PlayerInputManager playerInputManager2 = PlayerInputManager;
			playerInputManager2.UserInteractionStarted = (UserInteractionStartedHandler)Delegate.Remove(playerInputManager2.UserInteractionStarted, new UserInteractionStartedHandler(OnUserInteractionStarted));
			PlayerInputManager playerInputManager3 = PlayerInputManager;
			playerInputManager3.UserInteractionStopped = (UserInteractionStoppedHandler)Delegate.Remove(playerInputManager3.UserInteractionStopped, new UserInteractionStoppedHandler(OnUserInteractionStopped));
		}
		cameraInitialized = false;
	}

	public FixedVec3 GetPosition(GridCoordinate coordinate)
	{
		if (Grid == null) return new FixedVec3(0, 0, 0);

		return Grid.GetPosition(coordinate);
	}

	public void GetConfiguredCellOrEdge(Vector3 position, out GridCoordinate outCoordinate, out GridCoordinate edgeNeighborCoordinate, bool allowCell = true, bool allowEdge = true)
	{
		Vector3 vector = position - base.transform.position;
		float num = vector.x / ConfiguredCellSize.X;
		float num2 = (0f - vector.z) / ConfiguredCellSize.Y;
		int num3 = (int)num;
		int num4 = (int)num2;
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		if (!allowEdge || (num5 > 0.25f && num6 > 0.25f && num5 < 0.75f && num6 < 0.75f && allowCell))
		{
			outCoordinate = new GridCoordinate(num3, num4);
			edgeNeighborCoordinate = GridCoordinate.Invalid;
		}
		else
		{
			GridCoordinate gridCoordinate = ((num6 < 0.25f && num5 > num6 && num5 < 1f - num6) ? new GridCoordinate(num3, num4 - 1) : ((num6 > 0.75f && num5 > 1f - num6 && num5 < num6) ? new GridCoordinate(num3, num4 + 1) : ((!(num5 < 0.5f)) ? new GridCoordinate(num3 + 1, num4) : new GridCoordinate(num3 - 1, num4))));
			outCoordinate = new GridCoordinate(num3, num4);
			edgeNeighborCoordinate = gridCoordinate;
		}
	}

	public void GetConfiguredCoordinates(int edge, out GridCoordinate c1, out GridCoordinate c2)
	{
		GridModel.GetCoordinatesFromEdge(edge, out c1, out c2, ConfiguredWidth);
	}

	public GridCoordinate GetConfiguredCoordinate(Vector3 position)
	{
		Vector3 vector = position - base.transform.position;
		Vector2 vector2 = new Vector2(vector.x / ConfiguredCellSize.X, (0f - vector.z) / ConfiguredCellSize.Y);
		return new GridCoordinate((int)vector2.x, (int)vector2.y);
	}

	public Vector2 GetConfiguredCellOffset(Vector3 position)
	{
		Vector3 vector = position - base.transform.position;
		return new Vector2(vector.x % ConfiguredCellSize.X / ConfiguredCellSize.X, (0f - vector.z) % ConfiguredCellSize.Y / ConfiguredCellSize.Y);
	}

	public Vector3 GetConfiguredPosition(GridCoordinate coordinate)
	{
		float x = ConfiguredCellSize.X;
		float y = ConfiguredCellSize.Y;
		return new Vector3((float)coordinate.X * x + x * 0.5f, 0f, (float)(-coordinate.Y) * y - y * 0.5f) + base.transform.position;
	}

	public int GetConfiguredOffset(GridCoordinate coordinate)
	{
		return coordinate.Y * ConfiguredWidth + coordinate.X;
	}

	public override void Initialize(ModelObject model)
	{
		DebugTWD.Log("GridView Initialized!", DebugType.System);

		base.Initialize(model);
		if (!OfflineManager.IsLoadDataManager)
		{
			if (GameManager.Instance == null)
			{
				if (GameManager.Instance != null)
				{
					GameManager.Instance.GoToLoaderScene();
				}
				return;
			}
		}
		else
		{
			if (GameManager.Instance?.playerModel?.Combat == null)
			{
				return;
			}
		}

		Position = new FixedVec3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		combat = GameManager.Instance.playerModel.Combat;
		if (combat == null)
		{
			Debug.LogError("Could not get CombatModel in GridView!");
		}
		GridRenderer = GetComponent<MeshRenderer>();
		cellSize = Grid.CellSize;
		width = Grid.Width;
		height = Grid.Height;
		gridCollider = GetComponent<BoxCollider>();
		gridCollider.size = new Vector3((float)width * (float)cellSize.X * 2f, 0.1f, (float)height * (float)cellSize.Y * 2f);
		gridCollider.center = new Vector3(gridCollider.size.x * 0.5f, -0.05f, (0f - gridCollider.size.z) * 0.5f);
		abilityActivationContainer = new GameObject("AbilityActivationContainer");
		abilityActivationContainer.transform.parent = base.transform;
		SelectedAbilityToDisplayTargetCells = null;
		GetComponentInChildren<CombatGridAreaVisualization>().Initialize();
		if (PlayerInputManager != null)
		{
			PlayerInputManager playerInputManager = PlayerInputManager;
			playerInputManager.ActorSelectionChanged = (ActorSelectionChangedHandler)Delegate.Combine(playerInputManager.ActorSelectionChanged, new ActorSelectionChangedHandler(OnActorSelectionChanged));
			PlayerInputManager playerInputManager2 = PlayerInputManager;
			playerInputManager2.UserInteractionStarted = (UserInteractionStartedHandler)Delegate.Combine(playerInputManager2.UserInteractionStarted, new UserInteractionStartedHandler(OnUserInteractionStarted));
			PlayerInputManager playerInputManager3 = PlayerInputManager;
			playerInputManager3.UserInteractionStopped = (UserInteractionStoppedHandler)Delegate.Combine(playerInputManager3.UserInteractionStopped, new UserInteractionStoppedHandler(OnUserInteractionStopped));
		}
		cameraInitialized = false;
	}

	private void OnActorSelectionChanged(ActorModel actorSelected)
	{
		CombatView.Instance.ShowHUDForSelectedActor(actorSelected);
	}

	private void OnUserInteractionStarted()
	{
	}

	private void OnUserInteractionStopped()
	{
		PlayerInputManager.Instance.GetHandler<CameraInputHandler>().DraggingEnabled = true;
	}

	private void OnEnable()
	{
		if (VisualizationQueue.Instance != null)
		{
			VisualizationQueue.Instance.VisualizationTaskCompleted += OnVisualizationTaskCompleted;
		}
	}

	private void OnDisable()
	{
		if (VisualizationQueue.Instance != null)
		{
			VisualizationQueue.Instance.VisualizationTaskCompleted -= OnVisualizationTaskCompleted;
		}
	}

	private void OnVisualizationTaskCompleted(VisualizationTask completedTask)
	{
	}

	private void Update()
	{
		if (base.IsInitialized)
		{
			if (base.Model != null && GridRenderer != null)
			{
				UpdateGridMesh();
			}
			if (SingularityMonoBehaviour<ObjectPoolManager>.Instance != null && !gridHighlightsInitialized)
			{
				SingularityMonoBehaviour<ObjectPoolManager>.Instance.SetupCacheForObject(GridHighLightPrefab, 5);
				gridHighlightsInitialized = true;
			}
			if (!cameraInitialized && combat != null && combat.ActiveActor != null)
			{
				cameraInitialized = true;
				OnActorSelectionChanged(combat.ActiveActor);
			}
		}
	}

	public void HighlightCoordinates(List<GridCoordinate> coordinates, List<Color> colors, List<int> indices)
	{
		ClearHighlights();
		for (int i = 0; i < coordinates.Count; i++)
		{
			GridTargetHighlight gridTargetHighlight = AddHighlight(coordinates[i], colors[i], indices[i]);
			if (gridTargetHighlight != null)
			{
				abilityHighlights.Add(gridTargetHighlight);
			}
		}
	}

	public GridTargetHighlight AddHighlight(GridCoordinate coordinate, Color color, int index)
	{
		Vector3 position = GetPosition(coordinate).ToVector3();
		GameObject gameObject = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(GridHighLightPrefab);
		if (gameObject == null)
		{
			Debug.LogError("Could not find highlight object");
			return null;
		}
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = position;
		GridTargetHighlight component = gameObject.GetComponent<GridTargetHighlight>();
		component.SetIndicatorColor(color);
		component.SetIndicatorIndex(index);
		return component;
	}

	public void ClearHighlights()
	{
		foreach (GridTargetHighlight abilityHighlight in abilityHighlights)
		{
			abilityHighlight.gameObject.GetComponent<CacheableObject>().Destroy();
		}
		abilityHighlights.Clear();
	}

	public bool HasHighlight(GridCoordinate c)
	{
		Vector3 vector = GetPosition(c).ToVector3();
		float num = 0.1f;
		foreach (GridTargetHighlight abilityHighlight in abilityHighlights)
		{
			if ((abilityHighlight.transform.position - vector).sqrMagnitude < num)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateGridMesh()
	{
		Vector3[] array = new Vector3[width * height * 4];
		Vector3[] array2 = new Vector3[width * height * 4];
		Vector2[] array3 = new Vector2[width * height * 4];
		Color[] array4 = new Color[width * height * 4];
		int[] array5 = new int[width * height * 2 * 3];
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Color color = gridBackground;
				GridCoordinate target = new GridCoordinate(j, i);
				if (combat.IsBlocked(target))
				{
					color = gridBlockedBackground;
				}
				Vector3 vector = new Vector3((float)j * (float)cellSize.X + (float)cellSize.X * 0.5f, 0f, (float)(-i) * (float)cellSize.Y - (float)cellSize.Y * 0.5f);
				Vector2 vector2 = (cellSize * 0.949999988079071 * 0.5).ToVector2();
				array[num] = vector + new Vector3(0f - vector2.x, 0f, 0f - vector2.y);
				array[num + 1] = vector + new Vector3(vector2.x, 0f, 0f - vector2.y);
				array[num + 2] = vector + new Vector3(vector2.x, 0f, vector2.y);
				array[num + 3] = vector + new Vector3(0f - vector2.x, 0f, vector2.y);
				array2[num] = new Vector3(0f, 1f, 0f);
				array2[num + 1] = new Vector3(0f, 1f, 0f);
				array2[num + 2] = new Vector3(0f, 1f, 0f);
				array2[num + 3] = new Vector3(0f, 1f, 0f);
				array4[num] = color;
				array4[num + 1] = color;
				array4[num + 2] = color;
				array4[num + 3] = color;
				array3[num] = new Vector2(0f, 0f);
				array3[num + 1] = new Vector2(1f, 0f);
				array3[num + 2] = new Vector2(1f, 1f);
				array3[num + 3] = new Vector2(0f, 1f);
				array5[num2] = num;
				array5[num2 + 1] = num + 2;
				array5[num2 + 2] = num + 1;
				array5[num2 + 3] = num;
				array5[num2 + 4] = num + 3;
				array5[num2 + 5] = num + 2;
				num += 4;
				num2 += 6;
			}
		}
		if (GetComponent<MeshFilter>() == null)
		{
			base.gameObject.AddComponent<MeshFilter>();
		}
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = array;
		mesh.colors = array4;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array5;
	}
}
