using TWDModel;
using UnityEngine;

public class PlayerScoreCurrentChallengePanel : PlayerScorePanel
{
	[SerializeField]
	private UILabel currentChallengeLocalLabel;

	[SerializeField]
	private UILabel previousChallengeLocalLabel;

	[SerializeField]
	private GameObject previousGlobal;

	[SerializeField]
	private GameObject previousLocal;

	protected override void SetLocalTabsTextLabel()
	{
		string text = GameManager.GetCountryCode().ToUpper();
		HelpersUI.SetContentToLabel(currentChallengeLocalLabel, LocalizationManager.GetText("Popup.Challenge.HighScore.CurrentLocal") + "(" + text + ")");
		HelpersUI.SetContentToLabel(previousChallengeLocalLabel, LocalizationManager.GetText("Popup.Challenge.HighScore.PreviousLocal") + "(" + text + ")");
		Helpers.GameObjectSetActive(previousGlobal, WeeklyChallengeHelper.IsNormalChallenge);
		Helpers.GameObjectSetActive(previousLocal, WeeklyChallengeHelper.IsNormalChallenge);
	}

	protected override void InitializeProviders()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			int id = GameManager.Instance.playerModel.WeeklyChallenge.Id;
			string countryCode = GameManager.GetCountryCode();
			SetDataProvider(Leaderboards.GetPlayerChallengeWeeklyLeaderboardName(id.ToString()));
			SetDataProvider(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardName(countryCode, id.ToString()));
			SetDataProvider(Leaderboards.GetPlayerChallengeWeeklyLeaderboardName((id - 1).ToString()), useCachedOnlyData: true, previousChallenge: true);
			SetDataProvider(Leaderboards.GetPlayerChallengeWeeklyCountryLeaderboardName(countryCode, (id - 1).ToString()), useCachedOnlyData: true, previousChallenge: true);
			cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
			cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
			cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
			cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
		}
		else
		{
			int id2 = GameManager.Instance.playerModel.WeeklyChallenge.Id;
			string countryCode2 = GameManager.GetCountryCode();
			SetApocalypticDataProvider(Leaderboards.GetPlayerApocalypseChallengeWeeklyLeaderboardName(id2.ToString()));
			SetApocalypticDataProvider(Leaderboards.GetPlayerApocalypseChallengeWeeklyCountryLeaderboardName(countryCode2, id2.ToString()));
			cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
			cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
		}
	}

	private void SetDataProvider(string leaderboardName, bool useCachedOnlyData = false, bool previousChallenge = false)
	{
		ScoreDataProvider scoreDataProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderBoard(leaderboardName);
		if (scoreDataProvider == null)
		{
			scoreDataProvider = new PlayerLeaderboardCurrentChallengeScoreDataProvider(leaderboardName, useCachedOnlyData, previousChallenge);
			GameManager.Instance.CachedLeaderboardsManager.AddLeaderboard(leaderboardName, scoreDataProvider);
		}
		providers.Add(scoreDataProvider);
	}

	private void SetApocalypticDataProvider(string leaderboardName, bool useCachedOnlyData = false, bool previousChallenge = false)
	{
		ScoreDataProvider scoreDataProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderBoard(leaderboardName);
		if (scoreDataProvider == null)
		{
			scoreDataProvider = new PlayerLeaderboardCurrentApocalypticChallengeScoreDataProvider(leaderboardName, useCachedOnlyData, previousChallenge);
			GameManager.Instance.CachedLeaderboardsManager.AddLeaderboard(leaderboardName, scoreDataProvider);
		}
		providers.Add(scoreDataProvider);
	}
}
