namespace TWDModel
{
	public sealed class UnluckyTimedEffect : CoexistTimedEffectAbstract
	{
		public string TraitIdentifier { get; private set; }

		public UnluckyTimedEffect()
		{
		}

		public UnluckyTimedEffect(UnluckyTimedEffect unluckyTimedEffect)
			: base(unluckyTimedEffect)
		{
			TraitIdentifier = unluckyTimedEffect.TraitIdentifier;
		}

		public UnluckyTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, string traitIdentifier)
			: base(CoexistTimedEffectType.Unlucky, duration, counter, instigator, target)
		{
			TraitIdentifier = traitIdentifier;
		}

		public override void PostNewTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				actorModel.RemoveAnyLevelTrait(TraitIdentifier);
				actorModel.AddTemporaryTrait(TraitIdentifier, default(FixedPoint), null, 0L);
				actorModel.NotifyChange("AbilityVisited", new object[2] { "Unlucky", false });
				base.Target?.NotifyChange("ActorUnluckyUpdate");
			}
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is UnluckyTimedEffect && base.Target is ActorModel)
			{
				base.Instigator = newTimedEffect.Instigator;
				base.InstigatorFaction = newTimedEffect.InstigatorFaction;
				base.Counter = newTimedEffect.Counter;
				base.Duration = newTimedEffect.Duration;
				base.Target?.NotifyChange("ActorUnluckyUpdate");
			}
		}

		public override void PostFinishTimedEffect()
		{
			if (base.Target is ActorModel actorModel)
			{
				actorModel.RemoveTrait(TraitIdentifier);
				base.Target?.NotifyChange("ActorUnluckyUpdate");
			}
		}

		public override void OnFactionChanged(Faction currentFaction, Faction newFaction)
		{
		}
	}
}
