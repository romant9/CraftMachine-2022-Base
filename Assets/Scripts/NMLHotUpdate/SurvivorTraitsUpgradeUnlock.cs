using TWDModel;
using UnityEngine;

public class SurvivorTraitsUpgradeUnlock : MonoBehaviourExtended
{
	[SerializeField]
	private UISprite traitSprite;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel descLabel;

	private void Awake()
	{
		DebugIdString = "SurvivorTraitsUpgradeUnlock";
	}

	public void UpdateWithTrait(UpgradeTraitsData upgradeTraitsData, string localizationId)
	{
		if (!IsNotNull(upgradeTraitsData))
		{
			return;
		}
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
		if (IsNotNull(traitDefinition))
		{
			string text = LocalizationManager.GetText(localizationId, HelpersLocalization.GetTraitName(traitDefinition), (upgradeTraitsData.RarityLevel + 1).ToString());
			if (traitSprite != null)
			{
				traitSprite.spriteName = HelpersGfx.GetSurvivorTraitIconName(upgradeTraitsData);
			}
			if (descLabel != null)
			{
				descLabel.text = text;
			}
			if (nameLabel != null)
			{
				nameLabel.text = HelpersLocalization.GetTraitName(traitDefinition);
			}
		}
	}
}
