using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class EndlessModeLeaderBoardRewardList : MonoBehaviour
{
	[SerializeField]
	private GameObject rewardEntryPrefab;

	[SerializeField]
	private GameObject rewardEntryContainer;

	[SerializeField]
	private List<GameObject> Top3RankLeaderboardObjects;

	private readonly List<GameObject> rewardEntries = new List<GameObject>();

	private void OnEnable()
	{
		UpdateUI();
	}

	private void ShowLeaderboardRewardsByRanking(LeaderboardPosition leaderboardPosition)
	{
		if (this == null)
		{
			return;
		}
		List<EndlessModeLeaderBoardReward> list = EndlessModeHelpers.GetCurrentCycleLeaderBoardRewards().ToList();
		if (list.Count == 0)
		{
			return;
		}
		ClearRewardEntries();
		UITable component = rewardEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = rewardEntryContainer.GetComponentInParent<UIScrollView>();
		string leaderBoardRewardSetID = GameManager.Instance.playerModel.EndlessModeManager.GetActiveEndlessMode.LeaderBoardRewardSetID;
		EndlessModeLeaderBoardReward endlessModeLeaderBoardReward = null;
		if (leaderboardPosition != null)
		{
			endlessModeLeaderBoardReward = GameManager.Instance.gameEconomyData.GetEndlessModeLeaderBoardReward(leaderBoardRewardSetID, leaderboardPosition.Position, leaderboardPosition.LeaderboardCount);
		}
		for (int i = 0; i < 3; i++)
		{
			Top3RankLeaderboardObjects[i].SetActive(endlessModeLeaderBoardReward == list[i]);
		}
		for (int j = 3; j < list.Count; j++)
		{
			EndlessModeLeaderBoardReward endlessModeLeaderBoardReward2 = list[j];
			if (endlessModeLeaderBoardReward2 != null)
			{
				GameObject gameObject = rewardEntryContainer.AddChild(rewardEntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				_ = list.Count;
				_ = endlessModeLeaderBoardReward2 == endlessModeLeaderBoardReward;
				gameObject.TryGetComponent<EndlessModeLeaderBoardRewardEntry>(out var _);
				rewardEntries.Add(gameObject);
			}
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void ClearRewardEntries()
	{
		for (int i = 0; i < rewardEntries.Count; i++)
		{
			NGUITools.Destroy(rewardEntries[i]);
		}
		rewardEntries.Clear();
	}

	public void UpdateUI()
	{
		if (EndlessModeHelpers.GetCurrentExpertAttemptCount() > 0)
		{
			EndlessModeHelpers.GetCurrentLeaderboardPosition(ShowLeaderboardRewardsByRanking);
		}
		else
		{
			ShowLeaderboardRewardsByRanking(null);
		}
	}
}
