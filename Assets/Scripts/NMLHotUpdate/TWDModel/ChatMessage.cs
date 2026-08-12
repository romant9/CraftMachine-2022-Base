using Newtonsoft.Json;

namespace TWDModel
{
	public class ChatMessage
	{
		public string PlayerId { get; set; }

		public string GuildId { get; set; }

		public string Name { get; set; }

		public string SenderId { get; set; }

		public string SenderName { get; set; }

		public string Message { get; set; }

		public long Time { get; set; }

		public ChatNotificationType NotificationType { get; set; }

		public ChatNotificationGvgType NotificationGvGType { get; set; }

		public bool IsPinned { get; set; }

		[JsonIgnore]
		public bool IsBothTypesNone
		{
			get
			{
				if (NotificationType == ChatNotificationType.None)
				{
					return NotificationGvGType == ChatNotificationGvgType.None;
				}
				return false;
			}
		}

		[JsonIgnore]
		public string EitherTypeAsString
		{
			get
			{
				if (NotificationType != ChatNotificationType.None)
				{
					return NotificationType.ToString();
				}
				return NotificationGvGType.ToString();
			}
		}
	}
}
