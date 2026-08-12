using TWDModel;

public class GooWalkerExplosionEffect : BaseExplosionBehavior
{
	public override bool Execute(TWDModelManager manager, WalkerExplosionDefinition explosionDefinition, ActorModel source, ActorModel target)
	{
		if (manager != null && target != null && source != null)
		{
			int turns = (int)explosionDefinition.GetParameter<FixedPoint>(4);
			if (!target.IsDead && IsFactionEffected(target.Faction, explosionDefinition))
			{
				return manager.ExecuteAction(new StunAction(source, target, turns, ignoreSourceBeingDead: true));
			}
		}
		return false;
	}
}
