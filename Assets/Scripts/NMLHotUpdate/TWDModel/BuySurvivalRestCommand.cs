using BaseModel;

namespace TWDModel
{
	public class BuySurvivalRestCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult;
			if (tWDModelManager.Player.WeeklySurvival != null && tWDModelManager.Player.WeeklySurvival.IsRestAvailable)
			{
				tWDModelResult = tWDModelManager.Player.SurvivorContainer.SurvivalCharacters.BuyRest();
				if (tWDModelResult == TWDModelResult.OK)
				{
					tWDModelManager.Player.WeeklySurvival.IsRestAvailable = false;
				}
			}
			else
			{
				tWDModelResult = TWDModelResult.Error;
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
