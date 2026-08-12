using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessMissionHubLeaderBoardPanel : PlayerScorePanel
{
	[SerializeField]
	private List<SurvivorButtonFilter> survivalButtons;

	[SerializeField]
	private List<EndlessModeLeaderBoardRewardEntry> topLeaderBoardList;

	[SerializeField]
	private EndlessModeLeaderBoardRewardEntry ownLeaderBoardEntry;

	[SerializeField]
	private GameObject titleGroup;

	[SerializeField]
	private GameObject myScoreGroup;

	[SerializeField]
	private GameObject titleGroupProfessional;

	[SerializeField]
	private GameObject myScoreGroupProfessional;

	[SerializeField]
	private GameObject noScoreGroupProfessional;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel desLabel;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private List<EndlessModeAttemptTeamEntry> teamEntries;

	[SerializeField]
	private EndlessModeAttemptScoreList attemptScoreList;

	[SerializeField]
	private EndlessModeAttemptScoreList attemptScoreSurvivorClassList;

	[SerializeField]
	private UISprite ownBoxRewardIcon;

	private Dictionary<string, LeaderboardPosition> _positionDic = new Dictionary<string, LeaderboardPosition>();

	private List<GameObject> _percentRewardList = new List<GameObject>();

	private SurvivorClass _survivorClass;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	protected override void OnEnable()
	{
		base.OnEnable();
		UIEvent.OnUIEvent += OnUIEvent;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "EndlessScanEvent" && _survivorClass != SurvivorClass.None && attemptScoreSurvivorClassList.gameObject.activeInHierarchy)
		{
			attemptScoreSurvivorClassList.UpdateUI(EndlessModeGameModeType.Expert, _survivorClass);
		}
	}

	protected override void InitializeProviders()
	{
		_positionDic.Clear();
		EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = EndlessModeHelpers.EndlessManagerModel().CurrentEndlessModeCalendarDefinition;
		for (int i = 0; i < survivalButtons.Count; i++)
		{
			if (survivalButtons[i].SurvivorClass == SurvivorClass.None)
			{
				string endlessModeLeaderboardName = Leaderboards.GetEndlessModeLeaderboardName(currentEndlessModeCalendarDefinition.Identifier);
				SetDataProvider(endlessModeLeaderboardName);
				SignalRClient.Instance.RequestCommand("GetLeaderboardPosition", endlessModeLeaderboardName, playerModel.HashedId, OnDataMyRank, null, waitForResponse: true);
				_positionDic.Add(endlessModeLeaderboardName, null);
				cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.PlayerListEndless);
			}
			else if (GameManager.Instance.gameEconomyData.ConfigData.EndlessExpertClassLeaderboardSwitch)
			{
				string endlessModeLeaderboardNameByClass = Leaderboards.GetEndlessModeLeaderboardNameByClass(currentEndlessModeCalendarDefinition.Identifier, survivalButtons[i].SurvivorClass);
				SetSurvivorClassDataProvider(endlessModeLeaderboardNameByClass);
				_positionDic.Add(endlessModeLeaderboardNameByClass, null);
				cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.PlayerListEndless);
			}
			else if (!GameManager.Instance.gameEconomyData.ConfigData.EndlessExpertClassLeaderboardSwitch)
			{
				Helpers.GameObjectSetActive(tabs.GetUIButtonToggleList[i].gameObject, value: false);
			}
		}
		SignalRClient.Instance.RequestCommand("GetSurvivorClassLeaderboardPositions", currentEndlessModeCalendarDefinition.Identifier.ToString(), playerModel.HashedId, OnSurvivorClassDataMyRank, null, waitForResponse: true);
		ownLeaderBoardEntry.Type = GuildPlayerListCardBase.GuildPlayerListCardType.PlayerListEndless;
		Helpers.GameObjectSetActive(ownLeaderBoardEntry.gameObject, value: false);
	}

	private void SetDataProvider(string leaderboardName, bool isPrevious = false, bool useOnlyCachedOnly = false)
	{
		ScoreDataProvider scoreDataProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderBoard(leaderboardName);
		if (scoreDataProvider == null)
		{
			scoreDataProvider = new PlayerEndlessModeSurvivorClassScoreDataProvider(leaderboardName, 100, isPrevious, useOnlyCachedOnly);
			GameManager.Instance.CachedLeaderboardsManager.AddLeaderboard(leaderboardName, scoreDataProvider);
		}
		providers.Add(scoreDataProvider);
	}

	private void SetSurvivorClassDataProvider(string leaderboardName, bool isPrevious = false, bool useOnlyCachedOnly = false)
	{
		ScoreDataProvider scoreDataProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderBoard(leaderboardName);
		if (scoreDataProvider == null)
		{
			scoreDataProvider = new PlayerEndlessModeSurvivorClassScoreDataProvider(leaderboardName, 100, isPrevious, useOnlyCachedOnly);
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
		SetLoadingUIState(isActive: true);
		for (int i = 0; i <= 2; i++)
		{
			int num = data.Count - i - 1;
			UIListCard<ScoreDataEntry> uIListCard = ((num >= 0) ? getCardAt(num) : null);
			if (uIListCard != null)
			{
				topLeaderBoardList[i].Item = uIListCard.Item;
				topLeaderBoardList[i].UpdateUI();
				RemoveCard(uIListCard.Item);
			}
			else
			{
				topLeaderBoardList[i].SetTop3DefaultUI();
			}
			int rank = i + 1;
			GameObject target = topLeaderBoardList[i].target;
			EventDelegate.Set(topLeaderBoardList[i].button.onClick, delegate
			{
				OnClickLeaderBoardRewardToolTip(target, rank);
			});
		}
		for (int num2 = 0; num2 < cards.Count; num2++)
		{
			int rank2 = cards.Count - num2 + 3;
			EndlessModeLeaderBoardRewardEntry endlessModeLeaderBoardRewardEntry = (EndlessModeLeaderBoardRewardEntry)cards[num2];
			GameObject target2 = endlessModeLeaderBoardRewardEntry.target;
			EventDelegate.Set(endlessModeLeaderBoardRewardEntry.button.onClick, delegate
			{
				OnClickLeaderBoardRewardToolTip(target2, rank2);
			});
		}
		IEnumerable<EndlessModeLeaderBoardReward> enumerable = from n in EndlessModeHelpers.GetCurrentCycleLeaderBoardRewards().ToList()
			where n.RewardType == EndlessModeLeaderBoardRewardType.Percentage
			select n;
		ClearPercentRewardList();
		foreach (EndlessModeLeaderBoardReward item in enumerable)
		{
			GameObject gameObject = cardsContainer.AddChild(cardPrefab);
			EndlessModeLeaderBoardRewardEntry component = gameObject.GetComponent<EndlessModeLeaderBoardRewardEntry>();
			component.SetContent(item, isOwnRank: false, isLowerRank: false, isLastEntry: false, _survivorClass);
			component.Type = GuildPlayerListCardBase.GuildPlayerListCardType.PlayerListEndless;
			_percentRewardList.Add(gameObject);
		}
		cardsContainer.GetComponent<UITable>().Reposition();
		SetOwnEntryUI();
	}

	private EndlessModeLeaderBoardReward GetEndlessModeLeaderBoardReward(LeaderboardPosition leaderboardPosition)
	{
		string leaderBoardRewardSetID = GameManager.Instance.playerModel.EndlessModeManager.GetActiveEndlessMode.LeaderBoardRewardSetID;
		if (leaderboardPosition != null)
		{
			return GameManager.Instance.gameEconomyData.GetEndlessModeLeaderBoardReward(leaderBoardRewardSetID, leaderboardPosition.Position, leaderboardPosition.LeaderboardCount);
		}
		return null;
	}

	private void OnDataMyRank(string result)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(result))
		{
			Debug.LogError("OnDataMyRank failed");
			SignalRClient.Instance.ClearError();
			return;
		}
		LeaderboardPosition leaderboardPosition = GameManager.Instance.jsonSerializer.DeserializeObject<LeaderboardPosition>(result);
		if (leaderboardPosition != null)
		{
			leaderboardPosition.Position++;
			if (_positionDic.ContainsKey(leaderboardPosition.LeaderboardId))
			{
				_positionDic[leaderboardPosition.LeaderboardId] = leaderboardPosition;
			}
			if ((bool)ownLeaderBoardEntry)
			{
				Helpers.GameObjectSetActive(ownLeaderBoardEntry.gameObject, value: true);
				SetOwnEntryUI();
			}
		}
	}

	private void OnSurvivorClassDataMyRank(string result)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(result))
		{
			Debug.LogError("OnSurvivorClassDataMyRank failed");
			SignalRClient.Instance.ClearError();
			return;
		}
		List<LeaderboardPosition> list = GameManager.Instance.jsonSerializer.DeserializeObject<List<LeaderboardPosition>>(result);
		if (list == null)
		{
			return;
		}
		foreach (LeaderboardPosition item in list)
		{
			item.Position++;
			if (_positionDic.ContainsKey(item.LeaderboardId))
			{
				_positionDic[item.LeaderboardId] = item;
			}
		}
		if ((bool)ownLeaderBoardEntry)
		{
			Helpers.GameObjectSetActive(ownLeaderBoardEntry.gameObject, value: true);
			SetOwnEntryUI();
		}
	}

	protected override void OnNewTabSelected(UIButtonExtended button)
	{
		ClearPercentRewardList();
		_survivorClass = button.GetComponent<SurvivorButtonFilter>().SurvivorClass;
		SetLoadingUIState(isActive: false);
		base.OnNewTabSelected(button);
		if (_survivorClass == SurvivorClass.None)
		{
			Helpers.GameObjectSetActive(titleGroup, value: true);
			Helpers.GameObjectSetActive(myScoreGroup, value: true);
			Helpers.GameObjectSetActive(titleGroupProfessional, value: false);
			Helpers.GameObjectSetActive(myScoreGroupProfessional, value: false);
			Helpers.GameObjectSetActive(noScoreGroupProfessional, value: false);
			attemptScoreList.UpdateUI(EndlessModeGameModeType.Expert);
			return;
		}
		Helpers.GameObjectSetActive(titleGroup, value: false);
		Helpers.GameObjectSetActive(myScoreGroup, value: false);
		Helpers.GameObjectSetActive(titleGroupProfessional, value: true);
		Helpers.GameObjectSetActive(myScoreGroupProfessional, value: true);
		HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("Endless.ExpertMode.LeaderChallenge.Title", HelpersLocalization.GetSurvivorClassName(_survivorClass)));
		HelpersUI.SetContentToLabel(desLabel, LocalizationManager.GetText("Endless.ExpertMode.LeaderChallenge.Desc", HelpersLocalization.GetSurvivorClassName(_survivorClass)));
		EndlessModeAttemptData maxAttemptDataExpertBySurvivorClass = EndlessModeHelpers.GetMaxAttemptDataExpertBySurvivorClass(_survivorClass);
		if (maxAttemptDataExpertBySurvivorClass != null && maxAttemptDataExpertBySurvivorClass.SurvivorMockData != null)
		{
			Helpers.GameObjectSetActive(noScoreGroupProfessional, value: false);
			Helpers.GameObjectSetActive(myScoreGroupProfessional, value: true);
			HelpersUI.SetContentToLabel(scoreLabel, maxAttemptDataExpertBySurvivorClass.Score.ToString());
			for (int i = 0; i < teamEntries.Count; i++)
			{
				if (maxAttemptDataExpertBySurvivorClass.SurvivorMockData.Count >= i + 1)
				{
					teamEntries[i].SetTeamContent(maxAttemptDataExpertBySurvivorClass.SurvivorMockData[i]);
				}
			}
		}
		else
		{
			Helpers.GameObjectSetActive(noScoreGroupProfessional, value: true);
			Helpers.GameObjectSetActive(myScoreGroupProfessional, value: false);
		}
		attemptScoreSurvivorClassList.UpdateUI(EndlessModeGameModeType.Expert, _survivorClass);
	}

	private void ClearPercentRewardList()
	{
		for (int i = 0; i < _percentRewardList.Count; i++)
		{
			NGUITools.Destroy(_percentRewardList[i]);
		}
		_percentRewardList.Clear();
	}

	private void OnClickLeaderBoardRewardToolTip(GameObject target, int rank)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		string localisedRewardTierTitle = EndlessModeHelpers.GetLocalisedRewardTierTitle(rank);
		TooltipManager.OpenForEndlessModeReward(target, localisedRewardTierTitle, rank, TooltipManager.Prefabs.TooltipEndlessModeReward, _survivorClass);
	}

	private void SetLoadingUIState(bool isActive)
	{
		foreach (EndlessModeLeaderBoardRewardEntry topLeaderBoard in topLeaderBoardList)
		{
			Helpers.GameObjectSetActive(topLeaderBoard.gameObject, isActive);
		}
	}

	private void SetOwnEntryUI()
	{
		if (!ownLeaderBoardEntry || !ownLeaderBoardEntry.gameObject.activeSelf)
		{
			return;
		}
		EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = EndlessModeHelpers.EndlessManagerModel().CurrentEndlessModeCalendarDefinition;
		long num = 0L;
		string key;
		if (_survivorClass == SurvivorClass.None)
		{
			key = Leaderboards.GetEndlessModeLeaderboardName(currentEndlessModeCalendarDefinition.Identifier);
			num = EndlessModeHelpers.GetAllAttemptsScoreExpert();
		}
		else
		{
			key = Leaderboards.GetEndlessModeLeaderboardNameByClass(currentEndlessModeCalendarDefinition.Identifier, _survivorClass);
			num = EndlessModeHelpers.GetMaxAttemptDataExpertBySurvivorClass(_survivorClass).Score;
		}
		if (_positionDic.ContainsKey(key) && _positionDic[key] != null)
		{
			EndlessModeLeaderBoardReward endlessModeLeaderBoardReward = GetEndlessModeLeaderBoardReward(_positionDic[key]);
			if (_positionDic[key].Position <= 100)
			{
				long rank = _positionDic[key].Position;
				EndlessModeAttemptData maxAttemptDataExpertBySurvivorClass = EndlessModeHelpers.GetMaxAttemptDataExpertBySurvivorClass(_survivorClass);
				EndlessModePlayersScoreDataEntry endlessModePlayersScoreDataEntry = new EndlessModePlayersScoreDataEntry(GameManager.Instance.playerModel.Name, GameManager.Instance.playerModel.HashedId, num, 0, GameManager.Instance.playerModel.PlayerEmblem, 0L);
				if (maxAttemptDataExpertBySurvivorClass != null && maxAttemptDataExpertBySurvivorClass.SurvivorMockData != null)
				{
					endlessModePlayersScoreDataEntry.LeaderActorDefinitionId = maxAttemptDataExpertBySurvivorClass.SurvivorMockData[0].ActorDefinitionId;
				}
				ownLeaderBoardEntry.Item = endlessModePlayersScoreDataEntry;
				ownLeaderBoardEntry.UpdateUI();
				ownLeaderBoardEntry.SetRank(rank.ToString());
				GameObject target = ownLeaderBoardEntry.target;
				EventDelegate.Set(ownLeaderBoardEntry.button.onClick, delegate
				{
					OnClickLeaderBoardRewardToolTip(target, (int)rank);
				});
				if (_positionDic[key].Position == 1)
				{
					ownBoxRewardIcon.spriteName = "Ui_Icon_ChallengeCrateGold";
				}
				else if (_positionDic[key].Position == 2)
				{
					ownBoxRewardIcon.spriteName = "Ui_Icon_ChallengeCrateSilver";
				}
				else if (_positionDic[key].Position == 3)
				{
					ownBoxRewardIcon.spriteName = "Ui_Icon_ChallengeCrateBronze";
				}
				else
				{
					ownBoxRewardIcon.spriteName = "Ui_Icon_ChallengeCrate";
				}
			}
			else if (endlessModeLeaderBoardReward != null)
			{
				ownLeaderBoardEntry.SetContent(endlessModeLeaderBoardReward, isOwnRank: true, isLowerRank: false, isLastEntry: false, _survivorClass);
				ownLeaderBoardEntry.button.onClick.Clear();
			}
		}
		else
		{
			ownLeaderBoardEntry.SetDefaultUI();
		}
	}
}
