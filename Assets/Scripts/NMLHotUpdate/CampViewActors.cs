using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CampViewActors : MonoBehaviour
{
	private const string INHABITANT_HOST_PREFAB_RESOURCE_NAME = "InhabitantHostResourceEntry";

	[Tooltip("If set, navigation mesh will be set in this object.")]
	public MeshRenderer DebugGrid;

	private List<ActorView> actors = new List<ActorView>();

	[SerializeField]
	[Tooltip("Maximum number of actors in the camp.")]
	private int maxActors;

	[SerializeField]
	[Tooltip("How many actors do we show per 100 inhabitants.")]
	private int actorsToInhabitantsRatio;

	private CampView campView;

	private List<CampActorLogic> logics = new List<CampActorLogic>();

	private int currentActor;

	private bool recalculateAfterEnable;

	private Coroutine actorCreationCoroutine;

	public NavigationMesh NavigationMesh { get; private set; }

	public List<ActorView> Actors => actors;

	public List<StoryTellerView> StoryTellerViews { get; private set; }

	public bool MoveCharacterImmediately { get; private set; }

	private void Awake()
	{
		campView = GetComponent<CampView>();
	}

	public void Initialize()
	{
		RebuildNavigationMesh();
		StoryTellerViews = new List<StoryTellerView>();
		CreateStoryTellerView(GameManager.Instance.playerModel.SurvivorContainer.StoryTeller);
		EventManager.OnEvent += OnEvent;
		GameManager.Instance.playerModel.SurvivorContainer.Changed += OnSurvivorsChanged;
		actorCreationCoroutine = GameManager.Instance.StartCoroutine(DelayedCreateCampActors());
	}

	private IEnumerator DelayedCreateCampActors()
	{
		SurvivorContainerModel survivors = GameManager.Instance.playerModel.SurvivorContainer;
		MoveCharacterImmediately = true;
		logics.Add(new CampActorUpgradeBuildingLogic());
		logics.Add(new CampActorUpgradeInWorkshopLogic());
		logics.Add(new CampActorGraveyardLogic());
		logics.Add(new CampActorGuardPositionLogic());
		logics.Add(new CampActorCutVegetationLogic());
		for (int i = 0; i < logics.Count; i++)
		{
			logics[i].Initialize();
		}
		yield return null;
		int i2 = 0;
		while (i2 < survivors.Survivors.Count)
		{
			if (survivors.Survivors[i2] != null)
			{
				CreateActorView(survivors.Survivors[i2]);
			}
			yield return null;
			int num = i2 + 1;
			i2 = num;
		}
		MoveCharacterImmediately = false;
		actorCreationCoroutine = null;
	}

	public void UpdateQuestIndicators()
	{
		if (StoryTellerViews != null)
		{
			for (int i = 0; i < StoryTellerViews.Count; i++)
			{
				StoryTellerViews[i].UpdateIndicators();
			}
		}
	}

	public void InitTutorialRunningCharacters()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("IntroPath");
		if (array == null || array.Length < actors.Count)
		{
			return;
		}
		if (GameManager.Instance.playerModel.Tutorial.CurrentStep < 1)
		{
			for (int i = 0; i < actors.Count; i++)
			{
				actors[i].GetComponent<CampActorController>().ForceMovement(array[i].GetComponent<CampWaypointPath>());
			}
			return;
		}
		for (int j = 0; j < actors.Count; j++)
		{
			CampWaypointPath component = array[j].GetComponent<CampWaypointPath>();
			actors[j].GetComponent<CampActorController>().ForceStand(component.Waypoints[component.Waypoints.Count - 1].GetComponent<CampWaypoint>());
		}
	}

	private void OnEnable()
	{
		if (recalculateAfterEnable)
		{
			recalculateAfterEnable = false;
			RebuildNavigationMesh();
		}
		MoveCharacterImmediately = true;
		for (int i = 0; i < logics.Count; i++)
		{
			logics[i].OnEnable();
		}
		MoveCharacterImmediately = false;
	}

	private void OnDisable()
	{
		ResetActors();
	}

	private void OnDestroy()
	{
		if (actorCreationCoroutine != null)
		{
			GameManager.Instance.StopCoroutine(actorCreationCoroutine);
		}
		for (int i = 0; i < logics.Count; i++)
		{
			logics[i].OnDestroy();
		}
		EventManager.OnEvent -= OnEvent;
		GameManager.Instance.playerModel.SurvivorContainer.Changed -= OnSurvivorsChanged;
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.SurvivorContainer != null && GameManager.Instance.playerModel.SurvivorContainer.StoryTeller != null)
		{
			GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.Changed -= OnStoryTellerChanged;
		}
	}

	public void ResetActors()
	{
		currentActor = 0;
		for (int i = 0; i < actors.Count; i++)
		{
			if (actors[i] != null)
			{
				CampActorController component = actors[i].GetComponent<CampActorController>();
				if (component != null)
				{
					component.Reset();
				}
			}
		}
	}

	private IEnumerator DelayedCreateActorView(ActorModel actorModel)
	{
		yield return null;
		CreateActorView(actorModel);
	}

	private void CreateActorView(ActorModel actorModel)
	{
		ActorView.PrepareActor(actorModel);
		bool flag = true;
		bool flag2 = false;
		GameObject gameObject;
		if (PlatformInfo.HasFlag(PlatformFlag.SlowCPU) || PlatformInfo.HasFlag(PlatformFlag.SlowGPU) || PlatformInfo.HasFlag(PlatformFlag.SDResolution) || flag)
		{
			gameObject = Object.Instantiate(GameManager.Instance.GetResources<InhabitantResourceEntry>((actorModel.Gender == ActorGender.Male) ? "InhabitantsMale" : "InhabitantsFemale").PrefabList.GetRandomElement());
			flag2 = true;
		}
		else if (GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Contains(actorModel as SurvivorModel))
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actorModel);
			ModularCharacter prefabOverrideForActor = ActorView.GetPrefabOverrideForActor(actorModel);
			gameObject = Helpers.InstantiateToParent(GameManager.Instance.CharacterTemplate, campView.BuildingsContainer.gameObject);
			gameObject.GetComponent<ModularCharacterCombiner>().GenerateCharacter(prefabForActor, prefabOverrideForActor, CharacterBuildType.Camp);
		}
		else
		{
			gameObject = Object.Instantiate(GameManager.Instance.GetResources<InhabitantResourceEntry>((actorModel.Gender == ActorGender.Male) ? "InhabitantsMale" : "InhabitantsFemale").PrefabList.GetRandomElement());
			flag2 = true;
		}
		if (flag2)
		{
			GameObject gameObject2 = Helpers.InstantiateToParent(UnityUtils.LoadFromAssetBundle<PrefabResource>("InhabitantHostResourceEntry", "scriptableobjects").GetPrefab(), campView.BuildingsContainer.gameObject);
			gameObject.transform.SetParent(gameObject2.transform);
		}
		ActorView component = gameObject.GetComponent<ActorView>();
		component.UseModelForInitialPosition = false;
		component.Initialize(actorModel);
		component.LightWeight = flag2;
		gameObject.AddComponent<CampActorController>();
		SurvivorAnimationController component2 = gameObject.GetComponent<SurvivorAnimationController>();
		if (component2 != null)
		{
			component2.SetController("Camp");
			component2.ForceIdle();
		}
		actors.Add(gameObject.GetComponent<ActorView>());
	}

	private void RemoveActorView(SurvivorModel survivor)
	{
		ActorView actorView = GetActorView(survivor);
		if (actorView != null)
		{
			Object.Destroy(actorView.gameObject);
			actors.Remove(actorView);
		}
	}

	public ActorView GetActorView(ActorModel actorModel)
	{
		for (int i = 0; i < actors.Count; i++)
		{
			if (actors[i].Model == actorModel)
			{
				return actors[i];
			}
		}
		return null;
	}

	private void CreateStoryTellerView(StoryTellerModel storyTellerModel)
	{
		ActorResourceEntry resources = GameManager.Instance.GetResources<ActorResourceEntry>(storyTellerModel.Definition.ID);
		if (resources == null)
		{
			Debug.LogError("Could not find resources for actor prefab list " + storyTellerModel.Definition.ID + "!");
			return;
		}
		if (resources.PrefabResourceList == null)
		{
			Debug.LogError("Could not load prefab list for actor" + storyTellerModel.Definition.ID + "!");
			return;
		}
		StoryTellerView component = Helpers.InstantiateToParent(resources.GetRandomPrefab(), campView.BuildingsContainer.gameObject).GetComponent<StoryTellerView>();
		component.UseModelForInitialPosition = false;
		component.Initialize(storyTellerModel);
		StoryTellerViews.Add(component);
		storyTellerModel.Changed += OnStoryTellerChanged;
	}

	public void OnStoryTellerChanged(ModelObject model, string changed, object args)
	{
		if (changed == "StoryTellerChanged")
		{
			for (int i = 0; i < StoryTellerViews.Count; i++)
			{
				if (StoryTellerViews[i] != null)
				{
					Object.Destroy(StoryTellerViews[i].gameObject);
					actors.Remove(StoryTellerViews[i]);
				}
			}
			StoryTellerViews.Clear();
			CreateStoryTellerView(GameManager.Instance.playerModel.SurvivorContainer.StoryTeller);
		}
		else if (changed == "QuestCompleted")
		{
			GameManager.Instance.RequestPltv();
		}
	}

	private IEnumerator DelayedRebuildNavigationMesh()
	{
		yield return null;
		RebuildNavigationMesh();
	}

	public ActorView GetNextFreeSurvivor()
	{
		if (actors.Count == 0)
		{
			return null;
		}
		currentActor = (currentActor + 1) % actors.Count;
		if (actors[currentActor].GetComponent<CampActorController>().IsAvailable)
		{
			return actors[currentActor];
		}
		return null;
	}

	public void RebuildNavigationMesh()
	{
		CampWaypoint[] componentsInChildren = campView.CampViewBuildings.GetComponentsInChildren<CampWaypoint>();
		List<BuildingView> buildings = campView.CampViewBuildings.Buildings;
		List<Vector2> list = new List<Vector2>(buildings.Count * 4 + componentsInChildren.Length);
		for (int i = 0; i < buildings.Count; i++)
		{
			if (buildings[i].BuildingType == "Cage")
			{
				continue;
			}
			BoxCollider componentInChildren = buildings[i].GetComponentInChildren<BoxCollider>();
			if (!(componentInChildren == null))
			{
				Vector3 center = componentInChildren.center;
				Vector3 vector = componentInChildren.size * 0.5f;
				vector *= 1.025f;
				Vector3[] array = new Vector3[4]
				{
					new Vector3(center.x - vector.x, 0f, center.z - vector.z),
					new Vector3(center.x + vector.x, 0f, center.z - vector.z),
					new Vector3(center.x - vector.x, 0f, center.z + vector.z),
					new Vector3(center.x + vector.x, 0f, center.z + vector.z)
				};
				for (int j = 0; j < 4; j++)
				{
					Vector3 vector2 = buildings[i].transform.TransformPoint(array[j]);
					list.Add(new Vector2(vector2.x, vector2.z));
				}
			}
		}
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			list.Add(componentsInChildren[k].GetVector2Position);
		}
		Vector2[] points = list.ToArray();
		NavigationMesh = new NavigationMesh();
		NavigationMesh.RebuildMesh(points);
		if (DebugGrid != null)
		{
			NavigationMesh.CreateDebugMesh(DebugGrid);
		}
	}

	private void OnEvent(EventManager.EventType type, object parameter)
	{
		if (type == EventManager.EventType.CampVisualizationChanged)
		{
			if (base.gameObject.activeSelf)
			{
				GameManager.Instance.StartCoroutine(DelayedRebuildNavigationMesh());
			}
			else
			{
				recalculateAfterEnable = true;
			}
		}
	}

	private void OnSurvivorsChanged(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "addSurvivor":
			GameManager.Instance.StartCoroutine(DelayedCreateActorView(args as SurvivorModel));
			break;
		case "survivorDemoted":
			if (actors.Count < maxActors && base.gameObject.activeInHierarchy && actors.Count < GameManager.Instance.playerModel.GetCurrency(CurrencyType.Inhabitants).Value * actorsToInhabitantsRatio / 100)
			{
				GameManager.Instance.StartCoroutine(DelayedCreateActorView(args as SurvivorModel));
			}
			break;
		case "survivorDied":
			RemoveActorView(args as SurvivorModel);
			if (args is SurvivorModel survivorModel)
			{
				TwoOptionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TwoOptionPopup) as TwoOptionPopup;
				obj.SetContent(LocalizationManager.GetText("Popup.TraitExpired.Bitten.Title"), survivorModel.Name + " " + LocalizationManager.GetText("Popup.TraitExpired.Bitten.Content"));
				obj.SetOption1ButtonLabel(LocalizationManager.GetText("Popup.TraitExpired.Bitten.Confirmation"));
				obj.SetOption2ButtonLabel(LocalizationManager.GetText("Popup.TraitExpired.Bitten.Confirmation"));
				obj.SetCallbacks(OnDoNothing, OnDoNothing);
				obj.Open();
			}
			break;
		}
	}

	private void OnDoNothing()
	{
	}
}
