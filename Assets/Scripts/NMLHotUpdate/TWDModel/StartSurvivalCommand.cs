using BaseModel;

namespace TWDModel
{
	public class StartSurvivalCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			WeeklySurvival currentDefinition = player.WeeklySurvival.CurrentDefinition;
			WeeklySurvival nextWeeklySurvival = player.WeeklySurvival.NextWeeklySurvival;
			tWDModelManager.Debug.Log("StartSurvivalCommand: " + tWDModelManager.GameEconomyData.WeeklySurvivals.Count + " survivals.");
			if (currentDefinition == null)
			{
				tWDModelManager.Debug.Log("StartSurvivalCommand Current Survival is null");
			}
			else
			{
				tWDModelManager.Debug.Log("StartSurvivalCommand Current Survival start " + currentDefinition.StartTimeMilliseconds + " end " + currentDefinition.EndTimeMilliseconds);
			}
			if (nextWeeklySurvival != null)
			{
				tWDModelManager.Debug.Log("StartSurvivalCommand: Id=" + player.WeeklySurvival.Id + " Next Survival start " + nextWeeklySurvival.StartTimeMilliseconds + " end " + nextWeeklySurvival.EndTimeMilliseconds + " Now " + player.UtcTimeStamp);
			}
			else
			{
				tWDModelManager.Debug.Log("StartSurvivalCommand: Id=" + player.WeeklySurvival.Id + " Next is NulL");
			}
			if (player.WeeklySurvival.IsLockedByCouncilLevel)
			{
				tWDModelManager.Debug.LogError("StartSurvivalCommand: Cannot start survival because locked out by council level.");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (player.WeeklySurvival.CanPlayNextWeeklySurvival)
			{
				WeeklySurvival nextWeeklySurvival2 = player.WeeklySurvival.NextWeeklySurvival;
				player.WeeklySurvival.ResetForNewIdentifier(nextWeeklySurvival2.Identifier);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
