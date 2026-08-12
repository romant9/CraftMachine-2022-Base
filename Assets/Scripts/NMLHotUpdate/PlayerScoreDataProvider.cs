using System.Collections.Generic;
using TWDModel;

public abstract class PlayerScoreDataProvider : ScoreDataProvider
{
	protected override ScoreDataEntry CreateEntry()
	{
		return new PlayerScoreDataEntry();
	}

	protected List<string> GetCurrentPlayerSocialIds()
	{
		List<string> list = new List<string>();
		if (GameManager.Instance.GameCenterManager.Authenticated)
		{
			list.Add(GameManager.Instance.GameCenterManager.GetIdWithPrefix());
		}
		return list;
	}

	protected override void AddCurrentPlayerData(List<ScoreDataEntry> data)
	{
		PlayerScoreDataEntry playerScoreDataEntry = null;
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].Id == GameManager.Instance.playerModel.HashedId)
			{
				playerScoreDataEntry = data[i] as PlayerScoreDataEntry;
				break;
			}
		}
		if (playerScoreDataEntry == null)
		{
			playerScoreDataEntry = CreateEntry() as PlayerScoreDataEntry;
			data.Add(playerScoreDataEntry);
		}
		AssignCurrentPlayerData(playerScoreDataEntry);
	}

	protected virtual void AssignCurrentPlayerData(PlayerScoreDataEntry localPlayerEntry)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		localPlayerEntry.Name = playerModel.Name;
		localPlayerEntry.Score = playerModel.WeeklyChallenge.AllTimeNumberStars;
		localPlayerEntry.Id = playerModel.HashedId;
		localPlayerEntry.MemberInfo = new GuildMemberInfo();
		localPlayerEntry.MemberInfo.CurrentChallengeStars = playerModel.WeeklyChallenge.NumberStars;
		localPlayerEntry.MemberInfo.PlayerLevel = playerModel.Level;
		localPlayerEntry.MemberInfo.Name = playerModel.Name;
		localPlayerEntry.MemberInfo.MemberId = playerModel.HashedId;
		localPlayerEntry.MemberInfo.PlayerEmblem = playerModel.PlayerEmblem;
		localPlayerEntry.MemberInfo.TotalChallengeStars = playerModel.WeeklyChallenge.AllTimeNumberStars;
		if (GameManager.Instance.playerModel.IsGuildMember)
		{
			localPlayerEntry.MemberInfo.GuildId = playerModel.GuildId;
			localPlayerEntry.MemberInfo.GuildLeaderboardName = playerModel.GuildModel.Name;
			localPlayerEntry.MemberInfo.Role = playerModel.GuildModel.GetMemberRole(localPlayerEntry.Id).Value;
			localPlayerEntry.MemberInfo.PlayerLevel = playerModel.Level;
		}
	}
}
