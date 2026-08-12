using System;

namespace TWDModel
{
	[Serializable]
	public class RemotePushNotificationConfig
	{
		public string Id;

		public string Message;

		public string AndroidTitle;

		public GuildRemotePushNotification.SendGroup SendToGroup;
	}
}
