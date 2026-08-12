using TWDModel;
using UnityEngine;

public class GuildBattleTierListItem : NUIListItem<GuildTierDefinition>
{
	public GameObject IconLockedContainer;

	public GameObject ActiveContainer;

	public GuildShopItemPreview guildShopUnlock;

	public override void UpdateUI()
	{
		base.UpdateUI();
		GuildTierDefinition data = GetData();
		if (data == null)
		{
			return;
		}
		guildShopUnlock.OpenForTier(data.Tier);
		GuildTierDefinition currentGuildTier = GuildTierHelper.GetCurrentGuildTier();
		if (currentGuildTier != null)
		{
			bool flag = data.Tier < currentGuildTier.Tier;
			Helpers.GameObjectSetActive(IconLockedContainer, flag);
			Helpers.GameObjectSetActive(ActiveContainer, data.Tier == currentGuildTier.Tier);
			if (!flag)
			{
				Helpers.GameObjectSetActive(guildShopUnlock, value: false);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(IconLockedContainer, value: true);
			Helpers.GameObjectSetActive(ActiveContainer, value: false);
		}
	}
}
