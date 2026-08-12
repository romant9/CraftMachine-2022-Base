using TWDModel;

public interface ExplosionBehavior
{
	bool Execute(TWDModelManager manager, WalkerExplosionDefinition explosionDefinition, ActorModel source, ActorModel target);
}
