using TWDModel;

public class EndStunVisualizationTask : ActorVisualizationTask
{
	private bool isDead;

	public EndStunVisualizationTask(ActorModel actor)
		: base(null, affectsCovers: true)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		AddFactionDependency(base.Actor.Faction);
		AddDependency(base.Actor, reserve: false);
		isDead = base.Actor.IsDead;
	}

	public override bool Update(float deltaTime)
	{
		if (!isDead)
		{
			base.ActorView.EndStun();
		}
		return false;
	}
}
