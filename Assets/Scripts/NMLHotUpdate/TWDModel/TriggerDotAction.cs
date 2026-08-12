using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class TriggerDotAction : ModelAction
	{
		public ActorModel SourceActor { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public int TriggerCount { get; private set; }

		public FixedPoint DamageBoostPercentage { get; private set; }

		public TriggerDotAction(ActorModel sourceActor, ActorModel targetActor, int triggerCount, FixedPoint damageBoostPercentage)
			: base(sourceActor)
		{
			SourceActor = sourceActor;
			TargetActor = targetActor;
			TriggerCount = triggerCount;
			DamageBoostPercentage = damageBoostPercentage;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			if (combatModel == null || TargetActor == null || TargetActor.IsDead)
			{
				return false;
			}
			for (int i = 0; i < TriggerCount; i++)
			{
				TriggerBurning(tWDModelManager, combatModel);
				TriggerBleeding();
				TriggerPoison(manager, combatModel);
				TriggerQuantun(tWDModelManager, combatModel);
			}
			SourceActor.NotifyChange("AbilityVisited", new object[2] { "AttackWithTriggerDot", false });
			return true;
		}

		private void TriggerBurning(TWDModelManager twdModelManager, CombatModel combatModel)
		{
			if (!TargetActor.IsDead && TargetActor.IsBurning)
			{
				TargetActor.DealBurningDamage(DamageBoostPercentage);
			}
		}

		private void TriggerBleeding()
		{
			if (!TargetActor.IsDead && TargetActor.IsBleeding)
			{
				TargetActor.DealBleedingDamage();
			}
		}

		private void TriggerPoison(ModelManager manager, CombatModel combatModel)
		{
			if (TargetActor.IsDead)
			{
				return;
			}
			PoisonRelationsManager model = combatModel.GetModel<PoisonRelationsManager>();
			if (model == null || model.ExistedPoisonRelations == null)
			{
				return;
			}
			List<PoisonRelation> list = new List<PoisonRelation>();
			foreach (PoisonRelation existedPoisonRelation in model.ExistedPoisonRelations)
			{
				if (existedPoisonRelation.TargetActor == TargetActor)
				{
					list.Add(existedPoisonRelation);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (PoisonRelation item in list)
			{
				if (item.SourceActor is SurvivorModel survivorModel)
				{
					FixedPoint fixedPoint = item.AttackerDamagePercentage * (1.0 + DamageBoostPercentage);
					FixedPoint fixedPoint2 = survivorModel.GetDamageForPreferredWeapon() * fixedPoint * item.CurrentLayerCount;
					CombatHelpers.ExecuteDamage(combatModel, null, TargetActor, (int)fixedPoint2, 0, DamageType.Poison, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				}
			}
		}

		private void TriggerQuantun(TWDModelManager twdModelManager, CombatModel combatModel)
		{
			if (TargetActor.IsDead || !TargetActor.IsQuantuned)
			{
				return;
			}
			QuantunTimedEffect quantunTimedEffect = TargetActor.CoexistTimedEffectsManager?.GetCoexistTimedEffect<QuantunTimedEffect>(CoexistTimedEffectType.Quantun);
			if (quantunTimedEffect != null)
			{
				int num = 0;
				MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(twdModelManager);
				if (mapMissionModel != null)
				{
					num = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(mapMissionModel.GetChallengeDebuffs(), ChallengeDebuffType.DebuffQuantunDmgReduction);
				}
				FixedPoint baseDamagePercentage = quantunTimedEffect.BaseDamagePercentage;
				FixedPoint additionalDamagePercentage = quantunTimedEffect.AdditionalDamagePercentage;
				int currentLayer = quantunTimedEffect.CurrentLayer;
				FixedPoint fixedPoint = (baseDamagePercentage + (currentLayer - 1) * additionalDamagePercentage) * (1.0 + DamageBoostPercentage);
				fixedPoint -= (FixedPoint)((float)num / 100f);
				fixedPoint = ((fixedPoint > 0L) ? fixedPoint : ((FixedPoint)0L));
				FixedPoint fixedPoint2 = TargetActor.MaxHitPoints * fixedPoint;
				CombatHelpers.ExecuteDamage(combatModel, null, TargetActor, (int)fixedPoint2, 0, DamageType.Qunantun, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
			}
		}

		public override string ToString()
		{
			return "SourceActor = " + ((SourceActor != null) ? SourceActor.DebugInfo : "null") + ", TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null") + ", TriggerCount = " + TriggerCount;
		}
	}
}
