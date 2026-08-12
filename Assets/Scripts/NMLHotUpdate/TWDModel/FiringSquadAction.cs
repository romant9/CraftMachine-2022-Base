using BaseModel;

namespace TWDModel
{
	public class FiringSquadAction : GenericAbilityAction
	{
		public FixedPoint Multiplier;

		public FiringSquadAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, FixedPoint damageMultiplier, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, "ActorNotification.FiringSquad", targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage)
		{
			Multiplier = damageMultiplier;
		}

		public override bool CanExecute()
		{
			ActorModel actorModel = base.Actor?.manager?.CombatModel?.GetOccupier(base.TargetCell);
			if (base.CanExecute())
			{
				return !(actorModel?.IsDead ?? true);
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			base.Actor.GetWeaponEquipment().AddTemporaryTrait("FiringSquadDamageActive", TraitExpirationType.Activation, Multiplier - 100.0);
			return base.Execute(manager);
		}
	}
}
