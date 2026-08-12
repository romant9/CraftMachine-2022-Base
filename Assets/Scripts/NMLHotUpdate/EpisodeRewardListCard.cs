using TWDModel;
using UnityEngine;

public class EpisodeRewardListCard : UIListCard<IReward>
{
	[SerializeField]
	private UILabel missionRewardCurrencyLabel;

	[SerializeField]
	private UISprite missionRewardCurrencyIcon;

	[SerializeField]
	private SpeedUpTitle speedUpTitle;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item is RewardCurrency rewardCurrency)
		{
			if (missionRewardCurrencyLabel != null)
			{
				missionRewardCurrencyLabel.enabled = true;
				missionRewardCurrencyLabel.text = GetFormattedCurrencyReward(rewardCurrency.CurrencyType, rewardCurrency.Amount);
			}
			if (missionRewardCurrencyIcon != null)
			{
				missionRewardCurrencyIcon.enabled = true;
				missionRewardCurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			}
			if (speedUpTitle != null)
			{
				speedUpTitle.UpdateUI(rewardCurrency);
			}
		}
	}

	private string GetFormattedCurrencyReward(CurrencyType type, int amount)
	{
		if (ComponentHelper.IsComponentCurrency(type))
		{
			return HelpersLocalization.GetComponentRewardName(type, amount);
		}
		return amount.ToString();
	}
}
