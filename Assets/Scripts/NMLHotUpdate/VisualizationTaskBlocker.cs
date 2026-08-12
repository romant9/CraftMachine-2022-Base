public class VisualizationTaskBlocker : VisualizationTask
{
	public override bool IsGlobalBlocker => true;

	public VisualizationTaskBlocker()
		: base(null)
	{
	}
}
