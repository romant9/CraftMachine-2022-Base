using BaseModel;

namespace TWDModel
{
	public class InteractiveObjectFinishedAction : ModelAction
	{
		public ActorModel SourceActor { get; set; }

		public InteractiveObjectFinishedAction(ActorModel actor)
			: base(actor)
		{
			SourceActor = actor;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
