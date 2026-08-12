using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class TooltipButton : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Determines the position of the tooltip's cursor")]
	private GameObject tooltipTarget;

	[SerializeField]
	private string tooltipText = "";

	[SerializeField]
	private bool useLocalizationManager;

	public void SetText(string tooltipText_)
	{
		tooltipText = tooltipText_;
	}

	private void OnClick()
	{
		if (tooltipText != "")
		{
			TooltipManager.OpenTextBoxWithText(tooltipTarget ? tooltipTarget : base.gameObject, useLocalizationManager ? LocalizationManager.GetText(tooltipText) : tooltipText);
		}
	}
}
