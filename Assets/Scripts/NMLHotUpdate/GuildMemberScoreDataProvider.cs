using System.Collections.Generic;
using TWDModel;

public class GuildMemberScoreDataProvider : PlayerScoreDataProvider
{
	protected override bool RequestInternal()
	{
		if (GameManager.Instance.playerModel.IsGuildMember)
		{
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			List<GuildMemberInfo> guildMembers = GameManager.Instance.guildModel.GuildMembers;
			for (int i = 0; i < guildMembers.Count; i++)
			{
				list.Add(new PlayerScoreDataEntry(guildMembers[i], guildMembers[i].TotalChallengeStars));
			}
			NotifyDataReceived(list);
			return true;
		}
		return false;
	}

	protected override void AssignCurrentPlayerData(PlayerScoreDataEntry localPlayerEntry)
	{
		base.AssignCurrentPlayerData(localPlayerEntry);
		if (GameManager.Instance.playerModel.IsGuildMember)
		{
			GuildMemberInfo memberInfo = GameManager.Instance.playerModel.GuildModel.GetMemberInfo(localPlayerEntry.Id);
			localPlayerEntry.Score = memberInfo.TotalChallengeStars;
			localPlayerEntry.MemberInfo.TotalChallengeStars = memberInfo.TotalChallengeStars;
			localPlayerEntry.MemberInfo.CurrentChallengeStars = GameManager.Instance.playerModel.WeeklyChallenge.NumberStarsInCurrentGuild;
		}
	}

	public override int GetCacheDurationSeconds()
	{
		return 0;
	}
}
