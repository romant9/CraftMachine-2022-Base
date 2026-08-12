using System.Collections.Generic;
using TWDModel;

public class TradeGoodshopRewardsCollector : AvailableRewardsCollector
{
	private PlayerModel playerModel;

	public TradeGoodshopRewardsCollector(PlayerModel playerModel)
	{
		this.playerModel = playerModel;
	}

	public List<IReward> GetRewards()
	{
		List<IReward> list = new List<IReward>();
		List<TradeSlotInfo> currentTradeSlots = playerModel.CurrentTradeSlots;
		for (int i = 0; i < currentTradeSlots.Count; i++)
		{
			TradeSlotInfo tradeSlotInfo = currentTradeSlots[i];
			if (tradeSlotInfo != null && !tradeSlotInfo.Bought && tradeSlotInfo.CurrentTradeDefinition != null)
			{
				TradeDefinition currentTradeDefinition = tradeSlotInfo.CurrentTradeDefinition;
				if (currentTradeDefinition != null && currentTradeDefinition.SoldItems != null && currentTradeDefinition.SoldItems.RewardsList != null)
				{
					list.AddRange(tradeSlotInfo.CurrentTradeDefinition.SoldItems.RewardsList);
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
