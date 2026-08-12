using System.Collections.Generic;

namespace TWDModel
{
	public class BaseDeadlyFocusTrait : ActionModifier
	{
		private ActorModel attackMainTarget;

		private bool isChargeAttack;

		private AbilityModel ability;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction)
			{
				if (abilityAction.Actor == actor && !abilityAction.Ability.IsConsumableAbility)
				{
					attackMainTarget = base.manager.CombatModel.Occupiers[abilityAction.TargetCell];
					isChargeAttack = abilityAction.Ability.IsChargeAttack;
					if (isChargeAttack)
					{
						ChargAttackMark(actor);
					}
				}
				CombatModel combatModel = actor.manager.CombatModel;
				ActorModel actorModel = combatModel.Occupiers[abilityAction.TargetCell];
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (abilityAction.CanExecute() && abilityAction.OOTType != OOTType.PassByAttack && abilityAction.Actor != actor && !abilityAction.Ability.IsConsumableAbility && actorModel != null && !actorModel.IsDead && !weaponEquipment.NeedsReloading && !actor.IsInvisible)
				{
					bool flag = CheckBuff(actor, actorModel);
					bool flag2 = CheckMaxCount(actor);
					if (weaponEquipment.Ability.CanAbilityBePerformedOnGridCell_NoBypassTacticalCheck(combatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate) == AbilityResult.Success && flag && flag2)
					{
						bool flag3 = abilityAction.Ability.IsChargeAttack;
						ActorModel leaderBuffDeadlyFocusMan = CombatHelpers.GetLeaderBuffDeadlyFocusMan(combatModel, actor.Faction);
						if (leaderBuffDeadlyFocusMan == null)
						{
							return ActionListClearFlag.Keep;
						}
						FixedPoint value = 0.0;
						if (flag3)
						{
							base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_ChargePursuitChance", ref value, leaderBuffDeadlyFocusMan);
						}
						else
						{
							base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_PursuitChance", ref value, leaderBuffDeadlyFocusMan);
						}
						FixedPoint citadel_PursuitDown_ParameterMultiplier = actor.GetCitadel_PursuitDown_ParameterMultiplier();
						value *= citadel_PursuitDown_ParameterMultiplier;
						if (value <= ActorTraitContainerModel.Citadel_PercentBase)
						{
							value = ActorTraitContainerModel.Citadel_PercentBase;
						}
						if (base.manager.Player.RollDice(RollDiceType.DeadlyFocus, value) != PlayerRandomChanceResult.Failed)
						{
							combatModel.DeadlyFocus_TurnsEXAttack++;
							FixedPoint value2 = 0.0;
							base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_PursuitDmgPer", ref value2, leaderBuffDeadlyFocusMan);
							addedActions.Add(new DeadlyFocusAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, actorModel, value2, isTriggerExtraAttackDamage: true));
						}
					}
				}
			}
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor && attackMainTarget != null && postDamageAction.IsMainTarget && !attackMainTarget.IsEnvironmental && CheckBuff(actor, attackMainTarget))
			{
				ActorModel leaderBuffDeadlyFocusMan2 = CombatHelpers.GetLeaderBuffDeadlyFocusMan(actor.manager.CombatModel, actor.Faction);
				if (leaderBuffDeadlyFocusMan2 == null)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value3 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_LevelReq_ExApChance", ref value3, leaderBuffDeadlyFocusMan2);
				if (CombatHelpers.GetLeaderBuffDeadlyFocusLevel(base.manager.CombatModel, actor.Faction) + 1 < (int)value3)
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value4 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_ExApChance", ref value4, leaderBuffDeadlyFocusMan2);
				FixedPoint value5 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value5, actor);
				if (base.manager.Player.RollDice(RollDiceType.ChargeLoad, value4, value5) != PlayerRandomChanceResult.Failed)
				{
					actor.AddChargePoints(1);
				}
			}
			return ActionListClearFlag.Keep;
		}

		private bool CheckBuff(ActorModel actor, ActorModel targetActor)
		{
			if (actor == null || targetActor == null)
			{
				return false;
			}
			bool result = false;
			if (actor.Faction == Faction.Raider && targetActor.DeadlyFocusLeftCount_SourceRaider > 0)
			{
				result = true;
			}
			if (actor.Faction == Faction.Survivor && targetActor.DeadlyFocusLeftCount_SourceSurvivor > 0)
			{
				result = true;
			}
			return result;
		}

		private bool CheckMaxCount(ActorModel actor)
		{
			if (actor == null)
			{
				return false;
			}
			CombatModel combatModel = actor.manager.CombatModel;
			if (combatModel == null)
			{
				return false;
			}
			if (combatModel.DeadlyFocus_TurnsEXAttack <= 180)
			{
				return true;
			}
			return false;
		}

		private void ChargAttackMark(ActorModel actor)
		{
			if (actor == null || attackMainTarget == null)
			{
				return;
			}
			ActorModel leaderBuffDeadlyFocusMan = CombatHelpers.GetLeaderBuffDeadlyFocusMan(actor.manager.CombatModel, actor.Faction);
			if (leaderBuffDeadlyFocusMan == null)
			{
				return;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_BuffMaxTurns", ref value, leaderBuffDeadlyFocusMan);
			FixedPoint value2 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_LevelReq_ChargeBuff", ref value2, leaderBuffDeadlyFocusMan);
			if (CombatHelpers.GetLeaderBuffDeadlyFocusLevel(base.manager.CombatModel, actor.Faction) + 1 >= (int)value2)
			{
				switch (actor.Faction)
				{
				case Faction.Raider:
					attackMainTarget.DeadlyFocusLeftCount_SourceRaider = (int)value;
					attackMainTarget.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
					attackMainTarget.NotifyChange("UpdateDeadlyFocus");
					break;
				case Faction.Survivor:
					attackMainTarget.DeadlyFocusLeftCount_SourceSurvivor = (int)value;
					attackMainTarget.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
					attackMainTarget.NotifyChange("UpdateDeadlyFocus");
					break;
				}
			}
		}
	}
}
