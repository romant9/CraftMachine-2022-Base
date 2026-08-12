using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;

public class GuildLeaderboardScoreDataProvider : GuildScoreDataProvider
{
	protected string leaderboardName;

	public GuildLeaderboardScoreDataProvider(string leaderboardName)
	{
		this.leaderboardName = leaderboardName;
	}

	protected override bool RequestInternal()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetLeaderboard", leaderboardName, "50", OnLeaderboardData, null, waitForResponse: true);
			return true;
		}
		return false;
	}

	protected void OnLeaderboardData(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("GetLeaderboard failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
		}
		else
		{
			List<ScoreDataEntry> data = GuildScoreDataEntry.ParseLeaderboardData(GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(response), GameManager.Instance.jsonSerializer);
			NotifyDataReceived(data);
		}
	}
}
