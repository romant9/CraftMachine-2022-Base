using BaseModel;

namespace TWDModel
{
	public class PreAttackAction : ModelAction
	{
		public ActorModel TargetActor { get; private set; }

		public bool EffectiveAttack { get; private set; }

		public ActorModel DamagerActor { get; private set; }

		public bool Interrupted { get; set; }

		public PreAttackAction(ActorModel target, ActorModel damager, bool effectiveAttack = false)
			: base(target)
		{
			TargetActor = target;
			DamagerActor = damager;
			EffectiveAttack = effectiveAttack;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "DamagerActor = " + ((DamagerActor != null) ? DamagerActor.DebugInfo : "null") + ", TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null");
		}
	}
}
