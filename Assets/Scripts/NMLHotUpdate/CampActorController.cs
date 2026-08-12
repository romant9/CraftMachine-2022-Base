using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CampActorController : MonoBehaviour
{
	private GameObject inhabitantHostContainer;

	private ActorView actorView;

	private CampFootpaths footPaths;

	private GameObject visualization;

	private float idleWaitTime;

	private SurvivorAnimationController controller;

	private InhabitantLegacyAnimationController legacyController;

	private VisualizationTask pendingWaypointTask;

	private CampWaypoint currentWaypoint;

	private CampWaypointEndConditionDelegate currentWaypointEndCondition;

	private CampActorLogic currentLogic;

	private CampWaypointPath currentWaypointPath;

	private int currentWaypointPathIndex;

	private const float smallStep = 0.1f;

	private bool hasVisualizationQueueTask;

	public bool IsAvailable
	{
		get
		{
			if (!(currentWaypoint != null))
			{
				return true;
			}
			return !currentWaypoint.IsStatic();
		}
	}

	public ActorView ActorView
	{
		get
		{
			if (actorView == null)
			{
				actorView = GetComponent<ActorView>();
			}
			return actorView;
		}
	}

	public CampFootpaths FootPaths
	{
		get
		{
			if (footPaths == null && CampView.Instance != null && CampView.Instance.CampViewActors != null)
			{
				GameObject gameObject = CampView.Instance.CampViewActors.gameObject;
				if (gameObject != null)
				{
					footPaths = gameObject.GetComponent<CampFootpaths>();
				}
			}
			return footPaths;
		}
	}

	public SurvivorAnimationController Controller
	{
		get
		{
			if (controller == null)
			{
				controller = GetComponent<SurvivorAnimationController>();
			}
			return controller;
		}
	}

	public InhabitantLegacyAnimationController LegacyController
	{
		get
		{
			if (legacyController == null)
			{
				legacyController = GetComponent<InhabitantLegacyAnimationController>();
			}
			return legacyController;
		}
	}

	public BuildingModel currentBuildingModel { get; private set; }

	public bool IsPuppet { get; private set; }

	private bool isIdle => !hasVisualizationQueueTask;

	private void UpdateVisualizationQueueStatus()
	{
		if (VisualizationQueue.Instance != null)
		{
			hasVisualizationQueueTask = VisualizationQueue.Instance.HasDependencyObject(ActorView.Model);
		}
		else
		{
			hasVisualizationQueueTask = false;
		}
	}

	private void Awake()
	{
		inhabitantHostContainer = base.gameObject.transform.parent.gameObject;
		Vector2 randomWaypointPosition = CampView.Instance.CampViewActors.NavigationMesh.GetRandomWaypointPosition();
		base.transform.position = new Vector3(randomWaypointPosition.x, 0f, randomWaypointPosition.y);
		idleWaitTime = 0.01f;
		UpdateVisualizationQueueStatus();
		VisualizationQueue.Instance.VisualizationTaskCompleted += VisualizationTaskCompleted;
		GameManager.Instance.playerModel.Camp.Changed += OnCampChanged;
	}

	private void OnEnable()
	{
		idleWaitTime = 0.01f;
	}

	private void OnDestroy()
	{
		if (VisualizationQueue.Instance != null)
		{
			VisualizationQueue.Instance.VisualizationTaskCompleted -= VisualizationTaskCompleted;
		}
		GameManager.Instance.playerModel.Camp.Changed -= OnCampChanged;
	}

	public void ForceMovement(CampWaypointPath path)
	{
		if (path != null && path.Waypoints.Count >= 2)
		{
			StopDependentVisualTasks();
			IsPuppet = true;
			base.transform.position = path.Waypoints[0].transform.position;
			MoveAlongPath(path);
		}
	}

	public void ForceStand(CampWaypoint waypoint)
	{
		if (waypoint != null)
		{
			StopDependentVisualTasks();
			IsPuppet = true;
			base.transform.position = waypoint.transform.position;
		}
	}

	private void MoveAlongPath(CampWaypointPath path)
	{
		if (path != null && path.Waypoints.Count >= 2)
		{
			currentWaypointPath = path;
			currentWaypointPathIndex = 0;
			CheckNextWaypointAlongPath();
		}
	}

	private void CheckNextWaypointAlongPath()
	{
		if (hasVisualizationQueueTask || !(currentWaypointPath != null))
		{
			return;
		}
		currentWaypointPathIndex++;
		if (currentWaypointPath.Waypoints.Count > currentWaypointPathIndex)
		{
			CampWaypoint component = currentWaypointPath.Waypoints[currentWaypointPathIndex].GetComponent<CampWaypoint>();
			PolylinePath polylinePath = new PolylinePath();
			polylinePath.AddSegment(new LineSegment(ActorView.transform.position, component.transform.position, new Vector3(0f, 1f, 0f)));
			if (!ActorView.LightWeight)
			{
				VisualizationQueue.Instance.Add(new MoveVisualizationTask(ActorView.Model, polylinePath, component.MoveSpeed));
			}
			else
			{
				VisualizationQueue.Instance.Add(new InhabitantMoveVisualizationTask(ActorView.Model, inhabitantHostContainer, polylinePath, component.MoveSpeed));
			}
			UpdateVisualizationQueueStatus();
		}
		else
		{
			currentWaypointPath = null;
			currentWaypointPathIndex = 0;
		}
	}

	public void ReleaseForcedBehavior()
	{
		IsPuppet = false;
	}

	private void StopDependentVisualTasks()
	{
		if (VisualizationQueue.Instance != null)
		{
			VisualizationQueue.Instance.StopDependentTasks(ActorView.Model);
			UpdateVisualizationQueueStatus();
		}
	}

	public void Reset()
	{
		if (VisualizationQueue.Instance != null && ActorView != null && !IsPuppet)
		{
			LeaveWaypoint();
			StopDependentVisualTasks();
		}
	}

	public void GotoWaypoint(CampWaypoint waypoint, CampWaypointEndConditionDelegate endCondition, BuildingView refreshCondition, CampActorLogic logic)
	{
		if (!IsPuppet)
		{
			Reset();
			currentWaypoint = waypoint;
			currentWaypointEndCondition = endCondition;
			currentBuildingModel = refreshCondition.Model;
			currentLogic = logic;
			IssueWaypointTask();
		}
	}

	private void Update()
	{
		CheckNextWaypointAlongPath();
		if (IsPuppet)
		{
			return;
		}
		if (currentWaypointEndCondition != null && currentWaypointEndCondition(currentWaypoint, this))
		{
			LeaveWaypoint();
		}
		if (currentWaypointEndCondition != null || pendingWaypointTask != null)
		{
			return;
		}
		if (isIdle && idleWaitTime == 0f)
		{
			idleWaitTime = Random.Range(4f, 6f);
		}
		if (!(idleWaitTime > 0f))
		{
			return;
		}
		idleWaitTime = Mathf.Max(0f, idleWaitTime - Time.deltaTime);
		if (idleWaitTime != 0f)
		{
			return;
		}
		Vector2 vector = new Vector2(base.transform.position.x, base.transform.position.z);
		PolylinePath polylinePath = CampView.Instance.CampViewActors.NavigationMesh.FindPathToRandomWaypoint(vector);
		if (polylinePath != null)
		{
			if (!ActorView.LightWeight)
			{
				VisualizationQueue.Instance.Add(new MoveVisualizationTask(ActorView.Model, polylinePath, MoveSpeed.Walk));
			}
			else
			{
				VisualizationQueue.Instance.Add(new InhabitantMoveVisualizationTask(ActorView.Model, inhabitantHostContainer, polylinePath, MoveSpeed.Walk));
			}
			UpdateVisualizationQueueStatus();
			CreateDebugVisualization(polylinePath);
			if (!PlatformInfo.HasFlag(PlatformFlag.SlowCPU))
			{
				FootPaths.NewPathRequest(polylinePath);
			}
		}
	}

	private void CreateDebugVisualization(PolylinePath path)
	{
		if (CampView.Instance.CampViewActors.DebugGrid == null)
		{
			return;
		}
		if (visualization != null)
		{
			Object.Destroy(visualization);
			visualization = null;
		}
		visualization = Object.Instantiate(CampView.Instance.CampViewActors.DebugGrid.gameObject);
		visualization.SetActive(value: true);
		base.gameObject.transform.localPosition = new Vector3(0f, 0.2f, 0f);
		PolylinePathIterator polylinePathIterator = new PolylinePathIterator(path);
		polylinePathIterator.Advance(0.1f);
		List<int> list = new List<int>();
		List<Vector3> list2 = new List<Vector3>();
		int num = 0;
		while (!polylinePathIterator.AtEnd)
		{
			Vector3 position = polylinePathIterator.Position;
			polylinePathIterator.Advance(0.1f);
			list2.Add(position);
			if (!polylinePathIterator.AtEnd)
			{
				list.Add(num);
				list.Add(num + 1);
				num++;
			}
		}
		int[] indices = list.ToArray();
		Vector3[] array = list2.ToArray();
		Vector2[] uv = new Vector2[array.Length];
		Mesh mesh = visualization.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = array;
		mesh.uv = uv;
		mesh.normals = null;
		mesh.colors = null;
		mesh.SetIndices(indices, MeshTopology.Lines, 0);
		mesh.RecalculateBounds();
	}

	private void IssueWaypointTask()
	{
		currentWaypoint.Occupied = true;
		if (currentWaypoint.IsStatic())
		{
			base.transform.position = currentWaypoint.GetVector3Position;
			base.transform.rotation = currentWaypoint.transform.rotation;
			if (!ActorView.LightWeight)
			{
				pendingWaypointTask = new MoveVisualizationTask(ActorView.Model, new PolylinePath(), MoveSpeed.Walk);
			}
			else
			{
				pendingWaypointTask = new InhabitantMoveVisualizationTask(ActorView.Model, inhabitantHostContainer, new PolylinePath(), MoveSpeed.Walk);
			}
			VisualizationTaskCompleted(pendingWaypointTask);
			return;
		}
		Vector2 vector = new Vector2(base.transform.position.x, base.transform.position.z);
		PolylinePath polylinePath = CampView.Instance.CampViewActors.NavigationMesh.FindPath(vector, currentWaypoint.GetVector2Position);
		if (polylinePath == null)
		{
			return;
		}
		if (polylinePath.Length < 0.1f && polylinePath.Segments.Count == 1)
		{
			PathSegment pathSegment = polylinePath.Segments[0];
			PathSegment segment = new LineSegment(pathSegment.end - currentWaypoint.transform.forward.normalized * 0.1f, pathSegment.end, Vector3.up);
			polylinePath.RemoveSegment(pathSegment);
			polylinePath.AddSegment(segment);
		}
		else
		{
			PathSegment pathSegment2 = polylinePath.Segments[polylinePath.Segments.Count - 1];
			Vector3 endTangent = currentWaypoint.transform.forward.normalized * (pathSegment2.end - pathSegment2.start).magnitude * NavigationMesh.curvature;
			Vector3 vector2 = currentWaypoint.transform.forward.normalized * 0.1f;
			if (pathSegment2 is LineSegment)
			{
				CurveSegment segment2 = new CurveSegment(pathSegment2.start, pathSegment2.end - vector2, Vector3.zero, endTangent, Vector3.up);
				polylinePath.RemoveSegment(pathSegment2);
				polylinePath.AddSegment(segment2);
			}
			else if (pathSegment2 is CurveSegment)
			{
				CurveSegment curveSegment = pathSegment2 as CurveSegment;
				CurveSegment segment3 = new CurveSegment(curveSegment.start, curveSegment.end - vector2, curveSegment.GetTangent(0f), endTangent, Vector3.up);
				polylinePath.RemoveSegment(pathSegment2);
				polylinePath.AddSegment(segment3);
			}
			polylinePath.AddSegment(new LineSegment(pathSegment2.end - vector2, pathSegment2.end, Vector3.up));
		}
		if (!ActorView.LightWeight)
		{
			pendingWaypointTask = new MoveVisualizationTask(ActorView.Model, polylinePath, MoveSpeed.Walk);
		}
		else
		{
			pendingWaypointTask = new InhabitantMoveVisualizationTask(ActorView.Model, inhabitantHostContainer, polylinePath, MoveSpeed.Walk);
		}
		VisualizationQueue.Instance.Add(pendingWaypointTask);
		UpdateVisualizationQueueStatus();
		CreateDebugVisualization(polylinePath);
		if (!PlatformInfo.HasFlag(PlatformFlag.SlowCPU))
		{
			FootPaths.NewPathRequest(polylinePath);
		}
	}

	private void VisualizationTaskCompleted(VisualizationTask task)
	{
		if (task == pendingWaypointTask)
		{
			pendingWaypointTask = null;
			if (currentWaypoint != null && currentWaypoint.Animation != null)
			{
				if (Controller != null)
				{
					Controller.StartCustomAnimation(currentWaypoint.Animation);
				}
				else if (LegacyController != null)
				{
					LegacyController.StartCustomAnimation();
				}
			}
			ActorView.VisualizationTaskCompleted(task);
		}
		UpdateVisualizationQueueStatus();
	}

	private void OnCampChanged(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "EventUpgradeBuilding":
		case "EventLevelUpBuilding":
		case "EventPositionBuilding":
		{
			BuildingModel buildingModel = args as BuildingModel;
			if (buildingModel == currentBuildingModel)
			{
				CampActorLogic campActorLogic = currentLogic;
				Reset();
				if (campActorLogic != null)
				{
					StartCoroutine(DelayedSendSurvivorsToBuildingWaypoints(campActorLogic, CampView.Instance.CampViewBuildings.FindBuildingView(buildingModel)));
				}
			}
			break;
		}
		}
	}

	private IEnumerator DelayedSendSurvivorsToBuildingWaypoints(CampActorLogic logic, BuildingView view)
	{
		yield return 0;
		yield return 0;
		logic.SendSurvivorsToBuildingWaypoints(view);
	}

	private void LeaveWaypoint()
	{
		if (currentWaypoint != null)
		{
			currentWaypoint.Occupied = false;
		}
		pendingWaypointTask = null;
		currentWaypoint = null;
		currentWaypointEndCondition = null;
		currentBuildingModel = null;
		currentLogic = null;
		if (Controller != null && Controller.IsValid && Controller.IsCustomAnimationPlaying())
		{
			Controller.StopCustomAnimation();
		}
		else if (LegacyController != null)
		{
			LegacyController.StopCustomAnimation();
		}
	}
}
