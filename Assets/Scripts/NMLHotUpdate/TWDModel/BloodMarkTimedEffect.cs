namespace TWDModel
{
	public class BloodMarkTimedEffect : CoexistTimedEffectAbstract
	{
		public int MoveDistanceCap { get; private set; }

		public int DamageCount { get; private set; }

		public FixedPoint HealthPercentage { get; private set; }

		public FixedPoint Chance { get; private set; }

		public FixedPoint DamagePercentage { get; private set; }

		public int Range { get; private set; }

		public FixedPoint DamageLimit { get; private set; }

		public Faction MarkFaction { get; private set; }

		public int AccumulatedDamage { get; set; }

		public int LastSettledDamage { get; set; }

		public bool SkipAccumulateOnce { get; set; }

		public BloodMarkTimedEffect()
		{
		}

		public BloodMarkTimedEffect(BloodMarkTimedEffect other)
			: base(other)
		{
			MoveDistanceCap = other.MoveDistanceCap;
			DamageCount = other.DamageCount;
			HealthPercentage = other.HealthPercentage;
			Chance = other.Chance;
			DamagePercentage = other.DamagePercentage;
			Range = other.Range;
			DamageLimit = other.DamageLimit;
			MarkFaction = other.MarkFaction;
			AccumulatedDamage = other.AccumulatedDamage;
			LastSettledDamage = other.LastSettledDamage;
			SkipAccumulateOnce = other.SkipAccumulateOnce;
		}

		public BloodMarkTimedEffect(int duration, ActorModel instigator, ActorModel target, int moveDistanceCap, int damageCount, FixedPoint healthPercentage, FixedPoint chance, FixedPoint damagePercentage, int range, FixedPoint damageLimit)
			: base(CoexistTimedEffectType.BloodMark, duration, 0, instigator, target)
		{
			MoveDistanceCap = moveDistanceCap;
			DamageCount = damageCount;
			HealthPercentage = healthPercentage;
			Chance = chance;
			DamagePercentage = damagePercentage;
			Range = range;
			DamageLimit = damageLimit;
			MarkFaction = instigator?.Faction ?? Faction.Any;
		}

		public override void PostNewTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				actorModel.NotifyChange("AbilityVisited", new object[2] { "Equipment.Active.BloodMark", false });
				actorModel.NotifyChange("UpdateBloodMarkEvent");
			}
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is BloodMarkTimedEffect bloodMarkTimedEffect)
			{
				base.Duration = bloodMarkTimedEffect.Duration;
				base.Counter = 0;
				base.Instigator = bloodMarkTimedEffect.Instigator;
				base.InstigatorFaction = bloodMarkTimedEffect.InstigatorFaction;
				base.Target = bloodMarkTimedEffect.Target;
				MoveDistanceCap = bloodMarkTimedEffect.MoveDistanceCap;
				DamageCount = bloodMarkTimedEffect.DamageCount;
				HealthPercentage = bloodMarkTimedEffect.HealthPercentage;
				Chance = bloodMarkTimedEffect.Chance;
				DamagePercentage = bloodMarkTimedEffect.DamagePercentage;
				Range = bloodMarkTimedEffect.Range;
				DamageLimit = bloodMarkTimedEffect.DamageLimit;
				MarkFaction = bloodMarkTimedEffect.MarkFaction;
				SkipAccumulateOnce = bloodMarkTimedEffect.SkipAccumulateOnce;
				if (base.Target is ActorModel actorModel)
				{
					actorModel.NotifyChange("AbilityVisited", new object[2] { "Equipment.Active.BloodMark", false });
					actorModel.NotifyChange("UpdateBloodMarkEvent");
				}
			}
		}

		public override void PostFinishTimedEffect()
		{
			AccumulatedDamage = 0;
			LastSettledDamage = 0;
			SkipAccumulateOnce = false;
			if (base.Target is ActorModel actorModel)
			{
				actorModel.NotifyChange("UpdateBloodMarkEvent");
			}
		}
	}
}
