using System.Collections.Generic;

namespace TWDModel
{
	public class ResistDebuffTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is StunAction stunAction)
			{
				if (ShouldAvoid(actor, stunAction.TargetActor, RollDiceType.AvoidStun))
				{
					stunAction.Avoided = true;
				}
			}
			else if (action is BleedingOutAction bleedingOutAction)
			{
				if (ShouldAvoid(actor, bleedingOutAction.Target, RollDiceType.AvoidBleed))
				{
					bleedingOutAction.Avoided = true;
					DropToRedHealthIfNecessary(actor);
				}
			}
			else if (action is RootAction rootAction)
			{
				if (ShouldAvoid(actor, rootAction.TargetActor, RollDiceType.AvoidRoot))
				{
					rootAction.Avoided = true;
				}
			}
			else if (action is CrippleAction crippleAction)
			{
				if (ShouldAvoid(actor, crippleAction.TargetActor, RollDiceType.AvoidCripple))
				{
					crippleAction.Avoided = true;
				}
			}
			else if (action is BurningOutAction burningOutAction)
			{
				if (ShouldAvoid(actor, burningOutAction.TargetActor, RollDiceType.AvoidBurn))
				{
					burningOutAction.Avoided = true;
					DropToRedHealthIfNecessary(actor);
				}
			}
			else if (action is StaggerAction staggerAction)
			{
				if (ShouldAvoid(actor, staggerAction.TargetActor, RollDiceType.AvoidStagger))
				{
					staggerAction.Avoided = true;
				}
			}
			else if (action is ElectricShockAction electricShockAction)
			{
				if (ShouldAvoid(actor, electricShockAction.TargetActor, RollDiceType.AvoidElectricShock))
				{
					electricShockAction.Avoided = true;
				}
			}
			else if (action is QuantunAction quantunAction && ShouldAvoid(actor, quantunAction.TargetActor, RollDiceType.AvoidQuantun))
			{
				quantunAction.Avoided = true;
			}
			return ActionListClearFlag.Keep;
		}

		private static bool ShouldAvoid(ActorModel traitHolder, ActorModel target, RollDiceType diceType)
		{
			if (traitHolder != target)
			{
				return false;
			}
			TWDModelManager tWDModelManager = traitHolder.manager;
			AbilityManagerModel abilityManager = tWDModelManager.CombatModel.AbilityManager;
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("SupportTalent_resistDebuffParm1", ref value, traitHolder);
			abilityManager.VisitParameter("ExtendProbability", ref value2, traitHolder);
			return tWDModelManager.Player.RollDice(diceType, value, value2) != PlayerRandomChanceResult.Failed;
		}

		private void DropToRedHealthIfNecessary(ActorModel actor)
		{
			if (actor.Hitpoints == actor.manager.GameEconomyData.ConfigData.StruggleBaseThreshold && actor.StrugglesLeft > 0)
			{
				actor.SetHitPoints(actor.MaxHitPoints, actor.MaxHitPoints);
				actor.OnRedHealthBar = true;
				actor.StrugglesLeft--;
				actor.EndFortifications(interrupted: true);
			}
		}
	}
}
