using System;
using TWDModel;
using UnityEngine;

public class DailyQuestListCard : UIListCard<DailyQuest>
{
	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel progressBarLabel;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private UIButton claimRewardButton;

	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UILabel rewardAmountLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private GameObject rewardContainer;

	[SerializeField]
	private GameObject challengeBonusContainer;

	[SerializeField]
	private UILabel challengeBonusStarsLabel;

	[SerializeField]
	private GameObject complete;

	[SerializeField]
	private UIButton discardButton;

	private Rewards cachedRewards;

	private float EnabledTimer;

	private bool CanDiscard
	{
		get
		{
			if (!base.Item.IsCompleted && GameManager.Instance.playerModel.AchievementManager != null)
			{
				return GameManager.Instance.playerModel.AchievementManager.CanDiscardDailyQuest;
			}
			return false;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null)
		{
			return;
		}
		int result = 0;
		int.TryParse(base.Item.AchievementDefinition.Params, out result);
		descriptionLabel.text = LocalizationManager.GetText(base.Item.AchievementDefinition.DescriptionLocalizationKey + "{Param}", result);
		rewardLabel.text = LocalizationManager.GetText("Popup.Achievements.Reward");
		if (cachedRewards == null)
		{
			try
			{
				cachedRewards = base.Item.GetRewards();
			}
			catch (Exception)
			{
			}
		}
		RewardCurrency rewardCurrency = ((cachedRewards != null && cachedRewards.Count > 0) ? (cachedRewards.GetRewardAt(0) as RewardCurrency) : null);
		RewardLootEntry rewardLootEntry = ((cachedRewards != null && cachedRewards.Count > 0) ? (cachedRewards.GetRewardAt(0) as RewardLootEntry) : null);
		if (rewardCurrency != null)
		{
			rewardAmountLabel.text = rewardCurrency.Amount.ToString();
			rewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			rewardLabel.gameObject.SetActive(!base.Item.IsCompleted);
			rewardContainer.SetActive(!base.Item.RewardClaimed);
		}
		else if (rewardLootEntry != null)
		{
			rewardAmountLabel.text = "x 1";
			rewardIcon.spriteName = HelpersGfx.GetLootIconName(rewardLootEntry.DropType);
			rewardLabel.gameObject.SetActive(!base.Item.IsCompleted);
			rewardContainer.SetActive(!base.Item.RewardClaimed);
		}
		else
		{
			rewardContainer.SetActive(value: false);
		}
		progressBarLabel.text = Mathf.Clamp(base.Item.GetProgressStep(), 0, base.Item.GetProgressTarget()) + " / " + base.Item.GetProgressTarget();
		progressBar.gameObject.SetActive(!base.Item.RewardClaimed && base.Item.GetProgressTarget() > 1);
		progressBar.value = (float)base.Item.GetProgress() / 100f;
		challengeBonusContainer.gameObject.SetActive(!base.Item.IsCompleted && base.Item.ChallengeBonusStars > 0);
		if (base.Item.ChallengeBonusStars > 0)
		{
			challengeBonusStarsLabel.text = base.Item.ChallengeBonusStars.ToString();
		}
		complete.SetActive(base.Item.IsCompleted);
		claimRewardButton.gameObject.SetActive(base.Item.IsCompleted);
		discardButton.SetState((!CanDiscard) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, true);
	}

	public void Update()
	{
		if (!(EnabledTimer < 0.1f))
		{
			return;
		}
		EnabledTimer += Time.deltaTime;
		if (EnabledTimer >= 0.1f && base.Item != null && base.Item.ViewState < AchievementViewState.NewViewed)
		{
			Helpers.ExecuteCommand(new ChangeAchievementViewState(base.Item.AchievementDefinitionID, AchievementViewState.NewViewed));
			EffectSparkle[] componentsInChildren = GetComponentsInChildren<EffectSparkle>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
	}

	public void OnEnable()
	{
		EnabledTimer = 0f;
	}

	public override int GetSortValue()
	{
		return 0;
	}

	public void OnClaimRewardClick()
	{
		if (base.Item.IsCompleted)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_claim");
			CampView.Instance.BuildingsHud.CreateCollectAnim(cachedRewards, claimRewardButton.gameObject);
			Helpers.ExecuteCommand(new ClaimDailyQuestRewardCommand(base.Item.AchievementDefinitionID));
			if (GameManager.Instance.playerModel.HasLootBoxesToOpen)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.AchievementPopup);
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(GameManager.Instance.playerModel);
			}
			UpdateUI();
		}
	}

	public void OnDiscardClick()
	{
		if (CanDiscard)
		{
			Helpers.ExecuteCommand(new DiscardDailyQuestCommand(base.Item.AchievementDefinitionID));
			UpdateUI();
		}
		else
		{
			TooltipManager.OpenTextBoxWithText(discardButton.gameObject, LocalizationManager.GetText("Tooltip.DailyQuest.DiscardOnePerDay"));
		}
	}
}
