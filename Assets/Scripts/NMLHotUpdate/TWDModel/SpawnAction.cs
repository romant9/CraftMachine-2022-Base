using BaseModel;

namespace TWDModel
{
	public class SpawnAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public ActorModel Instigator { get; private set; }

		public ActorSpawnPointModel ActorSpawnPoint { get; private set; }

		public GridCoordinate SpawnLocation { get; private set; }

		public SpawnAction(ActorModel actor, ActorSpawnPointModel spawnPoint, GridCoordinate spawnLocation, ActorModel instigator)
			: base(actor)
		{
			Actor = actor;
			ActorSpawnPoint = spawnPoint;
			SpawnLocation = spawnLocation;
			Instigator = instigator;
		}

		public override bool Execute(ModelManager manager)
		{
			if ((manager as TWDModelManager).CombatModel != null && manager.GetModel<ActorModel>(base.ModelId) != null)
			{
				return true;
			}
			return false;
		}
	}
}
