using TWDModel;
using UnityEngine;

public class ChallengeInfoPopup : HUDElement
{
	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UILabel traitDescription;

	public void OpenForTrait(DifficultyIncrementalDebuff buff)
	{
		traitIcon.spriteName = buff.Image;
		HelpersUI.SetContentToLabel(traitName, LocalizationManager.GetText(buff.Name));
		HelpersUI.SetContentToLabel(traitDescription, LocalizationManager.GetText(buff.Description));
		base.Open();
	}
}
