using BaseModel;

namespace TWDModel
{
	public class SurvivalGameModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public ActorModel LeaderActor { get; private set; }

		[IgnoreModelProperty]
		public ActorModel EnemyActor { get; private set; }

		public int LeftCount { get; private set; }

		public int LeftCD { get; private set; }

		public int LeftNoDeadCount { get; private set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(SurvivalGameModel model)
		{
			LeaderActor = model.LeaderActor;
			EnemyActor = model.EnemyActor;
			LeftCount = model.LeftCount;
			LeftNoDeadCount = model.LeftNoDeadCount;
		}
	}
}
