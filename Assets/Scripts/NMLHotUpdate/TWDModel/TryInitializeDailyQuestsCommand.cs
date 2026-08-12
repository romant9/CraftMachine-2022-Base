using BaseModel;

namespace TWDModel
{
	public class TryInitializeDailyQuestsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.Error;
			if (tWDModelManager.Player != null)
			{
				if (tWDModelManager.Player.DailyQuestManager == null)
				{
					manager.Debug.LogError($"The daily quest manager has not been initialized.");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				tWDModelManager.Player.DailyQuestManager.TryInitializeQuests();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
