using System.Collections.Generic;

namespace TWDModel
{
	public class AttackChainTrait : ActionModifier
	{
		private int MaxEarnTimesOneTurn;

		private int ClearStatusOnAttackSingleActorTimes;

		private FixedPoint UpCriticalDamagePercentage;

		private FixedPoint UpSpecialActorDamagePercentage;

		public AttackChainTrait(int maxEarnTimesOneTurn, int clearStatusOnAttackSingleActorTimes, FixedPoint upCriticalDamagePercentage, FixedPoint upSpecialActorDamagePercentage)
		{
			MaxEarnTimesOneTurn = maxEarnTimesOneTurn;
			ClearStatusOnAttackSingleActorTimes = clearStatusOnAttackSingleActorTimes;
			UpCriticalDamagePercentage = upCriticalDamagePercentage;
			UpSpecialActorDamagePercentage = upSpecialActorDamagePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = actor?.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is DamageAction { DamagerActor: not null } damageAction && damageAction.DamagerActor == actor && !actor.IsDead && damageAction.IsMainTarget)
			{
				actor.AttackChainGainExtraActionPoint = false;
			}
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor != null && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction) && postDamageAction.IsMainTarget && combatModel.TurnManager.ActiveFaction == actor.Faction && !postDamageAction.DamagerActor.IsMoving && !postDamageAction.IsTriggerExtraAttackDamage)
			{
				if (postDamageAction.IsChargeAttack)
				{
					StartAttackChainStatus(combatModel, actor, postDamageAction.TargetActor, giveExtraPoint: true);
					return ActionListClearFlag.Keep;
				}
				if (combatModel.AttackChainContainer == null)
				{
					combatModel.AttackChainContainer = new AttackChainContainer();
					combatModel.AttackChainContainer.AttackChainSourceInfoRecords = new List<AttackChainSourceInfo>();
				}
				AttackChainSourceInfo attackChainSourceInfo = combatModel.AttackChainContainer.AttackChainSourceInfoRecords.Find((AttackChainSourceInfo x) => x.Source == actor);
				if (attackChainSourceInfo == null)
				{
					attackChainSourceInfo = new AttackChainSourceInfo();
					attackChainSourceInfo.Source = actor;
					attackChainSourceInfo.ThisTurnEarnAttackChainNums = 1;
					attackChainSourceInfo.AttackChainTargetInfoRecords = new List<AttackChainTargetInfo>();
					AttackChainTargetInfo attackChainTargetInfo = new AttackChainTargetInfo();
					attackChainTargetInfo.Target = postDamageAction.TargetActor;
					attackChainTargetInfo.AttackNums = 1;
					attackChainSourceInfo.AttackChainTargetInfoRecords.Add(attackChainTargetInfo);
					combatModel.AttackChainContainer.AttackChainSourceInfoRecords.Add(attackChainSourceInfo);
					StartAttackChainStatus(combatModel, actor, postDamageAction.TargetActor, giveExtraPoint: true);
					return ActionListClearFlag.Keep;
				}
				if (attackChainSourceInfo.ThisTurnEarnAttackChainNums >= MaxEarnTimesOneTurn)
				{
					return ActionListClearFlag.Keep;
				}
				if (attackChainSourceInfo.AttackChainTargetInfoRecords == null)
				{
					attackChainSourceInfo.AttackChainTargetInfoRecords = new List<AttackChainTargetInfo>();
				}
				AttackChainTargetInfo attackChainTargetInfo2 = attackChainSourceInfo.AttackChainTargetInfoRecords.Find((AttackChainTargetInfo x) => x.Target == postDamageAction.TargetActor);
				if (attackChainTargetInfo2 == null)
				{
					attackChainSourceInfo.ThisTurnEarnAttackChainNums++;
					attackChainTargetInfo2 = new AttackChainTargetInfo();
					attackChainTargetInfo2.Target = postDamageAction.TargetActor;
					attackChainTargetInfo2.AttackNums = 1;
					attackChainSourceInfo.AttackChainTargetInfoRecords.Add(attackChainTargetInfo2);
					StartAttackChainStatus(combatModel, actor, postDamageAction.TargetActor, giveExtraPoint: true);
					return ActionListClearFlag.Keep;
				}
				if (attackChainTargetInfo2.AttackNums + 1 >= ClearStatusOnAttackSingleActorTimes)
				{
					EndAttackChainStatus(actor, postDamageAction.TargetActor);
					return ActionListClearFlag.Keep;
				}
				attackChainSourceInfo.ThisTurnEarnAttackChainNums++;
				attackChainTargetInfo2.AttackNums++;
				StartAttackChainStatus(combatModel, actor, postDamageAction.TargetActor, giveExtraPoint: true);
				return ActionListClearFlag.Keep;
			}
			return ActionListClearFlag.Keep;
		}

		private void StartAttackChainStatus(CombatModel combatModel, ActorModel sourceActor, ActorModel targetActor, bool giveExtraPoint)
		{
			sourceActor.AttackChainStaus = new AttackChainStaus
			{
				IsAttackChain = true,
				UpCriticalDamagePercentage = UpCriticalDamagePercentage,
				UpSpecialActorDamagePercentage = UpSpecialActorDamagePercentage
			};
			if (giveExtraPoint)
			{
				sourceActor.AttackChainGainExtraActionPoint = true;
			}
			if (targetActor.IsDead)
			{
				return;
			}
			if (sourceActor.Faction == Faction.Survivor)
			{
				foreach (KeyValuePair<int, ActorModel> survivorSlot in combatModel.SurvivorSlots)
				{
					if (survivorSlot.Value == sourceActor && !targetActor.AsTargetAttackChainSlots.Contains(survivorSlot.Key))
					{
						targetActor.AsTargetAttackChainSlots.Add(survivorSlot.Key);
						break;
					}
				}
			}
			else if (sourceActor.Faction == Faction.Raider)
			{
				foreach (KeyValuePair<int, ActorModel> raiderSlot in combatModel.RaiderSlots)
				{
					if (raiderSlot.Value == sourceActor && !targetActor.AsTargetAttackChainSlots.Contains(raiderSlot.Key))
					{
						targetActor.AsTargetAttackChainSlots.Add(raiderSlot.Key);
						break;
					}
				}
			}
			targetActor.NotifyChange("AbilityVisited", new object[2] { "AttackChain", false });
			targetActor.NotifyChange("ActorAttackChainUpdate");
		}

		private void EndAttackChainStatus(ActorModel sourceActor, ActorModel targetActor)
		{
			sourceActor.AttackChainStaus = null;
			if (!targetActor.IsDead)
			{
				if (sourceActor.Faction == Faction.Survivor)
				{
					foreach (KeyValuePair<int, ActorModel> survivorSlot in sourceActor.manager.CombatModel.SurvivorSlots)
					{
						if (survivorSlot.Value == sourceActor)
						{
							targetActor.AsTargetAttackChainSlots.Remove(survivorSlot.Key);
							break;
						}
					}
				}
				else if (sourceActor.Faction == Faction.Raider)
				{
					foreach (KeyValuePair<int, ActorModel> raiderSlot in sourceActor.manager.CombatModel.RaiderSlots)
					{
						if (raiderSlot.Value == sourceActor)
						{
							targetActor.AsTargetAttackChainSlots.Remove(raiderSlot.Key);
							break;
						}
					}
				}
				targetActor.NotifyChange("ActorAttackChainUpdate");
			}
			sourceActor.AttackChainGainExtraActionPoint = false;
		}
	}
}
