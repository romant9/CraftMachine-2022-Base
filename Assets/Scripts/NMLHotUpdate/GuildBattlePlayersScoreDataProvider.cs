using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;

public class GuildBattlePlayersScoreDataProvider : ScoreDataProvider
{
	protected string leaderboardName;

	protected override ScoreDataEntry CreateEntry()
	{
		return new GuildBattlePlayersScoreDataEntry();
	}

	public GuildBattlePlayersScoreDataProvider(string leaderboardName)
	{
		this.leaderboardName = leaderboardName;
	}

	protected override bool RequestInternal()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetLeaderboard", leaderboardName, "40", OnLeaderboardData, null, waitForResponse: true);
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
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			list = GuildBattlePlayersScoreDataEntry.ParseLeaderboardData(GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(response), GameManager.Instance.jsonSerializer);
			NotifyDataReceived(list);
		}
	}

	public override int GetCacheDurationSeconds()
	{
		return 1;
	}
}
