using TWDModel;

public class OutpostSelectBackgroundList : ScrollableListPanel<OutpostTemplateDefinition>
{
	public void UpdateCards()
	{
		foreach (UIListCard<OutpostTemplateDefinition> card in cards)
		{
			if (card != null)
			{
				card.UpdateUI();
			}
		}
	}
}
