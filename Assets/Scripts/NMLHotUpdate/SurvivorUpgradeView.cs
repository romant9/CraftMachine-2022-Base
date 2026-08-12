using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivorUpgradeView : MonoBehaviourExtended
{
	[Header("Content")]
	[SerializeField]
	private SurvivorTraitsUpgradeUnlock traitUpgradeUnlockPanel;

	[SerializeField]
	private SurvivorRarityAndClassPanel promotePanel;

	[SerializeField]
	private SurvivorInfoBaseStatsUpgrade baseIncreasePanel;

	[Header("Buttons")]
	[SerializeField]
	private UIButtonExtended okButton;

	[Header("Ok Button Label")]
	[SerializeField]
	private UILabel okLabel;

	private void Awake()
	{
		DebugIdString = "SurvivorUpgradeView";
	}

	public void DisableOkButton()
	{
		okButton.disabledSprite = "Ui_Regular_Gray_Button_Deactive_Bg";
		okButton.isEnabled = false;
		okLabel.color = Color.gray;
	}

	public void EnableOkButton()
	{
		okButton.disabledSprite = "Ui_Regular_Button_Deactive_Bg";
		okButton.isEnabled = true;
		okLabel.color = Color.white;
	}

	public void UpdateWith(SurvivorModel survivorModel, SurvivorInfoStateBase.States currentState, UIButtonExtended.OnClickCallback onClickCallback)
	{
		if (!IsNotNull(survivorModel, "survivorModel"))
		{
			return;
		}
		if (okButton != null)
		{
			okButton.SetClickCallback(onClickCallback);
		}
		if (traitUpgradeUnlockPanel != null)
		{
			switch (currentState)
			{
			case SurvivorInfoStateBase.States.SurvivorPromoteDone:
				GameManager.Instance.CheckConnectionReachability(showPopup: true, "UpgradeSurvivorTraitCommand");
				traitUpgradeUnlockPanel.UpdateWithTrait(GetTraitById(survivorModel, survivorModel.LastUpgradedTraitId), "SurvivorInfo.Trait.PromoteDone.Desc{Name}{Level}");
				break;
			case SurvivorInfoStateBase.States.SurvivorUpgradeDone:
				traitUpgradeUnlockPanel.UpdateWithTrait(GetTraitById(survivorModel, survivorModel.LastUpgradedTraitId), "SurvivorInfo.Trait.UpgradeDone.Desc{Name}{Level}");
				break;
			}
		}
		if (promotePanel != null)
		{
			promotePanel.UpdateWithSurvivor(survivorModel, useRarityColor: false);
		}
		if (baseIncreasePanel != null)
		{
			baseIncreasePanel.UpdateWithSurvivor(survivorModel, currentState);
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (okButton != null)
		{
			okButton.Clear();
		}
	}

	private static UpgradeTraitsData GetTraitById(SurvivorModel survivorModel, string traitId)
	{
		if (survivorModel != null && traitId != "")
		{
			List<UpgradeTraitsData> upgradeTraits = survivorModel.UpgradeTraits;
			if (upgradeTraits != null)
			{
				for (int i = 0; i < upgradeTraits.Count; i++)
				{
					if (upgradeTraits[i] != null && upgradeTraits[i].Identifier == traitId)
					{
						return upgradeTraits[i];
					}
				}
			}
		}
		return null;
	}
}
