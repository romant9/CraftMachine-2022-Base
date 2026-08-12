using TWDModel;
using UnityEngine;

public class LeaderTraitVisual : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject traitPresentParent;

	[SerializeField]
	private GameObject traitNotPresent;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private GameObject tooltipParent;

	private string tooltipContent = "";

	private bool traitPresent;

	private Vector3 gvgModePosition = new Vector3(0f, -205f, 0f);

	public void SetTrait(bool traitPresent, TraitDefinition traitDefinition, bool isGvGMode)
	{
		this.traitPresent = traitPresent;
		traitPresentParent.SetActive(traitPresent);
		traitNotPresent.SetActive(!traitPresent);
		if (isGvGMode)
		{
			traitNotPresent.transform.localPosition = gvgModePosition;
			traitPresentParent.transform.localPosition = gvgModePosition;
		}
		if (traitDefinition != null)
		{
			tooltipContent = HelpersLocalization.GetLeaderTraitTeamDescription(traitDefinition);
			if (traitIcon != null)
			{
				traitIcon.spriteName = HelpersGfx.GetSurvivorTraitIconName(traitDefinition);
			}
			if (traitName != null && traitDefinition != null)
			{
				traitName.text = HelpersLocalization.GetTraitName(traitDefinition);
			}
		}
	}

	public void OnClick()
	{
		if (tooltipContent != "" && traitPresent && tooltipParent != null)
		{
			TooltipManager.OpenTextBoxWithText(tooltipParent, tooltipContent);
		}
		EventManager.NotifyClick("HeroTrait");
	}
}
