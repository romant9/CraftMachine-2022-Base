using System.Collections.Generic;

public class CachedLeaderboardsManager
{
	private Dictionary<string, ScoreDataProvider> cachedLeaderboards;

	private Dictionary<int, LeaderboardPositionProvider> cachedLeaderboardPositions;

	public CachedLeaderboardsManager()
	{
		cachedLeaderboards = new Dictionary<string, ScoreDataProvider>();
		cachedLeaderboardPositions = new Dictionary<int, LeaderboardPositionProvider>();
	}

	public ScoreDataProvider GetLeaderBoard(string leaderboardName)
	{
		cachedLeaderboards.TryGetValue(leaderboardName, out var value);
		return value;
	}

	public void AddLeaderboard(string leaderboardName, ScoreDataProvider scoreDataProvider)
	{
		if (!cachedLeaderboards.ContainsKey(leaderboardName))
		{
			cachedLeaderboards.Add(leaderboardName, scoreDataProvider);
		}
	}

	public LeaderboardPositionProvider GetLeaderboardPositionProvider(int leaderboardId)
	{
		if (!cachedLeaderboardPositions.TryGetValue(leaderboardId, out var value))
		{
			return new LeaderboardPositionProvider(leaderboardId);
		}
		return value;
	}
}
