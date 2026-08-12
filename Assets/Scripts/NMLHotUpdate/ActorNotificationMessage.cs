using TWDModel;

public class ActorNotificationMessage
{
	public ActorNotificationType MessageType;

	public string Message;

	public int MessageSize;

	public NotificationSound MessageSound;

	public string Icon;

	public TimedEffectType TimedEffectType;

	public int StackedNotificationCount = 1;

	public string SourceTraitIdentifier = "";

	public ActorNotificationMessage(string text, ActorNotificationType type = ActorNotificationType.Generic, int size = -1, NotificationSound sound = NotificationSound.None, string icon = "", TimedEffectType timedEffectType = TimedEffectType.None, string sourceTraitIdentifier = "")
	{
		Message = text;
		MessageSize = size;
		MessageType = type;
		MessageSound = sound;
		Icon = icon;
		TimedEffectType = timedEffectType;
		StackedNotificationCount = 1;
		SourceTraitIdentifier = sourceTraitIdentifier;
	}

	public ActorNotificationMessage(string text, string icon, NotificationSound sound = NotificationSound.None, ActorNotificationType type = ActorNotificationType.ActionNotification, TimedEffectType timedEffectType = TimedEffectType.None, string sourceTraitIdentifier = "")
	{
		Message = text;
		MessageType = type;
		MessageSound = sound;
		Icon = icon;
		TimedEffectType = timedEffectType;
		StackedNotificationCount = 1;
		SourceTraitIdentifier = sourceTraitIdentifier;
	}
}
