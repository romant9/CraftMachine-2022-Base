using System;

namespace TWDModel
{
	public class LockdownRemoteNotification : IGuildRemotePushNotification
	{
		private enum State
		{
			MissingPlayers = 0,
			EnoughPlayers = 1
		}

		private RemotePushData remotePushData;

		private int playerRegistered;

		private int missingPlayers;

		private int minutesToStartBattle;

		private string guildName;

		private State state;

		public string ConfigId => $"{Type}_{state}";

		public GuildRemotePushNotification.NotificationType Type => GuildRemotePushNotification.NotificationType.Lockdown;

		public RemotePushData RemotePushData => remotePushData;

		public LockdownRemoteNotification(TWDModelManager manager, GuildModel guildModel)
		{
			GuildWarModel guildWarModel = guildModel.GvGSeasonModel.GuildWarModel;
			long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(guildModel.TimeStamp);
			guildName = manager.Player.ValidateStringsAgainstProfanity(guildModel.Name);
			playerRegistered = guildWarModel.GetRegisteredPlayersCountForBattle(battleSlotForTimeStamp);
			int minPlayersToStartBattle = manager.GameEconomyData.GuildWarConfig.MinPlayersToStartBattle;
			missingPlayers = Math.Max(0, minPlayersToStartBattle - playerRegistered);
			state = ((missingPlayers <= 0) ? State.EnoughPlayers : State.MissingPlayers);
			int num = 600000;
			minutesToStartBattle = num / 60000;
			long num2 = 0L;
			if (guildModel.GuildWarModel.IsBattleSlotLocked(battleSlotForTimeStamp, guildModel.TimeStamp))
			{
				num2 = 0L;
				minutesToStartBattle = (int)(battleSlotForTimeStamp - guildModel.TimeStamp) / 60000;
			}
			else
			{
				long num3 = battleSlotForTimeStamp - num;
				num2 = num3 / 1000 + 300;
				minutesToStartBattle = (int)(battleSlotForTimeStamp - num3) / 60000;
			}
			minutesToStartBattle = Math.Max(1, minutesToStartBattle);
			remotePushData = new RemotePushData
			{
				ScheduledTimeEpochSeconds = num2,
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
			if (remotePushNotificationConfig != null)
			{
				if (state == State.MissingPlayers)
				{
					return string.Format(remotePushNotificationConfig.Message, playerRegistered, missingPlayers, guildName);
				}
				if (state == State.EnoughPlayers)
				{
					return string.Format(remotePushNotificationConfig.Message, minutesToStartBattle, guildName);
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
			if (playerRegistered == 0)
			{
				return false;
			}
			if (notificationManager.ScheduledNotifications.TryGetValue((long)Type, out var value))
			{
				return value.ExtraPushInfo < remotePushData.ExtraPushInfo;
			}
			return true;
		}
	}
}
