using TWDModel;
using UnityEngine;

public class EnterNamePopup : HUDElement
{
	public delegate void Callback(UIType popupType);

	[SerializeField]
	private UIInput nameInput;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel subtitle;

	[SerializeField]
	private UILabel message;

	public Callback OnSubmitCallback { get; set; }

	public Callback OnCancelCallback { get; set; }

	public UIType PopupToOpenOnConfirm { get; set; }

	public bool CanStartSocial => true;

	public override void Open()
	{
		base.Open();
		string value = GameManager.Instance.playerModel.Name;
		bool num = !string.IsNullOrEmpty(value);
		if (!num)
		{
			nameInput.value = value;
		}
		string textId = (num ? "Popup.EnterName.TitleRename" : "Popup.EnterName.Title");
		string textId2 = (num ? "Popup.EnterName.SubtitleRename" : "Popup.EnterName.Subtitle");
		string textId3 = (num ? "Popup.EnterName.MessageRename" : "Popup.EnterName.Message");
		if (title != null)
		{
			title.text = LocalizationManager.GetText(textId);
		}
		if (subtitle != null)
		{
			subtitle.text = LocalizationManager.GetText(textId2);
		}
		if (message != null)
		{
			message.text = LocalizationManager.GetText(textId3);
		}
		nameInput.characterLimit = 15;
		nameInput.defaultText = LocalizationManager.GetText("Popup.CreateGuild.EnterName");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_radiotent");
	}

	public override void OnClickClose()
	{
		if (OnCancelCallback != null)
		{
			OnCancelCallback(UIType.None);
			OnCancelCallback = null;
		}
		if (OnSubmitCallback != null)
		{
			OnSubmitCallback = null;
		}
		base.Close();
	}

	public void OnOkClicked()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		string name = nameInput.value.Trim();
		if (!playerModel.IsValidNameCharacters(name))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.EnterName.InvalidName"));
			return;
		}
		if (!playerModel.IsValidNameLength(name))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.EnterName.InvalidLength{Min}{Max}", 3, 15));
			return;
		}
		ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Yes"));
		confirmationPopup.SetCancelButtonLabel(LocalizationManager.GetText("Button.No"));
		bool flag = !string.IsNullOrEmpty(playerModel.Name);
		confirmationPopup.SetContent(LocalizationManager.GetText("Popup.AccountConfirmation.AdditionalCheck.Title"), flag ? LocalizationManager.GetText("Popup.EnterName.ConfirmationRename{Username}", name) : LocalizationManager.GetText("Popup.EnterName.Confirmation{Username}", name), useCustomColor: true);
		confirmationPopup.SetCallbacks(delegate
		{
			Helpers.ExecuteCommand(new SetPlayerNameCommand
			{
				Name = name
			});
			if (playerModel.IsGuildMember)
			{
				Helpers.ExecuteCommand(new UpdateMemberInfoCommand());
			}
			SingularityMonoBehaviour<SDKManager>.Instance.SetUserName(name);
			Close();
			if (OnSubmitCallback != null)
			{
				OnSubmitCallback(PopupToOpenOnConfirm);
				OnSubmitCallback = null;
			}
			if (OnCancelCallback != null)
			{
				OnCancelCallback = null;
			}
			PopupToOpenOnConfirm = UIType.None;
		}, delegate
		{
		});
		confirmationPopup.Open();
	}
}
