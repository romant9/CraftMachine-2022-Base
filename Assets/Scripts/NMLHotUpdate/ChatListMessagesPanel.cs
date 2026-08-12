using System.Collections.Generic;
using System.Linq;
using TWDModel;

public class ChatListMessagesPanel : ScrollableListPanel<ChatMessage>
{
	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		Setup();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SocialChatNewMessage" || type == "SocialChatPinnedMessage")
		{
			Setup();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/message_received");
		}
	}

	public void Setup()
	{
		if (!(GameManager.Instance != null))
		{
			return;
		}
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null && !guildModel.HasPendingRequest(GameManager.Instance.playerModel.HashedId))
		{
			if (guildModel.ChatMessages.Any((ChatMessage x) => x.IsPinned))
			{
				List<ChatMessage> chatMessagesSorted = GetChatMessagesSorted(guildModel);
				SetCards(chatMessagesSorted);
			}
			else
			{
				SetCards(guildModel.ChatMessages);
			}
			for (int num = 0; num < cards.Count; num++)
			{
				((ChatMessageCard)cards[num]).SetIndex(num);
			}
		}
	}

	private static List<ChatMessage> GetChatMessagesSorted(GuildModel currentGuild)
	{
		List<ChatMessage> list = new List<ChatMessage>();
		ChatMessage chatMessage = null;
		foreach (ChatMessage chatMessage2 in currentGuild.ChatMessages)
		{
			if (chatMessage2.IsPinned)
			{
				if (chatMessage != null)
				{
					Debug.LogError("More than one pinned message. Replacing last message");
				}
				chatMessage = chatMessage2;
			}
			else
			{
				list.Add(chatMessage2);
			}
		}
		if (chatMessage != null)
		{
			list.Add(chatMessage);
		}
		return list;
	}
}
