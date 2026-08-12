namespace TWDModel
{
	public class BattleStartedRemoteNotification : IGuildRemotePushNotification
	{
		private enum State
		{
			AvailableSpots = 0,
			FullTeam = 1,
			AvailableSpotsFullInfo = 2,
			FullTeamFullInfo = 3
		}

		private RemotePushData remotePushData;

		private int playersAmountLeft;

		private string opponentGuildName;

		private string guildName;

		private State state;

		public GuildRemotePushNotification.NotificationType Type => GuildRemotePushNotification.NotificationType.BattleStarted;

		public RemotePushData RemotePushData => remotePushData;

		public string ConfigId => $"{Type}_{state}";

		public BattleStartedRemoteNotification(TWDModelManager manager, GuildModel guildModel, long battleTimeSlot)
		{
			GuildBattleModel currentBattle = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle;
			guildName = manager.Player.ValidateStringsAgainstProfanity(guildModel.Name);
			opponentGuildName = null;
			int num = GvGModelHelper.NotificationDelayInMilliseconds(guildModel.Id, manager.GameEconomyData.GuildWarConfig.NotificationDelayInSeconds) + manager.GameEconomyData.GuildWarConfig.UniversalNotificationDelayInMilliseconds;
			long scheduledTimeEpochSeconds = (battleTimeSlot + num) / 1000;
			if (currentBattle.TimeSlot == battleTimeSlot && currentBattle.HasStarted())
			{
				state = State.FullTeamFullInfo;
				opponentGuildName = manager.Player.ValidateStringsAgainstProfanity(currentBattle.EnemyGuildName);
			}
			else
			{
				state = State.FullTeam;
			}
			remotePushData = new RemotePushData
			{
				ScheduledTimeEpochSeconds = scheduledTimeEpochSeconds,
				ExtraPushInfo = battleTimeSlot
			};
		}

		public GuildRemotePushNotification.SendGroup GetSendToGroup(GameEconomyData ged)
		{
			RemotePushNotificationConfig remotePushNotificationConfig = ged.GetRemotePushNotificationConfig(ConfigId);
			if (remotePushNotificationConfig == null)
			{
				if (state != State.FullTeamFullInfo)
				{
					return GuildRemotePushNotification.SendGroup.RegisteredForBattle;
				}
				return GuildRemotePushNotification.SendGroup.RegisteredForBattleOffline;
			}
			return remotePushNotificationConfig.SendToGroup;
		}

		public string GetMessage(GameEconomyData ged)
		{
			RemotePushNotificationConfig remotePushNotificationConfig = ged.GetRemotePushNotificationConfig(ConfigId);
			if (remotePushNotificationConfig != null)
			{
				if (state == State.FullTeam)
				{
					return remotePushNotificationConfig.Message;
				}
				if (state == State.FullTeamFullInfo)
				{
					return string.Format(remotePushNotificationConfig.Message, opponentGuildName);
				}
				if (state == State.AvailableSpots)
				{
					return string.Format(remotePushNotificationConfig.Message, playersAmountLeft);
				}
				if (state == State.AvailableSpotsFullInfo)
				{
					return string.Format(remotePushNotificationConfig.Message, playersAmountLeft, guildName, opponentGuildName);
				}
			}
			return "Let's play Guild Wars!";
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
			if (notificationManager.ScheduledNotifications.TryGetValue(remotePushData.ExtraPushInfo, out var value))
			{
				return value.ScheduledTimeEpochSeconds > remotePushData.ScheduledTimeEpochSeconds;
			}
			return true;
		}
	}
}
