using TWDModel;
using UnityEngine;

public class WorldBossDifficultyConfirmPopup : ConfirmationPopup
{
	[SerializeField]
	private UISprite difficultyIcon;

	[SerializeField]
	private UILabel difficultyLabel;

	[SerializeField]
	private UIButton cancelButton;

	public void Setup(string title, string message, Callback onConfirm, Callback onCancel, int difficulty)
	{
		cancelButton.gameObject.SetActive(value: false);
		SetContent(title, message);
		TextOnlyResize();
		SetOkButtonLabel("确认");
		SetCancelButtonLabel("取消");
		SetCallbacks(onConfirm, onCancel);
		UpdateDifficultyVisuals(difficulty);
	}

	private void UpdateDifficultyVisuals(int difficulty)
	{
		int valueOrDefault = (GameManager.Instance?.playerModel?.WorldBossModelManager?.GetCurrentSeasonId()).GetValueOrDefault();
		WorldBossDifficultyDefinition worldBossDifficultyDefinition = GameManager.Instance?.gameEconomyData?.FindWorldBossDifficultyDefinition(valueOrDefault, difficulty);
		if (worldBossDifficultyDefinition != null)
		{
			if (difficultyIcon != null)
			{
				difficultyIcon.spriteName = "UI_WB_Diffic_" + worldBossDifficultyDefinition.DifficultyClass;
			}
			if (difficultyLabel != null)
			{
				string text = LocalizationManager.GetText(worldBossDifficultyDefinition.Localization);
				HelpersUI.SetContentToLabel(difficultyLabel, text);
			}
		}
	}
}
