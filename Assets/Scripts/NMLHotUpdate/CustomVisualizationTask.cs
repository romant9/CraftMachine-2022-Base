using System;

public class CustomVisualizationTask : VisualizationTask
{
	private readonly Action executionAction;

	public CustomVisualizationTask(Action action)
		: base(null)
	{
		executionAction = action;
	}

	public override void Start()
	{
		executionAction?.Invoke();
	}
}
