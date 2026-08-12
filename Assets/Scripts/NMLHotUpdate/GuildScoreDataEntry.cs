using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class GuildScoreDataEntry : ScoreDataEntry
{
	public static List<ScoreDataEntry> ParseLeaderboardData(IEnumerable<LeaderboardEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (LeaderboardEntry entry in entries)
		{
			Leaderboards.ChallengeLeaderboardDetails challengeLeaderboardDetails = serializer.Deserialize<Leaderboards.ChallengeLeaderboardDetails>(entry.Details);
			GuildScoreDataEntry guildScoreDataEntry = new GuildScoreDataEntry();
			guildScoreDataEntry.Id = entry.Id;
			guildScoreDataEntry.Name = challengeLeaderboardDetails.Name;
			guildScoreDataEntry.Score = (int)entry.Score;
			list.Add(guildScoreDataEntry);
		}
		return list;
	}
}
