using UnityEngine;

public class AccountSyncPopup : ConfirmationPopup
{
	[SerializeField]
	private UILabel playerNameText;

	[SerializeField]
	private UILabel levelText;

	public void SetupWithAccount(SocialPlatform.AccountData accountData)
	{
		SetContent(LocalizationManager.GetText("Popup.GameCenter.OldGameFoundTitle"), LocalizationManager.GetText("Popup.AccountConfirmation.Text"));
		SetOkButtonLabel(LocalizationManager.GetText("Button.Yes"));
		SetCancelButtonLabel(LocalizationManager.GetText("Button.No"));
		if (accountData != null)
		{
			if (playerNameText != null)
			{
				playerNameText.text = accountData.name;
			}
			if (levelText != null)
			{
				levelText.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.AccountConfirmation.Level{Parameter}", accountData.level);
			}
		}
	}

	public override void Close()
	{
		base.Close();
		SetCallbacks();
	}
}
