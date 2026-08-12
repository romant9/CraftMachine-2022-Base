using TWDModel;
using UnityEngine;

public class PinMessageConfirmationPopup : HUDElement
{
	private const string ReplaceHeaderLocKey = "Popup.Chat.PinMessage.Header.Replace";

	private const string ReplaceButtonLocKey = "Popup.Chat.PinMessage.Button.Replace";

	private const string UnpinHeaderLocKey = "Popup.Chat.PinMessage.Header.Unpin";

	private const string UnpinButtonLocKey = "Popup.Chat.PinMessage.Button.Unpin";

	[SerializeField]
	private GameObject newMessageContainer;

	[SerializeField]
	private UITable mainContainer;

	[SerializeField]
	private UILabel headerLabel;

	[SerializeField]
	private UILabel confirmationButtonLabel;

	[SerializeField]
	private ChatMessageCard oldPinnedMessage;

	[SerializeField]
	private ChatMessageCard newPinnedMessage;

	private ChatMessage currentMessage;

	public void Unpin(ChatMessage messageToUnpin)
	{
		newMessageContainer.SetActive(value: false);
		mainContainer.Reposition();
		headerLabel.text = LocalizationManager.GetText("Popup.Chat.PinMessage.Header.Unpin");
		confirmationButtonLabel.text = LocalizationManager.GetText("Popup.Chat.PinMessage.Button.Unpin");
		oldPinnedMessage.UpdateUI(messageToUnpin);
		oldPinnedMessage.HidePinIndicators();
		currentMessage = messageToUnpin;
		Open();
	}

	public void Replace(ChatMessage messageToUnpin, ChatMessage messageToPin)
	{
		headerLabel.text = LocalizationManager.GetText("Popup.Chat.PinMessage.Header.Replace");
		confirmationButtonLabel.text = LocalizationManager.GetText("Popup.Chat.PinMessage.Button.Replace");
		oldPinnedMessage.UpdateUI(messageToUnpin);
		oldPinnedMessage.HidePinIndicators();
		newPinnedMessage.UpdateUI(messageToPin);
		newPinnedMessage.HidePinIndicators();
		currentMessage = messageToPin;
		Open();
	}

	public void OnConfirmClicked()
	{
		Helpers.ExecuteCommand(new TogglePinnedChatMessageCommand
		{
			SenderName = currentMessage.SenderName,
			MsgTime = currentMessage.Time,
			Message = currentMessage.Message
		});
		Close();
	}
}
