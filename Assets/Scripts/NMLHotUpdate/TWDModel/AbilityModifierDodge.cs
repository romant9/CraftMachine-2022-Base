using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierDodge : ActionModifier
	{
		private DamageType dodgedType;

		public AbilityModifierDodge()
		{
		}

		public AbilityModifierDodge(DamageType damageType)
		{
			dodgedType = damageType;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction damageAction && !(action is DamageConsumableAction))
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (damageAction.BaseDamage > 0 && damageAction.TargetActor == actor && combatModel != null && !combatModel.MissionCompleted)
				{
					PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
					if (!actor.IsStunned && !actor.IsStruggling)
					{
						FixedPoint value = 0.0;
						FixedPoint value2 = 0.0;
						if (dodgedType == DamageType.Melee)
						{
							base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseMeleeDodgeChance", ref value2, actor);
							base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_DodgeMeleeParm1", ref value2, actor);
							if (damageAction.DamagerActor != null && damageAction.DamagerActor.HasTraitsThatContains("Riposte"))
							{
								MomentumTimedEffect momentumTimedEffect = damageAction.DamagerActor.MomentumTimedEffect;
								if (momentumTimedEffect != null)
								{
									value2 -= momentumTimedEffect.ReduceEnemyDodgePercentageBase * momentumTimedEffect.CurrentLayer;
								}
							}
							if (damageAction.DamagerActor != null && damageAction.DamagerActor.HasAnyLevelTrait("SupportTalent_HitrateMelee"))
							{
								FixedPoint value3 = 0.0;
								base.manager.Player.AbilityManager.VisitParameter("SupportTalent_HitrateMeleeParm1", ref value3, damageAction.DamagerActor);
								value2 -= value3;
							}
							if (damageAction?.DamagerActor != null)
							{
								FixedPoint snapshotCombatAttributeValueByAttributeType = damageAction.DamagerActor.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.HitrateMelee);
								if (snapshotCombatAttributeValueByAttributeType > 0L)
								{
									value2 -= snapshotCombatAttributeValueByAttributeType;
								}
							}
						}
						else if (dodgedType == DamageType.Ranged)
						{
							base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseRangedDodgeChance", ref value2, actor);
							base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseRangedEquipmentBulletDodgeChance", ref value2, actor);
							base.manager.CombatModel.AbilityManager.VisitParameter("SupportTalent_DodgeRangeParm1", ref value2, actor);
							if (damageAction.DamagerActor != null && damageAction.DamagerActor.HasAnyLevelTrait("SupportTalent_HitrateRange"))
							{
								FixedPoint value4 = 0.0;
								base.manager.Player.AbilityManager.VisitParameter("SupportTalent_HitrateRangeParm1", ref value4, damageAction.DamagerActor);
								value2 -= value4;
							}
							if (damageAction?.DamagerActor != null)
							{
								FixedPoint snapshotCombatAttributeValueByAttributeType2 = damageAction.DamagerActor.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.HitrateRange);
								if (snapshotCombatAttributeValueByAttributeType2 > 0L)
								{
									value2 -= snapshotCombatAttributeValueByAttributeType2;
								}
							}
						}
						if (combatModel.IsGuildBattleMission && actor.IsFriendlyHuman)
						{
							base.manager.CombatModel.AbilityManager.VisitParameter("GuildBattleAbilityModifierDodgeChance", ref value2, actor);
						}
						FixedPoint leaderBuffDeadlyFocus_ExDmgHitRate_HitRate = GetLeaderBuffDeadlyFocus_ExDmgHitRate_HitRate(damageAction.DamagerActor, damageAction.TargetActor);
						value2 -= leaderBuffDeadlyFocus_ExDmgHitRate_HitRate;
						if (damageAction.DamagerActor != null)
						{
							TraitEntry traitAnyLevel = damageAction.DamagerActor.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_I");
							if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) > 0)
							{
								FixedPoint value5 = 0.0;
								base.manager.Player.AbilityManager.VisitParameter("SurvivalManualStorySkill_IParm1", ref value5, damageAction.DamagerActor);
								value2 -= value5;
							}
						}
						if (damageAction.DamagerActor != null)
						{
							FixedPoint value6 = 0.0;
							base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierBoostHitRatePercentage", ref value6, damageAction.DamagerActor);
							if (value6 > 0L)
							{
								value2 -= value6;
							}
						}
						if (damageAction.DamagerActor != null && damageAction.DamagerActor.SupportTalent_NoMoveHitrateFlag)
						{
							FixedPoint value7 = 0.0;
							base.manager.Player.AbilityManager.VisitParameter("SupportTalent_NoMoveHitrateParm1", ref value7, damageAction.DamagerActor);
							value2 -= value7;
						}
						if (damageAction.DamagerActor != null && damageAction.DamagerActor.BlindLeftTurns > 0)
						{
							value2 += damageAction.DamagerActor.BlindDecreaseRate;
						}
						if (value2 <= 0L)
						{
							value2 = 0L;
						}
						IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
						if (challengeDebuffProvider != null && actor.Faction == Faction.Survivor)
						{
							TraitEntry traitAnyLevel2 = actor.TraitContainer.GetTraitAnyLevel("LeaderBuffMysteriousWays");
							if (traitAnyLevel2 == null)
							{
								foreach (ActorModel factionActor in combatModel.GetFactionActors(actor.Faction))
								{
									if (factionActor is SurvivorModel { IsLeader: not false })
									{
										traitAnyLevel2 = factionActor.TraitContainer.GetTraitAnyLevel("LeaderBuffMysteriousWays");
										if (traitAnyLevel2 != null)
										{
											break;
										}
									}
								}
							}
							if (traitAnyLevel2 != null)
							{
								FixedPoint minDebuffParamPercentageByTraitId = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(challengeDebuffProvider.GetChallengeDebuffs(), ChallengeDebuffType.DebuffGabrielLT, traitAnyLevel2.TraitIdentifier);
								if (minDebuffParamPercentageByTraitId > 0L && minDebuffParamPercentageByTraitId < value + value2)
								{
									value = 0L;
									value2 = minDebuffParamPercentageByTraitId;
								}
							}
						}
						base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
						if (damageAction.DamageType == dodgedType && value2 > 0.0)
						{
							playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Dodge, value2, value);
						}
						if (HelpersModel.IsDodge)
						{
							if (!actor.IsEnemy(actor) && playerRandomChanceResult == PlayerRandomChanceResult.Failed)
							{
								playerRandomChanceResult = PlayerRandomChanceResult.Success;
							}
						}
					}
					if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
					{
						damageAction.Dodged = true;
						if (actor.HasAnyLevelTrait("LeaderBuffMysteriousWays") && !damageAction.Critical)
						{
							actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffMysteriousWays", true });
						}
						if (damageAction.DamagerActor.CanBenefitFromStaggerInstantly)
						{
							damageAction.DamagerActor.CanBenefitFromStaggerInstantly = false;
						}
						if (damageAction.TargetActor.CanBenefitFromStaggerInstantly)
						{
							damageAction.TargetActor.CanBenefitFromStaggerInstantly = false;
						}
						if (damageAction.DamagerActor.CanReceiveChargePointFromStagger)
						{
							damageAction.DamagerActor.CanReceiveChargePointFromStagger = false;
						}
						if (damageAction.TargetActor.CanReceiveChargePointFromStagger)
						{
							damageAction.TargetActor.CanReceiveChargePointFromStagger = false;
						}
						damageAction.ProbabilityOutcome = (PlayerRandomChanceResult)Math.Max((int)damageAction.ProbabilityOutcome, (int)playerRandomChanceResult);
						if (base.manager.CurrentCommandLogEntry != null)
						{
							base.manager.CurrentCommandLogEntry.Dodge(damageAction.DamagerActor, damageAction.TargetActor);
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private FixedPoint GetLeaderBuffDeadlyFocus_ExDmgHitRate_HitRate(ActorModel source, ActorModel target)
		{
			if (source == null || target == null)
			{
				return 0.0;
			}
			CombatModel combatModel = source.manager.CombatModel;
			if (combatModel == null)
			{
				return 0.0;
			}
			ActorModel actorModel = null;
			FixedPoint value = 0.0;
			int num = 0;
			switch (source.Faction)
			{
			case Faction.Raider:
				if (target.DeadlyFocusLeftCount_SourceRaider <= 0)
				{
					return 0.0;
				}
				actorModel = CombatHelpers.GetLeaderBuffDeadlyFocusMan(combatModel, Faction.Raider);
				if (actorModel == null)
				{
					return 0.0;
				}
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_LevelReq_ExDmgHitRate", ref value, actorModel);
				num = CombatHelpers.GetLeaderBuffDeadlyFocusLevel(base.manager.CombatModel, Faction.Raider);
				break;
			case Faction.Survivor:
				if (target.DeadlyFocusLeftCount_SourceSurvivor <= 0)
				{
					return 0.0;
				}
				actorModel = CombatHelpers.GetLeaderBuffDeadlyFocusMan(combatModel, Faction.Survivor);
				if (actorModel == null)
				{
					return 0.0;
				}
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_LevelReq_ExDmgHitRate", ref value, actorModel);
				num = CombatHelpers.GetLeaderBuffDeadlyFocusLevel(base.manager.CombatModel, Faction.Survivor);
				break;
			}
			if (num + 1 >= (int)value)
			{
				FixedPoint value2 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_ExDmgHitRate_HitRate", ref value2, actorModel);
				return value2;
			}
			return 0.0;
		}
	}
}
