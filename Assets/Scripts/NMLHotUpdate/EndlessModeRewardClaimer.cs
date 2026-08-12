using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class EndlessModeRewardClaimer : MonoBehaviour
{
	[SerializeField]
	private GameObject claimButtonContainer;

	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private UILabel timerLabel;

	private const string LocalizationKeyTimer = "Endless.RewardsIn{Time}";

	private float lastTimeClaimButtonWasClicked;

	private EndlessModeManagerModel endlessManagerModel;

	private List<IReward> _rewardList;

	private void OnEnable()
	{
		UpdateUI();
	}

	private void Update()
	{
		if (timerLabel.gameObject.activeInHierarchy && endlessManagerModel != null)
		{
			string text = Helpers.FormatTimeNoZero(endlessManagerModel.GetMillisecondsUntilRewardsCanBeClaimed());
			string text2 = LocalizationManager.GetText("Endless.RewardsIn{Time}", text);
			timerLabel.text = text2;
		}
	}

	private void UpdateUI()
	{
		endlessManagerModel = EndlessModeHelpers.EndlessManagerModel();
		bool active = endlessManagerModel.DoWeHaveRewardsUnclaimed() || endlessManagerModel.DoWeHaveSurvivorClassRewardsUnclaimed();
		claimButtonContainer.SetActive(active);
		timerContainer.SetActive(endlessManagerModel.AreWeInLockdownTimerBeforeRewardsAreGiven());
	}

	public void OnClaimButtonClicked()
	{
		if (!(Time.realtimeSinceStartup < lastTimeClaimButtonWasClicked + 1f))
		{
			lastTimeClaimButtonWasClicked = Time.realtimeSinceStartup;
			if (endlessManagerModel.DoWeHaveRewardsUnclaimed())
			{
				EndlessModeHelpers.GetPreviousLeaderboardPosition(OnLeaderboardPositionLoadedHandler);
			}
			if (endlessManagerModel.DoWeHaveSurvivorClassRewardsUnclaimed())
			{
				RequestSurvivorClassLeaderBoardPosition();
			}
		}
	}

	private void OnLeaderboardPositionLoadedHandler(LeaderboardPosition leaderboardPosition)
	{
		if (leaderboardPosition == null || this == null)
		{
			return;
		}
		ClaimLeaderBoardRewardCommand claimLeaderBoardRewardCommand = new ClaimLeaderBoardRewardCommand
		{
			LeaderBoardPosition = leaderboardPosition.Position,
			LeaderBoardEntryCount = leaderboardPosition.LeaderboardCount
		};
		if (Helpers.ExecuteCommand(claimLeaderBoardRewardCommand) == TWDModelResult.OK)
		{
			if (_rewardList == null)
			{
				_rewardList = claimLeaderBoardRewardCommand.Rewards.RewardsList;
			}
			else
			{
				_rewardList.AddRange(claimLeaderBoardRewardCommand.Rewards.RewardsList);
			}
			if (!endlessManagerModel.DoWeHaveSurvivorClassRewardsUnclaimed())
			{
				ShowRewardPopUp();
			}
		}
	}

	private void ShowRewardPopUp()
	{
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		if (iAPConfirmPopupNew != null && _rewardList != null)
		{
			iAPConfirmPopupNew.OpenForRewards(_rewardList);
			iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			_rewardList = null;
		}
		UpdateUI();
	}

	private void RequestSurvivorClassLeaderBoardPosition()
	{
		int num = EndlessModeHelpers.EndlessManagerModel().CurrentEndlessModeCalendarDefinition.Identifier - 1;
		SignalRClient.Instance.RequestCommand("GetSurvivorClassLeaderboardPositions", num.ToString(), GameManager.Instance.playerModel.HashedId, OnDataMyRank, null, waitForResponse: true);
	}

	private void OnDataMyRank(string result)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(result))
		{
			Debug.LogError("OnDataMyRank failed");
			SignalRClient.Instance.ClearError();
			return;
		}
		List<LeaderboardPosition> list = GameManager.Instance.jsonSerializer.DeserializeObject<List<LeaderboardPosition>>(result);
		if (list != null && list.Count > 0)
		{
			ExecuteSurvivorClassCommand(list);
		}
	}

	private void ExecuteSurvivorClassCommand(List<LeaderboardPosition> positionList)
	{
		ClaimSurvivorClassLeaderBoardRewardCommand claimSurvivorClassLeaderBoardRewardCommand = new ClaimSurvivorClassLeaderBoardRewardCommand();
		List<SurvivorClassLeaderboardInfo> list = new List<SurvivorClassLeaderboardInfo>();
		foreach (LeaderboardPosition position in positionList)
		{
			SurvivorClassLeaderboardInfo survivorClassLeaderboardInfo = new SurvivorClassLeaderboardInfo();
			survivorClassLeaderboardInfo.SurvivorClass = GetSurvivorClassFromLeaderboardName(position.LeaderboardId);
			survivorClassLeaderboardInfo.LeaderBoardPosition = position.Position + 1;
			survivorClassLeaderboardInfo.LeaderBoardEntryCount = position.LeaderboardCount;
			list.Add(survivorClassLeaderboardInfo);
		}
		claimSurvivorClassLeaderBoardRewardCommand.SurvivorClassLeaderboardInfos = list;
		if (Helpers.ExecuteCommand(claimSurvivorClassLeaderBoardRewardCommand) == TWDModelResult.OK)
		{
			if (_rewardList == null)
			{
				_rewardList = claimSurvivorClassLeaderBoardRewardCommand.Rewards.RewardsList;
			}
			else
			{
				_rewardList.AddRange(claimSurvivorClassLeaderBoardRewardCommand.Rewards.RewardsList);
			}
			if (!endlessManagerModel.DoWeHaveRewardsUnclaimed())
			{
				ShowRewardPopUp();
			}
		}
	}

	public static SurvivorClass GetSurvivorClassFromLeaderboardName(string leaderboardName)
	{
		string[] array = leaderboardName.Split('_');
		if (array.Length < 2)
		{
			return SurvivorClass.None;
		}
		if (Enum.TryParse<SurvivorClass>(array[^1], out var result))
		{
			return result;
		}
		return SurvivorClass.None;
	}
}
