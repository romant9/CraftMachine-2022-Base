using System.Collections.Generic;
using System.Linq;
using TWDModel;

public class GuildShopRewardsCollector : AvailableRewardsCollector
{
	private GuildShopModel guildShopModel;

	public GuildShopRewardsCollector(GuildShopModel guildShopModel)
	{
		this.guildShopModel = guildShopModel;
	}

	public List<IReward> GetRewards()
	{
		List<IReward> list = new List<IReward>();
		GuildShopItemInfo[] array = guildShopModel.GuildShopAvailableItems.Values.ToArray();
		foreach (GuildShopItemInfo guildShopItemInfo in array)
		{
			if (guildShopItemInfo.Unlocked && !guildShopItemInfo.SoldOut)
			{
				GuildShopDefinition itemDefinition = guildShopItemInfo.ItemDefinition;
				if (itemDefinition != null && itemDefinition.ContentRewards != null)
				{
					list.AddRange(itemDefinition.ContentRewards.RewardsList);
				}
			}
		}
		return list;
	}

	public object CreateParameterObject()
	{
		return null;
	}
}
