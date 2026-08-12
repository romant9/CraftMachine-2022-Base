namespace TWDModel
{
	public class BattleSignUpFirstPlayerRemoteNotification : IGuildRemotePushNotification
	{
		private RemotePushData remotePushData;

		private int registeredPlayersAmount;

		private string registeredPlayerName;

		private string guildName;

		public string ConfigId => Type.ToString();

		public GuildRemotePushNotification.NotificationType Type => GuildRemotePushNotification.NotificationType.BattleSignUpFirstPlayer;

		public RemotePushData RemotePushData => remotePushData;

		public BattleSignUpFirstPlayerRemoteNotification(GuildModel guildModel, TWDModelManager manager, string playerName)
		{
			long battleSlotForTimeStamp = guildModel.GuildWarModel.GetBattleSlotForTimeStamp(guildModel.TimeStamp);
			registeredPlayersAmount = guildModel.GuildWarModel.GetRegisteredPlayersCountForBattle(battleSlotForTimeStamp);
			registeredPlayerName = playerName;
			guildName = manager.Player.ValidateStringsAgainstProfanity(guildModel.Name);
			remotePushData = new RemotePushData
			{
				ScheduledTimeEpochSeconds = 0L,
				ExtraPushInfo = battleSlotForTimeStamp
			};
		}

		public GuildRemotePushNotification.SendGroup GetSendToGroup(GameEconomyData ged)
		{
			return ged.GetRemotePushNotificationConfig(ConfigId)?.SendToGroup ?? GuildRemotePushNotification.SendGroup.AllGuildMembersEligibleForGuildWars;
		}

		public string GetMessage(GameEconomyData ged)
		{
			RemotePushNotificationConfig remotePushNotificationConfig = ged.GetRemotePushNotificationConfig(ConfigId);
			if (remotePushNotificationConfig == null)
			{
				return "Let's play Guild Wars!";
			}
			return string.Format(remotePushNotificationConfig.Message, registeredPlayerName, guildName);
		}

		public string GetAndroidTitle(GameEconomyData ged)
		{
			RemotePushNotificationConfig remotePushNotificationConfig = ged.GetRemotePushNotificationConfig(ConfigId);
			if (remotePushNotificationConfig == null)
			{
				return "Guild Wars";
			}
			return remotePushNotificationConfig.AndroidTitle;
		}

		public bool CanSend(GuildRemotePushNotification notificationManager)
		{
			if (notificationManager.ScheduledNotifications.TryGetValue((long)Type, out var value))
			{
				return value.ExtraPushInfo < remotePushData.ExtraPushInfo;
			}
			return registeredPlayersAmount == 1;
		}
	}
}
