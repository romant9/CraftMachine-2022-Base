using System.Collections.Generic;
using TWDModel;

public class PlightConsumableEffectListPanel : ScrollableListPanel<WeeklyChallengeApocalypseBuff>
{
	protected override bool LastEntryAtTop => false;

	public void Init(List<WeeklyChallengeApocalypseBuff> data)
	{
		SetCards(data);
	}
}
