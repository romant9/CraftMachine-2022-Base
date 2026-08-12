using TWDModel;
using UnityEngine;

public class SurvivorTraitRerollView : MonoBehaviour
{
	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private UIButtonWithLabelAndIcon[] traitButtons;

	[SerializeField]
	private UIButtonExtended[] traitChoice;

	[SerializeField]
	private UIButtonExtended cancelButton;

	[SerializeField]
	private UIButtonExtended okButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon oldTrait;

	[SerializeField]
	private UIButtonWithLabelAndIcon newTrait;

	[SerializeField]
	private GameObject chooseReplacement;

	[SerializeField]
	private GameObject traitReplaced;

	private UIButtonExtended.OnClickCallback keepTraitCallback;

	private TraitDefinition[] traitDefinitions = new TraitDefinition[3];

	public void UpdateWith(SurvivorModel survivorModel, UIButtonExtended.OnClickCallback[] onClickCallback)
	{
		ClearCallbacks(includeOkButton: true);
		int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(survivorModel.TraitToBeRerolledCandidate);
		for (int i = 0; i < survivorModel.RandomTraitsFromReroll.Count; i++)
		{
			traitDefinitions[i] = GameManager.Instance.gameEconomyData.GetTraitDefinition(UpgradeTraitsData.CompileUpgradeTraitIdentifier(survivorModel.RandomTraitsFromReroll[i], traitLevelIdentifier, isLocked: false));
			SetTraitDataToButton(traitButtons[i], traitDefinitions[i], traitLevelIdentifier);
			if (onClickCallback[i] != null)
			{
				traitChoice[i].SetClickCallback(onClickCallback[i]);
			}
		}
		traitDefinitions[2] = GameManager.Instance.gameEconomyData.GetTraitDefinition(survivorModel.TraitToBeRerolledCandidate);
		cancelButton.SetClickCallback(CancelCallback);
		keepTraitCallback = onClickCallback[2];
		okButton.SetClickCallback(onClickCallback[3]);
		Helpers.GameObjectSetActive(cancelButton, value: true);
		Helpers.GameObjectSetActive(chooseReplacement, value: true);
		Helpers.GameObjectSetActive(okButton, value: false);
		Helpers.GameObjectSetActive(traitReplaced, value: false);
		TweenManager.PlayTweenGroup(base.gameObject, 0);
	}

	public void TraitChosen(int index)
	{
		HelpersUI.SetContentToLabel(contentLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SurvivorInfoPopup.TraitUpdated"));
		int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitDefinitions[2].Identifier);
		ClearCallbacks();
		SetTraitDataToButton(newTrait, traitDefinitions[index], traitLevelIdentifier);
		SetTraitDataToButton(oldTrait, traitDefinitions[2], traitLevelIdentifier);
		Helpers.GameObjectSetActive(cancelButton, value: false);
		Helpers.GameObjectSetActive(chooseReplacement, value: false);
		Helpers.GameObjectSetActive(okButton, value: true);
		Helpers.GameObjectSetActive(traitReplaced, value: true);
		TweenManager.PlayTweenGroup(base.gameObject, 0);
	}

	public void ClearCallbacks(bool includeOkButton = false)
	{
		for (int i = 0; i < traitButtons.Length; i++)
		{
			traitButtons[i].Clear();
			traitChoice[i].Clear();
		}
		cancelButton.Clear();
		newTrait.Clear();
		oldTrait.Clear();
		if (includeOkButton)
		{
			okButton.Clear();
		}
	}

	private void CancelCallback(UIButtonExtended button)
	{
		ConfirmationPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("TraitReroll.Alert.Cancel.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("TraitReroll.Alert.Cancel.Message"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Button.Confirm"), Confirm);
	}

	private void Confirm()
	{
		keepTraitCallback(cancelButton);
	}

	private void SetTraitDataToButton(UIButtonWithLabelAndIcon traitButton, TraitDefinition traitDefinition, int level)
	{
		traitButton.SetContentToLabelOne(HelpersLocalization.GetTraitName(traitDefinition));
		traitButton.SetContentToLabelTwo((level + 1).ToString());
		traitButton.SetContentToIconOne(HelpersGfx.GetSurvivorTraitIconName(traitDefinition));
		traitButton.SetClickCallback(delegate
		{
			TooltipManager.OpenTextBoxWithText(traitButton.gameObject, HelpersLocalization.GetTraitDescription(traitDefinition));
		});
	}
}
