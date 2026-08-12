namespace TWDModel
{
	public abstract class CoexistTimedEffectAbstract : TimedEffect
	{
		public CoexistTimedEffectType CoexistTimedEffectType { get; private set; }

		public sealed override TimedEffectExistType ExistType => TimedEffectExistType.Coexist;

		public virtual bool TurnCheck => true;

		public CoexistTimedEffectAbstract()
		{
		}

		public CoexistTimedEffectAbstract(CoexistTimedEffectAbstract coexistTimedEffectAbstract)
			: base(coexistTimedEffectAbstract.Type, coexistTimedEffectAbstract.Duration, coexistTimedEffectAbstract.Counter, coexistTimedEffectAbstract.Instigator, coexistTimedEffectAbstract.Target, coexistTimedEffectAbstract.TargetCoordinate)
		{
			CoexistTimedEffectType = coexistTimedEffectAbstract.CoexistTimedEffectType;
		}

		public CoexistTimedEffectAbstract(CoexistTimedEffectType coexistTimedEffectType, int duration, int counter, ActorModel instigator, ActorModel target)
			: base(TimedEffectType.None, duration, counter, instigator, target)
		{
			CoexistTimedEffectType = coexistTimedEffectType;
		}

		public abstract void PostNewTimedEffect();

		public abstract void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect);

		public abstract void PostFinishTimedEffect();

		public virtual void OnFactionChanged(Faction currentFaction, Faction newFaction)
		{
		}
	}
}
