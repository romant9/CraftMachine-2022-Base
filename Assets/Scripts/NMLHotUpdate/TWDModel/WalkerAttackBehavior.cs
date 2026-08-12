using System.Collections.Generic;

namespace TWDModel
{
	public class WalkerAttackBehavior : BehaviorBase
	{
		public WalkerAttackBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (!base.Actor.AbilityCompleted && !base.Actor.IsStunned && !base.Actor.IsEatingLure && !base.Actor.IsElectricShocked && !base.Actor.IsQuantunCanNotMove)
			{
				ActorModel actorModel = base.AIDataModel.GetCurrentTarget();
				if (actorModel != null)
				{
					CombatModel combatModel = base.Controller.CombatModel;
					if (!combatModel.CanTraverse(null, base.Actor.GridCoordinate, actorModel.GridCoordinate))
					{
						List<ActorModel> attackTargetsInAttackRange = AIBehaviorHelpers.GetAttackTargetsInAttackRange(base.Actor, combatModel);
						actorModel = ((attackTargetsInAttackRange.Count <= 0) ? null : attackTargetsInAttackRange[0]);
					}
				}
				if (actorModel == null || base.Actor.IsHerded)
				{
					return 0;
				}
				return 100;
			}
			return 0;
		}

		public override void ExecuteAction()
		{
			ActorModel currentTarget = base.AIDataModel.GetCurrentTarget();
			if (currentTarget != null && (!currentTarget.IsDead || currentTarget.Faction == Faction.Lure) && currentTarget.IsEnemy(base.Actor) && !base.Actor.IsFistSpike)
			{
				EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
				if (weaponEquipment != null)
				{
					AbilityCommand.PerformActions(base.Actor.manager, base.Actor, weaponEquipment.Ability, currentTarget.GridCoordinate);
				}
				else
				{
					base.Actor.manager.Debug.LogWarning("Walker '" + base.Actor?.ToString() + "' tried to attack but did not have valid weapon equipment!");
				}
				base.Actor.EndAction();
				base.Controller.IsStuck(enabled: false);
			}
			else
			{
				base.Actor.manager.Debug.LogWarning("Walker '" + base.Actor?.ToString() + "' tried to attack but could not find valid target!");
			}
		}
	}
}
