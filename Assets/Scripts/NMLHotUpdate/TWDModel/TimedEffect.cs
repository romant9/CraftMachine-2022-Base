using BaseModel;

namespace TWDModel
{
	public class TimedEffect : TWDModelObject
	{
		public TimedEffectType Type { get; set; }

		public virtual TimedEffectExistType ExistType => TimedEffectExistType.Replace;

		public int Duration { get; set; }

		public int Counter { get; set; }

		public Faction InstigatorFaction { get; set; }

		[IgnoreModelProperty]
		public TWDModelObject Target { get; set; }

		[IgnoreModelProperty]
		public ActorModel Instigator { get; set; }

		public GridCoordinate TargetCoordinate { get; set; }

		public TimedEffect()
		{
		}

		public TimedEffect(TimedEffectType type, int duration, int counter, ActorModel instigator, TWDModelObject target)
			: this(type, duration, counter, instigator, target, GridCoordinate.Invalid)
		{
		}

		public TimedEffect(TimedEffectType type, int duration, int counter, ActorModel instigator, TWDModelObject target, GridCoordinate targetCoordinate)
		{
			Target = target;
			Type = type;
			Duration = duration;
			Counter = counter;
			Instigator = instigator;
			InstigatorFaction = Instigator.Faction;
			TargetCoordinate = targetCoordinate;
		}

		public TimedEffect(TimedEffectType type, int duration, int counter, Faction faction)
		{
			Type = type;
			Duration = duration;
			Counter = counter;
			Instigator = null;
			Target = null;
			InstigatorFaction = faction;
			TargetCoordinate = GridCoordinate.Invalid;
		}

		public TimedEffect(TimedEffectType type, int duration, int counter, ActorModel instigator)
		{
			Type = type;
			Duration = duration;
			Counter = counter;
			Instigator = instigator;
			Target = null;
			InstigatorFaction = Instigator.Faction;
			TargetCoordinate = GridCoordinate.Invalid;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
