using TWDModel;
using UnityEngine;

public class PlightConsumableCardEffect : UIListCard<WeeklyChallengeApocalypseBuff>
{
	[SerializeField]
	private UILabel descriptionLabel;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null && descriptionLabel != null)
		{
			descriptionLabel.text = HelpersLocalization.GetApocalypticDescription(base.Item);
		}
	}
}
