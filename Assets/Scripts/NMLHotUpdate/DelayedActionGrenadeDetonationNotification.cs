using UnityEngine;

internal class DelayedActionGrenadeDetonationNotification : MonoBehaviour
{
	private ActorNotificationManager notificationManager;

	public void Play(string text)
	{
		notificationManager = new ActorNotificationManager(base.transform);
		notificationManager.AddNotification(new ActorNotificationMessage(text));
	}

	private void Update()
	{
		if (notificationManager == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		notificationManager.Update(Time.deltaTime);
		if (notificationManager.GetTotalPendingNotifications() == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
