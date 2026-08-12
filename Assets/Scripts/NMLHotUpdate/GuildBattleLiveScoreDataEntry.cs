using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class GuildBattleLiveScoreDataEntry : ScoreDataEntry
{
	public GuildBattleLiveScoreDataEntry()
	{
	}

	public GuildBattleLiveScoreDataEntry(string groupId, string groupName, int score)
	{
		Id = groupId;
		Name = groupName;
		Score = score;
	}

	public static List<ScoreDataEntry> ParseLeaderboardData(IEnumerable<LeaderboardEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (LeaderboardEntry entry in entries)
		{
			Leaderboards.GuildBattleLiveScoreLeaderboardDetails guildBattleLiveScoreLeaderboardDetails = serializer.Deserialize<Leaderboards.GuildBattleLiveScoreLeaderboardDetails>(entry.Details);
			list.Add(new GuildBattleLiveScoreDataEntry(guildBattleLiveScoreLeaderboardDetails.GroupId, guildBattleLiveScoreLeaderboardDetails.GroupName, (int)entry.Score));
		}
		return list;
	}
}
