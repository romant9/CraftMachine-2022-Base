using BaseModel;

namespace TWDModel
{
	public class ClaimDailyQuestCommand : ModelCommand
	{
		public int QuestId { get; set; }

		public ClaimDailyQuestCommand()
		{
		}

		public ClaimDailyQuestCommand(int questId)
		{
			QuestId = questId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.DailyQuestManager != null)
			{
				tWDModelManager.Player.DailyQuestManager.TryClaimQuest(QuestId);
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
