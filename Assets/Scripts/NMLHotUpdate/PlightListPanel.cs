using System.Collections.Generic;
using TWDModel;

public class PlightListPanel : ScrollableListPanel<DifficultyIncrementalDebuff>
{
	public void Init(List<DifficultyIncrementalDebuff> data)
	{
		SetCards(data);
	}
}
