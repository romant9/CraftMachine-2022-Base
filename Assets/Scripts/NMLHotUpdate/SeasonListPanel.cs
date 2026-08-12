using TWDModel;
using UnityEngine;

public class SeasonListPanel : ScrollableListPanel<SeasonDefinition>
{
	[SerializeField]
	private GameObject highlightCardPrefab;

	protected override GameObject CreateCard(SeasonDefinition item)
	{
		return Helpers.InstantiateToParentAndLayer(item.Highlighted ? highlightCardPrefab : cardPrefab, cardsContainer);
	}
}
