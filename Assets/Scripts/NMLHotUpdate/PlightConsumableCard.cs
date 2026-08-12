using TWDModel;
using UnityEngine;

public class PlightConsumableCard : UIListCard<DifficultyIncrementalDebuff>
{
	[SerializeField]
	private UILabel descriptionLabel;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null && descriptionLabel != null)
		{
			descriptionLabel.text = HelpersLocalization.GetChallengeDebuffDescription(base.Item);
		}
	}
}
