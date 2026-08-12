namespace TWDModel
{
	public class RaiderAttackBehavior : BehaviorBase
	{
		public RaiderAttackBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.Controller.IsActorIncapacitated)
			{
				return 0;
			}
			if (!base.Actor.AbilityCompleted)
			{
				ActorModel actorModel = base.AIDataModel.GetCurrentTarget();
				if (actorModel != null)
				{
					EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
					if (weaponEquipment == null || weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(base.Controller.CombatModel, base.Actor, base.Actor.GridCoordinate, actorModel.GridCoordinate) != AbilityResult.Success)
					{
						actorModel = null;
					}
				}
				if (actorModel == null)
				{
					return 0;
				}
				return 100;
			}
			return 0;
		}

		public override void ExecuteAction()
		{
			TryAutoUseAbilityRangeTrident();
			ActorModel currentTarget = base.AIDataModel.GetCurrentTarget();
			if (currentTarget != null && !currentTarget.IsDead && currentTarget.IsEnemy(base.Actor) && !base.Actor.IsFistSpike && !currentTarget.IsSneak && !base.Actor.AbilityCompleted)
			{
				EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
				if (weaponEquipment != null)
				{
					AbilityCommand.PerformActions(base.Actor.manager, base.Actor, weaponEquipment.Ability, currentTarget.GridCoordinate);
				}
				else
				{
					base.Actor.manager.Debug.LogWarning("Raider '" + base.Actor?.ToString() + "' tried to attack but did not have valid weapon equipment!");
				}
				base.Actor.EndAction();
				base.Controller.IsStuck(enabled: false);
			}
			else if (base.Actor.AbilityCompleted || base.Actor.MoveCompleted)
			{
				base.Actor.EndAction();
				base.Controller.IsStuck(enabled: false);
			}
			else
			{
				base.Actor.manager.Debug.LogWarning("Raider '" + base.Actor?.ToString() + "' tried to attack but could not find valid target!");
			}
		}

		private bool TryAutoUseAbilityRangeTrident()
		{
			AbilityRangeTridentSkill abilityRangeTridentSkill = GetAbilityRangeTridentSkill();
			if (abilityRangeTridentSkill == null || abilityRangeTridentSkill.IsActive)
			{
				return false;
			}
			return abilityRangeTridentSkill.ReleaseSkillToTargetCell(base.Actor.GridCoordinate);
		}

		private AbilityRangeTridentSkill GetAbilityRangeTridentSkill()
		{
			CommandSkillModelManager commandSkillModelManager = base.Actor.CommandSkillModelManager;
			if (commandSkillModelManager == null)
			{
				return null;
			}
			AbilityRangeTridentSkill abilityRangeTridentSkill = commandSkillModelManager.GetActorCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
			if (abilityRangeTridentSkill == null)
			{
				abilityRangeTridentSkill = commandSkillModelManager.GetCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
			}
			return abilityRangeTridentSkill;
		}
	}
}
