using BaseModel;

namespace TWDModel
{
	public class TransformActorAction : ModelAction
	{
		public ActorModel SourceActor { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public ActorModel Instigator { get; private set; }

		public TransformActorAction(ActorModel source, ActorModel target, ActorModel instigator)
			: base(target)
		{
			SourceActor = source;
			TargetActor = target;
			Instigator = instigator;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "TransformActorAction = " + ((SourceActor != null) ? SourceActor.DebugInfo : "null") + " -> " + ((TargetActor != null) ? TargetActor.DebugInfo : "null");
		}
	}
}
