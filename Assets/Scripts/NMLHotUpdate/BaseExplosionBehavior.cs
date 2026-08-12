using System.Collections.Generic;
using TWDModel;

public abstract class BaseExplosionBehavior : ExplosionBehavior
{
	public abstract bool Execute(TWDModelManager manager, WalkerExplosionDefinition explosionDefinition, ActorModel source, ActorModel target);

	public bool IsFactionEffected(Faction faction, WalkerExplosionDefinition definition)
	{
		return ((definition.EffectedFactions == null) ? new List<Faction> { Faction.Any } : definition.EffectedFactions)?.Contains(faction) ?? false;
	}
}
