namespace TWDModel
{
	public sealed class SkillIncreaseAttackTimedEffect : CoexistTimedEffectAbstract
	{
		public FixedPoint NormalAttackMultiplier { get; private set; }

		public FixedPoint ChargeAttackMultiplier { get; private set; }

		public SkillIncreaseAttackTimedEffect()
		{
		}

		public SkillIncreaseAttackTimedEffect(SkillIncreaseAttackTimedEffect skillIncreaseAttackTimedEffect)
			: base(skillIncreaseAttackTimedEffect)
		{
			NormalAttackMultiplier = skillIncreaseAttackTimedEffect.NormalAttackMultiplier;
			ChargeAttackMultiplier = skillIncreaseAttackTimedEffect.ChargeAttackMultiplier;
		}

		public SkillIncreaseAttackTimedEffect(FixedPoint normalAttackMultiplier, FixedPoint chargeAttackMultiplier, int duration, int counter, ActorModel instigator, ActorModel target)
			: base(CoexistTimedEffectType.SkillIncreaseAttack, duration, counter, instigator, target)
		{
			NormalAttackMultiplier = normalAttackMultiplier;
			ChargeAttackMultiplier = chargeAttackMultiplier;
		}

		public override void PostNewTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				actorModel.NotifyChange("AbilityVisited", new object[2] { "SkillIncreaseAttack", false });
				base.Target.NotifyChange("SkillIncreaseAttackChanged");
			}
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is SkillIncreaseAttackTimedEffect skillIncreaseAttackTimedEffect && base.Target is ActorModel actorModel)
			{
				base.Instigator = skillIncreaseAttackTimedEffect.Instigator;
				base.InstigatorFaction = skillIncreaseAttackTimedEffect.InstigatorFaction;
				base.Counter = skillIncreaseAttackTimedEffect.Counter;
				base.Duration = skillIncreaseAttackTimedEffect.Duration;
				NormalAttackMultiplier = skillIncreaseAttackTimedEffect.NormalAttackMultiplier;
				ChargeAttackMultiplier = skillIncreaseAttackTimedEffect.ChargeAttackMultiplier;
				actorModel.NotifyChange("AbilityVisited", new object[2] { "SkillIncreaseAttack", false });
				base.Target.NotifyChange("SkillIncreaseAttackChanged");
			}
		}

		public override void PostFinishTimedEffect()
		{
			base.Target.NotifyChange("SkillIncreaseAttackChanged");
		}
	}
}
