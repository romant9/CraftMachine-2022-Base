namespace TWDModel
{
	public sealed class SkillShieldType1TimedEffect : CoexistTimedEffectAbstract
	{
		public int Shield { get; private set; }

		public SkillShieldType1TimedEffect()
		{
		}

		public SkillShieldType1TimedEffect(SkillShieldType1TimedEffect skillShieldType1TimedEffect)
			: base(skillShieldType1TimedEffect)
		{
			Shield = skillShieldType1TimedEffect.Shield;
		}

		public SkillShieldType1TimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, int shield)
			: base(CoexistTimedEffectType.SkillShieldType1, duration, counter, instigator, target)
		{
			Shield = shield;
		}

		public override void PostNewTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				base.Target?.NotifyChange("AbilityVisited", new object[2] { "SkillShieldType1", false });
				actorModel.ChangeShieldHitPoints(Shield);
			}
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is SkillShieldType1TimedEffect skillShieldType1TimedEffect && base.Target is ActorModel actorModel)
			{
				actorModel.ChangeShieldHitPoints(-Shield);
				base.Instigator = skillShieldType1TimedEffect.Instigator;
				base.InstigatorFaction = skillShieldType1TimedEffect.InstigatorFaction;
				base.Counter = skillShieldType1TimedEffect.Counter;
				base.Duration = skillShieldType1TimedEffect.Duration;
				Shield = skillShieldType1TimedEffect.Shield;
				actorModel.NotifyChange("AbilityVisited", new object[2] { "SkillShieldType1", false });
				actorModel.ChangeShieldHitPoints(Shield);
			}
		}

		public override void PostFinishTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				actorModel.ChangeShieldHitPoints(-Shield);
			}
		}
	}
}
