using System.Collections.Generic;

public class CampActorCutVegetationLogic : CampActorLogic
{
	public override void Initialize()
	{
		InitializeInternal(WaypointType.CutVegetation);
		EventManager.OnEvent += OnEvent;
		Refresh();
	}

	public override void OnEnable()
	{
		Refresh();
	}

	public override void OnDestroy()
	{
		EventManager.OnEvent -= OnEvent;
	}

	public void Refresh()
	{
		List<BuildingView> buildings = CampView.Instance.CampViewBuildings.Buildings;
		for (int i = 0; i < buildings.Count; i++)
		{
			if (buildings[i].Model is VegetationModel { IsBeingCut: not false })
			{
				SendSurvivorsToBuildingWaypoints(buildings[i]);
				break;
			}
		}
	}

	public override bool EndCondition(CampWaypoint waypoint, CampActorController campActorController)
	{
		if (waypoint == null || waypoint.GameObjectCached == null)
		{
			return true;
		}
		BuildingView buildingView = HelpersBuilding.GetBuildingView(waypoint.GameObjectCached);
		if (buildingView == null)
		{
			return true;
		}
		if (buildingView.Model is VegetationModel vegetationModel)
		{
			return !vegetationModel.IsBeingCut;
		}
		return true;
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype == EventManager.EventType.StartCutVegetation)
		{
			Refresh();
		}
	}
}
