using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierJumpingshot : ActionModifier
	{
		private FixedPoint JumpingshotChance = 0.0;

		private FixedPoint JumpingshotChanceMax = 0L;

		private FixedPoint JumpingshotChanceIncrease = 0.0;

		private FixedPoint JumpingshotChanceIncreaseMax = 0.0;

		public AbilityModifierJumpingshot()
		{
		}

		public AbilityModifierJumpingshot(FixedPoint jumpingshotChance, FixedPoint jumpingshotChanceMax, FixedPoint jumpingshotChanceIncrease, FixedPoint jumpingshotChanceIncreaseMax)
		{
			JumpingshotChance = jumpingshotChance;
			JumpingshotChanceMax = jumpingshotChanceMax;
			JumpingshotChanceIncrease = jumpingshotChanceIncrease;
			JumpingshotChanceIncreaseMax = jumpingshotChanceIncreaseMax;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction damageAction && !(action is DamageConsumableAction))
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (damageAction.BaseDamage > 0 && damageAction.TargetActor == actor && combatModel != null && !combatModel.MissionCompleted)
				{
					PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
					if (!actor.IsStunned && !actor.IsStruggling)
					{
						FixedPoint successProbabilityExtension = 0.0;
						FixedPoint fixedPoint = 0.0;
						if (damageAction.DamageType == DamageType.Ranged)
						{
							if (JumpingshotChance > 0L)
							{
								fixedPoint = JumpingshotChance;
							}
							if (JumpingshotChanceIncrease > 0L && damageAction.DamagerActor != null)
							{
								GridCoordinate gridCoordinate = damageAction.DamagerActor.GridCoordinate;
								GridCoordinate closestOccupiedCell = damageAction.TargetActor.GetClosestOccupiedCell(gridCoordinate);
								fixedPoint = ((!(gridCoordinate.DistanceTo(closestOccupiedCell) > JumpingshotChanceIncrease)) ? JumpingshotChance : JumpingshotChanceIncreaseMax);
							}
							if (fixedPoint > JumpingshotChanceMax)
							{
								fixedPoint = JumpingshotChanceMax;
							}
						}
						if (fixedPoint <= 0L)
						{
							fixedPoint = 0L;
						}
						if (damageAction.DamagerActor != null && damageAction.DamagerActor.HasAnyLevelTrait("ResistJumpingshot"))
						{
							FixedPoint value = 0.0;
							base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierPercentageReduceJumpingshotDamage", ref value, damageAction.DamagerActor);
							fixedPoint = FixedPoint.Max(fixedPoint * (1L - value), 0L);
						}
						if (fixedPoint > 0.0)
						{
							playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Dodge, fixedPoint, successProbabilityExtension);
						}
					}
					if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
					{
						damageAction.Jumpingshot = true;
						damageAction.TargetActor.NotifyChange("AbilityVisited", new object[2] { "Jumpingshot", false });
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.Jumpingshot(damageAction.DamagerActor, damageAction.TargetActor);
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
