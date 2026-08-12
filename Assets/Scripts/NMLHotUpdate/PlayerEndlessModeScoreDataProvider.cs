using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class PlayerEndlessModeScoreDataProvider : PlayerLeaderboardScoreDataProvider
{
	private readonly bool isPrevious;

	public PlayerEndlessModeScoreDataProvider(string leaderboardName, int entries, bool isPrevious = false, bool useCachedOnly = false)
		: base(leaderboardName)
	{
		base.useCachedOnly = useCachedOnly;
		this.isPrevious = isPrevious;
		base.entries = entries;
	}

	protected override bool RequestInternal()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetLeaderboard", leaderboardName, entries.ToString(), OnLeaderboardData, null, waitForResponse: true);
			return true;
		}
		return false;
	}

	protected new void OnLeaderboardData(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("GetLeaderboard failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
		}
		else
		{
			List<ScoreDataEntry> data = EndlessModePlayersScoreDataEntry.ParseLeaderboardData(GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(response), GameManager.Instance.jsonSerializer);
			NotifyDataReceived(data);
		}
	}

	protected override void AssignCurrentPlayerData(PlayerScoreDataEntry localPlayerEntry)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		localPlayerEntry.Name = playerModel.Name;
		localPlayerEntry.Id = playerModel.HashedId;
		if (isPrevious)
		{
			localPlayerEntry.Score = playerModel.EndlessModeManager.PreviousOverAllScore;
		}
		else
		{
			localPlayerEntry.Score = playerModel.EndlessModeManager.OverAllScore;
		}
	}
}
