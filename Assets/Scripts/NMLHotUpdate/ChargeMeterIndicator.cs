using UnityEngine;

public class ChargeMeterIndicator : HUDElement
{
	[Tooltip("Charge Meter fill sprite.")]
	public UISprite FillSprite;

	[Tooltip("Charge Meter fill glow sprite.")]
	public UISprite FillGlowSprite;

	[Tooltip("Cancel charge icon object.")]
	public GameObject CancelChargeObject;

	[Tooltip("Button object that is active when charge is inactive.")]
	public GameObject InactiveButtonObject;

	[Tooltip("Button object that is active when charge is active.")]
	public GameObject ActiveButtonObject;

	[Tooltip("Label for active charges count.")]
	public UILabel ChargeCountLabel;
}
