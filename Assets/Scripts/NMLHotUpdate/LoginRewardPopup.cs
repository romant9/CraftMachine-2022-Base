using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;
using TWDModel;
using UnityEngine;

public class LoginRewardPopup : HUDElement
{
	[JsonIgnore]
	private const string SevenDayRewardLocalization = "Popup.LoginReward.Description{Reward}";

	[SerializeField]
	private List<LoginRewardCard> loginRewardCards;

	[SerializeField]
	private UILabel daySevenRewardText;

	[SerializeField]
	private UIButton claimButton;

	[SerializeField]
	private LocalizationUIUpdaterWithParams timeToNextReward;

	[SerializeField]
	private LocalizationUIUpdaterWithParams titleRewardDay7;

	public override void Open()
	{
		base.Open();
		Init();
	}

	private void OnEnable()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		if (dailyLoginCalendar != null)
		{
			dailyLoginCalendar.Changed += ModelUpdateEventHandler;
		}
	}

	private void OnDisable()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		if (dailyLoginCalendar != null)
		{
			dailyLoginCalendar.Changed -= ModelUpdateEventHandler;
		}
	}

	public void Init()
	{
		if (GameManager.Instance == null || GameManager.Instance.playerModel == null)
		{
			return;
		}
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		if (dailyLoginCalendar?.Rewards == null)
		{
			return;
		}
		ModelList<DailyLoginCampaignRewardModelItem> rewards = dailyLoginCalendar.Rewards;
		int activeDay = dailyLoginCalendar.ActiveDay;
		if (rewards.Count > 0)
		{
			for (int i = 0; i < rewards.Count; i++)
			{
				loginRewardCards[i].Item = rewards[i];
				loginRewardCards[i].UpdateUI(i + 1, activeDay + 1);
			}
		}
		UpdateDay7RewardText(rewards);
		UpdateDay7Title();
	}

	private void UpdateDay7RewardText(ModelList<DailyLoginCampaignRewardModelItem> rewardItems)
	{
		if (rewardItems[rewardItems.Count - 1].Reward is RewardCurrency rewardCurrency)
		{
			string text = GameManager.Instance.gameEconomyData.GetActorDefinitionForToken(rewardCurrency.CurrencyType)?.Name;
			HelpersUI.SetContentToLabel(daySevenRewardText, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.LoginReward.Description{Reward}", text));
		}
	}

	private void UpdateDay7Title()
	{
		string bundleTitleForIReward = HelpersLocalization.GetBundleTitleForIReward(loginRewardCards[loginRewardCards.Count - 1].Item.Reward);
		titleRewardDay7.UpdateParameters(bundleTitleForIReward);
	}

	private void LateUpdate()
	{
		UpdateTimeUntilNextRewardUnlock();
		UpdateClaimButton();
	}

	private void UpdateTimeUntilNextRewardUnlock()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		long num = dailyLoginCalendar.NextRewardTime - GameManager.Instance.playerModel.UtcTimeStamp;
		if (num > 0 && !dailyLoginCalendar.IsCompleted)
		{
			Helpers.GameObjectSetActive(timeToNextReward.transform.parent.gameObject, value: true);
			timeToNextReward.UpdateParameters(Helpers.FormatTime(num));
		}
		else
		{
			Helpers.GameObjectSetActive(timeToNextReward.transform.parent.gameObject, value: false);
		}
	}

	private void UpdateClaimButton()
	{
		DailyLoginCampaignRewardModelItem currentActiveReward = GetCurrentActiveReward();
		if (currentActiveReward == null)
		{
			HelpersUI.SetButtonState(claimButton, UIButtonColor.State.Disabled);
		}
		else
		{
			HelpersUI.SetButtonState(claimButton, currentActiveReward.Claimed ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		}
	}

	private void ModelUpdateEventHandler(ModelObject x, string y, object z)
	{
		Init();
	}

	public void OnClaimRewardClick()
	{
		DailyLoginCampaignRewardModelItem currentActiveReward = GetCurrentActiveReward();
		if (!currentActiveReward.Claimed)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_claim");
			if (Helpers.ExecuteCommand(new ClaimDailyLoginCampaignRewardCommand(currentActiveReward.ModelId)) == TWDModelResult.OK)
			{
				TriggerRewardUnlockVisualEffect(currentActiveReward);
				Init();
			}
		}
	}

	private void TriggerRewardUnlockVisualEffect(DailyLoginCampaignRewardModelItem activeReward)
	{
		BuildingsHUD buildingsHUD = BuildingsHUD.Get();
		if (buildingsHUD == null)
		{
			return;
		}
		IReward reward = activeReward.Reward;
		if (reward == null)
		{
			return;
		}
		if (!(reward is RewardCurrency rewardCurrency))
		{
			if (reward is RewardRandomEquipment || reward is RewardEquipment)
			{
				if (reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
				{
					ShowConsumableReward(rewardEquipment);
				}
				else
				{
					ShowEquipmentRewards(activeReward.LastRewardedEquipment);
				}
			}
		}
		else
		{
			DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
			buildingsHUD.CreateCollectAnim(rewardCurrency.CurrencyType, loginRewardCards[dailyLoginCalendar.ActiveDay].gameObject, rewardCurrency.Amount);
		}
	}

	private static void ShowEquipmentRewards(EquipmentItemModel lastRewardedEquipment)
	{
		if (lastRewardedEquipment != null)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForEquipment(lastRewardedEquipment, "Popup.IAPConfirm.Title.GenericReward");
			}
		}
	}

	private void ShowConsumableReward(RewardEquipment consumable)
	{
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		if (iAPConfirmPopupNew != null)
		{
			iAPConfirmPopupNew.OpenForConsumable(consumable, "Popup.IAPConfirm.Title.GenericReward");
		}
	}

	private static DailyLoginCampaignRewardModelItem GetCurrentActiveReward()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		int activeDay = dailyLoginCalendar.ActiveDay;
		ModelList<DailyLoginCampaignRewardModelItem> modelList = dailyLoginCalendar?.Rewards;
		if (modelList == null || activeDay < 0 || activeDay >= modelList.Count)
		{
			return null;
		}
		return modelList[activeDay];
	}
}
