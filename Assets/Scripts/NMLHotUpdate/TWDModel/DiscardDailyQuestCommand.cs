using BaseModel;

namespace TWDModel
{
	public class DiscardDailyQuestCommand : ModelCommand
	{
		public string AchievementID { get; private set; }

		public DiscardDailyQuestCommand()
		{
		}

		public DiscardDailyQuestCommand(string achievementID)
		{
			AchievementID = achievementID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player.AchievementManager != null && tWDModelManager.Player.AchievementManager.DiscardDailyQuest(AchievementID))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
