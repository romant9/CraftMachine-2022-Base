using System.Collections.Generic;
using TWDModel;

public class OutfitList : ScrollableListPanel<OutfitDefinition>
{
	public void CreateItems(OutfitDefinition selectOutfit)
	{
		List<OutfitDefinition> availableOutfitDefinitions = GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp);
		SetCards(availableOutfitDefinitions);
		updateListState(selectOutfit);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	protected override void Sort()
	{
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnNewOutfitSeleted":
		case "OnNewOutfitBought":
		{
			OutfitDefinition currentDefinition = parameter as OutfitDefinition;
			updateListState(currentDefinition);
			break;
		}
		case "OnNewOutfitDeseleted":
			updateListState(null);
			break;
		}
	}

	private void updateListState(OutfitDefinition currentDefinition)
	{
		for (int i = 0; i < GetCards().Count; i++)
		{
			OutfitListItem outfitListItem = GetCards()[i] as OutfitListItem;
			if (outfitListItem != null && currentDefinition != null && outfitListItem.GetItemDefinition() == currentDefinition)
			{
				outfitListItem.Select();
			}
			else
			{
				outfitListItem.Deselect();
			}
			outfitListItem.UpdateUI();
		}
	}
}
