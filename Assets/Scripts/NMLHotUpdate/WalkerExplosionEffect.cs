using TWDModel;

public class WalkerExplosionEffect : BaseExplosionBehavior
{
	public override bool Execute(TWDModelManager manager, WalkerExplosionDefinition explosionDefinition, ActorModel source, ActorModel target)
	{
		if (manager != null && target != null && !target.IsDead && IsFactionEffected(target.Faction, explosionDefinition))
		{
			FixedPoint parameter = explosionDefinition.GetParameter<FixedPoint>(2);
			int randomInRange = manager.Player.PlayerRandom.GetRandomInRange(1, 100);
			int dmg = source.CalculateExplosionDamage(explosionDefinition);
			if (randomInRange <= parameter && !target.HasTrait("Burning"))
			{
				manager.ExecuteAction(new BurningOutAction(source, target, onRedHealthBar: false, null, () => dmg));
			}
		}
		return false;
	}
}
