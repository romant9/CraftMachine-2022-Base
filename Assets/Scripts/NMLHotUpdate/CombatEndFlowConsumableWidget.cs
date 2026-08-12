using UnityEngine;

public class CombatEndFlowConsumableWidget : CombatEndWidget
{
	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UILabel amountLabel;

	public void SetupConsumableReward(RewardEquipment rewardEquipment)
	{
		consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
		amountLabel.text = rewardEquipment.Amount.ToString();
	}
}
