using System.Collections.Generic;

namespace BaseModel
{
	public sealed class NotificationHubSendPushRequest
	{
		public List<string> HashedIds;

		public string Message;

		public long ScheduledTimeEpochSeconds;

		public string AndroidTitle;

		public int IosBadgeNumber;
	}
}
