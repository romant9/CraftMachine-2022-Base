using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class SupportTraitCardList : ScrollableListPanel<SupportTalentDefinition>
{
	[SerializeField]
	private UILabel emptyLabel;

	private SupportModel _supportModel;

	private int _slotIndex;

	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SupportDetailSelectedEvent")
		{
			UpdateContent(_supportModel, _slotIndex);
		}
	}

	public void UpdateContent(SupportModel supportModel, int slotIndex)
	{
		_supportModel = supportModel;
		_slotIndex = slotIndex;
		List<SupportTalentDefinition> list = new List<SupportTalentDefinition>();
		foreach (int id in supportModel.GetAvailableTraitsTalentIds())
		{
			SupportTalentDefinition supportTalentDefinitionById = GameManager.Instance.gameEconomyData.GetSupportTalentDefinitionById(id);
			if (supportTalentDefinitionById != null)
			{
				if (supportModel.SlotAssembledTalentIds.Values.Any((int value) => value == id))
				{
					list.Insert(0, supportTalentDefinitionById);
				}
				else
				{
					list.Add(supportTalentDefinitionById);
				}
			}
		}
		if (list.Count == 0)
		{
			Helpers.GameObjectSetActive(emptyLabel.gameObject, value: true);
			return;
		}
		Helpers.GameObjectSetActive(emptyLabel.gameObject, value: false);
		SetCards(list);
		foreach (UIListCard<SupportTalentDefinition> card in GetCards())
		{
			card.TryGetComponent<SupportTraitCard>(out var component);
			if (component != null)
			{
				component.SetContent(supportModel, slotIndex);
			}
			Helpers.GameObjectSetActive(card.gameObject, value: true);
		}
	}
}
