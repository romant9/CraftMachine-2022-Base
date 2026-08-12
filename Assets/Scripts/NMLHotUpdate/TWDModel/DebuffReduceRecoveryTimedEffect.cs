using System;

namespace TWDModel
{
	public class DebuffReduceRecoveryTimedEffect : CoexistTimedEffectAbstract
	{
		public int Param0 { get; private set; }

		public int Param1 { get; private set; }

		public int HealReduceAmount { get; private set; }

		public override bool TurnCheck => false;

		public DebuffReduceRecoveryTimedEffect()
		{
		}

		public DebuffReduceRecoveryTimedEffect(DebuffReduceRecoveryTimedEffect debuffReduceRecoveryTimedEffect)
			: base(debuffReduceRecoveryTimedEffect)
		{
			Param0 = debuffReduceRecoveryTimedEffect.Param0;
			Param1 = debuffReduceRecoveryTimedEffect.Param1;
			HealReduceAmount = debuffReduceRecoveryTimedEffect.HealReduceAmount;
		}

		public DebuffReduceRecoveryTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, int param0, int param1)
			: base(CoexistTimedEffectType.DebuffReduceRecovery, duration, counter, instigator, target)
		{
			Param0 = param0;
			Param1 = param1;
			HealReduceAmount = 0;
		}

		public override void PostNewTimedEffect()
		{
			HealReduceAmount = Math.Min(Param1, 100);
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
		}

		public override void PostFinishTimedEffect()
		{
		}
	}
}
