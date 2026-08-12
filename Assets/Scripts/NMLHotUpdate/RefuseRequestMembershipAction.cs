using TWDModel;

public class RefuseRequestMembershipAction
{
	private string memberId;

	public RefuseRequestMembershipAction(string memberId)
	{
		this.memberId = memberId;
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildRefuseMembershipConfirmation.Title"), LocalizationManager.GetText("Popup.GuildRefuseMembershipConfirmation.Message"), LocalizationManager.GetText("Button.Yes"), OnRefuseRequestMembershipConfirmed, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnRefuseRequestMembershipConfirmed()
	{
		GuildMemberInfo memberPendingInfo = GameManager.Instance.guildModel.GetMemberPendingInfo(memberId);
		if (memberPendingInfo == null || memberPendingInfo.State != GuildMemberState.PendingRequest)
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.GuildRequestNotFound", "Button.Ok", null);
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		GameManager.Instance.GuildManager.RefuseGuildMember(memberId);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_refuse");
	}
}
