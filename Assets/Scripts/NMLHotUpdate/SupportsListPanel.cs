using TWDModel;
using UnityEngine;
using UnityEngine.Events;

public class SupportsListPanel : ScrollableListPanel<SupportModel>
{
	[SerializeField]
	private UnityEvent onSupportUpgrade;

	private void Start()
	{
		SetCards(GameManager.Instance.playerModel.SupportModels);
	}

	protected override GameObject CreateCard(SupportModel item)
	{
		GameObject gameObject = base.CreateCard(item);
		SupportCard card = gameObject.GetComponent<SupportCard>();
		card.Initialize(item, delegate
		{
			((SupportDetailsPopup)HUDManager.TryOpenPopup(UIType.SupportDetailsPopup)).Show(item, canUpgrade: true, delegate
			{
				card.Refresh();
				onSupportUpgrade.Invoke();
			});
		});
		return gameObject;
	}

	protected override void Sort()
	{
		cards.StableSort(delegate(UIListCard<SupportModel> a, UIListCard<SupportModel> b)
		{
			int index = a.Item.definition.Index;
			int index2 = b.Item.definition.Index;
			if (index == index2)
			{
				return 0;
			}
			return (index >= index2) ? 1 : (-1);
		});
	}
}
