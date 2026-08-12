using System;

public class PitfallActorEffectView : CombatAreaActorEffectView
{
	public override void StartKill(Action killEndAction)
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.StartKill(killEndAction);
		}));
	}
}
