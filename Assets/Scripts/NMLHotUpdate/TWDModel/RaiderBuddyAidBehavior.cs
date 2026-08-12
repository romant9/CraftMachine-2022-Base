namespace TWDModel
{
	public class RaiderBuddyAidBehavior : BehaviorBase
	{
		public RaiderBuddyAidBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.AIDataModel.GetModelReference<ActorModel>(AIDataModel.BuddyAidTarget) != null)
			{
				return 100;
			}
			return 0;
		}

		public override void ExecuteAction()
		{
			ActorModel modelReference = base.AIDataModel.GetModelReference<ActorModel>(AIDataModel.BuddyAidTarget);
			if (modelReference == null)
			{
				return;
			}
			TimedEffect exclusiveTimedEffect = modelReference.ExclusiveTimedEffect;
			if (exclusiveTimedEffect.Type == TimedEffectType.Struggle)
			{
				ActorModel instigator = exclusiveTimedEffect.Instigator;
				EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
				if (weaponEquipment != null)
				{
					if (weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(base.CombatModel, base.Actor, base.Actor.GridCoordinate, instigator.GridCoordinate) == AbilityResult.Success)
					{
						AbilityCommand.PerformActions(base.Actor.manager, base.Actor, weaponEquipment.Ability, instigator.GridCoordinate);
						base.Actor.EndAction();
						return;
					}
					GridPath pathTowardsTarget = GridHelpers.GetPathTowardsTarget(base.Actor, base.CombatModel, instigator.GridCoordinate, base.Actor.MoveRange + 1);
					if (pathTowardsTarget.IsValid)
					{
						MoveCommand.PerformActions(base.Actor.manager, base.Actor, pathTowardsTarget);
						if (weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(base.CombatModel, base.Actor, base.Actor.GridCoordinate, instigator.GridCoordinate) == AbilityResult.Success)
						{
							AbilityCommand.PerformActions(base.Actor.manager, base.Actor, weaponEquipment.Ability, instigator.GridCoordinate);
							base.Actor.EndAction();
						}
						else
						{
							base.Actor.EndMovement();
						}
					}
				}
				else
				{
					base.Actor.manager.Debug.LogWarning("Raider [" + base.Actor?.ToString() + "] could not help struggling buddy [" + modelReference?.ToString() + "] - actor has no weapon!");
					base.Actor.EndAction();
				}
			}
			else
			{
				if (exclusiveTimedEffect.Type != TimedEffectType.BleedOut)
				{
					return;
				}
				GridCoordinate gridCoordinate = modelReference.GridCoordinate;
				GridPath pathTowardsTarget2 = GridHelpers.GetPathTowardsTarget(base.Actor, base.CombatModel, gridCoordinate, base.Actor.MoveRange + 1);
				if (pathTowardsTarget2.IsValid)
				{
					MoveCommand.PerformActions(base.Actor.manager, base.Actor, pathTowardsTarget2);
					if (base.CombatModel.Grid.AreNeighbors(pathTowardsTarget2.End, gridCoordinate))
					{
						modelReference.FinishTimedEffect(interrupted: true);
						modelReference.NotifyBleedingOutFinished();
						base.Actor.EndAction();
					}
					else
					{
						base.Actor.EndMovement();
					}
				}
			}
		}
	}
}
