using System.Collections.Generic;
using TWDModel;

public class EndlessModeLeaderboardPanel : PlayerScorePanel
{
	protected override void InitializeProviders()
	{
		EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = EndlessModeHelpers.EndlessManagerModel().CurrentEndlessModeCalendarDefinition;
		SetDataProvider(Leaderboards.EndlessModeCycle + currentEndlessModeCalendarDefinition.Identifier);
		SetDataProvider(Leaderboards.EndlessModeCycle + (currentEndlessModeCalendarDefinition.Identifier - 1), isPrevious: true, useOnlyCachedOnly: true);
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.PlayerListEndless);
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.PlayerListEndless);
	}

	private void SetDataProvider(string leaderboardName, bool isPrevious = false, bool useOnlyCachedOnly = false)
	{
		ScoreDataProvider scoreDataProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderBoard(leaderboardName);
		if (scoreDataProvider == null)
		{
			scoreDataProvider = new PlayerEndlessModeScoreDataProvider(leaderboardName, 100, isPrevious, useOnlyCachedOnly);
			GameManager.Instance.CachedLeaderboardsManager.AddLeaderboard(leaderboardName, scoreDataProvider);
		}
		providers.Add(scoreDataProvider);
	}

	protected override void Sort()
	{
		cards.StableSort(delegate(UIListCard<ScoreDataEntry> a, UIListCard<ScoreDataEntry> b)
		{
			long sortLongValue = a.GetSortLongValue();
			long sortLongValue2 = b.GetSortLongValue();
			if (sortLongValue == sortLongValue2)
			{
				if (a.Item is EndlessModePlayersScoreDataEntry endlessModePlayersScoreDataEntry && b.Item is EndlessModePlayersScoreDataEntry endlessModePlayersScoreDataEntry2)
				{
					if (endlessModePlayersScoreDataEntry.UtcTimeStamp <= endlessModePlayersScoreDataEntry2.UtcTimeStamp)
					{
						return 1;
					}
					return -1;
				}
				return 0;
			}
			return (sortLongValue <= sortLongValue2) ? 1 : (-1);
		});
	}

	protected override void OnDataReceived(ScoreDataProvider provider, List<ScoreDataEntry> data)
	{
		base.OnDataReceived(provider, data);
		for (int i = 100; i < cards.Count; i++)
		{
			GuildPlayerListCard guildPlayerListCard = (GuildPlayerListCard)cards[i];
			if ((bool)guildPlayerListCard)
			{
				guildPlayerListCard.SetRank("-");
			}
		}
	}
}
