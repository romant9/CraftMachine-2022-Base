using TWDModel;
using UnityEngine;

public class SeasonChangePopup : HUDElement
{
	[SerializeField]
	private UILabel SeasonNameLabel;

	[SerializeField]
	private UILabel TierNameLabel;

	[SerializeField]
	private UILabel RewardAmountLabel;

	[SerializeField]
	private UISprite SeasonEmblemSprite;

	[SerializeField]
	private UILabel RankResetLabel;

	[SerializeField]
	private string EmblemSpritePrefix = "Ui_Emblem_Challenges_Guild_";

	public override void Open()
	{
		base.Open();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		OutpostSeason outpostSeasonById = playerModel.gameEconomyData.GetOutpostSeasonById(playerModel.PreviousOutpostSeasonId);
		OutpostTier outpostTier = ((outpostSeasonById != null) ? playerModel.gameEconomyData.GetOutpostInfluenceTier(playerModel.PreviousSeasonRankingScore, outpostSeasonById.TierSetId) : null);
		if (SeasonNameLabel != null)
		{
			string text = ((outpostSeasonById != null) ? LocalizationManager.GetText(outpostSeasonById.LocalizationKey, outpostSeasonById.Id) : "Season");
			SeasonNameLabel.text = LocalizationManager.GetText("Popup.SeasonChange.Title{SeasonName}", text);
		}
		if (TierNameLabel != null && outpostTier != null)
		{
			string text2 = LocalizationManager.GetText(outpostTier.LocalizationKey);
			TierNameLabel.text = LocalizationManager.GetText("Popup.SeasonChange.ReachedTier{TierName}", text2);
		}
		if (RewardAmountLabel != null)
		{
			Rewards rewards = outpostTier?.GetRewards();
			if (rewards != null)
			{
				int totalCurrencyRewardAmount = rewards.GetTotalCurrencyRewardAmount(CurrencyType.Outpost);
				RewardAmountLabel.text = totalCurrencyRewardAmount.ToString();
			}
			else
			{
				RewardAmountLabel.text = "0";
			}
		}
		if (SeasonEmblemSprite != null && outpostTier != null)
		{
			SeasonEmblemSprite.spriteName = EmblemSpritePrefix + outpostTier.Id;
		}
		if (RankResetLabel != null)
		{
			if (outpostTier != null && outpostTier.ResetInfluence >= 0)
			{
				RankResetLabel.gameObject.SetActive(value: true);
				RankResetLabel.text = LocalizationManager.GetText("Popup.SeasonChange.InfluenceReset{Influence}", playerModel.RankingScore.ToString());
			}
			else
			{
				RankResetLabel.gameObject.SetActive(value: false);
			}
		}
	}
}
