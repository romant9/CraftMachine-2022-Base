public class SufferAreaView : CombatAreaView
{
	public override void Kill()
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.Kill();
		}));
	}
}
