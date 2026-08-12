using System;
using TWDModel;

public class ActorNotificationElement : HUDElementFollowTarget
{
	[Tooltip("Actor Notification Element.")]
	public UILabel NotificationElement;

	[Tooltip("Notification's Icon.")]
	public UISprite NotificationIcon;

	[Tooltip("Time before the next notification.")]
	public float Time;

	[Tooltip("Lucky Icon for Notification")]
	public UISprite LuckyIcon;

	public ActorNotificationType MessageType;

	public NotificationSound MessageSound;

	public TimedEffectType TimedEffectType;

	public string SourceTraitIdentifier;

	public Action OnStartedPlaying;

	private ActorNotificationManager notificationManager;

	public bool IsPlaying { get; set; }

	public void SetManager(ActorNotificationManager manager)
	{
		notificationManager = manager;
	}

	public void OnFinished()
	{
		if (notificationManager != null)
		{
			notificationManager.RemoveNotification(this);
		}
	}

	public void OnStarted()
	{
		OnStartedPlaying?.Invoke();
	}
}
