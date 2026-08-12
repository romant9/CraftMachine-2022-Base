public class BuildingIndicator : HUDElementFollowTarget
{
	public string Type;

	public BuildingView Building { get; set; }

	public void FollowBasicIndicatorPosition(BuildingView buildingView)
	{
		UIIndicatorsPosition component = buildingView.BuildingGameObject.GetComponent<UIIndicatorsPosition>();
		if (component != null && component.BasicIndicatorPosition != null)
		{
			FollowTarget(component.BasicIndicatorPosition);
		}
		else
		{
			FollowTarget(buildingView.BuildingGameObject);
		}
	}

	public void FollowUpgradeAvailableIndicatorPosition(BuildingView buildingView)
	{
		UIIndicatorsPosition component = buildingView.BuildingGameObject.GetComponent<UIIndicatorsPosition>();
		if (component != null && component.UpgradeAvailableIndicatorPosition != null)
		{
			FollowTarget(component.UpgradeAvailableIndicatorPosition);
		}
		else
		{
			FollowTarget(buildingView.BuildingGameObject);
		}
	}
}
