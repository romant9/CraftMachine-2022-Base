using System.Text;
using UnityEngine;

public class TooltipSurvivalReward : TooltipTextbox
{
	[Tooltip("Completion count amount")]
	[SerializeField]
	private UILabel completionCountValueLabel;

	[Tooltip("Desc of reward trade crates")]
	[SerializeField]
	private UILabel rewardTradeCrateLabel;

	[Tooltip("Preview of currency rewards")]
	[SerializeField]
	private RewardIcon rewardIcon;

	private StringBuilder builder;

	protected override void Deactivate()
	{
		base.Deactivate();
		if (builder != null)
		{
			builder.Length = 0;
			builder = null;
		}
	}
}
