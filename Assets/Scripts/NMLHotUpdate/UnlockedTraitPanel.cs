using TWDModel;
using UnityEngine;

public class UnlockedTraitPanel : MonoBehaviour
{
	[Header("Labels")]
	[SerializeField]
	private UILabel upgradeLabel;

	[SerializeField]
	private UILabel traitTitle;

	[SerializeField]
	private UISprite traitIcon;

	public void SetInfo(UpgradeTraitsData trait)
	{
		if (trait != null)
		{
			TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(trait.Identifier);
			if (upgradeLabel != null)
			{
				upgradeLabel.text = LocalizationManager.GetText("Popup.SurvivorUpgradeView.Trait.Unlock");
			}
			if (traitTitle != null && traitDefinition != null)
			{
				traitTitle.text = HelpersLocalization.GetTraitName(traitDefinition);
			}
			if (traitIcon != null)
			{
				traitIcon.spriteName = HelpersGfx.GetSurvivorTraitIconName(trait);
			}
		}
	}
}
