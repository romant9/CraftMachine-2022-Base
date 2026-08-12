using System.Collections.Generic;
using TWDModel;

public class PlayerHubActivityList : ScrollableListPanel<ActiveInformationDefinition>
{
	public PlayerHubActivityCard selectedCard;

	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
		InitContentList(GameManager.Instance.gameEconomyData.ActiveInformationDefinitions);
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	public void InitContentList(ActiveInformationDefinition[] activeInformationDefinitions)
	{
		List<ActiveInformationDefinition> list = new List<ActiveInformationDefinition>();
		foreach (ActiveInformationDefinition activeInformationDefinition in activeInformationDefinitions)
		{
			if (activeInformationDefinition != null && Helpers.IsInSpenderTier(activeInformationDefinition.SpenderTiers))
			{
				long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
				if (utcTimeStamp >= activeInformationDefinition.ShowTimeMilliseconds && utcTimeStamp <= activeInformationDefinition.EndTimeMilliseconds)
				{
					list.Add(activeInformationDefinition);
				}
			}
		}
		SetCards(list);
		foreach (UIListCard<ActiveInformationDefinition> card in GetCards())
		{
			Helpers.GameObjectSetActive(card.gameObject, value: true);
		}
		PlayerHubActivityCard playerHubActivityCard = getCardAt(0) as PlayerHubActivityCard;
		if (playerHubActivityCard != null)
		{
			selectedCard = playerHubActivityCard;
			playerHubActivityCard.OnClick();
			playerHubActivityCard.SetSelected(selected: true);
		}
		else
		{
			UIEvent.Send("PlayerHubActivitySelectedEvent");
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "PlayerHubActivitySelectedEvent")
		{
			ActiveInformationDefinition activeInformationDefinition = parameter as ActiveInformationDefinition;
			if (selectedCard.Item != activeInformationDefinition)
			{
				selectedCard.SetSelected(selected: false);
				selectedCard = (PlayerHubActivityCard)GetCard(activeInformationDefinition);
				selectedCard.SetSelected(selected: true);
			}
		}
	}
}
