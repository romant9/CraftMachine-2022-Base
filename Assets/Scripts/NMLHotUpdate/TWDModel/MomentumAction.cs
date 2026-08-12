using System;
using BaseModel;

namespace TWDModel
{
	public class MomentumAction : StatusEffectAction
	{
		public int AddLayer { get; private set; }

		public FixedPoint AddDamagePercentageBase { get; private set; }

		public FixedPoint ReduceEnemyDodgePercentageBase { get; private set; }

		public FixedPoint ReduceEnemyDamageReductionBase { get; private set; }

		public int MaxLayer { get; private set; }

		public MomentumAction(ActorModel targetActor, int addLayer, FixedPoint addDamagePercentageBase, FixedPoint reduceEnemyDodgePercentageBase, FixedPoint reduceEnemyDamageReductionBase, int maxLayer, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(targetActor, targetActor, sourceSupport, damage)
		{
			AddLayer = addLayer;
			AddDamagePercentageBase = addDamagePercentageBase;
			ReduceEnemyDodgePercentageBase = reduceEnemyDodgePercentageBase;
			ReduceEnemyDamageReductionBase = reduceEnemyDamageReductionBase;
			MaxLayer = maxLayer;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.TargetActor != null && base.TargetActor.IsValid() && !base.TargetActor.IsDead)
			{
				base.TargetActor.StartMomentum(base.SourceActor, AddLayer, AddDamagePercentageBase, ReduceEnemyDodgePercentageBase, ReduceEnemyDamageReductionBase, MaxLayer);
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Riposte momentum action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided;
		}
	}
}
