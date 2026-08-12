using System.Collections.Generic;
using TWDModel;

public class SupportTalentTreeCardList : ScrollableListPanel<SupportTalentTreeMainDefinition>
{
	private SupportTalentTreeCard _selectedCard;

	private SupportModel supportModel;

	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	public void InitContentList(SupportModel model, List<SupportTalentTreeMainDefinition> definitions)
	{
		supportModel = model;
		SetCards(definitions);
		foreach (UIListCard<SupportTalentTreeMainDefinition> card in GetCards())
		{
			Helpers.GameObjectSetActive(card.gameObject, value: true);
		}
		SupportTalentTreeCard supportTalentTreeCard = getCardAt(0) as SupportTalentTreeCard;
		if (supportTalentTreeCard != null)
		{
			_selectedCard = supportTalentTreeCard;
			supportTalentTreeCard.OnClick();
			supportTalentTreeCard.SetSelected(selected: true);
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SupportTalentSelectedEvent" && parameter is SupportTalentTreeMainDefinition supportTalentTreeMainDefinition && _selectedCard.Item != supportTalentTreeMainDefinition && supportModel.Level >= supportTalentTreeMainDefinition.UnlockLevel)
		{
			_selectedCard.SetSelected(selected: false);
			_selectedCard = (SupportTalentTreeCard)GetCard(supportTalentTreeMainDefinition);
			_selectedCard.SetSelected(selected: true);
		}
	}
}
