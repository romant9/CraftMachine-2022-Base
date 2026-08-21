using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class TeamSelectionSelectedSurvivorPanel : ListPanel
{
	public const string EventCurrentTeamUpdated = "EventCurrentTeamUpdated";

	private Vector3 panelOriginalPosition;

	[SerializeField]
	[Tooltip("Prefab for an empty survivor card.")]
	private GameObject emptySurvivorCardPrefab;

	[SerializeField]
	private GameObject survivorSupportsBackground;

	[SerializeField]
	private GameObject[] objectsToBeHidden;

	private TeamSelectionEmptyCard[] emptySurvivorCards;

	private int selectedCardIndex;

	private bool slotsCreated;

	private MissionData missionData;

	private Dictionary<int, SurvivorModel> indexToSurvivor = new Dictionary<int, SurvivorModel>();

	private List<SurvivorModel> currentTeam;

	public TeamSelectionSurvivorsListPanel TeamSelectionSurvivorsList { get; set; }

	public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

	public void SetMissionData(MissionData missionData)
	{
		this.missionData = missionData;
	}

	public void ClearMissionData()
	{
		missionData = null;
	}

	private void Awake()
	{
		if (!slotsCreated)
		{
			CreateSlots(3);
			slotsCreated = true;
		}
	}

	public int GetCurrentTeamSize()
	{
		if (missionData != null && missionData.ExtraData != null && missionData.ExtraData.InUse && (SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || SurvivorType == SurvivorContainerModel.SurvivorType.Outpost))
		{
			return 3;
		}
		int num = ((missionData != null) ? missionData.MaxTeamSize : 3);
		if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival)
		{
			int numSurvivorsAvailableForAction = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters.GetNumSurvivorsAvailableForAction();
			if (numSurvivorsAvailableForAction < num)
			{
				num = numSurvivorsAvailableForAction;
			}
		}
		return num;
	}

	public void UpdateSlots()
	{
		int currentTeamSize = GetCurrentTeamSize();
		List<SurvivorModel> survivorsForType = TeamSelectionPopup.GetSurvivorsForType(SurvivorType);
		if (currentTeam == null)
		{
			currentTeam = new List<SurvivorModel>();
		}
		else
		{
			currentTeam.Clear();
		}
		Helpers.GameObjectSetActive(survivorSupportsBackground, SurvivorType != SurvivorContainerModel.SurvivorType.GvGDefenders);
		bool flag = SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival;
		if (emptySurvivorCards == null)
		{
			emptySurvivorCards = new TeamSelectionEmptyCard[3];
		}
		int num = 0;
		List<int> list = new List<int> { 0, 1, 2 };
		ModelRandom random = new ModelRandom();
		GameManager.Instance.SurvivorsFromMission = null;
		int num2 = 0;
		if (missionData != null && missionData.ExtraData != null && missionData.ExtraData.InUse && SurvivorType != SurvivorContainerModel.SurvivorType.CombatOutpost && SurvivorType != SurvivorContainerModel.SurvivorType.Outpost)
		{
			num2 = ((missionData.ExtraData.PlayableSurvivors != null && missionData.ExtraData.PlayableSurvivors.Count > 0) ? missionData.ExtraData.PlayableSurvivors.Count : 0);
			TWDModelManager modelManager = GameManager.Instance.modelManager;
			MapMissionModel mission = GameManager.Instance.playerModel.MapContainerModel.GetMission(missionData.DisplayTextID);
			for (int i = 0; i < num2; i++)
			{
				int[] survivorStartingLevelsForMission = modelManager.GameEconomyData.GetSurvivorStartingLevelsForMission(mission.MissionLevel, missionData.ExtraData.PlayableSurvivors[i].Rarity);
				PlayableSurvivor playableSurvivor = missionData.ExtraData.PlayableSurvivors[i];
				if (!list.Contains(playableSurvivor.RosterIndex))
				{
					Debug.LogError("Playable survivor " + playableSurvivor.ActorID + " Roster Index " + playableSurvivor.RosterIndex + " already taken!");
				}
				else
				{
					SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.CreateSurvivorFromDefinition(playableSurvivor.ActorID, survivorStartingLevelsForMission[0] + playableSurvivor.MinLevel, survivorStartingLevelsForMission[1] + playableSurvivor.MaxLevel, playableSurvivor.Rarity, playableSurvivor.EqLevel, playableSurvivor.EqRarity, random, playableSurvivor.WeaponID, playableSurvivor.ArmorID, isMock: true);
					survivorModel.SetupMockTraits();
					survivorModel.PvPDefenderIndex = playableSurvivor.RosterIndex;
					ActorView.PrepareActor(survivorModel, isTransient: true);
					SetSurvivorAtCard(survivorModel, playableSurvivor.RosterIndex, locked: false, isTeaserSurvivor: true, flag);
					list.Remove(playableSurvivor.RosterIndex);
					currentTeam.Add(survivorModel);
					num++;
				}
			}
			GameManager.Instance.SurvivorsFromMission = new List<SurvivorModel>(currentTeam);
		}
		for (int j = 0; j < currentTeamSize; j++)
		{
			if (j < survivorsForType.Count)
			{
				int num3 = list[0];
				SetSurvivorAtCard(survivorsForType[j], num3, locked: false, isTeaserSurvivor: false, flag);
				currentTeam.Add(survivorsForType[j]);
				list.Remove(num3);
				num++;
			}
		}
		for (int k = 0; k < currentTeamSize; k++)
		{
			GetSlotAt(k).GetComponent<SurvivorCard>().SurvivorsFilterDelegate = () => new List<SurvivorModel>(currentTeam);
		}
		for (int num4 = num; num4 < 3; num4++)
		{
			Transform slotAt = GetSlotAt(num4);
			if (emptySurvivorCards[num4] == null)
			{
				emptySurvivorCards[num4] = Helpers.InstantiateToParentAndLayer(emptySurvivorCardPrefab, slotAt.parent.gameObject).GetComponent<TeamSelectionEmptyCard>();
			}
			emptySurvivorCards[num4].transform.localPosition = slotAt.transform.localPosition;
			emptySurvivorCards[num4].SlotIndex = num4;
			emptySurvivorCards[num4].MaxTeamSize = currentTeamSize;
			emptySurvivorCards[num4].IsSurvivalMode = flag;
			emptySurvivorCards[num4].Locked = num4 >= currentTeamSize;
			int num5 = currentTeamSize;
			SetSurvivorAtCard(null, num4, num4 >= currentTeamSize, isTeaserSurvivor: false, flag);
			if (num2 > 0)
			{
				Helpers.GameObjectSetActive(emptySurvivorCards[num4], num4 <= num5);
			}
		}
		UIEvent.Send("EventCurrentTeamUpdated", currentTeam);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "EventCloseSelectionPanel":
			MoveBackInPlace();
			break;
		case "EventSurvivorReplaced":
		{
			object[] array = (object[])parameter;
			StartCoroutine(SurvivorReplaced(array[0] as SurvivorModel, (int)array[1]));
			break;
		}
		case "SurvivorDeleted":
		case "ReloadSurvivorList":
			UpdateSlots();
			break;
		case "SurvivorListRefreshed":
			UpdateCards();
			break;
		}
	}

	public void SurvivorSelected(int survivorCardIndex)
	{
		EnableBackground(enabled: false);
		StartCoroutine(MoveToLeftCoroutine(survivorCardIndex));
	}

	private void SetSurvivorCardsClickable(bool clickable)
	{
		for (int i = 0; GetSlotAt(i) != null; i++)
		{
			if (GetSlotAt(i).TryGetComponent<SurvivorCard>(out var component))
			{
				component.SetCanClick(clickable);
			}
		}
	}

	public void MoveBackInPlace()
	{
		StartCoroutine(MoveToRightCoroutine());
	}

	private IEnumerator MoveToLeftCoroutine(int survivorCardIndex)
	{
		selectedCardIndex = survivorCardIndex;
		panelOriginalPosition = container.transform.localPosition;
		Vector3 newPosition = panelOriginalPosition;
		Transform selectedCard = GetSlotAt(survivorCardIndex);
		float num = UICamera.currentCamera.WorldToScreenPoint(selectedCard.position).x + selectedCard.GetComponent<BoxCollider>().size.x / 2f;
		Transform slotAt = GetSlotAt(0);
		float num2 = UICamera.currentCamera.WorldToScreenPoint(slotAt.position).x + slotAt.GetComponent<BoxCollider>().size.x / 2f;
		float timeForPanelToReachSelectedCard = TeamSelectionSurvivorsList.ApparitionTime * ((float)Screen.width - num) / ((float)Screen.width - num2);
		yield return new WaitForSeconds(timeForPanelToReachSelectedCard);
		newPosition.x -= (GetSlotAt(0).GetComponent<BoxCollider>().size.x + (float)pixelsBetweenSlots) * (float)survivorCardIndex;
		TweenPosition.Begin(container, TeamSelectionSurvivorsList.ApparitionTime - timeForPanelToReachSelectedCard, newPosition);
		selectedCard.gameObject.GetComponent<SurvivorCard>().ShowTeamSelection(null);
	}

	private IEnumerator MoveToRightCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		GetSlotAt(selectedCardIndex).gameObject.GetComponent<SurvivorCard>().ShowTeamSelection(LocalizationManager.GetText("Popup.TeamSelection.TapToReplace"));
		EventDelegate eventDelegate = new EventDelegate(delegate
		{
			EnableBackground(enabled: true);
		});
		eventDelegate.oneShot = true;
		TweenPosition.Begin(container, 0.3f, panelOriginalPosition).AddOnFinished(eventDelegate);
	}

	private void EnableBackground(bool enabled)
	{
		SetSurvivorCardsClickable(enabled);
		GameObject[] array = objectsToBeHidden;
		for (int i = 0; i < array.Length; i++)
		{
			Helpers.GameObjectSetActive(array[i], enabled);
		}
		Helpers.GameObjectSetActive(survivorSupportsBackground, SurvivorType != SurvivorContainerModel.SurvivorType.GvGDefenders && enabled);
	}

	private IEnumerator SurvivorReplaced(SurvivorModel newSurvivorModel, int index)
	{
		yield return new WaitForSeconds(TeamSelectionSurvivorsList.CardFlyingTime);
		bool survivalMode = SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival;
		SetSurvivorAtCard(newSurvivorModel, index, locked: false, isTeaserSurvivor: false, survivalMode);
		UpdateCards();
	}

	private bool IsWorldBossTeamSelection()
	{
		if (SurvivorType != SurvivorContainerModel.SurvivorType.WorldBossPVE && SurvivorType != SurvivorContainerModel.SurvivorType.WorldBossPVP)
		{
			return SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss;
		}
		return true;
	}

	private void ConfigureSelectedSurvivorCardWorldBossDisplay(SurvivorCard card)
	{
		if (!(card == null))
		{
			card.WorldBossSelectedTeamTiredOnlyDisplay = IsWorldBossTeamSelection();
		}
	}

	private void SetSurvivorAtCard(SurvivorModel survivorModel, int cardIndex, bool locked, bool isTeaserSurvivor, bool survivalMode)
	{
		SurvivorCard component = GetSlotAt(cardIndex).GetComponent<SurvivorCard>();
		component.Item = survivorModel;
		component.Locked = locked;
		component.IsMissionSurvivor = isTeaserSurvivor;
		component.IsSurvivalMode = survivalMode;
		component.TeamSelectionSurvivorType = SurvivorType;
		component.EnableEquipmentContainers(enable: true);
		component.Type = SurvivorCard.CardType.TeamSelect;
		ConfigureSelectedSurvivorCardWorldBossDisplay(component);
		component.UpdateUI();
		component.ShowActorHitMessage();
		component.ShowTeamSelection(LocalizationManager.GetText("Popup.TeamSelection.TapToReplace"));
		indexToSurvivor[cardIndex] = survivorModel;
		component.gameObject.SetActive(survivorModel != null);
		if (emptySurvivorCards != null && emptySurvivorCards[cardIndex] != null)
		{
			emptySurvivorCards[cardIndex].gameObject.SetActive(survivorModel == null);
		}
		if (cardIndex == 0 && (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.Outpost || SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle || SurvivorType == SurvivorContainerModel.SurvivorType.GvGDefenders || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVE || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBossPVP || SurvivorType == SurvivorContainerModel.SurvivorType.WorldBoss))
		{
			component.SetLeaderTraitVisual(visible: true);
			if (survivorModel != null)
			{
				component.UpdateLeaderTraitVisual(survivorModel);
			}
		}
		else
		{
			component.SetLeaderTraitVisual(visible: false);
		}
	}

	private void UpdateCards()
	{
		if (currentTeam == null)
		{
			currentTeam = new List<SurvivorModel>();
		}
		else
		{
			currentTeam.Clear();
		}
		for (int i = 0; i < base.NumberSlots; i++)
		{
			SurvivorCard component = GetSlotAt(i).GetComponent<SurvivorCard>();
			if (component != null)
			{
				ConfigureSelectedSurvivorCardWorldBossDisplay(component);
				component.UpdateUI();
				component.ShowActorHitMessage();
				component.UpdateSurvivorUnavailableContainerState();
				currentTeam.Add(component.Item);
			}
		}
		UIEvent.Send("EventCurrentTeamUpdated", currentTeam);
	}

	public int GetSurvivorIndex(SurvivorModel survivor)
	{
		foreach (KeyValuePair<int, SurvivorModel> item in indexToSurvivor)
		{
			if (item.Value == survivor)
			{
				return item.Key;
			}
		}
		return -1;
	}

	public SurvivorModel GetSurvivorAtIndex(int index)
	{
		SurvivorModel value = null;
		if (indexToSurvivor.TryGetValue(index, out value))
		{
			return value;
		}
		return null;
	}

	public Vector3 GetFirstCardPosition()
	{
		return GetSlotAt(0).transform.position;
	}

	public Transform GetSelectedCard()
	{
		return GetSlotAt(selectedCardIndex).transform;
	}

	public void StartSurvivalRestAnimation(SurvivorModel survivor)
	{
		int survivorIndex = GetSurvivorIndex(survivor);
		if (survivorIndex != -1)
		{
			SurvivorCard component = GetSlotAt(survivorIndex).gameObject.GetComponent<SurvivorCard>();
			if (component != null)
			{
				component.StartSurvivalRestAnimation();
			}
		}
	}

	public void EndSurvivalRestAnimation(SurvivorModel survivor)
	{
		int survivorIndex = GetSurvivorIndex(survivor);
		if (survivorIndex != -1)
		{
			SurvivorCard component = GetSlotAt(survivorIndex).gameObject.GetComponent<SurvivorCard>();
			if (component != null)
			{
				component.EndSurvivalRestAnimation();
			}
		}
	}

	public void UpdateSurvivalRestAnimation(SurvivorModel survivor, float normalizedAnimationTime)
	{
		int survivorIndex = GetSurvivorIndex(survivor);
		if (survivorIndex != -1)
		{
			SurvivorCard component = GetSlotAt(survivorIndex).gameObject.GetComponent<SurvivorCard>();
			if (component != null)
			{
				component.UpdateSurvivalRestAnimation(normalizedAnimationTime);
			}
		}
	}

	public List<SurvivorModel> GetCurrentTeam()
	{
		return currentTeam;
	}
}
