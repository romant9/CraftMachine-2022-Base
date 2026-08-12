using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildRequestMenu : UIToggleContent
{
	public const int TabIndex = 3;

	[SerializeField]
	private UILabel MembersAmountLabel;

	[SerializeField]
	private GameObject cannotReceiveRequestContainer;

	private GuildRequestsPanel RequestsList;

	private int updateContentAfterNumMessageReceived;

	public override void Activate()
	{
		base.Activate();
		if (RequestsList == null)
		{
			RequestsList = GetComponent<GuildRequestsPanel>();
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed += OnGuildChanged;
		}
		UpdateContent();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (GameManager.Instance != null && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed -= OnGuildChanged;
		}
	}

	public void UpdateContent()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel == null)
		{
			return;
		}
		if (MembersAmountLabel != null)
		{
			MembersAmountLabel.text = guildModel.NumberMembers + "/" + 20;
		}
		if (guildModel.GuildMembersPending.Count <= 0)
		{
			base.GetOwningSet.OpenContentByIndex(0);
			return;
		}
		if (cannotReceiveRequestContainer != null)
		{
			cannotReceiveRequestContainer.SetActive(!guildModel.CanReceiveRequest);
		}
		if (RequestsList != null)
		{
			RequestsList.SetupRequestNotification();
			RequestsList.SetNotificationsCards();
		}
	}

	public void OnCloseRequestNotification()
	{
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
			OnCloseRequestNotification();
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
			GuildModel model = GameManager.Instance.GuildManager.Model;
			if (model != null && model.NumberMembers + RequestsList.MemberRequestList.Count >= 20)
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
		OnCloseRequestNotification();
		int num = RequestsList.MemberRequestList.Count;
		GuildModel model = GameManager.Instance.GuildManager.Model;
		if (model != null && model.NumberMembers + RequestsList.MemberRequestList.Count >= 20)
		{
			num = 20 - model.NumberMembers;
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
