using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class CombatHelpers
	{
		private static void CheckAndAddDamageNotification(ActorModel source, ref Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications, DamageNotificationData traitIdentifierData)
		{
			if (!damageNotifications.TryGetValue(source, out var value))
			{
				value = new List<DamageNotificationData>();
				damageNotifications.Add(source, value);
			}
			if (!value.Contains(traitIdentifierData))
			{
				value.Add(traitIdentifierData);
			}
		}

		public static int[] CalculateDamage(CombatModel combatModel, ActorModel source, ActorModel target, DamageType type, out PlayerRandomChanceResult criticalResult, out PlayerRandomChanceResult bodyShotResult, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls, bool isSingleTarget, bool isChargeAttack, ref Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications, AbilityModel ability = null, bool isMainTarget = false, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			GameEconomyData gameEconomyData = combatModel.manager.GameEconomyData;
			bool flag = source.GetTraitWithTag("PushCollisionDamage") != null;
			SurvivorModel survivorModel = source as SurvivorModel;
			SurvivorModel survivorModel2 = target as SurvivorModel;
			int rarityLevel = survivorModel?.SurvivorRarityLevel ?? 0;
			int rarityLevel2 = survivorModel2?.SurvivorRarityLevel ?? 0;
			int num = source.Level + gameEconomyData.GetRarityActorLevelModifier(rarityLevel);
			int num2 = ((target != null) ? (target.Level + gameEconomyData.GetRarityActorLevelModifier(rarityLevel2)) : 0) - num;
			if (survivorModel != null && survivorModel.IsHero)
			{
				num2--;
			}
			if (survivorModel2 != null && survivorModel2.IsHero)
			{
				num2++;
			}
			FixedPoint value = 0.0;
			FixedPoint fixedPoint = 0.0;
			FixedPoint fixedPoint2 = 0.0;
			abilityManager.VisitParameter(AbilityModifierIncreaseFinalDamage.FinalDamage, ref value, source);
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageStart(value);
			}
			FixedPoint modifiedDamage = value;
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("PercentageIncreaseBaseDamage", ref value2, source);
			modifiedDamage += modifiedDamage * value2;
			if (source.BlindLeftTurns > 0)
			{
				target?.AddTemporaryTrait("ModifierBlinTrait", default(FixedPoint), null, 0L);
			}
			if (source.HasAnyLevelTrait("Equipment_Active_Groupdmgboost"))
			{
				FixedPoint value3 = 0L;
				combatModel.AbilityManager.VisitParameter("AbilityModifierGroupdmgboostNumberofEnemiesAttacked", ref value3, source);
				if (source.NumberOfEnemiesAttacked >= value3)
				{
					FixedPoint value4 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierGroupdmgboostprobability", ref value4, source);
					FixedPoint successProbabilityExtension = 0.0;
					if (value4 != 0.0 && source.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value4, successProbabilityExtension) != PlayerRandomChanceResult.Failed)
					{
						FixedPoint value5 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierGroupdmgboostAdditionalweapondamage", ref value5, source);
						modifiedDamage += modifiedDamage * value5;
						CheckAndAddDamageNotification(target, ref damageNotifications, new DamageNotificationData("Equipment_Active_Groupdmgboost", dueLuck: false));
					}
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Apocalyptic_DMG_Scout"))
			{
				FixedPoint value6 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierDMGScoutAttackingAHighRanking", ref value6, source);
				if (target.Level - source.Level > value6)
				{
					FixedPoint value7 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGScoutLevelDifference", ref value7, source);
					FixedPoint value8 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGScoutIncreaseDamage", ref value8, source);
					FixedPoint value9 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGScoutMaximumLiftingValue", ref value9, source);
					FixedPoint value10 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGScoutMaxLeveLimitValue", ref value10, source);
					FixedPoint fixedPoint3 = 0L;
					fixedPoint3 = ((!(target.Level - source.Level > value10)) ? ((FixedPoint)Math.Pow((double)(1L + value8), (double)((target.Level - source.Level - value6) / value7))) : ((FixedPoint)Math.Pow((double)(1L + value8), (double)((value10 - value6) / value7))));
					if (fixedPoint3 > 50L)
					{
						fixedPoint3 = 50L;
					}
					if (fixedPoint3 > value9)
					{
						fixedPoint3 = value9;
					}
					modifiedDamage *= fixedPoint3;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Apocalyptic_DMG_Bruiser"))
			{
				FixedPoint value11 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierDMGBruiserAttackingAHighRanking", ref value11, source);
				if (target.Level - source.Level > value11)
				{
					FixedPoint value12 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGBruiserLevelDifference", ref value12, source);
					FixedPoint value13 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGBruiserIncreaseDamage", ref value13, source);
					FixedPoint value14 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGBruiserMaximumLiftingValue", ref value14, source);
					FixedPoint value15 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGBruiserMaxLeveLimitValue", ref value15, source);
					FixedPoint fixedPoint4 = 0L;
					fixedPoint4 = ((!(target.Level - source.Level > value15)) ? ((FixedPoint)Math.Pow((double)(1L + value13), (double)((target.Level - source.Level - value11) / value12))) : ((FixedPoint)Math.Pow((double)(1L + value13), (double)((value15 - value11) / value12))));
					if (fixedPoint4 > 50L)
					{
						fixedPoint4 = 50L;
					}
					if (fixedPoint4 > value14)
					{
						fixedPoint4 = value14;
					}
					modifiedDamage *= fixedPoint4;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Apocalyptic_DMG_Warrior"))
			{
				FixedPoint value16 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierDMGWarriorAttackingAHighRanking", ref value16, source);
				if (target.Level - source.Level > value16)
				{
					FixedPoint value17 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGWarriorLevelDifference", ref value17, source);
					FixedPoint value18 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGWarriorIncreaseDamage", ref value18, source);
					FixedPoint value19 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGWarriorMaximumLiftingValue", ref value19, source);
					FixedPoint value20 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGWarriorMaxLeveLimitValue", ref value20, source);
					FixedPoint fixedPoint5 = 0L;
					fixedPoint5 = ((!(target.Level - source.Level > value20)) ? ((FixedPoint)Math.Pow((double)(1L + value18), (double)((target.Level - source.Level - value16) / value17))) : ((FixedPoint)Math.Pow((double)(1L + value18), (double)((value20 - value16) / value17))));
					if (fixedPoint5 > 50L)
					{
						fixedPoint5 = 50L;
					}
					if (fixedPoint5 > value19)
					{
						fixedPoint5 = value19;
					}
					modifiedDamage *= fixedPoint5;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Apocalyptic_DMG_Shooter"))
			{
				FixedPoint value21 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierDMGShooterAttackingAHighRanking", ref value21, source);
				if (target.Level - source.Level > value21)
				{
					FixedPoint value22 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGShooterLevelDifference", ref value22, source);
					FixedPoint value23 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGShooterIncreaseDamage", ref value23, source);
					FixedPoint value24 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGShooterMaximumLiftingValue", ref value24, source);
					FixedPoint value25 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGShooterMaxLeveLimitValue", ref value25, source);
					FixedPoint fixedPoint6 = 0L;
					fixedPoint6 = ((!(target.Level - source.Level > value25)) ? ((FixedPoint)Math.Pow((double)(1L + value23), (double)((target.Level - source.Level - value21) / value22))) : ((FixedPoint)Math.Pow((double)(1L + value23), (double)((value25 - value21) / value22))));
					if (fixedPoint6 > 50L)
					{
						fixedPoint6 = 50L;
					}
					if (fixedPoint6 > value24)
					{
						fixedPoint6 = value24;
					}
					modifiedDamage *= fixedPoint6;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Apocalyptic_DMG_Hunter"))
			{
				FixedPoint value26 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierDMGHunterAttackingAHighRanking", ref value26, source);
				if (target.Level - source.Level > value26)
				{
					FixedPoint value27 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGHunterLevelDifference", ref value27, source);
					FixedPoint value28 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGHunterIncreaseDamage", ref value28, source);
					FixedPoint value29 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGHunterMaximumLiftingValue", ref value29, source);
					FixedPoint value30 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGHunterMaxLeveLimitValue", ref value30, source);
					FixedPoint fixedPoint7 = 0L;
					fixedPoint7 = ((!(target.Level - source.Level > value30)) ? ((FixedPoint)Math.Pow((double)(1L + value28), (double)((target.Level - source.Level - value26) / value27))) : ((FixedPoint)Math.Pow((double)(1L + value28), (double)((value30 - value26) / value27))));
					if (fixedPoint7 > 50L)
					{
						fixedPoint7 = 50L;
					}
					if (fixedPoint7 > value29)
					{
						fixedPoint7 = value29;
					}
					modifiedDamage *= fixedPoint7;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Apocalyptic_DMG_Assault"))
			{
				FixedPoint value31 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierDMGAssaultAttackingAHighRanking", ref value31, source);
				if (target.Level - source.Level > value31)
				{
					FixedPoint value32 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGAssaultLevelDifference", ref value32, source);
					FixedPoint value33 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGAssaultIncreaseDamage", ref value33, source);
					FixedPoint value34 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGAssaultMaximumLiftingValue", ref value34, source);
					FixedPoint value35 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierDMGAssaultMaxLeveLimitValue", ref value35, source);
					FixedPoint fixedPoint8 = 0L;
					fixedPoint8 = ((!(target.Level - source.Level > value35)) ? ((FixedPoint)Math.Pow((double)(1L + value33), (double)((target.Level - source.Level - value31) / value32))) : ((FixedPoint)Math.Pow((double)(1L + value33), (double)((value35 - value31) / value32))));
					if (fixedPoint8 > 50L)
					{
						fixedPoint8 = 50L;
					}
					if (fixedPoint8 > value34)
					{
						fixedPoint8 = value34;
					}
					modifiedDamage *= fixedPoint8;
				}
			}
			if (combatModel.gameEconomyData.ConfigData.DamageVariation)
			{
				FixedPoint value36 = 0.0;
				abilityManager.VisitParameter(AbilityModifierIncreaseDamageVariation.DamageVariation, ref value36, source);
				FixedPoint fixedPoint9 = modifiedDamage * value36;
				int min = (int)(modifiedDamage - fixedPoint9);
				int max = (int)(modifiedDamage + fixedPoint9);
				modifiedDamage = combatModel.RollCombatDiceFromRange(RollDiceType.Damage, min, max);
			}
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageVariation(modifiedDamage);
			}
			FixedPoint value37 = 0.0;
			FixedPoint value38 = 0.0;
			switch (type)
			{
			case DamageType.Melee:
				if (source.BloodThirst)
				{
					abilityManager.VisitParameter("AbilityModifierBloodThirst", ref value37, source);
				}
				abilityManager.VisitParameter("AddMeleeDamage", ref value38, source);
				break;
			case DamageType.Ranged:
				abilityManager.VisitParameter("AddRangedDamage", ref value38, source);
				break;
			case DamageType.Heal:
				abilityManager.VisitParameter("PercentageIncreaseHealing", ref value37, source);
				break;
			}
			if (combatModel.IsTargetNextToAlly(source, target))
			{
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseTargetDamageNextToAlly", ref value37, source);
			}
			FixedPoint fixedPoint10 = 0.0;
			FixedPoint fixedPoint11 = 0.0;
			fixedPoint10 += FixedPoint.Max(0.0, source.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.Attack));
			fixedPoint11 += FixedPoint.Max(0.0, source.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.AttackRatio));
			modifiedDamage += modifiedDamage * (value37 + fixedPoint11) + value38 + fixedPoint10;
			ApplyAstheniaDamage(combatModel, source, target, ref modifiedDamage);
			abilityManager.VisitParameter("AbilityModifierIncreaseBaseDamageFlat", ref modifiedDamage, source);
			FixedPoint value39 = 0.0;
			if (isChargeAttack && abilityManager.VisitParameter("AbilityModifierIncreaseChargeAbilityDamage", ref value39, source) && value39 != 0L)
			{
				modifiedDamage += modifiedDamage * value39;
				TraitEntry traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_B");
				if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) > 0)
				{
					source.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_B", false });
				}
			}
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageTypeModified(modifiedDamage, type, value37, value38);
			}
			FixedPoint value40 = 0.0;
			if (flag)
			{
				bodyShotResult = PlayerRandomChanceResult.Failed;
			}
			else if (resolvedRolls != null && resolvedRolls.ContainsKey(RollDiceType.BodyShot))
			{
				bodyShotResult = resolvedRolls[RollDiceType.BodyShot];
			}
			else
			{
				bodyShotResult = IsBodyShot(combatModel, source, target, ability, isTriggerExtraAttackDamage);
			}
			PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
			if (!source.HasAnyLevelTrait("DebuffMarkEnemy") && (source.HasAnyLevelTrait("Stagger") || source.HasTraitsThatContains("LeaderBuffGoodEnoughStaggerBase") || source.HasTraitsThatContains("Equipment.Stagger") || source.OneTurnStagger || source.HasTraitsThatContains("Equipment.FollowStatus.Stagger")))
			{
				FixedPoint value41 = 0.0;
				FixedPoint value42 = 0.0;
				FixedPoint value43 = 1.0;
				combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value41, source);
				combatModel.AbilityManager.VisitParameter("StaggerChance", ref value42, source);
				combatModel.AbilityManager.VisitParameter("LeaderBuffGoodEnoughStaggerChance", ref value42, source);
				if (source.OneTurnStagger)
				{
					combatModel.AbilityManager.VisitParameter("AbilityModifierRepulseStaggerChance", ref value42, source);
				}
				if (!combatModel.AbilityManager.VisitParameter("StaggerActiveChargeChance", ref value43, target))
				{
					value43 = 0.0;
				}
				FixedPoint successProbability = FixedPoint.Max(value42, value43);
				playerRandomChanceResult = source.manager.Player.RollDice(RollDiceType.Stagger, successProbability, value41);
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					source.CanBenefitFromStaggerInstantly = true;
					target.CanBenefitFromStaggerInstantly = true;
				}
			}
			if (type == DamageType.Melee && (target.IsStaggered || playerRandomChanceResult != PlayerRandomChanceResult.Failed))
			{
				bodyShotResult = PlayerRandomChanceResult.Failed;
				FixedPoint value44 = 1.0;
				FixedPoint value45 = 1.0;
				FixedPoint value46 = 0.0;
				if (!combatModel.AbilityManager.VisitParameter("StaggerActiveChargeChance", ref value44, target))
				{
					value44 = 0.0;
				}
				if (!combatModel.AbilityManager.VisitParameter("StaggerActiveChargeChance", ref value45, source))
				{
					value45 = 0.0;
				}
				combatModel.AbilityManager.VisitParameter("LeaderBuffGoodEnoughStaggerChargeChance", ref value46, source);
				FixedPoint successProbability2 = FixedPoint.Max(value45, FixedPoint.Max(value44, value46));
				if (source.manager.Player.RollDice(RollDiceType.Stagger, successProbability2) != PlayerRandomChanceResult.Failed)
				{
					source.CanReceiveChargePointFromStagger = true;
					if (source.Faction != Faction.Environmental && target.Faction != Faction.Environmental && !source.IsDead && !target.IsDead && !flag && !source.CanBenefitFromStaggerInstantly)
					{
						source.AddChargePoints(1);
					}
				}
			}
			if (type == DamageType.Ranged && target.IsRemoteWeakened)
			{
				bodyShotResult = PlayerRandomChanceResult.Failed;
			}
			FixedPoint value47 = 0.0;
			abilityManager.VisitParameter("ExtendProbability", ref value47, source);
			if (source.OneTurnCriticalHit)
			{
				bodyShotResult = PlayerRandomChanceResult.Failed;
			}
			if (source.TacticalResupplyMagazineNextDragLineCritPending && ootType == OOTType.None && !isAssistAttack && isMainTarget && (type == DamageType.Melee || type == DamageType.Ranged))
			{
				bodyShotResult = PlayerRandomChanceResult.Failed;
			}
			if (bodyShotResult != PlayerRandomChanceResult.Failed)
			{
				abilityManager.VisitParameter(AbilityModifierIncreaseBodyShot.FetchIncreaseBodyShotMultiplier, ref value40, target);
				abilityManager.VisitParameter(AbilityModifierBodyShot.FetchBodyShotMultiplier, ref value40, source);
				modifiedDamage *= value40;
				FixedPoint value48 = 0.0;
				if (abilityManager.VisitParameter("AbilityModifierIncreaseExtraChargePointChanceAfterBodyShot", ref value48, source))
				{
					PlayerRandomChanceResult playerRandomChanceResult2 = combatModel.manager.Player.RollDice(RollDiceType.GainAP, value48, value47);
					if (playerRandomChanceResult2 != PlayerRandomChanceResult.Failed)
					{
						source.AddChargePoints(1);
						source.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffGoodOutOfBad",
							playerRandomChanceResult2 == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
				}
			}
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageBodyShot(modifiedDamage, value40, bodyShotResult);
			}
			fixedPoint = modifiedDamage;
			FixedPoint value49 = 0.0;
			FixedPoint value50 = 1.0;
			bool flag2 = true;
			FixedPoint fixedPoint12 = gameEconomyData.GetLevelBalanceCriticalChanceModifier(source.Faction, target.Faction, num2);
			value49 += fixedPoint12;
			if (fixedPoint12 > combatModel.manager.GameEconomyData.ConfigData.MaximumCriticalChance / 100.0)
			{
				flag2 = false;
			}
			if (bodyShotResult == PlayerRandomChanceResult.Failed && !flag)
			{
				if (source.OneTurnCriticalHit)
				{
					abilityManager.VisitParameter("AbilityModifierPursuitCH", ref value49, source);
					abilityManager.VisitParameter("AbilityModifierRepulseCriticalHitChance", ref value49, source);
					abilityManager.VisitParameter("AbilityModifierAdvanceCriticalHitChance", ref value49, source);
				}
				abilityManager.VisitParameter(AbilityModifierCritical.FetchCriticalChance, ref value49, source);
				abilityManager.VisitParameter(AbilityModifierCritical.FetchCriticalMultiplier, ref value50, source);
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseCriticalChance", ref value49, source);
				if (source.IsSneak && source.HasAnyLevelTrait("LeaderBuffCriticalChance"))
				{
					abilityManager.VisitParameter("AbilityModifierCarolCriticalChance", ref value49, source);
					abilityManager.VisitParameter("AbilityModifierCarolCriticalDamage", ref value50, source);
				}
				if (source.HasAnyLevelTrait("LeaderBuffCriticalChance"))
				{
					abilityManager.VisitParameter("AbilityModifierCarolCriticalChance", ref value49, source);
					abilityManager.VisitParameter("AbilityModifierCarolCriticalDamage", ref value50, source);
				}
				if (source.SupportTalent_NoMoveCritRateFlag && source.HasAnyLevelTrait("SupportTalent_NoMoveCritRate"))
				{
					abilityManager.VisitParameter("SupportTalent_NoMoveCritRateParm1", ref value49, source);
				}
				if (source.HasAnyLevelTrait("SupportTalent_CritRate"))
				{
					abilityManager.VisitParameter("SupportTalent_CritRateParm1", ref value49, source);
				}
				if (source.HasAnyLevelTrait("SupportTalent_CritDmg"))
				{
					abilityManager.VisitParameter("SupportTalent_CritDmgParm1", ref value50, source);
				}
				if (target.HasAnyLevelTrait("SupportTalent_RefCritRate"))
				{
					FixedPoint value51 = 0.0;
					abilityManager.VisitParameter("SupportTalent_CritRateRefParm1", ref value51, target);
					value49 += value51;
				}
				if (target.HasAnyLevelTrait("SupportTalent_RefCritDmg"))
				{
					FixedPoint value52 = 0.0;
					abilityManager.VisitParameter("SupportTalent_CritDmgRefParm1", ref value52, target);
					value50 += value52;
				}
				TraitEntry traitAnyLevel2 = source.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_G");
				if (traitAnyLevel2 != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel2.TraitIdentifier) > 0)
				{
					FixedPoint value53 = 0.0;
					combatModel.AbilityManager.VisitParameter("SurvivalManualStorySkill_GParm1", ref value53, source);
					if (FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0) >= value53)
					{
						FixedPoint value54 = 0.0;
						abilityManager.VisitParameter("SurvivalManualStorySkill_GParm2", ref value54, source);
						value49 += value54;
						abilityManager.VisitParameter("SurvivalManualStorySkill_GParm3", ref value50, source);
						if (value54 != 0L)
						{
							source.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_G", false });
						}
					}
				}
				abilityManager.VisitParameter("AbilityModifierIncreaseCriticalChanceResistance", ref value49, target);
				if (!source.MoveCompleted || (source.GetIsUsingAdditionalAttacks() && source.MoveCompleted))
				{
					abilityManager.VisitParameter("PercentageIncreaseCriticalChanceNoMove", ref value49, source);
				}
				if (source.HasTraitsThatContains("Equipment_Passive_SawAxe") && target.TornApartMarkCount > 0L)
				{
					FixedPoint value55 = 0.0;
					abilityManager.VisitParameter("Equipment_Passive_SawAxe_CriticalChance", ref value55, source);
					value49 += ((value55 * target.TornApartMarkCount >= 0.9) ? ((FixedPoint)0.9) : (value55 * target.TornApartMarkCount));
				}
				if (type == DamageType.Ranged)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseRangedCriticalChance", ref value49, source);
					if (target != null && source.IsRangedClass && target.Level > source.Level)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseTargetHigherLevelCritChance", ref value49, source);
					}
				}
				abilityManager.VisitParameter(AbilityModifierIncreaseCriticalChance.CriticalChance, ref value49, source);
				FixedPoint fixedPoint13 = 0.0;
				FixedPoint fixedPoint14 = 0.0;
				fixedPoint13 += FixedPoint.Max(0.0, source.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.Critical));
				fixedPoint14 += FixedPoint.Max(0.0, target.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.CriticalRef));
				value49 = FixedPoint.Max(value49 + fixedPoint13 - fixedPoint14, 0.0);
				if (combatModel.IsGuildBattleMission && source.IsFriendlyHuman)
				{
					abilityManager.VisitParameter("GuildBattleAbilityModifierCriticalChance", ref value49, source);
				}
				abilityManager.VisitParameter(AbilityModifierIncreaseCriticalMultiplier.CriticalMultiplier, ref value50, source);
				if (source.HasAnyLevelTrait("Equipment_Passive_Rage"))
				{
					FixedPoint value56 = 0.0;
					if (abilityManager.VisitParameter("Equipment_Passive_RageParam4", ref value56, source))
					{
						value50 += source.TotalRage * value56;
					}
				}
				if (source != null && source.IsInChargeConvertState())
				{
					value50 += source.ChargeConvertCritDamageBonus;
				}
				FixedPoint fixedPoint15 = 0.0;
				FixedPoint fixedPoint16 = 0.0;
				fixedPoint15 += FixedPoint.Max(0.0, source.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.DmgCriticalRatio));
				fixedPoint16 += FixedPoint.Max(0.0, target.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.DmgCriticalRatioRef));
				value50 = FixedPoint.Max(value50 + fixedPoint15 - fixedPoint16, 0.0);
				if (source.HasTraitsThatContains("Equipment_Passive_SawAxe") && target.TornApartMarkCount > 0L)
				{
					FixedPoint value57 = 0.0;
					abilityManager.VisitParameter("Equipment_Passive_SawAxe_CriticalMultiplier", ref value57, source);
					value50 += target.TornApartMarkCount * value57;
				}
				abilityManager.VisitParameter("FortifiedCriticalReduction", ref value49, target);
				bool isStunned = target.IsStunned;
				bool flag3 = source.TacticalResupplyMagazineNextDragLineCritPending && ootType == OOTType.None && !isAssistAttack && isMainTarget && (type == DamageType.Melee || type == DamageType.Ranged);
				FixedPoint fixedPoint17 = value49 * (1.0 + value47);
				MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
				if (mapMissionModel != null && target.Faction == Faction.Survivor)
				{
					TraitEntry traitAnyLevel3 = target.TraitContainer.GetTraitAnyLevel("LeaderBuffMulletTime");
					if (traitAnyLevel3 == null)
					{
						foreach (ActorModel factionActor in combatModel.GetFactionActors(target.Faction))
						{
							if (factionActor is SurvivorModel { IsLeader: not false })
							{
								traitAnyLevel3 = factionActor.TraitContainer.GetTraitAnyLevel("LeaderBuffMulletTime");
								if (traitAnyLevel3 != null)
								{
									break;
								}
							}
						}
					}
					if (traitAnyLevel3 != null)
					{
						FixedPoint minDebuffParamPercentageByTraitId = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel.GetChallengeDebuffs(), ChallengeDebuffType.DebuffEugeneLT, traitAnyLevel3.TraitIdentifier);
						if (minDebuffParamPercentageByTraitId > 0L)
						{
							value49 = Math.Max((int)(-minDebuffParamPercentageByTraitId), (int)fixedPoint17);
							value47 = 0L;
						}
					}
				}
				if (!source.SelectedEquipment.IsChargeEquipment && flag2 && fixedPoint17 > combatModel.manager.GameEconomyData.ConfigData.MaximumCriticalChance / 100.0)
				{
					criticalResult = (isStunned ? PlayerRandomChanceResult.Success : combatModel.manager.Player.RollDice(RollDiceType.Critical, combatModel.manager.GameEconomyData.ConfigData.MaximumCriticalChance / 100.0, 0.0));
				}
				else
				{
					criticalResult = (isStunned ? PlayerRandomChanceResult.Success : combatModel.manager.Player.RollDice(RollDiceType.Critical, value49, value47));
				}
				if (flag3)
				{
					criticalResult = PlayerRandomChanceResult.Success;
				}
				if (criticalResult != PlayerRandomChanceResult.Failed)
				{
					bool flag4 = target?.HasTrait("CommonwealthArmorActive") ?? false;
					if (target != null && target.HasTrait("TemFullyStateTrait"))
					{
						flag4 = true;
					}
					if (flag3)
					{
						flag4 = false;
					}
					if (flag4)
					{
						target.NotifyChange("CriticalHitAvoided");
						criticalResult = PlayerRandomChanceResult.Failed;
					}
				}
				if (criticalResult != PlayerRandomChanceResult.Failed)
				{
					modifiedDamage *= value50;
				}
				fixedPoint2 = modifiedDamage - fixedPoint;
				if (flag3 && criticalResult != PlayerRandomChanceResult.Failed)
				{
					source.TacticalResupplyMagazineNextDragLineCritPending = false;
				}
			}
			else
			{
				criticalResult = PlayerRandomChanceResult.Failed;
			}
			combatModel.manager.CurrentCommandLogEntry?.CalculateDamageCritical(modifiedDamage, value50, criticalResult);
			FixedPoint value58 = 1.0;
			FixedPoint value59 = 1.0;
			FixedPoint fixedPoint18 = 0.0;
			abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageIncrementer", ref value58, source);
			abilityManager.VisitParameter("AbilityModifierLeaderBuffInspireDamageIncrease", ref value58, source);
			abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageFeaturedHero", ref value59, source);
			if (target.HasPrincessStatusEffect)
			{
				abilityManager.VisitParameter("LeaderBuffPrincess.ExtraDamage", ref value58, source);
			}
			if (target.HasTrait("UnleashedActive"))
			{
				abilityManager.VisitParameter("LeaderBuffUnleashedFighterExtraDamageLeader", ref value58, source);
			}
			if (GetHeadshotTraitDamage(combatModel, source, target, value47, out var damage))
			{
				fixedPoint18 += damage;
				target.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffHeadshot", false });
			}
			if (source.Faction == Faction.Survivor)
			{
				FixedPoint value60 = 0.0;
				abilityManager.VisitParameter("LeaderBuffClosingTimeRange", ref value60, source);
				if (IsHaveSurvivorWithinRangEnemy(value60, source, target, combatModel))
				{
					if (isMainTarget)
					{
						FixedPoint value61 = target.Hitpoints;
						if (abilityManager.VisitParameter("LeaderBuffClosingTimeMainTargetDamageChance", ref value61, source))
						{
							MapMissionModel mapMissionModel2 = MapMissionDebuffHelper.CanUseDebuffMission(source.manager);
							TraitEntry traitAnyLevel4 = source.TraitContainer.GetTraitAnyLevel("LeaderBuffClosingTime");
							if (traitAnyLevel4 == null)
							{
								traitAnyLevel4 = source.TraitContainer.GetTraitAnyLevel("BaseClosingTime");
								if (traitAnyLevel4 != null)
								{
									foreach (ActorModel factionActor2 in combatModel.GetFactionActors(source.Faction))
									{
										traitAnyLevel4 = factionActor2.TraitContainer.GetTraitAnyLevel("LeaderBuffClosingTime");
										if (traitAnyLevel4 != null)
										{
											break;
										}
									}
								}
							}
							if (mapMissionModel2 != null && traitAnyLevel4 != null)
							{
								FixedPoint minDebuffParamPercentageByTraitId2 = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel2.GetChallengeDebuffs(), ChallengeDebuffType.DebuffQuinnLT, traitAnyLevel4.TraitIdentifier);
								if (minDebuffParamPercentageByTraitId2 > 0L)
								{
									value61 = FixedPoint.Min(value61, modifiedDamage * minDebuffParamPercentageByTraitId2);
								}
							}
							fixedPoint18 += value61;
						}
					}
					else
					{
						abilityManager.VisitParameter("LeaderBuffClosingTimeSecondaryTargetDamageChance", ref value58, source);
					}
					CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffClosingTime", dueLuck: false));
				}
			}
			FixedPoint value62 = source.MaxHitPoints;
			string paramName = (isChargeAttack ? "LeaderBuffProtectChargeDamageChance" : "LeaderBuffProtectDamageChance");
			if (source.HasAnyLevelTrait("LeaderBuffProtect") && abilityManager.VisitParameter(paramName, ref value62, source))
			{
				fixedPoint18 += value62;
				target.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffProtect", false });
			}
			if (source.HasAnyLevelTrait("BaseShadowedGuard"))
			{
				FixedPoint value63 = 0L;
				ActorModel leaderBuffShadowedGuardMan = GetLeaderBuffShadowedGuardMan(combatModel, source.Faction);
				abilityManager.VisitParameter("LeaderBuffShadowedGuard_HpDmg", ref value63, leaderBuffShadowedGuardMan);
				value63 *= (FixedPoint)source.MaxHitPoints;
				fixedPoint18 += value63;
				target.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffShadowedGuard", false });
			}
			FixedPoint fixedPoint19 = target.Hitpoints;
			FixedPoint value64 = 1L;
			if (abilityManager.VisitParameter("EmitterDamageActiveMultiplier", ref value64, source))
			{
				FixedPoint fixedPoint20 = fixedPoint19 * value64;
				MapMissionModel mapMissionModel3 = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
				if (mapMissionModel3 != null && source.Faction == Faction.Survivor && source is SurvivorModel survivorModel4)
				{
					TraitEntry traitAnyLevel5 = source.TraitContainer.GetTraitAnyLevel("EmitterDamageActive");
					if (traitAnyLevel5 == null)
					{
						foreach (ActorModel factionActor3 in combatModel.GetFactionActors(source.Faction))
						{
							if (factionActor3 is SurvivorModel { IsLeader: not false })
							{
								traitAnyLevel5 = factionActor3.TraitContainer.GetTraitAnyLevel("LeaderBuffEmitter");
								if (traitAnyLevel5 != null)
								{
									break;
								}
							}
						}
					}
					if (traitAnyLevel5 != null)
					{
						FixedPoint minDebuffParamPercentageByTraitId3 = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel3.GetChallengeDebuffs(), ChallengeDebuffType.DebuffBethLT, traitAnyLevel5.TraitIdentifier);
						if (minDebuffParamPercentageByTraitId3 > 0L)
						{
							fixedPoint20 = Math.Min((int)(survivorModel4.GetDamageForPreferredWeapon() * minDebuffParamPercentageByTraitId3), (int)fixedPoint20);
						}
					}
				}
				fixedPoint18 += fixedPoint20;
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffEmitter", dueLuck: false));
			}
			if (target.HasAnyLevelTrait("DebuffMarkEnemy"))
			{
				abilityManager.VisitParameter("LeaderBuffMarkEnemy.ExtraDamage", ref value58, target);
				FixedPoint value65 = 1.0;
				abilityManager.VisitParameter("LeaderBuffMarkEnemy.ExtraDamage", ref value65, target);
				fixedPoint18 *= value65;
			}
			if (combatModel.HasActorBeenAttackedByFaction(target, source.Faction) && abilityManager.VisitParameter("AbilityModifierPercentageIncreaseSameTargetDamage", ref value58, source))
			{
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffTeamwork", dueLuck: false));
			}
			if (target != null && target.Level > source.Level && abilityManager.VisitParameter("AbilityModifierPercentageIncreaseTargetHigherLevelDamage", ref value58, source))
			{
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffJackass", dueLuck: false));
			}
			if (isSingleTarget && abilityManager.VisitParameter("LeaderBuffDeadlyTactics", ref value58, source))
			{
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffDeadlyTactics", dueLuck: false));
			}
			if (source.OnRedHealthBar && abilityManager.VisitParameter(AbilityModifierSurvivalInstinct.FetchIncreaseDamageDone, ref value58, source))
			{
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffSurvivalInstinct", dueLuck: false));
			}
			ActorDefinition actorDefinition = target?.Definition;
			if (actorDefinition != null && ((actorDefinition.Faction == Faction.Walker && actorDefinition.IsSpecial) || actorDefinition.Faction == Faction.Raider || actorDefinition.Faction == Faction.Any) && abilityManager.VisitParameter("AbilityModifierIncreaseDamageOnSpecial", ref value58, source))
			{
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffHunterDesperation", dueLuck: false));
			}
			if (source != null)
			{
				FixedPoint value66 = 0.0;
				if (abilityManager.VisitParameter("LeaderBuffRedactIncreaseHumanDamage", ref value66, source) && target != null && target.Faction == Faction.Raider && combatModel.IsRedacting)
				{
					float num3 = (float)combatModel.RedactTimedEffect.IncreaseDamageRatio * 1f / 100f;
					if (source.Faction == Faction.Survivor)
					{
						MapMissionModel mapMissionModel4 = MapMissionDebuffHelper.CanUseDebuffMission(source.manager);
						TraitEntry traitAnyLevel6 = source.TraitContainer.GetTraitAnyLevel("LeaderBuffRedact");
						if (traitAnyLevel6 == null)
						{
							foreach (ActorModel factionActor4 in combatModel.GetFactionActors(source.Faction))
							{
								if (factionActor4 is SurvivorModel { IsLeader: not false })
								{
									traitAnyLevel6 = factionActor4.TraitContainer.GetTraitAnyLevel("LeaderBuffRedact");
									if (traitAnyLevel6 != null)
									{
										break;
									}
								}
							}
						}
						if (mapMissionModel4 != null && traitAnyLevel6 != null)
						{
							FixedPoint minDebuffParamPercentageByTraitId4 = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel4.GetChallengeDebuffs(), ChallengeDebuffType.DebuffSimonLT, traitAnyLevel6.TraitIdentifier);
							if (minDebuffParamPercentageByTraitId4 > 0L)
							{
								num3 = Math.Min(num3, (float)minDebuffParamPercentageByTraitId4);
							}
						}
					}
					value58 += (FixedPoint)num3;
				}
			}
			if (target.DebuffKnockKnockMarkCount > 0L && source.IsFriendlyHuman)
			{
				FixedPoint value67 = 0.0;
				combatModel.AbilityManager.VisitParameter("LeaderBuffKnockKnockOneMarkDamageMultiplier", ref value67, source);
				value58 += target.DebuffKnockKnockMarkCount * value67;
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffKnockKnock", dueLuck: false));
			}
			if (source.IsHaveOverloadTrait())
			{
				value58 += new FixedPoint(source.ChargeMeter.ChargeLevel) * source.Overload_ChargePointDmgPer();
			}
			if (source.IsHaveOverloadTrait() && source.OverloadStatusLeftTurns > 0)
			{
				value58 += new FixedPoint(source.ChargeMeter.LastChargeConsume) * source.Overload_AddDmgPer();
			}
			if (isAssistAttack)
			{
				FixedPoint value68 = 0.0;
				combatModel.AbilityManager.VisitParameter("OverloadDamageActiveActiveMultiplier", ref value68, source);
				if (value68 > 0.0)
				{
					value58 -= 1L - value68;
				}
			}
			if (isAssistAttack)
			{
				FixedPoint value69 = 0.0;
				combatModel.AbilityManager.VisitParameter("DeadlyFocusEXDamageActiveMultiplier", ref value69, source);
				if (value69 > 0.0)
				{
					value58 -= 1L - value69;
				}
			}
			if (source.DeadlyFocus_EXDamageLayerCount > 0)
			{
				FixedPoint deadlyFocus_TotalEXDamageMultiplier = source.DeadlyFocus_TotalEXDamageMultiplier;
				value58 += deadlyFocus_TotalEXDamageMultiplier;
			}
			if (target.DeadlyFocusLeftCount_SourceRaider > 0 && source.Faction == Faction.Raider)
			{
				FixedPoint leaderBuffDeadlyFocus_ExDmgHitRate_ExDmg = GetLeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg(source, Faction.Raider);
				value58 += leaderBuffDeadlyFocus_ExDmgHitRate_ExDmg;
			}
			if (target.DeadlyFocusLeftCount_SourceSurvivor > 0 && source.Faction == Faction.Survivor)
			{
				FixedPoint leaderBuffDeadlyFocus_ExDmgHitRate_ExDmg2 = GetLeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg(source, Faction.Survivor);
				value58 += leaderBuffDeadlyFocus_ExDmgHitRate_ExDmg2;
			}
			if (target.IsSurvivalGameEnemy())
			{
				FixedPoint value70 = 0.0;
				combatModel.AbilityManager.VisitParameter("LeaderBuffSurvivalGame_DmgUp", ref value70, source);
				value58 += value70;
			}
			if (source.IsSurvivalGameLeadFlag())
			{
				int enemyNegativeCount = source.GetLeaderFaction_SurvivalGameModel().GetEnemyNegativeCount();
				FixedPoint value71 = 0.0;
				combatModel.AbilityManager.VisitParameter("LeaderBuffSurvivalGame_DmgUpEachEff", ref value71, source);
				value58 += value71 * enemyNegativeCount;
			}
			if (source.IsRangedClass && criticalResult != PlayerRandomChanceResult.Failed)
			{
				FixedPoint value72 = 0.0;
				combatModel.AbilityManager.VisitParameter("LeaderBuffNoThreatRangedCriticalIncreaseDamage", ref value72, source);
				if (value72 > 0.0)
				{
					value58 += value72;
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Active_Recoil") && target.IsRecoilEffected)
			{
				FixedPoint value73 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierRecoilDamageReduce", ref value73, source);
				value58 -= value73;
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("Equipment_Active_Recoil", dueLuck: false));
			}
			if (target.IsABTesterAed)
			{
				value58 *= target.GetABtestDamageMultiplier();
			}
			if (source.HasAnyLevelTrait("Heirlooms_RiotGearGlenn_Fetter") && target.IsBurning)
			{
				abilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_BurnDmg", ref value58, source);
			}
			if (source.HasAnyLevelTrait("Equipment_Passive_Detonation") && target.IsBurning)
			{
				abilityManager.VisitParameter("Equipment_Passive_Detonation_Dmg", ref value58, source);
			}
			if (source.HasAnyLevelTrait("Equipment_Passive_Detonation_1") && target.IsBurning)
			{
				abilityManager.VisitParameter("Equipment_Passive_Detonation_Dmg_1", ref value58, source);
			}
			if (type == DamageType.Melee)
			{
				abilityManager.VisitParameter("PercentageIncreaseMeleeDamage", ref value58, source);
				abilityManager.VisitParameter("AbilityModifierEquipPercentageIncreaseMeleeDamage", ref value58, source);
			}
			if (type == DamageType.Ranged)
			{
				abilityManager.VisitParameter("PercentageIncreaseRangeDamage", ref value58, source);
				abilityManager.VisitParameter("PercentageNewIncreaseRangeDamage", ref value58, source);
				FixedPoint value74 = 0.0;
				if (abilityManager.VisitParameter("RangedDamageFalloffRange", ref value74, source))
				{
					FixedPoint fixedPoint21 = value74 * value74;
					if (source.GridCoordinate.SquaredDistanceTo(target.GridCoordinate) > fixedPoint21)
					{
						abilityManager.VisitParameter("RangedDamageFalloffMultiplier", ref value58, source);
					}
				}
			}
			if (survivorModel != null)
			{
				switch (survivorModel.SurvivorClass)
				{
				case SurvivorClass.Assault:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageAssault", ref value58, source);
					break;
				case SurvivorClass.Bruiser:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageBruiser", ref value58, source);
					break;
				case SurvivorClass.Hunter:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageHunter", ref value58, source);
					break;
				case SurvivorClass.Scout:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageScout", ref value58, source);
					break;
				case SurvivorClass.Shooter:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageShooter", ref value58, source);
					break;
				case SurvivorClass.Warrior:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageWarrior", ref value58, source);
					break;
				}
				switch (type)
				{
				case DamageType.Melee:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageMelee", ref value58, source);
					break;
				case DamageType.Ranged:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageRanged", ref value58, source);
					if (combatModel.IsInCover(source.GridCoordinate, target.GridCoordinate) && !combatModel.IsCoverFlanked(source.GridCoordinate, source))
					{
						abilityManager.VisitParameter("PercentageIncreaseRangeDamageInCover", ref value58, source);
					}
					break;
				}
				if (!survivorModel.MoveCompleted || (survivorModel.GetIsUsingAdditionalAttacks() && survivorModel.MoveCompleted))
				{
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageNoMove", ref value58, source);
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalNewDamageNoMove", ref value58, source);
				}
				if (target.IsHuman)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageVsHumans", ref value58, source);
				}
				if (combatModel.IsGuildBattleMission && source.IsFriendlyHuman)
				{
					abilityManager.VisitParameter("GuildBattleAbilityModifierDamage", ref value58, source);
				}
			}
			FixedPoint value75 = 1.0;
			abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageIncrementerBadges", ref value75, source);
			abilityManager.VisitParameter("FiringSquadDamageActiveMultiplier", ref value58, source);
			abilityManager.VisitParameter("AbilityModifierMultiAttackExtraDamageMultiplier", ref value58, source);
			if (isAssistAttack)
			{
				abilityManager.VisitParameter("EquipmentActiveAssistAttackActiveMultiplier", ref value58, source);
			}
			if (source.HasTraitsThatContains("Equipment_Active_ChargeLoad") && source.ChargeLoadFloor > 0.0)
			{
				FixedPoint value76 = 0.0;
				abilityManager.VisitParameter("EquipmentActiveChargeLoadBumpDmgRatio", ref value76, source);
				value58 += value76 * source.ChargeLoadFloor;
			}
			bool flag5 = false;
			if (source.GetWeaponEquipment().HasTemporaryTrait("CoupDeGraceActive"))
			{
				flag5 = abilityManager.VisitParameter("LeaderBuffCoupDeGraceFollowUpDamage", ref value58, source);
			}
			bool flag6 = abilityManager.gameEconomyData.GetEquipmentDefinition(source.SelectedEquipment.EquipmentDefinitionIdentifier).ActiveTraits?.Any((string t) => t.Contains("Equipment_Active_ExtraAP")) ?? false;
			bool flag7 = ability is FiringSquadAbility;
			if (source.CanMoveWithoutAttacking && source.AdditionalAttackCount > 0 && !flag6 && source.FightingFuryActivated && !flag7 && !flag5)
			{
				FixedPoint value77 = 1L;
				if (abilityManager.VisitParameter("LeaderBuffFightingFuryDamageModifier", ref value77, source))
				{
					value77 -= (FixedPoint)1L;
					value58 *= value77;
				}
			}
			FixedPoint value78 = 0.0;
			if (source.BetterTogetherMultiplier > 0)
			{
				if (abilityManager.VisitParameter("LeaderBuffBetterTogetherAdditionalDamageModifier", ref value78, source))
				{
					ApplyBetterTogetherDamageBonus(source, ref value58, value78);
				}
				else
				{
					foreach (ActorModel factionActor5 in combatModel.GetFactionActors(source.Faction))
					{
						if (factionActor5 != source && abilityManager.VisitParameter("LeaderBuffBetterTogetherAdditionalDamageModifier", ref value78, factionActor5))
						{
							ApplyBetterTogetherDamageBonus(source, ref value58, value78);
							break;
						}
					}
				}
			}
			FixedPoint value79 = 0.0;
			if (abilityManager.VisitParameter(AbilityModifierFirstStrike.FirstStrikeAbilityHealthThreshold, ref value79, source))
			{
				FixedPoint value80 = 0.0;
				if (target.MaxHitPoints * value79 <= target.Hitpoints && abilityManager.VisitParameter(AbilityModifierFirstStrike.FirstStrikeAbilityDamageMultiplier, ref value80, source))
				{
					value58 += value80;
				}
			}
			value79 = 0.0;
			if (abilityManager.VisitParameter("FirstStrikeDamageThreshold", ref value79, source))
			{
				FixedPoint value81 = 0.0;
				if (target.MaxHitPoints * value79 <= target.Hitpoints && abilityManager.VisitParameter("FirstStrikeAdditionalDamage", ref value81, source))
				{
					value58 += value81;
					source.NotifyChange("AbilityVisited", new object[3] { "FirstStrike", false, true });
				}
			}
			bool num4 = source.HasAnyLevelTrait("LeaderBuffDeathsDoor") || source.HasAnyLevelTrait("BaseDeathsDoor");
			int num5 = target.DeathsDoor_DmgUpLayer - target.DeathsDoor_DmgUpLayerGainedThisAttack;
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num4 && num5 > 0)
			{
				FixedPoint value82 = 0.0;
				FixedPoint value83 = 0.0;
				ActorModel leaderBuffDeathsDoorMan = GetLeaderBuffDeathsDoorMan(combatModel, source.Faction);
				if (abilityManager.VisitParameter("LeaderBuffDeathsDoor_DmgUpPerLayer", ref value82, leaderBuffDeathsDoorMan))
				{
					abilityManager.VisitParameter("LeaderBuffDeathsDoor_MaxDmgUp", ref value83, leaderBuffDeathsDoorMan);
					FixedPoint fixedPoint22 = FixedPoint.Min(num5 * value82, value83);
					value58 += fixedPoint22;
				}
			}
			if (source.DeathsDoor_IsPursuitAttack)
			{
				FixedPoint value84 = 0.0;
				ActorModel leaderBuffDeathsDoorMan2 = GetLeaderBuffDeathsDoorMan(combatModel, source.Faction);
				if (combatModel.AbilityManager.VisitParameter("LeaderBuffDeathsDoor_PursuitDmgUp", ref value84, leaderBuffDeathsDoorMan2))
				{
					value58 *= value84;
				}
				CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("LeaderBuffDeathsDoor", dueLuck: false));
			}
			modifiedDamage = modifiedDamage * value58 * value75 * value59;
			if (source.ShadowedGuard_Atk > 0)
			{
				fixedPoint18 += (FixedPoint)source.ShadowedGuard_Atk;
			}
			fixedPoint = fixedPoint * value58 * value75 * value59 + fixedPoint18;
			fixedPoint2 = fixedPoint2 * value58 * value59 * value75;
			FixedPoint value85 = 1.0;
			abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamage", ref value85, source);
			if (ability != null && !ability.IsConsumableAbility && source.HasTrait("CarolsCookiesActive"))
			{
				SupportModel supportModel = source.manager.Player.GetSupportModel("CarolsCookies");
				MapMissionModel mapMissionModel5 = source.manager.Player.GetAttackTargetMissionModel() as MapMissionModel;
				if (supportModel.Unlocked && (mapMissionModel5 == null || !mapMissionModel5.IsSupportCoolDown(supportModel.definition)))
				{
					value85 *= 1.0 + supportModel.GetParameter(ability.IsChargeAttack ? 1 : 0) * 0.009999999776482582;
					CheckAndAddDamageNotification(source, ref damageNotifications, new DamageNotificationData("CarolsCookiesTrait", dueLuck: false));
				}
			}
			SkillIncreaseAttackTimedEffect skillIncreaseAttackTimedEffect = source.SkillIncreaseAttackTimedEffect;
			if (ability != null && !ability.IsConsumableAbility && skillIncreaseAttackTimedEffect != null)
			{
				value85 *= 1.0 + (ability.IsChargeAttack ? skillIncreaseAttackTimedEffect.ChargeAttackMultiplier : skillIncreaseAttackTimedEffect.NormalAttackMultiplier);
			}
			fixedPoint *= value85;
			fixedPoint2 *= value85;
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageFinal(modifiedDamage, value58);
			}
			if (source != null && source.manager != null)
			{
				GameEconomyData gameEconomyData2 = source.manager.GameEconomyData;
				if (gameEconomyData2 != null && source.manager.Player.Tutorial.StaticTutorialComplete && gameEconomyData2.WeeklyClassEvents != null)
				{
					for (int num6 = 0; num6 < gameEconomyData2.WeeklyClassEvents.Length; num6++)
					{
						WeeklyClassEvent weeklyClassEvent = gameEconomyData2.WeeklyClassEvents[num6];
						if (combatModel.MapCategory == weeklyClassEvent.MissionCategory && weeklyClassEvent.Multiplier != 0.0)
						{
							if (survivorModel != null && survivorModel.Faction != Faction.Raider && weeklyClassEvent.Affects == WeeklyClassEvent.AffectType.Damage && survivorModel.SurvivorClass == weeklyClassEvent.SurvivorClass)
							{
								fixedPoint *= weeklyClassEvent.Multiplier;
								fixedPoint2 *= weeklyClassEvent.Multiplier;
							}
							else if (survivorModel2 != null && survivorModel2.Faction != Faction.Raider && weeklyClassEvent.Affects == WeeklyClassEvent.AffectType.Defense && survivorModel2.SurvivorClass == weeklyClassEvent.SurvivorClass)
							{
								fixedPoint *= weeklyClassEvent.Multiplier;
								fixedPoint2 *= weeklyClassEvent.Multiplier;
							}
						}
					}
				}
			}
			if (criticalResult != PlayerRandomChanceResult.Failed)
			{
				abilityManager.VisitParameter("AbilityModifierIncreaseCritDamageFlat", ref fixedPoint2, source);
			}
			FixedPoint value86 = 0.0;
			FixedPoint amountDmgReduced = 0.0;
			FixedPoint value87 = 100.0;
			abilityManager.VisitParameter("AbilityModifierPercentageMaxReduceOnCritical", ref value87);
			StrengthenDefense(target, combatModel, ref fixedPoint, ref fixedPoint2);
			FixedPoint fixedPoint23 = fixedPoint;
			FixedPoint fixedPoint24 = fixedPoint2;
			FixedPoint maxDmgWithoutCritAllowedToReduce = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * fixedPoint;
			FixedPoint maxCritDmgAllowedToReduce = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * (value87 / 100.0) * fixedPoint2;
			FixedPoint fixedPoint25 = maxDmgWithoutCritAllowedToReduce;
			FixedPoint fixedPoint26 = maxCritDmgAllowedToReduce;
			FixedPoint reduction = 0.0;
			FixedPoint value88 = 1.0;
			if (source.HasAnyLevelTrait("DebuffMarkEnemy"))
			{
				abilityManager.VisitParameter("LeaderBuffMarkEnemy.DamageReduction", ref value88, source);
				if (maxDmgWithoutCritAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint * (1L - value88), maxDmgWithoutCritAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint -= reduction;
					maxDmgWithoutCritAllowedToReduce -= reduction;
				}
				if (maxCritDmgAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint2 * (1L - value88), maxCritDmgAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint2 -= reduction;
					maxCritDmgAllowedToReduce -= reduction;
				}
			}
			if (source.HasTraitsThatContains("Equipment_Active_Ripped") && target.HasTrait("Skinned"))
			{
				FixedPoint value89 = 0.0;
				abilityManager.VisitParameter("AbilityModifierRippedAdditionalDmgPercent", ref value89, source);
				if (combatModel.manager.Player.RollDice(RollDiceType.Ripped, value89, value47) != PlayerRandomChanceResult.Failed)
				{
					FixedPoint value90 = 0.0;
					FixedPoint value91 = 0.0;
					abilityManager.VisitParameter("AbilityModifierRippedAdditionalDmgRatio", ref value90, source);
					abilityManager.VisitParameter("AbilityModifierRippedAdditionalDmgMaxRatio", ref value91, source);
					FixedPoint a = target.MaxHitPoints * value90;
					FixedPoint fixedPoint27 = ((!(source is SurvivorModel survivorModel7)) ? fixedPoint : ((FixedPoint)survivorModel7.GetDamageForPreferredWeapon()));
					FixedPoint b = fixedPoint27 * value91;
					FixedPoint fixedPoint28 = FixedPoint.Min(a, b);
					fixedPoint2 += fixedPoint28;
					target.NotifyChange("AbilityVisited", new object[2] { "Ripped", false });
				}
			}
			if (source.HasTraitsThatContains("Healthdmg"))
			{
				FixedPoint value92 = 0.0;
				abilityManager.VisitParameter("AbilityModifierExtraHealthDmgMultiplier", ref value92, source);
				if (value92 > 0.0)
				{
					fixedPoint += source.MaxHitPoints * value92;
				}
			}
			if (type != DamageType.Heal)
			{
				FixedPoint value93 = 0.0;
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistance", ref value93, target);
				abilityManager.VisitParameter("SupportTalent_GuardParm3", ref value93, target);
				abilityManager.VisitParameter("AbilityModifierPercentageDecreaseResistance", ref value93, source);
				TraitEntry traitAnyLevel7 = target.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_E");
				if (traitAnyLevel7 != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel7.TraitIdentifier) > 0)
				{
					FixedPoint value94 = 0L;
					abilityManager.VisitParameter("SurvivalManualStorySkill_EParm2", ref value94, target);
					if (FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0) <= value94)
					{
						abilityManager.VisitParameter("SurvivalManualStorySkill_EParm3", ref value93, target);
						target.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_E", false });
						target.NotifyChange("SurvivalManualStorySkill_E");
					}
				}
				TraitEntry traitAnyLevel8 = target.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_H");
				if (traitAnyLevel8 != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel8.TraitIdentifier) > 0)
				{
					abilityManager.VisitParameter("SurvivalManualStorySkill_HParm1", ref value93, target);
					bool flag8 = false;
					FixedPoint value95 = 0.0;
					abilityManager.VisitParameter("SurvivalManualStorySkill_HParm2", ref value95, target);
					foreach (ActorModel factionActor6 in combatModel.GetFactionActors(target.Faction))
					{
						if (!factionActor6.GridCoordinate.Equals(target.GridCoordinate) && factionActor6.GridCoordinate.DistanceTo(target.GridCoordinate) <= value95)
						{
							flag8 = true;
							break;
						}
					}
					if (!flag8)
					{
						FixedPoint value96 = 0L;
						abilityManager.VisitParameter("SurvivalManualStorySkill_HParm3", ref value96, target);
						value93 += value96;
						if (value96 != 0L)
						{
							target.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_H", false });
							target.NotifyChange("SurvivalManualStorySkill_H");
						}
					}
				}
				switch (type)
				{
				case DamageType.Melee:
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMelee", ref value93, target);
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMeleeArmor", ref value93, target);
					break;
				case DamageType.Ranged:
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRanged", ref value93, target);
					break;
				}
				FixedPoint fixedPoint29 = 0.0;
				fixedPoint29 += FixedPoint.Max(0.0, target.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.DmgTotalRefRatio));
				value93 -= fixedPoint29;
				value93 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value93);
				if (maxDmgWithoutCritAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint * value93, maxDmgWithoutCritAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint -= reduction;
					maxDmgWithoutCritAllowedToReduce -= reduction;
				}
				if (maxCritDmgAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint2 * value93, maxCritDmgAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint2 -= reduction;
					maxCritDmgAllowedToReduce -= reduction;
				}
				FixedPoint value97 = 0.0;
				_ = (FixedPoint)0.0;
				if (target.HadActionPointsAtEndOfTurn || target.OverwatchedOnTurn)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceOverwatch", ref value97, target);
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseNewResistanceOverwatch", ref value97, target);
					value97 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value97);
					if (maxDmgWithoutCritAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint * value97, maxDmgWithoutCritAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint -= reduction;
						maxDmgWithoutCritAllowedToReduce -= reduction;
					}
					if (maxCritDmgAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint2 * value97, maxCritDmgAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint2 -= reduction;
						maxCritDmgAllowedToReduce -= reduction;
					}
				}
				if (target.HasAnyLevelTrait("LeaderBuffBodyguard") || combatModel.IsTargetNextToActorWithTrait(target, "LeaderBuffBodyguard"))
				{
					FixedPoint value98 = 0.0;
					abilityManager.VisitParameter("AbilityModifierIncreaseChanceForBodyguard", ref value98, target);
					PlayerRandomChanceResult playerRandomChanceResult3 = combatModel.manager.Player.RollDice(RollDiceType.Generic, value98, value47);
					FixedPoint value99 = 0.0;
					abilityManager.VisitParameter("AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry", ref value99, target);
					if (playerRandomChanceResult3 != PlayerRandomChanceResult.Failed)
					{
						if (maxDmgWithoutCritAllowedToReduce > 0.0)
						{
							reduction = FixedPoint.Min(fixedPoint * value99, maxDmgWithoutCritAllowedToReduce);
							amountDmgReduced += reduction;
							fixedPoint -= reduction;
							maxDmgWithoutCritAllowedToReduce -= reduction;
						}
						if (maxCritDmgAllowedToReduce > 0.0)
						{
							reduction = FixedPoint.Min(fixedPoint2 * value99, maxCritDmgAllowedToReduce);
							amountDmgReduced += reduction;
							fixedPoint2 -= reduction;
							maxCritDmgAllowedToReduce -= reduction;
						}
						target.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffBodyguard",
							playerRandomChanceResult3 == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
				}
				if (combatModel.IsGuildBattleMission && source.IsFriendlyHuman)
				{
					_ = (FixedPoint)0.0;
					FixedPoint value100 = 0.0;
					abilityManager.VisitParameter("GuildBattleAbilityModifierDamageReduction", ref value100, target);
					if (value100 > 0L && maxDmgWithoutCritAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint * value100, maxDmgWithoutCritAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint -= reduction;
						maxDmgWithoutCritAllowedToReduce -= reduction;
					}
				}
				if (combatModel.HasPvPRules)
				{
					FixedPoint value101 = 0.0;
					if (source.IsHuman && target.IsHuman)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceHumanVsHuman", ref value101, target);
					}
					if (source.Faction == Faction.Survivor && target.Faction == Faction.Raider)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceSurvivorVsRaider", ref value101, target);
					}
					if (source.Faction == Faction.Raider && target.Faction == Faction.Survivor)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRaiderVsSurvivor", ref value101, target);
					}
					if (maxDmgWithoutCritAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint * value101, maxDmgWithoutCritAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint -= reduction;
						maxDmgWithoutCritAllowedToReduce -= reduction;
					}
					if (maxCritDmgAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint2 * value101, maxCritDmgAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint2 -= reduction;
						maxCritDmgAllowedToReduce -= reduction;
					}
				}
				if (type == DamageType.Ranged && target.IsHuman && combatModel.IsInCover(target.GridCoordinate, source.GridCoordinate) && !combatModel.IsCoverFlanked(target.GridCoordinate, target))
				{
					value86 = gameEconomyData.ConfigData.HalfCoverModifier;
					abilityManager.VisitParameter("AbilityModifierIncreaseCoverDamageReduction", ref value86, target);
				}
				if (criticalResult != PlayerRandomChanceResult.Failed)
				{
					FixedPoint value102 = 0.0;
					if (source.IsHuman)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceCriticalDamageFromHumans", ref value102, target);
					}
					if (maxDmgWithoutCritAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint * value102, maxDmgWithoutCritAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint -= reduction;
						maxDmgWithoutCritAllowedToReduce -= reduction;
					}
					if (maxCritDmgAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint2 * value102, maxCritDmgAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint2 -= reduction;
						maxCritDmgAllowedToReduce -= reduction;
					}
				}
				value86 = ((value86 > 1.0) ? ((FixedPoint)1.0) : value86);
				if (maxDmgWithoutCritAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint * value86, maxDmgWithoutCritAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint -= reduction;
					maxDmgWithoutCritAllowedToReduce -= reduction;
				}
				if (maxCritDmgAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint2 * value86, maxCritDmgAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint2 -= reduction;
					maxCritDmgAllowedToReduce -= reduction;
				}
				FixedPoint value103 = 0.0;
				if (abilityManager.VisitParameter(AbilityModifierSurvivalInstinct.FetchReduceDamageTaken, ref value103, target) && target.OnRedHealthBar)
				{
					if (maxDmgWithoutCritAllowedToReduce > 0.0 || maxCritDmgAllowedToReduce > 0.0)
					{
						CheckAndAddDamageNotification(target, ref damageNotifications, new DamageNotificationData("LeaderBuffSurvivalInstinct", dueLuck: false));
					}
					if (maxDmgWithoutCritAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint * value103, maxDmgWithoutCritAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint -= reduction;
						maxDmgWithoutCritAllowedToReduce -= reduction;
					}
					if (maxCritDmgAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(fixedPoint2 * value103, maxCritDmgAllowedToReduce);
						amountDmgReduced += reduction;
						fixedPoint2 -= reduction;
						maxCritDmgAllowedToReduce -= reduction;
					}
				}
				if ((type == DamageType.Base || type == DamageType.Melee || type == DamageType.Ranged || type == DamageType.Struggle) && target.IsHuman)
				{
					if (target.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Scout"))
					{
						FixedPoint value104 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutAttackedByHighLevel", ref value104, target);
						if (source.Level - target.Level > value104)
						{
							FixedPoint value105 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutLevelDifference", ref value105, target);
							FixedPoint value106 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutDamageReduction", ref value106, target);
							FixedPoint value107 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaximumLiftingValue", ref value107, target);
							FixedPoint value108 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaxLeveLimitValue", ref value108, target);
							FixedPoint fixedPoint30 = 0L;
							fixedPoint30 = ((!(source.Level - target.Level > value108)) ? ((FixedPoint)Math.Pow((double)(1L - value106), (double)((source.Level - target.Level - value104) / value105))) : ((FixedPoint)Math.Pow((double)(1L - value106), (double)((value108 - value104) / value105))));
							if (fixedPoint30 > 50L)
							{
								fixedPoint30 = 50L;
							}
							if (fixedPoint > 0L)
							{
								reduction = ((!(fixedPoint * fixedPoint30 / fixedPoint23 < value107)) ? (fixedPoint * fixedPoint30) : (fixedPoint23 * value107));
								FixedPoint fixedPoint31 = fixedPoint - reduction;
								fixedPoint = reduction;
								maxDmgWithoutCritAllowedToReduce -= fixedPoint31;
							}
							if (fixedPoint2 > 0L)
							{
								reduction = ((!(fixedPoint2 * fixedPoint30 / fixedPoint24 < value107)) ? (fixedPoint2 * fixedPoint30) : (fixedPoint24 * value107));
								FixedPoint fixedPoint32 = fixedPoint - reduction;
								amountDmgReduced += fixedPoint32;
								fixedPoint2 = reduction;
								maxCritDmgAllowedToReduce -= fixedPoint32;
							}
						}
					}
					if (target.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Bruiser"))
					{
						FixedPoint value109 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserAttackedByHighLevel", ref value109, target);
						if (source.Level - target.Level > value109)
						{
							FixedPoint value110 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserLevelDifference", ref value110, target);
							FixedPoint value111 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserDamageReduction", ref value111, target);
							FixedPoint value112 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaximumLiftingValue", ref value112, target);
							FixedPoint value113 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaxLeveLimitValue", ref value113, target);
							FixedPoint fixedPoint33 = 0L;
							fixedPoint33 = ((!(source.Level - target.Level > value113)) ? ((FixedPoint)Math.Pow((double)(1L - value111), (double)((source.Level - target.Level - value109) / value110))) : ((FixedPoint)Math.Pow((double)(1L - value111), (double)((value113 - value109) / value110))));
							if (fixedPoint33 > 50L)
							{
								fixedPoint33 = 50L;
							}
							if (fixedPoint > 0L)
							{
								reduction = ((!(fixedPoint * fixedPoint33 / fixedPoint23 < value112)) ? (fixedPoint * fixedPoint33) : (fixedPoint23 * value112));
								FixedPoint fixedPoint34 = fixedPoint - reduction;
								fixedPoint = reduction;
								maxDmgWithoutCritAllowedToReduce -= fixedPoint34;
							}
							if (fixedPoint2 > 0L)
							{
								reduction = ((!(fixedPoint2 * fixedPoint33 / fixedPoint24 < value112)) ? (fixedPoint2 * fixedPoint33) : (fixedPoint24 * value112));
								FixedPoint fixedPoint35 = fixedPoint - reduction;
								amountDmgReduced += fixedPoint35;
								fixedPoint2 = reduction;
								maxCritDmgAllowedToReduce -= fixedPoint35;
							}
						}
					}
					if (target.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Warrior"))
					{
						FixedPoint value114 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorAttackedByHighLevel", ref value114, target);
						if (source.Level - target.Level > value114)
						{
							FixedPoint value115 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorLevelDifference", ref value115, target);
							FixedPoint value116 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorDamageReduction", ref value116, target);
							FixedPoint value117 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaximumLiftingValue", ref value117, target);
							FixedPoint value118 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaxLeveLimitValue", ref value118, target);
							FixedPoint fixedPoint36 = 0L;
							fixedPoint36 = ((!(source.Level - target.Level > value118)) ? ((FixedPoint)Math.Pow((double)(1L - value116), (double)((source.Level - target.Level - value114) / value115))) : ((FixedPoint)Math.Pow((double)(1L - value116), (double)((value118 - value114) / value115))));
							if (fixedPoint36 > 50L)
							{
								fixedPoint36 = 50L;
							}
							if (fixedPoint > 0L)
							{
								reduction = ((!(fixedPoint * fixedPoint36 / fixedPoint23 < value117)) ? (fixedPoint * fixedPoint36) : (fixedPoint23 * value117));
								FixedPoint fixedPoint37 = fixedPoint - reduction;
								fixedPoint = reduction;
								maxDmgWithoutCritAllowedToReduce -= fixedPoint37;
							}
							if (fixedPoint2 > 0L)
							{
								reduction = ((!(fixedPoint2 * fixedPoint36 / fixedPoint24 < value117)) ? (fixedPoint2 * fixedPoint36) : (fixedPoint24 * value117));
								FixedPoint fixedPoint38 = fixedPoint - reduction;
								amountDmgReduced += fixedPoint38;
								fixedPoint2 = reduction;
								maxCritDmgAllowedToReduce -= fixedPoint38;
							}
						}
					}
					if (target.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Shooter"))
					{
						FixedPoint value119 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterAttackedByHighLevel", ref value119, target);
						if (source.Level - target.Level > value119)
						{
							FixedPoint value120 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterLevelDifference", ref value120, target);
							FixedPoint value121 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterDamageReduction", ref value121, target);
							FixedPoint value122 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaximumLiftingValue", ref value122, target);
							FixedPoint value123 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaxLeveLimitValue", ref value123, target);
							FixedPoint fixedPoint39 = 0L;
							fixedPoint39 = ((!(source.Level - target.Level > value123)) ? ((FixedPoint)Math.Pow((double)(1L - value121), (double)((source.Level - target.Level - value119) / value120))) : ((FixedPoint)Math.Pow((double)(1L - value121), (double)((value123 - value119) / value120))));
							if (fixedPoint39 > 50L)
							{
								fixedPoint39 = 50L;
							}
							if (fixedPoint > 0L)
							{
								reduction = ((!(fixedPoint * fixedPoint39 / fixedPoint23 < value122)) ? (fixedPoint * fixedPoint39) : (fixedPoint23 * value122));
								FixedPoint fixedPoint40 = fixedPoint - reduction;
								fixedPoint = reduction;
								maxDmgWithoutCritAllowedToReduce -= fixedPoint40;
							}
							if (fixedPoint2 > 0L)
							{
								reduction = ((!(fixedPoint2 * fixedPoint39 / fixedPoint24 < value122)) ? (fixedPoint2 * fixedPoint39) : (fixedPoint24 * value122));
								FixedPoint fixedPoint41 = fixedPoint - reduction;
								amountDmgReduced += fixedPoint41;
								fixedPoint2 = reduction;
								maxCritDmgAllowedToReduce -= fixedPoint41;
							}
						}
					}
					if (target.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Hunter"))
					{
						FixedPoint value124 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterAttackedByHighLevel", ref value124, target);
						if (source.Level - target.Level > value124)
						{
							FixedPoint value125 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterLevelDifference", ref value125, target);
							FixedPoint value126 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterDamageReduction", ref value126, target);
							FixedPoint value127 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaximumLiftingValue", ref value127, target);
							FixedPoint value128 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaxLeveLimitValue", ref value128, target);
							FixedPoint fixedPoint42 = 0L;
							fixedPoint42 = ((!(source.Level - target.Level > value128)) ? ((FixedPoint)Math.Pow((double)(1L - value126), (double)((source.Level - target.Level - value124) / value125))) : ((FixedPoint)Math.Pow((double)(1L - value126), (double)((value128 - value124) / value125))));
							if (fixedPoint42 > 50L)
							{
								fixedPoint42 = 50L;
							}
							if (fixedPoint > 0L)
							{
								reduction = ((!(fixedPoint * fixedPoint42 / fixedPoint23 < value127)) ? (fixedPoint * fixedPoint42) : (fixedPoint23 * value127));
								FixedPoint fixedPoint43 = fixedPoint - reduction;
								fixedPoint = reduction;
								maxDmgWithoutCritAllowedToReduce -= fixedPoint43;
							}
							if (fixedPoint2 > 0L)
							{
								reduction = ((!(fixedPoint2 * fixedPoint42 / fixedPoint24 < value127)) ? (fixedPoint2 * fixedPoint42) : (fixedPoint24 * value127));
								FixedPoint fixedPoint44 = fixedPoint - reduction;
								amountDmgReduced += fixedPoint44;
								fixedPoint2 = reduction;
								maxCritDmgAllowedToReduce -= fixedPoint44;
							}
						}
					}
					if (target.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Assault"))
					{
						FixedPoint value129 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultAttackedByHighLevel", ref value129, target);
						if (source.Level - target.Level > value129)
						{
							FixedPoint value130 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultLevelDifference", ref value130, target);
							FixedPoint value131 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultDamageReduction", ref value131, target);
							FixedPoint value132 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaximumLiftingValue", ref value132, target);
							FixedPoint value133 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaxLeveLimitValue", ref value133, target);
							FixedPoint fixedPoint45 = 0L;
							fixedPoint45 = ((!(source.Level - target.Level > value133)) ? ((FixedPoint)Math.Pow((double)(1L - value131), (double)((source.Level - target.Level - value129) / value130))) : ((FixedPoint)Math.Pow((double)(1L - value131), (double)((value133 - value129) / value130))));
							if (fixedPoint45 > 50L)
							{
								fixedPoint45 = 50L;
							}
							if (fixedPoint > 0L)
							{
								reduction = ((!(fixedPoint * fixedPoint45 / fixedPoint23 < value132)) ? (fixedPoint * fixedPoint45) : (fixedPoint23 * value132));
								FixedPoint fixedPoint46 = fixedPoint - reduction;
								fixedPoint = reduction;
								maxDmgWithoutCritAllowedToReduce -= fixedPoint46;
							}
							if (fixedPoint2 > 0L)
							{
								reduction = ((!(fixedPoint2 * fixedPoint45 / fixedPoint24 < value132)) ? (fixedPoint2 * fixedPoint45) : (fixedPoint24 * value132));
								FixedPoint fixedPoint47 = fixedPoint - reduction;
								amountDmgReduced += fixedPoint47;
								fixedPoint2 = reduction;
								maxCritDmgAllowedToReduce -= fixedPoint47;
							}
						}
					}
				}
			}
			TraitDefinition equipmentTrait = source.GetEquipmentTrait("PointBlankShot");
			FixedPoint value134 = 1.0;
			if (equipmentTrait != null)
			{
				if (IsWithinRange(combatModel, (int)equipmentTrait.GetParameter<FixedPoint>(0), source.GridCoordinate, target.GridCoordinate))
				{
					abilityManager.VisitParameter(AbilityModifierPointBlankShot.FetchIncreasePercentageRanagedDamageWithPointBlankShot, ref value134, source);
				}
				fixedPoint *= value134;
			}
			bool flag9 = false;
			FixedPoint fixedPoint48 = 0L;
			FixedPoint fixedPoint49 = 0L;
			FixedPoint fixedPoint50 = 0L;
			if (source.HasAnyLevelTrait("Equipment_Active_King") && source.EquipmentActiveKingFactor > 0)
			{
				flag9 = true;
				FixedPoint value135 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveKingSuperpositionNumber", ref value135, source);
				FixedPoint value136 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveKingMaxSuperpositionNumber", ref value136, source);
				FixedPoint fixedPoint51 = 0L;
				fixedPoint51 = ((!(source.EquipmentActiveKingFactor > value136)) ? (value135 * source.EquipmentActiveKingFactor) : (value135 * value136));
				fixedPoint48 = fixedPoint51;
			}
			if (source.HasAnyLevelTrait("Equipment_Active_Suppress_1"))
			{
				FixedPoint value137 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveSuppress1CheckNumber", ref value137, source);
				if (target.GridCoordinate.DistanceTo(source.MainTargetCell) < value137 + 1L)
				{
					FixedPoint value138 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveSuppress1BloodRestriction", ref value138, source);
					if (FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0) >= value138)
					{
						flag9 = true;
						FixedPoint value139 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveSuppress1DamageBonus", ref value139, source);
						fixedPoint49 = value139;
						CheckAndAddDamageNotification(target, ref damageNotifications, new DamageNotificationData("Equipment_Active_Suppress_1", dueLuck: false));
					}
				}
			}
			if (source.HasAnyLevelTrait("Equipment_Active_Suppress_2"))
			{
				FixedPoint value140 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveSuppress2CheckNumber", ref value140, source);
				if (target.GridCoordinate.DistanceTo(source.MainTargetCell) < value140 + 1L)
				{
					FixedPoint value141 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveSuppress2BloodRestriction", ref value141, source);
					if (FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0) >= value141)
					{
						flag9 = true;
						FixedPoint value142 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierEquipmentActiveSuppress2DamageBonus", ref value142, source);
						fixedPoint50 = value142;
						CheckAndAddDamageNotification(target, ref damageNotifications, new DamageNotificationData("Equipment_Active_Suppress_2", dueLuck: false));
					}
				}
			}
			if (flag9)
			{
				FixedPoint fixedPoint52 = 1L + fixedPoint48 + fixedPoint49 + fixedPoint50;
				fixedPoint *= fixedPoint52;
				fixedPoint2 *= fixedPoint52;
			}
			if (source.HasTrait("Skinned"))
			{
				FixedPoint value143 = 1.0;
				abilityManager.VisitParameter("SkinnedDebuffMarkReduceAttackPowerPercent", ref value143, source);
				if (maxDmgWithoutCritAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint * (1L - value143), maxDmgWithoutCritAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint -= reduction;
					maxDmgWithoutCritAllowedToReduce -= reduction;
				}
				if (maxCritDmgAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(fixedPoint2 * (1L - value143), maxCritDmgAllowedToReduce);
					amountDmgReduced += reduction;
					fixedPoint2 -= reduction;
					maxCritDmgAllowedToReduce -= reduction;
				}
			}
			FixedPoint damageWithoutCritReduceResult = fixedPoint25 - maxDmgWithoutCritAllowedToReduce;
			FixedPoint additionalCritDamageReduceResult = fixedPoint26 - maxCritDmgAllowedToReduce;
			ApplyAstheniaDamageReduce(combatModel, source, target, ref reduction, ref maxDmgWithoutCritAllowedToReduce, ref maxCritDmgAllowedToReduce, ref amountDmgReduced, ref fixedPoint, ref fixedPoint2);
			ApplyMomentumDamageReduce(combatModel, source, target, ref reduction, ref maxDmgWithoutCritAllowedToReduce, ref maxCritDmgAllowedToReduce, ref amountDmgReduced, ref fixedPoint, ref fixedPoint2, damageWithoutCritReduceResult, additionalCritDamageReduceResult);
			ApplyDefendingheartDamageReduce(combatModel, source, target, ref reduction, ref maxDmgWithoutCritAllowedToReduce, ref maxCritDmgAllowedToReduce, ref amountDmgReduced, ref fixedPoint, ref fixedPoint2);
			if (source.HasTraitsThatContains("Vigilance"))
			{
				FixedPoint value144 = 0.0;
				abilityManager.VisitParameter("AbilityModifierVigilanceDamageMultiplier", ref value144, source);
				if (value144 > 0.0)
				{
					fixedPoint += fixedPoint * (combatModel.ThreatMeter.ThreatLevel * value144);
					fixedPoint2 += fixedPoint2 * (combatModel.ThreatMeter.ThreatLevel * value144);
				}
			}
			if (source.HasTraitsThatContains("Equipment_Active_HPNailgun") && isTriggerExtraAttackDamage)
			{
				FixedPoint value145 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierAttackDamageEnhancement", ref value145, source);
				if (bodyShotResult != PlayerRandomChanceResult.Failed)
				{
					combatModel.AbilityManager.VisitParameter("AbilityModifierExtrAtorsoAttackDamageBoost", ref value145, source);
				}
				fixedPoint = (1L + value145) * fixedPoint;
				fixedPoint2 = (1L + value145) * fixedPoint2;
			}
			if (source != null && combatModel?.TurnManager?.ActiveActor != null && combatModel.TurnManager.ActiveActor != source && source.HasTraitsThatContains("AddDamage.AddAttack"))
			{
				FixedPoint value146 = 0.0;
				FixedPoint value147 = 0.0;
				FixedPoint value148 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierAddDamageAddAttackMinHPPercentage", ref value146, source);
				combatModel.AbilityManager.VisitParameter("AbilityModifierAddDamageAddAttackMaxHPPercentage", ref value147, source);
				combatModel.AbilityManager.VisitParameter("AbilityModifierAddDamageAddAttackExtraDamagePercentage", ref value148, source);
				FixedPoint fixedPoint53 = FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0);
				if (fixedPoint53 >= value146 && fixedPoint53 <= value147)
				{
					fixedPoint = (1L + value148) * fixedPoint;
					fixedPoint2 = (1L + value148) * fixedPoint2;
				}
			}
			if (isChargeAttack && source != null && source.HasTraitsThatContains("AddDamage.ChargeAttack"))
			{
				FixedPoint value149 = 0.0;
				FixedPoint value150 = 0.0;
				FixedPoint value151 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierAddDamageChargeAttackMinHPPercentage", ref value149, source);
				combatModel.AbilityManager.VisitParameter("AbilityModifierAddDamageChargeAttackMaxHPPercentage", ref value150, source);
				combatModel.AbilityManager.VisitParameter("AbilityModifierAddDamageChargeAttackExtraDamagePercentage", ref value151, source);
				FixedPoint fixedPoint54 = FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0);
				if (fixedPoint54 >= value149 && fixedPoint54 <= value150)
				{
					fixedPoint = (1L + value151) * fixedPoint;
					fixedPoint2 = (1L + value151) * fixedPoint2;
				}
			}
			if (source.FocusModeState && !isChargeAttack)
			{
				FixedPoint value152 = 0.0;
				abilityManager.VisitParameter("AbilityModifierFocusModeDamageIncrease", ref value152, source);
				fixedPoint = (1L + value152) * fixedPoint;
				fixedPoint2 = (1L + value152) * fixedPoint2;
			}
			if (source.HasTraitsThatContains("Conductive") && target.GetBeElectronChargeList().Count > 0)
			{
				FixedPoint value153 = 0.0;
				abilityManager.VisitParameter("AbilityModifierConductiveAdditionalDamagePercentage", ref value153, source);
				fixedPoint *= 1L + value153;
				fixedPoint2 *= 1L + value153;
			}
			if (source.HasTraitsThatContains("VoltCharge") && target.GetBeElectronChargeList().Count > 0)
			{
				int value154;
				if (target.IsElectricShocked)
				{
					target.DebuffParameterManager.TryGetParameterValueByParameterKey<int>("ElectronShockAsElectronChargeLayer", out value154);
				}
				else
				{
					value154 = target.GetElectronChargeLayerByFaction(source.Faction);
				}
				if (value154 > 0)
				{
					FixedPoint value155 = 0.0;
					abilityManager.VisitParameter("AbilityModifierVoltChargeAdditionalDamagePercentage", ref value155, source);
					fixedPoint *= 1L + value155 * value154;
					fixedPoint2 *= 1L + value155 * value154;
				}
			}
			FixedPoint fixedPoint55 = CombatTotalDamageBonusHelpers.CalculateTotalDamageBonus(combatModel, source, target, type, criticalResult, ability, isMainTarget, ootType, isAssistAttack, isTriggerExtraAttackDamage);
			if (fixedPoint55 > 0L)
			{
				fixedPoint = (1L + fixedPoint55) * fixedPoint;
				fixedPoint2 = (1L + fixedPoint55) * fixedPoint2;
			}
			if (source.OverloadStatusLeftTurns > 0)
			{
				FixedPoint value156 = 0L;
				if (source.HasAnyLevelTrait("LeaderBuffOverload"))
				{
					combatModel.manager.Player.AbilityManager.VisitParameter("LeaderBuffOverload_LifeDmgPer", ref value156, source);
					MapMissionModel mapMissionModel6 = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
					if (mapMissionModel6 != null && source.Faction == Faction.Survivor && source is SurvivorModel survivorModel8)
					{
						TraitEntry traitAnyLevel9 = source.TraitContainer.GetTraitAnyLevel("LeaderBuffOverload");
						if (traitAnyLevel9 == null)
						{
							foreach (ActorModel factionActor7 in combatModel.GetFactionActors(source.Faction))
							{
								if (factionActor7 is SurvivorModel { IsLeader: not false })
								{
									traitAnyLevel9 = factionActor7.TraitContainer.GetTraitAnyLevel("LeaderBuffOverload");
									if (traitAnyLevel9 != null)
									{
										break;
									}
								}
							}
						}
						if (traitAnyLevel9 != null)
						{
							FixedPoint minDebuffParamPercentageByTraitId5 = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel6.GetChallengeDebuffs(), ChallengeDebuffType.DebuffGauntletAaronLT, traitAnyLevel9.TraitIdentifier);
							if (minDebuffParamPercentageByTraitId5 > 0L)
							{
								int val = (int)(value156 * target.Hitpoints);
								int val2 = (int)(minDebuffParamPercentageByTraitId5 * survivorModel8.GetDamageForPreferredWeapon());
								fixedPoint += (FixedPoint)Math.Min(val2, val);
							}
							else
							{
								fixedPoint += value156 * target.Hitpoints;
							}
						}
					}
					else
					{
						fixedPoint += value156 * target.Hitpoints;
					}
				}
				else if (source.HasAnyLevelTrait("BaseOverload"))
				{
					combatModel.manager.Player.AbilityManager.VisitParameter("BaseLeaderBuffOverload_LifeDmgPer", ref value156, source);
					MapMissionModel mapMissionModel7 = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
					if (mapMissionModel7 != null && source.Faction == Faction.Survivor && source is SurvivorModel survivorModel10)
					{
						TraitEntry traitAnyLevel10 = source.TraitContainer.GetTraitAnyLevel("LeaderBuffOverload");
						if (traitAnyLevel10 == null)
						{
							foreach (ActorModel factionActor8 in combatModel.GetFactionActors(source.Faction))
							{
								if (factionActor8 is SurvivorModel { IsLeader: not false })
								{
									traitAnyLevel10 = factionActor8.TraitContainer.GetTraitAnyLevel("LeaderBuffOverload");
									if (traitAnyLevel10 != null)
									{
										break;
									}
								}
							}
						}
						if (traitAnyLevel10 != null)
						{
							FixedPoint minDebuffParamPercentageByTraitId6 = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel7.GetChallengeDebuffs(), ChallengeDebuffType.DebuffGauntletAaronLT, traitAnyLevel10.TraitIdentifier);
							if (minDebuffParamPercentageByTraitId6 > 0L)
							{
								int val3 = (int)(value156 * target.Hitpoints);
								int val4 = (int)(minDebuffParamPercentageByTraitId6 * survivorModel10.GetDamageForPreferredWeapon());
								fixedPoint += (FixedPoint)Math.Min(val4, val3);
							}
							else
							{
								fixedPoint += value156 * target.Hitpoints;
							}
						}
					}
					else
					{
						fixedPoint += value156 * target.Hitpoints;
					}
				}
			}
			combatModel.manager.CurrentCommandLogEntry?.CalculateDamageAfterReduction(fixedPoint + fixedPoint2, amountDmgReduced, value86);
			if (abilityManager.VisitParameter("TutorialSetDamage", ref fixedPoint, source))
			{
				fixedPoint2 = 0L;
			}
			if (target.HasTrait("TutorialInvulnerable"))
			{
				int struggleBaseThreshold = combatModel.manager.GameEconomyData.ConfigData.StruggleBaseThreshold;
				int num7 = (int)(fixedPoint + fixedPoint2);
				if (num7 >= target.Hitpoints - struggleBaseThreshold - 1)
				{
					num7 = target.Hitpoints - struggleBaseThreshold - 1;
					if (num7 < 0)
					{
						fixedPoint = 0L;
						fixedPoint2 = 0L;
					}
					else
					{
						fixedPoint = num7;
						fixedPoint2 = 0L;
					}
				}
			}
			if (target.HasTrait("StruggleInvulnerable") && target.Faction == Faction.Survivor && target.ExclusiveTimedEffect != null && target.ExclusiveTimedEffect.Type == TimedEffectType.Struggle && target.ExclusiveTimedEffect.Instigator != source)
			{
				fixedPoint = 0L;
				fixedPoint2 = 0L;
			}
			if (target.IsImmuneToStun)
			{
				FixedPoint value157 = 0.0;
				abilityManager.VisitParameter(AbilityModifierOverflow.FetchOverflowMultiplier, ref value157, source);
				fixedPoint += value157 * target.Hitpoints;
			}
			if (source.HasTrait("Special_Stun_Active_Flag") && target.GetWeaponEquipment().HasTemporaryTrait("SpecialStunTargetActiveFlag"))
			{
				criticalResult = PlayerRandomChanceResult.Failed;
				bodyShotResult = PlayerRandomChanceResult.Failed;
				fixedPoint = 0.25 * (double)target.Hitpoints;
				fixedPoint2 = 0L;
				target.GetWeaponEquipment().RemoveTemporaryTraitsByExpirationType(TraitExpirationType.Activation);
			}
			if (target.IsSurvivalGameNoDead())
			{
				if (source.DeathsBlockSecondChance)
				{
					target.NotifyChange("DeathsDoorBlockSecondChance");
				}
				else if (target.Hitpoints - fixedPoint - fixedPoint2 < (double)target.MaxHitPoints * 0.1)
				{
					fixedPoint = (double)target.Hitpoints - (double)target.MaxHitPoints * 0.1;
					fixedPoint2 = 0L;
					target.SurvivalGameNoDeadReduce();
				}
			}
			if (target.CapFirstAttack)
			{
				target.CapFirstAttack = false;
				SupportModel supportModel2 = source.manager.Player.GetSupportModel("Cap");
				if (supportModel2 != null && supportModel2.Unlocked && target.Hitpoints - fixedPoint - fixedPoint2 < target.MaxHitPoints * supportModel2.GetParameter(1) * 0.01 && combatModel.manager.Player.RollDice(RollDiceType.Unknown, supportModel2.GetParameter(0) * 0.01) != PlayerRandomChanceResult.Failed)
				{
					fixedPoint = target.Hitpoints - target.MaxHitPoints * supportModel2.GetParameter(1) * 0.01;
					fixedPoint2 = 0L;
				}
			}
			FixedPoint fixedPoint56 = fixedPoint + fixedPoint2;
			if (fixedPoint < 0L || fixedPoint2 < 0L || fixedPoint56 < 0L || fixedPoint56 > 2147483647L)
			{
				fixedPoint = 2147483647L;
				fixedPoint2 = 0L;
			}
			if ((int)fixedPoint == 0 && (int)fixedPoint2 == 0)
			{
				fixedPoint = 2L;
				fixedPoint2 = 2L;
			}
			combatModel.manager.CurrentCommandLogEntry?.CalculateDamageEnd(new int[2]
			{
				(int)fixedPoint,
				(int)fixedPoint2
			});
			return new int[2]
			{
				(int)fixedPoint,
				(int)fixedPoint2
			};
		}

		private static void ApplyAstheniaDamage(CombatModel combatModel, ActorModel source, ActorModel target, ref FixedPoint modifiedDamage)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			TWDModelManager manager = combatModel.manager;
			if (source.HasTraitsThatContains("Asthenia") && source.ThisAbilityActionAttackUseAsthenia)
			{
				FixedPoint value = 0.0;
				if (abilityManager.VisitParameter("AbilityModifierDamagerActorUpDamagePercentage", ref value, source))
				{
					modifiedDamage += modifiedDamage * value;
				}
			}
			AstheniaRelationsManager model = manager.Player.Combat.GetModel<AstheniaRelationsManager>();
			if (model != null && model.ExistedAstheniaRelations != null)
			{
				AstheniaRelation astheniaRelation = model.ExistedAstheniaRelations.Find((AstheniaRelation x) => source == x.TargetActor);
				if (astheniaRelation != null)
				{
					modifiedDamage -= modifiedDamage * astheniaRelation.MakeEnemyDecreaseAttackPercentage;
				}
			}
		}

		private static void ApplyAstheniaDamageReduce(CombatModel combatModel, ActorModel source, ActorModel target, ref FixedPoint reduction, ref FixedPoint maxDmgWithoutCritAllowedToReduce, ref FixedPoint maxCritDmgAllowedToReduce, ref FixedPoint amountDmgReduced, ref FixedPoint damageWithoutCrit, ref FixedPoint additionalCritDamage)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			AstheniaRelationsManager model = combatModel.manager.Player.Combat.GetModel<AstheniaRelationsManager>();
			FixedPoint fixedPoint = 0.0;
			FixedPoint fixedPoint2 = 0.0;
			if (target.HasTraitsThatContains("Asthenia") && source.GetAstheniaLeftTurns() > 0)
			{
				FixedPoint value = 0.0;
				if (abilityManager.VisitParameter("AbilityModifierDamagerActorDamageReducePercentage", ref value, target))
				{
					if (maxDmgWithoutCritAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(damageWithoutCrit * value, maxDmgWithoutCritAllowedToReduce);
						amountDmgReduced += reduction;
						damageWithoutCrit -= reduction;
						maxDmgWithoutCritAllowedToReduce -= reduction;
					}
					if (maxCritDmgAllowedToReduce > 0.0)
					{
						reduction = FixedPoint.Min(additionalCritDamage * value, maxCritDmgAllowedToReduce);
						amountDmgReduced += reduction;
						additionalCritDamage -= reduction;
						maxCritDmgAllowedToReduce -= reduction;
					}
				}
			}
			if (model == null || model.ExistedAstheniaRelations == null)
			{
				return;
			}
			AstheniaRelation astheniaRelation = model.ExistedAstheniaRelations.Find((AstheniaRelation x) => target == x.TargetActor);
			if (astheniaRelation == null || !(astheniaRelation.MakeEnemyDecreaseDecreaseDamagePercentage > 0.0))
			{
				return;
			}
			fixedPoint = 0.0;
			fixedPoint2 = 0.0;
			if (maxDmgWithoutCritAllowedToReduce > 0.0)
			{
				fixedPoint = damageWithoutCrit * astheniaRelation.MakeEnemyDecreaseDecreaseDamagePercentage;
				if (amountDmgReduced > fixedPoint)
				{
					fixedPoint2 = fixedPoint;
					amountDmgReduced -= fixedPoint2;
				}
				else
				{
					fixedPoint2 = amountDmgReduced;
					amountDmgReduced = 0L;
				}
				damageWithoutCrit += fixedPoint2;
			}
			if (maxCritDmgAllowedToReduce > 0.0)
			{
				fixedPoint = additionalCritDamage * astheniaRelation.MakeEnemyDecreaseDecreaseDamagePercentage;
				if (amountDmgReduced > fixedPoint)
				{
					fixedPoint2 = fixedPoint;
					amountDmgReduced -= fixedPoint2;
				}
				else
				{
					fixedPoint2 = amountDmgReduced;
					amountDmgReduced = 0L;
				}
				additionalCritDamage += fixedPoint2;
			}
		}

		private static void ApplyDefendingheartDamageReduce(CombatModel combatModel, ActorModel source, ActorModel target, ref FixedPoint reduction, ref FixedPoint maxDmgWithoutCritAllowedToReduce, ref FixedPoint maxCritDmgAllowedToReduce, ref FixedPoint amountDmgReduced, ref FixedPoint damageWithoutCrit, ref FixedPoint additionalCritDamage)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			_ = combatModel.manager;
			_ = (FixedPoint)0.0;
			_ = (FixedPoint)0.0;
			if (!target.HasTraitsThatContains("Equipment_Passive_DefendingHeart") || target.DefendingHeartTraitEffectLeftTurns <= 0)
			{
				return;
			}
			FixedPoint value = 0.0;
			if (abilityManager.VisitParameter("Equipment_Passive_DefendingHeartPercentage", ref value, target))
			{
				if (maxDmgWithoutCritAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(damageWithoutCrit * value, maxDmgWithoutCritAllowedToReduce);
					amountDmgReduced += reduction;
					damageWithoutCrit -= reduction;
					maxDmgWithoutCritAllowedToReduce -= reduction;
				}
				if (maxCritDmgAllowedToReduce > 0.0)
				{
					reduction = FixedPoint.Min(additionalCritDamage * value, maxCritDmgAllowedToReduce);
					amountDmgReduced += reduction;
					additionalCritDamage -= reduction;
					maxCritDmgAllowedToReduce -= reduction;
				}
			}
		}

		private static void ApplyMomentumDamageReduce(CombatModel combatModel, ActorModel source, ActorModel target, ref FixedPoint reduction, ref FixedPoint maxDmgWithoutCritAllowedToReduce, ref FixedPoint maxCritDmgAllowedToReduce, ref FixedPoint amountDmgReduced, ref FixedPoint damageWithoutCrit, ref FixedPoint additionalCritDamage, FixedPoint damageWithoutCritReduceResult, FixedPoint additionalCritDamageReduceResult)
		{
			if (!source.HasTraitsThatContains("Riposte"))
			{
				return;
			}
			MomentumTimedEffect momentumTimedEffect = source.MomentumTimedEffect;
			if (momentumTimedEffect == null || momentumTimedEffect.CurrentLayer <= 0)
			{
				return;
			}
			FixedPoint fixedPoint = momentumTimedEffect.ReduceEnemyDamageReductionBase * momentumTimedEffect.CurrentLayer;
			if (!(fixedPoint <= 0.0))
			{
				FixedPoint fixedPoint2 = 0.0;
				if (damageWithoutCritReduceResult > 0.0)
				{
					fixedPoint2 = damageWithoutCritReduceResult * fixedPoint;
					amountDmgReduced -= fixedPoint2;
					maxDmgWithoutCritAllowedToReduce += fixedPoint2;
					damageWithoutCrit += fixedPoint2;
				}
				if (additionalCritDamageReduceResult > 0.0)
				{
					fixedPoint2 = additionalCritDamageReduceResult * fixedPoint;
					amountDmgReduced -= fixedPoint2;
					maxCritDmgAllowedToReduce += fixedPoint2;
					additionalCritDamage += fixedPoint2;
				}
			}
		}

		private static void ApplyBetterTogetherDamageBonus(ActorModel source, ref FixedPoint finalDamageMultiplier, FixedPoint betterTogetherDamageModAmount)
		{
			betterTogetherDamageModAmount *= (FixedPoint)source.BetterTogetherMultiplier;
			finalDamageMultiplier += betterTogetherDamageModAmount;
			source.NotifyChange("AbilityVisited", new object[3] { "LeaderBuffBetterTogether", false, true });
		}

		public static PlayerRandomChanceResult IsBodyShot(CombatModel combatModel, ActorModel source, ActorModel target, AbilityModel ability = null, bool isTriggerExtraAttackDamage = false)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			GameEconomyData gameEconomyData = combatModel.manager.GameEconomyData;
			SurvivorModel survivorModel = source as SurvivorModel;
			SurvivorModel survivorModel2 = target as SurvivorModel;
			int rarityLevel = survivorModel?.SurvivorRarityLevel ?? 0;
			int rarityLevel2 = survivorModel2?.SurvivorRarityLevel ?? 0;
			int num = ((source != null) ? (source.Level + gameEconomyData.GetRarityActorLevelModifier(rarityLevel)) : 0);
			int num2 = ((target != null) ? (target.Level + gameEconomyData.GetRarityActorLevelModifier(rarityLevel2)) : 0) - num;
			if (survivorModel != null && survivorModel.IsHero)
			{
				num2--;
			}
			if (survivorModel2 != null && survivorModel2.IsHero)
			{
				num2++;
			}
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("AbilityModifierRarityModifierFeaturedHero", ref value, source);
			num2 -= (int)value;
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			bool flag;
			bool flag2;
			if (gameEconomyData.GetFeature("BodyshotExploit").Enabled)
			{
				flag = abilityManager.VisitParameterWithAbility((ability == null) ? source.SelectedAbility : ability, AbilityModifierBodyShot.FetchBodyShotChance, ref value2, source);
				flag2 = abilityManager.VisitParameterWithAbility((ability == null) ? source.SelectedAbility : ability, AbilityModifierMeleeBodyShot.FetchMeleeBodyShotChance, ref value3, source);
			}
			else
			{
				flag = abilityManager.VisitParameterWithAbility(source.SelectedAbility, AbilityModifierBodyShot.FetchBodyShotChance, ref value2, source);
				flag2 = abilityManager.VisitParameterWithAbility(source.SelectedAbility, AbilityModifierMeleeBodyShot.FetchMeleeBodyShotChance, ref value3, source);
			}
			if (source.FocusModeState)
			{
				flag = false;
			}
			if (flag || flag2)
			{
				FixedPoint fixedPoint = gameEconomyData.GetLevelBalanceBodyShotChanceModifier(source.Faction, target.Faction, num2);
				if (value2 < 0.0)
				{
					value2 = fixedPoint;
					abilityManager.VisitParameter("AbilityModifierDecreaseBodyshotMeleeChance", ref value2, source);
					if (combatModel.IsGuildBattleMission && source.IsFriendlyHuman)
					{
						abilityManager.VisitParameter("GuildBattleAbilityModifierBodyShotReduction", ref value2, source);
					}
				}
				else if (value3 < 0.0)
				{
					value3 = fixedPoint;
					abilityManager.VisitParameter("AbilityModifierDecreaseBodyshotMeleeChance", ref value3, source);
				}
				else
				{
					value2 += fixedPoint;
					value3 += fixedPoint;
					abilityManager.VisitParameter(AbilityModifierIncreaseBodyShot.FetchIncreaseBodyShotChance, ref value2, target);
					abilityManager.VisitParameter("AbilityModifierDecreaseBodyshotChance", ref value2, source);
					abilityManager.VisitParameter("AbilityModifierDecreaseBodyshotMeleeChance", ref value2, source);
					if (source.IsRangedClass)
					{
						abilityManager.VisitParameter("AbilityModifierDecreaseBodyshotChanceColdBlooded", ref value2, source);
					}
					if (combatModel.IsGuildBattleMission && source.IsFriendlyHuman)
					{
						abilityManager.VisitParameter("GuildBattleAbilityModifierBodyShotReduction", ref value2, source);
					}
					abilityManager.VisitParameter(AbilityModifierIncreaseMeleeBodyShot.FetchIncreaseMeleeBodyShotChance, ref value3, target);
					abilityManager.VisitParameter("AbilityModifierDecreaseBodyshotMeleeChance", ref value3, source);
				}
				if (source.HasAnyLevelTrait("Equipment_Apocalyptic_BS_Scout"))
				{
					FixedPoint value4 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierBSScoutAttackingAHighRanking", ref value4, source);
					if (target.Level - source.Level > value4)
					{
						FixedPoint value5 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSScoutLevelDifference", ref value5, source);
						FixedPoint value6 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSScoutProbabilityReduction", ref value6, source);
						FixedPoint value7 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSScoutMaximumLiftingValue", ref value7, source);
						FixedPoint fixedPoint2 = value2 - value6 * ((target.Level - source.Level - value4) / value5);
						if (fixedPoint2 < value7)
						{
							fixedPoint2 = value7;
						}
						value2 = fixedPoint2;
					}
				}
				if (source.HasAnyLevelTrait("Equipment_Apocalyptic_BS_Bruiser"))
				{
					FixedPoint value8 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierBSBruiserAttackingAHighRanking", ref value8, source);
					if (target.Level - source.Level > value8)
					{
						FixedPoint value9 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSBruiserLevelDifference", ref value9, source);
						FixedPoint value10 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSBruiserProbabilityReduction", ref value10, source);
						FixedPoint value11 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSBruiserMaximumLiftingValue", ref value11, source);
						FixedPoint fixedPoint3 = value2 - value10 * ((target.Level - source.Level - value8) / value9);
						if (fixedPoint3 < value11)
						{
							fixedPoint3 = value11;
						}
						value2 = fixedPoint3;
					}
				}
				if (source.HasAnyLevelTrait("Equipment_Apocalyptic_BS_Warrior"))
				{
					FixedPoint value12 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierBSWarriorAttackingAHighRanking", ref value12, source);
					if (target.Level - source.Level > value12)
					{
						FixedPoint value13 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSWarriorLevelDifference", ref value13, source);
						FixedPoint value14 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSWarriorProbabilityReduction", ref value14, source);
						FixedPoint value15 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSWarriorMaximumLiftingValue", ref value15, source);
						FixedPoint fixedPoint4 = value2 - value14 * ((target.Level - source.Level - value12) / value13);
						if (fixedPoint4 < value15)
						{
							fixedPoint4 = value15;
						}
						value2 = fixedPoint4;
					}
				}
				if (source.HasAnyLevelTrait("Equipment_Apocalyptic_BS_Shooter"))
				{
					FixedPoint value16 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierBSShooterAttackingAHighRanking", ref value16, source);
					if (target.Level - source.Level > value16)
					{
						FixedPoint value17 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSShooterLevelDifference", ref value17, source);
						FixedPoint value18 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSShooterProbabilityReduction", ref value18, source);
						FixedPoint value19 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSShooterMaximumLiftingValue", ref value19, source);
						FixedPoint fixedPoint5 = value2 - value18 * ((target.Level - source.Level - value16) / value17);
						if (fixedPoint5 < value19)
						{
							fixedPoint5 = value19;
						}
						value2 = fixedPoint5;
					}
				}
				if (source.HasAnyLevelTrait("Equipment_Apocalyptic_BS_Hunter"))
				{
					FixedPoint value20 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierBSHunterAttackingAHighRanking", ref value20, source);
					if (target.Level - source.Level > value20)
					{
						FixedPoint value21 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSHunterLevelDifference", ref value21, source);
						FixedPoint value22 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSHunterProbabilityReduction", ref value22, source);
						FixedPoint value23 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSHunterMaximumLiftingValue", ref value23, source);
						FixedPoint fixedPoint6 = value2 - value22 * ((target.Level - source.Level - value20) / value21);
						if (fixedPoint6 < value23)
						{
							fixedPoint6 = value23;
						}
						value2 = fixedPoint6;
					}
				}
				if (source.HasAnyLevelTrait("Equipment_Apocalyptic_BS_Assault"))
				{
					FixedPoint value24 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierBSAssaultAttackingAHighRanking", ref value24, source);
					if (target.Level - source.Level > value24)
					{
						FixedPoint value25 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSAssaultLevelDifference", ref value25, source);
						FixedPoint value26 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSAssaultProbabilityReduction", ref value26, source);
						FixedPoint value27 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierBSAssaultMaximumLiftingValue", ref value27, source);
						FixedPoint fixedPoint7 = value2 - value26 * ((target.Level - source.Level - value24) / value25);
						if (fixedPoint7 < value27)
						{
							fixedPoint7 = value27;
						}
						value2 = fixedPoint7;
					}
				}
				TraitEntry traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_A");
				if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) > 0)
				{
					FixedPoint value28 = 0.0;
					combatModel.AbilityManager.VisitParameter("SurvivalManualDecreaseBodyshotChance", ref value28, source);
					if (target.Level - source.Level > 0)
					{
						FixedPoint value29 = 0.0;
						combatModel.AbilityManager.VisitParameter("SurvivalManualMaxDecreaseBodyshotChance", ref value29, source);
						FixedPoint fixedPoint8 = 0L;
						fixedPoint8 = ((!(value28 * (target.Level - source.Level) >= value29)) ? (value28 * (target.Level - source.Level)) : value29);
						value2 -= fixedPoint8;
						if (value2 <= 0L)
						{
							value2 = 0L;
						}
						if (value29 != 0L)
						{
							source.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_A", false });
						}
					}
				}
				TraitEntry traitAnyLevel2 = source.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_I");
				if (traitAnyLevel2 != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel2.TraitIdentifier) > 0)
				{
					FixedPoint value30 = 0.0;
					combatModel.AbilityManager.VisitParameter("SurvivalManualStorySkill_IParm2", ref value30, source);
					if (IsWithinRange(combatModel, (int)value30, source.GridCoordinate, target.GridCoordinate))
					{
						FixedPoint value31 = 0.0;
						combatModel.AbilityManager.VisitParameter("SurvivalManualStorySkill_IParm3", ref value31, source);
						value2 -= value31;
						if (value2 <= 0L)
						{
							value2 = 0L;
						}
						if (value31 != 0L)
						{
							source.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_I", false });
							source.NotifyChange("SurvivalManualStorySkill_I");
						}
					}
				}
				if (source.HasAnyLevelTrait("SupportTalent_BodyshootRate"))
				{
					FixedPoint value32 = 0.0;
					combatModel.AbilityManager.VisitParameter("SupportTalent_BodyshootRateParm1", ref value32, source);
					value2 += value32;
					if (value2 <= 0L)
					{
						value2 = 0L;
					}
				}
				if (target.HasAnyLevelTrait("SupportTalent_RefBodyshootRate"))
				{
					FixedPoint value33 = 0.0;
					combatModel.AbilityManager.VisitParameter("SupportTalent_BodyshootRateRefParm1", ref value33, target);
					value2 += value33;
					if (value2 <= 0L)
					{
						value2 = 0L;
					}
				}
				FixedPoint value34 = 0.0;
				if (abilityManager.VisitParameter(AbilityModifierFirstStrike.FirstStrikeAbilityHealthThreshold, ref value34, source))
				{
					FixedPoint value35 = 0.0;
					if (target.Hitpoints >= target.MaxHitPoints * value34 && abilityManager.VisitParameter(AbilityModifierFirstStrike.FirstStrikeAbilityDamageMultiplier, ref value35, source))
					{
						return PlayerRandomChanceResult.Failed;
					}
				}
				value34 = 0.0;
				if (abilityManager.VisitParameter("FirstStrikeDamageThreshold", ref value34, source))
				{
					FixedPoint value36 = 0.0;
					if (target.Hitpoints >= target.MaxHitPoints)
					{
						return PlayerRandomChanceResult.Failed;
					}
					if (target.Hitpoints >= target.MaxHitPoints * value34 && abilityManager.VisitParameter("FirstStrikeAdditionalDamage", ref value36, source))
					{
						return PlayerRandomChanceResult.Failed;
					}
				}
				if (source.IsSneak && source.HasAnyLevelTrait("LeaderBuffCriticalChance"))
				{
					FixedPoint value37 = 0.0;
					abilityManager.VisitParameter("AbilityModifierCarolCriticalChance", ref value37, source);
					if (combatModel.manager.Player.RollDice(RollDiceType.BodyShot, value37) != PlayerRandomChanceResult.Failed)
					{
						return PlayerRandomChanceResult.Failed;
					}
				}
				return combatModel.manager.Player.RollDice(RollDiceType.BodyShot, flag ? value2 : value3);
			}
			return PlayerRandomChanceResult.Failed;
		}

		public static bool ExecuteDamage(CombatModel combatModel, ActorModel source, ActorModel target, int damage, int additionalCritDamage, DamageType type, PlayerRandomChanceResult criticalResult, PlayerRandomChanceResult bodyShotResult, Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications = null, bool dealDamagePostAbility = false, SupportModel sourceSupport = null, bool noChargeGain = false, ActorModel originalDamageInstigator = null, bool isMainTarget = false, bool isTriggerExtraAttackDamage = false, OOTType ootType = OOTType.None, bool isChargeAttack = false, bool isEquipmentKaboomReflect = false, bool applyIncomingDamageMitigation = false)
		{
			bool flag = false;
			if (target != null)
			{
				if (type == DamageType.Heal)
				{
					damage *= -1;
				}
				if (!(combatModel.Manager is TWDModelManager tWDModelManager))
				{
					return false;
				}
				damageNotifications = damageNotifications ?? new Dictionary<ActorModel, List<DamageNotificationData>>();
				bool flag2 = source?.GetTraitWithTag("PushCollisionDamage") != null;
				bool critical = criticalResult != PlayerRandomChanceResult.Failed;
				bool bodyShot = bodyShotResult != PlayerRandomChanceResult.Failed;
				if (isTriggerExtraAttackDamage && source.HasTraitsThatContains("Wrestler") && !source.dashTraitAttackFlag && source.HasTraitsThatContains("Equipment_Active_HPNailgun"))
				{
					FixedPoint value = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierAttackDamageEnhancement", ref value, source);
					if (bodyShotResult != PlayerRandomChanceResult.Failed)
					{
						combatModel.AbilityManager.VisitParameter("AbilityModifierExtrAtorsoAttackDamageBoost", ref value, source);
					}
					damage = (int)((1L + value) * damage);
				}
				if (source != null && source.HasTraitsThatContains("NegativeFlagFatalFlag") && target.Hitpoints > damage + additionalCritDamage && target.Iskill)
				{
					damage = target.MaxHitPoints;
				}
				if (damage < 0)
				{
					damage = int.MaxValue;
				}
				if (applyIncomingDamageMitigation && type != DamageType.Heal && damage > 0 && damage != int.MaxValue)
				{
					int num = damage + additionalCritDamage;
					if (num > 0)
					{
						damage = ApplyIncomingDamageMitigation(combatModel, source, target, num, type);
						additionalCritDamage = 0;
					}
				}
				DamageAction damageAction = new DamageAction(target, source, damage, additionalCritDamage, bodyShot, critical, criticalResult, type, target.Faction, damageNotifications, noChargeGain, sourceSupport, originalDamageInstigator, isMainTarget, isTriggerExtraAttackDamage, isChargeAttack);
				damageAction.IsPushDamage = flag2;
				damageAction.IsEquipmentKaboomReflect = isEquipmentKaboomReflect;
				if (dealDamagePostAbility && combatModel.AbilityManager.AbilityUnderApplication != null)
				{
					damageAction.DealDamagePostAbility = true;
					combatModel.AbilityManager.AbilityUnderApplication.PostExecuteActions.Add(damageAction);
				}
				flag = tWDModelManager.ExecuteAction(damageAction);
				if (flag && (!damageAction.Dodged || (damageAction.Dodged && damageAction.Critical)))
				{
					if (!dealDamagePostAbility && !damageAction.DealDamagePostAbility)
					{
						bool receivedFireDamage = false;
						if (!flag2)
						{
							CheckForBurningEffects(tWDModelManager, source, target, damageAction, ref receivedFireDamage, sourceSupport);
						}
						CheckForStruggle(tWDModelManager, source, target, damageAction, receivedFireDamage);
					}
					PlayerRandomChanceResult playerRandomChanceResult;
					if ((playerRandomChanceResult = ShouldGainCharge(source, target, tWDModelManager.Player, "AbilityModifierIncreaseExtraChargePointAtAttackDmgChance")) != PlayerRandomChanceResult.Failed)
					{
						target.AddChargePoints(1);
						damageAction.TargetGotChargePoint = true;
						target.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffExtraChargePointAtAttackDmgTaken",
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
					if (target.HasTrait("CommonwealthArmorActive") && target.ChargeMeter != null && target.ChargeMeter.ChargeLevel < target.ChargeMeter.MaxLevel && (playerRandomChanceResult = ShouldGainCharge(source, target, tWDModelManager.Player, "CommonwealthArmorExtraChargeChance")) != PlayerRandomChanceResult.Failed)
					{
						target.AddChargePoints(1);
						damageAction.TargetGotChargePoint = true;
						target.NotifyChange("AbilityVisited", new object[2]
						{
							"CommonwealthArmorExtraChargeChance",
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
					CheckAPJadis(tWDModelManager.CombatModel, source, target);
					tWDModelManager.ExecuteAction(new PostDamageAction(damageAction, target, source, isMainTarget, isChargeAttack, isTriggerExtraAttackDamage));
					if (ShouldActorExplode(source, target, type, damage))
					{
						TraitDefinition traitWithTag = target.GetTraitWithTag("Explosive");
						target.Explode(traitWithTag.Identifier);
					}
					if (type == DamageType.ShadowedGuard)
					{
						target.VengefulCharge_LeaderBuffShadowedGuard();
					}
				}
				if (target.IsDead)
				{
					if (target.DebuffParameterManager != null)
					{
						tWDModelManager.CombatModel.TurnManager.FactionChanged -= target.DebuffParameterManager.RemoveExpiryParameterOnFactionChanged;
					}
					if (target.CoexistTimedEffectsManager != null)
					{
						tWDModelManager.CombatModel.TurnManager.FactionChanged -= target.CoexistTimedEffectsManager.OnFactionChanged;
					}
					if (target.CommandSkillModelManager != null)
					{
						tWDModelManager.CombatModel.TurnManager.FactionChanged -= target.CommandSkillModelManager.OnFactionChanged;
					}
				}
			}
			return flag;
		}

		public static bool ShouldActorExplode(ActorModel source, ActorModel target, DamageType type, int damage)
		{
			bool num = source?.GetTraitWithTag("PushCollisionDamage") != null;
			TraitDefinition traitWithTag = target.GetTraitWithTag("Explosive");
			if (num || target == source || (type != DamageType.Ranged && type != DamageType.Explosion) || traitWithTag == null)
			{
				return false;
			}
			WalkerExplosionDefinition walkerExplosionDefinition = target.manager.GameEconomyData.GetWalkerExplosionDefinition(traitWithTag.Identifier);
			if (walkerExplosionDefinition == null)
			{
				return false;
			}
			if (walkerExplosionDefinition.ExplodeOnKill)
			{
				return target.Hitpoints - damage <= 0;
			}
			return true;
		}

		public static int ApplyIncomingDamageMitigation(CombatModel combatModel, ActorModel source, ActorModel target, int damage, DamageType type, FixedPoint luck = default(FixedPoint))
		{
			if (combatModel == null || target == null || damage <= 0 || type == DamageType.Heal)
			{
				return Math.Max(0, damage);
			}
			if (damage == int.MaxValue)
			{
				return damage;
			}
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			if (abilityManager == null || combatModel.manager?.GameEconomyData?.ConfigData == null)
			{
				return damage;
			}
			FixedPoint damageWithoutCrit = damage;
			FixedPoint additionalCritDamage = 0.0;
			FixedPoint fixedPoint = 0.0;
			StrengthenDefense(target, combatModel, ref damageWithoutCrit, ref additionalCritDamage);
			FixedPoint fixedPoint2 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * damageWithoutCrit;
			if (source != null && source.HasAnyLevelTrait("DebuffMarkEnemy"))
			{
				FixedPoint value = 1.0;
				abilityManager.VisitParameter("LeaderBuffMarkEnemy.DamageReduction", ref value, source);
				if (fixedPoint2 > 0.0)
				{
					fixedPoint = FixedPoint.Min(damageWithoutCrit * (1L - value), fixedPoint2);
					damageWithoutCrit -= fixedPoint;
					fixedPoint2 -= fixedPoint;
				}
			}
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistance", ref value2, target);
			abilityManager.VisitParameter("SupportTalent_GuardParm3", ref value2, target);
			if (source != null)
			{
				abilityManager.VisitParameter("AbilityModifierPercentageDecreaseResistance", ref value2, source);
			}
			TraitEntry traitAnyLevel = target.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_E");
			if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) > 0)
			{
				FixedPoint value3 = 0L;
				abilityManager.VisitParameter("SurvivalManualStorySkill_EParm2", ref value3, target);
				if (FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0) <= value3)
				{
					abilityManager.VisitParameter("SurvivalManualStorySkill_EParm3", ref value2, target);
					target.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_E", false });
					target.NotifyChange("SurvivalManualStorySkill_E");
				}
			}
			TraitEntry traitAnyLevel2 = target.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_H");
			if (traitAnyLevel2 != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel2.TraitIdentifier) > 0)
			{
				abilityManager.VisitParameter("SurvivalManualStorySkill_HParm1", ref value2, target);
				bool flag = false;
				FixedPoint value4 = 0.0;
				abilityManager.VisitParameter("SurvivalManualStorySkill_HParm2", ref value4, target);
				foreach (ActorModel factionActor in combatModel.GetFactionActors(target.Faction))
				{
					if (!factionActor.GridCoordinate.Equals(target.GridCoordinate) && factionActor.GridCoordinate.DistanceTo(target.GridCoordinate) <= value4)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					FixedPoint value5 = 0L;
					abilityManager.VisitParameter("SurvivalManualStorySkill_HParm3", ref value5, target);
					value2 += value5;
					if (value5 != 0L)
					{
						target.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_H", false });
						target.NotifyChange("SurvivalManualStorySkill_H");
					}
				}
			}
			switch (type)
			{
			case DamageType.Melee:
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMelee", ref value2, target);
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMeleeArmor", ref value2, target);
				break;
			case DamageType.Ranged:
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRanged", ref value2, target);
				break;
			}
			FixedPoint fixedPoint3 = 0.0;
			fixedPoint3 += FixedPoint.Max(0.0, target.GetSnapshotCombatAttributeValueByAttributeType(AttributeType.DmgTotalRefRatio));
			value2 -= fixedPoint3;
			value2 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value2);
			if (fixedPoint2 > 0.0)
			{
				fixedPoint = FixedPoint.Min(damageWithoutCrit * value2, fixedPoint2);
				damageWithoutCrit -= fixedPoint;
				fixedPoint2 -= fixedPoint;
			}
			if (target.HadActionPointsAtEndOfTurn || target.OverwatchedOnTurn)
			{
				FixedPoint value6 = 0.0;
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceOverwatch", ref value6, target);
				abilityManager.VisitParameter("AbilityModifierPercentageIncreaseNewResistanceOverwatch", ref value6, target);
				value6 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value6);
				if (fixedPoint2 > 0.0)
				{
					fixedPoint = FixedPoint.Min(damageWithoutCrit * value6, fixedPoint2);
					damageWithoutCrit -= fixedPoint;
					fixedPoint2 -= fixedPoint;
				}
			}
			if (target.HasAnyLevelTrait("LeaderBuffBodyguard") || combatModel.IsTargetNextToActorWithTrait(target, "LeaderBuffBodyguard"))
			{
				FixedPoint value7 = 0.0;
				abilityManager.VisitParameter("AbilityModifierIncreaseChanceForBodyguard", ref value7, target);
				PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.Generic, value7, luck);
				FixedPoint value8 = 0.0;
				abilityManager.VisitParameter("AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry", ref value8, target);
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					if (fixedPoint2 > 0.0)
					{
						fixedPoint = FixedPoint.Min(damageWithoutCrit * value8, fixedPoint2);
						damageWithoutCrit -= fixedPoint;
						fixedPoint2 -= fixedPoint;
					}
					target.NotifyChange("AbilityVisited", new object[2]
					{
						"LeaderBuffBodyguard",
						playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
					});
				}
			}
			if (combatModel.IsGuildBattleMission && source != null && source.IsFriendlyHuman)
			{
				FixedPoint value9 = 0.0;
				abilityManager.VisitParameter("GuildBattleAbilityModifierDamageReduction", ref value9, target);
				if (value9 > 0L && fixedPoint2 > 0.0)
				{
					fixedPoint = FixedPoint.Min(damageWithoutCrit * value9, fixedPoint2);
					damageWithoutCrit -= fixedPoint;
					fixedPoint2 -= fixedPoint;
				}
			}
			if (combatModel.HasPvPRules && source != null)
			{
				FixedPoint value10 = 0.0;
				if (source.IsHuman && target.IsHuman)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceHumanVsHuman", ref value10, target);
				}
				if (source.Faction == Faction.Survivor && target.Faction == Faction.Raider)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceSurvivorVsRaider", ref value10, target);
				}
				if (source.Faction == Faction.Raider && target.Faction == Faction.Survivor)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRaiderVsSurvivor", ref value10, target);
				}
				if (fixedPoint2 > 0.0)
				{
					fixedPoint = FixedPoint.Min(damageWithoutCrit * value10, fixedPoint2);
					damageWithoutCrit -= fixedPoint;
					fixedPoint2 -= fixedPoint;
				}
			}
			FixedPoint value11 = 0.0;
			if (abilityManager.VisitParameter(AbilityModifierSurvivalInstinct.FetchReduceDamageTaken, ref value11, target) && target.OnRedHealthBar && fixedPoint2 > 0.0)
			{
				fixedPoint = FixedPoint.Min(damageWithoutCrit * value11, fixedPoint2);
				damageWithoutCrit -= fixedPoint;
				fixedPoint2 -= fixedPoint;
			}
			return (int)FixedPoint.Max(0L, damageWithoutCrit + additionalCritDamage);
		}

		public static void StrengthenDefense(ActorModel target, CombatModel combatModel, ref FixedPoint damageWithoutCrit, ref FixedPoint additionalCritDamage)
		{
			if (target == null || combatModel == null || combatModel.AbilityManager == null)
			{
				return;
			}
			if (target.HasTraitsThatContains("StrengthenDefenseFunc1"))
			{
				FixedPoint value = 0.0;
				combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc1Param1", ref value, target);
				FixedPoint value2 = 0.0;
				combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc1Param2", ref value2, target);
				FixedPoint value3 = 0.0;
				combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc1Param3", ref value3, target);
				if (!(damageWithoutCrit < 0L) && !(additionalCritDamage < 0L))
				{
					FixedPoint fixedPoint = damageWithoutCrit;
					FixedPoint fixedPoint2 = additionalCritDamage;
					FixedPoint fixedPoint3 = damageWithoutCrit + additionalCritDamage;
					FixedPoint fixedPoint4 = 0L;
					FixedPoint fixedPoint5 = 0L;
					if (damageWithoutCrit > 0L && fixedPoint3 > 0L)
					{
						fixedPoint4 = damageWithoutCrit * 1.0 / fixedPoint3;
					}
					if (additionalCritDamage > 0L && fixedPoint3 > 0L)
					{
						fixedPoint5 = 1L - fixedPoint4;
					}
					damageWithoutCrit = damageWithoutCrit - damageWithoutCrit * value - (value2 + target.MaxHitPoints * value3) * fixedPoint4;
					additionalCritDamage = additionalCritDamage - additionalCritDamage * value - (value2 + target.MaxHitPoints * value3) * fixedPoint5;
					if (fixedPoint > 2L && damageWithoutCrit <= 2L)
					{
						damageWithoutCrit = 2L;
					}
					if (fixedPoint2 > 2L && additionalCritDamage <= 2L)
					{
						additionalCritDamage = 2L;
					}
				}
			}
			if (!target.HasTraitsThatContains("StrengthenDefenseFunc2"))
			{
				return;
			}
			FixedPoint value4 = 0.0;
			combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc2Param1", ref value4, target);
			FixedPoint value5 = 0.0;
			combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc2Param2", ref value5, target);
			FixedPoint value6 = 0.0;
			combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc2Param3", ref value6, target);
			FixedPoint value7 = 0.0;
			combatModel.AbilityManager.VisitParameter("StrengthenDefenseFunc2Param4", ref value7, target);
			if (FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0) <= value7)
			{
				FixedPoint fixedPoint6 = damageWithoutCrit;
				FixedPoint fixedPoint7 = additionalCritDamage;
				FixedPoint fixedPoint8 = damageWithoutCrit + additionalCritDamage;
				FixedPoint fixedPoint9 = 0L;
				FixedPoint fixedPoint10 = 0L;
				if (damageWithoutCrit > 0L && fixedPoint8 > 0L)
				{
					fixedPoint9 = damageWithoutCrit * 1.0 / fixedPoint8;
				}
				if (additionalCritDamage > 0L && fixedPoint8 > 0L)
				{
					fixedPoint10 = 1L - fixedPoint9;
				}
				damageWithoutCrit = damageWithoutCrit - damageWithoutCrit * value4 - (value5 + target.MaxHitPoints * value6) * fixedPoint9;
				additionalCritDamage = additionalCritDamage - additionalCritDamage * value4 - (value5 + target.MaxHitPoints * value6) * fixedPoint10;
				if (fixedPoint6 > 2L && damageWithoutCrit <= 2L)
				{
					damageWithoutCrit = 2L;
				}
				if (fixedPoint7 > 2L && additionalCritDamage <= 2L)
				{
					additionalCritDamage = 2L;
				}
			}
		}

		public static bool ExecuteDamageConsumable(CombatModel combatModel, ActorModel source, ActorModel target, int damage, int additionalCritDamage, DamageType type, PlayerRandomChanceResult criticalResult, PlayerRandomChanceResult bodyShotResult, Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications = null, bool dealDamagePostAbility = false)
		{
			if (target == null)
			{
				return false;
			}
			bool flag = false;
			TWDModelManager tWDModelManager = combatModel.Manager as TWDModelManager;
			bool isPushDamage = source.GetTraitWithTag("PushCollisionDamage") != null;
			if (type == DamageType.Heal)
			{
				flag &= tWDModelManager.ExecuteAction(new HealAction(source, target, damage));
			}
			else
			{
				bool critical = criticalResult != PlayerRandomChanceResult.Failed;
				bool bodyShot = bodyShotResult != PlayerRandomChanceResult.Failed;
				damageNotifications = damageNotifications ?? new Dictionary<ActorModel, List<DamageNotificationData>>();
				DamageConsumableAction damageConsumableAction = new DamageConsumableAction(target, source, damage, additionalCritDamage, bodyShot, critical, criticalResult, type, target.Faction, damageNotifications);
				damageConsumableAction.IsPushDamage = isPushDamage;
				flag = tWDModelManager.ExecuteAction(damageConsumableAction);
				if (flag && (!damageConsumableAction.Dodged || (damageConsumableAction.Dodged && damageConsumableAction.Critical)))
				{
					if (!dealDamagePostAbility)
					{
						CheckForStruggle(tWDModelManager, source, target, damageConsumableAction);
					}
					if (ShouldActorExplode(source, target, type, damage))
					{
						TraitDefinition traitWithTag = target.GetTraitWithTag("Explosive");
						target.Explode(traitWithTag.Identifier);
					}
				}
			}
			return flag;
		}

		public static void CheckForBurningEffects(TWDModelManager modelManager, ActorModel source, ActorModel target, DamageAction damageAction, ref bool receivedFireDamage, SupportModel sourceSupport = null)
		{
			if (damageAction.FinalDamage > 0 && source != null && !source.HasAnyLevelTrait("DebuffMarkEnemy"))
			{
				FixedPoint value = 0.0;
				modelManager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, source);
				FixedPoint value2 = damageAction.FinalDamage;
				modelManager.CombatModel.AbilityManager.VisitParameter(AbilityModifierGiveTrait.RollForTrait, ref value2, target);
				modelManager.CombatModel.AbilityManager.VisitParameter(AbilityModifierGivePunishTrait.RollForPunishTrait, ref value2, target);
				FixedPoint value3 = 0.0;
				modelManager.CombatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseChanceToSetTargetOnFire", ref value3, source);
				modelManager.CombatModel.AbilityManager.VisitParameter("HeirloomsMaggiePocketWatchBurnChance", ref value3, source);
				if (target.AttributeModel?.GetAttributeModelValue("burn_be_ratio") != 0L)
				{
					FixedPoint value4 = value3;
					FixedPoint value5 = 1L;
					FixedPoint? obj = target.AttributeModel?.GetAttributeModelValue("burn_be_ratio");
					FixedPoint? fixedPoint = value5 + obj;
					value3 = (value4 * fixedPoint).Value;
				}
				if (modelManager.Player.RollDice(RollDiceType.ActivateChance, value3, value) != PlayerRandomChanceResult.Failed)
				{
					modelManager.ExecuteAction(new BurningOutAction(source, target, onRedHealthBar: false, sourceSupport, () => damageAction.FinalDamage));
					receivedFireDamage = true;
				}
			}
			if (source != null && source.HasTrait("Burning") && !target.HasTrait("StruggleInvulnerable") && damageAction.DamageType == DamageType.Melee)
			{
				TraitDefinition traitDefinition = modelManager.GameEconomyData.GetTraitDefinition("Burning");
				if (traitDefinition != null)
				{
					FixedPoint parameter = traitDefinition.GetParameter<FixedPoint>(0);
					FixedPoint value6 = 0.0;
					FixedPoint fixedPoint2 = FixedPoint.Round(parameter * ((FixedPoint)target.MaxHitPoints / (FixedPoint)100.0));
					bool flag = false;
					if (modelManager.Player.AbilityManager.VisitParameter("AbilityModifierDecreaseBurningDamage", ref value6, target))
					{
						flag = true;
					}
					if (modelManager.Player.AbilityManager.VisitParameter("FlameDMGReduceBouns_ReduceBurn", ref value6, target))
					{
						flag = true;
					}
					if (flag)
					{
						fixedPoint2 -= fixedPoint2 * value6;
						fixedPoint2 = FixedPoint.Max(0.0, fixedPoint2);
					}
					receivedFireDamage = receivedFireDamage || modelManager.ExecuteAction(new DamageAction(target, source, (int)fixedPoint2, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Failed, DamageType.Fire));
				}
			}
			if (source == null || !target.HasTrait("Burning") || source.HasTrait("StruggleInvulnerable") || damageAction.DamageType != DamageType.Melee)
			{
				return;
			}
			TraitDefinition traitDefinition2 = modelManager.GameEconomyData.GetTraitDefinition("Burning");
			if (traitDefinition2 != null)
			{
				FixedPoint parameter2 = traitDefinition2.GetParameter<FixedPoint>(0);
				FixedPoint value7 = 0.0;
				FixedPoint fixedPoint3 = FixedPoint.Round(parameter2 * ((FixedPoint)source.MaxHitPoints / (FixedPoint)100.0));
				bool flag2 = false;
				if (modelManager.Player.AbilityManager.VisitParameter("AbilityModifierDecreaseBurningDamage", ref value7, source))
				{
					flag2 = true;
				}
				if (modelManager.Player.AbilityManager.VisitParameter("FlameDMGReduceBouns_ReduceBurn", ref value7, source))
				{
					flag2 = true;
				}
				if (flag2)
				{
					fixedPoint3 -= fixedPoint3 * value7;
					fixedPoint3 = FixedPoint.Max(0.0, fixedPoint3);
				}
				receivedFireDamage = receivedFireDamage || modelManager.ExecuteAction(new DamageAction(source, null, (int)fixedPoint3, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Failed, DamageType.Fire));
			}
		}

		public static void CheckForStruggle(TWDModelManager modelManager, ActorModel source, ActorModel target, DamageAction damageAction, bool receivedFireDamage = false)
		{
			GameEconomyData gameEconomyData = modelManager.GameEconomyData;
			bool flag = target.Faction == Faction.Civilian && target.CivilianCanStruggle;
			bool flag2 = target.Faction == Faction.Survivor;
			bool flag3 = target.Faction == Faction.Raider;
			DamageType damageType = damageAction.DamageType;
			bool flag4 = false;
			int struggleBaseThreshold = gameEconomyData.ConfigData.StruggleBaseThreshold;
			int struggleBaseChance = gameEconomyData.ConfigData.StruggleBaseChance;
			if (source != null && source.Faction == Faction.Walker && source.Hitpoints > 0 && target.IsHuman && (flag || flag2 || flag3))
			{
				int num = modelManager.CombatModel.RollCombatDice(RollDiceType.Struggle, 100);
				if (target.StrugglesLeft > 0 && target.Hitpoints <= struggleBaseThreshold && num <= struggleBaseChance && !target.IsStruggling && !target.IsBleedingOut && damageType != DamageType.Explosion && damageType != DamageType.Bleeding)
				{
					flag4 = modelManager.ExecuteAction(new StruggleAction(source, target));
				}
			}
			else if (target.IsWalker && !target.IsDead && !target.IsStunned && source != null && source.IsHuman)
			{
				int num2 = modelManager.CombatModel.RollCombatDice(RollDiceType.Struggle, 100);
				if (source.StrugglesLeft > 0 && source.Hitpoints <= struggleBaseThreshold && num2 <= struggleBaseChance && !source.IsStruggling && !source.IsBleedingOut && damageType != DamageType.Explosion && damageType != DamageType.Bleeding)
				{
					flag4 = modelManager.ExecuteAction(new StruggleAction(target, source));
				}
			}
			if (flag4)
			{
				return;
			}
			if (source != null && source.IsHuman && source.StrugglesLeft > 0 && source.Hitpoints <= struggleBaseThreshold && !source.IsStruggling && !source.IsBleedingOut && !source.OnRedHealthBar)
			{
				if (receivedFireDamage)
				{
					modelManager.ExecuteAction(new BurningOutAction(target, source, null, () => damageAction.FinalDamage));
				}
				else if (!modelManager.GameEconomyData.GetFeature("BleedingOnOneHealthFix").Enabled)
				{
					modelManager.ExecuteAction(new BleedingOutAction(target, source));
				}
				if (target == source)
				{
					damageAction.IgnoreIndicatorUpdate = true;
				}
			}
			if (!target.IsHuman || target.StrugglesLeft <= 0 || target.Hitpoints > struggleBaseThreshold || target.IsStruggling || target.IsBleedingOut || target.OnRedHealthBar)
			{
				return;
			}
			if (damageType != DamageType.Bleeding)
			{
				if (receivedFireDamage)
				{
					modelManager.ExecuteAction(new BurningOutAction(source, target, null, () => damageAction.FinalDamage));
				}
				else
				{
					modelManager.ExecuteAction(new BleedingOutAction(source, target));
				}
			}
			else if (damageType == DamageType.Bleeding)
			{
				modelManager.ExecuteAction(new BleedingOutAction(source, target));
			}
		}

		public static bool AttackTarget(CombatModel combatModel, ActorModel source, ActorModel target, AbilityModel ability, DamageType damageType, bool ignoreRandomHitChance = false, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, bool isSingleTarget = false, bool isMainTarget = true, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			bool flag = true;
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter(AbilityModifierIncreaseSecondaryHitsChance.SecondaryHitsChance, ref value, source);
			value += ability.Definition.SecondaryTargetsHitChance;
			FixedPoint value2 = 0.0;
			combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, source);
			if (combatModel.manager.Player.RollDice(RollDiceType.HitChance, value, value2) != PlayerRandomChanceResult.Failed || ignoreRandomHitChance)
			{
				PlayerRandomChanceResult bodyShotResult = PlayerRandomChanceResult.Failed;
				Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications = new Dictionary<ActorModel, List<DamageNotificationData>>();
				FixedPoint value3 = 0L;
				combatModel.AbilityManager.VisitParameter("Equipment_Active_ExtraDamageExecution", ref value3, source);
				int num = 1 + (int)value3;
				bool isAddPhonePortrait = false;
				CheckAndAddPhonePortraitEffect(combatModel, source, target, ref isAddPhonePortrait);
				bool isChargeAttack = ability.IsChargeAttack;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (!target.IsDead)
					{
						(combatModel.Manager as TWDModelManager)?.ExecuteAction(new PreCalculateDamageAction(target, source, damageType));
						PlayerRandomChanceResult criticalResult;
						int[] array = CalculateDamage(combatModel, source, target, damageType, out criticalResult, out bodyShotResult, resolvedRolls, isSingleTarget, isChargeAttack, ref damageNotifications, ability, isMainTarget, ootType, isAssistAttack, isTriggerExtraAttackDamage);
						flag = ExecuteDamage(combatModel, source, target, array[0], array[1], damageType, criticalResult, bodyShotResult, damageNotifications, i > 0, null, noChargeGain: false, null, isMainTarget, isTriggerExtraAttackDamage, ootType, isChargeAttack);
						num2 = ((criticalResult != PlayerRandomChanceResult.Failed) ? (num2 + (array[0] + array[1])) : (num2 + array[0]));
					}
				}
				combatModel.AddAttackedTarget(source, target);
				if (flag)
				{
					if (!combatModel.manager.GameEconomyData.GetFeature("ChainsawThreatFix").Enabled)
					{
						CalculateThreatReduction(combatModel, source, target, ability);
					}
					if (target.IsDead || (ability.PushEffect != null && num2 >= target.Hitpoints))
					{
						CheckForExplosiveBulletTrait(combatModel, source, target);
					}
					if (!source.VisitedExtraApChance && target.Definition.IsNotBasicWalker)
					{
						source.VisitedExtraApChance = true;
						FixedPoint value4 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseExtraAPChanceSpecialEnemies", ref value4, source);
						value2 = 0.0;
						if (value4 != 0.0)
						{
							combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, source);
						}
						PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.GainAP, value4, value2);
						source.EnsureExtraAP = playerRandomChanceResult != PlayerRandomChanceResult.Failed;
					}
					if (!target.IsDead || (ability.PushEffect != null && num2 < target.Hitpoints))
					{
						CheckAndAffectPhonePortrait(combatModel, source, target, isAddPhonePortrait, num2);
					}
					if ((!source.EnsureExtraAP || source.HasGainedExtraAP) && source.IsRangedClass && !source.HasGainedExtraMoveAp && bodyShotResult != PlayerRandomChanceResult.Failed)
					{
						FixedPoint value5 = 0.0;
						combatModel.AbilityManager.VisitParameterWithAbility(ability, "AbilityModifierExtraMoveChance", ref value5, source);
						PlayerRandomChanceResult playerRandomChanceResult2 = combatModel.manager.Player.RollDice(RollDiceType.GainAP, value5, value2);
						source.ExtraMoveApNotificationKey = "LeaderBuffColdBlooded";
						source.EnsureGainedExtraMoveAp = playerRandomChanceResult2 != PlayerRandomChanceResult.Failed;
					}
					foreach (ActorModel factionActor in combatModel.GetFactionActors(Faction.Survivor))
					{
						if (factionActor.HasAnyLevelTrait("BaseKnockKnock"))
						{
							factionActor.BuffKnockKnockChargePoint(target);
						}
					}
					if (source.HasAnyLevelTrait("Equipment_Active_Recoil") && !target.IsDead)
					{
						FixedPoint value6 = 0.0;
						if (target.IsRecoilEffected)
						{
							combatModel.AbilityManager.VisitParameter("AbilityModifierRecoilCircleStunChance", ref value6, source);
						}
						else
						{
							combatModel.AbilityManager.VisitParameter("AbilityModifierRecoilNormalStunChance", ref value6, source);
						}
						PlayerRandomChanceResult playerRandomChanceResult3 = combatModel.manager.Player.RollDice(RollDiceType.Stun, value6, 0.0);
						if (playerRandomChanceResult3 != PlayerRandomChanceResult.Failed)
						{
							StunAction stunAction = new StunAction(source, target, 1, ignoreSourceBeingDead: true);
							stunAction.CausedByTrait = "Equipment_Active_Recoil";
							combatModel.manager.ExecuteAction(stunAction);
							target.NotifyChange("AbilityVisited", new object[2]
							{
								"Equipment_Active_Recoil",
								playerRandomChanceResult3 == PlayerRandomChanceResult.SuccessDueToExtension
							});
						}
					}
					CheckForHeirlooms_RiotGearGlenn_Fetter(combatModel, source, target, isChargeAttack);
					CheckForEquipment_Passive_Detonation(combatModel, source, target, isChargeAttack);
				}
				if (target.IsDead)
				{
					source.KilledEnemyNum++;
				}
			}
			return flag;
		}

		public static bool AttackTargetConsumable(CombatModel combatModel, ActorModel source, ActorModel target, AbilityModel ability, DamageType damageType, bool ignoreRandomHitChance = false, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, bool isSingleTarget = false)
		{
			bool result = true;
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter(AbilityModifierIncreaseSecondaryHitsChance.SecondaryHitsChance, ref value, source);
			value += ability.Definition.SecondaryTargetsHitChance;
			if (combatModel.manager.Player.RollDice(RollDiceType.HitChance, value) != PlayerRandomChanceResult.Failed || ignoreRandomHitChance)
			{
				PlayerRandomChanceResult bodyShotResult = PlayerRandomChanceResult.Failed;
				Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications = new Dictionary<ActorModel, List<DamageNotificationData>>();
				if (!target.IsDead)
				{
					PlayerRandomChanceResult criticalResult;
					int[] array = CalculateDamageConsumable(combatModel, source, target, damageType, out criticalResult, out bodyShotResult, resolvedRolls, isSingleTarget, isChargeAttack: false, ref damageNotifications, ability);
					result = ExecuteDamageConsumable(combatModel, source, target, array[0], array[1], damageType, criticalResult, bodyShotResult, damageNotifications);
				}
				combatModel.AddAttackedTarget(source, target);
			}
			return result;
		}

		public static int[] CalculateDamageConsumable(CombatModel combatModel, ActorModel source, ActorModel target, DamageType type, out PlayerRandomChanceResult criticalResult, out PlayerRandomChanceResult bodyShotResult, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls, bool isSingleTarget, bool isChargeAttack, ref Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications, AbilityModel ability = null)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			_ = combatModel.manager.GameEconomyData;
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("AbilityModifierDamageScaleOnTargetHealth", ref value2, source);
			value += value2 * target.MaxHitPoints;
			abilityManager.VisitParameter("AbilityModifierScaleDamageByMaxSurvivorLevel", ref value, source);
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageStart(value);
			}
			if (combatModel.manager.CurrentCommandLogEntry != null)
			{
				combatModel.manager.CurrentCommandLogEntry.CalculateDamageEnd(new int[2]
				{
					(int)value,
					0
				});
			}
			criticalResult = PlayerRandomChanceResult.Failed;
			bodyShotResult = PlayerRandomChanceResult.Failed;
			return new int[2]
			{
				(int)value,
				0
			};
		}

		public static void CalculateThreatReduction(CombatModel combatModel, ActorModel source, ActorModel target, AbilityModel ability)
		{
			FixedPoint value = 0.0;
			bool flag = source.HasAnyLevelTrait("LeaderBuffReduceThreatMelee");
			if (source == null || source.Faction != Faction.Survivor || !target.IsDead || target.Definition.IsEnvironmental || combatModel.ThreatMeter.ThreatLevel <= 0)
			{
				return;
			}
			FixedPoint value2 = 0.0;
			combatModel.AbilityManager.VisitParameter("AbilityThreatReductionChance", ref value2, source);
			combatModel.AbilityManager.VisitParameter("AbilityThreatFreeChance", ref value2, source);
			if (source.PastaCurrentTurn)
			{
				SupportModel supportModel = source.manager.Player.GetSupportModel("Pasta");
				if (supportModel != null && supportModel.Unlocked)
				{
					value2 += supportModel.GetParameter(3) * 0.009999999776482582;
				}
			}
			if (value2 != 0.0)
			{
				combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, source);
			}
			EquipmentItemModel equipmentItemModel = (ability.IsChargeAttack ? source.GetChargeEquipment() : source.GetEquipmentWithAbility(ability));
			if (equipmentItemModel != null && equipmentItemModel.Definition != null && equipmentItemModel.Definition.Category == EquipmentCategory.MeleeWeapon)
			{
				combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseReduceThreatChanceMelee", ref value2, source);
			}
			PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.Silenced, value2, value);
			if (playerRandomChanceResult == PlayerRandomChanceResult.Failed)
			{
				return;
			}
			List<TraitEntry> traitsThatContain = source.GetTraitsThatContain("ThreatReduction");
			List<TraitEntry> traitsThatContain2 = source.GetTraitsThatContain("ThreatFree");
			if (traitsThatContain2.Count > 0)
			{
				traitsThatContain.AddRange(traitsThatContain2);
			}
			if (traitsThatContain != null && traitsThatContain.Count > 0)
			{
				int num = -(int)combatModel.gameEconomyData.GetTraitDefinition(traitsThatContain[0].TraitIdentifier).GetParameter<FixedPoint>(1);
				combatModel.manager.ExecuteAction(new NoiseAction(source, source.GridCoordinate, 0, num));
				combatModel.manager.ExecuteAction(new ThreatAction(source, num));
				source.NotifyChange("actorThreatReduction", new object[2]
				{
					num,
					playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
				});
				return;
			}
			int num2 = -1;
			combatModel.manager.ExecuteAction(new NoiseAction(source, source.GridCoordinate, 0, num2));
			combatModel.manager.ExecuteAction(new ThreatAction(source, num2));
			if (flag)
			{
				source.NotifyChange("AbilityVisited", new object[2]
				{
					"LeaderBuffReduceThreatMelee",
					playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
				});
			}
		}

		public static void CheckForLeaderBuffLeadByExample(CombatModel combatModel, ActorModel source)
		{
			FixedPoint value = 0.0;
			if (!combatModel.AbilityManager.VisitParameter("LeaderBuffLeadByExample", ref value, source) || combatModel.Survivors.Count <= 1)
			{
				return;
			}
			List<ActorModel> list = new List<ActorModel>();
			for (int i = 0; i < combatModel.Survivors.Count; i++)
			{
				ActorModel actorModel = combatModel.Survivors[i];
				if (source != actorModel && actorModel.ChargeMeter.ChargeLevel < actorModel.ChargeMeter.MaxLevel)
				{
					list.Add(actorModel);
				}
			}
			if (list.Count > 0)
			{
				int randomInRange = combatModel.manager.Player.PlayerRandom.GetRandomInRange(0, list.Count - 1);
				list[randomInRange].AddChargePoints(1);
				list[randomInRange].NotifyChange("AbilityVisited", new object[2] { "LeaderBuffLeadByExample", false });
			}
		}

		public static void CheckForExtraApMovement(ActorModel source, List<ActorModel> targets, CombatModel combatModel)
		{
			FixedPoint value = 0.0;
			if (combatModel.AbilityManager.VisitParameter("LeaderBuffDeadlyTactics", ref value, source) && targets.Count > 1 && (!source.EnsureExtraAP || source.HasGainedExtraAP) && !source.HasGainedExtraMoveAp)
			{
				source.EnsureGainedExtraMoveAp = true;
				source.ExtraMoveApNotificationKey = "LeaderBuffDeadlyTactics";
			}
			bool flag = targets.Any((ActorModel x) => !x.IsDead);
			if (flag)
			{
				targets.RemoveAll((ActorModel x) => x.IsDead);
				foreach (ActorModel target in targets)
				{
					if (combatModel.manager.Player.AbilityManager.AbilityUnderApplication.PostExecuteActions.Find((ModelAction action) => action is DamageAction damageAction && !ShouldActorExplode(source, target, damageAction.DamageType, damageAction.FinalDamage)) != null)
					{
						flag = false;
					}
				}
			}
			if (combatModel.AbilityManager.VisitParameter("LeaderBuffGoodEnough", ref value, source) && flag && (!source.EnsureExtraAP || source.HasGainedExtraAP) && !source.HasGainedExtraMoveAp)
			{
				source.EnsureGainedExtraMoveAp = true;
				source.ExtraMoveApNotificationKey = "LeaderBuffGoodEnough";
			}
			if ((source.HasAnyLevelTrait("FightingFury") || (source.HasAnyLevelTrait("BaseFightingFury") && source.CanMoveWithoutAttacking)) && (!source.EnsureExtraAP || source.HasGainedExtraAP) && !source.HasGainedExtraMoveAp)
			{
				source.EnsureGainedExtraMoveAp = true;
				source.ExtraMoveApNotificationKey = "LeaderBuffFightingFury";
			}
		}

		public static void CheckAndAddPhonePortraitEffect(CombatModel combatModel, ActorModel source, ActorModel target, ref bool isAddPhonePortrait)
		{
			if (source.HasAnyLevelTrait("Heirlooms_Rick_PhonePortrait") && !target.IsTakePhonePortraitEffectThisTurn())
			{
				FixedPoint value = 0.0;
				FixedPoint value2 = 0.0;
				combatModel.AbilityManager.VisitParameter("BounsPhonePortraitAfterKilledTimes", ref value, source);
				combatModel.AbilityManager.VisitParameter("BonusPhonePortraitTargetHitPointsBelowPercent", ref value2, source);
				if (source.KilledEnemyNum >= value && (float)target.Hitpoints / (float)target.MaxHitPoints <= value2)
				{
					target.NotifyChange("PhonePortraitUpdateEvent", true);
					target.AddPhonePortraitModel();
					isAddPhonePortrait = true;
				}
			}
		}

		public static void CheckAndAffectPhonePortrait(CombatModel combatModel, ActorModel source, ActorModel target, bool isAddPhonePortrait, int baseDmage)
		{
			combatModel.CheckAndClearExpiredPhonePortraitKilledNum();
			if (isAddPhonePortrait || !source.HasAnyLevelTrait("Heirlooms_Rick_PhonePortrait") || !target.IsTakePhonePortraitEffectThisTurn())
			{
				return;
			}
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("BounsPhonePortraitOnceKilledMaxTarget", ref value, source);
			if (combatModel.getPhonePortraitKilledNum() >= value)
			{
				return;
			}
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("BounsPhonePortraitKilledTargetPercentage", ref value2, source);
			if (combatModel.manager.Player.RollDice(RollDiceType.Damage, value2, 0.0) == PlayerRandomChanceResult.Failed)
			{
				return;
			}
			target.NotifyChange("AbilityVisited", new object[2] { "Heirlooms_Rick_PhonePortrait", true });
			int num = target.Hitpoints;
			if (source is SurvivorModel survivorModel)
			{
				MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(source.manager);
				TraitEntry traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("Heirlooms_Rick_PhonePortrait");
				if (mapMissionModel != null && traitAnyLevel != null)
				{
					FixedPoint minDebuffParamPercentageByTraitId = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel.GetChallengeDebuffs(), ChallengeDebuffType.DebuffRickHT, traitAnyLevel.TraitIdentifier);
					if (minDebuffParamPercentageByTraitId > 0L)
					{
						num = (int)(survivorModel.GetDamageForPreferredWeapon() * minDebuffParamPercentageByTraitId);
					}
				}
			}
			num += baseDmage;
			ExecuteDamage(combatModel, source, target, num, 0, DamageType.Base, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: false, source);
			combatModel.AddPhonePortraitKilledNum();
		}

		public static void CheckForExplosiveBulletTrait(CombatModel combatModel, ActorModel source, ActorModel target)
		{
			FixedPoint value = 0.0;
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			bool flag = abilityManager.VisitParameter("AbilityModifierExplosiveBulletDamageScaleOnTargetHealth", ref value, source);
			if (!(source.IsRangedClass && flag))
			{
				return;
			}
			if (target.Definition.InitialTraits == null || (!target.Definition.InitialTraits.Contains("Explosive") && !target.Definition.InitialTraits.Contains("ExplosiveGoo")))
			{
				target.NotifyChange("actorExploded", "LeaderBuffExplosiveBullets");
			}
			List<ActorModel> actorsInRange = combatModel.GetActorsInRange(target.GridCoordinate, 1);
			for (int i = 0; i < actorsInRange.Count; i++)
			{
				ActorModel actorModel = actorsInRange[i];
				if (!actorModel.IsEnemy(source) || actorModel.IsDead)
				{
					continue;
				}
				int damage = (int)(value * target.MaxHitPoints);
				if (source.Faction == Faction.Survivor && source is SurvivorModel survivorModel)
				{
					MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(source.manager);
					TraitEntry traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("LeaderBuffExplosiveBullets");
					if (traitAnyLevel == null)
					{
						foreach (ActorModel factionActor in combatModel.GetFactionActors(source.Faction))
						{
							traitAnyLevel = factionActor.TraitContainer.GetTraitAnyLevel("LeaderBuffExplosiveBullets");
							if (traitAnyLevel != null)
							{
								break;
							}
						}
					}
					if (mapMissionModel != null && traitAnyLevel != null)
					{
						FixedPoint minDebuffParamPercentageByTraitId = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(mapMissionModel.GetChallengeDebuffs(), ChallengeDebuffType.DebuffShaneLT, traitAnyLevel.TraitIdentifier);
						if (minDebuffParamPercentageByTraitId > 0L)
						{
							damage = Math.Min(damage, (int)(survivorModel.GetDamageForPreferredWeapon() * minDebuffParamPercentageByTraitId));
						}
					}
				}
				ExecuteDamage(combatModel, target, actorModel, damage, 0, DamageType.Explosion, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: false, source);
				if (actorModel.IsDead)
				{
					continue;
				}
				FixedPoint value2 = 0.0;
				abilityManager.VisitParameter("AbilityModifierExplosiveBulletStunChance", ref value2, source);
				FixedPoint value3 = 0.0;
				abilityManager.VisitParameter("ExtendProbability", ref value3, source);
				PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.Stun, value2, value3);
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					StunAction stunAction = new StunAction(source, actorModel, 1, ignoreSourceBeingDead: false, null, () => damage);
					stunAction.CausedByTrait = "AbilityModifierExplosiveBulletStunChance";
					combatModel.manager.ExecuteAction(stunAction);
					actorModel.NotifyChange("AbilityVisited", new object[2]
					{
						"LeaderBuffExplosiveBullets",
						playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
					});
				}
			}
		}

		public static void AttackTargets(CombatModel combatModel, ActorModel source, List<ActorModel> targets, AbilityModel ability, DamageType damageType, bool ignoreRandomHitChance = false, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			foreach (ActorModel target in targets)
			{
				AttackTarget(combatModel, source, target, ability, damageType, ignoreRandomHitChance, null, isSingleTarget: false, isMainTarget: false, ootType, isAssistAttack, isTriggerExtraAttackDamage);
			}
			ApplyBuffKnockKnock(source);
		}

		public static void AttackTargetsConsumable(CombatModel combatModel, ActorModel source, List<ActorModel> targets, AbilityModel ability, DamageType damageType, bool ignoreRandomHitChance = false)
		{
			foreach (ActorModel target in targets)
			{
				AttackTargetConsumable(combatModel, source, target, ability, damageType, ignoreRandomHitChance);
			}
		}

		public static bool IsOccupiedOrBlocked(CombatModel combatModel, GridCoordinate coordinate, ActorModel ignoreActorModel)
		{
			if (!combatModel.IsBlocked(coordinate))
			{
				if (combatModel.GetOccupier(coordinate) != null)
				{
					return combatModel.GetOccupier(coordinate) != ignoreActorModel;
				}
				return false;
			}
			return true;
		}

		public static int GetMoveRange(ActorModel actor)
		{
			if (actor.TurnComplete || actor.IsRooted || actor.IsPitfalled || actor.FocusModeState)
			{
				return 0;
			}
			int num = actor.MoveRange + actor.AdditionalMoveRange;
			if (actor.IsCrippled)
			{
				return Math.Min(1, num);
			}
			int num2 = ((!actor.MoveCompleted && !actor.IsInvisible && (!actor.AbilityCompleted || !actor.AllowSecondMoveAfterAbility)) ? (num * 2) : num);
			if (actor.TryGetBloodMarkMoveDistanceCap(out var moveDistanceCap) && moveDistanceCap > 0)
			{
				num2 = Math.Min(num2, moveDistanceCap);
			}
			return num2;
		}

		public static GridCoordinate GetClosestFreeNeighbor(CombatModel combat, GridField<FixedPoint> distanceField, GridCoordinate coordinate, ActorModel movingActor, FixedPoint maxDistance, InteractiveObjectModel interactiveObject, bool checkVisibility, bool edgeCheck = true)
		{
			GridModel grid = combat.Grid;
			bool flag = interactiveObject != null && interactiveObject.Placement == Placement.Edge;
			List<GridCoordinate> list = new List<GridCoordinate>();
			if (flag)
			{
				for (int i = 0; i < interactiveObject.Location.Edges.Count; i++)
				{
					grid.GetCoordinatesFromEdge(interactiveObject.Location.Edges[i], out var a, out var b);
					list.Add(combat.IsBlocked(a) ? b : a);
				}
			}
			GridCoordinate result = GridCoordinate.Invalid;
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			for (int j = 0; j < 8; j++)
			{
				GridCoordinate coordinateNeighbor = grid.GetCoordinateNeighbor(coordinate, j);
				if (grid.IsCoordinateValid(coordinateNeighbor))
				{
					FixedPoint fixedPoint2 = maxDistance - ((!flag && edgeCheck) ? DistanceField.DistanceToNeighbor(j) : ((FixedPoint)0L));
					bool flag2 = combat.GetOccupier(coordinateNeighbor) == null || combat.GetOccupier(coordinateNeighbor) == movingActor;
					FixedPoint fixedPoint3 = distanceField[coordinateNeighbor];
					bool flag3 = flag || !edgeCheck || combat.CanTraverse(movingActor, coordinateNeighbor, coordinate);
					bool flag4 = !checkVisibility || combat.IsGridCellVisible(coordinateNeighbor, coordinate);
					bool flag5 = !flag || list.Contains(coordinateNeighbor);
					if (fixedPoint3 < fixedPoint && fixedPoint3 <= fixedPoint2 && flag2 && flag3 && flag5 && flag4)
					{
						fixedPoint = fixedPoint3;
						result = coordinateNeighbor;
					}
				}
			}
			return result;
		}

		public static FixedPoint GetDistanceToClosestFreeCoordinate(CombatModel combat, GridField<FixedPoint> distanceField, GridCoordinate coordinate, ActorModel movingActor, FixedPoint maxDistance, InteractiveObjectModel interactiveObject, bool edgeCheck = true)
		{
			GridCoordinate closestFreeNeighbor = GetClosestFreeNeighbor(combat, distanceField, coordinate, movingActor, maxDistance, interactiveObject, edgeCheck);
			if (closestFreeNeighbor.IsValid)
			{
				return distanceField[closestFreeNeighbor];
			}
			return DistanceField.DistanceNotSet;
		}

		public static void GetThreatField(CombatModel combatModel, ActorModel activeActor, GridField<CellValidity> validTargets, ref GridField<FixedPoint> threatField)
		{
			threatField.Clear();
			List<ActorModel> enemyFactionsActors = combatModel.GetEnemyFactionsActors(activeActor.Faction);
			List<ActorModel> list = new List<ActorModel>();
			for (int i = 0; i < enemyFactionsActors.Count; i++)
			{
				ActorModel actorModel = enemyFactionsActors[i];
				if (!actorModel.IsVisibleToSurvivors)
				{
					continue;
				}
				if (actorModel.ExclusiveTimedEffect != null)
				{
					TimedEffect exclusiveTimedEffect = actorModel.ExclusiveTimedEffect;
					if (exclusiveTimedEffect.Type == TimedEffectType.EatingLure || exclusiveTimedEffect.Type == TimedEffectType.Stun || exclusiveTimedEffect.Type == TimedEffectType.Struggle || exclusiveTimedEffect.Type == TimedEffectType.InteractingWithObject)
					{
						continue;
					}
				}
				switch (actorModel.Faction)
				{
				case Faction.Walker:
				case Faction.Dormant:
				case Faction.Environmental:
					list.Add(actorModel);
					break;
				case Faction.Raider:
					if (actorModel.Definition.Class == "Scout" || actorModel.Definition.Class == "Warrior" || actorModel.Definition.Class == "Bruiser")
					{
						list.Add(actorModel);
					}
					break;
				}
			}
			GridModel grid = combatModel.Grid;
			for (int j = 0; j < grid.NumCells; j++)
			{
				GridCoordinate coordinate = grid.GetCoordinate(j);
				if (validTargets[coordinate].Status == CellStatus.Valid || validTargets[coordinate].Status == CellStatus.Extended)
				{
					for (int k = 0; k < list.Count; k++)
					{
						ActorModel actorModel2 = list[k];
						FixedPoint fixedPoint = actorModel2.GridCoordinate.SquaredDistanceTo(coordinate);
						threatField[coordinate] += fixedPoint;
						_ = actorModel2.Faction;
					}
				}
			}
		}

		public static void GetValidTargets(CombatModel combatModel, ActorModel activeActor, GridCoordinate startCoordinate, FixedPoint remainingMoveRange, ref GridField<CellValidity> validTargets, ref List<AfterMoveAbilityTarget> afterMoveAbilityTargets, ref GridField<FixedPoint> threatField)
		{
			if (activeActor == null || activeActor.GridCoordinate != startCoordinate)
			{
				return;
			}
			GridModel grid = combatModel.Grid;
			validTargets.Clear();
			FixedPoint fixedPoint = activeActor.MoveRange;
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combatModel, startCoordinate, new DistanceFieldOptions(1.5f, activeActor, activeActor));
			validTargets[startCoordinate] = new CellValidity(CellStatus.Valid, validTargets[startCoordinate].InteractiveObject, validTargets[startCoordinate].Target);
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = 0; i < grid.NumCells; i++)
			{
				GridCoordinate coordinate = grid.GetCoordinate(i);
				if (gridField[coordinate] <= remainingMoveRange && !IsOccupiedOrBlocked(combatModel, coordinate, activeActor))
				{
					list.Add(coordinate);
					validTargets[coordinate] = new CellValidity((gridField[coordinate] <= fixedPoint) ? CellStatus.Valid : CellStatus.Extended, null, null);
				}
				else
				{
					validTargets[coordinate] = new CellValidity(CellStatus.Invalid, null, null);
				}
			}
			List<ActorModel> factionActors = combatModel.GetFactionActors(Faction.Survivor);
			for (int j = 0; j < factionActors.Count; j++)
			{
				ActorModel actorModel = factionActors[j];
				if (actorModel == activeActor)
				{
					continue;
				}
				FixedPoint distance = DistanceField.GetDistance(grid, gridField, actorModel.GridCoordinate);
				if (distance <= remainingMoveRange)
				{
					if (!combatModel.HasPvPRules && actorModel.IsBleedingOut)
					{
						validTargets[actorModel.GridCoordinate] = new CellValidity(CellStatus.Valid, null, null);
					}
					else
					{
						validTargets[actorModel.GridCoordinate] = new CellValidity((distance <= fixedPoint) ? CellStatus.Friendly : CellStatus.FriendlyExtended, null, null);
					}
				}
			}
			List<ActorModel> factionActors2 = combatModel.GetFactionActors(Faction.Civilian);
			for (int k = 0; k < factionActors2.Count; k++)
			{
				ActorModel actorModel2 = factionActors2[k];
				validTargets[actorModel2.GridCoordinate] = new CellValidity(CellStatus.Invalid, null, null);
			}
			List<TWDModelObject> models = combatModel.GetModels<InteractiveObjectModel>();
			for (int l = 0; l < models.Count; l++)
			{
				InteractiveObjectModel interactiveObjectModel = models[l] as InteractiveObjectModel;
				if (!interactiveObjectModel.CanBeInteracted || interactiveObjectModel.HasInteractionStarted || interactiveObjectModel.InteractBy != InteractBy.Use || !interactiveObjectModel.IsVisibleToSurvivors)
				{
					continue;
				}
				if (interactiveObjectModel.Placement == Placement.Cell)
				{
					List<GridCoordinate> coordinates = interactiveObjectModel.Location.Coordinates;
					for (int m = 0; m < coordinates.Count; m++)
					{
						GridCoordinate coordinate2 = coordinates[m];
						if (!validTargets[coordinate2].Valid)
						{
							FixedPoint distanceToClosestFreeCoordinate = GetDistanceToClosestFreeCoordinate(combatModel, gridField, coordinate2, activeActor, remainingMoveRange, interactiveObjectModel);
							if (distanceToClosestFreeCoordinate != DistanceField.DistanceNotSet)
							{
								validTargets[coordinate2] = new CellValidity((distanceToClosestFreeCoordinate <= activeActor.MoveRange) ? CellStatus.Valid : CellStatus.Extended, interactiveObjectModel, null);
							}
						}
					}
					continue;
				}
				List<int> edges = interactiveObjectModel.Location.Edges;
				for (int n = 0; n < edges.Count; n++)
				{
					int edgeId = edges[n];
					grid.GetCoordinatesFromEdge(edgeId, out var a, out var b);
					bool flag = validTargets[a].Valid || a == activeActor.GridCoordinate;
					bool flag2 = validTargets[b].Valid || b == activeActor.GridCoordinate;
					if (flag != flag2)
					{
						if (!flag)
						{
							validTargets[a] = new CellValidity(CellStatus.Valid, interactiveObjectModel, null);
						}
						else
						{
							validTargets[b] = new CellValidity(CellStatus.Valid, interactiveObjectModel, null);
						}
					}
				}
			}
			if (threatField != null)
			{
				GetThreatField(combatModel, activeActor, validTargets, ref threatField);
			}
			AbilityModel selectedAbility = activeActor.SelectedAbility;
			EquipmentItemModel selectedEquipment = activeActor.SelectedEquipment;
			if (selectedAbility == null || selectedEquipment == null)
			{
				return;
			}
			bool isUsingAdditionalAttacks = activeActor.GetIsUsingAdditionalAttacks();
			FixedPoint range = selectedAbility.Definition.AbilityRange;
			if (!selectedAbility.IsConsumableAbility)
			{
				CalculateRangeExtension(ref range, activeActor, combatModel.AbilityManager);
			}
			bool abilityTargetDiagonal = selectedAbility.Definition.AbilityTargetDiagonal;
			FixedPoint fixedPoint2 = (range + (abilityTargetDiagonal ? 0.42f : 0f)) * grid.CellSize.X;
			FixedPoint fixedPoint3 = fixedPoint2 * fixedPoint2;
			FixedVec3[] array = new FixedVec3[list.Count];
			for (int num = 0; num < list.Count; num++)
			{
				array[num] = grid.GetPosition(list[num]);
			}
			for (int num2 = 0; num2 < grid.NumCells; num2++)
			{
				GridCoordinate coordinate3 = grid.GetCoordinate(num2);
				if (validTargets[coordinate3].Status != CellStatus.Invalid)
				{
					continue;
				}
				ActorModel occupier = combatModel.GetOccupier(coordinate3);
				if (occupier != null && occupier.Faction != Faction.Survivor && !occupier.IsVisibleToSurvivors && selectedAbility.Definition.TriggerType != AbilityTriggerType.Grid)
				{
					continue;
				}
				GridCoordinate gridCoordinate = GridCoordinate.Invalid;
				GridCoordinate coordinate4 = GridCoordinate.Invalid;
				FixedPoint fixedPoint4 = FixedPoint.MaxValue;
				FixedPoint fixedPoint5 = FixedPoint.MinValue;
				FixedVec3 position = grid.GetPosition(coordinate3);
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					GridCoordinate gridCoordinate2 = list[num3];
					FixedPoint fixedPoint6 = gridField[gridCoordinate2];
					if (!isUsingAdditionalAttacks && (array[num3] - position).SqrMagnitude >= fixedPoint3)
					{
						continue;
					}
					FixedPoint fixedPoint7 = 0L;
					if (threatField != null)
					{
						fixedPoint7 = threatField[gridCoordinate2] + 2L;
					}
					if (!combatModel.manager.Player.Blackboard.IsToggleOn("Toggle.AutoCoverDisabled") && combatModel.IsInCover(gridCoordinate2, coordinate3))
					{
						fixedPoint7 += (FixedPoint)1L;
					}
					if (!(fixedPoint7 < fixedPoint5) && (!(fixedPoint7 == fixedPoint5) || !(fixedPoint6 >= fixedPoint4)))
					{
						bool flag3 = false;
						if (isUsingAdditionalAttacks ? selectedAbility.CanAbilityBeTargetedOnGridCell(combatModel, activeActor, activeActor.GridCoordinate, coordinate3) : (selectedAbility.CanAbilityBePerformedOnGridCell(combatModel, activeActor, gridCoordinate2, coordinate3, range) == AbilityResult.Success))
						{
							fixedPoint4 = fixedPoint6;
							fixedPoint5 = fixedPoint7;
							gridCoordinate = coordinate3;
							coordinate4 = gridCoordinate2;
						}
					}
				}
				if (gridCoordinate.IsValid && coordinate4.IsValid)
				{
					if (afterMoveAbilityTargets != null)
					{
						afterMoveAbilityTargets.Add(new AfterMoveAbilityTarget(gridCoordinate, coordinate4));
					}
					validTargets[coordinate3] = new CellValidity((DistanceField.GetDistance(grid, gridField, gridCoordinate) <= fixedPoint) ? CellStatus.Valid : CellStatus.Extended, null, null);
				}
			}
		}

		public static void GetValidMoveTargets(CombatModel combatModel, ActorModel actor, GridCoordinate startCoordinate, FixedPoint remainingMoveRange, GridField<CellValidity> validTargets, List<AfterMoveAbilityTarget> afterMoveAbilityTargets)
		{
			if (actor == null || actor.GridCoordinate != startCoordinate)
			{
				return;
			}
			GridModel grid = combatModel.Grid;
			validTargets.Clear();
			float num = actor.MoveRange;
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combatModel, startCoordinate, new DistanceFieldOptions(1.5f, actor, actor));
			validTargets[startCoordinate] = new CellValidity(CellStatus.Valid, validTargets[startCoordinate].InteractiveObject, validTargets[startCoordinate].Target);
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = 0; i < grid.NumCells; i++)
			{
				GridCoordinate coordinate = grid.GetCoordinate(i);
				if (gridField[coordinate] <= remainingMoveRange && combatModel.GetOccupier(coordinate) == null)
				{
					list.Add(coordinate);
					validTargets[coordinate] = new CellValidity((gridField[coordinate] <= num) ? CellStatus.Valid : CellStatus.Extended, null, null);
				}
				else
				{
					validTargets[coordinate] = new CellValidity(CellStatus.Invalid, null, null);
				}
			}
		}

		public static bool IsWithinRange(CombatModel combatModel, int range, GridCoordinate sourceCell, GridCoordinate targetCell)
		{
			GridModel grid = combatModel.Grid;
			FixedPoint fixedPoint = ((float)range + 0.42f) * grid.CellSize.X;
			FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
			FixedVec3 position = grid.GetPosition(sourceCell);
			FixedVec3 position2 = grid.GetPosition(targetCell);
			FixedPoint sqrMagnitude = (position - position2).SqrMagnitude;
			return fixedPoint2 >= sqrMagnitude;
		}

		public static List<ActorModel> GetClosestWalkersToLure(CombatModel combat, GridCoordinate lureCoordinate, bool preview)
		{
			List<ActorModel> list = new List<ActorModel>();
			if (combat.Walkers.Count > 0)
			{
				ActorModel actorModel = combat.Walkers[0];
				for (int i = 0; i < 8; i++)
				{
					GridCoordinate coordinateNeighbor = combat.Grid.GetCoordinateNeighbor(lureCoordinate, i);
					if (combat.Grid.IsCoordinateValid(coordinateNeighbor) && !combat.IsBlocked(coordinateNeighbor) && combat.CanTraverse(null, coordinateNeighbor, lureCoordinate))
					{
						ActorModel occupier = combat.GetOccupier(coordinateNeighbor);
						if (occupier != null && occupier.Faction == Faction.Walker && !occupier.IsStruggling && !occupier.IsStunned && !occupier.IsEatingLure)
						{
							list.Add(occupier);
						}
					}
				}
				ActorModel occupier2 = combat.GetOccupier(lureCoordinate);
				if (occupier2 != null && occupier2.Faction == Faction.Walker && !occupier2.IsStruggling && !occupier2.IsStunned && !occupier2.IsEatingLure)
				{
					list.Add(occupier2);
				}
				for (int j = 0; j < 8; j++)
				{
					GridCoordinate coordinateNeighbor2 = combat.Grid.GetCoordinateNeighbor(lureCoordinate, j);
					if (!combat.Grid.IsCoordinateValid(coordinateNeighbor2) || combat.IsBlocked(coordinateNeighbor2) || !combat.CanTraverse(null, coordinateNeighbor2, lureCoordinate) || combat.GetOccupier(coordinateNeighbor2) != null)
					{
						continue;
					}
					GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combat, coordinateNeighbor2, new DistanceFieldOptions(1f, actorModel, actorModel, 3f));
					FixedPoint fixedPoint = FixedPoint.MaxValue;
					ActorModel actorModel2 = null;
					for (int k = 0; k < combat.Walkers.Count; k++)
					{
						ActorModel actorModel3 = combat.Walkers[k];
						if (!actorModel3.IsDead && !actorModel3.IsStruggling && !actorModel3.IsStunned && !actorModel3.IsRooted && !actorModel3.IsPitfalled && !actorModel3.IsEatingLure && !list.Contains(actorModel3) && !actorModel3.IsABTesterAed && !actorModel3.IsABTesterA2ed && combat.Grid.IsCoordinateValid(actorModel3.GridCoordinate))
						{
							FixedPoint fixedPoint2 = gridField[actorModel3.GridCoordinate];
							if (fixedPoint2 < fixedPoint && fixedPoint2 < 3.0)
							{
								fixedPoint = fixedPoint2;
								actorModel2 = actorModel3;
							}
						}
					}
					if (actorModel2 == null)
					{
						continue;
					}
					GridPath gridPath = combat.FindPath(actorModel2, actorModel2.GridCoordinate, coordinateNeighbor2);
					if (gridPath != null && gridPath.IsValid)
					{
						if (preview)
						{
							list.Add(actorModel2);
						}
						else if (MoveCommand.PerformActions(combat.manager, actorModel2, gridPath) && actorModel2.GridCoordinate == coordinateNeighbor2)
						{
							list.Add(actorModel2);
						}
					}
				}
			}
			return list;
		}

		public static bool GetHeadshotTraitDamage(CombatModel combatModel, ActorModel source, ActorModel target, FixedPoint luck, out FixedPoint damage)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			FixedPoint value = 0.0;
			if (abilityManager.VisitParameter("LeaderBuffHeadshotCurrentHealthDamageChance", ref value, source) && combatModel.manager.Player.RollDice(RollDiceType.ActivateChance, value, luck) != PlayerRandomChanceResult.Failed)
			{
				FixedPoint value2 = target.Hitpoints;
				string paramName = (source.IsMeleeClass ? "LeaderBuffHeadshotCurrentHealthDamageMultiplierMelee" : "LeaderBuffHeadshotCurrentHealthDamageMultiplierRanged");
				if (!target.HasHeadshotLTTriggered && abilityManager.VisitParameter(paramName, ref value2, source))
				{
					target.HasHeadshotLTTriggered = true;
					damage = value2;
					if (source.Faction == Faction.Survivor)
					{
						TraitEntry traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("LeaderBuffHeadshot");
						if (traitAnyLevel == null)
						{
							traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("BaseHeadshot");
							if (traitAnyLevel != null)
							{
								foreach (ActorModel factionActor in combatModel.GetFactionActors(source.Faction))
								{
									traitAnyLevel = factionActor.TraitContainer.GetTraitAnyLevel("LeaderBuffHeadshot");
									if (traitAnyLevel != null)
									{
										break;
									}
								}
							}
						}
						MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(source.manager);
						if (mapMissionModel != null && traitAnyLevel != null && source is SurvivorModel survivorModel)
						{
							List<DifficultyIncrementalDebuff> challengeDebuffs = mapMissionModel.GetChallengeDebuffs();
							if (source.IsRangedClass)
							{
								FixedPoint minDebuffParamPercentageByTraitId = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(challengeDebuffs, ChallengeDebuffType.DebuffMercerLTRange, traitAnyLevel.TraitIdentifier);
								if (minDebuffParamPercentageByTraitId > 0L)
								{
									int val = (int)(survivorModel.GetDamageForPreferredWeapon() * minDebuffParamPercentageByTraitId);
									damage = Math.Min(val, (int)damage);
								}
							}
							else if (source.IsMeleeClass)
							{
								FixedPoint minDebuffParamPercentageByTraitId2 = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(challengeDebuffs, ChallengeDebuffType.DebuffMercerLTMelee, traitAnyLevel.TraitIdentifier);
								if (minDebuffParamPercentageByTraitId2 > 0L)
								{
									int val2 = (int)(survivorModel.GetDamageForPreferredWeapon() * minDebuffParamPercentageByTraitId2);
									damage = Math.Min(val2, (int)damage);
								}
							}
						}
					}
					return true;
				}
			}
			damage = 0.0;
			return false;
		}

		private static bool IsHaveSurvivorWithinRangEnemy(FixedPoint range, ActorModel sourceActor, ActorModel targetActor, CombatModel combatModel)
		{
			if (sourceActor == null || targetActor == null || combatModel == null || range <= 0.0)
			{
				return false;
			}
			List<ActorModel> models = combatModel.Survivors.Models;
			GridCoordinate gridCoordinate = targetActor.GridCoordinate;
			foreach (ActorModel item in models)
			{
				if (!item.IsDead && !item.IsEnvironmental && item != sourceActor && gridCoordinate.ChebyshevDistance(item.GridCoordinate) <= range)
				{
					return true;
				}
			}
			return false;
		}

		public static PlayerRandomChanceResult ShouldGainCharge(ActorModel source, ActorModel target, PlayerModel player, string probabilityParameterId)
		{
			FixedPoint value = 0.0;
			if (source != null && player.AbilityManager.VisitParameter(probabilityParameterId, ref value, target))
			{
				FixedPoint value2 = 0.0;
				if (value != 0.0)
				{
					player.AbilityManager.VisitParameter("ExtendProbability", ref value2, target);
				}
				PlayerRandomChanceResult playerRandomChanceResult = player.RollDice(RollDiceType.GainChargePoint, value, value2);
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && target.ChargeMeter != null)
				{
					return playerRandomChanceResult;
				}
			}
			return PlayerRandomChanceResult.Failed;
		}

		public static ActorModel FollowThrough(DamageAction damageAction, FixedPoint luck, FixedPoint multiplier, ICollection<ModelAction> addedActions, bool requireSourceActorNeighbour, string notificationKey = null, bool skipAbilityTargetable = false, ICollection<ActorModel> excludedActors = null)
		{
			ActorModel damagerActor = damageAction.DamagerActor;
			ActorModel targetActor = damageAction.TargetActor;
			CombatModel combatModel = damagerActor.manager.CombatModel;
			List<GridCoordinate> list = new List<GridCoordinate>();
			EnumerableNeighbors enumerableNeighbors = combatModel.Grid.Neighbors(targetActor.GridCoordinate);
			if (requireSourceActorNeighbour)
			{
				foreach (GridCoordinate item2 in combatModel.Grid.Neighbors(damagerActor.GridCoordinate))
				{
					foreach (GridCoordinate item3 in enumerableNeighbors)
					{
						if (item2 == item3)
						{
							list.Add(item2);
							break;
						}
					}
				}
			}
			else
			{
				foreach (GridCoordinate item4 in enumerableNeighbors)
				{
					list.Add(item4);
				}
			}
			AbilityModel abilityUnderApplication = combatModel.AbilityManager.AbilityUnderApplication;
			List<ActorModel> list2 = new List<ActorModel>();
			foreach (GridCoordinate item5 in list)
			{
				ActorModel occupier = combatModel.GetOccupier(item5);
				if (occupier != null && damagerActor.IsEnemy(occupier) && occupier.Faction != Faction.Environmental && (skipAbilityTargetable || abilityUnderApplication.CanAbilityBeTargetedOnGridCell(combatModel, damagerActor, targetActor.GridCoordinate, item5)) && (excludedActors == null || !excludedActors.Contains(occupier)))
				{
					list2.Add(occupier);
				}
			}
			ActorModel actorModel = ((list2.Count > 0) ? combatModel.manager.Player.PlayerRandom.GetRandomElement(list2, remove: false) : null);
			int num = -damageAction.HealthAfterDamage;
			if (actorModel != null && num > 0)
			{
				List<DamageNotificationData> list3 = new List<DamageNotificationData>();
				if (notificationKey != null)
				{
					list3.Add(new DamageNotificationData(notificationKey, dueLuck: false));
				}
				int num2 = (int)(multiplier * num);
				if (GetHeadshotTraitDamage(combatModel, damagerActor, actorModel, luck, out var damage))
				{
					num2 += (int)(multiplier * damage);
					list3.Add(new DamageNotificationData("LeaderBuffHeadshot", dueLuck: false));
				}
				DamageAction damageAction2 = new DamageAction(actorModel, damagerActor, num2, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Failed, damageAction.DamageType, actorModel.Faction, new Dictionary<ActorModel, List<DamageNotificationData>> { { actorModel, list3 } });
				damageAction2.IsFollowThrough = true;
				damageAction2.IgnoreIndicatorUpdate = true;
				addedActions.Add(damageAction2);
				damagerActor.FollowThroughTriggeredInAttack = true;
				return actorModel;
			}
			return null;
		}

		public static void SplashDamage(DamageAction damageAction, FixedPoint multiplier)
		{
			ActorModel damagerActor = damageAction.DamagerActor;
			ActorModel targetActor = damageAction.TargetActor;
			CombatModel combatModel = damagerActor.manager.CombatModel;
			new List<GridCoordinate>();
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("FollowAttackWithSplashDamageParam3", ref value, damagerActor);
			FixedPoint value2 = 0.0;
			combatModel.AbilityManager.VisitParameter("FollowAttackWithSplashDamageParam4", ref value2, damagerActor);
			List<ActorModel> enemyFactionsActors = combatModel.GetEnemyFactionsActors(damagerActor.Faction);
			List<ActorModel> list = new List<ActorModel>();
			FixedPoint fixedPoint = value2;
			int num = 0;
			foreach (ActorModel item in enemyFactionsActors)
			{
				if (item.GridCoordinate.Equals(targetActor.GridCoordinate) || !damagerActor.IsEnemy(item))
				{
					continue;
				}
				FixedPoint fixedPoint2 = value;
				if (item.GridCoordinate.DistanceTo(targetActor.GridCoordinate) <= fixedPoint2)
				{
					list.Add(item);
					num++;
					if (num >= (int)fixedPoint)
					{
						break;
					}
				}
			}
			FixedPoint fixedPoint3 = damageAction.FinalDamage * multiplier;
			foreach (ActorModel item2 in list)
			{
				if (item2 != null)
				{
					item2.DealDamage((int)fixedPoint3, damagerActor, DamageType.Base);
					item2.NotifyChange("ActorHealthChanged");
					item2.NotifyChange("HelpHandDamageChanged", (int)fixedPoint3);
				}
			}
		}

		public static bool CheckWeeklyChallengePushAvoid(ActorModel pushedActor)
		{
			if (pushedActor.IsWalker)
			{
				MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(pushedActor.manager);
				if (mapMissionModel != null)
				{
					return mapMissionModel.CheckChallengeDebuffAvoid(ChallengeDebuffType.DebuffPushRate, RollDiceType.AvoidPush);
				}
			}
			if (pushedActor.IsRaider)
			{
				MapMissionModel mapMissionModel2 = MapMissionDebuffHelper.CanUseDebuffMission(pushedActor.manager);
				if (mapMissionModel2 != null)
				{
					return mapMissionModel2.CheckChallengeDebuffAvoid(ChallengeDebuffType.DebuffPushRateRaider, RollDiceType.AvoidPush);
				}
			}
			return false;
		}

		public static bool CheckPreventPush(ActorModel pushedActor)
		{
			FixedPoint value = 0.0;
			pushedActor.manager.Player.AbilityManager.VisitParameter("AbilityModifierPreventPushPercentage", ref value, pushedActor);
			if (value > 0.0)
			{
				FixedPoint value2 = 0.0;
				pushedActor.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value2, pushedActor);
				if (pushedActor.manager.Player.RollDice(RollDiceType.PreventPush, value, value2) != PlayerRandomChanceResult.Failed)
				{
					return true;
				}
			}
			return false;
		}

		public static bool CheckPreventIncendiary(ActorModel actor)
		{
			FixedPoint value = 0.0;
			actor.manager.Player.AbilityManager.VisitParameter("AbilityModifierPreventIncendiaryPercentage", ref value, actor);
			if (value > 0.0)
			{
				FixedPoint value2 = 0.0;
				actor.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
				if (actor.AttributeModel?.GetAttributeModelValue("burn_be_ratio") != 0L)
				{
					FixedPoint value3 = value;
					FixedPoint value4 = 1L;
					FixedPoint? obj = actor.AttributeModel?.GetAttributeModelValue("burn_be_ratio");
					FixedPoint? fixedPoint = value4 + obj;
					value = (value3 * fixedPoint).Value;
				}
				if (actor.manager.Player.RollDice(RollDiceType.PreventIncendiary, value, value2) != PlayerRandomChanceResult.Failed)
				{
					return true;
				}
			}
			return false;
		}

		public static void CalculateRangeExtension(ref FixedPoint range, ActorModel actor, AbilityManagerModel abilityManager)
		{
			abilityManager.VisitParameter("AbilityModifierIncreaseRange", ref range, actor);
			abilityManager.VisitParameter("AbilityModifierEquipmentPassiveFortunaClub", ref range, actor);
			range += AbilityRangeTridentSkill.GetActiveMiddleExtraRange(actor);
			if (actor.FocusModeState && !actor.SelectedAbility.IsChargeAttack)
			{
				abilityManager.VisitParameter("AbilityModifierFocusModeAttackDistance", ref range, actor);
			}
			EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
			if (weaponEquipment != null && weaponEquipment.Definition != null && weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon)
			{
				range = actor.GetCitadel_RangeDown_Range(range);
			}
			MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(actor.manager);
			if (actor.Faction == Faction.Raider)
			{
				range = FixedPoint.Min(range, abilityManager.manager.GameEconomyData.ConfigData.EnemySurvivorsMaxRange);
			}
			else if (actor.IsRangedClass && mapMissionModel != null)
			{
				List<DifficultyIncrementalDebuff> challengeDebuffs = mapMissionModel.GetChallengeDebuffs();
				range -= ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffRangeShorten);
				range = FixedPoint.Min(range, ChallengeDebufHelps.GetDebufMinFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffRangeLimit));
				range = FixedPoint.Max(range, 1L);
			}
		}

		private static void ApplyBuffKnockKnock(ActorModel actor)
		{
			if (actor.TraitContainer.GetTraitAnyLevel("BaseKnockKnock") == null)
			{
				return;
			}
			CombatModel combatModel = actor.manager.CombatModel;
			if (combatModel == null || combatModel.MissionCompleted)
			{
				return;
			}
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("LeaderBuffKnockKnockTargetMaxNum", ref value, actor);
			if (value <= 0L)
			{
				return;
			}
			int val = (int)value;
			List<ActorModel> targets = new List<ActorModel>();
			foreach (ActorModel allActor in combatModel.GetAllActors())
			{
				if (allActor.IsEnemy(actor) && !allActor.IsEnvironmental && !allActor.IsDead && allActor.IsVisibleToSurvivors)
				{
					targets.Add(allActor);
				}
			}
			if (targets.Count > 0)
			{
				GetSortedActors(actor, in targets);
				for (int i = 0; i < Math.Min(val, targets.Count); i++)
				{
					ActorModel actorModel = targets[i];
					actorModel.DebuffKnockKnockMarkCount += (FixedPoint)1L;
					FixedPoint value2 = 0L;
					combatModel.AbilityManager.VisitParameter("LeaderBuffKnockKnockMarkMaxNum", ref value2, actor);
					actorModel.DebuffKnockKnockMarkMaxConfig = value2;
					actorModel.NotifyChange("KnockKnockMarkUpdateEvent", new object[2] { "LeaderBuffKnockKnock", false });
				}
			}
		}

		private static List<ActorModel> GetSortedActors(ActorModel actorModel, in List<ActorModel> targets)
		{
			targets.StableSort(delegate(ActorModel actor1, ActorModel actor2)
			{
				int num = actorModel.GridCoordinate.ChebyshevDistance(actor1.GridCoordinate);
				int num2 = actorModel.GridCoordinate.ChebyshevDistance(actor2.GridCoordinate);
				if (num == num2)
				{
					if (actor1.Definition.IsSpecial && !actor2.Definition.IsSpecial)
					{
						return -1;
					}
					if (!actor1.Definition.IsSpecial && actor2.Definition.IsSpecial)
					{
						return 1;
					}
				}
				FixedVec2 fixedVec = actorModel.GridCoordinate.ToVector2() - actor1.GridCoordinate.ToVector2();
				FixedVec2 fixedVec2 = actorModel.GridCoordinate.ToVector2() - actor2.GridCoordinate.ToVector2();
				return (fixedVec.SqrMagnitude >= fixedVec2.SqrMagnitude) ? 1 : (-1);
			});
			return targets;
		}

		public static void CheckAPJadis(CombatModel combatModel, ActorModel source, ActorModel target)
		{
			if (combatModel == null || combatModel.manager == null || source == null || target == null || source.Faction != Faction.Survivor)
			{
				return;
			}
			ActorModel.ABtestParam aBtestParam = target.GetABtestParam();
			if (aBtestParam != null && aBtestParam.B_APChance > 0L && aBtestParam.B_source != null)
			{
				PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.ABTesterTrait2, aBtestParam.B_APChance);
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					source.AddChargePoints(1);
					source.NotifyChange("AbilityVisited", new object[2]
					{
						"LeaderBuffABTester",
						playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
					});
					aBtestParam.B_source.AddChargePoints(1);
					aBtestParam.B_source.NotifyChange("AbilityVisited", new object[2]
					{
						"LeaderBuffABTester",
						playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
					});
				}
			}
		}

		public static ActorModel getHelpHandActor(CombatModel combatModel, ActorModel target)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			_ = combatModel.manager.GameEconomyData;
			List<ActorModel> factionActors = combatModel.GetFactionActors(target.Faction);
			List<ActorModel> list = new List<ActorModel>();
			bool flag = false;
			foreach (ActorModel item in factionActors)
			{
				if (item.GridCoordinate.Equals(target.GridCoordinate) || (!item.HasAnyLevelTrait("Equipment.HelpHand") && !item.HasAnyLevelTrait("Equipment_Passive_HelpHand")))
				{
					continue;
				}
				FixedPoint value = 0.0;
				abilityManager.VisitParameter("HelpHandNumberOfGuardianGrids", ref value, item);
				if (item.GridCoordinate.DistanceTo(target.GridCoordinate) <= value)
				{
					flag = true;
				}
				if (flag)
				{
					FixedPoint value2 = 0.0;
					abilityManager.VisitParameter("HelpHandGuardianshipProbability", ref value2, item);
					FixedPoint successProbabilityExtension = 0.0;
					if (value2 != 0.0 && item.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value2, successProbabilityExtension) != PlayerRandomChanceResult.Failed)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 1)
			{
				FixedPoint fixedPoint = target.GridCoordinate.DistanceTo(list[0].GridCoordinate);
				FixedPoint fixedPoint2 = target.GridCoordinate.DistanceTo(list[1].GridCoordinate);
				if (fixedPoint > fixedPoint2)
				{
					return list[1];
				}
				if (fixedPoint < fixedPoint2)
				{
					return list[0];
				}
				if (fixedPoint == fixedPoint2)
				{
					if (list[0].Hitpoints > list[1].Hitpoints)
					{
						return list[0];
					}
					if (list[0].Hitpoints < list[1].Hitpoints)
					{
						return list[1];
					}
					if (list[0].Hitpoints == list[1].Hitpoints)
					{
						if (target.manager.Player.PlayerRandom.GetRandomInRange(0, 1) != 0)
						{
							return list[1];
						}
						return list[0];
					}
				}
				return null;
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}

		public static ActorModel getGuardActor(CombatModel combatModel, ActorModel target)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			_ = combatModel.manager.GameEconomyData;
			List<ActorModel> factionActors = combatModel.GetFactionActors(target.Faction);
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel item in factionActors)
			{
				if (!item.GridCoordinate.Equals(target.GridCoordinate) && item.HasTraitsThatContains("SupportTalent_Guard"))
				{
					FixedPoint value = 0.0;
					abilityManager.VisitParameter("SupportTalent_GuardParm1", ref value, item);
					if (item.GridCoordinate.DistanceTo(target.GridCoordinate) <= value)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 1)
			{
				FixedPoint fixedPoint = target.GridCoordinate.DistanceTo(list[0].GridCoordinate);
				FixedPoint fixedPoint2 = target.GridCoordinate.DistanceTo(list[1].GridCoordinate);
				if (fixedPoint > fixedPoint2)
				{
					return list[1];
				}
				if (fixedPoint < fixedPoint2)
				{
					return list[0];
				}
				if (fixedPoint == fixedPoint2)
				{
					if (list[0].Hitpoints > list[1].Hitpoints)
					{
						return list[0];
					}
					if (list[0].Hitpoints < list[1].Hitpoints)
					{
						return list[1];
					}
					if (list[0].Hitpoints == list[1].Hitpoints)
					{
						if (target.manager.Player.PlayerRandom.GetRandomInRange(0, 1) != 0)
						{
							return list[1];
						}
						return list[0];
					}
				}
				return null;
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}

		public static int[] HelpHandDamageCalculation(CombatModel combatModel, PreDealDamageAction preDealDamageAction, ActorModel target, ActorModel helpHandActor, DamageType type, FixedPoint damageWithoutCrit, FixedPoint additionalCritDamage)
		{
			if (combatModel?.AbilityManager != null && combatModel.manager?.GameEconomyData?.ConfigData != null && combatModel.manager.Player != null && preDealDamageAction?.DamageAction != null && target != null && helpHandActor != null)
			{
				AbilityManagerModel abilityManager = combatModel.AbilityManager;
				_ = combatModel.manager.GameEconomyData;
				ActorModel actorModel = preDealDamageAction.DamageAction.DamagerActor ?? target;
				FixedPoint value = 0.0;
				abilityManager.VisitParameter("HelpHandGuardianDamageValues", ref value, helpHandActor);
				if (!(value <= 0.0))
				{
					FixedPoint damageWithoutCrit2 = damageWithoutCrit * value;
					FixedPoint additionalCritDamage2 = additionalCritDamage * value;
					damageWithoutCrit -= damageWithoutCrit2;
					additionalCritDamage -= additionalCritDamage2;
					FixedPoint fixedPoint = 100.0;
					FixedPoint fixedPoint2 = 0.0;
					_ = (FixedPoint)0.0;
					FixedPoint fixedPoint3 = 0.0;
					FixedPoint successProbabilityExtension = 0.0;
					if (type != DamageType.Heal)
					{
						StrengthenDefense(helpHandActor, combatModel, ref damageWithoutCrit2, ref additionalCritDamage2);
						FixedPoint fixedPoint4 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * damageWithoutCrit2;
						FixedPoint fixedPoint5 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * (fixedPoint / 100.0) * additionalCritDamage2;
						FixedPoint value2 = 0.0;
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistance", ref value2, helpHandActor);
						abilityManager.VisitParameter("SupportTalent_GuardParm3", ref value2, helpHandActor);
						switch (type)
						{
						case DamageType.Melee:
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMelee", ref value2, helpHandActor);
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMeleeArmor", ref value2, helpHandActor);
							break;
						case DamageType.Ranged:
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRanged", ref value2, helpHandActor);
							break;
						}
						value2 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value2);
						if (fixedPoint4 > 0.0)
						{
							fixedPoint2 = FixedPoint.Min(damageWithoutCrit2 * value2, fixedPoint4);
							fixedPoint3 += fixedPoint2;
							damageWithoutCrit2 -= fixedPoint2;
							fixedPoint4 -= fixedPoint2;
						}
						if (fixedPoint5 > 0.0)
						{
							fixedPoint2 = FixedPoint.Min(additionalCritDamage2 * value2, fixedPoint5);
							fixedPoint3 += fixedPoint2;
							additionalCritDamage2 -= fixedPoint2;
							fixedPoint5 -= fixedPoint2;
						}
						FixedPoint value3 = 0.0;
						if (helpHandActor.HadActionPointsAtEndOfTurn || helpHandActor.OverwatchedOnTurn)
						{
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceOverwatch", ref value3, helpHandActor);
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseNewResistanceOverwatch", ref value3, helpHandActor);
							value3 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value3);
							if (fixedPoint4 > 0.0)
							{
								fixedPoint2 = FixedPoint.Min(damageWithoutCrit2 * value3, fixedPoint4);
								fixedPoint3 += fixedPoint2;
								damageWithoutCrit2 -= fixedPoint2;
								fixedPoint4 -= fixedPoint2;
							}
							if (fixedPoint5 > 0.0)
							{
								fixedPoint2 = FixedPoint.Min(additionalCritDamage2 * value3, fixedPoint5);
								fixedPoint3 += fixedPoint2;
								additionalCritDamage2 -= fixedPoint2;
								fixedPoint5 -= fixedPoint2;
							}
						}
						if (helpHandActor.HasAnyLevelTrait("LeaderBuffBodyguard") || combatModel.IsTargetNextToActorWithTrait(helpHandActor, "LeaderBuffBodyguard"))
						{
							FixedPoint value4 = 0.0;
							abilityManager.VisitParameter("AbilityModifierIncreaseChanceForBodyguard", ref value4, helpHandActor);
							PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.Generic, value4, successProbabilityExtension);
							FixedPoint value5 = 0.0;
							abilityManager.VisitParameter("AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry", ref value5, helpHandActor);
							if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
							{
								if (fixedPoint4 > 0.0)
								{
									fixedPoint2 = FixedPoint.Min(damageWithoutCrit2 * value5, fixedPoint4);
									fixedPoint3 += fixedPoint2;
									damageWithoutCrit2 -= fixedPoint2;
									fixedPoint4 -= fixedPoint2;
								}
								if (fixedPoint5 > 0.0)
								{
									fixedPoint2 = FixedPoint.Min(additionalCritDamage2 * value5, fixedPoint5);
									fixedPoint3 += fixedPoint2;
									additionalCritDamage2 -= fixedPoint2;
									fixedPoint5 -= fixedPoint2;
								}
								helpHandActor.NotifyChange("AbilityVisited", new object[2]
								{
									"LeaderBuffBodyguard",
									playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
								});
							}
						}
						if ((type == DamageType.Base || type == DamageType.Melee || type == DamageType.Ranged || type == DamageType.Struggle) && helpHandActor.IsHuman)
						{
							if (helpHandActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Scout"))
							{
								FixedPoint value6 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutAttackedByHighLevel", ref value6, helpHandActor);
								if (actorModel.Level - helpHandActor.Level > value6)
								{
									FixedPoint value7 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutLevelDifference", ref value7, helpHandActor);
									FixedPoint value8 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutDamageReduction", ref value8, helpHandActor);
									FixedPoint value9 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaximumLiftingValue", ref value9, helpHandActor);
									FixedPoint value10 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaxLeveLimitValue", ref value10, target);
									FixedPoint fixedPoint6 = 0L;
									fixedPoint6 = ((!(actorModel.Level - helpHandActor.Level > value10)) ? ((FixedPoint)Math.Pow((double)(1L - value8), (double)((actorModel.Level - helpHandActor.Level - value6) / value7))) : ((FixedPoint)Math.Pow((double)(1L - value8), (double)((value10 - value6) / value7))));
									if (fixedPoint6 > 50L)
									{
										fixedPoint6 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint2 = ((!(damageWithoutCrit * fixedPoint6 / damageWithoutCrit2 < value9)) ? (damageWithoutCrit * fixedPoint6) : (damageWithoutCrit2 * value9));
										FixedPoint fixedPoint7 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint7;
										damageWithoutCrit2 = fixedPoint2;
										fixedPoint4 -= fixedPoint7;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint2 = ((!(additionalCritDamage * fixedPoint6 / additionalCritDamage2 < value9)) ? (additionalCritDamage * fixedPoint6) : (additionalCritDamage2 * value9));
										FixedPoint fixedPoint8 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint8;
										additionalCritDamage2 = fixedPoint2;
										fixedPoint5 -= fixedPoint8;
									}
								}
							}
							if (helpHandActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Bruiser"))
							{
								FixedPoint value11 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserAttackedByHighLevel", ref value11, helpHandActor);
								if (actorModel.Level - helpHandActor.Level > value11)
								{
									FixedPoint value12 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserLevelDifference", ref value12, helpHandActor);
									FixedPoint value13 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserDamageReduction", ref value13, helpHandActor);
									FixedPoint value14 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaximumLiftingValue", ref value14, helpHandActor);
									FixedPoint value15 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaxLeveLimitValue", ref value15, target);
									FixedPoint fixedPoint9 = 0L;
									fixedPoint9 = ((!(actorModel.Level - helpHandActor.Level > value15)) ? ((FixedPoint)Math.Pow((double)(1L - value13), (double)((actorModel.Level - helpHandActor.Level - value11) / value12))) : ((FixedPoint)Math.Pow((double)(1L - value13), (double)((value15 - value11) / value12))));
									if (fixedPoint9 > 50L)
									{
										fixedPoint9 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint2 = ((!(damageWithoutCrit * fixedPoint9 / damageWithoutCrit2 < value14)) ? (damageWithoutCrit * fixedPoint9) : (damageWithoutCrit2 * value14));
										FixedPoint fixedPoint10 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint10;
										damageWithoutCrit2 = fixedPoint2;
										fixedPoint4 -= fixedPoint10;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint2 = ((!(additionalCritDamage * fixedPoint9 / additionalCritDamage2 < value14)) ? (additionalCritDamage * fixedPoint9) : (additionalCritDamage2 * value14));
										FixedPoint fixedPoint11 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint11;
										additionalCritDamage2 = fixedPoint2;
										fixedPoint5 -= fixedPoint11;
									}
								}
							}
							if (helpHandActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Warrior"))
							{
								FixedPoint value16 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorAttackedByHighLevel", ref value16, helpHandActor);
								if (actorModel.Level - helpHandActor.Level > value16)
								{
									FixedPoint value17 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorLevelDifference", ref value17, helpHandActor);
									FixedPoint value18 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorDamageReduction", ref value18, helpHandActor);
									FixedPoint value19 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaximumLiftingValue", ref value19, helpHandActor);
									FixedPoint value20 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaxLeveLimitValue", ref value20, target);
									FixedPoint fixedPoint12 = 0L;
									fixedPoint12 = ((!(actorModel.Level - helpHandActor.Level > value20)) ? ((FixedPoint)Math.Pow((double)(1L - value18), (double)((actorModel.Level - helpHandActor.Level - value16) / value17))) : ((FixedPoint)Math.Pow((double)(1L - value18), (double)((value20 - value16) / value17))));
									if (fixedPoint12 > 50L)
									{
										fixedPoint12 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint2 = ((!(damageWithoutCrit * fixedPoint12 / damageWithoutCrit2 < value19)) ? (damageWithoutCrit * fixedPoint12) : (damageWithoutCrit2 * value19));
										FixedPoint fixedPoint13 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint13;
										damageWithoutCrit2 = fixedPoint2;
										fixedPoint4 -= fixedPoint13;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint2 = ((!(additionalCritDamage * fixedPoint12 / additionalCritDamage2 < value19)) ? (additionalCritDamage * fixedPoint12) : (additionalCritDamage2 * value19));
										FixedPoint fixedPoint14 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint14;
										additionalCritDamage2 = fixedPoint2;
										fixedPoint5 -= fixedPoint14;
									}
								}
							}
							if (helpHandActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Shooter"))
							{
								FixedPoint value21 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterAttackedByHighLevel", ref value21, helpHandActor);
								if (actorModel.Level - helpHandActor.Level > value21)
								{
									FixedPoint value22 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterLevelDifference", ref value22, helpHandActor);
									FixedPoint value23 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterDamageReduction", ref value23, helpHandActor);
									FixedPoint value24 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaximumLiftingValue", ref value24, helpHandActor);
									FixedPoint value25 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaxLeveLimitValue", ref value25, target);
									FixedPoint fixedPoint15 = 0L;
									fixedPoint15 = ((!(actorModel.Level - helpHandActor.Level > value25)) ? ((FixedPoint)Math.Pow((double)(1L - value23), (double)((actorModel.Level - helpHandActor.Level - value21) / value22))) : ((FixedPoint)Math.Pow((double)(1L - value23), (double)((value25 - value21) / value22))));
									if (fixedPoint15 > 50L)
									{
										fixedPoint15 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint2 = ((!(damageWithoutCrit * fixedPoint15 / damageWithoutCrit2 < value24)) ? (damageWithoutCrit * fixedPoint15) : (damageWithoutCrit2 * value24));
										FixedPoint fixedPoint16 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint16;
										damageWithoutCrit2 = fixedPoint2;
										fixedPoint4 -= fixedPoint16;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint2 = ((!(additionalCritDamage * fixedPoint15 / additionalCritDamage2 < value24)) ? (additionalCritDamage * fixedPoint15) : (additionalCritDamage2 * value24));
										FixedPoint fixedPoint17 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint17;
										additionalCritDamage2 = fixedPoint2;
										fixedPoint5 -= fixedPoint17;
									}
								}
							}
							if (helpHandActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Hunter"))
							{
								FixedPoint value26 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterAttackedByHighLevel", ref value26, helpHandActor);
								if (actorModel.Level - helpHandActor.Level > value26)
								{
									FixedPoint value27 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterLevelDifference", ref value27, helpHandActor);
									FixedPoint value28 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterDamageReduction", ref value28, helpHandActor);
									FixedPoint value29 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaximumLiftingValue", ref value29, helpHandActor);
									FixedPoint value30 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaxLeveLimitValue", ref value30, target);
									FixedPoint fixedPoint18 = 0L;
									fixedPoint18 = ((!(actorModel.Level - helpHandActor.Level > value30)) ? ((FixedPoint)Math.Pow((double)(1L - value28), (double)((actorModel.Level - helpHandActor.Level - value26) / value27))) : ((FixedPoint)Math.Pow((double)(1L - value28), (double)((value30 - value26) / value27))));
									if (fixedPoint18 > 50L)
									{
										fixedPoint18 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint2 = ((!(damageWithoutCrit * fixedPoint18 / damageWithoutCrit2 < value29)) ? (damageWithoutCrit * fixedPoint18) : (damageWithoutCrit2 * value29));
										FixedPoint fixedPoint19 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint19;
										damageWithoutCrit2 = fixedPoint2;
										fixedPoint4 -= fixedPoint19;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint2 = ((!(additionalCritDamage * fixedPoint18 / additionalCritDamage2 < value29)) ? (additionalCritDamage * fixedPoint18) : (additionalCritDamage2 * value29));
										FixedPoint fixedPoint20 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint20;
										additionalCritDamage2 = fixedPoint2;
										fixedPoint5 -= fixedPoint20;
									}
								}
							}
							if (helpHandActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Assault"))
							{
								FixedPoint value31 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultAttackedByHighLevel", ref value31, helpHandActor);
								if (actorModel.Level - helpHandActor.Level > value31)
								{
									FixedPoint value32 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultLevelDifference", ref value32, helpHandActor);
									FixedPoint value33 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultDamageReduction", ref value33, helpHandActor);
									FixedPoint value34 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaximumLiftingValue", ref value34, helpHandActor);
									FixedPoint value35 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaxLeveLimitValue", ref value35, target);
									FixedPoint fixedPoint21 = 0L;
									fixedPoint21 = ((!(actorModel.Level - helpHandActor.Level > value35)) ? ((FixedPoint)Math.Pow((double)(1L - value33), (double)((actorModel.Level - helpHandActor.Level - value31) / value32))) : ((FixedPoint)Math.Pow((double)(1L - value33), (double)((value35 - value31) / value32))));
									if (fixedPoint21 > 50L)
									{
										fixedPoint21 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint2 = ((!(damageWithoutCrit * fixedPoint21 / damageWithoutCrit2 < value34)) ? (damageWithoutCrit * fixedPoint21) : (damageWithoutCrit2 * value34));
										FixedPoint fixedPoint22 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint22;
										damageWithoutCrit2 = fixedPoint2;
										fixedPoint4 -= fixedPoint22;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint2 = ((!(additionalCritDamage * fixedPoint21 / additionalCritDamage2 < value34)) ? (additionalCritDamage * fixedPoint21) : (additionalCritDamage2 * value34));
										FixedPoint fixedPoint23 = damageWithoutCrit - fixedPoint2;
										fixedPoint3 += fixedPoint23;
										additionalCritDamage2 = fixedPoint2;
										fixedPoint5 -= fixedPoint23;
									}
								}
							}
						}
					}
					return new int[4]
					{
						(int)damageWithoutCrit,
						(int)additionalCritDamage,
						(int)damageWithoutCrit2,
						(int)additionalCritDamage2
					};
				}
				return new int[4]
				{
					(int)damageWithoutCrit,
					(int)additionalCritDamage,
					0,
					0
				};
			}
			return new int[4]
			{
				(int)damageWithoutCrit,
				(int)additionalCritDamage,
				0,
				0
			};
		}

		public static int[] GuardianVowTransferDamageCalculation(CombatModel combatModel, PreDealDamageAction preDealDamageAction, ActorModel target, ActorModel guardianActor, DamageType type, FixedPoint damageWithoutCrit, FixedPoint additionalCritDamage, FixedPoint transferRatio, FixedPoint transferReduction)
		{
			if (combatModel?.AbilityManager != null && combatModel.manager?.GameEconomyData?.ConfigData != null && combatModel.manager.Player != null && preDealDamageAction?.DamageAction != null && target != null && guardianActor != null)
			{
				if (!(transferRatio <= 0.0))
				{
					AbilityManagerModel abilityManager = combatModel.AbilityManager;
					ActorModel actorModel = preDealDamageAction.DamageAction.DamagerActor ?? target;
					FixedPoint damageWithoutCrit2 = damageWithoutCrit * transferRatio;
					FixedPoint additionalCritDamage2 = additionalCritDamage * transferRatio;
					damageWithoutCrit -= damageWithoutCrit2;
					additionalCritDamage -= additionalCritDamage2;
					if (transferReduction > 0.0)
					{
						if (transferReduction > 1.0)
						{
							transferReduction = 1.0;
						}
						damageWithoutCrit2 *= 1.0 - transferReduction;
						additionalCritDamage2 *= 1.0 - transferReduction;
					}
					FixedPoint fixedPoint = 0.0;
					FixedPoint fixedPoint2 = 0.0;
					FixedPoint successProbabilityExtension = 0.0;
					FixedPoint fixedPoint3 = 100.0;
					if (type != DamageType.Heal)
					{
						StrengthenDefense(guardianActor, combatModel, ref damageWithoutCrit2, ref additionalCritDamage2);
						FixedPoint fixedPoint4 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * damageWithoutCrit2;
						FixedPoint fixedPoint5 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * (fixedPoint3 / 100.0) * additionalCritDamage2;
						FixedPoint value = 0.0;
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistance", ref value, guardianActor);
						abilityManager.VisitParameter("SupportTalent_GuardParm3", ref value, guardianActor);
						switch (type)
						{
						case DamageType.Melee:
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMelee", ref value, guardianActor);
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMeleeArmor", ref value, guardianActor);
							break;
						case DamageType.Ranged:
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRanged", ref value, guardianActor);
							break;
						}
						value = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value);
						if (fixedPoint4 > 0.0)
						{
							fixedPoint = FixedPoint.Min(damageWithoutCrit2 * value, fixedPoint4);
							fixedPoint2 += fixedPoint;
							damageWithoutCrit2 -= fixedPoint;
							fixedPoint4 -= fixedPoint;
						}
						if (fixedPoint5 > 0.0)
						{
							fixedPoint = FixedPoint.Min(additionalCritDamage2 * value, fixedPoint5);
							fixedPoint2 += fixedPoint;
							additionalCritDamage2 -= fixedPoint;
							fixedPoint5 -= fixedPoint;
						}
						if (guardianActor.HadActionPointsAtEndOfTurn || guardianActor.OverwatchedOnTurn)
						{
							FixedPoint value2 = 0.0;
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceOverwatch", ref value2, guardianActor);
							abilityManager.VisitParameter("AbilityModifierPercentageIncreaseNewResistanceOverwatch", ref value2, guardianActor);
							value2 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value2);
							if (fixedPoint4 > 0.0)
							{
								fixedPoint = FixedPoint.Min(damageWithoutCrit2 * value2, fixedPoint4);
								fixedPoint2 += fixedPoint;
								damageWithoutCrit2 -= fixedPoint;
								fixedPoint4 -= fixedPoint;
							}
							if (fixedPoint5 > 0.0)
							{
								fixedPoint = FixedPoint.Min(additionalCritDamage2 * value2, fixedPoint5);
								fixedPoint2 += fixedPoint;
								additionalCritDamage2 -= fixedPoint;
								fixedPoint5 -= fixedPoint;
							}
						}
						if (guardianActor.HasAnyLevelTrait("LeaderBuffBodyguard") || combatModel.IsTargetNextToActorWithTrait(guardianActor, "LeaderBuffBodyguard"))
						{
							FixedPoint value3 = 0.0;
							abilityManager.VisitParameter("AbilityModifierIncreaseChanceForBodyguard", ref value3, guardianActor);
							PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.Generic, value3, successProbabilityExtension);
							FixedPoint value4 = 0.0;
							abilityManager.VisitParameter("AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry", ref value4, guardianActor);
							if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
							{
								if (fixedPoint4 > 0.0)
								{
									fixedPoint = FixedPoint.Min(damageWithoutCrit2 * value4, fixedPoint4);
									fixedPoint2 += fixedPoint;
									damageWithoutCrit2 -= fixedPoint;
									fixedPoint4 -= fixedPoint;
								}
								if (fixedPoint5 > 0.0)
								{
									fixedPoint = FixedPoint.Min(additionalCritDamage2 * value4, fixedPoint5);
									fixedPoint2 += fixedPoint;
									additionalCritDamage2 -= fixedPoint;
									fixedPoint5 -= fixedPoint;
								}
								guardianActor.NotifyChange("AbilityVisited", new object[2]
								{
									"LeaderBuffBodyguard",
									playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
								});
							}
						}
						if ((type == DamageType.Base || type == DamageType.Melee || type == DamageType.Ranged || type == DamageType.Struggle) && guardianActor.IsHuman)
						{
							if (guardianActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Scout"))
							{
								FixedPoint value5 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutAttackedByHighLevel", ref value5, guardianActor);
								if (actorModel.Level - guardianActor.Level > value5)
								{
									FixedPoint value6 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutLevelDifference", ref value6, guardianActor);
									FixedPoint value7 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutDamageReduction", ref value7, guardianActor);
									FixedPoint value8 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaximumLiftingValue", ref value8, guardianActor);
									FixedPoint value9 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaxLeveLimitValue", ref value9, target);
									FixedPoint fixedPoint6 = 0L;
									fixedPoint6 = ((!(actorModel.Level - guardianActor.Level > value9)) ? ((FixedPoint)Math.Pow((double)(1L - value7), (double)((actorModel.Level - guardianActor.Level - value5) / value6))) : ((FixedPoint)Math.Pow((double)(1L - value7), (double)((value9 - value5) / value6))));
									if (fixedPoint6 > 50L)
									{
										fixedPoint6 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint = ((!(damageWithoutCrit * fixedPoint6 / damageWithoutCrit2 < value8)) ? (damageWithoutCrit * fixedPoint6) : (damageWithoutCrit2 * value8));
										FixedPoint fixedPoint7 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint7;
										damageWithoutCrit2 = fixedPoint;
										fixedPoint4 -= fixedPoint7;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint = ((!(additionalCritDamage * fixedPoint6 / additionalCritDamage2 < value8)) ? (additionalCritDamage * fixedPoint6) : (additionalCritDamage2 * value8));
										FixedPoint fixedPoint8 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint8;
										additionalCritDamage2 = fixedPoint;
										fixedPoint5 -= fixedPoint8;
									}
								}
							}
							if (guardianActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Bruiser"))
							{
								FixedPoint value10 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserAttackedByHighLevel", ref value10, guardianActor);
								if (actorModel.Level - guardianActor.Level > value10)
								{
									FixedPoint value11 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserLevelDifference", ref value11, guardianActor);
									FixedPoint value12 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserDamageReduction", ref value12, guardianActor);
									FixedPoint value13 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaximumLiftingValue", ref value13, guardianActor);
									FixedPoint value14 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaxLeveLimitValue", ref value14, target);
									FixedPoint fixedPoint9 = 0L;
									fixedPoint9 = ((!(actorModel.Level - guardianActor.Level > value14)) ? ((FixedPoint)Math.Pow((double)(1L - value12), (double)((actorModel.Level - guardianActor.Level - value10) / value11))) : ((FixedPoint)Math.Pow((double)(1L - value12), (double)((value14 - value10) / value11))));
									if (fixedPoint9 > 50L)
									{
										fixedPoint9 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint = ((!(damageWithoutCrit * fixedPoint9 / damageWithoutCrit2 < value13)) ? (damageWithoutCrit * fixedPoint9) : (damageWithoutCrit2 * value13));
										FixedPoint fixedPoint10 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint10;
										damageWithoutCrit2 = fixedPoint;
										fixedPoint4 -= fixedPoint10;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint = ((!(additionalCritDamage * fixedPoint9 / additionalCritDamage2 < value13)) ? (additionalCritDamage * fixedPoint9) : (additionalCritDamage2 * value13));
										FixedPoint fixedPoint11 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint11;
										additionalCritDamage2 = fixedPoint;
										fixedPoint5 -= fixedPoint11;
									}
								}
							}
							if (guardianActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Warrior"))
							{
								FixedPoint value15 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorAttackedByHighLevel", ref value15, guardianActor);
								if (actorModel.Level - guardianActor.Level > value15)
								{
									FixedPoint value16 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorLevelDifference", ref value16, guardianActor);
									FixedPoint value17 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorDamageReduction", ref value17, guardianActor);
									FixedPoint value18 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaximumLiftingValue", ref value18, guardianActor);
									FixedPoint value19 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaxLeveLimitValue", ref value19, target);
									FixedPoint fixedPoint12 = 0L;
									fixedPoint12 = ((!(actorModel.Level - guardianActor.Level > value19)) ? ((FixedPoint)Math.Pow((double)(1L - value17), (double)((actorModel.Level - guardianActor.Level - value15) / value16))) : ((FixedPoint)Math.Pow((double)(1L - value17), (double)((value19 - value15) / value16))));
									if (fixedPoint12 > 50L)
									{
										fixedPoint12 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint = ((!(damageWithoutCrit * fixedPoint12 / damageWithoutCrit2 < value18)) ? (damageWithoutCrit * fixedPoint12) : (damageWithoutCrit2 * value18));
										FixedPoint fixedPoint13 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint13;
										damageWithoutCrit2 = fixedPoint;
										fixedPoint4 -= fixedPoint13;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint = ((!(additionalCritDamage * fixedPoint12 / additionalCritDamage2 < value18)) ? (additionalCritDamage * fixedPoint12) : (additionalCritDamage2 * value18));
										FixedPoint fixedPoint14 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint14;
										additionalCritDamage2 = fixedPoint;
										fixedPoint5 -= fixedPoint14;
									}
								}
							}
							if (guardianActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Shooter"))
							{
								FixedPoint value20 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterAttackedByHighLevel", ref value20, guardianActor);
								if (actorModel.Level - guardianActor.Level > value20)
								{
									FixedPoint value21 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterLevelDifference", ref value21, guardianActor);
									FixedPoint value22 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterDamageReduction", ref value22, guardianActor);
									FixedPoint value23 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaximumLiftingValue", ref value23, guardianActor);
									FixedPoint value24 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaxLeveLimitValue", ref value24, target);
									FixedPoint fixedPoint15 = 0L;
									fixedPoint15 = ((!(actorModel.Level - guardianActor.Level > value24)) ? ((FixedPoint)Math.Pow((double)(1L - value22), (double)((actorModel.Level - guardianActor.Level - value20) / value21))) : ((FixedPoint)Math.Pow((double)(1L - value22), (double)((value24 - value20) / value21))));
									if (fixedPoint15 > 50L)
									{
										fixedPoint15 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint = ((!(damageWithoutCrit * fixedPoint15 / damageWithoutCrit2 < value23)) ? (damageWithoutCrit * fixedPoint15) : (damageWithoutCrit2 * value23));
										FixedPoint fixedPoint16 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint16;
										damageWithoutCrit2 = fixedPoint;
										fixedPoint4 -= fixedPoint16;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint = ((!(additionalCritDamage * fixedPoint15 / additionalCritDamage2 < value23)) ? (additionalCritDamage * fixedPoint15) : (additionalCritDamage2 * value23));
										FixedPoint fixedPoint17 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint17;
										additionalCritDamage2 = fixedPoint;
										fixedPoint5 -= fixedPoint17;
									}
								}
							}
							if (guardianActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Hunter"))
							{
								FixedPoint value25 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterAttackedByHighLevel", ref value25, guardianActor);
								if (actorModel.Level - guardianActor.Level > value25)
								{
									FixedPoint value26 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterLevelDifference", ref value26, guardianActor);
									FixedPoint value27 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterDamageReduction", ref value27, guardianActor);
									FixedPoint value28 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaximumLiftingValue", ref value28, guardianActor);
									FixedPoint value29 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaxLeveLimitValue", ref value29, target);
									FixedPoint fixedPoint18 = 0L;
									fixedPoint18 = ((!(actorModel.Level - guardianActor.Level > value29)) ? ((FixedPoint)Math.Pow((double)(1L - value27), (double)((actorModel.Level - guardianActor.Level - value25) / value26))) : ((FixedPoint)Math.Pow((double)(1L - value27), (double)((value29 - value25) / value26))));
									if (fixedPoint18 > 50L)
									{
										fixedPoint18 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint = ((!(damageWithoutCrit * fixedPoint18 / damageWithoutCrit2 < value28)) ? (damageWithoutCrit * fixedPoint18) : (damageWithoutCrit2 * value28));
										FixedPoint fixedPoint19 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint19;
										damageWithoutCrit2 = fixedPoint;
										fixedPoint4 -= fixedPoint19;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint = ((!(additionalCritDamage * fixedPoint18 / additionalCritDamage2 < value28)) ? (additionalCritDamage * fixedPoint18) : (additionalCritDamage2 * value28));
										FixedPoint fixedPoint20 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint20;
										additionalCritDamage2 = fixedPoint;
										fixedPoint5 -= fixedPoint20;
									}
								}
							}
							if (guardianActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Assault"))
							{
								FixedPoint value30 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultAttackedByHighLevel", ref value30, guardianActor);
								if (actorModel.Level - guardianActor.Level > value30)
								{
									FixedPoint value31 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultLevelDifference", ref value31, guardianActor);
									FixedPoint value32 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultDamageReduction", ref value32, guardianActor);
									FixedPoint value33 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaximumLiftingValue", ref value33, guardianActor);
									FixedPoint value34 = 0.0;
									combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaxLeveLimitValue", ref value34, target);
									FixedPoint fixedPoint21 = 0L;
									fixedPoint21 = ((!(actorModel.Level - guardianActor.Level > value34)) ? ((FixedPoint)Math.Pow((double)(1L - value32), (double)((actorModel.Level - guardianActor.Level - value30) / value31))) : ((FixedPoint)Math.Pow((double)(1L - value32), (double)((value34 - value30) / value31))));
									if (fixedPoint21 > 50L)
									{
										fixedPoint21 = 50L;
									}
									if (fixedPoint4 > 0L)
									{
										fixedPoint = ((!(damageWithoutCrit * fixedPoint21 / damageWithoutCrit2 < value33)) ? (damageWithoutCrit * fixedPoint21) : (damageWithoutCrit2 * value33));
										FixedPoint fixedPoint22 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint22;
										damageWithoutCrit2 = fixedPoint;
										fixedPoint4 -= fixedPoint22;
									}
									if (fixedPoint5 > 0L)
									{
										fixedPoint = ((!(additionalCritDamage * fixedPoint21 / additionalCritDamage2 < value33)) ? (additionalCritDamage * fixedPoint21) : (additionalCritDamage2 * value33));
										FixedPoint fixedPoint23 = damageWithoutCrit - fixedPoint;
										fixedPoint2 += fixedPoint23;
										additionalCritDamage2 = fixedPoint;
										fixedPoint5 -= fixedPoint23;
									}
								}
							}
						}
					}
					return new int[4]
					{
						(int)damageWithoutCrit,
						(int)additionalCritDamage,
						(int)damageWithoutCrit2,
						(int)additionalCritDamage2
					};
				}
				return new int[4]
				{
					(int)damageWithoutCrit,
					(int)additionalCritDamage,
					0,
					0
				};
			}
			return new int[4]
			{
				(int)damageWithoutCrit,
				(int)additionalCritDamage,
				0,
				0
			};
		}

		public static int[] GuardDamageCalculation(CombatModel combatModel, PreDealDamageAction preDealDamageAction, ActorModel target, ActorModel GuardActor, DamageType type, FixedPoint damageWithoutCrit, FixedPoint additionalCritDamage)
		{
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			_ = combatModel.manager.GameEconomyData;
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("SupportTalent_GuardParm2", ref value, GuardActor);
			if (!(value <= 0.0))
			{
				FixedPoint damageWithoutCrit2 = damageWithoutCrit * value;
				FixedPoint additionalCritDamage2 = additionalCritDamage * value;
				damageWithoutCrit -= damageWithoutCrit2;
				additionalCritDamage -= additionalCritDamage2;
				FixedPoint fixedPoint = 100.0;
				FixedPoint fixedPoint2 = 0.0;
				_ = (FixedPoint)0.0;
				FixedPoint fixedPoint3 = 0.0;
				FixedPoint successProbabilityExtension = 0.0;
				if (type != DamageType.Heal)
				{
					StrengthenDefense(GuardActor, combatModel, ref damageWithoutCrit2, ref additionalCritDamage2);
					FixedPoint fixedPoint4 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * damageWithoutCrit2;
					FixedPoint fixedPoint5 = combatModel.manager.GameEconomyData.ConfigData.MaximumDamageReduction / 100.0 * (fixedPoint / 100.0) * additionalCritDamage2;
					FixedPoint value2 = 0.0;
					abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistance", ref value2, GuardActor);
					abilityManager.VisitParameter("SupportTalent_GuardParm3", ref value2, GuardActor);
					switch (type)
					{
					case DamageType.Melee:
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMelee", ref value2, GuardActor);
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceMeleeArmor", ref value2, GuardActor);
						break;
					case DamageType.Ranged:
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceRanged", ref value2, GuardActor);
						break;
					}
					value2 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value2);
					if (fixedPoint4 > 0.0)
					{
						fixedPoint2 = FixedPoint.Min(damageWithoutCrit2 * value2, fixedPoint4);
						fixedPoint3 += fixedPoint2;
						damageWithoutCrit2 -= fixedPoint2;
						fixedPoint4 -= fixedPoint2;
					}
					if (fixedPoint5 > 0.0)
					{
						fixedPoint2 = FixedPoint.Min(additionalCritDamage2 * value2, fixedPoint5);
						fixedPoint3 += fixedPoint2;
						additionalCritDamage2 -= fixedPoint2;
						fixedPoint5 -= fixedPoint2;
					}
					FixedPoint value3 = 0.0;
					if (GuardActor.HadActionPointsAtEndOfTurn || GuardActor.OverwatchedOnTurn)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseResistanceOverwatch", ref value3, GuardActor);
						abilityManager.VisitParameter("AbilityModifierPercentageIncreaseNewResistanceOverwatch", ref value3, GuardActor);
						value3 = Math.Max((float)combatModel.manager.GameEconomyData.ConfigData.MinArmorReductionPercentage / 100f, (float)value3);
						if (fixedPoint4 > 0.0)
						{
							fixedPoint2 = FixedPoint.Min(damageWithoutCrit2 * value3, fixedPoint4);
							fixedPoint3 += fixedPoint2;
							damageWithoutCrit2 -= fixedPoint2;
							fixedPoint4 -= fixedPoint2;
						}
						if (fixedPoint5 > 0.0)
						{
							fixedPoint2 = FixedPoint.Min(additionalCritDamage2 * value3, fixedPoint5);
							fixedPoint3 += fixedPoint2;
							additionalCritDamage2 -= fixedPoint2;
							fixedPoint5 -= fixedPoint2;
						}
					}
					if (GuardActor.HasAnyLevelTrait("LeaderBuffBodyguard") || combatModel.IsTargetNextToActorWithTrait(GuardActor, "LeaderBuffBodyguard"))
					{
						FixedPoint value4 = 0.0;
						abilityManager.VisitParameter("AbilityModifierIncreaseChanceForBodyguard", ref value4, GuardActor);
						PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.Generic, value4, successProbabilityExtension);
						FixedPoint value5 = 0.0;
						abilityManager.VisitParameter("AbilityModifierIncreaseDamageReductionIfJerryOrNextToJerry", ref value5, GuardActor);
						if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
						{
							if (fixedPoint4 > 0.0)
							{
								fixedPoint2 = FixedPoint.Min(damageWithoutCrit2 * value5, fixedPoint4);
								fixedPoint3 += fixedPoint2;
								damageWithoutCrit2 -= fixedPoint2;
								fixedPoint4 -= fixedPoint2;
							}
							if (fixedPoint5 > 0.0)
							{
								fixedPoint2 = FixedPoint.Min(additionalCritDamage2 * value5, fixedPoint5);
								fixedPoint3 += fixedPoint2;
								additionalCritDamage2 -= fixedPoint2;
								fixedPoint5 -= fixedPoint2;
							}
							GuardActor.NotifyChange("AbilityVisited", new object[2]
							{
								"LeaderBuffBodyguard",
								playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
							});
						}
					}
					if ((type == DamageType.Base || type == DamageType.Melee || type == DamageType.Ranged || type == DamageType.Struggle) && GuardActor.IsHuman)
					{
						if (GuardActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Scout"))
						{
							FixedPoint value6 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutAttackedByHighLevel", ref value6, GuardActor);
							if (preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value6)
							{
								FixedPoint value7 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutLevelDifference", ref value7, GuardActor);
								FixedPoint value8 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutDamageReduction", ref value8, GuardActor);
								FixedPoint value9 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaximumLiftingValue", ref value9, GuardActor);
								FixedPoint value10 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFScoutMaxLeveLimitValue", ref value10, target);
								FixedPoint fixedPoint6 = 0L;
								fixedPoint6 = ((!(preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value10)) ? ((FixedPoint)Math.Pow((double)(1L - value8), (double)((preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level - value6) / value7))) : ((FixedPoint)Math.Pow((double)(1L - value8), (double)((value10 - value6) / value7))));
								if (fixedPoint6 > 50L)
								{
									fixedPoint6 = 50L;
								}
								if (fixedPoint4 > 0L)
								{
									fixedPoint2 = ((!(damageWithoutCrit * fixedPoint6 / damageWithoutCrit2 < value9)) ? (damageWithoutCrit * fixedPoint6) : (damageWithoutCrit2 * value9));
									FixedPoint fixedPoint7 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint7;
									damageWithoutCrit2 = fixedPoint2;
									fixedPoint4 -= fixedPoint7;
								}
								if (fixedPoint5 > 0L)
								{
									fixedPoint2 = ((!(additionalCritDamage * fixedPoint6 / additionalCritDamage2 < value9)) ? (additionalCritDamage * fixedPoint6) : (additionalCritDamage2 * value9));
									FixedPoint fixedPoint8 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint8;
									additionalCritDamage2 = fixedPoint2;
									fixedPoint5 -= fixedPoint8;
								}
							}
						}
						if (GuardActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Bruiser"))
						{
							FixedPoint value11 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserAttackedByHighLevel", ref value11, GuardActor);
							if (preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value11)
							{
								FixedPoint value12 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserLevelDifference", ref value12, GuardActor);
								FixedPoint value13 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserDamageReduction", ref value13, GuardActor);
								FixedPoint value14 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaximumLiftingValue", ref value14, GuardActor);
								FixedPoint value15 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFBruiserMaxLeveLimitValue", ref value15, target);
								FixedPoint fixedPoint9 = 0L;
								fixedPoint9 = ((!(preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value15)) ? ((FixedPoint)Math.Pow((double)(1L - value13), (double)((preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level - value11) / value12))) : ((FixedPoint)Math.Pow((double)(1L - value13), (double)((value15 - value11) / value12))));
								if (fixedPoint9 > 50L)
								{
									fixedPoint9 = 50L;
								}
								if (fixedPoint4 > 0L)
								{
									fixedPoint2 = ((!(damageWithoutCrit * fixedPoint9 / damageWithoutCrit2 < value14)) ? (damageWithoutCrit * fixedPoint9) : (damageWithoutCrit2 * value14));
									FixedPoint fixedPoint10 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint10;
									damageWithoutCrit2 = fixedPoint2;
									fixedPoint4 -= fixedPoint10;
								}
								if (fixedPoint5 > 0L)
								{
									fixedPoint2 = ((!(additionalCritDamage * fixedPoint9 / additionalCritDamage2 < value14)) ? (additionalCritDamage * fixedPoint9) : (additionalCritDamage2 * value14));
									FixedPoint fixedPoint11 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint11;
									additionalCritDamage2 = fixedPoint2;
									fixedPoint5 -= fixedPoint11;
								}
							}
						}
						if (GuardActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Warrior"))
						{
							FixedPoint value16 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorAttackedByHighLevel", ref value16, GuardActor);
							if (preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value16)
							{
								FixedPoint value17 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorLevelDifference", ref value17, GuardActor);
								FixedPoint value18 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorDamageReduction", ref value18, GuardActor);
								FixedPoint value19 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaximumLiftingValue", ref value19, GuardActor);
								FixedPoint value20 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFWarriorMaxLeveLimitValue", ref value20, target);
								FixedPoint fixedPoint12 = 0L;
								fixedPoint12 = ((!(preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value20)) ? ((FixedPoint)Math.Pow((double)(1L - value18), (double)((preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level - value16) / value17))) : ((FixedPoint)Math.Pow((double)(1L - value18), (double)((value20 - value16) / value17))));
								if (fixedPoint12 > 50L)
								{
									fixedPoint12 = 50L;
								}
								if (fixedPoint4 > 0L)
								{
									fixedPoint2 = ((!(damageWithoutCrit * fixedPoint12 / damageWithoutCrit2 < value19)) ? (damageWithoutCrit * fixedPoint12) : (damageWithoutCrit2 * value19));
									FixedPoint fixedPoint13 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint13;
									damageWithoutCrit2 = fixedPoint2;
									fixedPoint4 -= fixedPoint13;
								}
								if (fixedPoint5 > 0L)
								{
									fixedPoint2 = ((!(additionalCritDamage * fixedPoint12 / additionalCritDamage2 < value19)) ? (additionalCritDamage * fixedPoint12) : (additionalCritDamage2 * value19));
									FixedPoint fixedPoint14 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint14;
									additionalCritDamage2 = fixedPoint2;
									fixedPoint5 -= fixedPoint14;
								}
							}
						}
						if (GuardActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Shooter"))
						{
							FixedPoint value21 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterAttackedByHighLevel", ref value21, GuardActor);
							if (preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value21)
							{
								FixedPoint value22 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterLevelDifference", ref value22, GuardActor);
								FixedPoint value23 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterDamageReduction", ref value23, GuardActor);
								FixedPoint value24 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaximumLiftingValue", ref value24, GuardActor);
								FixedPoint value25 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFShooterMaxLeveLimitValue", ref value25, target);
								FixedPoint fixedPoint15 = 0L;
								fixedPoint15 = ((!(preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value25)) ? ((FixedPoint)Math.Pow((double)(1L - value23), (double)((preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level - value21) / value22))) : ((FixedPoint)Math.Pow((double)(1L - value23), (double)((value25 - value21) / value22))));
								if (fixedPoint15 > 50L)
								{
									fixedPoint15 = 50L;
								}
								if (fixedPoint4 > 0L)
								{
									fixedPoint2 = ((!(damageWithoutCrit * fixedPoint15 / damageWithoutCrit2 < value24)) ? (damageWithoutCrit * fixedPoint15) : (damageWithoutCrit2 * value24));
									FixedPoint fixedPoint16 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint16;
									damageWithoutCrit2 = fixedPoint2;
									fixedPoint4 -= fixedPoint16;
								}
								if (fixedPoint5 > 0L)
								{
									fixedPoint2 = ((!(additionalCritDamage * fixedPoint15 / additionalCritDamage2 < value24)) ? (additionalCritDamage * fixedPoint15) : (additionalCritDamage2 * value24));
									FixedPoint fixedPoint17 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint17;
									additionalCritDamage2 = fixedPoint2;
									fixedPoint5 -= fixedPoint17;
								}
							}
						}
						if (GuardActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Hunter"))
						{
							FixedPoint value26 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterAttackedByHighLevel", ref value26, GuardActor);
							if (preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value26)
							{
								FixedPoint value27 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterLevelDifference", ref value27, GuardActor);
								FixedPoint value28 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterDamageReduction", ref value28, GuardActor);
								FixedPoint value29 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaximumLiftingValue", ref value29, GuardActor);
								FixedPoint value30 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFHunterMaxLeveLimitValue", ref value30, target);
								FixedPoint fixedPoint18 = 0L;
								fixedPoint18 = ((!(preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value30)) ? ((FixedPoint)Math.Pow((double)(1L - value28), (double)((preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level - value26) / value27))) : ((FixedPoint)Math.Pow((double)(1L - value28), (double)((value30 - value26) / value27))));
								if (fixedPoint18 > 50L)
								{
									fixedPoint18 = 50L;
								}
								if (fixedPoint4 > 0L)
								{
									fixedPoint2 = ((!(damageWithoutCrit * fixedPoint18 / damageWithoutCrit2 < value29)) ? (damageWithoutCrit * fixedPoint18) : (damageWithoutCrit2 * value29));
									FixedPoint fixedPoint19 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint19;
									damageWithoutCrit2 = fixedPoint2;
									fixedPoint4 -= fixedPoint19;
								}
								if (fixedPoint5 > 0L)
								{
									fixedPoint2 = ((!(additionalCritDamage * fixedPoint18 / additionalCritDamage2 < value29)) ? (additionalCritDamage * fixedPoint18) : (additionalCritDamage2 * value29));
									FixedPoint fixedPoint20 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint20;
									additionalCritDamage2 = fixedPoint2;
									fixedPoint5 -= fixedPoint20;
								}
							}
						}
						if (GuardActor.HasAnyLevelTrait("Equipment_Apocalyptic_DEF_Assault"))
						{
							FixedPoint value31 = 0.0;
							combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultAttackedByHighLevel", ref value31, GuardActor);
							if (preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value31)
							{
								FixedPoint value32 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultLevelDifference", ref value32, GuardActor);
								FixedPoint value33 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultDamageReduction", ref value33, GuardActor);
								FixedPoint value34 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaximumLiftingValue", ref value34, GuardActor);
								FixedPoint value35 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierDEFAssaultMaxLeveLimitValue", ref value35, target);
								FixedPoint fixedPoint21 = 0L;
								fixedPoint21 = ((!(preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level > value35)) ? ((FixedPoint)Math.Pow((double)(1L - value33), (double)((preDealDamageAction.DamageAction.DamagerActor.Level - GuardActor.Level - value31) / value32))) : ((FixedPoint)Math.Pow((double)(1L - value33), (double)((value35 - value31) / value32))));
								if (fixedPoint21 > 50L)
								{
									fixedPoint21 = 50L;
								}
								if (fixedPoint4 > 0L)
								{
									fixedPoint2 = ((!(damageWithoutCrit * fixedPoint21 / damageWithoutCrit2 < value34)) ? (damageWithoutCrit * fixedPoint21) : (damageWithoutCrit2 * value34));
									FixedPoint fixedPoint22 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint22;
									damageWithoutCrit2 = fixedPoint2;
									fixedPoint4 -= fixedPoint22;
								}
								if (fixedPoint5 > 0L)
								{
									fixedPoint2 = ((!(additionalCritDamage * fixedPoint21 / additionalCritDamage2 < value34)) ? (additionalCritDamage * fixedPoint21) : (additionalCritDamage2 * value34));
									FixedPoint fixedPoint23 = damageWithoutCrit - fixedPoint2;
									fixedPoint3 += fixedPoint23;
									additionalCritDamage2 = fixedPoint2;
									fixedPoint5 -= fixedPoint23;
								}
							}
						}
					}
				}
				return new int[4]
				{
					(int)damageWithoutCrit,
					(int)additionalCritDamage,
					(int)damageWithoutCrit2,
					(int)additionalCritDamage2
				};
			}
			return new int[4]
			{
				(int)damageWithoutCrit,
				(int)additionalCritDamage,
				0,
				0
			};
		}

		public static void CheckForHeirlooms_RiotGearGlenn_Fetter(CombatModel combatModel, ActorModel source, ActorModel target, bool isChargeAttack)
		{
			if (source == null || target == null || source.IsDead || target.IsDead || !source.HasAnyLevelTrait("Heirlooms_RiotGearGlenn_Fetter") || !target.IsBurning)
			{
				return;
			}
			FixedPoint value = 0.0;
			if (isChargeAttack)
			{
				combatModel.AbilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_ChargeChance", ref value, source);
			}
			else
			{
				combatModel.AbilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_AtkChance", ref value, source);
			}
			if (source.manager.Player.RollDice(RollDiceType.Heirlooms_RiotGearGlenn_Fetter, value) != PlayerRandomChanceResult.Failed)
			{
				FixedPoint value2 = 0.0;
				if (isChargeAttack)
				{
					combatModel.AbilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_ChargeTimes", ref value2, source);
				}
				else
				{
					combatModel.AbilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_AtkTimes", ref value2, source);
				}
				target.NotifyChange("FlameTrigger");
				for (int i = 0; i < (int)value2; i++)
				{
					target.DealBurningDamage();
				}
				value = 0.0;
				if (isChargeAttack)
				{
					combatModel.AbilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_ChargeChanceStun", ref value, source);
				}
				else
				{
					combatModel.AbilityManager.VisitParameter("Heirlooms_RiotGearGlenn_Fetter_AtkChanceStun", ref value, source);
				}
				if (source.manager.Player.RollDice(RollDiceType.Heirlooms_RiotGearGlenn_Fetter, value) != PlayerRandomChanceResult.Failed)
				{
					target.Stun(1, source);
				}
			}
		}

		public static void CheckForEquipment_Passive_Detonation(CombatModel combatModel, ActorModel source, ActorModel target, bool isChargeAttack)
		{
			if (source == null || target == null || source.IsDead || target.IsDead)
			{
				return;
			}
			bool flag = source.HasAnyLevelTrait("Equipment_Passive_Detonation");
			if (flag && target.IsBurning)
			{
				FixedPoint value = 0.0;
				if (flag)
				{
					combatModel.AbilityManager.VisitParameter("Equipment_Passive_DetonationProbility", ref value, source);
				}
				FixedPoint value2 = 0.0;
				source.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, source);
				if (source.manager.Player.RollDice(RollDiceType.Heirlooms_RiotGearGlenn_Fetter, value, value2) != PlayerRandomChanceResult.Failed)
				{
					target.DealBurningDamage();
					target.NotifyChange("FlameTrigger");
				}
			}
		}

		public static void RangeAttackAddChargePointFromRemoteWeaken(CombatModel combatModel, ActorModel source, List<ActorModel> TargetCandidates)
		{
			ActorModel actorModel = null;
			foreach (ActorModel TargetCandidate in TargetCandidates)
			{
				if (TargetCandidate.IsRemoteWeakened)
				{
					actorModel = TargetCandidate;
					break;
				}
			}
			if (actorModel != null && !(actorModel.DebuffRemoteRepulseWeakenAddChargePointPercentage <= 0.0))
			{
				FixedPoint value = 0.0;
				combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, source);
				if (combatModel.manager.Player.RollDice(RollDiceType.RemoteRepulseAddChargePoint, actorModel.DebuffRemoteRepulseWeakenAddChargePointPercentage, value) != PlayerRandomChanceResult.Failed && actorModel.DebuffRemoteRepulseWeakenAddChargePoints > 0)
				{
					source.AddChargePoints(actorModel.DebuffRemoteRepulseWeakenAddChargePoints);
				}
			}
		}

		public static List<ActorModel> GetFlagTeamByDistance(ActorModel target, int distance)
		{
			if (target == null || target.manager == null)
			{
				return null;
			}
			CombatModel combatModel = target.manager.CombatModel;
			if (combatModel == null)
			{
				return null;
			}
			combatModel = target.manager.CombatModel;
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(combatModel.GetFactionActors(target.Faction));
			list.Remove(target);
			return list.FindAll((ActorModel x) => x.GridCoordinate.ChebyshevDistance(target.GridCoordinate) - 1 < distance);
		}

		public static ActorModel GetLeaderBuffDeadlyFocusMan(CombatModel combatModel, Faction faction)
		{
			return combatModel.GetFactionActors(faction).Find((ActorModel t) => t.HasAnyLevelTrait("LeaderBuffDeadlyFocus"));
		}

		public static int GetLeaderBuffDeadlyFocusLevel(CombatModel combatModel, Faction faction)
		{
			ActorModel leaderBuffDeadlyFocusMan = GetLeaderBuffDeadlyFocusMan(combatModel, faction);
			if (leaderBuffDeadlyFocusMan == null)
			{
				return -1;
			}
			return UpgradeTraitsData.GetTraitLevelIdentifier(leaderBuffDeadlyFocusMan.TraitContainer.GetTraitAnyLevel("LeaderBuffDeadlyFocus").TraitIdentifier);
		}

		private static FixedPoint GetLeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg(ActorModel source, Faction buffFaction)
		{
			if (source == null)
			{
				return 0.0;
			}
			if (source.Faction != buffFaction)
			{
				return 0.0;
			}
			CombatModel combatModel = source.manager.CombatModel;
			ActorModel leaderBuffDeadlyFocusMan = GetLeaderBuffDeadlyFocusMan(combatModel, buffFaction);
			if (leaderBuffDeadlyFocusMan == null)
			{
				return 0.0;
			}
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_LevelReq_ExDmgHitRate", ref value, leaderBuffDeadlyFocusMan);
			if (GetLeaderBuffDeadlyFocusLevel(combatModel, buffFaction) + 1 >= (int)value)
			{
				FixedPoint value2 = 0.0;
				combatModel.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_ExDmgHitRate_ExDmg", ref value2, leaderBuffDeadlyFocusMan);
				return value2;
			}
			return 0.0;
		}

		public static ActorModel GetLeaderBuffShadowedGuardMan(CombatModel combatModel, Faction faction)
		{
			return combatModel.GetFactionActors(faction).Find((ActorModel t) => t.HasAnyLevelTrait("LeaderBuffShadowedGuard"));
		}

		public static int GetLeaderBuffShadowedGuardLevel(CombatModel combatModel, Faction faction)
		{
			ActorModel leaderBuffShadowedGuardMan = GetLeaderBuffShadowedGuardMan(combatModel, faction);
			if (leaderBuffShadowedGuardMan == null)
			{
				return -1;
			}
			return UpgradeTraitsData.GetTraitLevelIdentifier(leaderBuffShadowedGuardMan.TraitContainer.GetTraitAnyLevel("LeaderBuffShadowedGuard").TraitIdentifier);
		}

		public static ActorModel GetLeaderBuffDeathsDoorMan(CombatModel combatModel, Faction faction)
		{
			return combatModel.GetFactionActors(faction).Find((ActorModel t) => t.HasAnyLevelTrait("LeaderBuffDeathsDoor"));
		}

		public static int GetLeaderBuffDeathsDoorLevel(CombatModel combatModel, Faction faction)
		{
			ActorModel leaderBuffDeathsDoorMan = GetLeaderBuffDeathsDoorMan(combatModel, faction);
			if (leaderBuffDeathsDoorMan == null)
			{
				return -1;
			}
			return UpgradeTraitsData.GetTraitLevelIdentifier(leaderBuffDeathsDoorMan.TraitContainer.GetTraitAnyLevel("LeaderBuffDeathsDoor").TraitIdentifier);
		}
	}
}
