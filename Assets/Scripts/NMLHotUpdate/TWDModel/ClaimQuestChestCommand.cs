using BaseModel;

namespace TWDModel
{
	public class ClaimQuestChestCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.DailyQuestManager != null)
			{
				tWDModelManager.Player.DailyQuestManager.TryClaimQuestChest();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
