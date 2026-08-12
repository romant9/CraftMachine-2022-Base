using UnityEngine;

public class GuildBattleHighscoresPlayerEntry : GuildBattlePlayerLabel
{
	[SerializeField]
	private UILabel playerScore;

	[SerializeField]
	private UILabel playerAttacks;

	public void SetPlayerData(GuildBattlePlayersScoreDataEntry playerData)
	{
		if (playerData == null)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		if (playerAttacks != null)
		{
			int value;
			bool flag = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.NumberOfAttacksPerPlayer.TryGetValue(playerData.Id, out value);
			HelpersUI.SetContentToLabel(playerAttacks, flag ? value.ToString() : GameManager.Instance.gameEconomyData.GuildWarConfig.KeysPerBattle.ToString());
		}
		HelpersUI.SetContentToLabel(playerNameLabel, GameManager.Instance.GetFilteredText(playerData.Name));
		HelpersUI.SetContentToLabel(playerScore, playerData.Score.ToString());
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(playerData.PlayerEmblem);
		}
	}
}
