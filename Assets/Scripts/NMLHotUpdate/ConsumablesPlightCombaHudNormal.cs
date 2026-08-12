using TWDModel;
using UnityEngine;

public class ConsumablesPlightCombaHudNormal : HUDElement
{
	[SerializeField]
	private PlightConsumableListPanel consumableListPanel;

	public override void Open()
	{
		base.Open();
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null && WeeklyChallengeHelper.IsChallengeOngoing() && consumableListPanel != null)
		{
			consumableListPanel.Init(weeklyChallengeModel.CurrentCircleDefinition.DebuffConfigs);
		}
	}
}
