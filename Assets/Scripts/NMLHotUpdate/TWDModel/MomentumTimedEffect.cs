using System;

namespace TWDModel
{
	public sealed class MomentumTimedEffect : CoexistTimedEffectAbstract
	{
		public int AddLayer { get; private set; }

		public FixedPoint AddDamagePercentageBase { get; private set; }

		public FixedPoint ReduceEnemyDodgePercentageBase { get; private set; }

		public FixedPoint ReduceEnemyDamageReductionBase { get; private set; }

		public int MaxLayer { get; private set; }

		public int CurrentLayer { get; private set; }

		public override bool TurnCheck => false;

		public MomentumTimedEffect()
		{
		}

		public MomentumTimedEffect(MomentumTimedEffect momentumTimedEffect)
			: base(momentumTimedEffect)
		{
			AddLayer = momentumTimedEffect.AddLayer;
			AddDamagePercentageBase = momentumTimedEffect.AddDamagePercentageBase;
			ReduceEnemyDodgePercentageBase = momentumTimedEffect.ReduceEnemyDodgePercentageBase;
			ReduceEnemyDamageReductionBase = momentumTimedEffect.ReduceEnemyDamageReductionBase;
			MaxLayer = momentumTimedEffect.MaxLayer;
			CurrentLayer = momentumTimedEffect.CurrentLayer;
		}

		public MomentumTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, int addLayer, FixedPoint addDamagePercentageBase, FixedPoint reduceEnemyDodgePercentageBase, FixedPoint reduceEnemyDamageReductionBase, int maxLayer)
			: base(CoexistTimedEffectType.Momentum, duration, counter, instigator, target)
		{
			AddLayer = addLayer;
			AddDamagePercentageBase = addDamagePercentageBase;
			ReduceEnemyDodgePercentageBase = reduceEnemyDodgePercentageBase;
			ReduceEnemyDamageReductionBase = reduceEnemyDamageReductionBase;
			MaxLayer = maxLayer;
			CurrentLayer = Math.Min(AddLayer, MaxLayer);
		}

		public override void PostNewTimedEffect()
		{
			base.Target?.NotifyChange("AbilityVisited", new object[2] { "Momentum", false });
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is MomentumTimedEffect momentumTimedEffect && CurrentLayer < MaxLayer)
			{
				CurrentLayer = Math.Min(CurrentLayer + momentumTimedEffect.AddLayer, MaxLayer);
			}
		}

		public override void PostFinishTimedEffect()
		{
			base.Target?.NotifyChange("ActorMomentumUpdate");
		}
	}
}
