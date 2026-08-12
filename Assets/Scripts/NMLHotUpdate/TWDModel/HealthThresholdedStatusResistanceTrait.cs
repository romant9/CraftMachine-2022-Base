using System.Collections.Generic;

namespace TWDModel
{
	public class HealthThresholdedStatusResistanceTrait : ActionModifier
	{
		private ICollection<KeyValuePair<FixedPoint, FixedPoint>> healthThresholdAvoidanceChances;

		public HealthThresholdedStatusResistanceTrait(ICollection<KeyValuePair<FixedPoint, FixedPoint>> healthThresholdAvoidanceChancePairs)
		{
			healthThresholdAvoidanceChances = healthThresholdAvoidanceChancePairs;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!(action is StunAction stunAction))
			{
				if (!(action is RootAction rootAction))
				{
					if (!(action is CrippleAction crippleAction))
					{
						if (!(action is BurningOutAction burningOutAction))
						{
							if (!(action is StaggerAction staggerAction))
							{
								if (!(action is HerdAction herdAction))
								{
									if (!(action is DisorientAction disorientAction))
									{
										if (action is ABTesterAction aBTesterAction && actor == aBTesterAction.TargetActor && RollActivation(actor, RollDiceType.AvoidStun, aBTesterAction.DamageDealt))
										{
											aBTesterAction.Avoided = true;
										}
									}
									else if (actor == disorientAction.TargetActor && RollActivation(actor, RollDiceType.AvoidStun, disorientAction.DamageDealt))
									{
										disorientAction.Avoided = true;
									}
								}
								else if (actor == herdAction.TargetActor && RollActivation(actor, RollDiceType.AvoidHerd, herdAction.DamageDealt))
								{
									herdAction.Avoided = true;
								}
							}
							else if (actor == staggerAction.TargetActor && RollActivation(actor, RollDiceType.AvoidStagger, staggerAction.DamageDealt))
							{
								staggerAction.Avoided = true;
							}
						}
						else if (actor == burningOutAction.TargetActor && RollActivation(actor, RollDiceType.AvoidBurn, burningOutAction.DamageDealt))
						{
							burningOutAction.Avoided = true;
						}
					}
					else if (actor == crippleAction.TargetActor && RollActivation(actor, RollDiceType.AvoidCripple, crippleAction.DamageDealt))
					{
						crippleAction.Avoided = true;
					}
				}
				else if (actor == rootAction.TargetActor && RollActivation(actor, RollDiceType.AvoidRoot, rootAction.DamageDealt))
				{
					rootAction.Avoided = true;
				}
			}
			else if (actor == stunAction.TargetActor && RollActivation(actor, RollDiceType.AvoidStun, stunAction.DamageDealt) && stunAction.CanNotAvoidStunType == CanNotAvoidStunType.None)
			{
				stunAction.Avoided = true;
			}
			return ActionListClearFlag.Keep;
		}

		private bool RollActivation(ActorModel actor, RollDiceType rollDiceType, int damageDealtPrior)
		{
			FixedPoint fixedPoint = FixedPoint.Min((FixedPoint)(actor.Hitpoints + damageDealtPrior) / (FixedPoint)actor.MaxHitPoints, 1.0);
			foreach (KeyValuePair<FixedPoint, FixedPoint> healthThresholdAvoidanceChance in healthThresholdAvoidanceChances)
			{
				if (fixedPoint >= healthThresholdAvoidanceChance.Key)
				{
					return actor.manager.Player.RollDice(rollDiceType, healthThresholdAvoidanceChance.Value) != PlayerRandomChanceResult.Failed;
				}
			}
			return false;
		}
	}
}
