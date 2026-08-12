using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class SurvivalManualScoreDataEntry : ScoreDataEntry
{
	public PlayerEmblem PlayerEmblem;

	public List<int> HaveMedalStoryIds;

	public static List<ScoreDataEntry> ParseLeaderboardData(IEnumerable<SurvivalManualEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (SurvivalManualEntry entry in entries)
		{
			Leaderboards.SurvivalManualLeaderboardDetails survivalManualLeaderboardDetails = serializer.Deserialize<Leaderboards.SurvivalManualLeaderboardDetails>(entry.Details);
			SurvivalManualScoreDataEntry survivalManualScoreDataEntry = new SurvivalManualScoreDataEntry();
			survivalManualScoreDataEntry.Id = entry.Id;
			survivalManualScoreDataEntry.Name = survivalManualLeaderboardDetails.Name;
			survivalManualScoreDataEntry.Score = (int)entry.Score;
			survivalManualScoreDataEntry.PlayerEmblem = serializer.Deserialize<PlayerEmblem>(survivalManualLeaderboardDetails.PlayerEmblem ?? "");
			survivalManualScoreDataEntry.HaveMedalStoryIds = survivalManualLeaderboardDetails.SurvivalManualIds;
			list.Add(survivalManualScoreDataEntry);
		}
		return list;
	}
}
