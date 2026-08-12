using System.Collections.Generic;
using TWDModel;

public class PreEmptiveStrikeTrait : ActionModifier
{
	public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
	{
		if (!actor.CanPerformOOT || actor.PreAttackedOnTurn || actor.dashTraitAttackFlag)
		{
			return ActionListClearFlag.Keep;
		}
		if (action is PreAttackAction preAttackAction)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			if (actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.manager.CombatModel, actor, actor.GridCoordinate, preAttackAction.DamagerActor.GridCoordinate) != AbilityResult.Success)
			{
				return ActionListClearFlag.Keep;
			}
			if (preAttackAction.TargetActor == actor && combatModel != null && !combatModel.MissionCompleted && combatModel.TurnManager.ActiveFaction != actor.Faction)
			{
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (weaponEquipment != null && preAttackAction.DamagerActor != null)
				{
					FixedPoint value = 0.0;
					combatModel.AbilityManager.VisitParameter("LeaderBuffPercentageIncreasePreEmptiveStrikeDamage", ref value, actor);
					weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, value * 100L);
					AbilityAction action2 = new AbilityAction(actor, weaponEquipment.Ability, preAttackAction.DamagerActor.GridCoordinate, preAttackAction.DamagerActor, OOTType.PreEmptiveStrike, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
					if (base.manager.ExecuteAction(action2))
					{
						if (actor.HasTraitsThatContains("Interruptor") || actor.HasAnyLevelTrait("Equipment_Active_Interruptor"))
						{
							if (!preAttackAction.DamagerActor.IsDisoriented)
							{
								FixedPoint value2 = 0.0;
								FixedPoint value3 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseInterruptChance", ref value2, actor);
								combatModel.AbilityManager.VisitParameter("Equipment_Active_Interruptor", ref value2, actor);
								combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value3, actor);
								if (base.manager.Player.RollDice(RollDiceType.InterruptAttack, value2, value3) != PlayerRandomChanceResult.Failed)
								{
									preAttackAction.Interrupted = true;
									preAttackAction.DamagerActor.EndAction();
								}
							}
							MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
							if (mapMissionModel != null)
							{
								List<DifficultyIncrementalDebuff> challengeDebuffs = mapMissionModel.GetChallengeDebuffs();
								if (preAttackAction.TargetActor.IsWalker)
								{
									int chance = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffInterruptRate);
									if (base.manager.Player.RollDice(RollDiceType.AvoidInterrupt, chance) == PlayerRandomChanceResult.Success)
									{
										preAttackAction.Interrupted = false;
									}
								}
								else if (preAttackAction.TargetActor.IsRaider)
								{
									int chance2 = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffInterruptRateRaider);
									if (base.manager.Player.RollDice(RollDiceType.AvoidInterrupt, chance2) == PlayerRandomChanceResult.Success)
									{
										preAttackAction.Interrupted = false;
									}
								}
							}
						}
						else if (actor.SelectedAbility.PushEffect != null && actor.SelectedAbility.PushEffect.FindFurthestPushCoordinateByCoordinates(combatModel, actor.GridCoordinate, preAttackAction.DamagerActor.GridCoordinate).ChebyshevDistance(actor.GridCoordinate) - 1 > 1)
						{
							preAttackAction.DamagerActor.EndAction();
						}
						actor.PreAttackedOnTurn = true;
						actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffRegalAuthority", false });
					}
				}
			}
		}
		return ActionListClearFlag.Keep;
	}
}
