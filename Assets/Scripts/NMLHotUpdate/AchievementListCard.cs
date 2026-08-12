using System;
using TWDModel;
using UnityEngine;

public class AchievementListCard : UIListCard<Achievement>
{
	[SerializeField]
	private UILabel titleLabel;

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
	private UISprite timedRewardIcon;

	[SerializeField]
	private UILabel timedRewardDescription;

	[SerializeField]
	private UITexture rewardTexture;

	[SerializeField]
	private GameObject rewardContainer;

	[SerializeField]
	private GameObject incomplete;

	[SerializeField]
	private GameObject complete;

	private Rewards cachedRewards;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item is CompleteEpisodeAchievement)
		{
			string episodeTitle = HelpersLocalization.GetEpisodeTitle(((CompleteEpisodeAchievement)base.Item).EpisodeID);
			titleLabel.text = LocalizationManager.GetText(base.Item.AchievementDefinition.TitleLocalizationKey + "{Param}", episodeTitle);
			descriptionLabel.text = LocalizationManager.GetText(base.Item.AchievementDefinition.DescriptionLocalizationKey + "{Param}", episodeTitle);
		}
		else
		{
			titleLabel.text = LocalizationManager.GetText(base.Item.AchievementDefinition.TitleLocalizationKey);
			descriptionLabel.text = LocalizationManager.GetText(base.Item.AchievementDefinition.DescriptionLocalizationKey);
		}
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
		IReward reward = ((cachedRewards != null && cachedRewards.Count > 0) ? cachedRewards.GetRewardAt(0) : null);
		if (reward is RewardCurrency rewardCurrency)
		{
			rewardAmountLabel.text = rewardCurrency.Amount.ToString();
			rewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			rewardLabel.gameObject.SetActive(!base.Item.IsCompleted);
			rewardContainer.SetActive(!base.Item.RewardClaimed);
			Helpers.GameObjectSetActive(rewardTexture, value: false);
			Helpers.GameObjectSetActive(timedRewardIcon, value: false);
			Helpers.GameObjectSetActive(timedRewardDescription, value: false);
		}
		else if (reward is RewardTimedBonus rewardTimedBonus)
		{
			timedRewardIcon.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
			rewardLabel.gameObject.SetActive(!base.Item.IsCompleted);
			HelpersUI.SetContentToLabel(timedRewardDescription, HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration));
			rewardContainer.SetActive(!base.Item.RewardClaimed);
			Helpers.GameObjectSetActive(timedRewardDescription, value: true);
			Helpers.GameObjectSetActive(timedRewardIcon, value: true);
			Helpers.GameObjectSetActive(rewardTexture, value: false);
			Helpers.GameObjectSetActive(rewardIcon, value: false);
			Helpers.GameObjectSetActive(rewardAmountLabel, value: false);
		}
		else if (reward is RewardEquipment rewardEquipment)
		{
			if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
			{
				rewardAmountLabel.text = rewardEquipment.Amount.ToString();
				rewardTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
				rewardLabel.gameObject.SetActive(!base.Item.IsCompleted);
				rewardContainer.SetActive(!base.Item.RewardClaimed);
				Helpers.GameObjectSetActive(rewardIcon, value: false);
				Helpers.GameObjectSetActive(timedRewardIcon, value: false);
				Helpers.GameObjectSetActive(timedRewardDescription, value: false);
			}
			else
			{
				rewardContainer.SetActive(value: false);
			}
		}
		progressBarLabel.text = Mathf.Clamp(base.Item.GetProgressStep(), 0, base.Item.GetProgressTarget()) + " / " + base.Item.GetProgressTarget();
		progressBar.gameObject.SetActive(!base.Item.RewardClaimed && base.Item.GetProgressTarget() > 1);
		progressBar.value = (float)base.Item.GetProgress() / 100f;
		if (base.Item.RewardClaimed)
		{
			complete.SetActive(value: true);
			incomplete.SetActive(value: false);
		}
		else
		{
			complete.SetActive(value: false);
			incomplete.SetActive(base.Item.GetProgressTarget() == 1 && !base.Item.IsCompleted);
		}
		claimRewardButton.gameObject.SetActive(base.Item.IsCompleted && !base.Item.RewardClaimed);
	}

	public override int GetSortValue()
	{
		return -(base.GetSortValue() + ((!base.Item.IsCompleted) ? 1000 : 0) + (base.Item.RewardClaimed ? 2000 : 0));
	}

	public void OnClaimRewardClick()
	{
		if (!base.Item.RewardClaimed && base.Item.IsCompleted)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_claim");
			if (cachedRewards.RewardsList[0] is RewardEquipment equipment)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForConsumable(equipment, "Popup.IAPConfirm.Title.GenericReward");
			}
			else if (cachedRewards.RewardsList[0] is RewardTimedBonus timedReward)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForTimedReward(timedReward, "Popup.IAPConfirm.Title.GenericReward");
			}
			else
			{
				CampView.Instance.BuildingsHud.CreateCollectAnim(cachedRewards, claimRewardButton.gameObject);
			}
			Helpers.ExecuteCommand(new ClaimAchievementRewardCommand(base.Item.AchievementDefinitionID));
			UpdateUI();
		}
	}
}
