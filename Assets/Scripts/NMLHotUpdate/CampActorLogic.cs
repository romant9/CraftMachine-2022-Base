using UnityEngine;

public abstract class CampActorLogic
{
	protected CampViewActors viewActors;

	protected WaypointType waypointType;

	public abstract void Initialize();

	public abstract void OnEnable();

	public abstract void OnDestroy();

	public abstract bool EndCondition(CampWaypoint waypoint, CampActorController campActorController);

	protected void InitializeInternal(WaypointType type)
	{
		viewActors = CampView.Instance.CampViewActors;
		waypointType = type;
	}

	public void SendSurvivorsToBuildingWaypoints(BuildingView view)
	{
		CampWaypoint[] componentsInChildren = view.GetComponentsInChildren<CampWaypoint>();
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].Type == waypointType && !componentsInChildren[i].Occupied)
			{
				ActorView nextFreeSurvivor = viewActors.GetNextFreeSurvivor();
				if (nextFreeSurvivor == null || !nextFreeSurvivor.enabled || !nextFreeSurvivor.gameObject.activeInHierarchy)
				{
					break;
				}
				CampActorController component = nextFreeSurvivor.GetComponent<CampActorController>();
				if (viewActors.MoveCharacterImmediately)
				{
					Vector3 getVector3Position = componentsInChildren[i].GetVector3Position;
					nextFreeSurvivor.transform.position = getVector3Position;
				}
				component.GotoWaypoint(componentsInChildren[i], EndCondition, view, this);
			}
		}
	}

	public void SendSurvivorToBuildingWaypoints(BuildingView view, ActorView actorToSend)
	{
		CampWaypoint[] componentsInChildren = view.GetComponentsInChildren<CampWaypoint>();
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].Type == waypointType && !componentsInChildren[i].Occupied)
			{
				if (actorToSend == null || !actorToSend.enabled || !actorToSend.gameObject.activeInHierarchy)
				{
					break;
				}
				CampActorController component = actorToSend.GetComponent<CampActorController>();
				if (viewActors.MoveCharacterImmediately)
				{
					Vector3 getVector3Position = componentsInChildren[i].GetVector3Position;
					actorToSend.transform.position = getVector3Position;
				}
				component.GotoWaypoint(componentsInChildren[i], EndCondition, view, this);
			}
		}
	}
}
