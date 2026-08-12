using BaseModel;
using UnityEngine;

public class VegetationView : BuildingView
{
	protected override void ResetVisualization(bool updateBuildingGraphics = true)
	{
		base.ResetVisualization(updateBuildingGraphics);
		if ((base.Model as VegetationModel).IsBeingCut)
		{
			BuildingUpgradeIndicator buildingUpgradeIndicator = CampView.Instance.BuildingsHud.CreateUpgradeIndicator(this);
			buildingUpgradeIndicator.FollowTarget(base.BuildingGameObject);
			buildingUpgradeIndicator.SetType(UpgradeType.CutVegetation);
			indicators.Add(buildingUpgradeIndicator);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (CampView.Instance != null)
		{
			GameObject obj = Helpers.InstantiateToParent((base.Model.BuildingType.SizeX > 10 || base.Model.BuildingType.SizeY > 10) ? CampView.Instance.BuildingsHud.effectBigVegetationDisappear : CampView.Instance.BuildingsHud.effectVegetationDisappear, base.transform.parent.gameObject);
			obj.transform.position = base.transform.position;
			obj.SetActive(value: true);
		}
	}

	protected override void OnModelChange(ModelObject model, string changed, object args)
	{
		base.OnModelChange(model, changed, args);
		if (model == base.Model && changed == "ActionStartEvent")
		{
			ResetVisualization(updateBuildingGraphics: false);
			EventManager.NotifyEvent(EventManager.EventType.StartCutVegetation);
		}
	}
}
