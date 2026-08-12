using BaseModel;

namespace TWDModel
{
	public class ClaimReturnRepeatQuestRewardCommand : ModelCommand
	{
		public int DefinitionId { get; set; }

		public ClaimReturnRepeatQuestRewardCommand()
		{
		}

		public ClaimReturnRepeatQuestRewardCommand(int definitionId)
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
			bool flag = ((DefinitionId > 0) ? tWDModelManager.Player.ReturnActivityManager.ReturnQuestAndExchange.TryClaimRepeatQuestReward(DefinitionId) : tWDModelManager.Player.ReturnActivityManager.ReturnQuestAndExchange.TryClaimRepeatQuestReward());
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
