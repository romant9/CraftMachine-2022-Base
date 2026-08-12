using UnityEngine;

public class MissionRewardsPanel : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Panel showing the rewards amounts")]
	private CurrencyAmountPanel[] rewardsCurrencyPanels;

	[SerializeField]
	private UILabel mainRewardLabel;

	public void SetRewards()
	{
	}
}
