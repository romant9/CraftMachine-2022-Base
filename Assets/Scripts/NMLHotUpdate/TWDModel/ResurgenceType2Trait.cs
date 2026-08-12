using System.Collections.Generic;

namespace TWDModel
{
	public class ResurgenceType2Trait : ActionModifier
	{
		private int Turns;

		public ResurgenceType2Trait(int turns)
		{
			Turns = turns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor?.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			ResurgenceType2Info resurgenceType2Info = combatModel.ResurgenceType2Container.ResurgenceType2InfoRecords.Find((ResurgenceType2Info x) => x.Source == actor);
			if (resurgenceType2Info == null)
			{
				resurgenceType2Info = new ResurgenceType2Info();
				resurgenceType2Info.Source = actor;
				resurgenceType2Info.UsedChargeAttackActors = new List<ActorModel>();
				combatModel.ResurgenceType2Container.ResurgenceType2InfoRecords.Add(resurgenceType2Info);
			}
			if (combatModel.TurnManager.ActiveFaction != actor.Faction)
			{
				resurgenceType2Info.UsedChargeAttackActors.Clear();
				return ActionListClearFlag.Keep;
			}
			if (combatModel.TurnManager.TurnCount < resurgenceType2Info.NextCanTriggedRestoreAPTurn)
			{
				return ActionListClearFlag.Keep;
			}
			if (combatModel.TurnManager.TurnCount == 0 || action is ChangeTurnAction)
			{
				resurgenceType2Info.TurnStartFactionActorNums = combatModel.GetFactionActors(actor.Faction).Count;
			}
			if (action is AbilityAction abilityAction)
			{
				resurgenceType2Info.ThisAbilityActionBearerTriggedRestoreAP = false;
				if (abilityAction.Ability.IsChargeAttack && !resurgenceType2Info.UsedChargeAttackActors.Contains(abilityAction.Actor))
				{
					resurgenceType2Info.UsedChargeAttackActors.Add(abilityAction.Actor);
					if (resurgenceType2Info.TurnStartFactionActorNums > 0 && resurgenceType2Info.UsedChargeAttackActors.Count > 0 && resurgenceType2Info.UsedChargeAttackActors.Count >= resurgenceType2Info.TurnStartFactionActorNums)
					{
						resurgenceType2Info.ThisAbilityActionBearerTriggedRestoreAP = true;
						resurgenceType2Info.UsedChargeAttackActors.Clear();
						return ActionListClearFlag.Keep;
					}
				}
			}
			if (action is PostDamageAction { DamagerActor: not null, TargetActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.TargetActor.IsDead && !resurgenceType2Info.ThisAbilityActionBearerTriggedRestoreAP && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				resurgenceType2Info.ThisAbilityActionBearerTriggedRestoreAP = true;
				return ActionListClearFlag.Keep;
			}
			if (action is PostAbilityExecuteAction && resurgenceType2Info.ThisAbilityActionBearerTriggedRestoreAP && combatModel.TurnManager.ActiveFaction == actor.Faction)
			{
				addedActions.Add(new RestoreAPAction(actor));
				resurgenceType2Info.NextCanTriggedRestoreAPTurn = combatModel.TurnManager.TurnCount + Turns;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
