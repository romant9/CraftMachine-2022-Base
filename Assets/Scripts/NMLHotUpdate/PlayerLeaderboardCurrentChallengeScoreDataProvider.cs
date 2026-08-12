using TWDModel;

public class PlayerLeaderboardCurrentChallengeScoreDataProvider : PlayerLeaderboardScoreDataProvider
{
	protected bool isPreviousChallenge;

	public PlayerLeaderboardCurrentChallengeScoreDataProvider(string leaderboardName, bool usedOnlyCachedData = false, bool isPreviousChallenge = false)
		: base(leaderboardName)
	{
		this.isPreviousChallenge = isPreviousChallenge;
		useCachedOnly = usedOnlyCachedData;
		entries = ((GameManager.Instance.gameEconomyData.ConfigData.CurrentChallengeLeaderboardMaxSize > 0) ? GameManager.Instance.gameEconomyData.ConfigData.CurrentChallengeLeaderboardMaxSize : 50);
	}

	protected override void AssignCurrentPlayerData(PlayerScoreDataEntry localPlayerEntry)
	{
		base.AssignCurrentPlayerData(localPlayerEntry);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (isPreviousChallenge)
		{
			localPlayerEntry.Score = playerModel.WeeklyChallenge.PreviousNumberStars;
		}
		else
		{
			localPlayerEntry.Score = playerModel.WeeklyChallenge.NumberStars;
		}
	}
}
