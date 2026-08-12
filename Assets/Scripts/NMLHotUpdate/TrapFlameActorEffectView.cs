using System;

public class TrapFlameActorEffectView : CombatAreaActorEffectView
{
	public override void StartKill(Action killEndAction)
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.StartKill(killEndAction);
		}));
	}
}
