using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class PlayerLeaderboardScoreDataProvider : PlayerScoreDataProvider
{
	protected string leaderboardName;

	protected int entries = 50;

	public PlayerLeaderboardScoreDataProvider(string leaderboardName)
	{
		this.leaderboardName = leaderboardName;
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

	protected void OnLeaderboardData(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("GetLeaderboard failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
			return;
		}
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		IEnumerable<LeaderboardEntry> enumerable = GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(response);
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (LeaderboardEntry item in enumerable)
		{
			Leaderboards.ChallengeLeaderboardDetails challengeLeaderboardDetails = GameManager.Instance.jsonSerializer.Deserialize<Leaderboards.ChallengeLeaderboardDetails>(item.Details);
			GuildMemberInfo guildMemberInfo = new GuildMemberInfo();
			guildMemberInfo.MemberId = item.Id;
			guildMemberInfo.TotalChallengeStars = (int)item.Score;
			guildMemberInfo.Name = challengeLeaderboardDetails.Name;
			guildMemberInfo.CurrentChallengeStars = challengeLeaderboardDetails.CurrentChallengeStars;
			guildMemberInfo.PlayerLevel = challengeLeaderboardDetails.Level;
			guildMemberInfo.GuildLeaderboardName = challengeLeaderboardDetails.GroupName;
			guildMemberInfo.MemberId = item.Id;
			guildMemberInfo.PlayerEmblem = GameManager.Instance.jsonSerializer.DeserializeObject<PlayerEmblem>(challengeLeaderboardDetails.PlayerEmblem ?? "");
			list.Add(new PlayerScoreDataEntry(guildMemberInfo, item.Score));
		}
		NotifyDataReceived(list);
	}
}
