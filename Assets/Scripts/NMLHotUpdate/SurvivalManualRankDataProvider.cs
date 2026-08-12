using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class SurvivalManualRankDataProvider : ScoreDataProvider
{
	private string max = "40";

	protected override ScoreDataEntry CreateEntry()
	{
		return new SurvivalManualScoreDataEntry();
	}

	protected override bool RequestInternal()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			string playerSurvivalManualLeaderboardName = Leaderboards.GetPlayerSurvivalManualLeaderboardName();
			SignalRClient.Instance.RequestCommand("GetLeaderboard", playerSurvivalManualLeaderboardName, max, OnData, null, waitForResponse: true);
			return true;
		}
		return false;
	}

	protected void OnData(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("get SurvivalManual RankData failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
		}
		else
		{
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			list = SurvivalManualScoreDataEntry.ParseLeaderboardData(GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<SurvivalManualEntry>>(response), GameManager.Instance.jsonSerializer);
			NotifyDataReceived(list);
		}
	}

	public override int GetCacheDurationSeconds()
	{
		return 2;
	}
}
