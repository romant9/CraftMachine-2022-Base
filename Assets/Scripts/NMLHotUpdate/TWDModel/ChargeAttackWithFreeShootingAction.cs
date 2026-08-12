using BaseModel;

namespace TWDModel
{
	public class ChargeAttackWithFreeShootingAction : GenericAbilityAction
	{
		public FixedPoint Multiplier;

		public ChargeAttackWithFreeShootingAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, FixedPoint damageMultiplier)
			: base(sourceActor, ability, targetCell, "ActorNotification.ChargeAttackWithFreeShooting", targetActor, OOTType.FreeShooting)
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
			base.Actor.NotifyChange("AbilityVisited", new object[2] { "ChargeAttackWithFreeShooting", false });
			base.Actor.GetWeaponEquipment()?.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, Multiplier * 100.0);
			return base.Execute(manager);
		}
	}
}
