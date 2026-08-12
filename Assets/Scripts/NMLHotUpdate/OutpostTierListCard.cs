using TWDModel;
using UnityEngine;

public class OutpostTierListCard : UIListCard<OutpostTier>
{
	[SerializeField]
	private GameObject resetTargetGO;

	[SerializeField]
	private UILabel resetTargetLabel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel tierLimitLabel;

	[SerializeField]
	private UILabel rewardAmountLabel;

	[SerializeField]
	private GameObject activeTierGameObject;

	[SerializeField]
	private UILabel activeTierInfoLabel;

	[SerializeField]
	private UISprite tierEmblemSprite;

	[SerializeField]
	private string EmblemSpritePrefix = "Ui_Emblem_Challenges_Guild_";

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (tierEmblemSprite != null)
		{
			tierEmblemSprite.spriteName = EmblemSpritePrefix + base.Item.Id;
		}
		if (tierLimitLabel != null)
		{
			if (base.Item.TierType == TierType.InfluenceTier)
			{
				tierLimitLabel.text = base.Item.MinInfluence + "+";
			}
			else if (base.Item.TierType == TierType.RankTier)
			{
				tierLimitLabel.text = "";
			}
		}
		if (resetTargetGO != null)
		{
			resetTargetGO.SetActive(base.Item.ResetInfluence >= 0);
		}
		if (resetTargetLabel != null)
		{
			resetTargetLabel.text = base.Item.ResetInfluence.ToString();
		}
		if (rewardAmountLabel != null)
		{
			Rewards rewards = base.Item.GetRewards();
			if (rewards != null)
			{
				rewardAmountLabel.text = rewards.GetTotalCurrencyRewardAmount(CurrencyType.Outpost).ToString();
			}
		}
		if (nameLabel != null)
		{
			nameLabel.text = LocalizationManager.GetText(base.Item.LocalizationKey);
		}
		if (!(activeTierGameObject != null))
		{
			return;
		}
		bool flag = false;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		OutpostSeason outpostSeasonById = GameManager.Instance.gameEconomyData.GetOutpostSeasonById(playerModel.CurrentOutpostSeasonId);
		if (outpostSeasonById != null && GameManager.Instance.gameEconomyData.GetOutpostInfluenceTier(GameManager.Instance.playerModel.RankingScore, outpostSeasonById.TierSetId) == base.Item)
		{
			flag = true;
		}
		activeTierGameObject.SetActive(flag);
		if (flag && activeTierInfoLabel != null)
		{
			if (base.Item.TierType == TierType.InfluenceTier)
			{
				activeTierInfoLabel.text = playerModel.RankingScore.ToString();
			}
			else if (base.Item.TierType == TierType.RankTier)
			{
				activeTierInfoLabel.text = "";
			}
		}
	}

	public void OnClick()
	{
	}
}
