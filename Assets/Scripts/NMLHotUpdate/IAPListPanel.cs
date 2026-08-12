using TWDModel;
using UnityEngine;

public class IAPListPanel : ScrollableListPanel<BundleStoreDefinition>
{
	[SerializeField]
	[Tooltip("Prefab of IAPs card that are on a limited offer.")]
	private GameObject specialOfferCardPrefab;

	protected override GameObject CreateCard(BundleStoreDefinition item)
	{
		GameObject gameObject = null;
		gameObject = (string.IsNullOrEmpty(item.CardPrefab) ? GameManager.Instance.GetBundleCard(BundleCardsResource.DefaultBundleCard) : GameManager.Instance.GetBundleCard(item.CardPrefab));
		if (gameObject != null)
		{
			return Helpers.InstantiateToParent(gameObject, cardsContainer);
		}
		return null;
	}

	public void UpdateCards()
	{
		foreach (UIListCard<BundleStoreDefinition> card in cards)
		{
			if (card != null)
			{
				card.UpdateUI();
			}
		}
	}
}
