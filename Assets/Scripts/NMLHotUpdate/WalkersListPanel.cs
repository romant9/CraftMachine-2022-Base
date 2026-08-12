using TWDModel;

public class WalkersListPanel : ScrollableListPanel<OutpostWalkerModel>
{
	public OutpostWalkerModel Walker { get; set; }

	protected override void SetCard(UIListCard<OutpostWalkerModel> card)
	{
		if (card is WalkerCard)
		{
			((WalkerCard)card).EnableToggle();
		}
	}

	public void RefreshCards()
	{
		SetCards(GameManager.Instance.playerModel.OutpostModel.CageEnabledWalkerModels);
		UIEvent.Send("SurvivorListRefreshed");
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		RefreshCards();
		if (Walker == null)
		{
			SelectSurvivor(0);
		}
		else
		{
			SelectSurvivor(GetWalkerIndex(Walker));
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewWalkerSelected")
		{
			Walker = parameter as OutpostWalkerModel;
		}
	}

	private int GetWalkerIndex(OutpostWalkerModel outpostWalkerModel)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			if (getCardAt(i).Item == outpostWalkerModel)
			{
				return i;
			}
		}
		return 0;
	}

	private void SelectSurvivor(int index)
	{
		if (GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count > 0)
		{
			SelectCard(index);
			Walker = getCardAt(index).Item;
		}
		else
		{
			UIEvent.Send("OnNewWalkerSelected");
		}
	}

	public WalkerCard GetCardFromSurvivor(OutpostWalkerModel outpostWalker)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			WalkerCard walkerCard = getCardAt(i) as WalkerCard;
			if (walkerCard.Item == outpostWalker)
			{
				return walkerCard;
			}
		}
		return null;
	}
}
