using System.Collections.Generic;

namespace TWDModel
{
	public class FirstAidTrait : ActionModifier
	{
		private FixedPoint HealDamagePercentage;

		private int CooldownTurns;

		private int ThisAbilityHealAmount;

		public FirstAidTrait(FixedPoint healDamagePercentage, int cooldownTurns)
		{
			HealDamagePercentage = healDamagePercentage;
			CooldownTurns = cooldownTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (combatModel.TurnManager.ActiveActor != actor)
			{
				return ActionListClearFlag.Keep;
			}
			if (combatModel.TurnManager.TurnCount < actor.NextCanTriggerFirstAidTurn)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is AbilityAfterAddActiveTraitAction abilityAfterAddActiveTraitAction && abilityAfterAddActiveTraitAction.Source == actor)
			{
				ThisAbilityHealAmount = 0;
			}
			if (action is DamageActionExecuteFinishedAction { DamageAction: not null } damageActionExecuteFinishedAction && damageActionExecuteFinishedAction.DamageAction.DamagerActor != null && damageActionExecuteFinishedAction.DamageAction.TargetActor != null && actor == damageActionExecuteFinishedAction.DamageAction.DamagerActor && damageActionExecuteFinishedAction.DamageAction.TargetActor.Faction != Faction.Environmental && damageActionExecuteFinishedAction.DamageAction.DamagerActor.Faction != Faction.Environmental && damageActionExecuteFinishedAction.DamageAction.SourceSupport == null && !(damageActionExecuteFinishedAction.DamageAction is DamageConsumableAction))
			{
				ThisAbilityHealAmount += damageActionExecuteFinishedAction.DamageAction.FinalDamage;
			}
			if (action is AbilityBeforeRemoveActiveTraitAction abilityBeforeRemoveActiveTraitAction)
			{
				if (abilityBeforeRemoveActiveTraitAction.Source == actor && ThisAbilityHealAmount > 0)
				{
					int amountHealed = (int)(ThisAbilityHealAmount * HealDamagePercentage);
					base.manager.ExecuteAction(new HealAction(actor, actor, amountHealed));
					actor.NotifyChange("AbilityVisited", new object[2] { "FirstAid", false });
					actor.NextCanTriggerFirstAidTurn = combatModel.TurnManager.TurnCount + CooldownTurns;
				}
				ThisAbilityHealAmount = 0;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
