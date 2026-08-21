using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Constants;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class CombatGridAreaVisualization : GridAreaVisualization
{
	public GameObject DrawnPath;

	public GameObject DrawnPathEnd;

	public Material DrawnPathEndMaterial;

	public Material DrawnPathEndMaterialBlocked;

	public float DrawnPathThickness = 0.5f;

	public Color AttackPathColor;

	public Color UsePathColor;

	public Color NormalPathColor;

	public Color ExtendedPathColor;

	public Color DangerPathColor;

	public GameObject ExtendedShapeFill;

	public GameObject ExtendedShapeOutline;

	public ActionGridMarker ActionGridMarker;

	private InteractiveObjectModel currentInteractiveObject;

	[Header("Prefabs for grid corner visualisation")]
	[SerializeField]
	private GameObject northEastCornerVisualization;

	[SerializeField]
	private GameObject northWestCornerVisualization;

	[SerializeField]
	private GameObject southEastCornerVisualization;

	[SerializeField]
	private GameObject southWestCornerVisualization;

	[SerializeField]
	private GameObject northEastCornerAllowedVisualization;

	[SerializeField]
	private GameObject northWestCornerAllowedVisualization;

	[SerializeField]
	private GameObject southEastCornerAllowedVisualization;

	[SerializeField]
	private GameObject southWestCornerAllowedVisualization;

	[SerializeField]
	private GameObject cornerVisualizationParent;

	private HashSet<GridCoordinate> coordinatesToHandle = new HashSet<GridCoordinate>();

	private HashSet<GridCoordinate> blockedCoordinates = new HashSet<GridCoordinate>();

	private HashSet<GridCoordinate> blockedByWalls = new HashSet<GridCoordinate>();

	private HashSet<GridCoordinate> blockedDiagonallyCoordinates = new HashSet<GridCoordinate>();

	private Dictionary<Vector3, GameObject> blockedCorners = new Dictionary<Vector3, GameObject>();

	private Dictionary<Vector3, GameObject> allowedCorners = new Dictionary<Vector3, GameObject>();

	private HashSet<GridCoordinate> disabledInteractables = new HashSet<GridCoordinate>();

	private static List<Color> onDrawnPathChangedColors = new List<Color>();

	private static List<Vector3> onDrawnPathChangedVertices = new List<Vector3>();

	private static List<Vector2> onDrawnPathChangedUvs = new List<Vector2>();

	private static List<int> onDrawnPathChangedTriangles = new List<int>();

	private void Awake()
	{
		UpdateCellEdgeVisibility();
		GameManager.Instance.Blackboard.BlackboardChanged += BlackboardChangedHandler;
		GameManager.Instance.modelManager.CombatModel.Changed += HandleCombatModelChanged;
	}

	private void BlackboardChangedHandler(BlackboardEntryType changedType, string keyChanged)
	{
		if (changedType == BlackboardEntryType.Toggle && !(keyChanged != "Toggle.ToggleCombatGridEnabled"))
		{
			UpdateCellEdgeVisibility();
		}
	}

	private void Start()
	{
		base.transform.localPosition = -base.transform.parent.position;
	}

	protected override void ClearAreaVisualization()
	{
		base.ClearAreaVisualization();
		Mesh mesh = ExtendedShapeOutline.GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			mesh.Clear();
			mesh.RecalculateBounds();
		}
		mesh = ExtendedShapeFill.GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			mesh.Clear();
			mesh.RecalculateBounds();
		}
	}

	public void SetGridField(GridField<CellStatus> cellData)
	{
		if (cellData == null || cellData.IsClear)
		{
			ClearAreaVisualization();
			return;
		}
		Mesh mesh = ShapeFill.GetComponent<MeshFilter>().mesh;
		Mesh mesh2 = ExtendedShapeFill.GetComponent<MeshFilter>().mesh;
		Mesh mesh3 = ExtendedShapeOutline.GetComponent<MeshFilter>().mesh;
		Mesh mesh4 = ShapeOutline.GetComponent<MeshFilter>().mesh;
		GridField<bool> gridField = new GridField<bool>(gridModel.Width, gridModel.Height, defaultValue: false);
		bool flag = false;
		foreach (GridCoordinate coordinate3 in gridModel.Coordinates)
		{
			gridField[coordinate3] = cellData[coordinate3] != CellStatus.Invalid;
			if (cellData[coordinate3] == CellStatus.Extended || cellData[coordinate3] == CellStatus.FriendlyExtended)
			{
				flag = true;
			}
		}
		GridAreaVisualization.UpdateAreaVisualization(gridField, borderFlags, gridAreaSettings, gridModel, mesh2, mesh3);
		if (flag)
		{
			gridField.Clear();
			foreach (GridCoordinate coordinate4 in gridModel.Coordinates)
			{
				gridField[coordinate4] = cellData[coordinate4] == CellStatus.Valid || cellData[coordinate4] == CellStatus.Friendly;
			}
			GridAreaVisualization.UpdateAreaVisualization(gridField, borderFlags, gridAreaSettings, gridModel, mesh, mesh4);
		}
		else
		{
			mesh.Clear();
			mesh4.Clear();
		}
	}

	private void ShowInnerArea(bool show)
	{
		ShapeFill.SetActive(value: false);
		ShapeOutline.SetActive(show);
	}

	public void Initialize()
	{
		Initialize(GridView.Instance.Grid, null);
		PlayerInputManager instance = PlayerInputManager.Instance;
		instance.ValidTargetsChanged = (ValidTargetsChangedHandler)Delegate.Combine(instance.ValidTargetsChanged, new ValidTargetsChangedHandler(OnValidTargetsChanged));
		ActorMoveInputHandler handler = PlayerInputManager.Instance.GetHandler<ActorMoveInputHandler>();
		handler.DrawnPathChanged = (DrawnPathChangedHandler)Delegate.Combine(handler.DrawnPathChanged, new DrawnPathChangedHandler(OnDrawnPathChanged));
		handler.InteractiveObjectChanged = (InteractiveObjectChangedHandler)Delegate.Combine(handler.InteractiveObjectChanged, new InteractiveObjectChangedHandler(OnInteractiveObjectChanged));
		AbilityTargetGridInputHandler handler2 = PlayerInputManager.Instance.GetHandler<AbilityTargetGridInputHandler>();
		handler2.DrawnPathChanged = (DrawnPathChangedHandler)Delegate.Combine(handler2.DrawnPathChanged, new DrawnPathChangedHandler(OnDrawnPathChanged));
		PlayerInputManager.Instance.GetHandler<SupportInputHandler>().DrawnPathChanged += OnDrawnPathChanged;
		SetCellEdgeVisualisations();

		DebugTWD.LogWarning("Grid View FixShadersFromMatList");
		List<Material> matList = new List<Material>() { DrawnPathEndMaterial, DrawnPathEndMaterialBlocked };
		GameManager.FixShadersFromMatList(matList);
	}

	private void OnDestroy()
	{
		if (PlayerInputManager.Instance != null)
		{
			PlayerInputManager instance = PlayerInputManager.Instance;
			instance.ValidTargetsChanged = (ValidTargetsChangedHandler)Delegate.Remove(instance.ValidTargetsChanged, new ValidTargetsChangedHandler(OnValidTargetsChanged));
			ActorMoveInputHandler handler = PlayerInputManager.Instance.GetHandler<ActorMoveInputHandler>();
			handler.DrawnPathChanged = (DrawnPathChangedHandler)Delegate.Remove(handler.DrawnPathChanged, new DrawnPathChangedHandler(OnDrawnPathChanged));
			handler.InteractiveObjectChanged = (InteractiveObjectChangedHandler)Delegate.Remove(handler.InteractiveObjectChanged, new InteractiveObjectChangedHandler(OnInteractiveObjectChanged));
			AbilityTargetGridInputHandler handler2 = PlayerInputManager.Instance.GetHandler<AbilityTargetGridInputHandler>();
			handler2.DrawnPathChanged = (DrawnPathChangedHandler)Delegate.Remove(handler2.DrawnPathChanged, new DrawnPathChangedHandler(OnDrawnPathChanged));
			PlayerInputManager.Instance.GetHandler<SupportInputHandler>().DrawnPathChanged -= OnDrawnPathChanged;
		}
		GameManager.Instance.Blackboard.BlackboardChanged -= BlackboardChangedHandler;
		if (GameManager.Instance.modelManager.CombatModel != null)
		{
			GameManager.Instance.modelManager.CombatModel.Changed -= HandleCombatModelChanged;
		}
	}

	private void OnInteractiveObjectChanged(InteractiveObjectModel model)
	{
		currentInteractiveObject = model;
	}

	private void SetActionMarker(Vector3 position, MoveActionType type)
	{
		if (ActionGridMarker != null)
		{
			ActionGridMarker.gameObject.SetActive(value: true);
			ActionGridMarker.transform.position = position;
			Color value = ((type == MoveActionType.MoveSprint) ? ExtendedPathColor : NormalPathColor);
			ActionGridMarker.GetComponent<Renderer>().material.SetColor(MaterialParameters.TintColor, value);
			ActionGridMarker.SetActionMarker(type);
		}
	}

	private void ClearActionMarker()
	{
		if (ActionGridMarker != null)
		{
			ActionGridMarker.gameObject.SetActive(value: false);
		}
	}

	private void OnDrawnPathChanged(GridPath path, bool doubleMove)
	{
		bool flag = path.IsValid && Math.Abs((float)path.MoveDistance - (float)GameManager.Instance.playerModel.Combat.ActiveActor.MoveRange) <= 1f;
		flag = false;
		ShowInnerArea(flag);
		Vector3 vector = new Vector3(0f, 1f, 0f);
		ClearActionMarker();
		Mesh mesh = DrawnPath.GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			mesh.Clear();
			if (path != null && (path.IsValid || path.HasTargetCoordinate))
			{
				Vector3 position = gridModel.GetPosition(path.End).ToVector3();
				bool flag2 = path.HasTargetCoordinate && GameManager.Instance.playerModel.Combat.GetOccupier(path.TargetCoordinate) != null;
				if (path.HasTargetCoordinate)
				{
					SetActionMarker(position, flag2 ? MoveActionType.Melee : MoveActionType.Loot);
				}
				else
				{
					SetActionMarker(position, doubleMove ? MoveActionType.MoveSprint : MoveActionType.Move);
				}
			}
			if (path != null && path.IsValid)
			{
				bool flag3 = path.HasTargetCoordinate && GameManager.Instance.playerModel.Combat.GetOccupier(path.TargetCoordinate) != null;
				Color color = NormalPathColor;
				Color color2 = ExtendedPathColor;
				if (currentInteractiveObject != null)
				{
					color = UsePathColor;
					color2 = color;
				}
				else if (path.IsDanger)
				{
					color = DangerPathColor;
					color2 = DangerPathColor;
				}
				else if (path.HasTargetCoordinate)
				{
					color = (flag3 ? AttackPathColor : UsePathColor);
					color2 = color;
				}
				List<ColorLine> list = new List<ColorLine>();
				bool flag4 = false;
				for (int i = 0; i < path.Length - 1; i++)
				{
					bool num = flag4;
					flag4 = path.GetMoveDistanceAt(i) > GameManager.Instance.playerModel.Combat.ActiveActor.MoveRange / 2;
					Color lineColor = (flag4 ? color2 : color);
					if (num != flag4)
					{
						Vector3 vector2 = gridModel.GetPosition(path[i]).ToVector3();
						Vector3 vector3 = gridModel.GetPosition(path[i + 1]).ToVector3();
						Vector3 vector4 = (vector2 + vector3) * 0.5f;
						list.Add(new ColorLine(vector2, vector4, color));
						list.Add(new ColorLine(vector4, vector3, color2));
					}
					else
					{
						list.Add(new ColorLine(gridModel.GetPosition(path[i]).ToVector3(), gridModel.GetPosition(path[i + 1]).ToVector3(), lineColor));
					}
				}
				PolylinePath polylinePath = new PolylinePath();
				for (int j = 0; j < list.Count; j++)
				{
					ColorLine colorLine = list[j];
					ColorLine colorLine2 = ((j + 1 < list.Count) ? list[j + 1] : null);
					if (colorLine2 == null || Vector3.Dot(Vector3.Normalize(colorLine2.end - colorLine2.start), Vector3.Normalize(colorLine.end - colorLine.start)) > 0.95f)
					{
						if (!polylinePath.EndsAtCurve)
						{
							polylinePath.AddSegment(new LineSegment(colorLine.start, colorLine.end, vector, colorLine.color));
						}
						else
						{
							polylinePath.AddSegment(new LineSegment(colorLine.center, colorLine.end, vector, colorLine.color));
						}
						continue;
					}
					Vector3 startTangent = (colorLine.end - colorLine.center) * (1f - gridAreaSettings.Curvature * 0.75f);
					Vector3 endTangent = (colorLine2.end - colorLine2.center) * (1f - gridAreaSettings.Curvature * 0.75f);
					if (!polylinePath.EndsAtCurve)
					{
						polylinePath.AddSegment(new LineSegment(colorLine.start, colorLine.center, vector, colorLine.color));
					}
					polylinePath.AddSegment(new CurveSegment(colorLine.center, colorLine2.center, startTangent, endTangent, vector, colorLine.color));
				}
				List<Vector3> list2 = new List<Vector3>();
				List<Vector3> list3 = new List<Vector3>();
				List<Color> list4 = new List<Color>();
				polylinePath.GetPathPoints(list2, list3, list4, 8);
				onDrawnPathChangedColors.Clear();
				onDrawnPathChangedVertices.Clear();
				onDrawnPathChangedUvs.Clear();
				onDrawnPathChangedTriangles.Clear();
				for (int k = 0; k < onDrawnPathChangedColors.Count; k++)
				{
					onDrawnPathChangedColors[k] = color;
				}
				MeshGenerator.CreateThickline(list2, list3, list4, DrawnPathThickness, onDrawnPathChangedVertices, onDrawnPathChangedUvs, onDrawnPathChangedTriangles, onDrawnPathChangedColors, gridAreaSettings.TextureScale);
				mesh.vertices = onDrawnPathChangedVertices.ToArray();
				mesh.normals = null;
				mesh.uv = onDrawnPathChangedUvs.ToArray();
				mesh.colors = onDrawnPathChangedColors.ToArray();
				mesh.triangles = onDrawnPathChangedTriangles.ToArray();
			}
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
		Mesh mesh2 = DrawnPathEnd.GetComponent<MeshFilter>().mesh;
		if (!(mesh2 != null))
		{
			return;
		}
		mesh2.Clear();
		if (path != null && (path.IsValid || path.HasTargetCoordinate) && path.HasTargetCoordinate)
		{
			onDrawnPathChangedColors.Clear();
			onDrawnPathChangedVertices.Clear();
			onDrawnPathChangedUvs.Clear();
			onDrawnPathChangedTriangles.Clear();
			Vector3 vector5 = gridModel.GetPosition(path.End).ToVector3();
			Vector3 vector6 = gridModel.GetPosition(path.TargetCoordinate).ToVector3();
			Vector3 vector7 = vector6 - vector5;
			vector7.Normalize();
			float num2 = DrawnPathThickness * 1.1f;
			GridCoordinate gridCoordinate = GameManager.Instance.playerModel.Combat.GetFirstAimTrajectoryBlockCoordinate(includeImpenetrable: PlayerInputManager.Instance.ControlledActor?.SelectedAbility?.Definition.CanBeBlocked ?? true, sourceActor: PlayerInputManager.Instance.ControlledActor, from: path.End, to: path.TargetCoordinate);
			if (gridCoordinate != GridCoordinate.Invalid && gridCoordinate != path.TargetCoordinate)
			{
				float num3 = Vector3.Dot(gridModel.GetPosition(gridCoordinate).ToVector3() - vector5, vector6 - vector5) / (vector6 - vector5).sqrMagnitude;
				MeshGenerator.CreateMultiPartRectangle(vector5 - vector7 * num2 * 0.5f, vector6 + vector7 * num2 * 0.5f, num2, new float[3] { 1.4f, num3, 0.5f }, new float[3] { 0.48f, 0.7012f, 0.8f }, onDrawnPathChangedVertices, onDrawnPathChangedUvs, onDrawnPathChangedTriangles);
				DrawnPathEnd.GetComponent<Renderer>().sharedMaterial = DrawnPathEndMaterialBlocked;
			}
			else
			{
				MeshGenerator.CreateMultiPartRectangle(vector5 - vector7 * num2 * 0.5f, vector6 + vector7 * num2 * 0.5f, num2, new float[2] { 1.4f, 0.5f }, new float[2] { 0.48f, 0.8f }, onDrawnPathChangedVertices, onDrawnPathChangedUvs, onDrawnPathChangedTriangles);
				DrawnPathEnd.GetComponent<Renderer>().sharedMaterial = DrawnPathEndMaterial;
			}
			for (int l = 0; l < onDrawnPathChangedColors.Count; l++)
			{
				onDrawnPathChangedColors[l] = new Color(1f, 1f, 1f, 1f);
			}
			mesh2.vertices = onDrawnPathChangedVertices.ToArray();
			mesh2.normals = null;
			mesh2.uv = onDrawnPathChangedUvs.ToArray();
			mesh2.colors = onDrawnPathChangedColors.ToArray();
			mesh2.triangles = onDrawnPathChangedTriangles.ToArray();
		}
		mesh2.RecalculateNormals();
		mesh2.RecalculateBounds();
	}

	private void SetCellEdgeVisualisations()
	{
		DestroyCornerIndicators();
		blockedCoordinates.Clear();
		blockedByWalls.Clear();
		blockedDiagonallyCoordinates.Clear();
		blockedCorners.Clear();
		allowedCorners.Clear();
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		blockedCoordinates = GetBlockedCoordinates(combat);
		disabledInteractables = GetDisabledInteractiveObjectCoordinates(combat);
		if (disabledInteractables.Count > 0)
		{
			HashSet<GridCoordinate> hashSet = new HashSet<GridCoordinate>();
			foreach (GridCoordinate blockedCoordinate in blockedCoordinates)
			{
				bool flag = true;
				foreach (GridCoordinate disabledInteractable in disabledInteractables)
				{
					if (blockedCoordinate.X == disabledInteractable.X && blockedCoordinate.Y == disabledInteractable.Y)
					{
						flag = false;
					}
				}
				if (flag)
				{
					hashSet.Add(blockedCoordinate);
				}
			}
			blockedCoordinates = hashSet;
		}
		blockedByWalls = GetCoordinatesBlockedByWalls(combat, blockedCoordinates);
		blockedDiagonallyCoordinates = GetCoordinatesBlockedDiagonallyOnly(combat, blockedCoordinates, blockedByWalls);
		foreach (GridCoordinate blockedCoordinate2 in blockedCoordinates)
		{
			FixedVec3 position = gridModel.GetPosition(blockedCoordinate2);
			Vector3 vector = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
			Vector3 vector2 = vector + new Vector3(0.49f * (float)gridModel.CellSize.X, 0f, 0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector3 = vector + new Vector3(-0.49f * (float)gridModel.CellSize.X, 0f, 0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector4 = vector + new Vector3(0.49f * (float)gridModel.CellSize.X, 0f, -0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector5 = vector + new Vector3(-0.49f * (float)gridModel.CellSize.X, 0f, -0.49f * (float)gridModel.CellSize.Y);
			for (int i = 0; i < 8; i += 4)
			{
				GridCoordinate coordinateNeighbor = combat.Grid.GetCoordinateNeighbor(blockedCoordinate2, i);
				int index = ((i - 2 < 0) ? 6 : (i - 2));
				int index2 = ((i + 2 > 7) ? 2 : (i + 2));
				int index3 = ((i + 4 <= 7) ? (i + 4) : 0);
				GridCoordinate coordinateNeighbor2 = combat.Grid.GetCoordinateNeighbor(blockedCoordinate2, index);
				GridCoordinate coordinateNeighbor3 = combat.Grid.GetCoordinateNeighbor(blockedCoordinate2, index2);
				GridCoordinate coordinateNeighbor4 = combat.Grid.GetCoordinateNeighbor(blockedCoordinate2, index3);
				if ((combat.IsBlocked(coordinateNeighbor) && combat.IsBlocked(coordinateNeighbor4)) || (combat.IsBlocked(coordinateNeighbor2) && combat.IsBlocked(coordinateNeighbor3)))
				{
					break;
				}
				Vector3 key = ((i == 0) ? vector3 : vector4);
				GameObject value = ((i == 0) ? northWestCornerVisualization : southEastCornerVisualization);
				GameObject value2 = ((i == 0) ? northWestCornerAllowedVisualization : southEastCornerAllowedVisualization);
				Vector3 key2 = ((i == 0) ? vector2 : vector5);
				GameObject value3 = ((i == 0) ? northEastCornerVisualization : southWestCornerVisualization);
				GameObject value4 = ((i == 0) ? northEastCornerAllowedVisualization : southWestCornerAllowedVisualization);
				if (!combat.IsBlocked(coordinateNeighbor3) && !combat.IsBlocked(coordinateNeighbor))
				{
					if (combat.IsGridLineMovementBlocked(coordinateNeighbor, coordinateNeighbor3) || combat.IsGridLineMovementBlocked(coordinateNeighbor3, coordinateNeighbor))
					{
						if (!blockedCorners.ContainsKey(key2))
						{
							blockedCorners.Add(key2, value3);
						}
					}
					else if (!allowedCorners.ContainsKey(key2))
					{
						allowedCorners.Add(key2, value4);
					}
				}
				if (combat.IsBlocked(coordinateNeighbor2) || combat.IsBlocked(coordinateNeighbor))
				{
					continue;
				}
				if (combat.IsGridLineMovementBlocked(coordinateNeighbor, coordinateNeighbor2) || combat.IsGridLineMovementBlocked(coordinateNeighbor2, coordinateNeighbor))
				{
					if (!blockedCorners.ContainsKey(key))
					{
						blockedCorners.Add(key, value);
					}
				}
				else if (!allowedCorners.ContainsKey(key))
				{
					allowedCorners.Add(key, value2);
				}
			}
		}
		foreach (GridCoordinate blockedByWall in blockedByWalls)
		{
			FixedVec3 position2 = gridModel.GetPosition(blockedByWall);
			Vector3 vector6 = new Vector3((float)position2.X, (float)position2.Y, (float)position2.Z);
			Vector3 vector7 = vector6 + new Vector3(0.49f * (float)gridModel.CellSize.X, 0f, 0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector8 = vector6 + new Vector3(-0.49f * (float)gridModel.CellSize.X, 0f, 0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector9 = vector6 + new Vector3(0.49f * (float)gridModel.CellSize.X, 0f, -0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector10 = vector6 + new Vector3(-0.49f * (float)gridModel.CellSize.X, 0f, -0.49f * (float)gridModel.CellSize.Y);
			for (int j = 0; j < 8; j += 2)
			{
				GridCoordinate coordinateNeighbor5 = combat.Grid.GetCoordinateNeighbor(blockedByWall, j);
				int index4 = ((j - 2 < 0) ? 6 : (j - 2));
				int index5 = ((j - 1 < 0) ? 7 : (j - 1));
				int index6 = ((j + 2 <= 7) ? (j + 2) : 0);
				int index7 = ((j + 1 > 7) ? 1 : (j + 1));
				GridCoordinate coordinateNeighbor6 = combat.Grid.GetCoordinateNeighbor(blockedByWall, index5);
				GridCoordinate coordinateNeighbor7 = combat.Grid.GetCoordinateNeighbor(blockedByWall, index7);
				GridCoordinate coordinateNeighbor8 = combat.Grid.GetCoordinateNeighbor(blockedByWall, index4);
				GridCoordinate coordinateNeighbor9 = combat.Grid.GetCoordinateNeighbor(blockedByWall, index6);
				Vector3 key3 = Vector3.zero;
				Vector3 key4 = Vector3.zero;
				GameObject value5 = null;
				GameObject value6 = null;
				GameObject value7 = null;
				GameObject value8 = null;
				switch (j)
				{
				case 0:
					key3 = vector8;
					key4 = vector7;
					value5 = northWestCornerVisualization;
					value6 = northWestCornerAllowedVisualization;
					value7 = northEastCornerVisualization;
					value8 = northEastCornerAllowedVisualization;
					break;
				case 2:
					key3 = vector7;
					key4 = vector9;
					value5 = northEastCornerVisualization;
					value6 = northEastCornerAllowedVisualization;
					value7 = southEastCornerVisualization;
					value8 = southEastCornerAllowedVisualization;
					break;
				case 4:
					key3 = vector9;
					key4 = vector10;
					value5 = southEastCornerVisualization;
					value6 = southEastCornerAllowedVisualization;
					value7 = southWestCornerVisualization;
					value8 = southWestCornerAllowedVisualization;
					break;
				case 6:
					key3 = vector10;
					key4 = vector8;
					value5 = southWestCornerVisualization;
					value6 = southWestCornerAllowedVisualization;
					value7 = northWestCornerVisualization;
					value8 = northWestCornerAllowedVisualization;
					break;
				}
				if (combat.CanTraverse(null, blockedByWall, coordinateNeighbor5))
				{
					continue;
				}
				if (!blockedCoordinates.Contains(coordinateNeighbor8) && !blockedCoordinates.Contains(coordinateNeighbor5) && !blockedCoordinates.Contains(coordinateNeighbor6) && combat.CanTraverse(null, coordinateNeighbor6, coordinateNeighbor5))
				{
					if (!combat.CanTraverse(null, coordinateNeighbor8, coordinateNeighbor5))
					{
						if (combat.CanTraverse(null, coordinateNeighbor8, coordinateNeighbor6) && !blockedCorners.ContainsKey(key3))
						{
							blockedCorners.Add(key3, value5);
						}
					}
					else if (!allowedCorners.ContainsKey(key3))
					{
						allowedCorners.Add(key3, value6);
					}
				}
				if (blockedCoordinates.Contains(coordinateNeighbor9) || blockedCoordinates.Contains(coordinateNeighbor5) || blockedCoordinates.Contains(coordinateNeighbor7) || !combat.CanTraverse(null, coordinateNeighbor7, coordinateNeighbor5))
				{
					continue;
				}
				if (!combat.CanTraverse(null, coordinateNeighbor9, coordinateNeighbor5))
				{
					if (combat.CanTraverse(null, coordinateNeighbor9, coordinateNeighbor7) && !blockedCorners.ContainsKey(key4))
					{
						blockedCorners.Add(key4, value7);
					}
				}
				else if (!allowedCorners.ContainsKey(key4))
				{
					allowedCorners.Add(key4, value8);
				}
			}
		}
		foreach (GridCoordinate blockedDiagonallyCoordinate in blockedDiagonallyCoordinates)
		{
			FixedVec3 position3 = gridModel.GetPosition(blockedDiagonallyCoordinate);
			Vector3 vector11 = new Vector3((float)position3.X, (float)position3.Y, (float)position3.Z);
			Vector3 vector12 = vector11 + new Vector3(0.49f * (float)gridModel.CellSize.X, 0f, 0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector13 = vector11 + new Vector3(-0.49f * (float)gridModel.CellSize.X, 0f, 0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector14 = vector11 + new Vector3(0.49f * (float)gridModel.CellSize.X, 0f, -0.49f * (float)gridModel.CellSize.Y);
			Vector3 vector15 = vector11 + new Vector3(-0.49f * (float)gridModel.CellSize.X, 0f, -0.49f * (float)gridModel.CellSize.Y);
			for (int k = 1; k < 8; k += 2)
			{
				GridCoordinate coordinateNeighbor10 = combat.Grid.GetCoordinateNeighbor(blockedDiagonallyCoordinate, k);
				Vector3 key5 = Vector3.zero;
				GameObject value9 = null;
				switch (k)
				{
				case 1:
					key5 = vector12;
					value9 = northEastCornerVisualization;
					break;
				case 3:
					key5 = vector14;
					value9 = southEastCornerVisualization;
					break;
				case 5:
					key5 = vector15;
					value9 = southWestCornerVisualization;
					break;
				case 7:
					key5 = vector13;
					value9 = northWestCornerVisualization;
					break;
				}
				if (!combat.CanTraverse(null, blockedDiagonallyCoordinate, coordinateNeighbor10) && !blockedCorners.ContainsKey(key5))
				{
					blockedCorners.Add(key5, value9);
				}
			}
		}
		foreach (Vector3 key6 in blockedCorners.Keys)
		{
			UnityEngine.Object.Instantiate(blockedCorners[key6], key6, Quaternion.identity, cornerVisualizationParent.transform);
		}
		foreach (Vector3 key7 in allowedCorners.Keys)
		{
			UnityEngine.Object.Instantiate(allowedCorners[key7], key7, Quaternion.identity, cornerVisualizationParent.transform);
		}
		GameManager.FixShadersMeshFromParent(cornerVisualizationParent.transform);
	}

	private HashSet<GridCoordinate> GetCoordinatesBlockedDiagonallyOnly(CombatModel combatModel, IEnumerable<GridCoordinate> blockedCompletely, IEnumerable<GridCoordinate> wallBlocked)
	{
		HashSet<GridCoordinate> hashSet = new HashSet<GridCoordinate>();
		foreach (GridCoordinate coordinate in gridModel.Coordinates)
		{
			if (wallBlocked.Contains(coordinate) || blockedCompletely.Contains(coordinate))
			{
				continue;
			}
			GridCoordinate gridCoordinate2 = new GridCoordinate(-1, -1);
			GridCoordinate gridCoordinate3 = new GridCoordinate(-1, -1);
			for (int i = 1; i < 8; i += 2)
			{
				GridCoordinate coordinateNeighbor = combatModel.Grid.GetCoordinateNeighbor(coordinate, i);
				int index = ((i - 1 < 0) ? 7 : (i - 1));
				int index2 = ((i + 1 <= 7) ? (i + 1) : 0);
				gridCoordinate2 = combatModel.Grid.GetCoordinateNeighbor(coordinate, index);
				gridCoordinate3 = combatModel.Grid.GetCoordinateNeighbor(coordinate, index2);
				if (!combatModel.CanTraverse(null, coordinate, coordinateNeighbor) && !blockedCompletely.Contains(coordinateNeighbor) && !blockedCompletely.Contains(gridCoordinate2) && !blockedCompletely.Contains(gridCoordinate3) && !wallBlocked.Contains(gridCoordinate3) && !wallBlocked.Contains(gridCoordinate2) && coordinateNeighbor.X != -1 && coordinateNeighbor.Y != -1)
				{
					hashSet.Add(coordinate);
				}
			}
		}
		return hashSet;
	}

	private HashSet<GridCoordinate> GetCoordinatesBlockedByWalls(CombatModel combatModel, IEnumerable<GridCoordinate> blocked)
	{
		HashSet<GridCoordinate> hashSet = new HashSet<GridCoordinate>();
		foreach (GridCoordinate coordinate in gridModel.Coordinates)
		{
			for (int i = 0; i < 8; i += 2)
			{
				GridCoordinate coordinateNeighbor = combatModel.Grid.GetCoordinateNeighbor(coordinate, i);
				if (!combatModel.CanTraverse(null, coordinate, coordinateNeighbor) && !blocked.Contains(coordinateNeighbor) && coordinateNeighbor.X != -1 && coordinateNeighbor.Y != -1)
				{
					hashSet.Add(coordinate);
				}
			}
		}
		return hashSet;
	}

	private HashSet<GridCoordinate> GetBlockedCoordinates(CombatModel combatModel)
	{
		HashSet<GridCoordinate> hashSet = new HashSet<GridCoordinate>();
		foreach (GridCoordinate coordinate in gridModel.Coordinates)
		{
			if (combatModel.IsBlocked(coordinate))
			{
				hashSet.Add(coordinate);
			}
		}
		return hashSet;
	}

	private HashSet<GridCoordinate> GetDisabledInteractiveObjectCoordinates(CombatModel combatModel)
	{
		HashSet<GridCoordinate> hashSet = new HashSet<GridCoordinate>();
		if (combatModel != null && combatModel.InteractiveObjects != null)
		{
			for (int i = 0; i < combatModel.InteractiveObjects.Length; i++)
			{
				if (combatModel.InteractiveObjects != null && combatModel.InteractiveObjects[i] != null && combatModel.InteractiveObjects[i].InteractionDisabled && combatModel.InteractiveObjects[i].Location != null)
				{
					hashSet.Add(combatModel.InteractiveObjects[i].Location.Coordinate);
				}
			}
		}
		return hashSet;
	}

	private void HandleCombatModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "collidersUpdated")
		{
			SetCellEdgeVisualisations();
		}
	}

	private void OnValidTargetsChanged(GridField<CellStatus> inValidTargets)
	{
		SetGridField(inValidTargets);
	}

	private void UpdateCellEdgeVisibility()
	{
		bool IsActive = !OfflineManager.IsLoadDataManager ? GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatGridEnabled") : OfflineManager.IsCombatGridEnabled;
		cornerVisualizationParent.SetActive(IsActive);
	}

	private void DestroyCornerIndicators()
	{
		foreach (Transform item in cornerVisualizationParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
