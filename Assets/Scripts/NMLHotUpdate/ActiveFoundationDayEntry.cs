using TWDModel;
using UnityEngine;

public class ActiveFoundationDayEntry : MonoBehaviour
{
	[SerializeField]
	private GameObject tierReachGameObject;

	[SerializeField]
	private UILabel tierLabel;

	[SerializeField]
	private ActiveFoundationDayRewardEntry[] freeRewards;

	[SerializeField]
	private ActiveFoundationDayRewardEntry[] premiumRewards;

	private int tierIndex;

	private ActiveFoundationManager activeFoundation;

	public void Bind(int tier, bool isSpecialReward)
	{
		activeFoundation = GameManager.Instance.playerModel.ActiveFoundationManager;
		tierIndex = tier;
		tierLabel.text = (tierIndex + 1).ToString();
		SetRewards(premium: false, isSpecialReward);
		SetRewards(premium: true, isSpecialReward);
		RefreshReachState();
	}

	private void SetRewards(bool premium, bool isSpecialReward)
	{
		if (!premium)
		{
			_ = activeFoundation.CurrentPeriodModel.RewardDays[tierIndex].FreeRewardStatus;
		}
		else
		{
			_ = activeFoundation.CurrentPeriodModel.RewardDays[tierIndex].PremiumRewardStatus;
		}
		ActiveFoundationDayRewardEntry[] array = (premium ? premiumRewards : freeRewards);
		for (int i = 0; i < array.Length; i++)
		{
			int rewardCount = activeFoundation.CurrentPeriodModel.GetRewardCount(tierIndex, premium);
			if (i < rewardCount)
			{
				IReward reward = activeFoundation.CurrentPeriodModel.GetReward(tierIndex, premium, i);
				array[i].Bind(reward, tierIndex, premium, i, interactable: true, isSpecialReward);
			}
			else
			{
				Helpers.GameObjectSetActive(array[i], value: false);
			}
		}
	}

	public void RefreshReachState()
	{
		bool value = activeFoundation.CurrentPeriodModel.CurrentDay >= tierIndex + 1;
		Helpers.GameObjectSetActive(tierReachGameObject, value);
		ActiveFoundationDayRewardEntry[] array = freeRewards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshState();
		}
		array = premiumRewards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshState();
		}
	}
}
