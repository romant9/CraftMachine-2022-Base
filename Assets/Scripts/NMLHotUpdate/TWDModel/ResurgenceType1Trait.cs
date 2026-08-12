using System.Collections.Generic;

namespace TWDModel
{
	public class ResurgenceType1Trait : ActionModifier
	{
		private int Times;

		public ResurgenceType1Trait(int times)
		{
			Times = times;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor?.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			ResurgenceType1Info resurgenceType1Info = combatModel.ResurgenceType1Container.ResurgenceType1InfoRecords.Find((ResurgenceType1Info x) => x.Source == actor);
			if (resurgenceType1Info == null)
			{
				resurgenceType1Info = new ResurgenceType1Info();
				resurgenceType1Info.Source = actor;
				resurgenceType1Info.UsedChargeAttackActors = new List<ActorModel>();
				combatModel.ResurgenceType1Container.ResurgenceType1InfoRecords.Add(resurgenceType1Info);
			}
			if (combatModel.TurnManager.ActiveFaction != actor.Faction)
			{
				resurgenceType1Info.UsedChargeAttackActors.Clear();
				return ActionListClearFlag.Keep;
			}
			if (combatModel.TurnManager.TurnCount == 0)
			{
				resurgenceType1Info.TurnStartFactionActorNums = combatModel.GetFactionActors(actor.Faction).Count;
			}
			if (action is ChangeTurnAction)
			{
				resurgenceType1Info.ThisTurnAlreadyTiggerTimes = 0;
				resurgenceType1Info.TurnStartFactionActorNums = combatModel.GetFactionActors(actor.Faction).Count;
			}
			if (action is AbilityAction { IsFromAbilityCommand: not false } abilityAction)
			{
				resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP = false;
				if (abilityAction.Ability.IsChargeAttack && !resurgenceType1Info.UsedChargeAttackActors.Contains(abilityAction.Actor))
				{
					resurgenceType1Info.UsedChargeAttackActors.Add(abilityAction.Actor);
					if (resurgenceType1Info.TurnStartFactionActorNums > 0 && resurgenceType1Info.UsedChargeAttackActors.Count > 0 && resurgenceType1Info.UsedChargeAttackActors.Count >= resurgenceType1Info.TurnStartFactionActorNums && resurgenceType1Info.ThisTurnAlreadyTiggerTimes < Times)
					{
						resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP = true;
						resurgenceType1Info.ThisTurnAlreadyTiggerTimes++;
						resurgenceType1Info.UsedChargeAttackActors.Clear();
						return ActionListClearFlag.Keep;
					}
				}
			}
			if (action is PostDamageAction { DamagerActor: not null, TargetActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.TargetActor.IsDead && !resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction) && resurgenceType1Info.ThisTurnAlreadyTiggerTimes < Times)
			{
				resurgenceType1Info.ThisTurnAlreadyTiggerTimes++;
				resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP = true;
				return ActionListClearFlag.Keep;
			}
			if ((action is PostAbilityExecuteAction || action is PostMoveSuccessAction) && resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP && combatModel.TurnManager.ActiveFaction == actor.Faction)
			{
				resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP = false;
				addedActions.Add(new RestoreAPAction(actor));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
