using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class SurvivorsListPanel : ScrollableListPanel<SurvivorModel>
{
	[Header("Special cards")]
	[SerializeField]
	private GameObject heroLockedCard;

	private SurvivorListFilter filterSettings = new SurvivorListFilter();

	[SerializeField]
	[Tooltip("Prefab of the more slots card.")]
	private GameObject moreSlotsCardPrefab;

	[SerializeField]
	[Tooltip("Class info card prefab.")]
	private GameObject classInfoCardPrefab;

	public SurvivorModel SurvivorModel { get; set; }

	public SurvivorClass ClassFilter
	{
		get
		{
			return filterSettings.ClassFilter;
		}
		set
		{
			if (filterSettings.ClassFilter != value)
			{
				filterSettings.ClassFilter = value;
			}
		}
	}

	public SurvivorListFilter FilterSettings
	{
		get
		{
			return filterSettings;
		}
		set
		{
			filterSettings = value;
		}
	}

	public bool IsAcceptingSurvivor { get; set; }

	protected virtual List<SurvivorModel> GetSurvivors()
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		foreach (SurvivorModel survivor in GameManager.Instance.playerModel.SurvivorContainer.Survivors)
		{
			list.Add(survivor);
		}
		return list;
	}

	protected override GameObject CreateCard(SurvivorModel item)
	{
		GameObject gameObject = base.CreateCard(item);
		if (gameObject != null)
		{
			SurvivorCard component = gameObject.GetComponent<SurvivorCard>();
			if (component != null)
			{
				component.IsSurvivalMode = false;
			}
		}
		return gameObject;
	}

	protected override void SetCard(UIListCard<SurvivorModel> card)
	{
		if (card is SurvivorCard)
		{
			((SurvivorCard)card).EnableToggle();
		}
	}

	public virtual void SetupCardsByFiltering()
	{
		IEnumerable<SurvivorModel> enumerable = new List<SurvivorModel>();
		if (FilterSettings.TypeFilter == SurvivorListFilter.FilterType.SurvivorClass || FilterSettings.TypeFilter == SurvivorListFilter.FilterType.All)
		{
			if (FilterSettings.ClassFilter != SurvivorClass.None)
			{
				List<SurvivorModel> survivors = GetSurvivors();
				enumerable = enumerable.Union(survivors.Where((SurvivorModel survivor) => survivor.IsFavourite && survivor.SurvivorClass == ClassFilter)).Union(survivors.Where((SurvivorModel survivor) => !survivor.IsFavourite && survivor.SurvivorClass == ClassFilter));
				SetCards(enumerable);
				AddClassInfoCard();
			}
			else
			{
				List<SurvivorModel> survivors2 = GetSurvivors();
				enumerable = enumerable.Union(survivors2.Where((SurvivorModel survivor) => survivor.IsFavourite).Union(survivors2.Where((SurvivorModel survivor) => !survivor.IsFavourite)));
				SetCards(enumerable);
				AddLockedHeroCards(onlyUnlockableHeros: true);
			}
		}
		if (FilterSettings.TypeFilter == SurvivorListFilter.FilterType.Hero || FilterSettings.TypeFilter == SurvivorListFilter.FilterType.All)
		{
			if (FilterSettings.TypeFilter != SurvivorListFilter.FilterType.All)
			{
				ClearCards();
			}
			List<SurvivorModel> survivors3 = GetSurvivors();
			enumerable = enumerable.Union(survivors3.Where((SurvivorModel survivor) => survivor.IsFavourite && survivor.IsHero).Union(survivors3.Where((SurvivorModel survivor) => !survivor.IsFavourite && survivor.IsHero)));
			SetCards(enumerable);
			AddLockedHeroCards(FilterSettings.TypeFilter == SurvivorListFilter.FilterType.All);
		}
		PositionCards();
		UIEvent.Send("SurvivorListRefreshed");

		if (!IsInit) IsInit = true;
	}

	private void AddBuyMoreSlot()
	{
		if ((TutorialView.Instance.Running && !TutorialView.Instance.Model.HasCompletedPart("Tutorial_Training_Ground")) || !(moreSlotsCardPrefab != null))
		{
			return;
		}
		GameObject gameObject = Helpers.InstantiateToParent(moreSlotsCardPrefab, cardsContainer);
		if ((bool)gameObject)
		{
			SurvivorCardMoreSlots component = gameObject.GetComponent<SurvivorCardMoreSlots>();
			if (component != null)
			{
				component.Item = null;
				SetCard(component);
				component.UpdateUI();
				cards.Add(component);
			}
		}
	}

	private void AddClassInfoCard()
	{
		if ((TutorialView.Instance.Running && !TutorialView.Instance.Model.HasCompletedPart("Tutorial_Training_Ground")) || !(classInfoCardPrefab != null))
		{
			return;
		}
		GameObject gameObject = Helpers.InstantiateToParent(classInfoCardPrefab, cardsContainer);
		if ((bool)gameObject)
		{
			ClassInfoCard component = gameObject.GetComponent<ClassInfoCard>();
			if (component != null)
			{
				component.SetClassInfo(FilterSettings.ClassFilter, GameManager.Instance.playerModel.GetCurrency(SurvivorToken.GetClassAsCurrency(FilterSettings.ClassFilter)).Value);
				component.Item = null;
				SetCard(component);
				component.UpdateUI();
				cards.Add(component);
			}
		}
	}

	private void AddLockedHeroCards(bool onlyUnlockableHeros = false)
	{
		if (!(heroLockedCard != null))
		{
			return;
		}
		List<ActorDefinition> actorDefinitions = GameManager.Instance.playerModel.gameEconomyData.ActorDefinitions;
		for (int i = 0; i < actorDefinitions.Count; i++)
		{
			if (actorDefinitions[i] == null || !actorDefinitions[i].ID.ToLower().Contains("hero_") || GameManager.Instance.playerModel.SurvivorContainer.HasHero(actorDefinitions[i].ID) || (FilterSettings.ClassFilter != SurvivorClass.None && !(FilterSettings.ClassFilter.ToString() == actorDefinitions[i].Class)) || (onlyUnlockableHeros && !IsHeroUnlockPossible(actorDefinitions[i])))
			{
				continue;
			}
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(heroLockedCard, cardsContainer);
			if (gameObject != null)
			{
				SurvivorCardHeroLocked component = gameObject.GetComponent<SurvivorCardHeroLocked>();
				if (component != null)
				{
					component.SetActorDefinition(actorDefinitions[i]);
					SetCard(component);
					component.UpdateUI();
					cards.Add(component);
				}
			}
		}
	}

	private bool IsHeroUnlockPossible(ActorDefinition actorDefinition)
	{
		if (actorDefinition != null)
		{
			if (GameManager.Instance.playerModel.GetCurrency(actorDefinition.TraitUpgradeCurrency).Value >= actorDefinition.TokensToUnlock)
			{
				return actorDefinition.IsAvailableToUnlock(GameManager.Instance.playerModel.UtcTimeStamp);
			}
			return false;
		}
		return false;
	}

	public void RefreshCards()
	{
		SetupCardsByFiltering();
	}

	private void OnEnable()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if (component != null)
		{
			component.enabled = false;
		}
		UIEvent.OnUIEvent += OnUIEvent;
		SetupCardsByFiltering();
		IsAcceptingSurvivor = false;
		if (SurvivorModel == null)
		{
			SelectSurvivor(0);
		}
		else
		{
			SelectSurvivor(GetSurvivorIndex(SurvivorModel));
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewSurvivorSelected")
		{
			SurvivorModel = parameter as SurvivorModel;
		}
		else if (type == "SurvivorDeleted")
		{
			SetupCardsByFiltering();
			SelectSurvivor(0);
		}
	}

	private int GetSurvivorIndex(SurvivorModel survivorModel)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			if (getCardAt(i).Item == survivorModel)
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
			SurvivorModel = getCardAt(index).Item;
		}
		else
		{
			UIEvent.Send("OnNewSurvivorSelected");
		}
	}

	public void OnFilterClicked(GameObject filterButton)
	{
		if (filterButton != null)
		{
			SurvivorButtonFilter component = filterButton.GetComponent<SurvivorButtonFilter>();
			FilterSettings = new SurvivorListFilter();
			FilterSettings.TypeFilter = component.FilterType;
			FilterSettings.ClassFilter = component.SurvivorClass;
			ClassFilter = ((component != null) ? component.FilterSettings.ClassFilter : SurvivorClass.None);
		}
	}

	public SurvivorCard GetCardFromSurvivor(SurvivorModel survivor)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			SurvivorCard survivorCard = getCardAt(i) as SurvivorCard;
			if (survivorCard.Item == survivor)
			{
				return survivorCard;
			}
		}
		return null;
	}

	public void ClickCardFromActorDefinitionID(string id)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			UIListCard<SurvivorModel> cardAt = getCardAt(i);
			if (cardAt.Item != null && cardAt.Item.ActorDefinitionID == id)
			{
				SurvivorCard survivorCard = cardAt as SurvivorCard;
				if ((bool)survivorCard)
				{
					survivorCard.OnCardClicked();
				}
			}
			else
			{
				SurvivorCardHeroLocked survivorCardHeroLocked = cardAt as SurvivorCardHeroLocked;
				if ((bool)survivorCardHeroLocked && survivorCardHeroLocked.ActorDefinition.ID == id)
				{
					survivorCardHeroLocked.OnCardClicked();
				}
			}
		}
	}
	protected override void Sort()
	{
		base.Sort();
	}



	#region myparams
	private bool IsInit = false;
	#endregion

	#region mycode
	protected override void Awake()
	{
		base.Awake();
		FilterSettings.TypeFilter = SurvivorListFilter.FilterType.All;
	}
	#endregion
}
