using System;
using System.Collections;
using System.Collections.Generic;
using Client.Camp;
using TWDModel;
using UnityEngine;

public class TeamSelectionSurvivorsListPanel : SurvivorsListPanel
{
	[Tooltip("Time for the panel to appear.")]
	public float ApparitionTime;

	[Tooltip("Time for the card flying anim.")]
	public float CardFlyingTime;

	[SerializeField]
	[Tooltip("How many cards is the minimum to show. If not reached, will show empty cards.")]
	private int minCardsShown;

	[SerializeField]
	[Tooltip("Prefab for an empty survivor card.")]
	private GameObject emptySurvivorCardPrefab;

	[SerializeField]
	[Tooltip("Container that will contain the flying card when a survivor is added to the team.")]
	private Transform cardFlyingContainer;

	[SerializeField]
	[Tooltip("Survivor class filter position.")]
	private Transform survivorFilterPosition;

	[SerializeField]
	[Tooltip("Survivor class filter prefab.")]
	private GameObject survivorFilterPrefab;

	[SerializeField]
	private SupportSelectionPanel supportSelectionPanel;

	[SerializeField]
	private UIPanel scrollviewPanel;

	[SerializeField]
	private int scrollviewPanelRightOffset = -10;

	private SurvivorClassFilter survivorFilter;

	private List<SurvivorModel> survivorsExcludingTeam = new List<SurvivorModel>();

	public bool isAnimating;

	private MapMissionModel mapMissionModel;

	private MissionData missionData;

	private bool filterHidden;

	private TweenPosition panelTween;

	private Vector3 filterOffset;

	private List<SurvivorModel> currentTeam;

	public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

	public ISurvivorSlotProvider SurvivorSlotProvider { get; set; }

	public bool IncludeTeamSurvivors { get; set; }

	public void Start()
	{
		if (survivorFilter == null && survivorFilterPrefab != null)
		{
			GameObject gameObject = Helpers.InstantiateToParent(survivorFilterPrefab, base.gameObject);
			if (gameObject != null)
			{
				survivorFilter = gameObject.GetComponent<SurvivorClassFilter>();
			}
			if (survivorFilter != null && survivorFilterPosition != null)
			{
				survivorFilter.SurvivorList = this;
				survivorFilter.transform.localPosition = survivorFilterPosition.localPosition + filterOffset;
				if (base.gameObject.GetComponent<UIPanel>() != null)
				{
					survivorFilter.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
				}
			}
			SetupFilterForAvailableClasses();
			gameObject.SetActive(!filterHidden);
		}
		isAnimating = false;
		SetScrollViewPanelRightAnchorToHUDRight();
	}

	protected override List<SurvivorModel> GetSurvivors()
	{
		return survivorsExcludingTeam;
	}

	public void UpdateCards()
	{
		ResolveExcludedSurvivors();
		if (survivorFilter != null)
		{
			survivorFilter.SurvivorList = this;
		}
		SetupCardsByFiltering();
		SurvivorContainerModel survivorContainer = GameManager.Instance.playerModel.SurvivorContainer;
		for (int i = 0; i < ((cards != null) ? cards.Count : 0); i++)
		{
			SurvivorCard component = cards[i].GetComponent<SurvivorCard>();
			if (!(component != null))
			{
				continue;
			}
			if (mapMissionModel != null && missionData != null)
			{
				bool flag = missionData.HasCivilianActorIdContaining("daryl") && component.Item.ActorDefinitionID.ToLowerInvariant().Contains("daryl");
				component.SurvivorUnavailable = flag || !EndlessModeHelpers.IsSurvivorAvailableForCombat(component.Item, mapMissionModel);
				component.UpdateSurvivorUnavailableContainerState();
			}
			if (!OfflineManager.IsTutorialDisable)
			{
				if (TutorialView.Instance != null && !TutorialView.Instance.Model.StaticTutorialComplete)
				{
					component.ShowTeamSelection(LocalizationManager.GetText("Popup.TeamSelection.TapToPick"));
				}
			}
			string txt = null;
			if (SurvivorType != SurvivorContainerModel.SurvivorType.CombatSurvival)
			{
				if (SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
				{
					if (survivorContainer.CombatSurvivors.Contains(component.Item))
					{
						txt = LocalizationManager.GetText("Indicator.InTeam.TrainingGround");
					}
				}
				else if (survivorContainer.OutpostDefendingSurvivors.Contains(component.Item))
				{
					txt = LocalizationManager.GetText("Indicator.InDefendingTeam.TrainingGround");
				}
			}
			component.ShowInTeamIndicator(txt);
			component.IsSurvivalMode = SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival;
			if (component.IsSurvivalMode)
			{
				component.UpdateUI();
			}
		}
		if (survivorFilter != null)
		{
			survivorFilter.UpdatePositionAndState();
		}
		if (!OfflineManager.IsTutorialDisable)
		{
			if (TutorialView.Instance.Running && cardsContainer != null)
			{
				UIScrollView component2 = cardsContainer.GetComponent<UIScrollView>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
			}
		}
	}

	public override void SetupCardsByFiltering()
	{
		base.SetupCardsByFiltering();
		if (GetCards() != null && GetCards().Count < minCardsShown)
		{
			for (int i = GetCards().Count; i < minCardsShown; i++)
			{
				AddCard(null);
			}
			PositionCards();
		}
	}

	protected override GameObject CreateCard(SurvivorModel item)
	{
		if (item == null)
		{
			return Helpers.InstantiateToParent(emptySurvivorCardPrefab, cardsContainer);
		}
		GameObject obj = base.CreateCard(item);
		SurvivorCard component = obj.GetComponent<SurvivorCard>();
		component.Type = SurvivorCard.CardType.TeamSelect;
		if (OfflineManager.IsLoadDataManager) component.IsProtector = true;
		component.IsSurvivalMode = SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival;
		component.IsEndlessModeExpertActor = EndlessModeHelpers.IsSurvivorAvailableForCombat(item, mapMissionModel) && mapMissionModel != null && mapMissionModel.IsEndlessMission;
		component.SurvivorsFilterDelegate = () => new List<SurvivorModel>(SurvivorInfoPopup.GetSurvivorsFromCards(GetCards()));
		return obj;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		ClearCards();
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "EventSurvivorReplaced":
		{
			object[] array = (object[])parameter;
			ClosePanel(array[0] as SurvivorModel);
			break;
		}
		case "SurvivorDeleted":
		case "OnSurvivorFavouriteToggled":
		case "OnClickSurvivorFilter":
			UpdateCards();
			break;
		}
	}

	private void ResolveExcludedSurvivors()
	{
		survivorsExcludingTeam.Clear();
		PlayerModel model = GameManager.Instance.playerModel;
		List<SurvivorModel> survivorsForType = TeamSelectionPopup.GetSurvivorsForType(SurvivorType);
		for (int i = 0; i < model.SurvivorContainer.Survivors.Count; i++)
		{
			if (IncludeTeamSurvivors || !survivorsForType.Contains(model.SurvivorContainer.Survivors[i]))
			{
				survivorsExcludingTeam.Add(model.SurvivorContainer.Survivors[i]);
			}
		}
		if (model.Tutorial.CurrentPartId == "Phone")
		{
			survivorsExcludingTeam.Sort(delegate(SurvivorModel a, SurvivorModel b)
			{
				if (a != null && b != null)
				{
					if (a.SurvivorClass == SurvivorClass.Bruiser && b.SurvivorClass != SurvivorClass.Bruiser)
					{
						return -1;
					}
					if (a.SurvivorClass != SurvivorClass.Bruiser && b.SurvivorClass == SurvivorClass.Bruiser)
					{
						return 1;
					}
					if (a.SurvivorClass == SurvivorClass.Bruiser && b.SurvivorClass == SurvivorClass.Bruiser)
					{
						return model.SurvivorContainer.Survivors.IndexOf(b).CompareTo(model.SurvivorContainer.Survivors.IndexOf(a));
					}
					return a.SurvivorClass.CompareTo(b.SurvivorClass);
				}
				return 0;
			});
		}
		SetupFilterForAvailableClasses();
	}

	protected override void Sort()
	{
		cards.StableSort(delegate(UIListCard<SurvivorModel> a, UIListCard<SurvivorModel> b)
		{
			int num = 0;
			int num2 = 0;
			if (a.TryGetComponent<SurvivorCard>(out var component) && b.TryGetComponent<SurvivorCard>(out var component2))
			{
				num = component.GetSortValueForCombatType(currentTeam);
				num2 = component2.GetSortValueForCombatType(currentTeam);
			}
			if (num == num2)
			{
				return 0;
			}
			return (num <= num2) ? 1 : (-1);
		});
	}

	private void SetupFilterForAvailableClasses()
	{
		if (!(survivorFilter != null))
		{
			return;
		}
		SurvivorClassFilter component = survivorFilter.GetComponent<SurvivorClassFilter>();
		for (int i = 0; i < 6; i++)
		{
			component.EnableButtonForClass((SurvivorClass)i, enable: false);
		}
		foreach (SurvivorModel item in survivorsExcludingTeam)
		{
			component.EnableButtonForClass(item.SurvivorClass, enable: true);
		}
		component.UpdatePositionAndState();
	}

	public void OnClick()
	{
		if (!TutorialView.Instance.Running)
		{
			ClosePanel();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		}
	}

	public void OpenPanel(MissionData missionData = null, MapMissionModel mapMissionModel = null)
	{
		this.missionData = missionData;
		this.mapMissionModel = mapMissionModel;
		if (!isAnimating)
		{
			supportSelectionPanel.ClearCards();
			StartCoroutine(OpenPanelCoroutine());
		}
	}

	public void OpenPanelWithoutCards()
	{
		base.gameObject.SetActive(value: true);
		if (!isAnimating)
		{
			StartCoroutine(OpenPanelCoroutine(skipCardGeneration: true));
		}
	}

	private IEnumerator OpenPanelCoroutine(bool skipCardGeneration = false)
	{
		isAnimating = true;
		Vector3 panelHiddenPosition = GetPanelHiddenPosition();
		base.transform.localPosition = panelHiddenPosition;
		if (!skipCardGeneration)
		{
			UpdateCards();
		}
		filterHidden = skipCardGeneration;
		if ((bool)survivorFilter)
		{
			survivorFilter.gameObject.SetActive(!filterHidden);
		}
		StartCoroutine(CenterScrollableMapToNormalizedMapPosition());
		Transform firstSlotPosition = SurvivorSlotProvider.FirstSlotPosition;
		panelHiddenPosition.x = base.transform.parent.InverseTransformPoint(firstSlotPosition.position).x + firstSlotPosition.GetComponent<BoxCollider>().size.x / 2f;
		panelTween = TweenPosition.Begin(base.gameObject, ApparitionTime, panelHiddenPosition);
		yield return new WaitForSeconds(ApparitionTime);
		panelTween = null;
		isAnimating = false;
	}

	public void ClosePanel(SurvivorModel survivorChosen = null)
	{
		if (!isAnimating && panelTween == null)
		{
			StartCoroutine(ClosePanelCoroutine(survivorChosen));
		}
	}

	private IEnumerator ClosePanelCoroutine(SurvivorModel survivorChosen)
	{
		if (survivorChosen != null)
		{
			isAnimating = true;
			UIListCard<SurvivorModel> card = GetCard(survivorChosen);
			if (card != null)
			{
				GameObject cardToMove = card.gameObject;
				UIDragScrollView component = cardToMove.GetComponent<UIDragScrollView>();
				if (component != null)
				{
					component.enabled = false;
				}
				cardFlyingContainer.transform.localPosition = cardsContainer.transform.localPosition;
				cardToMove.transform.parent = cardFlyingContainer;
				NGUITools.MarkParentAsChanged(base.gameObject);
				TweenPosition.Begin(cardToMove, CardFlyingTime, cardToMove.transform.parent.InverseTransformPoint(SurvivorSlotProvider.SelectedSlotPosition.position));
				yield return new WaitForSeconds(CardFlyingTime);
				UnityEngine.Object.Destroy(cardToMove);
			}
		}
		UIEvent.Send("EventCloseSelectionPanel");
		TweenPosition.Begin(base.gameObject, ApparitionTime, GetPanelHiddenPosition());
		yield return new WaitForSeconds(ApparitionTime);
		base.gameObject.SetActive(value: false);
		isAnimating = false;
		UIEvent.Send("EventAnimationFinished");
	}

	private Vector3 GetPanelHiddenPosition()
	{
		Vector3 zero = Vector3.zero;
		zero.x = (float)Screen.width / 2f;
		return zero;
	}

	public void SetFilterOffset(Vector3 newFilterOffset)
	{
		filterOffset = newFilterOffset;
	}

	public void SetCurrentTeam(List<SurvivorModel> team)
	{
		currentTeam = team;
	}

	private void SetScrollViewPanelRightAnchorToHUDRight()
	{
		if (!(scrollviewPanel == null))
		{
			if (SingularityMonoBehaviour<HUDManager>.Instance.UIParent != null)
			{
				scrollviewPanel.rightAnchor.target = SingularityMonoBehaviour<HUDManager>.Instance.UIParent;
				scrollviewPanel.rightAnchor.absolute = scrollviewPanelRightOffset;
			}
			else
			{
				scrollviewPanel.rightAnchor.target = base.gameObject.transform.parent;
			}
		}
	}

	private IEnumerator CenterScrollableMapToNormalizedMapPosition()
	{
		yield return new WaitForSeconds(1f);
		try
		{
			float normalizedPos = GetNormalizedPos();
			UIPanel component = scrollviewPanel.GetComponent<UIPanel>();
			if (component == null)
			{
				yield break;
			}
			UIScrollView component2 = scrollviewPanel.GetComponent<UIScrollView>();
			if (!(component2 == null))
			{
				float num = component2.customBoundsForRestrict.extents.x * 2f;
				float num2 = 0f;
				if (num > 0f)
				{
					num2 = component.width / num;
				}
				float value = 0.5f + (normalizedPos - 0.5f) * (1f + num2);
				value = UtilsMath.Clamp(value, 0f, 1f);
				component.ResetAndUpdateAnchors();
				component2.SetDragAmount(value, 0f, updateScrollbars: false);
				component2.RestrictWithinBounds(instant: true);
				component2.UpdateScrollbars();
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"CenterScrollableMapToNormalizedMapPosition Error:{arg}");
		}
	}

	private float GetNormalizedPos()
	{
		float result = 0f;
		List<string> getCurrentActions = GameManager.Instance.playerModel.Tutorial.GetCurrentActions;
		if (getCurrentActions == null)
		{
			return result;
		}
		foreach (string item in getCurrentActions)
		{
			string[] array = item.Split(',');
			if (!(array[0] == "WaitClickButton"))
			{
				continue;
			}
			string text = array[1];
			text.Equals("SurvivorCardReserve");
			if (text.Contains("HeroSelect_"))
			{
				string[] array2 = text.Split('_');
				if (array2.Length != 0)
				{
					string heroName = array2[^1];
					result = GetNormalizedPosBySelectHero(heroName);
				}
			}
		}
		return result;
	}

	private float GetNormalizedPosBySelectHero(string heroName)
	{
		int count = cards.Count;
		if (count <= 1)
		{
			return 0f;
		}
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (cards[i].Item.Name.Equals(heroName))
			{
				num = i;
			}
		}
		return (float)num * 1f / (float)(count - 1);
	}
}
