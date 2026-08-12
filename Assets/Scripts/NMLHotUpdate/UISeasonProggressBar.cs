using TWDModel;
using UnityEngine;

public class UISeasonProggressBar : UIProgressBarExtended
{
	public UISprite HeroIcon;

	[SerializeField]
	private GameObject tooltipParent;

	[SerializeField]
	private GameObject dividerContainer;

	[SerializeField]
	private GameObject completedContainer;

	[SerializeField]
	private UILabel rewardLabel;

	private SeasonDefinition currentSeason;

	public override void OnEnable()
	{
		base.OnEnable();
		UpdateUI();
	}

	public void SetSeason(SeasonDefinition season)
	{
		currentSeason = season;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		CurrencyType currencyType = ((currentSeason != null) ? currentSeason.RewardCurrency : CurrencyType.None);
		if (!GameManager.Instance.gameEconomyData.IsHeroToken(currencyType))
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		string seasonTitle = HelpersLocalization.GetSeasonTitle(currentSeason.Id);
		HelpersUI.SetContentToLabel(rewardLabel, LocalizationManager.GetText("Popup.MissionHub.SpecialRewardDescription", seasonTitle));
		int value = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
		int heroUnlockCost = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCost(currencyType);
		string heroId = SurvivorToken.GetHeroId(currencyType);
		bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId);
		HeroIcon.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
		if (progressBar != null)
		{
			progressBar.value = Mathf.InverseLerp(0f, heroUnlockCost, value);
		}
		if (progressBarLabel != null)
		{
			if (flag)
			{
				base.gameObject.SetActive(value: false);
			}
			else if (value < heroUnlockCost)
			{
				Helpers.GameObjectSetActive(dividerContainer, value: true);
				Helpers.GameObjectSetActive(completedContainer, value: false);
				progressBarLabel.enabled = true;
				HelpersUI.SetContentToLabel(progressBarLabel, value + "/" + heroUnlockCost);
			}
			else
			{
				Helpers.GameObjectSetActive(dividerContainer, value: false);
				Helpers.GameObjectSetActive(completedContainer, value: true);
				progressBarLabel.enabled = false;
			}
		}
	}

	public void OnClick()
	{
		if (!(tooltipParent != null))
		{
			return;
		}
		CurrencyType currencyType = ((currentSeason != null) ? currentSeason.RewardCurrency : CurrencyType.None);
		if (currencyType != CurrencyType.None)
		{
			int value = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
			int heroUnlockCost = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCost(currencyType);
			if (value < heroUnlockCost)
			{
				TooltipManager.OpenTextBoxWithText(tooltipParent, LocalizationManager.GetText("Season.Progressbar.Tooltip"));
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(tooltipParent, LocalizationManager.GetText("Season.Progressbar.Tooltip.Unlocked"));
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
	}
}
