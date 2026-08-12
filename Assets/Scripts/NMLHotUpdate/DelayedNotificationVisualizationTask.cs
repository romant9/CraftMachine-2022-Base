using System;
using TWDModel;

public class DelayedNotificationVisualizationTask : VisualizationTask
{
	private bool globallyBlocking { get; set; }

	public override bool IsGlobalBlocker => globallyBlocking;

	private Action DelayedNotification { get; set; }

	public DelayedNotificationVisualizationTask(ActorModel actor, Action delayedNotification, bool addDependencyToOtherActors = false)
		: base(null)
	{
		if (actor != null)
		{
			AddFactionDependency(actor.Faction);
			AddActorDependency(actor);
			globallyBlocking = false;
		}
		else
		{
			globallyBlocking = true;
		}
		DelayedNotification = delayedNotification;
		if (addDependencyToOtherActors)
		{
			AddDependencyToAllActors(reserve: true, actor);
		}
	}

	public override bool Update(float deltaTime)
	{
		DelayedNotification();
		return false;
	}
}
