using BaseModel;

namespace TWDModel
{
	public class PestilenceAction : ModelAction
	{
		public ActorModel Source { get; private set; }

		public ActorModel Target { get; private set; }

		public int ResetTurns { get; private set; }

		public bool IsMainTarget { get; private set; }

		public PestilenceAction(ActorModel source, ActorModel target, int resetTurns, bool isMainTarget = false)
			: base(source)
		{
			Source = source;
			Target = target;
			ResetTurns = resetTurns;
			IsMainTarget = isMainTarget;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + ", TargetActor = " + ((Target != null) ? Target.DebugInfo : "null") + $", ResetTurns = {ResetTurns}, IsMainTarget = {IsMainTarget}";
		}
	}
}
