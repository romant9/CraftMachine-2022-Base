using System;

namespace TWDModel
{
	public abstract class StatusEffectAction : ModelAction
	{
		private Func<int> damageDealt;

		public ActorModel SourceActor { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public bool Avoided { get; set; }

		public SupportModel SourceSupport { get; private set; }

		public int DamageDealt => damageDealt?.Invoke() ?? 0;

		protected StatusEffectAction(ActorModel source, ActorModel target, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(source)
		{
			SourceActor = source;
			TargetActor = target;
			SourceSupport = sourceSupport;
			damageDealt = damage;
		}
	}
}
