using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostSelectedTeam : ListPanel
{
	[SerializeField]
	[Tooltip("Prefab for an empty survivor card.")]
	private GameObject emptySurvivorCardPrefab;

	private TeamSelectionEmptyCard[] emptySurvivorCards;

	private bool slotsCreated;

	private int maxTeamSize = 3;

	private void OnEnable()
	{
		UpdateSlots();
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public void UpdateSlots()
	{
		List<SurvivorModel> outpostDefendingSurvivors = GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors;
		if (!slotsCreated)
		{
			CreateSlots(3);
			slotsCreated = true;
		}
		if (emptySurvivorCards == null)
		{
			emptySurvivorCards = new TeamSelectionEmptyCard[3];
		}
		int num = Mathf.Min(maxTeamSize, outpostDefendingSurvivors.Count);
		for (int i = 0; i < num; i++)
		{
			SetSurvivorAtCard(outpostDefendingSurvivors[i], i, locked: false);
		}
		for (int j = num; j < 3; j++)
		{
			Transform slotAt = GetSlotAt(j);
			if (emptySurvivorCards[j] == null)
			{
				emptySurvivorCards[j] = Helpers.InstantiateToParent(emptySurvivorCardPrefab, slotAt.parent.gameObject).GetComponent<TeamSelectionEmptyCard>();
			}
			emptySurvivorCards[j].transform.localPosition = slotAt.transform.localPosition;
			emptySurvivorCards[j].SlotIndex = j;
			emptySurvivorCards[j].MaxTeamSize = maxTeamSize;
			emptySurvivorCards[j].Locked = j >= maxTeamSize;
			SetSurvivorAtCard(null, j, j >= maxTeamSize);
		}
	}

	private void SetSurvivorAtCard(SurvivorModel survivorModel, int cardIndex, bool locked)
	{
		SurvivorCard component = GetSlotAt(cardIndex).GetComponent<SurvivorCard>();
		component.Item = survivorModel;
		component.Locked = locked;
		component.EnableEquipmentContainers(enable: true);
		component.Type = SurvivorCard.CardType.TeamSelect;
		component.UpdateUI();
		component.ShowTeamSelection(LocalizationManager.GetText("Popup.TeamSelection.TapToReplace"));
		component.gameObject.SetActive(survivorModel != null);
		if (emptySurvivorCards != null && emptySurvivorCards[cardIndex] != null)
		{
			emptySurvivorCards[cardIndex].gameObject.SetActive(survivorModel == null);
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "EventSurvivorReplaced")
		{
			UpdateSlots();
		}
	}
}
