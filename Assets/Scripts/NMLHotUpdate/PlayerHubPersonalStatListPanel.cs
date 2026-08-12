using System.Collections.Generic;
using TWDModel;

public class PlayerHubPersonalStatListPanel : ScrollableListPanel<PlayerHubPersonalStatCardItem>
{
	private List<PlayerHubPersonalStatCardItem> stats = new List<PlayerHubPersonalStatCardItem>();

	protected override bool LastEntryAtTop => false;

	public void OnEnable()
	{
		CreateStats();
		SetCards(stats);
	}

	private void CreateStats()
	{
		stats.Clear();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.MissionStatistics != null)
		{
			stats.Add(new PlayerHubPersonalStatCardItem
			{
				Type = "MissionsCompleted",
				Value = playerModel.MissionStatistics.MissionsCompleted
			});
			stats.Add(new PlayerHubPersonalStatCardItem
			{
				Type = "WalkersKilled",
				Value = playerModel.MissionStatistics.WalkersKilled
			});
			stats.Add(new PlayerHubPersonalStatCardItem
			{
				Type = "RaidersKilled",
				Value = playerModel.MissionStatistics.RaidersKilled
			});
			stats.Add(new PlayerHubPersonalStatCardItem
			{
				Type = "ChallengeStars",
				Value = ((playerModel.WeeklyChallenge != null) ? playerModel.WeeklyChallenge.AllTimeNumberStars : 0)
			});
		}
	}
}
