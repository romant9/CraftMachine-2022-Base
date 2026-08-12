using BaseModel;

namespace TWDModel
{
	public class ClaimReturnDailyQuestRewardCommand : ModelCommand
	{
		public int DefinitionId { get; set; }

		public ClaimReturnDailyQuestRewardCommand(int definitionId)
		{
			DefinitionId = definitionId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnQuestAndExchange == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = tWDModelManager.Player.ReturnActivityManager.ReturnQuestAndExchange.TryClaimDailyQuestReward(DefinitionId);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
