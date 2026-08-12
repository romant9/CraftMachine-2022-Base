using BaseModel;

namespace TWDModel
{
	public class MarkDailyQuestSeenComand : ModelCommand
	{
		public enum Value
		{
			CompletedCount = 0,
			ClaimedSeen = 1
		}

		public int QuestId { get; set; }

		public Value SeenValue { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!((manager as TWDModelManager).GetModel(QuestId) is DailyQuestItemModel dailyQuestItemModel))
			{
				manager.Debug.LogError($"Could not find daily quest item with ID {QuestId}.");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			switch (SeenValue)
			{
			case Value.CompletedCount:
				dailyQuestItemModel.CompletedCountSeen = true;
				break;
			case Value.ClaimedSeen:
				dailyQuestItemModel.ClaimedSeen = true;
				break;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
