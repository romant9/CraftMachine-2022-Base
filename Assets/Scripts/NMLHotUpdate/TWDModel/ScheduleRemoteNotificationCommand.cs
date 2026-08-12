using BaseModel;

namespace TWDModel
{
	public class ScheduleRemoteNotificationCommand : TWDSocialModelCommand
	{
		public GuildRemotePushNotification.NotificationType Type { get; private set; }

		public bool CancelBeforeTrigger { get; private set; }

		public ScheduleRemoteNotificationCommand()
		{
		}

		public ScheduleRemoteNotificationCommand(GuildRemotePushNotification.NotificationType type, bool cancelBeforeTrigger)
		{
			Type = type;
			CancelBeforeTrigger = cancelBeforeTrigger;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			GuildModel guildModel = modelManager.Player.GuildModel;
			if (!modelManager.Player.IsGuildMember)
			{
				modelManager.GvGLogError("ScheduleRemoteNotificationCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (guildModel.GuildRemotePushNotification != null && GuildRemotePushNotification.CreateNotification(Type, modelManager, guildModel) == null)
			{
				modelManager.GvGLog("ScheduleRemoteNotificationGroupCommand: Skip, can't send notification: " + Type);
				return TWDModelResult.Skip;
			}
			return TWDModelResult.OK;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ScheduleRemoteNotificationGroupCommand(Type, CancelBeforeTrigger);
		}
	}
}
