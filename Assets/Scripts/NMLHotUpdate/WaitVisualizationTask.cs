using TWDModel;
using UnityEngine;

public class WaitVisualizationTask : ActorVisualizationTask
{
	private bool CanSkipWithTap { get; set; }

	private float waitSeconds { get; set; }

	public WaitVisualizationTask(ActorModel actor, float seconds, bool canSkipWithTap = false)
		: base(null)
	{
		if (actor != null)
		{
			AddFactionDependency(actor.Faction, reserve: true);
			base.Actor = actor;
			base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		}
		waitSeconds = seconds;
		CanSkipWithTap = canSkipWithTap;
	}

	public override bool Update(float deltaTime)
	{
		waitSeconds -= deltaTime;
		if (CanSkipWithTap && Input.GetMouseButtonUp(0))
		{
			return false;
		}
		return waitSeconds > 0f;
	}
}
