using BaseModel;

namespace TWDModel
{
	public class PostAbilityExecuteAction : ModelAction
	{
		public ActorModel TargetActor { get; private set; }

		public ActorModel DamagerActor { get; private set; }

		public PostAbilityExecuteAction(ActorModel target, ActorModel damager)
			: base(target)
		{
			TargetActor = target;
			DamagerActor = damager;
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
