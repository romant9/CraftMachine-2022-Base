using BaseModel;

namespace TWDModel
{
	public class DeadlyFocusAction : GenericAbilityAction
	{
		public FixedPoint Multiplier;

		public DeadlyFocusAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, FixedPoint damageMultiplier, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, "ActorNotification.DeadlyFocus", targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: true, isTriggerExtraAttackDamage)
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
			base.Actor.GetWeaponEquipment().AddTemporaryTrait("DeadlyFocusEXDamageActive", TraitExpirationType.Activation, Multiplier);
			return base.Execute(manager);
		}
	}
}
