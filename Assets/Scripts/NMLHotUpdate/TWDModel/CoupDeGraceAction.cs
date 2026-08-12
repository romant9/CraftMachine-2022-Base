using BaseModel;

namespace TWDModel
{
	public class CoupDeGraceAction : GenericAbilityAction
	{
		public CoupDeGraceAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, "ActorNotification.CoupDeGrace", targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: true, isAssistAttack: false, isTriggerExtraAttackDamage)
		{
		}

		public override bool CanExecute()
		{
			if (base.CanExecute())
			{
				ActorModel targetActor = base.TargetActor;
				if (targetActor != null && !targetActor.IsDead)
				{
					return base.Ability.CanAbilityBePerformedOnGridCell(base.Actor.manager.CombatModel, base.Actor, base.Actor.GridCoordinate, base.TargetCell) == AbilityResult.Success;
				}
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			base.Actor.FollowUpAttackedOnTurn = true;
			base.Actor.GetWeaponEquipment().AddTemporaryTrait("CoupDeGraceActive", TraitExpirationType.Activation, 1.0);
			return base.Execute(manager);
		}
	}
}
