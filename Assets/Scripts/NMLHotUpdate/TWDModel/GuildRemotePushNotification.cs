using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class GuildRemotePushNotification
	{
		public enum NotificationType
		{
			BattleSignUpFirstPlayer = 1,
			BattleStarted = 2,
			Lockdown = 3
		}

		public enum SendGroup
		{
			AllGuildMembers = 0,
			NotRegisteredForBattleMembers = 1,
			RegisteredForBattle = 2,
			RegisteredForBattleOffline = 3,
			AllGuildMembersOffline = 4,
			AllGuildMembersEligibleForGuildWars = 5,
			AllGuildMembersEligibleForGuildWarsOffline = 6
		}

		public const string DefaultMessage = "Let's play Guild Wars!";

		public const string AndroidPushNotificationTitle = "Guild Wars";

		private const string FeatureKeyword = "RemotePush";

		public const int SendPushNotificationImmediately = 0;

		public const int DayInSeconds = 86400;

		public const int DelayThreshold = 300;

		public Dictionary<long, RemotePushData> ScheduledNotifications { get; private set; }

		public GuildRemotePushNotification()
		{
			ScheduledNotifications = new Dictionary<long, RemotePushData>();
		}

		public bool RemotePushDataExists(long type)
		{
			return ScheduledNotifications.ContainsKey(type);
		}

		public bool TryGetRemotePushData(long timeslot, out RemotePushData pushData)
		{
			return ScheduledNotifications.TryGetValue(timeslot, out pushData);
		}

		private void AddRemotePushData(long timeslot, RemotePushData newPushData)
		{
			ScheduledNotifications[timeslot] = newPushData;
		}

		private bool RemovePushData(long timeslot)
		{
			return ScheduledNotifications.Remove(timeslot);
		}

		public bool TryToSendPushNotification(TWDModelManager manager, GuildModel guildModel, string senderId, IGuildRemotePushNotification notification)
		{
			bool result = false;
			if (manager.GameEconomyData.GetFeature(string.Format("{0}_{1}", "RemotePush", notification.Type.ToString())).Enabled && notification.CanSend(this))
			{
				manager.GvGLog($"GuildRemotePushNotification: Can send notification type = {notification.ConfigId} to {notification.GetSendToGroup(manager.GameEconomyData)} with message \"{notification.GetMessage(manager.GameEconomyData)}\" scheduled at {notification.RemotePushData.ScheduledTimeEpochSeconds}", guildModel);
				SendNotification(manager, guildModel, ref notification, senderId);
				AddRemotePushData(notification.RemotePushData.ExtraPushInfo, notification.RemotePushData);
				result = true;
			}
			return result;
		}

		public bool CancelRemotePushNotification(TWDModelManager manager, long timeslot, string senderId)
		{
			IServerService serverService = manager.ServerService;
			if (TryGetRemotePushData(timeslot, out var pushData))
			{
				if (serverService != null && senderId == manager.Player.HashedId && pushData.NotificationIds != null)
				{
					manager.ServerService.CancelRemotePush(pushData.NotificationIds);
				}
				manager.GvGLog($"GuildRemotePushNotification: Canceling notification timeslot = {pushData.ExtraPushInfo}, scheduled at {pushData.ScheduledTimeEpochSeconds}");
				return RemovePushData(timeslot);
			}
			return false;
		}

		public void ClearPushNotificationsData(long timeslot)
		{
			RemovePushData(timeslot);
		}

		private void SendNotification(TWDModelManager manager, GuildModel guildModel, ref IGuildRemotePushNotification notification, string senderId)
		{
			IServerService serverService = manager.ServerService;
			List<string> list = CreateSendGroup(notification.GetSendToGroup(manager.GameEconomyData), notification, guildModel);
			if (serverService == null || !(senderId == manager.Player.HashedId) || list.Count <= 0)
			{
				return;
			}
			NotificationHubSendPushRequest notificationHubSendPushRequest = new NotificationHubSendPushRequest
			{
				AndroidTitle = notification.GetAndroidTitle(manager.GameEconomyData),
				IosBadgeNumber = 1,
				Message = notification.GetMessage(manager.GameEconomyData),
				ScheduledTimeEpochSeconds = notification.RemotePushData.ScheduledTimeEpochSeconds,
				HashedIds = list
			};
			manager.GvGLog($"GuildRemotePushNotification: Sending notification type = {notification.ConfigId} to {notification.GetSendToGroup(manager.GameEconomyData)} with message \"{notification.GetMessage(manager.GameEconomyData)}\" scheduled at {notification.RemotePushData.ScheduledTimeEpochSeconds}", guildModel);
			NotificationHubSendPushResponse notificationHubSendPushResponse = manager.ServerService.SendRemotePush(notificationHubSendPushRequest);
			if (notificationHubSendPushResponse.ScheduledRemotePushes != null)
			{
				List<string> list2 = new List<string>();
				for (int i = 0; i < notificationHubSendPushResponse.ScheduledRemotePushes.Count; i++)
				{
					ScheduledRemotePush scheduledRemotePush = notificationHubSendPushResponse.ScheduledRemotePushes[i];
					list2.Add(scheduledRemotePush.AppleNotificationId);
					list2.Add(scheduledRemotePush.GoogleNotificationId);
				}
				notification.RemotePushData.NotificationIds = list2;
			}
		}

		private List<string> CreateSendGroup(SendGroup sendGroupType, IGuildRemotePushNotification notification, GuildModel guildModel)
		{
			List<string> list = new List<string>();
			switch (sendGroupType)
			{
			case SendGroup.AllGuildMembers:
			{
				List<GuildMemberInfo> guildMembers2 = guildModel.GuildMembers;
				for (int j = 0; j < guildMembers2.Count; j++)
				{
					list.Add(guildMembers2[j].MemberId);
				}
				break;
			}
			case SendGroup.RegisteredForBattle:
			{
				GuildWarModel guildWarModel4 = guildModel.GuildWarModel;
				long extraPushInfo = notification.RemotePushData.ExtraPushInfo;
				List<string> registeredPlayersForCurrentOrNextBattle2 = guildWarModel4.GetRegisteredPlayersForCurrentOrNextBattle(extraPushInfo);
				for (int l = 0; l < registeredPlayersForCurrentOrNextBattle2.Count; l++)
				{
					list.Add(registeredPlayersForCurrentOrNextBattle2[l]);
				}
				break;
			}
			case SendGroup.RegisteredForBattleOffline:
			{
				GuildWarModel guildWarModel5 = guildModel.GuildWarModel;
				long extraPushInfo2 = notification.RemotePushData.ExtraPushInfo;
				List<string> registeredPlayersForCurrentOrNextBattle3 = guildWarModel5.GetRegisteredPlayersForCurrentOrNextBattle(extraPushInfo2);
				for (int m = 0; m < registeredPlayersForCurrentOrNextBattle3.Count; m++)
				{
					GuildMemberInfo memberInfo2 = guildModel.GetMemberInfo(registeredPlayersForCurrentOrNextBattle3[m]);
					if (memberInfo2 != null && !memberInfo2.IsOnline(guildModel.TimeStamp))
					{
						list.Add(memberInfo2.MemberId);
					}
				}
				break;
			}
			case SendGroup.AllGuildMembersOffline:
			{
				List<GuildMemberInfo> guildMembers3 = guildModel.GuildMembers;
				for (int k = 0; k < guildMembers3.Count; k++)
				{
					GuildMemberInfo guildMemberInfo2 = guildMembers3[k];
					if (guildMemberInfo2 != null && !guildMemberInfo2.IsOnline(guildModel.TimeStamp))
					{
						list.Add(guildMemberInfo2.MemberId);
					}
				}
				break;
			}
			case SendGroup.NotRegisteredForBattleMembers:
			{
				GuildWarModel guildWarModel3 = guildModel.GuildWarModel;
				long battleSlotForTimeStamp3 = guildWarModel3.GetBattleSlotForTimeStamp(guildModel.TimeStamp);
				List<string> registeredPlayersForCurrentOrNextBattle = guildWarModel3.GetRegisteredPlayersForCurrentOrNextBattle(battleSlotForTimeStamp3);
				List<GuildMemberInfo> guildMembers = guildModel.GuildMembers;
				for (int i = 0; i < guildMembers.Count; i++)
				{
					GuildMemberInfo guildMemberInfo = guildMembers[i];
					if (!registeredPlayersForCurrentOrNextBattle.Contains(guildMemberInfo.MemberId))
					{
						list.Add(guildMemberInfo.MemberId);
					}
				}
				break;
			}
			case SendGroup.AllGuildMembersEligibleForGuildWars:
			{
				GuildWarModel guildWarModel2 = guildModel.GuildWarModel;
				long battleSlotForTimeStamp2 = guildWarModel2.GetBattleSlotForTimeStamp(guildModel.TimeStamp);
				guildWarModel2.GetRegisteredPlayersForCurrentOrNextBattle(battleSlotForTimeStamp2);
				foreach (KeyValuePair<string, GuildBattleParticipantInfo> item in guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot)
				{
					list.Add(item.Key);
				}
				break;
			}
			case SendGroup.AllGuildMembersEligibleForGuildWarsOffline:
			{
				GuildWarModel guildWarModel = guildModel.GuildWarModel;
				long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(guildModel.TimeStamp);
				guildWarModel.GetRegisteredPlayersForCurrentOrNextBattle(battleSlotForTimeStamp);
				foreach (KeyValuePair<string, GuildBattleParticipantInfo> item2 in guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot)
				{
					GuildMemberInfo memberInfo = guildModel.GetMemberInfo(item2.Key);
					if (memberInfo != null && !memberInfo.IsOnline(guildModel.TimeStamp))
					{
						list.Add(memberInfo.MemberId);
					}
				}
				break;
			}
			}
			return list;
		}

		public static IGuildRemotePushNotification CreateNotification(NotificationType type, TWDModelManager twdManager, GuildModel groupModel)
		{
			IGuildRemotePushNotification result = null;
			if (type != NotificationType.BattleStarted && type == NotificationType.Lockdown)
			{
				result = new LockdownRemoteNotification(twdManager, groupModel);
			}
			return result;
		}
	}
}
