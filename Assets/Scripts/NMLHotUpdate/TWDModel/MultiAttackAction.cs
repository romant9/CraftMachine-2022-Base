using BaseModel;

namespace TWDModel
{
	public class MultiAttackAction : GenericAbilityAction
	{
		public FixedPoint Multiplier;

		public MultiAttackAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, FixedPoint damageMultiplier)
			: base(sourceActor, ability, targetCell, "ActorNotification.MultiAttacks", targetActor, OOTType.MultiAttacks, skipActiveWeaponTraits: true)
		{
			Multiplier = damageMultiplier;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute())
			{
				return !(base.TargetActor?.IsDead ?? true);
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			base.Actor.RemoveTrait("MultiAttackExtraDamageActive");
			base.Actor.AddTemporaryTrait("MultiAttackExtraDamageActive", Multiplier - 100.0, null, 1L);
			bool result = base.Execute(manager);
			base.Actor.RemoveTrait("MultiAttackExtraDamageActive");
			return result;
		}
	}
}
