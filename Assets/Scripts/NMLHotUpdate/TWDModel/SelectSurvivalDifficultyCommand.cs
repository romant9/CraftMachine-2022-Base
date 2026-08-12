using BaseModel;

namespace TWDModel
{
	public class SelectSurvivalDifficultyCommand : ModelCommand
	{
		public SurvivalDifficulty Difficulty { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			if (player.WeeklySurvival == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			WeeklySurvival currentDefinition = player.WeeklySurvival.CurrentDefinition;
			tWDModelManager.Debug.Log("SelectSurvivalDifficultyCommand: " + tWDModelManager.GameEconomyData.WeeklySurvivals.Count + " survivals.");
			if (currentDefinition == null)
			{
				tWDModelManager.Debug.Log("SelectSurvivalDifficultyCommand Current Survival is null");
			}
			else
			{
				tWDModelManager.Debug.Log("SelectSurvivalDifficultyCommand Current Survival start " + currentDefinition.StartTimeMilliseconds + " end " + currentDefinition.EndTimeMilliseconds);
			}
			if (player.WeeklySurvival.IsDifficultyLocked(Difficulty))
			{
				tWDModelManager.Debug.LogError("SelectSurvivalDifficultyCommand: Selected difficulty level still locked.");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!player.WeeklySurvival.IsDifficultySelected)
			{
				player.WeeklySurvival.ResetCurrentForDifficulty(Difficulty);
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
