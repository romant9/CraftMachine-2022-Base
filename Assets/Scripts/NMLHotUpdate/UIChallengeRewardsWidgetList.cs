public class UIChallengeRewardsWidgetList : ScrollableWidgetListPanel<UIChallengeRewardsWidget>
{
	public UIChallengeRewardsWidget CreateItemForLootEntry(LootEntry entry)
	{
		UIChallengeRewardsWidget uIChallengeRewardsWidget = null;
		if (cardPrefab != null)
		{
			uIChallengeRewardsWidget = InstantiateItemToList(cardPrefab) as UIChallengeRewardsWidget;
			if (uIChallengeRewardsWidget != null)
			{
				uIChallengeRewardsWidget.UpdateUI(entry);
				uIChallengeRewardsWidget.Deactivate();
			}
		}
		return uIChallengeRewardsWidget;
	}
}
