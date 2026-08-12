using BaseModel;

namespace TWDModel
{
	public class CheckAchievementsCommand : ModelCommand
	{
		public string Platform;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager.GetPlayer() as PlayerModel).AchievementManager.CheckAchievements();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
