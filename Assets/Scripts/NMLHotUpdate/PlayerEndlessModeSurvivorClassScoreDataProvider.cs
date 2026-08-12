using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;

public class PlayerEndlessModeSurvivorClassScoreDataProvider : PlayerEndlessModeScoreDataProvider
{
	public PlayerEndlessModeSurvivorClassScoreDataProvider(string leaderboardName, int entries, bool isPrevious = false, bool useCachedOnly = false)
		: base(leaderboardName, entries, isPrevious, useCachedOnly)
	{
	}

	protected override void AddCurrentPlayerData(List<ScoreDataEntry> data)
	{
	}

	protected override bool RequestInternal()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetLeaderboard", leaderboardName, entries.ToString(), OnSurvivorClassLeaderboardData, null, waitForResponse: true);
			return true;
		}
		return false;
	}

	protected void OnSurvivorClassLeaderboardData(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("GetLeaderboard failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
		}
		else
		{
			List<ScoreDataEntry> data = EndlessModePlayersScoreDataEntry.ParseSurvivorClassLeaderboardData(GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(response), GameManager.Instance.jsonSerializer);
			NotifyDataReceived(data);
		}
	}
}
