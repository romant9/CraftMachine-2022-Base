using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class PlayerScoreDataEntry : ScoreDataEntry
{
	public GuildMemberInfo MemberInfo;

	public List<string> SocialIds;

	public PlayerScoreDataEntry()
	{
		MemberInfo = new GuildMemberInfo();
	}

	public PlayerScoreDataEntry(GuildMemberInfo info, long score)
	{
		Id = info.MemberId;
		Name = info.Name;
		MemberInfo = info;
		Score = score;
	}

	public static List<ScoreDataEntry> ParseLeaderboardData(IEnumerable<ScoreEntry> entries, IMessageSerializer serializer)
	{
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		foreach (ScoreEntry entry in entries)
		{
			PlayerScoreDataEntry playerScoreDataEntry = new PlayerScoreDataEntry(new GuildMemberInfo
			{
				Name = entry.Nickname,
				TotalChallengeStars = (int)entry.Score,
				PlayerLevel = entry.Level,
				MemberId = entry.HashedId
			}, entry.Score);
			playerScoreDataEntry.SocialIds = entry.Socials;
			list.Add(playerScoreDataEntry);
		}
		return list;
	}
}
