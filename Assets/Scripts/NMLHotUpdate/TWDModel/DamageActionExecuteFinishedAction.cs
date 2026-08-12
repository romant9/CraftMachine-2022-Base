using BaseModel;

namespace TWDModel
{
	public class DamageActionExecuteFinishedAction : ModelAction
	{
		public DamageAction DamageAction { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public DamageActionExecuteFinishedAction(DamageAction dmgAction, ActorModel target)
			: base(target)
		{
			DamageAction = dmgAction;
			TargetActor = target;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null");
		}
	}
}
