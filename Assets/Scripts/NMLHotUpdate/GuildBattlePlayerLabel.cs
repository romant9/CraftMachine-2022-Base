using TWDModel;
using UnityEngine;
using UnityEngine.Serialization;

public class GuildBattlePlayerLabel : MonoBehaviour
{
	[SerializeField]
	[FormerlySerializedAs("playerName")]
	protected UILabel playerNameLabel;

	[SerializeField]
	protected PlayerEmblemIcon playerEmblemIcon;

	public void SetPlayerData(string playerName, PlayerEmblem playerEmblem)
	{
		HelpersUI.SetContentToLabel(playerNameLabel, GameManager.Instance.GetFilteredText(playerName));
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(playerEmblem);
		}
	}
}
