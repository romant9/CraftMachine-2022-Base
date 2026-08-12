using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class GuildBattlePlayersScoreDataEntry : ScoreDataEntry
{
	public string GuildId;

	public PlayerEmblem PlayerEmblem;

	public bool PointsWithHeld;

	public GuildBattlePlayersScoreDataEntry()
	{
	}

	public GuildBattlePlayersScoreDataEntry(string playerName, string playerHashedId, string groupId, PlayerEmblem playerEmblem, int score)
	{
		Id = playerHashedId;
		Name = playerName;
		GuildId = groupId;
		Score = score;
		PlayerEmblem = playerEmblem;
	}

	public static List<ScoreDataEntry> ParseLeaderboardData(IEnumerable<LeaderboardEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (LeaderboardEntry entry in entries)
		{
			Leaderboards.GuildBattlePlayersScoreLeaderboardDetails guildBattlePlayersScoreLeaderboardDetails = serializer.Deserialize<Leaderboards.GuildBattlePlayersScoreLeaderboardDetails>(entry.Details);
			PlayerEmblem playerEmblem = null;
			if (guildBattlePlayersScoreLeaderboardDetails.PlayerEmblem != null)
			{
				playerEmblem = serializer.Deserialize<PlayerEmblem>(guildBattlePlayersScoreLeaderboardDetails.PlayerEmblem);
			}
			list.Add(new GuildBattlePlayersScoreDataEntry(guildBattlePlayersScoreLeaderboardDetails.PlayerName, entry.Id, guildBattlePlayersScoreLeaderboardDetails.GroupId, playerEmblem, (int)entry.Score));
		}
		return list;
	}
}
