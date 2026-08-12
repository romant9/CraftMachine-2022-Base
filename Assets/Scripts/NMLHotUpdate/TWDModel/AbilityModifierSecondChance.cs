using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierSecondChance : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction { BaseDamage: >0 } damageAction && damageAction.TargetActor == actor)
			{
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				if (actor.IsHuman && actor.OnRedHealthBar)
				{
					FixedPoint value = 0.0;
					FixedPoint value2 = 0.0;
					base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseSecondChanceChance", ref value2, actor);
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					if (value2 > 0.0 && damageAction.DoesAttackKillHumanSurvivor() && (actor.SavedOnTurnIndex == -1 || actor.SavedOnTurnIndex == base.manager.CombatModel.TurnManager.TurnCount))
					{
						if (damageAction.DamagerActor != null && damageAction.DamagerActor.DeathsBlockSecondChance)
						{
							damageAction.DamagerActor.NotifyChange("DeathsDoorBlockSecondChance");
							return ActionListClearFlag.Keep;
						}
						playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Damage, value2, value);
					}
				}
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					damageAction.SavedFromDeath = true;
					actor.SavedOnTurnIndex = base.manager.CombatModel.TurnManager.TurnCount;
					damageAction.ProbabilityOutcome = (PlayerRandomChanceResult)Math.Max((int)damageAction.ProbabilityOutcome, (int)playerRandomChanceResult);
					if (base.manager.CurrentCommandLogEntry != null)
					{
						base.manager.CurrentCommandLogEntry.Dodge(damageAction.DamagerActor, damageAction.TargetActor);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
