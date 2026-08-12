using TWDModel;

public class GuildScorePanel : PlayerScorePanel
{
	protected override void InitializeProviders()
	{
		string challengeId = GameManager.Instance.playerModel.WeeklyChallenge.Id.ToString();
		providers.Add(new GuildLeaderboardScoreDataProvider(Leaderboards.GetGuildChallengeGlobalLeaderboardName(challengeId)));
		providers.Add(new GuildLeaderboardScoreDataProvider(Leaderboards.GetGuildChallengeCountryLeaderboardName(GameManager.GetCountryCode().ToLower(), challengeId)));
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.GuildList);
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.GuildList);
	}

	protected override void SetCard(UIListCard<ScoreDataEntry> card)
	{
		base.SetCard(card);
		((GuildPlayerListCard)card).Type = GuildPlayerListCardBase.GuildPlayerListCardType.GuildList;
	}
}
