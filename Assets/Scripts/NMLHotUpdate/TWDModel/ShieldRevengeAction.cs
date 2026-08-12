using BaseModel;

namespace TWDModel
{
	public class ShieldRevengeAction : AbilityAction
	{
		private FixedPoint Multiplier { get; set; }

		private int MaxShieldRevengedTimesOnTurn { get; set; }

		public ShieldRevengeAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, FixedPoint multiplier, int maxShieldRevengedTimesOnTurn, ActorModel targetActor = null, OOTType ootType = OOTType.None, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, targetActor, ootType, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage)
		{
			Multiplier = multiplier;
			MaxShieldRevengedTimesOnTurn = maxShieldRevengedTimesOnTurn;
			sortOrder = 7;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute() && !base.TargetActor.IsDead && base.Actor.ShieldRevengedTimesOnTurn < MaxShieldRevengedTimesOnTurn)
			{
				return base.Actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.Actor.manager.CombatModel, base.Actor, base.Actor.GridCoordinate, base.TargetActor.GridCoordinate) == AbilityResult.Success;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			base.Actor.SelectedEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, Multiplier * 100.0);
			if (base.Execute(manager))
			{
				base.Actor.ShieldRevengedTimesOnTurn++;
				return true;
			}
			return false;
		}
	}
}
