using BaseModel;

namespace TWDModel
{
	public class ChatMessageGroupCommand : TWDGroupCommand
	{
		public string Message;

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			ChatMessage chatMessage = new ChatMessage();
			chatMessage.PlayerId = SenderId;
			chatMessage.GuildId = GroupId;
			GuildMemberInfo memberInfo = guildModel.GetMemberInfo(SenderId);
			chatMessage.Name = ((memberInfo != null) ? memberInfo.Name : "");
			chatMessage.Message = Message;
			chatMessage.Time = Time;
			guildModel.AddChatMessage(chatMessage);
			if (manager is TWDModelManager { Metrics: not null } tWDModelManager)
			{
				tWDModelManager.Metrics.AddGuild(guildModel).AddMember(memberInfo).AddSendMessage(Message)
					.Send();
			}
			return this;
		}
	}
}
