using TWDModel;
using UnityEngine;

public class SurvivorInfoUnlockView : UIAnimatedObject
{
	[SerializeField]
	private UILabel heroQuoteLabel;

	[SerializeField]
	private UILabel unlockedLabel;

	[SerializeField]
	private UISprite shareRewardCurrenyIcon;

	[SerializeField]
	private UILabel shareRewardCurrenyAmount;

	[SerializeField]
	private GameObject shareRewardParent;

	[SerializeField]
	private GameObject unlockButtonsParent;

	[SerializeField]
	private GameObject unlockFade;

	private Callback introDoneCallbackInternal;

	private SurvivorModel survivor;

	private RewardCurrency reward;

	public void ShowUnlock(SurvivorModel survivorModel, Callback introDoneCallback = null, bool showFade = true)
	{
		if (survivorModel != null)
		{
			survivor = survivorModel;
			introDoneCallbackInternal = introDoneCallback;
			HelpersUI.SetContentToLabel(heroQuoteLabel, HelpersLocalization.GetHeroQuote(survivorModel.Definition));
			HelpersUI.SetContentToLabel(unlockedLabel, LocalizationManager.GetText("SurvivorInfoPopup.HeroUnlock{HeroName}", survivorModel.FullName));
			reward = GameManager.Instance.gameEconomyData.GetUnlockShareRewardForSurvivor(survivorModel.Definition);
			if (reward != null)
			{
				HelpersUI.SetSprite(shareRewardCurrenyIcon, HelpersGfx.GetCurrencyIconName(reward.CurrencyType));
				HelpersUI.SetContentToLabel(shareRewardCurrenyAmount, reward.Amount.ToString());
			}
			ShowButtons(value: true);
			Helpers.GameObjectSetActive(base.gameObject, value: true);
			Helpers.GameObjectSetActive(unlockFade, showFade);
		}
		else
		{
			Debug.LogError("SurvivorInfoUnlockView: SurvivorModel is NULL!");
		}
	}

	public void ShowButtons(bool value)
	{
		Helpers.GameObjectSetActive(unlockButtonsParent, value);
		Helpers.GameObjectSetActive(shareRewardParent, value && IsRewardActive());
	}

	public void AnimationIntroDone()
	{
		if (introDoneCallbackInternal != null)
		{
			introDoneCallbackInternal();
			introDoneCallbackInternal = null;
		}
	}

	public bool IsRewardActive()
	{
		if (GameManager.Instance.gameEconomyData.IsUnlockShareRewardEnabled())
		{
			if (survivor != null)
			{
				return survivor.UnlockShareRewardedAmount <= 0;
			}
			return false;
		}
		return false;
	}
}
