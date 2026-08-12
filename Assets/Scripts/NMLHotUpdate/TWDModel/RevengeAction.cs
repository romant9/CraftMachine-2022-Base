using BaseModel;

namespace TWDModel
{
	public class RevengeAction : AbilityAction
	{
		public ActorModel RevengedActor { get; set; }

		private FixedPoint multiplier { get; set; }

		public RevengeAction(ActorModel sourceActor, ActorModel revengedActor, AbilityModel ability, GridCoordinate targetCell, FixedPoint multiplier, ActorModel targetActor = null, OOTType ootType = OOTType.None, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, targetActor, ootType, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage)
		{
			RevengedActor = revengedActor;
			this.multiplier = multiplier;
			sortOrder = 6;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute() && !base.TargetActor.IsDead && !base.Actor.RevengedOnTurn)
			{
				return base.Actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.Actor.manager.CombatModel, base.Actor, base.Actor.GridCoordinate, base.TargetActor.GridCoordinate) == AbilityResult.Success;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			base.Actor.SelectedEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, multiplier * 100.0);
			return base.Actor.RevengedOnTurn = base.Execute(manager);
		}
	}
}
