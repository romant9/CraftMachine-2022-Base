using BaseModel;

namespace TWDModel
{
	public class FireWeaponAction : ModelAction
	{
		public GridCoordinate SourceActorGridCoordinate { get; private set; }

		public GridCoordinate TargetGridCoordinate { get; private set; }

		public ActorModel SourceActor { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public AbilityModel WeaponAbility { get; private set; }

		public FireWeaponAction(ActorModel sourceActor, ActorModel targetActor, GridCoordinate targetGridCoordinate, AbilityModel weaponAbility)
			: base(sourceActor)
		{
			SourceActor = sourceActor;
			SourceActorGridCoordinate = sourceActor.GridCoordinate;
			TargetActor = targetActor;
			TargetGridCoordinate = targetGridCoordinate;
			WeaponAbility = weaponAbility;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((SourceActor != null) ? SourceActor.DebugInfo : "null") + ", TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null") + ", WeaponAbilityID = " + WeaponAbility.DefinitionID;
		}
	}
}
