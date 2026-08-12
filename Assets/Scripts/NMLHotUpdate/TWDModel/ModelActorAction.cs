namespace TWDModel
{
	public class ModelActorAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public ModelActorAction(ActorModel actor)
			: base(actor)
		{
			Actor = actor;
		}

		public override bool CanExecute()
		{
			if (Actor != null)
			{
				return !Actor.IsDead;
			}
			return false;
		}

		public override string ToString()
		{
			return "Actor = " + ((Actor != null) ? Actor.DebugInfo : "null");
		}
	}
}
