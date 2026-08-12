using BaseModel;

namespace TWDModel
{
	public class DashAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public GridCoordinate TargetCoordinate { get; private set; }

		public GridCoordinate OriginalCoordinate { get; private set; }

		public AbilityModel Ability { get; private set; }

		public DashAction(ActorModel actor, GridCoordinate targetCoordinate, AbilityModel ability)
			: base(actor)
		{
			Actor = actor;
			OriginalCoordinate = actor.GridCoordinate;
			TargetCoordinate = targetCoordinate;
			Ability = ability;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null && Actor != null)
			{
				GridPath gridPath = GridPath.Create();
				gridPath.AddNode(OriginalCoordinate);
				gridPath.AddNode(TargetCoordinate);
				return combatModel.MoveActor(Actor, gridPath);
			}
			return false;
		}
	}
}
