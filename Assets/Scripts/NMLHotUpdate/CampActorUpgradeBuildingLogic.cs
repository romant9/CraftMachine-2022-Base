using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class CampActorUpgradeBuildingLogic : CampActorLogic
{
	private BuildingModel cachedBuildingModel;

	public override void Initialize()
	{
		InitializeInternal(WaypointType.UpgradeBuilding);
		GameManager.Instance.playerModel.Camp.Changed += OnCampChanged;
		Refresh();
	}

	public override void OnEnable()
	{
		Refresh();
	}

	public override void OnDestroy()
	{
		GameManager.Instance.playerModel.Camp.Changed -= OnCampChanged;
	}

	public void Refresh()
	{
		List<BuildingView> buildings = CampView.Instance.CampViewBuildings.Buildings;
		for (int i = 0; i < buildings.Count; i++)
		{
			if (buildings[i].Model != null && buildings[i].Model.IsUpgrading)
			{
				SendSurvivorsToBuildingWaypoints(buildings[i]);
				cachedBuildingModel = buildings[i].Model;
				break;
			}
		}
	}

	public override bool EndCondition(CampWaypoint waypoint, CampActorController campActorController)
	{
		if (cachedBuildingModel != null)
		{
			return !cachedBuildingModel.IsUpgrading;
		}
		return true;
	}

	private void OnCampChanged(ModelObject m, string changed, object args)
	{
		if (changed == "EventUpgradeBuilding")
		{
			cachedBuildingModel = args as BuildingModel;
			BuildingView view = CampView.Instance.CampViewBuildings.FindBuildingView(cachedBuildingModel);
			SendSurvivorsToBuildingWaypoints(view);
		}
	}
}
