using BaseModel;

namespace TWDModel
{
	public class GiveGuildGiftGroupCommand : TWDGroupCommand
	{
		public string GiftSenderName { get; set; }

		public string GiftSenderId { get; set; }

		public DropType GiftType { get; set; }

		public long ExpirationTimeMs { get; set; }

		public string Message { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			if (((GuildModel)manager.GetGroupModel(GroupId)).GiveGiftToMembers(GiftSenderId, GiftSenderName, GiftType, ExpirationTimeMs, Message) == TWDModelResult.OK)
			{
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
