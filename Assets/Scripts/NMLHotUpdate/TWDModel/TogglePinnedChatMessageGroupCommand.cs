using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class TogglePinnedChatMessageGroupCommand : TWDGroupCommand
	{
		public string SenderName { get; set; }

		public string Message { get; set; }

		public long MessageTime { get; set; }

		public TogglePinnedChatMessageGroupCommand()
		{
		}

		public TogglePinnedChatMessageGroupCommand(string senderName, string message, long messageTime)
		{
			SenderName = senderName;
			Message = message;
			MessageTime = messageTime;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			ChatMessage chatMessage = guildModel.ChatMessages.FirstOrDefault((ChatMessage x) => x.SenderName == SenderName && x.Message == Message);
			if (chatMessage == null)
			{
				return this;
			}
			if (chatMessage.IsPinned)
			{
				chatMessage.IsPinned = false;
			}
			else
			{
				ChatMessage chatMessage2 = guildModel.ChatMessages.FirstOrDefault((ChatMessage x) => x.IsPinned);
				if (chatMessage2 != null)
				{
					chatMessage2.IsPinned = false;
				}
				chatMessage.IsPinned = true;
			}
			guildModel.NotifyChange("PinnedChatMessaged");
			return this;
		}
	}
}
