using UnityEngine;

public class UIGuildBattleOpponentGuildNameLabel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel label;

	public virtual void OnEnable()
	{
		UpdateUI();
	}

	public virtual void UpdateUI()
	{
		string currentOpponentGuildName = GuildWarHelper.GetCurrentOpponentGuildName();
		HelpersUI.SetContentToLabel(label, currentOpponentGuildName, !string.IsNullOrEmpty(currentOpponentGuildName));
	}
}
