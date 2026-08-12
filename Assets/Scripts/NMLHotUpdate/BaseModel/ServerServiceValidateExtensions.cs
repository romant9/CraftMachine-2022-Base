using System;

namespace BaseModel
{
	public static class ServerServiceValidateExtensions
	{
		public static void Validate(this NotificationHubSendPushRequest self)
		{
			if (self.HashedIds == null)
			{
				throw new ArgumentException("HashedIds is null");
			}
			if (self.Message == null)
			{
				throw new ArgumentException("Message is null");
			}
			if (self.IosBadgeNumber < 0)
			{
				throw new ArgumentException("IosBadgeNumber is negative");
			}
		}
	}
}
