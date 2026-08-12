public class UISurvivalRewardsWidgetList : ScrollableWidgetListPanel<UISurvivalRewardsWidget>
{
	public UISurvivalRewardsWidget CreateItemForLootEntry(LootEntry entry)
	{
		UISurvivalRewardsWidget uISurvivalRewardsWidget = null;
		if (cardPrefab != null)
		{
			uISurvivalRewardsWidget = InstantiateItemToList(cardPrefab) as UISurvivalRewardsWidget;
			if (uISurvivalRewardsWidget != null)
			{
				uISurvivalRewardsWidget.UpdateUI(entry);
				uISurvivalRewardsWidget.Deactivate();
			}
		}
		return uISurvivalRewardsWidget;
	}
}
