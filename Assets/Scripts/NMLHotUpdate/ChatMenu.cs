using BaseModel;
using TWDModel;
using UnityEngine;

public class ChatMenu : UIToggleContent
{
	[SerializeField]
	private UILabel notInGuildLabel;

	[SerializeField]
	[Tooltip("This is used so we can hide the chat easily when there is no guild.")]
	private GameObject containerChatAvailable;

	[SerializeField]
	private GameObject inputContainer;

	[SerializeField]
	private int messageInputBgHeightOpened;

	[SerializeField]
	private int messageInputBgHeightClosed;

	[SerializeField]
	private UIInput messageInput;

	[SerializeField]
	private BoxCollider chatBox;

	[SerializeField]
	private UILabel chatLabel;

	private void Start()
	{
		if (messageInput != null)
		{
			messageInput.defaultText = LocalizationManager.GetText("Popup.Chat.SendInputField");
		}
		if (Helpers.IsPCPlatform() && chatLabel != null)
		{
			chatLabel.maxLineCount = 50;
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		if (GameManager.Instance != null)
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			if (guildModel != null)
			{
				guildModel.Changed += OnGuildChanged;
			}
			ShowInputField(show: false);
			Setup();
		}
	}

	public override void Activate()
	{
		base.Activate();
		UpdateLastSeenMessageTime();
	}

	public void UpdateLastSeenMessageTime()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			long lastReadChatTime = GameManager.Instance.playerModel.LastReadChatTime;
			long lastChatTime = guildModel.GetLastChatTime();
			if (lastReadChatTime < lastChatTime || lastReadChatTime > guildModel.LifeTime)
			{
				Helpers.ExecuteCommand(new SetChatReadCommand(GameManager.Instance.playerModel)
				{
					ReadTime = lastChatTime
				});
				UIEvent.Send("SocialChatRead");
			}
		}
	}

	private void OnGuildChanged(GroupModelBase model, string changed, object args)
	{
		if (changed == "MessageAdded" || changed == "MessagesTruncated")
		{
			UIEvent.Send("SocialChatNewMessage");
		}
		if (changed == "PinnedChatMessaged")
		{
			UIEvent.Send("SocialChatPinnedMessage");
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		if (GameManager.Instance != null)
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			if (guildModel != null)
			{
				guildModel.Changed -= OnGuildChanged;
			}
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SocialGuildPlayerChanged")
		{
			Setup();
		}
		if (type == "SocialChatNewMessage")
		{
			UpdateLastSeenMessageTime();
		}
	}

	public void Setup()
	{
		bool flag = false;
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null && guildModel.GetMemberInfo(GameManager.Instance.playerModel.HashedId) != null)
		{
			flag = true;
		}
		notInGuildLabel.gameObject.SetActive(!flag);
		containerChatAvailable.gameObject.SetActive(flag);
	}

	public void SendMeseesageToChat()
	{
		string message = messageInput.value.Trim();
		if (OfflineManager.IsLoadDataManager)
		{
			if (!string.IsNullOrEmpty(message))
			{
				ChatMessage chatMessage = new ChatMessage();
				chatMessage.PlayerId = GameManager.Instance.playerModel.HashedId;
				chatMessage.GuildId = GameManager.Instance.playerModel.GuildId;
				chatMessage.Name = GameManager.Instance.playerModel.Name;
				chatMessage.Message = message;
				chatMessage.Time = GameManager.Instance.playerModel.UtcTimeStamp;
				GameManager.Instance.guildModel.ChatMessages.Add(chatMessage);
				Helpers.ExecuteCommand(new ChatMessageCommand { Message = message });
				UIEvent.Send("SocialChatNewMessage");
			}
		}
		else
		{
			GameManager.Instance.GuildManager.SendChatMessage(message);
		}
		messageInput.value = "";
		ShowInputField(show: false);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/message_send");
	}

	public void OnClickInputSmallField()
	{
		ShowInputField(show: true);
		if (messageInput != null)
		{
			messageInput.isSelected = true;
		}
	}

	public void ShowInputField(bool show)
	{
		messageInput.GetComponent<UISprite>().height = (show ? messageInputBgHeightOpened : messageInputBgHeightClosed);
		inputContainer.SetActive(show);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && UICamera.selectedObject != messageInput.gameObject)
		{
			ShowInputField(show: false);
		}
	}

	public void Fit()
	{
		if (Helpers.IsPCPlatform() && chatLabel != null && chatBox != null)
		{
			Vector2 localSize = chatLabel.localSize;
			Vector4 drawingDimensions = chatLabel.drawingDimensions;
			float num = (drawingDimensions.x + drawingDimensions.z) * 0.5f;
			float num2 = (drawingDimensions.y + drawingDimensions.w) * 0.5f;
			chatBox.center = new Vector3(chatLabel.transform.localPosition.x + num, chatLabel.transform.localPosition.y + num2, 0f);
			chatBox.size = new Vector3(localSize.x, localSize.y, 0f);
		}
	}



	#region myparams
	//private bool IsAddMessage;
	//private string LastMessage;
	//rivate long LastTime;
	private ChatMessage LastMessage;
	#endregion
}
