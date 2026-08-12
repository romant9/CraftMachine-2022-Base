using BaseModel;

namespace TWDModel
{
	public class PostMoveSuccessAction : ModelActorAction
	{
		public MoveAction MoveAction { get; private set; }

		public PostMoveSuccessAction(ActorModel actor, MoveAction moveAction)
			: base(actor)
		{
			MoveAction = moveAction;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute() && !base.Actor.IsStruggling && !base.Actor.IsBleedingOut && !base.Actor.IsStunned && !base.Actor.IsElectricShocked && !base.Actor.IsEatingLure && !base.Actor.IsRooted)
			{
				return !base.Actor.IsABTesterAed;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
