using System.Text;
using TWDModel;
using UnityEngine;

public class SurvivorInfoBaseStatsUpgrade : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel healthLabel;

	[SerializeField]
	private UILabel damageLabel;

	public void UpdateWithSurvivor(SurvivorModel survivorModel, SurvivorInfoStateBase.States state)
	{
		int damageDiff = 0;
		int healthDiff = 0;
		switch (state)
		{
		case SurvivorInfoStateBase.States.SurvivorPromoteDone:
			survivorModel.GetStatsDifferenceToPreviousRarityLevel(out damageDiff, out healthDiff);
			break;
		case SurvivorInfoStateBase.States.SurvivorTrainDone:
			survivorModel.GetStatsDifferenceToPreviousLevel(out damageDiff, out healthDiff);
			break;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(LocalizationManager.GetText("Statistic.BuffHealth"));
		stringBuilder.Append(" +");
		stringBuilder.Append(healthDiff);
		HelpersUI.SetContentToLabel(healthLabel, stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.Append(LocalizationManager.GetText("Statistic.BuffDamage"));
		stringBuilder.Append(" +");
		stringBuilder.Append(damageDiff);
		HelpersUI.SetContentToLabel(damageLabel, stringBuilder.ToString());
	}
}
