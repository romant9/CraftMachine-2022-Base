using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActiveFoundationPremiumPreviewPopup : HUDElement
{
	[SerializeField]
	private List<ActiveFoundationDayRewardEntry> UIRewards;

	[SerializeField]
	private UILabel titleLabel;

	private ActiveFoundationManager activeFoundation;

	private Rewards rewards;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public override void Open()
	{
		base.Open();
		activeFoundation = GameManager.Instance.playerModel.ActiveFoundationManager;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		DisableUIRewards();
		if (activeFoundation == null || activeFoundation.CurrentPeriodModel == null)
		{
			return;
		}
		rewards = activeFoundation.CurrentPeriodModel.PremiumExtraRewards;
		if (rewards == null)
		{
			return;
		}
		List<IReward> rewardsList = rewards.RewardsList;
		if (rewardsList != null && rewardsList.Count > 0 && UIRewards != null && UIRewards.Count > 0)
		{
			Math.Min(UIRewards.Count, rewardsList.Count);
			for (int i = 0; i < rewardsList.Count; i++)
			{
				Helpers.GameObjectSetActive(UIRewards[i], value: true);
				UIRewards[i].BindNormalPremium(rewardsList[i]);
			}
		}
	}

	public override void Close()
	{
		base.Close();
	}

	private void DisableUIRewards()
	{
		if (UIRewards != null && UIRewards.Count > 0)
		{
			for (int i = 0; i < UIRewards.Count; i++)
			{
				Helpers.GameObjectSetActive(UIRewards[i], value: false);
			}
		}
	}

	public void OnClickLock()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActiveFoundationPremiumPurchaseInfoPopup).Open();
	}
}
