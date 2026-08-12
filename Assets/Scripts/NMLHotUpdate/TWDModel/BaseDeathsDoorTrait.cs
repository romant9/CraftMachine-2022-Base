using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class BaseDeathsDoorTrait : ActionModifier
	{
		private ActorModel attackMainTarget;

		private bool isChargeAttack;

		private bool isActiveAttack;

		private AbilityModel activeAttackAbility;

		private GridCoordinate activeAttackTargetCell;

		private bool hasAppliedDeathsDoorMarksThisAttack;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor && abilityAction.IsFromAbilityCommand && !abilityAction.IsTriggerExtraAttackDamage && !abilityAction.IsAssistAttack && !abilityAction.Ability.IsConsumableAbility)
			{
				attackMainTarget = actor.manager.CombatModel.Occupiers[abilityAction.TargetCell];
				isChargeAttack = abilityAction.Ability.IsChargeAttack;
				isActiveAttack = true;
				activeAttackAbility = abilityAction.Ability;
				activeAttackTargetCell = abilityAction.TargetCell;
				hasAppliedDeathsDoorMarksThisAttack = false;
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && isActiveAttack && !hasAppliedDeathsDoorMarksThisAttack)
			{
				DamageAction damageAction = postDamageAction.DamageAction;
				ActorModel targetActor = postDamageAction.TargetActor;
				if (targetActor != null && !targetActor.IsDead && !targetActor.IsEnvironmental && !postDamageAction.IsTriggerExtraAttackDamage && damageAction != null && !damageAction.IsPushDamage && !damageAction.DealDamagePostAbility)
				{
					ApplyDeathsDoorMarksForActiveAttack(actor, targetActor);
					hasAppliedDeathsDoorMarksThisAttack = true;
				}
			}
			if (action is PostAbilityExecuteAction postAbilityExecuteAction && postAbilityExecuteAction.DamagerActor == actor)
			{
				if (isActiveAttack)
				{
					ClearDeathsDoorGainedThisAttack(actor);
				}
				if (attackMainTarget != null && !attackMainTarget.IsEnvironmental && isChargeAttack && isActiveAttack && !attackMainTarget.IsDead)
				{
					TryTriggerPursuit(actor, addedActions);
				}
				ClearAttackContext();
			}
			return ActionListClearFlag.Keep;
		}

		private void TryTriggerPursuit(ActorModel actor, List<ModelAction> addedActions)
		{
			actor.DeathsDoor_PursuitCount = 0;
			ActorModel leaderBuffDeathsDoorMan = CombatHelpers.GetLeaderBuffDeathsDoorMan(actor.manager.CombatModel, actor.Faction);
			if (leaderBuffDeathsDoorMan == null)
			{
				return;
			}
			AbilityManagerModel abilityManager = actor.manager.Player.AbilityManager;
			FixedPoint value = 0L;
			abilityManager.VisitParameter("LeaderBuffDeathsDoor_MaxPursuitCount", ref value, leaderBuffDeathsDoorMan);
			for (int i = 0; i < (int)value; i++)
			{
				if (!TryPursuit(actor, addedActions))
				{
					break;
				}
				actor.DeathsDoor_PursuitCount++;
			}
		}

		private void ApplyDeathsDoorMarksForActiveAttack(ActorModel actor, ActorModel fallbackTarget)
		{
			List<ActorModel> list = null;
			if (activeAttackAbility != null)
			{
				list = actor.manager.CombatModel.AbilityManager.GetListOfActorsToBeTargetted(activeAttackAbility, actor, actor.GridCoordinate, activeAttackTargetCell);
			}
			if (list == null)
			{
				list = new List<ActorModel>();
			}
			if (fallbackTarget != null && !list.Contains(fallbackTarget))
			{
				list.Add(fallbackTarget);
			}
			for (int i = 0; i < list.Count; i++)
			{
				ActorModel actorModel = list[i];
				if (actorModel != null && !actorModel.IsDead && !actorModel.IsEnvironmental)
				{
					ApplyDeathsDoorMark(actor, actorModel);
				}
			}
		}

		private void ApplyDeathsDoorMark(ActorModel actor, ActorModel target)
		{
			ActorModel leaderBuffDeathsDoorMan = CombatHelpers.GetLeaderBuffDeathsDoorMan(actor.manager.CombatModel, actor.Faction);
			if (leaderBuffDeathsDoorMan != null)
			{
				AbilityManagerModel abilityManager = actor.manager.Player.AbilityManager;
				FixedPoint value = 0L;
				abilityManager.VisitParameter("LeaderBuffDeathsDoor_MaxLayer", ref value, leaderBuffDeathsDoorMan);
				FixedPoint value2 = 0L;
				abilityManager.VisitParameter("LeaderBuffDeathsDoor_DmgUpDuration", ref value2, leaderBuffDeathsDoorMan);
				FixedPoint value3 = 0L;
				abilityManager.VisitParameter("LeaderBuffDeathsDoor_DmgUpPerLayer", ref value3, leaderBuffDeathsDoorMan);
				int deathsDoor_DmgUpLayer = target.DeathsDoor_DmgUpLayer;
				target.DeathsDoor_DmgUpLayer = Math.Min(target.DeathsDoor_DmgUpLayer + 1, (int)value);
				target.DeathsDoor_DmgUpLeftTurns = (int)value2;
				target.DeathsDoor_DmgUpLayerGainedThisAttack += target.DeathsDoor_DmgUpLayer - deathsDoor_DmgUpLayer;
				target.NotifyChange("UpdateDeathsDoor");
			}
		}

		private void ClearDeathsDoorGainedThisAttack(ActorModel actor)
		{
			List<ActorModel> allActors = actor.manager.CombatModel.GetAllActors();
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actorModel = allActors[i];
				if (actorModel != null && actorModel.DeathsDoor_DmgUpLayerGainedThisAttack > 0)
				{
					actorModel.DeathsDoor_DmgUpLayerGainedThisAttack = 0;
				}
			}
		}

		private void ClearAllEnemiesDeathsDoorMark(ActorModel actor)
		{
			foreach (ActorModel allActor in actor.manager.CombatModel.GetAllActors())
			{
				if (actor.IsEnemy(allActor) && !allActor.IsDead && !allActor.IsEnvironmental && allActor.DeathsDoor_DmgUpLayer > 0)
				{
					allActor.DeathsDoor_DmgUpLayer = 0;
					allActor.DeathsDoor_DmgUpLeftTurns = 0;
					allActor.NotifyChange("UpdateDeathsDoor");
				}
			}
		}

		private bool TryPursuit(ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor == null || attackMainTarget == null || attackMainTarget.IsDead)
			{
				return false;
			}
			CombatModel combatModel = actor.manager.CombatModel;
			ActorModel leaderBuffDeathsDoorMan = CombatHelpers.GetLeaderBuffDeathsDoorMan(combatModel, actor.Faction);
			if (leaderBuffDeathsDoorMan == null)
			{
				return false;
			}
			AbilityManagerModel abilityManager = actor.manager.Player.AbilityManager;
			FixedPoint value = 0L;
			abilityManager.VisitParameter("LeaderBuffDeathsDoor_MaxPursuitCount", ref value, leaderBuffDeathsDoorMan);
			if (actor.DeathsDoor_PursuitCount >= (int)value)
			{
				return false;
			}
			FixedPoint value2 = 0L;
			abilityManager.VisitParameter("LeaderBuffDeathsDoor_PursuitChance", ref value2, leaderBuffDeathsDoorMan);
			FixedPoint value3 = 0L;
			abilityManager.VisitParameter("ExtendProbability", ref value3, actor);
			if (actor.manager.Player.RollDice(RollDiceType.DeathsDoorPursuit, value2, value3) == PlayerRandomChanceResult.Failed)
			{
				return false;
			}
			FixedPoint value4 = 0L;
			abilityManager.VisitParameter("LeaderBuffDeathsDoor_UnlockLevel", ref value4, leaderBuffDeathsDoorMan);
			EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
			if (weaponEquipment?.Ability == null)
			{
				return false;
			}
			bool blockSecondChance = CombatHelpers.GetLeaderBuffDeathsDoorLevel(combatModel, actor.Faction) >= (int)value4 && (int)value4 > 0;
			addedActions.Add(new DeathsDoorPursuitAction(actor, weaponEquipment.Ability, attackMainTarget.GridCoordinate, attackMainTarget, blockSecondChance));
			return true;
		}

		private void ClearAttackContext()
		{
			attackMainTarget = null;
			isChargeAttack = false;
			isActiveAttack = false;
			activeAttackAbility = null;
			activeAttackTargetCell = GridCoordinate.Invalid;
			hasAppliedDeathsDoorMarksThisAttack = false;
		}
	}
}
