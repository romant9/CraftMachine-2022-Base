using BaseModel;

namespace TWDModel
{
	public class TogglePinnedChatMessageCommand : TWDSocialModelCommand
	{
		public string SenderName { get; set; }

		public string Message { get; set; }

		public long MsgTime { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new TogglePinnedChatMessageGroupCommand(SenderName, Message, MsgTime);
		}
	}
}
