using BaseModel;
using TWDModel;
using UnityEngine;

[RequireComponent(typeof(BuildingInfoStatistics))]
public class BuildingInfoPopup : HUDElement
{
	[SerializeField]
	private UILabel buildingNameLabel;

	[SerializeField]
	private UILabel buildingLevelLabel;

	[SerializeField]
	private UILabel buildingDescriptionLabel;

	[SerializeField]
	private UILabel buildingDestructionTimerLabel;

	[SerializeField]
	private UITexture buildingImage;

	private BuildingModel building;

	public override void Open()
	{
		base.Open();
		building = GetModel<BuildingModel>();
		building.Changed += OnModelChange;
		UpdateGUI();
		GetComponent<BuildingInfoStatistics>().CreateStatistics(building, showUpgrade: false);
	}

	public override void Close()
	{
		base.Close();
		if (building != null)
		{
			building.Changed -= OnModelChange;
		}
		buildingImage.mainTexture = null;
		BuildingPhotoManager.Instance.RemoveAll();
	}

	private void UpdateGUI()
	{
		buildingNameLabel.text = HelpersLocalization.GetBuildingName(building);
		buildingLevelLabel.text = HelpersBuilding.GetLocalizedBuildingLevel(building);
		if (buildingDescriptionLabel != null)
		{
			buildingDescriptionLabel.text = LocalizationManager.GetText("Building.Description." + building.TypeName);
		}
		BuildingView buildingView = CampView.Instance.CampViewBuildings.FindBuildingView(building);
		buildingImage.mainTexture = BuildingPhotoManager.Instance.GetBuildingPhoto(buildingView.BuildingType, buildingView.Model.MaxUpgradeLevel);
		if (building is BuffBuildingModel)
		{
			buildingDestructionTimerLabel.gameObject.SetActive(value: true);
			buildingLevelLabel.gameObject.SetActive(value: false);
		}
		else
		{
			buildingDestructionTimerLabel.gameObject.SetActive(value: false);
			buildingLevelLabel.gameObject.SetActive(value: true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (building is BuffBuildingModel && buildingDestructionTimerLabel != null)
		{
			buildingDestructionTimerLabel.gameObject.SetActive(value: false);
		}
	}

	protected void OnModelChange(ModelObject model, string changed, object args)
	{
		if (changed == "RemoveBuilding" && model == building)
		{
			Close();
		}
	}
}
