using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class OutpostLeaderboardListPanel : ScrollableListPanel<OutpostLeaderboardScoreEntry>
{
	private class HighscoreState
	{
		public float timeAtLastScoreRetrieved;

		public string leaderboardId;

		public List<OutpostLeaderboardScoreEntry> scoreList;
	}

	private const float cacheScoreSeconds = 300f;

	[SerializeField]
	private UISprite loadingSprite;

	[SerializeField]
	private UIButtonToggleSet tabs;

	[SerializeField]
	private GameObject globalTab;

	[SerializeField]
	private GameObject guildTab;

	[SerializeField]
	private GameObject currentSeasonGlobalTab;

	[SerializeField]
	private GameObject currentSeasonLocalTab;

	[SerializeField]
	private GameObject previousSeasonGlobalTab;

	[SerializeField]
	private GameObject previousSeasonLocalTab;

	private List<HighscoreState> highscoreStates;

	private int pendingFetchTabIndex;

	private int fetchingTabIndex;

	private void OnEnable()
	{
		fetchingTabIndex = -1;
		pendingFetchTabIndex = -1;
		string guildId = GameManager.Instance.playerModel.GuildId;
		bool hasGuild = GameManager.Instance.playerModel.HasGuild;
		if (tabs != null)
		{
			UIButtonToggleSet uIButtonToggleSet = tabs;
			uIButtonToggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Combine(uIButtonToggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
		highscoreStates = new List<HighscoreState>();
		bool flag = !GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostSeasons;
		OutpostSeason outpostSeason = GameManager.Instance.gameEconomyData.GetOutpostSeason(GameManager.Instance.playerModel.UtcTimeStamp);
		OutpostSeason previousOutpostSeason = GameManager.Instance.gameEconomyData.GetPreviousOutpostSeason(GameManager.Instance.playerModel.UtcTimeStamp);
		globalTab.SetActive(!flag);
		guildTab.SetActive(!flag && hasGuild);
		currentSeasonGlobalTab.SetActive(flag && outpostSeason != null);
		currentSeasonLocalTab.SetActive(flag && outpostSeason != null);
		previousSeasonGlobalTab.SetActive(flag && previousOutpostSeason != null);
		previousSeasonLocalTab.SetActive(flag && previousOutpostSeason != null);
		if (flag)
		{
			string country = GameManager.Instance.playerModel.Country;
			if (outpostSeason != null)
			{
				highscoreStates.Add(new HighscoreState
				{
					leaderboardId = "OutpostSeason_" + outpostSeason.Id
				});
				highscoreStates.Add(new HighscoreState
				{
					leaderboardId = TWDModelManager.GetOutpostLocalLeaderboardId(outpostSeason.Id.ToString() ?? "", country)
				});
			}
			if (previousOutpostSeason != null)
			{
				highscoreStates.Add(new HighscoreState
				{
					leaderboardId = "OutpostSeason_" + previousOutpostSeason.Id
				});
				highscoreStates.Add(new HighscoreState
				{
					leaderboardId = TWDModelManager.GetOutpostLocalLeaderboardId(previousOutpostSeason.Id.ToString() ?? "", country)
				});
			}
			tabs.SetInitialToggle(2);
		}
		else
		{
			highscoreStates.Add(new HighscoreState
			{
				leaderboardId = "OutpostGlobal"
			});
			if (hasGuild)
			{
				highscoreStates.Add(new HighscoreState
				{
					leaderboardId = "OutpostGuild_" + guildId
				});
			}
			tabs.SetInitialToggle(0);
		}
	}

	private void OnDisable()
	{
		if (tabs != null)
		{
			UIButtonToggleSet uIButtonToggleSet = tabs;
			uIButtonToggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(uIButtonToggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
	}

	private void OnNewTabSelected(UIButtonExtended button)
	{
		int num = int.Parse(button.id);
		int num2 = ((num > 1) ? (num - 2) : num);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		}
		foreach (UIListCard<OutpostLeaderboardScoreEntry> card in cards)
		{
			card.GetComponent<CacheableObject>().Destroy();
		}
		if (num2 >= 0 && num2 < highscoreStates.Count && highscoreStates[num2] != null)
		{
			if (highscoreStates[num2].timeAtLastScoreRetrieved == 0f || Time.time > highscoreStates[num2].timeAtLastScoreRetrieved + 300f)
			{
				RequestScores(num2);
			}
			else
			{
				ShowCachedScores(num2);
			}
		}
	}

	public void Update()
	{
		if (fetchingTabIndex == -1 && pendingFetchTabIndex >= 0)
		{
			RequestScores(pendingFetchTabIndex);
			pendingFetchTabIndex = -1;
		}
	}

	private void RequestScores(int tabIndex)
	{
		loadingSprite.gameObject.SetActive(value: true);
		if (fetchingTabIndex >= 0)
		{
			pendingFetchTabIndex = tabIndex;
			return;
		}
		fetchingTabIndex = tabIndex;
		HighscoreState highscoreState = highscoreStates[fetchingTabIndex];
		SignalRClient.Instance.RequestCommand("GetLeaderboard", highscoreState.leaderboardId, "100", OnLeaderboardRetrieved, null, waitForResponse: true);
	}

	private void OnLeaderboardRetrieved(string message)
	{
		if (fetchingTabIndex >= 0)
		{
			try
			{
				highscoreStates[fetchingTabIndex].scoreList = new List<OutpostLeaderboardScoreEntry>();
				IEnumerable<LeaderboardEntry> enumerable = GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(message);
				if (enumerable != null)
				{
					foreach (LeaderboardEntry item in enumerable)
					{
						if (item.Details != null)
						{
							Leaderboards.OutpostLeaderboardDetails outpostLeaderboardDetails = GameManager.Instance.jsonSerializer.DeserializeObject<Leaderboards.OutpostLeaderboardDetails>(item.Details);
							highscoreStates[fetchingTabIndex].scoreList.Add(new OutpostLeaderboardScoreEntry
							{
								HashedId = item.Id,
								Score = item.Score,
								Name = outpostLeaderboardDetails.Name,
								Level = outpostLeaderboardDetails.Level,
								OutpostTierId = outpostLeaderboardDetails.OutpostTierId,
								IsOwnPlayer = false
							});
						}
					}
				}
				ShowScores(fetchingTabIndex);
			}
			catch (Exception)
			{
			}
		}
		fetchingTabIndex = -1;
	}

	protected virtual void SetupOwnPlayerEntry(ref List<OutpostLeaderboardScoreEntry> scores)
	{
		OutpostLeaderboardScoreEntry outpostLeaderboardScoreEntry = null;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		for (int i = 0; i < ((scores != null) ? scores.Count : 0); i++)
		{
			if (scores[i].HashedId == playerModel.HashedId)
			{
				outpostLeaderboardScoreEntry = scores[i];
			}
		}
		if (outpostLeaderboardScoreEntry != null)
		{
			outpostLeaderboardScoreEntry.Score = playerModel.RankingScore;
			outpostLeaderboardScoreEntry.IsOwnPlayer = true;
			return;
		}
		string outpostTierId = ((playerModel.CurrentOutpostTier != null) ? playerModel.CurrentOutpostTier.Id : "");
		scores.Add(new OutpostLeaderboardScoreEntry
		{
			Score = playerModel.RankingScore,
			Name = playerModel.Name,
			HashedId = playerModel.HashedId,
			Level = playerModel.Level,
			OutpostTierId = outpostTierId
		});
	}

	private bool ShouldInsertOwnPlayer(int tabIndex)
	{
		return tabIndex < 2;
	}

	private void ShowScores(int tabIndex)
	{
		if (loadingSprite != null && loadingSprite.gameObject != null)
		{
			loadingSprite.gameObject.SetActive(value: false);
		}
		if (highscoreStates == null || highscoreStates.Count == 0 || tabIndex < 0 || tabIndex >= highscoreStates.Count)
		{
			return;
		}
		int num = 100;
		List<OutpostLeaderboardScoreEntry> scores = highscoreStates[tabIndex].scoreList;
		if (scores != null)
		{
			if (ShouldInsertOwnPlayer(tabIndex))
			{
				SetupOwnPlayerEntry(ref scores);
			}
			if (scores.Count > num)
			{
				scores.RemoveAt(num);
			}
		}
		ShowCachedScores(tabIndex);
	}

	private void ShowCachedScores(int tabIndex)
	{
		if (tabIndex < 0 || tabIndex >= highscoreStates.Count)
		{
			return;
		}
		HighscoreState highscoreState = highscoreStates[tabIndex];
		if (highscoreState.scoreList == null)
		{
			Debug.LogError("Scores not yet downloaded");
			return;
		}
		loadingSprite.gameObject.SetActive(value: false);
		SetCards(highscoreState.scoreList);
		for (int i = 0; i < cards.Count; i++)
		{
			((OutpostLeaderboardListCard)cards[i]).SetRank(cards.Count - i);
		}
	}
}
