using BaseModel;

namespace TWDModel
{
	public sealed class EquipmentPassiveFightBackAction : AbilityAction
	{
		private readonly FixedPoint damageMultiplier;

		private readonly int maxTriggersPerRound;

		public bool TriggeredByLuck { get; set; }

		public EquipmentPassiveFightBackAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, FixedPoint damageMultiplier, int maxTriggersPerRound)
			: base(sourceActor, ability, targetCell, targetActor, OOTType.FightBack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true)
		{
			this.damageMultiplier = damageMultiplier;
			this.maxTriggersPerRound = maxTriggersPerRound;
		}

		public override bool CanExecute()
		{
			if (!base.CanExecute() || base.Actor.IsEnvironmental || base.Actor.FightBackTimesThisRound >= maxTriggersPerRound || maxTriggersPerRound <= 0 || base.TargetActor == null || base.TargetActor.IsDead || base.TargetActor.Faction == Faction.Lure || !base.Actor.IsEnemy(base.TargetActor))
			{
				return false;
			}
			EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
			CombatModel combatModel = base.Actor.manager?.CombatModel;
			if (weaponEquipment?.Ability == null || weaponEquipment.Ability != base.Ability || weaponEquipment.NeedsReloading || weaponEquipment.LimitOOT || base.Actor.IsInvisible || base.Actor.dashTraitAttackFlag || base.Actor.AIController == null || !base.Actor.AIController.CanPerformOOT || base.Actor.AIController.IsActorIncapacitated || (base.Actor.ExclusiveTimedEffect != null && base.Actor.ExclusiveTimedEffect.Type != TimedEffectType.Root) || combatModel == null)
			{
				return false;
			}
			return weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(combatModel, base.Actor, base.Actor.GridCoordinate, base.TargetCell) == AbilityResult.Success;
		}

		protected override AbilityModel GetOOTValidationAbility()
		{
			return base.Ability;
		}

		public override bool Execute(ModelManager manager)
		{
			if (!CanExecute())
			{
				return false;
			}
			EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
			if (weaponEquipment == null)
			{
				return false;
			}
			base.Actor.FightBackTimesThisRound++;
			bool flag = false;
			try
			{
				weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, damageMultiplier * 100.0);
				flag = base.Execute(manager);
				if (!flag)
				{
					return false;
				}
				base.Actor.NotifyChange("AbilityVisited", new object[2] { "Equipment.Passive.FightBack", TriggeredByLuck });
				return true;
			}
			finally
			{
				if (!flag)
				{
					base.Actor.FightBackTimesThisRound--;
				}
			}
		}
	}
}
