using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EndlessModeNormalRewardEntry : MonoBehaviour
{
	[SerializeField]
	private UILabel bracketLabel;

	[SerializeField]
	private GameObject rewardPrefab;

	[SerializeField]
	private UIButton rewardButton;

	[SerializeField]
	private GameObject rewardsContainer;

	[SerializeField]
	private GameObject greyContainer;

	[SerializeField]
	private UILabel buttonLabel;

	private List<GameObject> rewardsItems = new List<GameObject>();

	public void SetContent(EndlessModeNormalRewardDefiniton endlessModeNormalRewardDef)
	{
		ClearRewardIcons();
		SetupNormalRewards(endlessModeNormalRewardDef.Rewards);
		UpdateUI(endlessModeNormalRewardDef);
	}

	private void SetupNormalRewards(string rewards)
	{
		Rewards rewards2 = new Rewards(rewards);
		if (rewards2.RewardsList == null || rewards2.Count <= 0)
		{
			return;
		}
		UITable component = rewardsContainer.GetComponent<UITable>();
		UIScrollView componentInParent = rewardsContainer.GetComponentInParent<UIScrollView>();
		foreach (IReward rewards3 in rewards2.RewardsList)
		{
			GameObject gameObject = rewardsContainer.AddChild(rewardPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<RewardIcon>(out var component2))
			{
				component2.SetReward(rewards3);
			}
			rewardsItems.Add(gameObject);
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void ClearRewardIcons()
	{
		for (int i = 0; i < rewardsItems.Count; i++)
		{
			NGUITools.Destroy(rewardsItems[i]);
		}
		rewardsItems.Clear();
	}

	private void UpdateUI(EndlessModeNormalRewardDefiniton endlessModeNormalRewardDef)
	{
		HelpersUI.SetContentToLabel(bracketLabel, endlessModeNormalRewardDef.Score.ToString());
		if (EndlessModeHelpers.GetClaimedNormalProgressRewardIndex != null && EndlessModeHelpers.GetClaimedNormalProgressRewardIndex.Contains(endlessModeNormalRewardDef.RewardIndex))
		{
			rewardButton.isEnabled = false;
			Helpers.GameObjectSetActive(greyContainer, value: true);
			HelpersUI.SetContentToLabel(buttonLabel, LocalizationManager.GetText("Popup.BuildMenu.Claimed"));
		}
		else if (EndlessModeHelpers.GetAttemptsScoreNormal() < endlessModeNormalRewardDef.Score)
		{
			rewardButton.isEnabled = false;
			Helpers.GameObjectSetActive(greyContainer, value: false);
			HelpersUI.SetContentToLabel(buttonLabel, LocalizationManager.GetText("Popup.Challenge.ClaimButton"));
		}
		else
		{
			rewardButton.isEnabled = true;
			Helpers.GameObjectSetActive(greyContainer, value: false);
			HelpersUI.SetContentToLabel(buttonLabel, LocalizationManager.GetText("Popup.Challenge.ClaimButton"));
		}
	}
}
