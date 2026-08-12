using BaseModel;

namespace TWDModel
{
	public class ActiveActorChangedAction : ModelAction
	{
		public ActorModel PreviousActor { get; private set; }

		public ActorModel NewActor { get; private set; }

		public ActiveActorChangedAction(ActorModel previousActor, ActorModel newActor)
			: base(null)
		{
			PreviousActor = previousActor;
			NewActor = newActor;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
