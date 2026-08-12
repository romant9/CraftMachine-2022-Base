using BaseModel;

namespace TWDModel
{
	public class CheatGiveGuildGiftToSelfCommand : TWDSocialModelCommand
	{
		public string GiftSenderName { get; set; }

		public string GiftSenderId { get; set; }

		public DropType GiftType { get; set; }

		public long ExpirationTimeMs { get; set; }

		public string Message { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new CheatGiveGuildGiftToSelfGroupCommand
			{
				GiftSenderId = GiftSenderId,
				GiftSenderName = GiftSenderName,
				Message = Message,
				GiftType = GiftType,
				ExpirationTimeMs = ExpirationTimeMs
			};
		}
	}
}
