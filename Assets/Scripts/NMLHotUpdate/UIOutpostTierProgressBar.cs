using TWDModel;
using UnityEngine;

public class UIOutpostTierProgressBar : UIProgressBarExtended
{
	private OutpostSeason outpostSeason;

	private OutpostTier outpostTier;

	public override void OnEnable()
	{
		base.OnEnable();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null)
		{
			outpostSeason = GameManager.Instance.gameEconomyData.GetOutpostSeasonById(playerModel.CurrentOutpostSeasonId);
			if (outpostSeason != null)
			{
				outpostTier = GameManager.Instance.gameEconomyData.GetOutpostInfluenceTier(GameManager.Instance.playerModel.RankingScore, outpostSeason.TierSetId);
			}
			if (outpostTier != null)
			{
				HelpersUI.SetSprite(progressBarSprite, HelpersGfx.GetOutpostTierIconName(outpostTier.Id));
				num = outpostTier.MinInfluence;
				num2 = outpostTier.MaxInfluence;
				num3 = playerModel.RankingScore;
				if (progressBar != null)
				{
					progressBar.value = Mathf.InverseLerp(num, num2, num3);
				}
				if (progressBarLabel != null)
				{
					HelpersUI.SetContentToLabel(progressBarLabel, num3 + "/" + (num2 + 1));
				}
			}
		}
		Helpers.GameObjectSetActive(progressBar, playerModel != null && outpostSeason != null && outpostTier != null && outpostTier.TierType == TierType.InfluenceTier);
	}
}
