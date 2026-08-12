using UnityEngine;

public class RewardCard : MonoBehaviour
{
	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UITexture texture;

	private IReward reward;

	public void SetReward(IReward reward)
	{
		this.reward = reward;
		if (reward == null)
		{
			return;
		}
		if (reward is RewardCurrency)
		{
			RewardCurrency rewardCurrency = reward as RewardCurrency;
			if (amountLabel != null)
			{
				amountLabel.text = "x" + rewardCurrency.Amount;
			}
			texture.gameObject.SetActive(value: false);
			icon.gameObject.SetActive(value: true);
			icon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
		}
		else if (reward is RewardSurvivorClass)
		{
			RewardSurvivorClass rewardSurvivorClass = reward as RewardSurvivorClass;
			if (amountLabel != null)
			{
				amountLabel.text = HelpersLocalization.GetSurvivorClassName(rewardSurvivorClass.SurvivorClass);
			}
			icon.gameObject.SetActive(value: false);
			texture.gameObject.SetActive(value: true);
			HelpersGfx.SetSurvivorClassMaterial(texture, rewardSurvivorClass.SurvivorClass);
		}
	}

	private void OnClick()
	{
		if (reward is RewardCurrency)
		{
			RewardCurrency rewardCurrency = reward as RewardCurrency;
			NGTooltip.Show(rewardCurrency.Amount + " x " + HelpersLocalization.GetCurrencyName(rewardCurrency.CurrencyType));
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/get_reward");
		}
		else
		{
			_ = reward is RewardSurvivorClass;
		}
	}
}
