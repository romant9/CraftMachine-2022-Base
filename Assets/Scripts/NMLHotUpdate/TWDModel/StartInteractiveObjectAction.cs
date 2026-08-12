using BaseModel;

namespace TWDModel
{
	public class StartInteractiveObjectAction : ModelAction
	{
		public InteractiveObjectModel Target { get; private set; }

		public StartInteractiveObjectAction(ActorModel actor, InteractiveObjectModel target)
			: base(actor)
		{
			Target = target;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
