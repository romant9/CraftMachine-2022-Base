public class GuildWarAlertPopup : AlertPopup
{
	public static void ShowPopup(string title, string message, string okButtonLabel, Callback okCallback = null)
	{
		GuildWarAlertPopup guildWarAlertPopup = null;
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			guildWarAlertPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildWarAlertPopup) as GuildWarAlertPopup;
		}
		if (guildWarAlertPopup == null)
		{
			Debug.LogWarning("Alert popup not found!");
			return;
		}
		if (guildWarAlertPopup.IsOpen && !guildWarAlertPopup.IsClosing)
		{
			Debug.LogError("GuildWarAlertPopup popup is already open");
			return;
		}
		guildWarAlertPopup.SetContent(title, message);
		guildWarAlertPopup.SetOkButtonLabel(okButtonLabel);
		guildWarAlertPopup.SetCallbacks(okCallback);
		guildWarAlertPopup.Open();
	}
}
