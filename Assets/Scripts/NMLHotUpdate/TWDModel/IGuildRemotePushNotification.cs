namespace TWDModel
{
	public interface IGuildRemotePushNotification
	{
		RemotePushData RemotePushData { get; }

		string ConfigId { get; }

		GuildRemotePushNotification.NotificationType Type { get; }

		bool CanSend(GuildRemotePushNotification notificationManager);

		string GetMessage(GameEconomyData ged);

		string GetAndroidTitle(GameEconomyData ged);

		GuildRemotePushNotification.SendGroup GetSendToGroup(GameEconomyData ged);
	}
}
