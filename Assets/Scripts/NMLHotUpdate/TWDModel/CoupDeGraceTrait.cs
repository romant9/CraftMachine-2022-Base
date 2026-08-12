using System.Collections.Generic;

namespace TWDModel
{
	public class CoupDeGraceTrait : ActionModifier
	{
		private ActorModel attackTarget;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				if (postDamageAction.DamagerActor == actor && postDamageAction.TargetActor == attackTarget && !attackTarget.IsEnvironmental)
				{
					if (postDamageAction.DamagerActor != null && postDamageAction.DamagerActor.dashTraitAttackFlag)
					{
						return ActionListClearFlag.Keep;
					}
					AbilityAction abilityAction = null;
					DamageAction damageAction = postDamageAction.DamageAction;
					if (!attackTarget.IsDead)
					{
						abilityAction = ActivateFollowUp(damageAction);
					}
					attackTarget = null;
					if (abilityAction != null)
					{
						addedActions.Add(abilityAction);
					}
				}
			}
			else if (action is AbilityAction abilityAction2)
			{
				if (abilityAction2.Actor != null && abilityAction2.Actor.dashTraitAttackFlag)
				{
					return ActionListClearFlag.Keep;
				}
				if (abilityAction2.OOTType == OOTType.None && !(abilityAction2 is CoupDeGraceAction) && abilityAction2.Actor == actor && !actor.GetWeaponEquipment().HasTemporaryTrait("RetaliateMultiplier") && !actor.OverwatchedOnTurn && !abilityAction2.Ability.IsConsumableAbility)
				{
					attackTarget = base.manager.CombatModel.Occupiers[abilityAction2.TargetCell];
				}
			}
			else if (action is PostStatusEffectAction postStatusEffectAction)
			{
				if (postStatusEffectAction.SourceActor != null && postStatusEffectAction.SourceActor.dashTraitAttackFlag)
				{
					return ActionListClearFlag.Keep;
				}
				AbilityManagerModel abilityManager = actor.manager.CombatModel.AbilityManager;
				bool flag = postStatusEffectAction.Type == TimedEffectType.Stun && postStatusEffectAction.CausedByTrait == "AbilityModifierExplosiveBulletStunChance" && postStatusEffectAction.SourceActor.HasAnyLevelTrait("LeaderBuffExplosiveBullets");
				if (postStatusEffectAction.Type != TimedEffectType.Herd && postStatusEffectAction.SourceActor == actor && postStatusEffectAction.SourceSupport == null)
				{
					AbilityModel abilityUnderApplication = abilityManager.AbilityUnderApplication;
					if ((abilityUnderApplication == null || !abilityUnderApplication.IsConsumableAbility) && !flag)
					{
						ActivateChargePointGainEffect(actor);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private AbilityAction ActivateFollowUp(DamageAction damageAction)
		{
			AbilityManagerModel abilityManager = damageAction.DamagerActor.manager.CombatModel.AbilityManager;
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("ExtendProbability", ref value, damageAction.DamagerActor);
			abilityManager.VisitParameter("LeaderBuffCoupDeGraceFollowUpProbability", ref value2, damageAction.DamagerActor);
			FixedPoint citadel_PursuitDown_ParameterMultiplier = damageAction.DamagerActor.GetCitadel_PursuitDown_ParameterMultiplier();
			value2 *= citadel_PursuitDown_ParameterMultiplier;
			if (value2 <= ActorTraitContainerModel.Citadel_PercentBase)
			{
				value2 = ActorTraitContainerModel.Citadel_PercentBase;
			}
			if (abilityManager.manager.Player.RollDice(RollDiceType.FollowThrough, value2, value) != PlayerRandomChanceResult.Failed)
			{
				AbilityModel ability = damageAction.DamagerActor.GetWeaponEquipment().Ability;
				return new CoupDeGraceAction(damageAction.DamagerActor, ability, damageAction.TargetActor.GridCoordinate, damageAction.TargetActor, isTriggerExtraAttackDamage: true);
			}
			return null;
		}

		private void ActivateChargePointGainEffect(ActorModel actor)
		{
			AbilityManagerModel abilityManager = actor.manager.CombatModel.AbilityManager;
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("ExtendProbability", ref value, actor);
			abilityManager.VisitParameter("LeaderBuffCoupDeGraceChargeProbability", ref value2, actor);
			PlayerRandomChanceResult playerRandomChanceResult = actor.manager.Player.RollDice(RollDiceType.GainChargePoint, value2, value);
			if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && actor.ChargeMeter.ChargeLevel < actor.ChargeMeter.MaxLevel)
			{
				actor.AddChargePoints(1);
				actor.NotifyChange("AbilityVisited", new object[2]
				{
					"LeaderBuffCoupDeGrace",
					playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
				});
			}
		}
	}
}
