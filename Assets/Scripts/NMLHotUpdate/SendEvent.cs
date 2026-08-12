using UnityEngine;

public class SendEvent : MonoBehaviour
{
	public void EventManagerNotifyEvent(EventManager.EventType eventType)
	{
		EventManager.NotifyEvent(eventType);
	}
}
