using BaseModel;

namespace TWDModel
{
	public class SpecialStunExtraDamageAction : GenericAbilityAction
	{
		public SpecialStunExtraDamageAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor)
			: base(sourceActor, ability, targetCell, "ActorNotification.SpecialStun", targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: true)
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
			return base.Execute(manager);
		}
	}
}
