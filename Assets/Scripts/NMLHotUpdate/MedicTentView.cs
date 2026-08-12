using BaseModel;

public class MedicTentView : BuildingView
{
	protected override void ResetVisualization(bool updateBuildingGraphics = true)
	{
		base.ResetVisualization(updateBuildingGraphics);
	}

	protected override void OnModelChange(ModelObject model, string changed, object args)
	{
		base.OnModelChange(model, changed, args);
		if (changed == "EventStatusUpdated")
		{
			ResetVisualization(updateBuildingGraphics: false);
		}
	}
}
