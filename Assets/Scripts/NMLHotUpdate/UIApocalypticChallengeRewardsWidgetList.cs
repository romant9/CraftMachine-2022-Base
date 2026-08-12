public class UIApocalypticChallengeRewardsWidgetList : ScrollableWidgetListPanel<UIApocalypticChallengeRewardsWidget>
{
	public UIApocalypticChallengeRewardsWidget CreateItemForLootEntry(LootEntry entry)
	{
		UIApocalypticChallengeRewardsWidget uIApocalypticChallengeRewardsWidget = null;
		if (cardPrefab != null)
		{
			uIApocalypticChallengeRewardsWidget = InstantiateItemToList(cardPrefab) as UIApocalypticChallengeRewardsWidget;
			if (uIApocalypticChallengeRewardsWidget != null)
			{
				uIApocalypticChallengeRewardsWidget.UpdateUI(entry);
				uIApocalypticChallengeRewardsWidget.Deactivate();
			}
		}
		return uIApocalypticChallengeRewardsWidget;
	}
}
