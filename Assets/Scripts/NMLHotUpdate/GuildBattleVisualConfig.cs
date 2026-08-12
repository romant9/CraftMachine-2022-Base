using UnityEngine;

[CreateAssetMenu(menuName = "GuildWar/GuildBattleVisualConfig", fileName = "GuildBattleVisualConfig")]
public class GuildBattleVisualConfig : ScriptableObject
{
	public Color ValidColor;

	public Color NotValidColor;

	[Header("List element config")]
	public Color IsPlayerColor;

	public Color IsNotPlayer;

	public Color UnavailableBattlePassColor;

	public Color AvailableBattlePassColor;

	[Header("Timer colors")]
	public Color NormalTimerColor;

	public Color LastMinuteWarningLabelColor;

	[Header("Registered players gradient colors")]
	public Color ValidColorGradientTop;

	public Color ValidColorGradientBottom;

	public Color InvalidColorGradientTop;

	public Color InvalidColorGradientBottom;
}
