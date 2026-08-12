using TWDModel;
using UnityEngine;

public class UpdateGuildTiers : MonoBehaviour
{
	[SerializeField]
	private UISprite guildImage;

	[SerializeField]
	private UISprite enemyGuildImage;

	private void OnEnable()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		int num = GameManager.Instance?.guildModel?.GuildBattleTier ?? 10;
		int num2 = guildWarModel?.CurrentBattle?.EnemyGuildTier ?? 10;
		string text = "Ui_Emblem_GvG_Tier";
		guildImage.spriteName = text + num;
		enemyGuildImage.spriteName = text + num2;
	}
}
