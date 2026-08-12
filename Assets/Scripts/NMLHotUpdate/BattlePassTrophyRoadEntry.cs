using TWDModel;
using UnityEngine;

public class BattlePassTrophyRoadEntry : MonoBehaviour
{
	[SerializeField]
	private GameObject tierReachGameObject;

	[SerializeField]
	private UILabel tierLabel;

	[SerializeField]
	private BattlePassTrophyRoadRewardEntry[] freeRewards;

	[SerializeField]
	private BattlePassTrophyRoadRewardEntry[] premiumRewards;

	private int tierIndex;

	private BattlePassModel battlePass;

	public void Bind(int tier, bool isSpecialReward)
	{
		battlePass = GameManager.Instance.playerModel.BattlePass;
		tierIndex = tier;
		tierLabel.text = (tierIndex + 1).ToString();
		SetRewards(premium: false, isSpecialReward);
		SetRewards(premium: true, isSpecialReward);
		RefreshReachState();
	}

	private void SetRewards(bool premium, bool isSpecialReward)
	{
		bool[] array = (premium ? battlePass.TierClaimInfos[tierIndex].PremiumRewardsClaimed : battlePass.TierClaimInfos[tierIndex].FreeRewardsClaimed);
		BattlePassTrophyRoadRewardEntry[] array2 = (premium ? premiumRewards : freeRewards);
		for (int i = 0; i < array2.Length; i++)
		{
			if (i < array.Length)
			{
				Helpers.GameObjectSetActive(array2[i], value: true);
				array2[i].Bind(battlePass.GetReward(tierIndex, premium, i), tierIndex, premium, i, interactable: true, isSpecialReward);
			}
			else
			{
				Helpers.GameObjectSetActive(array2[i], value: false);
			}
		}
	}

	public void RefreshPremiumClaimState()
	{
		if (battlePass.TierClaimInfos != null)
		{
			int num = Mathf.Min(battlePass.TierClaimInfos[tierIndex].PremiumRewardsClaimed.Length, premiumRewards.Length);
			for (int i = 0; i < num; i++)
			{
				premiumRewards[i].RefreshState();
			}
		}
	}

	public void RefreshReachState()
	{
		bool value = battlePass.ReachedTier >= tierIndex;
		Helpers.GameObjectSetActive(tierReachGameObject, value);
		BattlePassTrophyRoadRewardEntry[] array = freeRewards;
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
