using TWDModel;
using UnityEngine;

public class BuildingUpgradeInsideIndicator : BuildingIndicator
{
	public int AnchorY;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UISprite iconSprite;

	protected BuildingView buildingView;

	public void SetBuildingView(BuildingView buildingView)
	{
		this.buildingView = buildingView;
		if (iconSprite != null && buildingView != null)
		{
			iconSprite.spriteName = HelpersGfx.GetBuildingIconName(buildingView.BuildingType);
		}
		if (!(titleLabel != null))
		{
			return;
		}
		if (buildingView is BuffBuildingView)
		{
			TraitDefinition traitDefinition = ((BuffBuildingModel)buildingView.Model).TraitDefinition;
			if (traitDefinition != null)
			{
				titleLabel.text = LocalizationManager.GetText("Indicator.UpgradeInside." + traitDefinition.Identifier + "{Value}", traitDefinition.GetParameter<int>(0));
			}
		}
		else if (buildingView is MedicTentView)
		{
			titleLabel.text = Helpers.FormatTimeNoZero(((MedicTentModel)buildingView.Model).TimedQueueModel.TotalTime);
		}
		else
		{
			titleLabel.text = LocalizationManager.GetText("Indicator.UpgradeInside." + buildingView.Model.TypeName);
		}
	}

	public void onClick()
	{
		if (buildingView != null && buildingView.BuildingGameObject != null && CampView.Instance != null && CampView.Instance.CampViewBuildings.SelectedBuilding != buildingView)
		{
			CampView.Instance.CampViewBuildings.SelectBuilding(buildingView.BuildingGameObject);
		}
	}

	private void Update()
	{
		if (buildingView is MedicTentView)
		{
			titleLabel.text = Helpers.FormatTimeNoZero(((MedicTentModel)buildingView.Model).TimedQueueModel.TotalTime);
		}
	}
}
