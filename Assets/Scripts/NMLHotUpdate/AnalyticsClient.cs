using System.Collections.Generic;
using BaseModel;

public class AnalyticsClient : IModelAnalytics
{
	public void CreateEvent<T>(string type, Dictionary<string, T> properties)
	{
		if (GameConfiguration.Instance.Config.ShowDebugMenu)
		{
			EventManager.NotifyEvent(EventManager.EventType.AnalyticsSent, new object[2] { type, properties });
		}
	}

	public int IncrementCounter()
	{
		return 0;
	}
}
