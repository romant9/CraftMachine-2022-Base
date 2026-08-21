using System.Collections.Generic;

namespace TWDModel
{
	public static class CombatTotalDamageBonusHelpers
	{
		public static FixedPoint CalculateTotalDamageBonus(CombatModel combatModel, ActorModel source, ActorModel target, DamageType type, PlayerRandomChanceResult criticalResult, AbilityModel ability = null, bool isMainTarget = false, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			FixedPoint totalDamageBonus = 0.0;
			AbilityManagerModel abilityManager = combatModel.AbilityManager;
			_ = combatModel.manager.GameEconomyData;
			if (source.HasTraitsThatContains("RangeArmorDominance") || source.HasTraitsThatContains("RangeEquipmentDominance") || source.HasTraitsThatContains("RangeActorDominance"))
			{
				totalDamageBonus = GrantsAnAdditionalTotalDamageBonus(source, target, abilityManager);
			}
			AddAdditionalTotalDamageBonus(source, target, abilityManager, ref totalDamageBonus, type, criticalResult, ability, isMainTarget, ootType, isAssistAttack, isTriggerExtraAttackDamage, combatModel);
			return totalDamageBonus;
		}

		private static FixedPoint GrantsAnAdditionalTotalDamageBonus(ActorModel source, ActorModel target, AbilityManagerModel abilityManager)
		{
			FixedPoint result = 0.0;
			FixedPoint fixedPoint = source.GridCoordinate.DistanceTo(target.GridCoordinate);
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("AbilityModifierArmorAttackingMoreNFrames", ref value, source);
			if (value != 0L && fixedPoint > value)
			{
				FixedPoint value2 = 0.0;
				abilityManager.VisitParameter("AbilityModifierArmorIncreaseInDamage", ref value2, source);
				FixedPoint value3 = 0.0;
				abilityManager.VisitParameter("AbilityModifierArmorIncreaseNFrame", ref value3, source);
				FixedPoint value4 = 0.0;
				abilityManager.VisitParameter("AbilityModifierArmorDamageBoost", ref value4, source);
				FixedPoint value5 = 0.0;
				abilityManager.VisitParameter("AbilityModifierArmorDamageBoostLimit", ref value5, source);
				FixedPoint a = FixedPoint.Ceiling((fixedPoint - value - value3) / value3) * value4 + value2;
				a = FixedPoint.Min(a, value5);
				result += a;
			}
			FixedPoint value6 = 0.0;
			abilityManager.VisitParameter("AbilityModifierEquipmentAttackingMoreNFrames", ref value6, source);
			if (value6 != 0L && fixedPoint > value6)
			{
				FixedPoint value7 = 0.0;
				abilityManager.VisitParameter("AbilityModifierEquipmentIncreaseInDamage", ref value7, source);
				FixedPoint value8 = 0.0;
				abilityManager.VisitParameter("AbilityModifierEquipmentIncreaseNFrame", ref value8, source);
				FixedPoint value9 = 0.0;
				abilityManager.VisitParameter("AbilityModifierEquipmentDamageBoost", ref value9, source);
				FixedPoint value10 = 0.0;
				abilityManager.VisitParameter("AbilityModifierEquipmentDamageBoostLimit", ref value10, source);
				FixedPoint a2 = FixedPoint.Ceiling((fixedPoint - value6 - value8) / value8 * value9) + value7;
				a2 = FixedPoint.Min(a2, value10);
				result += a2;
			}
			FixedPoint value11 = 0.0;
			abilityManager.VisitParameter("AbilityModifierActorAttackingMoreNFrames", ref value11, source);
			if (value11 != 0L && fixedPoint > value11)
			{
				FixedPoint value12 = 0.0;
				abilityManager.VisitParameter("AbilityModifierActorIncreaseInDamage", ref value12, source);
				FixedPoint value13 = 0.0;
				abilityManager.VisitParameter("AbilityModifierActorIncreaseNFrame", ref value13, source);
				FixedPoint value14 = 0.0;
				abilityManager.VisitParameter("AbilityModifierActorDamageBoost", ref value14, source);
				FixedPoint value15 = 0.0;
				abilityManager.VisitParameter("AbilityModifierActorDamageBoostLimit", ref value15, source);
				FixedPoint a3 = FixedPoint.Ceiling((fixedPoint - value11 - value13) / value13 * value14) + value12;
				a3 = FixedPoint.Min(a3, value15);
				result += a3;
			}
			return result;
		}

		private static void AddAdditionalTotalDamageBonus(ActorModel source, ActorModel target, AbilityManagerModel abilityManager, ref FixedPoint totalDamageBonus, DamageType type, PlayerRandomChanceResult criticalResult, AbilityModel ability = null, bool isMainTarget = false, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false, CombatModel combatModel = null)
		{
			if (ootType == OOTType.ParryRiposteRetaliation && source.ParryRiposteIncreaseStorey > 0)
			{
				abilityManager.VisitParameter("AbilityModifierRippedAdditionalPRIncreaseDmg", ref totalDamageBonus, source);
			}
			if (source.HasTrait("Equipment_Passive_ScoutDMGBoost") && target.Definition.Class == SurvivorClass.Scout.ToString())
			{
				abilityManager.VisitParameter("Equipment_Passive_ScoutDMGBoost_Dmg", ref totalDamageBonus, source);
			}
			if (source.HasTrait("Equipment_Passive_BruiserDMGBoost") && target.Definition.Class == SurvivorClass.Bruiser.ToString())
			{
				abilityManager.VisitParameter("Equipment_Passive_BruiserDMGBoost_Dmg", ref totalDamageBonus, source);
			}
			if (source.HasTrait("Equipment_Passive_WarriorDMGBoost") && target.Definition.Class == SurvivorClass.Warrior.ToString())
			{
				abilityManager.VisitParameter("Equipment_Passive_WarriorDMGBoost_Dmg", ref totalDamageBonus, source);
			}
			if (source.HasTrait("Equipment_Passive_ShooterDMGBoost") && target.Definition.Class == SurvivorClass.Shooter.ToString())
			{
				abilityManager.VisitParameter("Equipment_Passive_ShooterDMGBoost_Dmg", ref totalDamageBonus, source);
			}
			if (source.HasTrait("Equipment_Passive_HunterDMGBoost") && target.Definition.Class == SurvivorClass.Hunter.ToString())
			{
				abilityManager.VisitParameter("Equipment_Passive_HunterDMGBoost_Dmg", ref totalDamageBonus, source);
			}
			if (source.HasTrait("Equipment_Passive_AssaultDMGBoost") && target.Definition.Class == SurvivorClass.Assault.ToString())
			{
				abilityManager.VisitParameter("Equipment_Passive_AssaultDMGBoost_Dmg", ref totalDamageBonus, source);
			}
			if (source.HasAnyLevelTrait("Equipment_Passive_Fortuna_Spade"))
			{
				abilityManager.VisitParameter("AbilityModifierEquipmentPassiveFortunaSpade", ref totalDamageBonus, source);
			}
			if (source.HasTraitsThatContains("Riposte"))
			{
				MomentumTimedEffect momentumTimedEffect = source.MomentumTimedEffect;
				if (momentumTimedEffect != null)
				{
					totalDamageBonus += momentumTimedEffect.AddDamagePercentageBase * momentumTimedEffect.CurrentLayer;
				}
			}
			if (target.TornApartMarkCount > 0L)
			{
				if (source.HasTraitsThatContains("Equipment_Passive_SawAxe"))
				{
					FixedPoint value = 0L;
					abilityManager.VisitParameter("Equipment_Passive_SawAxe_ExtraDmgCount", ref value, source);
					if (target.TornApartMarkCount >= value)
					{
						FixedPoint value2 = 0.0;
						FixedPoint value3 = 0.0;
						FixedPoint value4 = 0.0;
						FixedPoint value5 = 0.0;
						abilityManager.VisitParameter("ExtendProbability", ref value2, source);
						abilityManager.VisitParameter("Equipment_Passive_SawAxe_ExtraDmgChance", ref value3, source);
						abilityManager.VisitParameter("Equipment_Passive_SawAxe_ExtraDmgMultiplier", ref value4, source);
						abilityManager.VisitParameter("Equipment_Passive_SawAxe_MaxExtraDmgMultiplier", ref value5, source);
						if (source.manager.Player.RollDice(RollDiceType.SawAxe, value3, value2) != PlayerRandomChanceResult.Failed)
						{
							if (value4 >= value5)
							{
								value4 = value5;
							}
							totalDamageBonus += value4;
						}
					}
				}
				FixedPoint value6 = 0.0;
				FixedPoint value7 = 0.0;
				abilityManager.VisitParameter("Equipment_Passive_TornDamageMultiplier", ref value6, source);
				abilityManager.VisitParameter("Equipment_Passive_TornExtraDamageMultiplier", ref value7, source);
				totalDamageBonus += value6 + (target.TornApartMarkCount - 1L) * value7;
			}
			if (source.HasTraitsThatContains("Equipment_Passive_HPPercentDmg"))
			{
				FixedPoint fixedPoint = FixedPoint.Min((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints, 1.0);
				FixedPoint fixedPoint2 = FixedPoint.Min((FixedPoint)source.Hitpoints / (FixedPoint)source.MaxHitPoints, 1.0);
				if (fixedPoint >= fixedPoint2)
				{
					FixedPoint value8 = 0.0;
					abilityManager.VisitParameter("Equipment_Passive_HPPercentDmg_Per", ref value8, source);
					totalDamageBonus += value8;
				}
			}
			if (source.HasTraitsThatContains("Equipment_Active_BloodFrenzy"))
			{
				FixedPoint value9 = 0.0;
				abilityManager.VisitParameter("Equipment_Active_BloodFrenzy_Dmg", ref value9, source);
				totalDamageBonus += value9;
				source.bloodFrenzyFlag = true;
				source.NotifyChange("bloodFrenzyFlagUpdate");
			}
			if (source.IsAttackChainStatus)
			{
				if (criticalResult != PlayerRandomChanceResult.Failed)
				{
					totalDamageBonus += source.AttackChainStaus.UpCriticalDamagePercentage;
				}
				if ((target.Faction == Faction.Walker && !target.IsNormalWalker) || target.Faction == Faction.Raider || target.Faction == Faction.Survivor)
				{
					totalDamageBonus += source.AttackChainStaus.UpSpecialActorDamagePercentage;
				}
			}
			if (source.HasTraitsThatContains("GodWarBless"))
			{
				FixedPoint value10 = 0.0;
				abilityManager.VisitParameter("GodWarBless_DmgPercentage", ref value10, source);
				totalDamageBonus += value10;
			}
			TraitEntry traitAnyLevel = source.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_C");
			if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) > 0)
			{
				FixedPoint value11 = 0L;
				abilityManager.VisitParameter("SurvivalManualIncreaseDmg", ref value11, source);
				if (CombatHelpers.IsWithinRange(combatModel, 3, source.GridCoordinate, target.GridCoordinate))
				{
					totalDamageBonus += value11;
					if (value11 != 0L)
					{
						source.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_C", false });
					}
				}
			}
			if (source.HasTraitsThatContains("SurvivalManualKillIncreaseDmgTrait"))
			{
				abilityManager.VisitParameter("SurvivalManualCurKillIncreaseDmg", ref totalDamageBonus, source);
			}
			if (source.HasAnyLevelTrait("SupportTalent_BodyshootDmg") && CombatHelpers.IsBodyShot(combatModel, source, target, ability, isTriggerExtraAttackDamage) != PlayerRandomChanceResult.Failed)
			{
				FixedPoint value12 = 0L;
				abilityManager.VisitParameter("SupportTalent_BodyshootDmgParm1", ref value12, source);
				totalDamageBonus += value12;
			}
			if (target.HasAnyLevelTrait("SupportTalent_RefBodyshootDmg") && CombatHelpers.IsBodyShot(combatModel, source, target, ability, isTriggerExtraAttackDamage) != PlayerRandomChanceResult.Failed)
			{
				FixedPoint value13 = 0L;
				abilityManager.VisitParameter("SupportTalent_BodyshootDmgRefParm1", ref value13, target);
				totalDamageBonus -= value13;
				totalDamageBonus = ((totalDamageBonus >= 0L) ? totalDamageBonus : ((FixedPoint)0L));
			}
			TraitEntry traitAnyLevel2 = source.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_F");
			if (traitAnyLevel2 != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel2.TraitIdentifier) > 0)
			{
				FixedPoint value14 = 0.0;
				abilityManager.VisitParameter("SurvivalManualStorySkill_FParm3", ref value14, source);
				value14 += value14 * (source.SharpBladeLayers - 1);
				totalDamageBonus += value14;
				if (value14 != 0L)
				{
					source.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_F", false });
				}
			}
			if (source.HasTraitsThatContains("FollowAttackWithSplashDamage") && isTriggerExtraAttackDamage)
			{
				FixedPoint value15 = 0.0;
				combatModel.AbilityManager.VisitParameter("FollowAttackWithSplashDamageParam1", ref value15, source);
				totalDamageBonus += value15;
			}
			if (target != null && source.HasTraitsThatContains("Blind"))
			{
				FixedPoint successProbability = 0L;
				if (target.Faction == Faction.Walker || target.Faction == Faction.Raider)
				{
					IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(source.manager);
					if (challengeDebuffProvider != null)
					{
						List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
						if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.WalkerStateRefBlind) != null)
						{
							successProbability = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.WalkerStateRefBlind);
							successProbability *= (FixedPoint)0.01;
						}
					}
				}
				FixedPoint value16 = 0.0;
				source.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value16, source);
				if (source.manager.Player.RollDice(RollDiceType.FistSpike, successProbability, value16) == PlayerRandomChanceResult.Failed)
				{
					FixedPoint value17 = 0.0;
					FixedPoint value18 = 0.0;
					combatModel.AbilityManager.VisitParameter("BlindParam1", ref value17, source);
					combatModel.AbilityManager.VisitParameter("BlindParam2", ref value18, source);
					target.BlindDecreaseRate = value17;
					target.BlindLeftTurns = (int)value18;
					target.NotifyChange("Blind");
					target.AddTemporaryTrait("ModifierBlinTrait", default(FixedPoint), null, 0L);
				}
			}
			if (target.ShieldHitPoints > 0)
			{
				FixedPoint value19 = 0.0;
				abilityManager.VisitParameter("AbilityModifierShieldBreakerStrikeType1Parameter1", ref value19, source);
				if (value19 > 0.0)
				{
					totalDamageBonus += value19;
				}
			}
			if (source.PastaTurns > 0)
			{
				SupportModel supportModel = source.manager.Player.GetSupportModel("Pasta");
				if (supportModel != null && supportModel.Unlocked && ability != null)
				{
					if (ability.IsChargeAttack)
					{
						totalDamageBonus += supportModel.GetParameter(1) * 0.009999999776482582;
					}
					else
					{
						totalDamageBonus += supportModel.GetParameter(0) * 0.009999999776482582;
					}
				}
			}
			if (ability != null && ability.IsChargeAttack && source.BaseRage > 0 && source.BerserkRageTimedEffect != null && source.BaseRage >= source.BerserkRageTimedEffect.BaseRageLayer)
			{
				totalDamageBonus += source.BerserkRageTimedEffect.AdditionDamageMultiplier;
			}
			if (source != null && source.HasAnyLevelTrait("Equipment_Passive_Rage"))
			{
				FixedPoint value20 = 0.0;
				if (abilityManager != null && abilityManager.VisitParameter("Equipment_Passive_RageParam3", ref value20, source))
				{
					totalDamageBonus += source.TotalRage * value20;
				}
				if (target != null && target.MaxHitPoints > 0)
				{
					FixedPoint enemyHealthPercent = ((target.Hitpoints > 0) ? ((FixedPoint)target.Hitpoints / (FixedPoint)target.MaxHitPoints) : ((FixedPoint)0L));
					FixedPoint bloodthirstExtraDamage = source.GetBloodthirstExtraDamage(enemyHealthPercent);
					if (bloodthirstExtraDamage > 0L)
					{
						totalDamageBonus += bloodthirstExtraDamage;
					}
				}
				if (source != null && source.IsInChargeConvertState())
				{
					totalDamageBonus += source.ChargeConvertDamageBonus;
				}
			}
			if (source != null && source.HasAnyLevelTrait("Equipment.VengefulCharge"))
			{
				FixedPoint value21 = 0.0;
				if (abilityManager != null && abilityManager.VisitParameter("Equipment_VengefulCharge_PerMarkDmg", ref value21, source))
				{
					totalDamageBonus += source.TotalVengefulChargeNums * value21;
				}
				source.ClearTotalVengefulChargeNums();
			}
			if (source != null && source.HasAnyLevelTrait("Equipment.LastStand"))
			{
				FixedPoint obj = ((source.Hitpoints > 0) ? ((FixedPoint)source.Hitpoints / (FixedPoint)source.MaxHitPoints) : ((FixedPoint)0L));
				FixedPoint value22 = 0.0;
				abilityManager.VisitParameter("Equipment_LastStand_HPLowerMultiplier", ref value22, source);
				FixedPoint value23 = 0.0;
				abilityManager.VisitParameter("Equipment_LastStand_DmgMultiplier", ref value23, source);
				if (obj <= value22)
				{
					totalDamageBonus += value23;
				}
			}
			if (source != null && source.HasAnyLevelTrait("Equipment_Passive_ShotGun") && combatModel != null && ability != null)
			{
				FixedPoint value24 = 0.0;
				abilityManager.VisitParameter("Equipment_Passive_ShotGun_Param0", ref value24, source);
				int num = (int)value24;
				FixedPoint value25 = 0.0;
				abilityManager.VisitParameter("Equipment_Passive_ShotGun_Param1", ref value25, source);
				int num2 = (int)value25;
				int numberOfEnemiesAttacked = source.NumberOfEnemiesAttacked;
				FixedPoint value26 = 0.0;
				if (numberOfEnemiesAttacked <= num)
				{
					abilityManager.VisitParameter("Equipment_Passive_ShotGun_Param2", ref value26, source);
				}
				else if (numberOfEnemiesAttacked < num2)
				{
					abilityManager.VisitParameter("Equipment_Passive_ShotGun_Param3", ref value26, source);
				}
				else
				{
					abilityManager.VisitParameter("Equipment_Passive_ShotGun_Param4", ref value26, source);
				}
				totalDamageBonus += value26;
			}
			if (source != null && target != null && source.HasAnyLevelTrait("Equipment_Active_HealthRealdmg") && combatModel != null && ability != null)
			{
				FixedPoint value27 = 0.0;
				abilityManager.VisitParameter("AbilityModifierHealthRealdmg", ref value27, source);
				totalDamageBonus += value27;
			}
		}
	}
}
