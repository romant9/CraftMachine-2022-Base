using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TradeItemsListPanel : ScrollableListPanel<TradeSlotInfo>
{
	public void SetupItems()
	{
		List<TradeSlotInfo> list = new List<TradeSlotInfo>();
		for (int i = 0; i < GameManager.Instance.playerModel.CurrentTradeSlots.Count; i++)
		{
			TradeSlotInfo tradeSlotInfo = GameManager.Instance.playerModel.CurrentTradeSlots[i];
			if ((!tradeSlotInfo.CurrentTradeDefinition.HasDateLimit || !tradeSlotInfo.Bought) && (!(tradeSlotInfo.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardOutfit rewardOutfit) || !GameManager.Instance.playerModel.SurvivorContainer.HasOutfit(rewardOutfit.PreferredOrder[0])))
			{
				list.Add(tradeSlotInfo);
			}
		}
		SetCards(list, resetScrollView: false);
	}

	public void UpdateCards()
	{
		foreach (UIListCard<TradeSlotInfo> card in cards)
		{
			if (card != null)
			{
				card.UpdateUI();
			}
		}
	}

	public float GetScrollHorisontalPosition()
	{
		return scrollView.horizontalScrollBar.value;
	}

	public void SetScrollPosition(float x, int amount)
	{
		if (amount == 0)
		{
			amount = cards.Count;
		}
		scrollView.ResetPosition();
		scrollView.horizontalScrollBar.value = Mathf.Max(0f, x);
	}
}
