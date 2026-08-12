using BaseModel;
using TWDModel;

public class CampActorUpgradeInWorkshopLogic : CampActorLogic
{
	public override void Initialize()
	{
		InitializeInternal(WaypointType.UpgradeInWorkshop);
		BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Workshop");
		if (building != null)
		{
			building.Changed += OnWorkshopChanged;
		}
		Refresh();
	}

	public override void OnEnable()
	{
		Refresh();
	}

	public override void OnDestroy()
	{
		BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Workshop");
		if (building != null)
		{
			building.Changed -= OnWorkshopChanged;
		}
	}

	public void Refresh()
	{
		if (GameManager.Instance.playerModel.Camp.GetBuilding("Workshop") is WorkshopBuildingModel { UpgradingModel: not null } workshopBuildingModel)
		{
			BuildingView view = CampView.Instance.CampViewBuildings.FindBuildingView(workshopBuildingModel);
			SendSurvivorsToBuildingWaypoints(view);
		}
	}

	public override bool EndCondition(CampWaypoint waypoint, CampActorController campActorController)
	{
		if (GameManager.Instance.playerModel.Camp.GetBuilding("Workshop") is WorkshopBuildingModel workshopBuildingModel)
		{
			return workshopBuildingModel.UpgradingModel == null;
		}
		return true;
	}

	private void OnWorkshopChanged(ModelObject m, string changed, object args)
	{
		if (changed == "buildItem")
		{
			BuildingModel model = m as BuildingModel;
			BuildingView view = CampView.Instance.CampViewBuildings.FindBuildingView(model);
			SendSurvivorsToBuildingWaypoints(view);
		}
	}
}
