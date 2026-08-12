using TWDModel;

public class AcceptRequestMembershipAction
{
	private string memberId;

	public AcceptRequestMembershipAction(string memberId)
	{
		this.memberId = memberId;
		GuildModel model = GameManager.Instance.GuildManager.Model;
		if (model != null && model.NumberMembers >= 20)
		{
			AlertPopup.ShowPopup("", LocalizationManager.GetText("Popup.Guild.DenyAcceptGuildFull"), LocalizationManager.GetText("Button.Ok"));
			return;
		}
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildAcceptMembershipConfirmation.Title"), LocalizationManager.GetText("Popup.GuildAcceptMembershipConfirmation.Message"), LocalizationManager.GetText("Button.Yes"), OnAcceptRequestMembershipConfirmed, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnAcceptRequestMembershipConfirmed()
	{
		GuildMemberInfo memberPendingInfo = GameManager.Instance.guildModel.GetMemberPendingInfo(memberId);
		if (memberPendingInfo == null || memberPendingInfo.State != GuildMemberState.PendingRequest)
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.GuildRequestNotFound", "Button.Ok", null);
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		GameManager.Instance.GuildManager.AcceptGuildMember(memberId);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_accept");
	}
}
