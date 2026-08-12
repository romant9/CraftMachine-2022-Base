namespace TWDModel
{
	public sealed class SkillEquipTauntShieldTimedEffect : CoexistTimedEffectAbstract
	{
		public int Shield { get; private set; }

		public SkillEquipTauntShieldTimedEffect()
		{
		}

		public SkillEquipTauntShieldTimedEffect(SkillEquipTauntShieldTimedEffect skillEquipTauntShieldTimedEffect)
			: base(skillEquipTauntShieldTimedEffect)
		{
			Shield = skillEquipTauntShieldTimedEffect.Shield;
		}

		public SkillEquipTauntShieldTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, int shield)
			: base(CoexistTimedEffectType.SkillEquipTauntShield, duration, counter, instigator, target)
		{
			Shield = shield;
		}

		public override void PostNewTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				base.Target?.NotifyChange("AbilityVisited", new object[2] { "SkillEquipTauntShield", false });
				actorModel.ChangeShieldHitPoints(Shield);
			}
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is SkillEquipTauntShieldTimedEffect skillEquipTauntShieldTimedEffect && base.Target is ActorModel actorModel)
			{
				actorModel.ChangeShieldHitPoints(-Shield);
				base.Instigator = skillEquipTauntShieldTimedEffect.Instigator;
				base.InstigatorFaction = skillEquipTauntShieldTimedEffect.InstigatorFaction;
				base.Counter = skillEquipTauntShieldTimedEffect.Counter;
				base.Duration = skillEquipTauntShieldTimedEffect.Duration;
				Shield = skillEquipTauntShieldTimedEffect.Shield;
				actorModel.NotifyChange("AbilityVisited", new object[2] { "SkillEquipTauntShield", false });
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
