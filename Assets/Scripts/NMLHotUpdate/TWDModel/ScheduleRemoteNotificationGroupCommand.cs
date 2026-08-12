using BaseModel;

namespace TWDModel
{
	public class ScheduleRemoteNotificationGroupCommand : TWDValidationGroupCommand
	{
		public GuildRemotePushNotification.NotificationType Type { get; private set; }

		public bool CancelBeforeTrigger { get; private set; }

		public ScheduleRemoteNotificationGroupCommand()
		{
		}

		public ScheduleRemoteNotificationGroupCommand(GuildRemotePushNotification.NotificationType type, bool cancelBeforeTrigger)
		{
			Type = type;
			CancelBeforeTrigger = cancelBeforeTrigger;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLog("ScheduleRemoteNotificationGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GuildRemotePushNotification != null && GuildRemotePushNotification.CreateNotification(Type, tWDModelManager, guildModel) == null)
			{
				manager.GvGLog("ScheduleRemoteNotificationGroupCommand: Cancel, can't send notification: " + Type);
				return TWDValidationCommandResult.Canceled;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			IGuildRemotePushNotification guildRemotePushNotification = GuildRemotePushNotification.CreateNotification(Type, tWDModelManager, guildModel);
			if (guildRemotePushNotification != null)
			{
				if (CancelBeforeTrigger)
				{
					guildModel.GuildRemotePushNotification.CancelRemotePushNotification(tWDModelManager, guildRemotePushNotification.RemotePushData.ExtraPushInfo, SenderId);
				}
				guildModel.GuildRemotePushNotification.TryToSendPushNotification(tWDModelManager, guildModel, SenderId, guildRemotePushNotification);
				return true;
			}
			return false;
		}
	}
}
