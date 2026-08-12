using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class EndlessModePlayersScoreDataEntry : PlayerScoreDataEntry
{
	public int EndlessModeExpertTagCount;

	public SurvivorClass SurvivorClass;

	public string LeaderActorDefinitionId;

	public long UtcTimeStamp;

	public EndlessModePlayersScoreDataEntry()
	{
	}

	public EndlessModePlayersScoreDataEntry(string playerName, string playerHashedId, long score, int endlessModeExpertTagCount, PlayerEmblem playerEmblem, long utcTimeStamp)
	{
		Id = playerHashedId;
		Name = playerName;
		Score = score;
		EndlessModeExpertTagCount = endlessModeExpertTagCount;
		MemberInfo = new GuildMemberInfo();
		MemberInfo.PlayerEmblem = playerEmblem;
		UtcTimeStamp = utcTimeStamp;
	}

	public static List<ScoreDataEntry> ParseLeaderboardData(IEnumerable<LeaderboardEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (LeaderboardEntry entry in entries)
		{
			Leaderboards.EndlessModeLeaderBoardDetails endlessModeLeaderBoardDetails = serializer.Deserialize<Leaderboards.EndlessModeLeaderBoardDetails>(entry.Details);
			PlayerEmblem playerEmblem = serializer.Deserialize<PlayerEmblem>(endlessModeLeaderBoardDetails.PlayerEmblem ?? "");
			list.Add(new EndlessModePlayersScoreDataEntry(endlessModeLeaderBoardDetails.Name, entry.Id, entry.Score, endlessModeLeaderBoardDetails.ExpertModeEntryCount, playerEmblem, entry.ScoreAt));
		}
		return list;
	}

	public static List<ScoreDataEntry> ParseSurvivorClassLeaderboardData(IEnumerable<LeaderboardEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		new HighScores().Scores = new List<ScoreEntry>();
		foreach (LeaderboardEntry entry in entries)
		{
			Leaderboards.EndlessModeLeaderSurvivorClassLeaderBoardDetails endlessModeLeaderSurvivorClassLeaderBoardDetails = serializer.Deserialize<Leaderboards.EndlessModeLeaderSurvivorClassLeaderBoardDetails>(entry.Details);
			PlayerEmblem playerEmblem = serializer.Deserialize<PlayerEmblem>(endlessModeLeaderSurvivorClassLeaderBoardDetails.PlayerEmblem ?? "");
			EndlessModePlayersScoreDataEntry endlessModePlayersScoreDataEntry = new EndlessModePlayersScoreDataEntry(endlessModeLeaderSurvivorClassLeaderBoardDetails.Name, entry.Id, entry.Score, 0, playerEmblem, entry.ScoreAt);
			endlessModePlayersScoreDataEntry.SurvivorClass = endlessModeLeaderSurvivorClassLeaderBoardDetails.LeaderSurvivorClass;
			endlessModePlayersScoreDataEntry.LeaderActorDefinitionId = endlessModeLeaderSurvivorClassLeaderBoardDetails.LeaderActorDefinitionId;
			list.Add(endlessModePlayersScoreDataEntry);
		}
		return list;
	}
}
