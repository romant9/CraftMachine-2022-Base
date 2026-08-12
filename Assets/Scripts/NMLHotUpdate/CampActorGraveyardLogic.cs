using BaseModel;
using TWDModel;
using UnityEngine;

public class CampActorGraveyardLogic : CampActorLogic
{
	private static int numberOfSurvivorsAtGraveyard = -1;

	private bool survivorDied;

	private float mournBeginTime;

	public override void Initialize()
	{
		InitializeInternal(WaypointType.MournAtGraveyard);
		GameManager.Instance.playerModel.SurvivorContainer.Changed += OnSurvivorsChanged;
		Refresh();
	}

	public override void OnDestroy()
	{
		numberOfSurvivorsAtGraveyard = GameManager.Instance.playerModel.SurvivorContainer.DeadSurvivors.Count;
		GameManager.Instance.playerModel.SurvivorContainer.Changed -= OnSurvivorsChanged;
	}

	public override void OnEnable()
	{
		if (numberOfSurvivorsAtGraveyard != -1 && GameManager.Instance.playerModel.SurvivorContainer.DeadSurvivors.Count > numberOfSurvivorsAtGraveyard)
		{
			survivorDied = true;
		}
		Refresh();
	}

	public void Refresh()
	{
		if (survivorDied && CampView.Instance != null && CampView.Instance.gameObject.activeSelf)
		{
			mournBeginTime = Time.time;
			BuildingModel building = CampView.Instance.Model.GetBuilding("Graveyard");
			if (building != null)
			{
				BuildingView view = CampView.Instance.CampViewBuildings.FindBuildingView(building);
				SendSurvivorsToBuildingWaypoints(view);
			}
			survivorDied = false;
		}
	}

	public override bool EndCondition(CampWaypoint waypoint, CampActorController campActorController)
	{
		return Time.time > mournBeginTime + waypoint.GetTimeParameter;
	}

	private void OnSurvivorsChanged(ModelObject m, string changed, object args)
	{
		if (changed == "survivorDied")
		{
			survivorDied = true;
			Refresh();
		}
	}
}
