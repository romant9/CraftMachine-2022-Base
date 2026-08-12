using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildRequestsPopup : HUDElement
{
	[SerializeField]
	private UILabel MembersAmountLabel;

	[SerializeField]
	private GameObject cannotReceiveRequestContainer;

	private GuildRequestsPanel RequestsList;

	private int updateContentAfterNumMessageReceived;

	public override void Open()
	{
		base.Open();
		UpdateContent();
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnEnable()
	{
		if (RequestsList == null)
		{
			RequestsList = GetComponent<GuildRequestsPanel>();
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed += OnGuildChanged;
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed -= OnGuildChanged;
		}
	}

	public void UpdateContent()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			if (MembersAmountLabel != null)
			{
				HelpersUI.SetContentToLabel(MembersAmountLabel, guildModel.NumberMembers + "/" + 20);
			}
			Helpers.GameObjectSetActive(cannotReceiveRequestContainer, !guildModel.CanReceiveRequest);
			if (RequestsList != null)
			{
				RequestsList.SetupRequestNotification();
				RequestsList.SetNotificationsCards();
			}
		}
	}

	public void OnRefuseAll()
	{
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildRefuseAllMembershipConfirmation.Title"), LocalizationManager.GetText("Popup.GuildRefuseAllMembershipConfirmation.Message"), LocalizationManager.GetText("Button.Yes"), OnRefuseAllConfirmed, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnGuildChanged(GroupModelBase model, string changed, object args)
	{
		if (changed == "MemberRefused" || changed == "MemberAccepted")
		{
			updateContentAfterNumMessageReceived--;
			if (updateContentAfterNumMessageReceived <= 0)
			{
				UpdateContent();
			}
		}
	}

	private void OnRefuseAllConfirmed()
	{
		if (RequestsList != null && RequestsList.MemberRequestList != null)
		{
			updateContentAfterNumMessageReceived = RequestsList.MemberRequestList.Count;
			for (int i = 0; i < RequestsList.MemberRequestList.Count; i++)
			{
				GameManager.Instance.GuildManager.RefuseGuildMember(RequestsList.MemberRequestList[i].MemberId);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_refuse");
		}
	}

	public void OnAcceptAll()
	{
		if (RequestsList != null && RequestsList.MemberRequestList != null)
		{
			string textId = "Popup.GuildAcceptAllMembershipConfirmation.Message";
			GuildModel guildModel = GameManager.Instance.GuildManager.Model;
			if (guildModel != null && guildModel.NumberMembers + RequestsList.MemberRequestList.Count >= 20)
			{
				textId = "Popup.GuildAcceptAllMembershipConfirmation.MessageGuildFull";
			}
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildAcceptAllMembershipConfirmation.Title"), LocalizationManager.GetText(textId), LocalizationManager.GetText("Button.Yes"), OnAcceptAllConfirmed, LocalizationManager.GetText("Button.Cancel"));
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void OnAcceptAllConfirmed()
	{
		if (!(RequestsList != null) || RequestsList.MemberRequestList == null)
		{
			return;
		}
		int num = RequestsList.MemberRequestList.Count;
		GuildModel guildModel = GameManager.Instance.GuildManager.Model;
		if (guildModel != null && guildModel.NumberMembers + RequestsList.MemberRequestList.Count >= 20)
		{
			num = 20 - guildModel.NumberMembers;
		}
		updateContentAfterNumMessageReceived = RequestsList.MemberRequestList.Count;
		for (int i = 0; i < RequestsList.MemberRequestList.Count; i++)
		{
			if (i < num)
			{
				GameManager.Instance.GuildManager.AcceptGuildMember(RequestsList.MemberRequestList[i].MemberId);
			}
			else
			{
				GameManager.Instance.GuildManager.RefuseGuildMember(RequestsList.MemberRequestList[i].MemberId);
			}
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_accept");
	}
}
