namespace TWDModel
{
	public sealed class BerserkRageTimedEffect : CoexistTimedEffectAbstract
	{
		public int BaseRageLayer { get; private set; }

		public FixedPoint AdditionDamageMultiplier { get; private set; }

		public int Layer { get; private set; }

		public override bool TurnCheck => true;

		public BerserkRageTimedEffect()
		{
		}

		public BerserkRageTimedEffect(BerserkRageTimedEffect berserkRageTimedEffect)
			: base(berserkRageTimedEffect)
		{
			Layer = berserkRageTimedEffect.Layer;
			BaseRageLayer = berserkRageTimedEffect.BaseRageLayer;
			AdditionDamageMultiplier = berserkRageTimedEffect.AdditionDamageMultiplier;
		}

		public BerserkRageTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, int layer, int baseRageLayer, FixedPoint additionDamageMultiplier)
			: base(CoexistTimedEffectType.BerserkRage, duration, counter, instigator, target)
		{
			Layer = layer;
			BaseRageLayer = baseRageLayer;
			AdditionDamageMultiplier = additionDamageMultiplier;
		}

		public override void PostNewTimedEffect()
		{
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is BerserkRageTimedEffect berserkRageTimedEffect)
			{
				Layer = berserkRageTimedEffect.Layer;
			}
		}

		public override void PostFinishTimedEffect()
		{
			base.Target?.NotifyChange("ActorRageUpdateEvent");
		}
	}
}
