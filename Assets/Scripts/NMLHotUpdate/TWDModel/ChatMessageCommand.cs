using BaseModel;

namespace TWDModel
{
	public class ChatMessageCommand : TWDSocialModelCommand
	{
		public string Message { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ChatMessageGroupCommand
			{
				Message = Message,
				SenderId = manager.Player.HashedId
			};
		}
	}
}
