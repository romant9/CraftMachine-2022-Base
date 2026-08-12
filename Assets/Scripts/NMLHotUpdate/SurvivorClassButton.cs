using UnityEngine;

public class SurvivorClassButton : MonoBehaviour
{
	[SerializeField]
	private GameObject notificationObject;

	[SerializeField]
	private GameObject notificationTokenObject;

	[SerializeField]
	private UILabel notificationCounterLabel;

	private int notificationCount;

	private int notificationTokenCount;

	public int NotificationCount
	{
		get
		{
			return notificationCount;
		}
		set
		{
			notificationCount = value;
			if (notificationCounterLabel != null)
			{
				notificationCounterLabel.text = notificationCount.ToString();
			}
			if (notificationObject != null)
			{
				notificationObject.SetActive(notificationCount > 0);
			}
		}
	}

	public int NotificationTokenCount
	{
		get
		{
			return notificationTokenCount;
		}
		set
		{
			notificationTokenCount = value;
			if (notificationTokenObject != null && !OfflineManager.IsLoadDataManager)
			{
				notificationTokenObject.SetActive(notificationTokenCount > 0);
			}
		}
	}

	public void Awake()
	{
		NotificationCount = 0;
		NotificationTokenCount = 0;
	}
}
