using BaseModel;

namespace TWDModel
{
	public class NewbieSevenQuestStageRewardCommand : ModelCommand
	{
		public int Point { get; set; }

		public NewbieSevenQuestStageRewardCommand()
		{
		}

		public NewbieSevenQuestStageRewardCommand(int point)
		{
			Point = point;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.NewbieSenvenQuest != null && tWDModelManager.Player.NewbieSenvenQuest.TryClaimStageReward(Point))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
