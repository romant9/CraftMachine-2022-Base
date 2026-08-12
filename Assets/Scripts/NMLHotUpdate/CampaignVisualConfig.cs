using UnityEngine;

[CreateAssetMenu(menuName = "Campaign Visual Config", fileName = "CampaignVisualConfig_Default")]
public class CampaignVisualConfig : ScriptableObject
{
	[Header("Campaign Screen - Background")]
	public Color bgColorGradientTop;

	public Color bgColorGradientBottom;

	public Color tabContentsBgColor;

	[Header("Campaign Screen - Reward Button")]
	public Color rewardBgColor;

	[Header("Campaign Screen - Campaign Texts")]
	public Color headerColorGradientTop;

	public Color headerColorGradientBottom;

	public Color headerShadowColor;

	public Color paragraphColor;

	public Color paragraphShadowColor;

	[Header("Campaign Screen - FX")]
	public Material highlightPrimaryGlowMaterial;

	public Color highlightSecondaryGlowColor;

	public Color highlightCenterGlowColor = Color.white;

	public bool enableGodRay;

	public bool enableFallingLeaves;

	public bool enableFallingSakuraLeaves;

	public bool enableFallingSnowFlakes;

	public bool enableSteam;

	public bool enableMeltingIce;

	public bool enableFire;

	public bool enableConfetti;

	[Header("Campaign Currency")]
	public Color currencyTextColor;

	public Color currencyTextShadowColor;

	[Header("Camp Button")]
	public Color buttonBgColorGradientTop;

	public Color buttonBgColorGradientBottom;

	public Color buttonTextColorGradientTop;

	public Color buttonTextColorGradientBottom;

	public Color buttonTextShadowColor;

	public Color buttonTimerBgGradientTop;

	public Color buttonTimerBgGradientBottom;

	public Color buttonTimerTextColor;

	[Header("Camp Button - FX")]
	public bool enableButtonGodRay = true;

	public bool enableButtonMeltingIce;

	public Color buttonGlowColor;
}
