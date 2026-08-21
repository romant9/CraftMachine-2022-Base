using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class DamageAction : ModelAction
	{
		public Dictionary<ActorModel, List<DamageNotificationData>> DamageRelatedVisualisations { get; private set; }

		public bool IsMainTarget { get; private set; }

		public bool IsTriggerExtraAttackDamage { get; private set; }

		public bool IsChargeAttack { get; private set; }

		public bool IsDealShield { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public ActorModel DamagerActor { get; private set; }

		public SupportModel SourceSupport { get; }

		public ActorModel OriginalDamageInstigator { get; private set; }

		public int BaseDamage { get; private set; }

		public int AdditionalCriticalDamage { get; private set; }

		public int FinalDamage { get; protected set; }

		public int HealthAfterDamage { get; protected set; }

		public bool NoChargeGain { get; set; }

		public bool IsEquipmentKaboomReflect { get; set; }

		public int LowestHpBeforeDmg { get; protected set; }

		public int LowestHpAfterDmg { get; protected set; }

		public Faction OriginalTargetFaction { get; private set; }

		public bool Critical { get; private set; }

		public bool BodyShot { get; private set; }

		public bool Dodged { get; set; }

		public bool Jumpingshot { get; set; }

		public bool IsFullyAvoided
		{
			get
			{
				if (!Jumpingshot)
				{
					if (Dodged)
					{
						return !Critical;
					}
					return false;
				}
				return true;
			}
		}

		public bool ShouldApplyHitEffects
		{
			get
			{
				if (!Jumpingshot)
				{
					if (Dodged)
					{
						if (Dodged)
						{
							return Critical;
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public bool DodgedShot { get; set; }

		public bool TargetGotChargePoint { get; set; }

		public bool SavedFromDeath { get; set; }

		public bool IgnoreIndicatorUpdate { get; set; }

		public PlayerRandomChanceResult ProbabilityOutcome { get; set; }

		public DamageType DamageType { get; set; }

		public bool DealDamagePostAbility { get; set; }

		public bool GotChargePoint { get; protected set; }

		public bool IsFollowThrough { get; set; }

		public bool IsPushDamage { get; set; }

		public bool DamageIgnored { get; set; }

		public int ModifyDamage { get; set; }

		public DamageAction(ActorModel target, ActorModel damager, int baseDamage, int additionalCriticalDamage, bool bodyShot, bool critical, PlayerRandomChanceResult probabilityOutcome, DamageType type, Faction originalTargetFaction = Faction.Any, Dictionary<ActorModel, List<DamageNotificationData>> damageVisualisations = null, bool noChargeGain = false, SupportModel sourceSupport = null, ActorModel originalDamageInstigator = null, bool isMainTarget = false, bool isTriggerExtraAttackDamage = false, bool isChargeAttack = false, bool isDealShield = true)
			: base(target)
		{
			TargetActor = target;
			DamagerActor = damager;
			BaseDamage = baseDamage;
			AdditionalCriticalDamage = additionalCriticalDamage;
			Critical = critical;
			BodyShot = bodyShot;
			Dodged = false;
			TargetGotChargePoint = false;
			ProbabilityOutcome = probabilityOutcome;
			DamageType = type;
			GotChargePoint = false;
			NoChargeGain = noChargeGain;
			SourceSupport = sourceSupport;
			OriginalDamageInstigator = originalDamageInstigator;
			IsMainTarget = isMainTarget;
			IsTriggerExtraAttackDamage = isTriggerExtraAttackDamage;
			IsChargeAttack = isChargeAttack;
			IsDealShield = isDealShield;
			if (originalTargetFaction != Faction.Any)
			{
				OriginalTargetFaction = originalTargetFaction;
			}
			else
			{
				OriginalTargetFaction = TargetActor.Faction;
			}
			DamageRelatedVisualisations = damageVisualisations;
		}

		public bool UpBaseDamage(int newDamage)
		{
			BaseDamage = newDamage;
			return true;
		}

		public bool UpAdditionalCriticalDamage(int newAdditionalCriticalDamage)
		{
			AdditionalCriticalDamage = newAdditionalCriticalDamage;
			return true;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null && TargetActor != null)
			{
				TWDModelManager tWDModelManager = combatModel.Manager as TWDModelManager;
				tWDModelManager?.ExecuteAction(new PreDealDamageAction(this));
				CalculateFinalDamage();
				if (!DealDamagePostAbility)
				{
					DealDamage(manager);
					if (DamagerActor != null && DamagerActor is SurvivorModel && TargetActor.IsWalker)
					{
						TargetActor.DamageCount++;
					}
					if (DamagerActor != null && DamagerActor is SurvivorModel && TargetActor.IsRaider)
					{
						TargetActor.DamageCount++;
					}
				}
				if (DamagerActor != null)
				{
					combatModel.ActorAttacked(DamagerActor, TargetActor);
				}
				tWDModelManager?.ExecuteAction(new DamageActionExecuteFinishedAction(this, TargetActor));
				return true;
			}
			return false;
		}

		public override void PostAbilityExecute(ModelManager manager)
		{
			if (DealDamagePostAbility)
			{
				DealDamage(manager);
			}
		}

		protected void DealDamageAfterPreHPDeduction(ModelManager manager)
		{
			PreHPDeductionAction preHPDeductionAction = new PreHPDeductionAction(TargetActor, DamagerActor, FinalDamage, DamageType);
			(manager as TWDModelManager)?.ExecuteAction(preHPDeductionAction);
			if (preHPDeductionAction.Avoided)
			{
				FinalDamage = 0;
				HealthAfterDamage = TargetActor.Hitpoints;
			}
			else
			{
				FinalDamage = Math.Max(0, preHPDeductionAction.Damage);
				HealthAfterDamage = ((TargetActor.ShieldHitPoints <= 0) ? (TargetActor.Hitpoints - FinalDamage) : TargetActor.Hitpoints);
				TargetActor.DealDamage(FinalDamage, DamagerActor, DamageType, OriginalDamageInstigator, preHPDeductionResolved: true);
			}
		}

		public virtual void DealDamage(ModelManager manager)
		{
			bool isDead = TargetActor.IsDead;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (!isDead)
			{
				ComputePrincessChargePoint(tWDModelManager);
			}
			if (TargetActor.ShieldHitPoints <= 0)
			{
				HealthAfterDamage = TargetActor.Hitpoints - FinalDamage;
			}
			else
			{
				HealthAfterDamage = TargetActor.Hitpoints;
			}
			LowestHpBeforeDmg = TargetActor.MinHitpoints;
			if (!SavedFromDeath)
			{
				DealDamageAfterPreHPDeduction(manager);
			}
			LowestHpAfterDmg = TargetActor.MinHitpoints;
			if (isDead)
			{
				return;
			}
			ComputeChargePoint(tWDModelManager);
			if (tWDModelManager.GameEconomyData.GetFeature("ChainsawThreatFix").Enabled && DamagerActor != null)
			{
				if (DealDamagePostAbility || IsFollowThrough)
				{
					bool receivedFireDamage = false;
					CombatHelpers.CheckForBurningEffects(tWDModelManager, DamagerActor, TargetActor, this, ref receivedFireDamage);
					CombatHelpers.CheckForStruggle(tWDModelManager, DamagerActor, TargetActor, this, receivedFireDamage);
					TargetActor.NotifyChange("ActorHealthChanged");
				}
				else if (IsPushDamage)
				{
					TargetActor.NotifyChange("ActorHealthChanged");
				}
				if (SourceSupport == null)
				{
					CombatHelpers.CalculateThreatReduction(tWDModelManager.CombatModel, DamagerActor, TargetActor, DamagerActor.SelectedEquipment.Ability);
				}
			}
			if (TargetActor.IsDead && !TargetActor.Definition.IsEnvironmental && DamagerActor != null && SourceSupport == null)
			{
				DamagerActor.AttackKilledAnyEnemy = true;
				CheckForDamageIncrease(tWDModelManager, DamagerActor);
				CheckForSurvivalManualDamageIncrease(tWDModelManager, DamagerActor);
				CheckForExtraChargePointChanceIncrease(tWDModelManager, DamagerActor);
				DamagerActor.OnEnemyKilledForRage();
			}
			else if (DamagerActor != null)
			{
				DamagerActor.AttackHasNotKilledAllEnemies = true;
			}
			if (IsChargeAttack && DamagerActor != null)
			{
				DamagerActor.OnChargedAttackCompletedForRage();
			}
			AddGuildBossDamageAndPoint(tWDModelManager);
		}

		protected void AddGuildBossDamageAndPoint(TWDModelManager modelManager)
		{
			if (modelManager == null || !(TargetActor is TankActorModel))
			{
				return;
			}
			modelManager.CombatModel.AddGuildBossDamage(FinalDamage);
			List<FixedPoint> guildBossPointFunctionValue = modelManager.GameEconomyData.ConfigData.GuildBossPointFunctionValue;
			if (guildBossPointFunctionValue == null || guildBossPointFunctionValue.Count < 6)
			{
				return;
			}
			double num = FinalDamage;
			double num2 = 0.0;
			double num3 = (double)guildBossPointFunctionValue[0];
			double num4 = (double)guildBossPointFunctionValue[1];
			double num5 = (double)guildBossPointFunctionValue[2];
			double num6 = (double)guildBossPointFunctionValue[3];
			double num7 = (double)guildBossPointFunctionValue[4];
			double num8 = (double)guildBossPointFunctionValue[5];
			double num9 = (double)guildBossPointFunctionValue[6];
			if (!(num3 <= 0.0) && !(num4 <= num3) && !(num9 <= 0.0) && !(num9 >= 1.0))
			{
				num2 = ((num <= 0.0) ? 0.0 : ((num <= num3) ? (num5 + (num6 - num5) * (num / num3)) : ((!(num <= num4)) ? (num8 - (num8 - num7) * Math.Exp((0.0 - num9) * (num - num4) / num4)) : (num6 + (num7 - num6) * (Math.Log(num / num3) / Math.Log(num4 / num3))))));
				if (modelManager.Player?.GetAttackTargetMissionModel() is WorldBossModelManager worldBossModelManager)
				{
					double myTowerBBossScoreMultiplier = worldBossModelManager.GetMyTowerBBossScoreMultiplier();
					num2 *= 1.0 + myTowerBBossScoreMultiplier / 100.0;
				}
				modelManager.CombatModel.AddGuildBossPoint(num2);
			}
		}

		public void CalculateFinalDamage()
		{
			if (TargetActor == null)
			{
				return;
			}
			bool flag = TargetActor.Faction == Faction.Civilian && TargetActor.CivilianCanStruggle;
			bool flag2 = TargetActor.Faction == Faction.Survivor;
			bool flag3 = TargetActor.Faction == Faction.Raider;
			bool flag4 = TargetActor is TankActorModel;
			if (Jumpingshot || DodgedShot)
			{
				FinalDamage = 0;
			}
			else if (!Dodged)
			{
				if (TargetActor.IsHuman && TargetActor.StrugglesLeft > 0 && !flag4 && (flag || flag2 || flag3))
				{
					FinalDamage = ((BaseDamage + AdditionalCriticalDamage >= TargetActor.Hitpoints) ? (TargetActor.Hitpoints - 1) : (BaseDamage + AdditionalCriticalDamage));
				}
				else
				{
					FinalDamage = BaseDamage + AdditionalCriticalDamage;
				}
				FinalDamage += ModifyDamage;
			}
			else if (Dodged && Critical)
			{
				if (TargetActor.IsHuman && TargetActor.StrugglesLeft > 0 && !flag4 && (flag || flag2 || flag3))
				{
					FinalDamage = ((BaseDamage >= TargetActor.Hitpoints) ? (TargetActor.Hitpoints - 1) : BaseDamage);
				}
				else
				{
					FinalDamage = BaseDamage;
				}
				FinalDamage += ModifyDamage;
			}
			else
			{
				FinalDamage = 0;
			}
		}

		public bool DoesAttackKillHumanSurvivor()
		{
			CalculateFinalDamage();
			if (TargetActor.IsHuman && TargetActor.OnRedHealthBar)
			{
				return TargetActor.Hitpoints - FinalDamage <= 0;
			}
			return false;
		}

		public void ZeroDamage()
		{
			BaseDamage = 0;
			AdditionalCriticalDamage = 0;
		}

		private void ComputePrincessChargePoint(TWDModelManager modelManager)
		{
			if (NoChargeGain || modelManager == null || !TargetActor.HasPrincessStatusEffect || DamagerActor?.ChargeMeter == null || TargetActor.Definition.IsEnvironmental || IsPushDamage)
			{
				return;
			}
			int num = 0;
			FixedPoint value = 0.0;
			modelManager.Player.AbilityManager.VisitParameter("LeaderBuffPrincess.ExtraChargePoints", ref value, DamagerActor);
			FixedPoint value2 = 0.0;
			if (value != 0.0)
			{
				modelManager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value2, DamagerActor);
			}
			PlayerRandomChanceResult playerRandomChanceResult = modelManager.Player.RollDice(RollDiceType.GainChargePoint, value, value2);
			if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
			{
				DamagerActor.NotifyChange("AbilityVisited", new object[2]
				{
					"LeaderBuffPrincess",
					playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
				});
				num++;
			}
			if (num != 0)
			{
				int chargeLevel = DamagerActor.ChargeMeter.ChargeLevel;
				DamagerActor.AddChargePoints(num);
				if (chargeLevel != DamagerActor.ChargeMeter.ChargeLevel)
				{
					GotChargePoint = true;
				}
			}
		}

		protected virtual void ComputeChargePoint(TWDModelManager modelManager)
		{
			if (NoChargeGain)
			{
				return;
			}
			ActorModel actorModel = ((OriginalDamageInstigator == null) ? DamagerActor : OriginalDamageInstigator);
			if (modelManager == null || !TargetActor.IsDead || actorModel == null || actorModel.ChargeMeter == null || actorModel.Definition.IsEnvironmental)
			{
				return;
			}
			int num = 0;
			FixedPoint value = 0.0;
			modelManager.Player.AbilityManager.VisitParameter("AbilityModifierIncreaseExtraChargePointChance", ref value, actorModel);
			modelManager.Player.AbilityManager.VisitParameter("AbilityModifierLeaderBuffInspireExtraChargePointChance", ref value, actorModel);
			FixedPoint value2 = 0.0;
			if (value != 0.0)
			{
				modelManager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value2, actorModel);
			}
			PlayerRandomChanceResult playerRandomChanceResult = modelManager.Player.RollDice(RollDiceType.GainChargePoint, value, value2);
			if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
			{
				num++;
				if (actorModel.HasAnyLevelTrait("LeaderBuffInspire") && playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension)
				{
					actorModel.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffInspire", true });
				}
			}
			int chargeLevel = actorModel.ChargeMeter.ChargeLevel;
			actorModel.AddChargePoints(actorModel.SelectedEquipment.Ability.Definition.ChargePointsPerKill + num);
			if (chargeLevel != actorModel.ChargeMeter.ChargeLevel)
			{
				GotChargePoint = true;
			}
		}

		public void CheckForDamageIncrease(TWDModelManager manager, ActorModel actor)
		{
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			AbilityManagerModel abilityManager = manager.Player.AbilityManager;
			if (abilityManager.VisitParameter("LeaderBuffInspireDamageIncreasePerKillPercentage", ref value, actor))
			{
				abilityManager.VisitParameter("LeaderBuffInspireMaxDamageIncreasePerKillPercentage", ref value2, actor);
				manager.Player.AbilityManager.VisitParameter("AbilityModifierLeaderBuffInspireDamageIncrease", ref value3, actor);
				if ((int)FixedPoint.Ceiling(value2 * 100.0) > (int)FixedPoint.Ceiling(value3 * 100.0))
				{
					FixedPoint multiplier = (value3 + value - 1.0) * 100.0;
					actor.AddTemporaryTrait("InspirePerKillIncreaseDamageModifierTrait", multiplier, null, 0L);
				}
			}
		}

		public void CheckForSurvivalManualDamageIncrease(TWDModelManager manager, ActorModel actor)
		{
			if (actor != null)
			{
				TraitEntry traitAnyLevel = actor.TraitContainer.GetTraitAnyLevel("SurvivalManualStorySkill_D");
				if (traitAnyLevel != null && UpgradeTraitsData.GetTraitLevelIdentifier(traitAnyLevel.TraitIdentifier) <= 0)
				{
					return;
				}
			}
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			AbilityManagerModel abilityManager = manager.Player.AbilityManager;
			if (abilityManager.VisitParameter("SurvivalManualKillIncreaseDmg", ref value, actor))
			{
				abilityManager.VisitParameter("SurvivalManualKillMaxIncreaseDmg", ref value2, actor);
				manager.Player.AbilityManager.VisitParameter("SurvivalManualCurKillIncreaseDmg", ref value3, actor);
				if ((int)FixedPoint.Ceiling(value2 * 100.0) > (int)FixedPoint.Ceiling(value3 * 100.0))
				{
					FixedPoint multiplier = (value3 + value - 1.0) * 100.0;
					actor.AddTemporaryTrait("SurvivalManualKillIncreaseDmgTrait", multiplier, null, 0L);
				}
				if (value != 0L)
				{
					actor.NotifyChange("AbilityVisited", new object[2] { "SurvivalManualStorySkill_D", false });
					actor.NotifyChange("SurvivalManualStorySkill_D");
				}
			}
		}

		public void CheckForExtraChargePointChanceIncrease(TWDModelManager manager, ActorModel actor)
		{
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			FixedPoint value3 = 0.0;
			AbilityManagerModel abilityManager = manager.Player.AbilityManager;
			if (abilityManager.VisitParameter("LeaderBuffInspireIncreaseExtraChargePointChance", ref value, actor))
			{
				abilityManager.VisitParameter("LeaderBuffInspireMaxExtraChargePointChance", ref value2, actor);
				manager.Player.AbilityManager.VisitParameter("AbilityModifierLeaderBuffInspireExtraChargePointChance", ref value3, actor);
				if ((int)FixedPoint.Ceiling(value2 * 100.0) > (int)FixedPoint.Ceiling(value3 * 100.0))
				{
					FixedPoint multiplier = (value3 + value - 1.0) * 100.0;
					actor.AddTemporaryTrait("InspirePerKillIncreaseExtraChargePointChanceModifierTrait", multiplier, null, 0L);
				}
			}
		}

		public override string ToString()
		{
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("DamagerActor = " + ((DamagerActor != null) ? DamagerActor.DebugInfo : "null") + ", TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null") + ", DamageType = " + DamageType, ", TotalDamage = ", FinalDamage.ToString()), ", LowestHpPreDamage = ", LowestHpBeforeDmg.ToString()), ", LowestHpAfterDamage = ", LowestHpAfterDmg.ToString()), ", OriginalTargetFaction = ", OriginalTargetFaction.ToString()), ", Critical = ", Critical.ToString()), ", BodyShot = ", BodyShot.ToString()), ", ProbabilityOutcome = ", ProbabilityOutcome.ToString()), ", GotChargePoint = ", GotChargePoint.ToString());
		}
	}
}
