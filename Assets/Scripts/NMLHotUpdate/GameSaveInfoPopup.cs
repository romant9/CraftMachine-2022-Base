using UnityEngine;

public class GameSaveInfoPopup : HUDElement
{
	private bool UsingGameCenter = true;

	[SerializeField]
	private UILabel titleText;

	[SerializeField]
	private UILabel bottomText;

	public override void Open()
	{
		base.Open();
		string text = (UsingGameCenter ? "Apple" : "Google");
		if (titleText != null)
		{
			titleText.text = LocalizationManager.GetText("Popup.GameSaveInfo.Text." + text);
		}
		if (bottomText != null)
		{
			bottomText.text = LocalizationManager.GetText("Popup.GameSaveInfo.OneGamePerAccount." + text);
		}
	}

	public override void Close()
	{
		base.Close();
		SettingsPopup settingsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SettingsPopup) as SettingsPopup;
		if (settingsPopup != null)
		{
			settingsPopup.Open();
			settingsPopup.SetHelpNotification(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
		}
	}
}
