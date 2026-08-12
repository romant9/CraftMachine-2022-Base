using System.Collections.Generic;
using TWDModel;

public class ActivityListPanel : ScrollableListPanel<IActivityManagerIntegrationInterface>
{
	public void Init(List<IActivityManagerIntegrationInterface> data)
	{
		SetCards(data);
	}
}
