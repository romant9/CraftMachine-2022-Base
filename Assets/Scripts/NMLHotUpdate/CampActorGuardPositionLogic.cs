using System.Collections.Generic;

public class CampActorGuardPositionLogic : CampActorLogic
{
	public override void Initialize()
	{
		InitializeInternal(WaypointType.GuardPosition);
		Refresh();
	}

	public override void OnEnable()
	{
		Refresh();
	}

	public override void OnDestroy()
	{
	}

	public void Refresh()
	{
		List<BuildingView> buildings = CampView.Instance.CampViewBuildings.Buildings;
		for (int i = 0; i < buildings.Count; i++)
		{
		}
	}

	public override bool EndCondition(CampWaypoint waypoint, CampActorController campActorController)
	{
		return false;
	}
}
