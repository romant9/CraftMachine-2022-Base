using BaseModel;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class CampaignRewardsGridPanel : MonoBehaviour
{
	[SerializeField]
	private UIGridExtended RewardsGrid;

	[SerializeField]
	private GameObject CampaignRewardItemPrefab;

	public void Init()
	{
		if (GameManager.Instance.playerModel == null)
		{
			return;
		}
		CampaignModel campaignModel = GameManager.Instance.playerModel.CampaignModel;
		if (campaignModel == null || (!campaignModel.Active && !campaignModel.CanClaimRewards && !campaignModel.isBetweenEndAndRewardTime))
		{
			return;
		}
		List<CampaignRewardItem> list = CreateRewardItems(campaignModel);
		if (!(RewardsGrid != null) || list.Count <= 0)
		{
			return;
		}
		CampaignDefinition campaignDefinition = GameManager.Instance.gameEconomyData.GetCampaignDefinition(campaignModel.Id);
		RewardsGrid.enabled = true;
		//bool isShowRewards = OfflineManager.IsLoadDataManager && showRewardsToggle;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(CampaignRewardItemPrefab, RewardsGrid.gameObject);
			if (gameObject != null)
			{
				CampaignRewardButton component = gameObject.GetComponent<CampaignRewardButton>();
				if (component != null)
				{
					component.Item = list[i];
					component.Order = i;
					component.IsMainReward = list[i].RewardsDefinition.Highlighted;
					component.CampaignTokenIcon = ((campaignDefinition != null) ? campaignDefinition.TokenIcon : "");
					component.UpdateUI();
					//if (isShowRewards)
					//{
					//	component.rewardTexture.gameObject.SetActive(!showRewardsToggle.value);
					//}
				}
			}
		}
	}

	public void Clean()
	{
		for (int i = 0; i < ((!(RewardsGrid == null)) ? RewardsGrid.transform.childCount : 0); i++)
		{
			Object.Destroy(RewardsGrid.transform.GetChild(i).gameObject);
		}
	}

	private List<CampaignRewardItem> CreateRewardItems(CampaignModel campaignModel)
	{
		List<CampaignRewardItem> list = new List<CampaignRewardItem>();
		if (campaignModel != null)
		{
			ModelList<CampaignRewardModelItem> rewards = campaignModel.Rewards;
			int num = 0;
			for (int i = 0; i < (rewards?.Count ?? 0); i++)
			{
				num = Mathf.Max(num, rewards[i].Control);
				list.Add(rewards[i]);
			}
			List<CampaignRewardsDefinition> campaignRewardsFrom = GameManager.Instance.gameEconomyData.GetCampaignRewardsFrom(campaignModel.Id, num);
			for (int j = 0; j < (campaignRewardsFrom?.Count ?? 0); j++)
			{
				CampaignRewardTarget item = new CampaignRewardTarget(campaignRewardsFrom[j]);
				list.Add(item);
			}
		}
		return list;
	}



	#region myparams
	[SerializeField]
	private UIToggle showRewardsToggle;
	#endregion

	#region mycode
	public void SwitchRewardSprites(UIToggle tg)
	{
		if (!gameObject.activeSelf || RewardsGrid == null || RewardsGrid.transform.childCount == 0) return;
		bool isOn = tg.value;
		var childs = RewardsGrid.GetChildList().Select(x => x.gameObject.GetComponent<CampaignRewardButton>());
		foreach (var child in childs)
		{
			var reward = child.Item.Reward;
			child.rewardTexture.gameObject.SetActive(!isOn);
			if (reward is RewardRandomEquipment || reward is RewardEquipment) 
			{
				child.equipmentParent.SetActive(isOn);
			}
			else if (reward is RewardEquipToken)
			{
				child.equipmentTokenParent.SetActive(isOn);
			}
			else
			{
				child.currencyParent.SetActive(isOn);
			}
		}
	}
	#endregion
}
