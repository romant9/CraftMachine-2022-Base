public class FactionChangeVisualizationTask : ActorVisualizationTask
{
	public FactionChangeVisualizationTask()
		: base(null)
	{
		AddGlobalDependency();
	}

	public override bool Update(float deltaTime)
	{
		return false;
	}
}
