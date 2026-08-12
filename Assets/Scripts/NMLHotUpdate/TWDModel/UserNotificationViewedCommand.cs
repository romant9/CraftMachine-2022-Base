using BaseModel;

namespace TWDModel
{
	public class UserNotificationViewedCommand : ModelCommand
	{
		public enum NotificationType
		{
			None = 0,
			AutoScrap = 1,
			AchievementMigration = 2,
			CombatAutoResolved = 3
		}

		public NotificationType Notification;

		public UserNotificationViewedCommand()
		{
		}

		public UserNotificationViewedCommand(NotificationType notification)
		{
			Notification = notification;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				switch (Notification)
				{
				case NotificationType.AutoScrap:
					tWDModelManager.Player.ScrappedExcessItems = false;
					break;
				case NotificationType.AchievementMigration:
					tWDModelManager.Player.MigratedAchievementRewards = null;
					break;
				case NotificationType.CombatAutoResolved:
					tWDModelManager.Player.CombatAutoResolved = false;
					break;
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
