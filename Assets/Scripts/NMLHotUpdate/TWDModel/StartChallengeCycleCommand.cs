using BaseModel;

namespace TWDModel
{
	public class StartChallengeCycleCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			WeeklyChallenge currentDefinition = player.WeeklyChallenge.CurrentDefinition;
			tWDModelManager.Debug.Log("StartChallengeCycleCommand: " + tWDModelManager.GameEconomyData.WeeklyChallenges.Count + " challenges.");
			if (currentDefinition == null)
			{
				tWDModelManager.Debug.Log("StartChallengeCycleCommand Current Challenge is null");
			}
			else
			{
				tWDModelManager.Debug.Log("StartChallengeCycleCommand Current Challenge start " + currentDefinition.StartTimeMilliseconds + " end " + currentDefinition.EndTimeMilliseconds);
			}
			if (player.WeeklyChallenge.CanStartNextCycle())
			{
				player.WeeklyChallenge.AddCycleCompleteRewards();
				player.WeeklyChallenge.StartNewCycle();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			tWDModelManager.Debug.Log("StartChallengeCycleCommand Command sent by the game when it's not possible to move to a new challenge cycle");
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
