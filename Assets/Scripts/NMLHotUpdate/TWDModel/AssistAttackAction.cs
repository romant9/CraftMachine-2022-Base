using BaseModel;

namespace TWDModel
{
	public class AssistAttackAction : GenericAbilityAction
	{
		public FixedPoint Multiplier;

		public AssistAttackAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, FixedPoint damageMultiplier, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, "ActorNotification.AssistAttack", targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: true, isTriggerExtraAttackDamage)
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
			base.Actor.AddTemporaryTrait("EquipmentActiveAssistAttackActive", Multiplier - 100.0, null, 1L);
			return base.Execute(manager);
		}
	}
}
