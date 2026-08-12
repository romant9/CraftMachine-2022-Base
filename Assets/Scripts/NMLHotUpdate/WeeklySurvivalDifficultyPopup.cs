using TWDModel;
using UnityEngine;

public class WeeklySurvivalDifficultyPopup : HUDElement
{
	[SerializeField]
	private UIButton selectNormalButton;

	[SerializeField]
	private UIButton selectHardButton;

	[SerializeField]
	private UIButton selectNightmareButton;

	[SerializeField]
	private GameObject hardLockedContainer;

	[SerializeField]
	private GameObject nightmareLockedContainer;

	[SerializeField]
	private UILabel hardUnlockLevelTextLabel;

	[SerializeField]
	private UILabel nightmareUnlockLevelTextLabel;

	[SerializeField]
	private UILabel normalModeDifficultyLabel;

	[SerializeField]
	private UILabel hardModeDifficultyLabel;

	[SerializeField]
	private UILabel nightmareModeDifficultyLabel;

	[SerializeField]
	private UISprite[] normalRewardPreviewIcons;

	[SerializeField]
	private UISprite[] hardRewardPreviewIcons;

	[SerializeField]
	private UISprite[] nightmareRewardPreviewIcons;

	public const string difficultyLocalizationString = "Popup.SurvivalDifficulty.Label{Difficulty}";

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	private void UpdateButtonLevelLocking(SurvivalDifficulty difficulty, GameObject lockedContainer, UIButton button)
	{
		WeeklySurvivalModel weeklySurvivalModel = GetModel<WeeklySurvivalModel>();
		if (weeklySurvivalModel == null)
		{
			return;
		}
		bool flag = weeklySurvivalModel.IsDifficultyLocked(difficulty);
		if (lockedContainer != null)
		{
			Helpers.GameObjectSetActive(lockedContainer, flag);
		}
		if (button != null)
		{
			button.isEnabled = !flag;
		}
		if (flag)
		{
			if (difficulty == SurvivalDifficulty.Hard && hardUnlockLevelTextLabel != null)
			{
				HelpersUI.SetContentToLabel(hardUnlockLevelTextLabel, LocalizationManager.GetText("Popup.SurvivalDifficulty.HardSurvivalUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.ConfigData.SurvivalHardUnlockAtCouncilLevel));
			}
			else if (difficulty == SurvivalDifficulty.Nightmare && nightmareUnlockLevelTextLabel != null)
			{
				HelpersUI.SetContentToLabel(nightmareUnlockLevelTextLabel, LocalizationManager.GetText("Popup.SurvivalDifficulty.HardSurvivalUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.ConfigData.SurvivalNightmareUnlockAtCouncilLevel));
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UISpriteIconHelper.SetIcons(normalRewardPreviewIcons, GameManager.Instance.gameEconomyData.ConfigData.SurvivalNormalRewardPreviewIcons);
		UISpriteIconHelper.SetIcons(hardRewardPreviewIcons, GameManager.Instance.gameEconomyData.ConfigData.SurvivalHardRewardPreviewIcons);
		UISpriteIconHelper.SetIcons(nightmareRewardPreviewIcons, GameManager.Instance.gameEconomyData.ConfigData.SurvivalNightmareRewardPreviewIcons);
		UpdateButtonLevelLocking(SurvivalDifficulty.Normal, null, selectNormalButton);
		UpdateButtonLevelLocking(SurvivalDifficulty.Hard, hardLockedContainer, selectHardButton);
		UpdateButtonLevelLocking(SurvivalDifficulty.Nightmare, nightmareLockedContainer, selectNightmareButton);
		int num = SurvivalMissionDifficultyLevelHelper.CalculateResultingSurvivalMissionLevel(GameManager.Instance.gameEconomyData, 0, GameManager.Instance.playerModel.CouncilLevel, SurvivalDifficulty.Normal) / 3;
		int num2 = SurvivalMissionDifficultyLevelHelper.CalculateResultingSurvivalMissionLevel(GameManager.Instance.gameEconomyData, 0, GameManager.Instance.playerModel.CouncilLevel, SurvivalDifficulty.Hard) / 3;
		int num3 = SurvivalMissionDifficultyLevelHelper.CalculateResultingSurvivalMissionLevel(GameManager.Instance.gameEconomyData, 0, GameManager.Instance.playerModel.CouncilLevel, SurvivalDifficulty.Nightmare) / 3;
		HelpersUI.SetContentToLabel(normalModeDifficultyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SurvivalDifficulty.Label{Difficulty}", num));
		HelpersUI.SetContentToLabel(hardModeDifficultyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SurvivalDifficulty.Label{Difficulty}", num2));
		HelpersUI.SetContentToLabel(nightmareModeDifficultyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SurvivalDifficulty.Label{Difficulty}", num3));
	}

	private void DifficultySelectionClicked(SurvivalDifficulty selectedDifficulty)
	{
		WeeklySurvivalModel weeklySurvivalModel = GetModel<WeeklySurvivalModel>();
		if (weeklySurvivalModel != null && !weeklySurvivalModel.IsDifficultySelected && !weeklySurvivalModel.IsDifficultyLocked(selectedDifficulty))
		{
			TWDModelResult result = Helpers.ExecuteCommand(new SelectSurvivalDifficultyCommand
			{
				Difficulty = selectedDifficulty
			});
			OnDifficultySelectComplete(result);
		}
	}

	public void OnNormalClicked()
	{
		SurvivalConfirmationPopup.ShowPopupGetText("Popup.SurvivalDifficulty.Title.Normal", "Popup.SurvivalDifficulty.Label1.Normal", "Popup.SurvivalDifficulty.Label2.Normal", "Button.Ok", OnNormalConfirmed, SurvivalDifficulty.Normal, "Button.Cancel");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnNormalConfirmed()
	{
		DifficultySelectionClicked(SurvivalDifficulty.Normal);
	}

	public void OnHardClicked()
	{
		SurvivalConfirmationPopup.ShowPopupGetText("Popup.SurvivalDifficulty.Title.Hard", "Popup.SurvivalDifficulty.Label1.Hard", "Popup.SurvivalDifficulty.Label2.Hard", "Button.Ok", OnHardConfirmed, SurvivalDifficulty.Hard, "Button.Cancel");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnNightmareClicked()
	{
		SurvivalConfirmationPopup.ShowPopupGetText("Popup.SurvivalDifficulty.Title.Nightmare", "Popup.SurvivalDifficulty.Label1.Nightmare", "Popup.SurvivalDifficulty.Label2.Nightmare", "Button.Ok", OnNightmareConfirmed, SurvivalDifficulty.Nightmare, "Button.Cancel");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnHardConfirmed()
	{
		DifficultySelectionClicked(SurvivalDifficulty.Hard);
	}

	public void OnNightmareConfirmed()
	{
		DifficultySelectionClicked(SurvivalDifficulty.Nightmare);
	}

	private void OnDifficultySelectComplete(TWDModelResult result)
	{
		DetailMapPopUp.ReloadSurvivalMap();
		Close();
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
		if (WeeklySurvivalHelper.GetWeeklySurvivalModel() != null)
		{
			WeeklySurvivalInfoPopup.TryOpenFromClick();
		}
	}

	public override void OnBackButtonClicked()
	{
		OnClickClose();
	}

	public static void OpenWithModel(WeeklySurvivalModel model)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklySurvivalDifficulty).OpenForModel(model);
	}
}
