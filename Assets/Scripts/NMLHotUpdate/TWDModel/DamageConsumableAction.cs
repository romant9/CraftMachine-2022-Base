using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class DamageConsumableAction : DamageAction
	{
		public DamageConsumableAction(ActorModel target, ActorModel damager, int baseDamage, int additionalCriticalDamage, bool bodyShot, bool critical, PlayerRandomChanceResult probabilityOutcome, DamageType type, Faction originalTargetFaction = Faction.Any, Dictionary<ActorModel, List<DamageNotificationData>> damageVisualisations = null)
			: base(target, damager, baseDamage, additionalCriticalDamage, bodyShot, critical, probabilityOutcome, type, originalTargetFaction, damageVisualisations)
		{
		}

		protected override void ComputeChargePoint(TWDModelManager modelManager)
		{
			if (modelManager != null && base.TargetActor.IsDead && base.DamagerActor != null && base.DamagerActor.ChargeMeter != null && !base.TargetActor.Definition.IsEnvironmental)
			{
				int chargeLevel = base.DamagerActor.ChargeMeter.ChargeLevel;
				base.DamagerActor.AddChargePoints(base.DamagerActor.SelectedEquipment.Ability.Definition.ChargePointsPerKill);
				if (chargeLevel != base.DamagerActor.ChargeMeter.ChargeLevel)
				{
					base.GotChargePoint = true;
				}
			}
		}

		public override void DealDamage(ModelManager manager)
		{
			bool isDead = base.TargetActor.IsDead;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (base.TargetActor.ShieldHitPoints <= 0)
			{
				base.HealthAfterDamage = base.TargetActor.Hitpoints - base.FinalDamage;
			}
			else
			{
				base.HealthAfterDamage = base.TargetActor.Hitpoints;
			}
			base.LowestHpBeforeDmg = base.TargetActor.MinHitpoints;
			if (!base.SavedFromDeath)
			{
				DealDamageAfterPreHPDeduction(manager);
			}
			base.LowestHpAfterDmg = base.TargetActor.MinHitpoints;
			if (isDead)
			{
				return;
			}
			ComputeChargePoint(tWDModelManager);
			if (tWDModelManager.GameEconomyData.GetFeature("ChainsawThreatFix").Enabled && base.DamagerActor != null)
			{
				CombatModel combatModel = tWDModelManager.CombatModel;
				if (base.DealDamagePostAbility || base.IsFollowThrough)
				{
					bool receivedFireDamage = false;
					CombatHelpers.CheckForStruggle(tWDModelManager, base.DamagerActor, base.TargetActor, this, receivedFireDamage);
					base.TargetActor.NotifyChange("ActorHealthChanged");
				}
				else if (base.IsPushDamage)
				{
					base.TargetActor.NotifyChange("ActorHealthChanged");
				}
				if (combatModel.IsEndlessBattleMission)
				{
					combatModel.EndlessModeCombatModel.HandleKillScoreIncrease();
					combatModel.NotifyChange("EndlessModeScoreChanged");
				}
			}
			AddGuildBossDamageAndPoint(tWDModelManager);
		}
	}
}
