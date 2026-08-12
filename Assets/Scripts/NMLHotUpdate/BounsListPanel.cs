using System.Collections.Generic;

public class BounsListPanel : ScrollableListPanel<BounsInfo>
{
	public void Init(List<BounsInfo> bounsInfos)
	{
		SetCards(bounsInfos);
	}
}
