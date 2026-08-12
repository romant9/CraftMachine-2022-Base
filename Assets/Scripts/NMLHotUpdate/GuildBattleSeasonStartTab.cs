using UnityEngine;

public class GuildBattleSeasonStartTab : UIToggleContent
{
	[SerializeField]
	private UILabel nextWarTimer;

	[SerializeField]
	private GuildShopItemPreview guildShopSeasonUnlock;

	public override void Activate()
	{
		base.Activate();
		if (guildShopSeasonUnlock != null)
		{
			guildShopSeasonUnlock.OpenForTier(1);
		}
		UpdateUI();
	}

	private void UpdateUI()
	{
		HelpersUI.SetContentToLabel(nextWarTimer, GuildWarHelper.GetFormatedTimeLeftToNextWar());
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	public void OnClickGuildShop()
	{
		GuildShopPopup.OpenGuildShop();
	}
}
