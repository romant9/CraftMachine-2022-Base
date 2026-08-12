using BaseModel;

namespace TWDModel
{
	public class MultipleTargetsAttackAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public GridCoordinate TargetCoordinate { get; private set; }

		public GridCoordinate OriginalCoordinate { get; private set; }

		public AbilityModel Ability { get; private set; }

		public MultipleTargetsAttackAction(ActorModel actor, GridCoordinate targetCoordinate, AbilityModel ability)
			: base(actor)
		{
			Actor = actor;
			OriginalCoordinate = actor.GridCoordinate;
			TargetCoordinate = targetCoordinate;
			Ability = ability;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
