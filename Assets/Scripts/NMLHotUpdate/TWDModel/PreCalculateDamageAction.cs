using BaseModel;

namespace TWDModel
{
	public class PreCalculateDamageAction : ModelAction
	{
		public ActorModel TargetActor { get; private set; }

		public ActorModel DamagerActor { get; private set; }

		public DamageType DamageType { get; private set; }

		public PreCalculateDamageAction(ActorModel target, ActorModel damager, DamageType damageType)
			: base(target)
		{
			TargetActor = target;
			DamagerActor = damager;
			DamageType = damageType;
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
