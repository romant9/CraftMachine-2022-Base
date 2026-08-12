using UnityEngine;

public class GuildBattleHighscoresListCard : NUIListItem<GuildBattleHighscoresEntry>
{
	[SerializeField]
	private GuildBattleHighscoresPlayerEntry leftPlayerContainer;

	[SerializeField]
	private GuildBattleHighscoresPlayerEntry rightPlayerContainer;

	public override void UpdateUI()
	{
		GuildBattleHighscoresEntry data = GetData();
		if (leftPlayerContainer != null)
		{
			leftPlayerContainer.SetPlayerData(data.playerA);
		}
		if (rightPlayerContainer != null)
		{
			rightPlayerContainer.SetPlayerData(data.playerB);
		}
	}
}
