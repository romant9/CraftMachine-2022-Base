using BaseModel;

namespace TWDModel
{
	public class StartChallengeCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			WeeklyChallenge currentDefinition = player.WeeklyChallenge.CurrentDefinition;
			WeeklyChallenge nextWeeklyChallenge = player.WeeklyChallenge.NextWeeklyChallenge;
			tWDModelManager.Debug.Log("StartChallengeCommand: " + tWDModelManager.GameEconomyData.WeeklyChallenges.Count + " challenges.");
			if (currentDefinition == null)
			{
				tWDModelManager.Debug.Log("StartChallengeCommand Current Challenge is null");
			}
			else
			{
				tWDModelManager.Debug.Log("StartChallengeCommand Current Challenge start " + currentDefinition.StartTimeMilliseconds + " end " + currentDefinition.EndTimeMilliseconds);
			}
			if (nextWeeklyChallenge != null)
			{
				tWDModelManager.Debug.Log("StartChallengeCommand: Id=" + player.WeeklyChallenge.Id + " Next Challenge start " + nextWeeklyChallenge.StartTimeMilliseconds + " end " + nextWeeklyChallenge.EndTimeMilliseconds + " Now " + player.UtcTimeStamp);
			}
			else
			{
				tWDModelManager.Debug.Log("StartChallengeCommand: Id=" + player.WeeklyChallenge.Id + " Next is NulL");
			}
			if (player.WeeklyChallenge.CanPlayNextWeeklyChallenge)
			{
				WeeklyChallenge nextWeeklyChallenge2 = player.WeeklyChallenge.NextWeeklyChallenge;
				player.WeeklyChallenge.Reset(nextWeeklyChallenge2.Identifier);
				player.ApocalypseWeeklyChallenge.Reset(nextWeeklyChallenge2.Identifier);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
