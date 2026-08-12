using TWDModel;
using UnityEngine;

public class WeeklyChallengeDifficultyPopup : HUDElement
{
	[SerializeField]
	private UILabel selectLabel;

	[SerializeField]
	private UILabel lockLabel;

	[SerializeField]
	private GameObject hardLockedContainer;

	[SerializeField]
	private UIButton selectButton;

	[SerializeField]
	private UIButton selectHardButton;

	[SerializeField]
	private GameObject seBG;

	[SerializeField]
	private GameObject shBG;

	[SerializeField]
	private GameObject TipsIconContainer;

	[SerializeField]
	private GameObject TipsIconHard;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public override void Open()
	{
		base.Open();
		UpdateUi();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "UpdateWeeklyChallengeDifficultyPopup")
		{
			UpdateUi();
		}
	}

	private void UpdateUi()
	{
		Helpers.GameObjectSetActive(hardLockedContainer, !WeeklyChallengeHelper.IsApocalypticUnlocked);
		if (selectHardButton != null)
		{
			selectHardButton.isEnabled = WeeklyChallengeHelper.IsApocalypticUnlocked;
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.ChallengeNormalSwitch)
		{
			selectButton.normalSprite = "Ui_Regular_Button_Bg";
			selectButton.hoverSprite = "Ui_Regular_Button_Bg";
			selectButton.pressedSprite = "Ui_Regular_Button_Bg_Pressed";
		}
		else
		{
			selectButton.normalSprite = "Ui_Regular_Gray_Button_Bg";
			selectButton.hoverSprite = "Ui_Regular_Gray_Button_Bg";
			selectButton.pressedSprite = "Ui_Regular_Gray_Button_Pressed_Bg";
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.ApocalypticChallengeSwitch)
		{
			selectHardButton.normalSprite = "Ui_Regular_Button_Bg";
			selectHardButton.hoverSprite = "Ui_Regular_Button_Bg";
			selectHardButton.pressedSprite = "Ui_Regular_Button_Bg_Pressed";
		}
		else
		{
			selectHardButton.normalSprite = "Ui_Regular_Gray_Button_Bg";
			selectHardButton.hoverSprite = "Ui_Regular_Gray_Button_Bg";
			selectHardButton.pressedSprite = "Ui_Regular_Gray_Button_Pressed_Bg";
		}
		HelpersUI.SetContentToLabel(selectLabel, LocalizationManager.GetText(IsHaveChallengeGuildReward() ? "Popup.Challenge.ClaimButton" : "Button.Select"));
		HelpersUI.SetContentToLabel(lockLabel, LocalizationManager.GetText(WeeklyChallengeHelper.IsChallengeOngoing() ? "WeeklyChallenge.Difficulty_Apocalyptic.Lock" : "WeeklyChallenge.Difficulty_Apocalyptic.Lock.NoTime"));
		Helpers.GameObjectSetActive(TipsIconHard, WeeklyChallengeHelper.IsApocalypticUnlocked);
		Helpers.GameObjectSetActive(TipsIconContainer, value: false);
		if (Helpers.IsChallengeRewardTipsOpen())
		{
			Helpers.GameObjectSetActive(TipsIconContainer, value: true);
		}
	}

	private bool IsHaveChallengeGuildReward()
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null && weeklyChallengeModel.CanCollectRewards)
		{
			if (!GameManager.Instance.gameEconomyData.GetFeature("GuildRewardList").Enabled)
			{
				return false;
			}
			if (WeeklyChallengeHelper.GetWeeklyChallengeModel().GetRewardsPerType(LootEntryType.ChallengeGuildReward).Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void OnNormalClicked()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.ChallengeNormalSwitch)
		{
			HUDNotification.Info(LocalizationManager.GetText("Tips.ChallengeMode.SwitchOff"));
			return;
		}
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel == null)
		{
			return;
		}
		if (weeklyChallengeModel.CanCollectRewards)
		{
			if (!WeeklyChallengeRewardListPopup.TryOpenForGuildGifts())
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(weeklyChallengeModel);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_complete");
		}
		else
		{
			MissionHubNavigation.TryOpenChallengeMap();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void OnHardClicked()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.ApocalypticChallengeSwitch)
		{
			HUDNotification.Info(LocalizationManager.GetText("Tips.ChallengeMode.SwitchOff"));
			return;
		}
		MissionHubNavigation.TryOpenApocalypticChallengeMap();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp, null, createIfNotExist: false);
		if (hUDElement != null)
		{
			hUDElement.OnClickClose();
		}
	}

	public void OnInfoClicked()
	{
		if (WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel() != null)
		{
			ApocalypticWeeklyChallengeInfoPopup.TryOpenFromClick();
		}
	}
}
