using System;

public class SufferActorEffectView : CombatAreaActorEffectView
{
	public override void StartKill(Action killEndAction)
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.StartKill(killEndAction);
		}));
	}
}
