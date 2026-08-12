using TWDModel;

public class PlayerLeaderboardCurrentApocalypticChallengeScoreDataProvider : PlayerLeaderboardScoreDataProvider
{
	public PlayerLeaderboardCurrentApocalypticChallengeScoreDataProvider(string leaderboardName, bool usedOnlyCachedData = false, bool isPreviousChallenge = false)
		: base(leaderboardName)
	{
		useCachedOnly = usedOnlyCachedData;
		entries = ((GameManager.Instance.gameEconomyData.ConfigData.CurrentChallengeLeaderboardMaxSize > 0) ? GameManager.Instance.gameEconomyData.ConfigData.CurrentChallengeLeaderboardMaxSize : 50);
	}

	protected override void AssignCurrentPlayerData(PlayerScoreDataEntry localPlayerEntry)
	{
		base.AssignCurrentPlayerData(localPlayerEntry);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		localPlayerEntry.Score = playerModel.ApocalypseWeeklyChallenge.NumberStars;
	}
}
