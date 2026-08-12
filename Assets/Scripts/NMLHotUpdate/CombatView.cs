using BaseModel;
using Client.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TWDModel;
using UnityEngine;
using UnityEngine.Rendering;

public class CombatView : ModelView<CombatModel>
{
	private const string MagazineGlowMaterialName = "lootGlowPlane_MAT";

	private const string SceneDependenciesBundleName = "scene_dependencies";

	public static bool SkipAskGore;

	private static CombatView instance;

	private CombatHUD combatHUD;

	private bool updateVisibility = true;

	private List<ActorView> actorViews = new List<ActorView>();

	private List<ActorView> deadBodies = new List<ActorView>();

	private bool combatEndRequested;

	private ECombatResult combatEndResult;

	private bool missionEndedPending;

	private GameObject exitsContainer;

	public GameObject ExitAreaFence;

	[Tooltip("Combat End Hooray Animation Clips")]
	public AnimationClip[] HoorayClips;

	private DelayedNotificationVisualizationTask showHandDelayedTask;

	private long combatStartUpTime;

	public bool ranOutOfTime;

	private bool outOfTurnsPopupShown;

	private bool missionCompletedOnReload;

	private float visibilityRefreshTimer;

	private float visibilityRefreshInterval = 0.2f;

	private CombatEndScreenHandler endScreenHandler;

	private bool tutorialStarting;

	private CombatModelViewResources modelViewResources;

	private List<WaveIndicatorGroup> WaveIndicators;

	private List<IHardCodedTutorial> hardCodedTutorials;

	private TraitActiveVisualizationsManager traitActiveVisualizationsManager;

	private Transform clapRoot;

	private Material magazineGlowMaterial;

	private bool isPlayerInputEnabled;

	public static CombatView Instance => instance;

	public CombatHUD CombatHUD
	{
		get
		{
			return combatHUD;
		}
		private set
		{
			combatHUD = value;
		}
	}

	public bool UpdateVisibility
	{
		get
		{
			return updateVisibility;
		}
		set
		{
			updateVisibility = value;
			if (updateVisibility)
			{
				RefreshVisibilityStatus();
			}
		}
	}

	public GroundType CurrentMissionGroundType { get; private set; }

	public AmbienceType CurrentMissionAmbienceType { get; private set; }

	public ThreatMeterIndicator ThreatMeter { get; private set; }

	public ChargeMeterIndicator ChargeMeter { get; private set; }

	public CombatTurnPanel TurnPanel { get; private set; }

	private WalkerTurnNotification walkerTurnNotification { get; set; }

	private FadeOutNotification fadeOut { get; set; }

	private MissionObjectiveView missionObjectiveView { get; set; }

	private int MaxDeadBodies
	{
		get
		{
			if (!PlatformInfo.HasFlag(PlatformFlag.LowMemory))
			{
				return 10;
			}
			return 5;
		}
	}

	public Faction CurrentViewFaction { get; private set; }

	public bool CombatWasResumed { get; set; }

	public bool IsPlayerInputEnabled
	{
		get
		{
			return isPlayerInputEnabled;
		}
		private set
		{
			isPlayerInputEnabled = value;
		}
	}

	private bool IsPlayerInputEnabledPending { get; set; }

	public override void Initialize(ModelObject model)
	{
		DebugTWD.Log("Initialize CombatView " + this.transform.name, DebugType.ActivateObject);

		CurrentViewFaction = Faction.Survivor;
		CombatWasResumed = false;
		base.Initialize(model);
		UpdateVisibility = true;
		if (model != GameManager.Instance.playerModel.Combat)
		{
			Debug.LogError("CombatView is being initialized with WRONG MODEL!!!!");
		}
		modelViewResources = UnityUtils.LoadFromAssetBundle<CombatModelViewResources>("CombatModelViewResources", "scriptableobjects");
		model.Changed += OnModelChange;
		combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		combatHUD.Open();
		combatHUD.HideMoveActionIndicator();
		combatHUD.HideCoverMoveIndicator();
		EventManager.NotifyEvent(EventManager.EventType.StateTransitionCompleted);
		GameManager.Instance.SetState(GameState.Combat);
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		SingularityMonoBehaviour<AudioManager>.Instance.SetForcedMusicMuteState(base.Model.MusicMuteForced);
		if (combat.IsGuildBattleMission)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StartCombatLoopingSounds(CurrentMissionAmbienceType, AmbienceType.Gvg.ToString().ToLower());
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StartCombatLoopingSounds(CurrentMissionAmbienceType);
		}
		combat.TurnManager.ActorChanged += OnActiveActorChanged;
		combat.AbilityManager.AbilityPerformed += OnAbilityPerformed;
		GridView.Instance.Initialize(combat.Grid);
		RefreshGridDependentModelViews();
		CreateClapPrefabs();
		int num = Enum.GetNames(typeof(Faction)).Length;
		for (int i = 0; i < num; i++)
		{
			if (i == 0)
			{
				continue;
			}
			foreach (ActorModel factionActor in combat.GetFactionActors((Faction)i))
			{
				CreateActorView(factionActor);
			}
		}
		CreateObjectiveView(combat.CurrentMissionObjective, 0);
		FogOfWarVisualization fogOfWarVisualization = UnityEngine.Object.FindObjectOfType<FogOfWarVisualization>();
		if (fogOfWarVisualization != null)
		{
			fogOfWarVisualization.Initialize();
		}
		CreateTurnPanel();
		FocusCameraOnActiveActor(forceFocus: true);
		combat.TurnManager.FactionChanging += OnFactionChanging;
		combat.TurnManager.FactionChanged += OnFactionChanged;
		IsPlayerInputEnabledPending = true;
		base.Model.UpdateDynamicColliders();
		SingularityMonoBehaviour<AudioManager>.Instance.UnloadAudio("CampSfx");
		endScreenHandler = new CombatEndScreenHandler(base.Model);
		EventManager.OnEvent += OnEventManagerEvent;
		if (PortraitManager.Instance != null && PortraitManager.Instance.IsActive)
		{
			StartCoroutine(DelayedUpdatePortraits());
		}
		walkerTurnNotification = combatHUD.CreateWalkerTurnNotificationIndicator();
		walkerTurnNotification.AnimationCompleted += OnWalkerTurnNotificationCompleted;
		fadeOut = combatHUD.CreateFadeOutObject();
		fadeOut.gameObject.SetActive(value: false);
		if (base.Model.MissionCompleted)
		{
			missionCompletedOnReload = true;
			RequestEndCombat(base.Model.MissionResult);
		}
		ShowSuggestedInteractionTarget();
		UpdateNewUserTOSAccepted();

		if (!OfflineManager.IsLoadDataManager)
		{
			if (!GameManager.Instance.GameCenterManager.Authenticated && (!TutorialView.Instance.Running || TutorialView.Instance.Model.CurrentPartId != "InitialCombat"))
			{
				GameManager.Instance.GameCenterManager.PromptGameCenterConnect();
			}
			if (GameManager.Instance.gameEconomyData.ConfigData.GoreDisabledCountryCodes != null && GameManager.Instance.gameEconomyData.ConfigData.GoreDisabledCountryCodes.Contains(GameManager.GetCountryCode()))
			{
				if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.GoreUsed"))
				{
					Helpers.ExecuteCommand(new ChangeGoreSettingCommand(enableGore: false));
				}
			}
			else if (GameManager.ActiveBranch != "develop" && !SkipAskGore && GameManager.Instance.gameEconomyData.ConfigData.AskForGore && !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.GoreUsed"))
			{
				GoreSettingPopup goreSettingPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GoreSettingPopup) as GoreSettingPopup;
				if (goreSettingPopup != null)
				{
					goreSettingPopup.Open();
				}
			}
		}
		else
		{
			DebugTWD.Log("Отключаем GoreSettingPopup (включить кровь). Позже проверить работу", DebugType.Wars);
		}

		if (base.Model.HasPvPRules)
		{
			StartCombatTimer();
			int survivorsIncapacitated = 0;
			int survivorsInExits = 0;
			base.Model.GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
			ECombatResult pvpResult = base.Model.GetPvpResult(survivorsIncapacitated, base.Model.Survivors.Count);
			combatHUD.ChangeMissionButtonState(pvpResult != ECombatResult.Failed, pvpResult);
		}
		if (base.Model.CombatRetryChoicePendingState != MissionRetryState.None)
		{
			combatHUD.SetupSurvivorPortraits();
			missionEndedPending = true;
		}
		tutorialStarting = false;
		foreach (TWDModelObject model2 in combat.Models.Models)
		{
			SpawnModelView(model2);
		}
		if (base.Model.IsEndlessBattleMission)
		{
			InitWaveIndicators();
		}
		if (!OfflineManager.IsTutorialDisable)
		{
			if (EndlessModeCombatTutorial.CanStartTutorial())
			{
				if (hardCodedTutorials == null)
				{
					hardCodedTutorials = new List<IHardCodedTutorial>();
				}
				hardCodedTutorials.Add(new EndlessModeCombatTutorial());
			}
		}

		traitActiveVisualizationsManager = new TraitActiveVisualizationsManager(combat);
	}

	private void UpdateNewUserTOSAccepted()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			if (playerModel != null && !playerModel.HasTakenGdprAction("NewUserTOSAccepted") && TWDPlayerPrefs.GetInt("NewUserTOSAccepted") == 1)
			{
				long result = 0L;
				if (long.TryParse(TWDPlayerPrefs.GetString("TOSAcceptedTimestamp"), out result))
				{
					Helpers.ExecuteCommand(new SetGdprStateCommand("NewUserTOSAccepted", accepted: true, result));
				}
			}
		}
		else
		{
			DebugTWD.Log("UpdateNewUserTOSAccepted is disabled", DebugType.ActivateObject);
		}
	}

	private long GetCombatTimeSinceStartup()
	{
		if (base.Model.CombatStartTime > 0)
		{
			return (GameManager.Instance.playerModel.UtcTimeStamp - base.Model.CombatStartTime) / 1000;
		}
		return 0L;
	}

	public void ResetTimer()
	{
		if (missionObjectiveView != null)
		{
			missionObjectiveView.Reset();
		}
	}

	private void RefreshGridDependentModelViews()
	{
		List<MovableView> views = GameManager.Instance.GetViews<MovableView>();
		if (views != null && views.Count > 0)
		{
			for (int i = 0; i < views.Count; i++)
			{
				views[i].RefreshPosition();
			}
		}
	}

	private void OnEventManagerEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype == EventManager.EventType.CombatStart)
		{
			if (!OfflineManager.IsFakeExecuteCommands)
			{
				EventManager.NotifyEvent(EventManager.EventType.CombatStartTutorial);
				Helpers.ExecuteCommand(new StartCombatCommand
				{
					MissionNameEnglish = base.Model.CurrentMissionTextID
				});
			}
			else
			{
				TWDModelResult result = TWDModelResult.Error;
				CombatModel combatModel = GameManager.Instance.playerModel.Combat;
				if (combatModel != null)
				{
					combatModel.MissionNameEnglish = base.Model.CurrentMissionTextID;
					result = combatModel.StartCombat();
				}
				if (result != TWDModelResult.OK)
				{
					DebugTWD.LogError("Error Starting Combat");
				}
			}
			EventManager.OnEvent -= OnEventManagerEvent;
		}
	}

	private IEnumerator DelayedUpdatePortraits()
	{
		int frames = 0;
		while (PortraitManager.Instance.IsActive)
		{
			int num = frames + 1;
			frames = num;
			if (num > 30)
			{
				yield break;
			}
			yield return null;
		}
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null && combat.ActiveActor != null)
		{
			ShowHUDForSelectedActor(combat.ActiveActor);
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		MissionSettings missionSettings = UnityEngine.Object.FindObjectOfType<MissionSettings>();
		if (missionSettings != null)
		{
			CurrentMissionGroundType = missionSettings.Ground;
			CurrentMissionAmbienceType = missionSettings.Ambience;
		}
	}

	public void OnDestroy()
	{
		instance = null;
		EventManager.OnEvent -= OnEventManagerEvent;
		CombatModel combatModel = ((GameManager.Instance != null && GameManager.Instance.playerModel != null) ? GameManager.Instance.playerModel.Combat : null);
		if (combatModel != null)
		{
			combatModel.Changed -= OnModelChange;
			if (combatModel.TurnManager != null)
			{
				combatModel.TurnManager.FactionChanging -= OnFactionChanging;
				combatModel.TurnManager.FactionChanged -= OnFactionChanged;
				combatModel.TurnManager.ActorChanged -= OnActiveActorChanged;
			}
			if (combatModel.AbilityManager != null)
			{
				combatModel.AbilityManager.AbilityPerformed -= OnAbilityPerformed;
			}
			for (int i = 0; i < (hardCodedTutorials?.Count ?? 0); i++)
			{
				hardCodedTutorials[i].DeregisterListeners();
			}
		}
		if (combatHUD != null)
		{
			combatHUD.OnAbilitySelected -= OnAbilitySelected;
			combatHUD.Close();
		}
		DisableCombatAudioListener();
	}

	public void ShowHUDForSelectedActor(ActorModel actor)
	{
		if (actor == null)
		{
			return;
		}
		if (actor.Faction == Faction.Survivor)
		{
			if (VisualizationQueue.Instance.HasTaskOfType<FactionChangeVisualizationTask>())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
				{
					(GameManager.Instance.GetViewForModel(actor) as ActorView).CreateSelectionChangedIndicator();
				}));
			}
			else
			{
				ActorView actorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
				if (actorView != null)
				{
					actorView.CreateSelectionChangedIndicator();
				}
			}
		}
		if (actor.Faction == Faction.Survivor && CurrentViewFaction == Faction.Survivor)
		{
			combatHUD.HideEnemyTurnHUD();
			combatHUD.SetSurvivorTurnHUD(actor);
		}
	}

	public void CheckEndTurnButtonHighlight()
	{
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		List<ActorModel> factionActors = base.Model.GetFactionActors(Faction.Survivor);
		for (int i = 0; i < factionActors.Count; i++)
		{
			ActorModel actorModel = factionActors[i];
			if (!actorModel.TurnComplete && !base.Model.IsActorInValidExit(actorModel))
			{
				flag2 = true;
			}
			if (!actorModel.TurnComplete && base.Model.IsActorInValidExit(actorModel))
			{
				flag3 = true;
			}
		}
		flag = !flag2 && flag3 && !base.Model.MissionCompleted;
		TurnPanel.SetEndTurnButtonHighlight(flag);
	}

	private void OnAbilitySelected(AbilityModel ability, ActorModel sourceActor)
	{
		AbilityActorTuple selectedAbilityToDisplayTargetCells = null;
		if (ability != null)
		{
			selectedAbilityToDisplayTargetCells = new AbilityActorTuple(ability, sourceActor);
		}
		if (GridView.Instance != null)
		{
			GridView.Instance.SelectedAbilityToDisplayTargetCells = selectedAbilityToDisplayTargetCells;
		}
		else
		{
			DebugTWD.LogError("GridView.Instance is null", DebugType.Error);
		}
	}

	public ActorView GetActorViewFromModel(ActorModel actorModel)
	{
		foreach (ActorView actorView in actorViews)
		{
			if (actorView.Model == actorModel)
			{
				return actorView;
			}
		}
		return null;
	}

	private void CreateClapPrefabs()
	{
		Scenario scenario = UnityEngine.Object.FindObjectOfType<Scenario>();
		if (scenario == null)
		{
			Debug.LogError("Could not find Scenario when removing Clap prefab.");
			return;
		}
		clapRoot = scenario.transform.Find("Clap");
		if (clapRoot == null)
		{
			clapRoot = new GameObject("Clap").transform;
			clapRoot.SetParent(scenario.transform, worldPositionStays: false);
		}
		clapRoot.localPosition = Vector3.zero;
		clapRoot.localRotation = Quaternion.identity;
		clapRoot.localScale = Vector3.one;
		List<MagazineArea> magazineAreas = GetMagazineAreas();
		RemoveClapPrefabInstance(magazineAreas);
	}

	private void CreateClapPrefabInstance(MagazineArea magazineArea)
	{
		GridCoordinate effectiveAreaGridCoordinate = magazineArea.EffectiveAreaGridCoordinate;
		GameObject gameObject = LoadRifleClapPrefab(magazineArea.Faction);
		if (gameObject == null)
		{
			return;
		}
		string clapName = GetClapName(effectiveAreaGridCoordinate);
		if (!(clapRoot.Find(clapName) != null))
		{
			Vector3 localScale = Vector3.one * 0.5f;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, clapRoot);
			gameObject2.name = clapName;
			gameObject2.transform.position = GridView.Instance.GetPosition(effectiveAreaGridCoordinate).ToVector3();
			gameObject2.transform.position += new Vector3(0.3f, 0f, -0.3f);
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = localScale;
			RifleClapClickHandler rifleClapClickHandler = gameObject2.GetComponent<RifleClapClickHandler>();
			if (rifleClapClickHandler == null)
			{
				rifleClapClickHandler = gameObject2.AddComponent<RifleClapClickHandler>();
			}
			rifleClapClickHandler.Initialize(LocalizationManager.GetText("CombatResupply_Popup"), magazineArea.Faction);
			CreateMagazineGlowOverlay(gameObject2);
			Collider[] componentsInChildren = gameObject2.GetComponentsInChildren<Collider>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			Collider2D[] componentsInChildren2 = gameObject2.GetComponentsInChildren<Collider2D>(includeInactive: true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].enabled = false;
			}
			UIButton component = gameObject2.GetComponent<UIButton>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}

	private GameObject LoadRifleClapPrefab(Faction faction)
	{
		string text = ((faction != Faction.Survivor) ? "Trait_CombatResupply_b_PREFAB" : "Trait_CombatResupply_PREFAB");
		GameObject obj = UnityUtils.LoadFromAssetBundle<GameObject>(text, "scene_dependencies");
		if (obj == null)
		{
			Debug.LogError("Could not load " + text + " prefab from bundle scene_dependencies.");
		}
		return obj;
	}

	private void CreateMagazineGlowOverlay(GameObject sourceObject)
	{
		Material material = LoadMagazineGlowMaterial();
		if (sourceObject == null || material == null)
		{
			return;
		}
		MeshFilter[] componentsInChildren = sourceObject.GetComponentsInChildren<MeshFilter>(includeInactive: true);
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			if (!(meshFilter.GetComponent<MeshRenderer>() == null) && !(meshFilter.sharedMesh == null))
			{
				GameObject obj = new GameObject(meshFilter.gameObject.name + "_InteractionIndicator");
				obj.transform.SetParent(meshFilter.transform, worldPositionStays: false);
				MeshFilter meshFilter2 = obj.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
				UvScroll uvScroll = obj.AddComponent<UvScroll>();
				meshFilter2.sharedMesh = meshFilter.sharedMesh;
				meshRenderer.sharedMaterial = material;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.receiveShadows = false;
				meshRenderer.lightProbeUsage = LightProbeUsage.Off;
				meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				uvScroll.uvScrollSpeed = new Vector2(0.5f, 0f);
			}
		}
	}

	private Material LoadMagazineGlowMaterial()
	{
		if (magazineGlowMaterial != null)
		{
			return magazineGlowMaterial;
		}
		string[] array = new string[4] { "scene_dependencies", "buildingsprefabs_dependencies", "prefabresources", "prefabresources_dependencies" };
		foreach (string bundleName in array)
		{
			magazineGlowMaterial = UnityUtils.LoadFromAssetBundle<Material>("lootGlowPlane_MAT", bundleName);
			if (magazineGlowMaterial != null)
			{
				return magazineGlowMaterial;
			}
		}
		Debug.LogWarning("Could not load glow material lootGlowPlane_MAT for magazine area overlay.");
		return null;
	}

	public bool ShowMagazineAreaTooltip(GridCoordinate coordinate)
	{
		if (clapRoot == null)
		{
			return false;
		}
		Transform transform = clapRoot.Find(GetClapName(coordinate));
		if (transform == null)
		{
			return false;
		}
		RifleClapClickHandler component = transform.GetComponent<RifleClapClickHandler>();
		if (component != null)
		{
			return component.HandleClick();
		}
		return false;
	}

	private List<MagazineArea> GetMagazineAreas()
	{
		List<MagazineArea> list = new List<MagazineArea>();
		MagazineAreasManager model = GameManager.Instance.playerModel.Combat.GetModel<MagazineAreasManager>();
		if (model?.ExistedMagazineAreas == null || model.ExistedMagazineAreas.Count == 0)
		{
			return list;
		}
		list.AddRange(model.ExistedMagazineAreas);
		return list;
	}

	private string GetClapName(GridCoordinate coordinate)
	{
		return $"Rifle_Clap_{coordinate.X}_{coordinate.Y}";
	}

	private void RemoveClapPrefabInstance(List<MagazineArea> magazineAreas)
	{
		List<MagazineArea> list = magazineAreas ?? new List<MagazineArea>();
		HashSet<string> hashSet = new HashSet<string>(list.Select((MagazineArea area) => area.EffectiveAreaGridCoordinate).ToList().Select(GetClapName));
		if (clapRoot == null)
		{
			return;
		}
		foreach (MagazineArea item in list)
		{
			if (clapRoot.Find(GetClapName(item.EffectiveAreaGridCoordinate)) == null)
			{
				CreateClapPrefabInstance(item);
			}
		}
		List<Transform> list2 = new List<Transform>();
		foreach (Transform item2 in clapRoot)
		{
			if (!hashSet.Contains(item2.name))
			{
				list2.Add(item2);
			}
		}
		foreach (Transform item3 in list2)
		{
			TooltipManager.HideAll(item3.gameObject);
			UnityEngine.Object.Destroy(item3.gameObject);
		}
	}

	private bool HasModularCharacter(ActorModel actorModel)
	{
		Faction faction = actorModel.Faction;
		if (faction == Faction.Survivor || faction == Faction.Civilian || (faction == Faction.Raider && base.Model.IsPVPMission) || (faction == Faction.Raider && actorModel.UseModularCharacter))
		{
			return true;
		}
		if (faction == Faction.Lure && actorModel.Definition.Class != "Props")
		{
			return true;
		}
		if (GameManager.Instance.HasResources<ActorResourceEntry>(actorModel.Definition.ID))
		{
			return false;
		}
		return true;
	}

	private void CreateActorView(ActorModel actorModel)
	{
		if (actorModel.Definition == null)
		{
			Debug.LogWarning("Not actor definition found for actor " + actorModel.ActorDefinitionID);
			return;
		}
		GameObject gameObject = null;
		gameObject = ((!HasModularCharacter(actorModel)) ? CreateActorsResourceCharacter(actorModel) : CreateModularCharacter(actorModel));
		if (!(gameObject == null))
		{
			FixedVec3 position = GridView.Instance.GetPosition(actorModel.GridCoordinate);
			gameObject.transform.parent = UnityEngine.Object.FindObjectOfType<Scenario>().transform;
			gameObject.transform.localPosition = position.ToVector3();
			ActorView component = gameObject.GetComponent<ActorView>();
			component.Initialize(actorModel);
			actorViews.Add(component);
			if (actorModel.Faction == Faction.Raider)
			{
				component.SetVisible(visible: false);
			}
			ChargeMeterView chargeMeterView = new GameObject("ChargeMeterView").AddComponent<ChargeMeterView>();
			chargeMeterView.Initialize(actorModel.ChargeMeter);
			chargeMeterView.transform.parent = component.gameObject.transform;
		}
		else
		{
			DebugTWD.LogError("Has not ModularCharacter", DebugType.Error);
		}
	}

	private GameObject CreateModularCharacter(ActorModel actor)
	{
		GameObject gameObject = null;
		string text = "";
		if (actor is WalkerModel)
		{
			WalkerModel walkerModel = actor as WalkerModel;
			if (walkerModel.VisualVariation != WalkerVisualization.Normal)
			{
				text = walkerModel.VisualVariation.ToString();
			}
		}
		if (actor is RaiderModel)
		{
			RaiderModel raiderModel = actor as RaiderModel;
			if (raiderModel.VisualVariation != RaiderVisualization.Normal)
			{
				text = raiderModel.VisualVariation.ToString();
			}
		}
		CharacterResourceEntry resources = GameManager.Instance.GetResources<CharacterResourceEntry>(actor.Definition.ID + text);
		if (resources == null)
		{
			Debug.LogError("Could not find resources for actor prefab list " + actor.Definition.ID + text + "!");
			return null;
		}
		if (resources != null)
		{
			ActorView.PrepareActor(actor);
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actor);
			if (prefabForActor == null)
			{
				Debug.LogError("Could not get prefab  for actor " + actor.Definition.ID + text + "!");
				return null;
			}
			ModularCharacter prefabOverrideForActor = ActorView.GetPrefabOverrideForActor(actor);
			gameObject = UnityEngine.Object.Instantiate(GameManager.Instance.CharacterTemplate);
			gameObject.GetComponent<ModularCharacterCombiner>().GenerateCharacter(prefabForActor, prefabOverrideForActor, CharacterBuildType.Combat);
			if (prefabForActor.Mirrored)
			{
				gameObject.GetComponent<ActorView>().SetMirrored(mirrored: true);
			}
		}
		return gameObject;
	}

	private GameObject CreateActorsResourceCharacter(ActorModel actor, bool useVisualVariation = true)
	{
		string text = "";
		if (useVisualVariation)
		{
			if (actor is WalkerModel)
			{
				WalkerModel walkerModel = actor as WalkerModel;
				if (walkerModel.VisualVariation != WalkerVisualization.Normal)
				{
					text = walkerModel.VisualVariation.ToString();
				}
			}
			if (actor is RaiderModel)
			{
				RaiderModel raiderModel = actor as RaiderModel;
				if (raiderModel.VisualVariation != RaiderVisualization.Normal)
				{
					text = raiderModel.VisualVariation.ToString();
				}
			}
		}
		ActorResourceEntry resources = GameManager.Instance.GetResources<ActorResourceEntry>(actor.Definition.ID + text);
		if (resources == null)
		{
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("Could not find resources for actor prefab list " + actor.Definition.ID + text + "!");
				return null;
			}
			Debug.LogWarning("Could not find resources for actor prefab list " + actor.Definition.ID + text + "! Using default resource " + actor.Definition.ID);
			return CreateActorsResourceCharacter(actor, useVisualVariation: false);
		}
		if (resources.PrefabResourceList == null)
		{
			Debug.LogError("Could not load prefab list for actor" + actor.Definition.ID + text + "!");
			return null;
		}
		GameObject gameObject = null;
		if (resources.PrefabResourceList.Contains(actor.CharacterPrefab))
		{
			//gameObject = UnityUtils.LoadAsset(actor.CharacterPrefab) as GameObject;
			gameObject = resources.GetPrefab(resources.PrefabResourceList.IndexOf(actor.CharacterPrefab));
		}
		if (gameObject == null)
		{
			gameObject = resources.GetRandomPrefab();
		}
		return UnityEngine.Object.Instantiate(gameObject);
	}

	public bool RemoveActorView(ActorView actorView)
	{
		if (actorViews.Contains(actorView))
		{
			actorViews.Remove(actorView);
			return GameManager.Instance.UnregisterViewWithModel(actorView.Model);
		}
		if (deadBodies.Contains(actorView))
		{
			deadBodies.Remove(actorView);
		}
		return false;
	}

	public void RemoveActorViewWithDelay(ActorView actorView, float delay)
	{
		StartCoroutine(RemoveActorView(actorView, delay));
	}

	private IEnumerator RemoveActorView(ActorView actorView, float delay)
	{
		yield return new WaitForSeconds(delay);
		RemoveActorView(actorView);
	}

	private void CreateObjectiveView(MissionObjective objectiveModel, int index)
	{
		MissionObjectiveView[] componentsInChildren = combatHUD.ObjectivesContainer.GetComponentsInChildren<MissionObjectiveView>(includeInactive: true);
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			if (missionObjectiveView != null)
			{
				UnityEngine.Object.Destroy(missionObjectiveView.gameObject);
				missionObjectiveView = null;
			}
		}
		GameObject gameObject = new GameObject("ObjectiveView");
		missionObjectiveView = gameObject.AddComponent<MissionObjectiveView>();
		missionObjectiveView.Initialize(objectiveModel);
		gameObject.transform.parent = combatHUD.ObjectivesContainer.transform;
		gameObject.transform.localPosition = new Vector3(-10f, (float)index * -40f, 0f);
		gameObject.transform.localScale = Vector3.one;
	}

	private void CreateTurnPanel()
	{
		TurnPanel = CombatHUD.CreateTurnPanel();
		TurnPanel.CreateTurnWarningNotification();
		TurnPanel.SetTurnCount(base.Model.TurnsToWave);
		TurnPanel.SetRedactCount(base.Model.RedactTimedEffect?.Layers ?? 0);
		TurnPanel.SetMaxTurnCount(base.Model.AfterAlarmTurns);
		TurnPanel.SetMaxTurnCountEnabled(base.Model.AfterAlarmTurns > 0 && base.Model.TurnTimerActivationTurn > 0);
		TurnPanel.ChangeTurnsLeft(base.Model.TurnsToFlee);
		TurnPanel.SetSurvivorTurn(base.Model.TurnsToWave, base.Model.ThreatMeter.ThreatLevel);
		TurnPanel.SetMaxClosetSize(base.Model.ThreatMeter.MaxThreatLevel);
		TurnPanel.SetEndTurnButtonHighlight(enabled: false);
		TurnPanel.UpdateThreatOverCount(base.Model.ThreatMeter.ThreatLevel);
		if (!base.Model.IsEndlessBattleMission)
		{
			TurnPanel.SetMonsterCloset(base.Model.ThreatMeter.ThreatLevel);
		}
		if (base.Model.IsEndlessBattleMission)
		{
			int waveCount = EndlessModeHelpers.OverAllWaveCount + 1;
			int currentSpawnCount = EndlessModeHelpers.CurrentSpawnCount;
			TurnPanel.SetWaveCount(waveCount);
			TurnPanel.ShowNextWaveComposition(currentSpawnCount);
			TurnPanel.SetEndlessModeMonsterCloset(base.Model.EndlessModeCombatModel.GetNextWaveSpawnCount);
		}
		UIEventListener uIEventListener = UIEventListener.Get(TurnPanel.SurvivorButtonLabel.transform.parent.gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(CombatHUD.OnSkipTurn));
		UIEventListener uIEventListener2 = UIEventListener.Get(TurnPanel.RedactButton);
		DebugTWD.Log("TurnPanel.RedactButton - проверить функционал", DebugType.System);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, (UIEventListener.VoidDelegate)delegate
		{
			if (base.Model.IsRedacting)
			{
				TraitDefinition trait = base.Model.RedactTimedEffect.GetTrait(base.Model);
				if (trait?.ConstructionParameters != null)
				{
					TooltipManager.OpenTextBoxWithText(TurnPanel.RedactButton, LocalizationManager.GetText("LeaderBuffRedact_InBattle_Text", trait.ConstructionParameters[1], trait.ConstructionParameters[2], trait.ConstructionParameters[3], trait.ConstructionParameters[4], trait.ConstructionParameters[5]));
				}
			}
		});
	}

	private void OnActiveActorChanged(ActorModel actor)
	{
		ActorView actorViewFromModel = GetActorViewFromModel(actor);
		foreach (ActorView actorView in actorViews)
		{
			if (actorView.Model.Faction == Faction.Survivor)
			{
				actorView.SetActiveActor(actorView == actorViewFromModel);
			}
		}
		StartCoroutine(DelayedActiveActorChanged(actor));
	}

	private IEnumerator DelayedActiveActorChanged(ActorModel actor)
	{
		yield return null;
		if (actor != null)
		{
			ShowHUDForSelectedActor(actor);
			if (!actor.IsAIControlled && !CombatHUD.IsSpeedUpEnabled)
			{
				FocusCameraOnActiveActor(forceFocus: false);
			}
		}
	}

	public void RequestEndCombat(ECombatResult result)
	{
		combatEndRequested = true;
		combatEndResult = result;
		for (int i = 0; i < actorViews.Count; i++)
		{
			if (actorViews[i].Model.Faction == Faction.Survivor && HoorayClips.Length != 0 && base.Model.IsActorInValidExit(actorViews[i].Model))
			{
				SurvivorAnimationController obj = actorViews[i].CharacterAnimationController as SurvivorAnimationController;
				AnimationClip customAnimationClip = HoorayClips[UnityEngine.Random.Range(0, HoorayClips.Length - 1)];
				obj.StartCustomAnimation(customAnimationClip);
			}
		}
	}

	private void PlayFadeOut()
	{
		fadeOut.PlayFadeOut(combatEndResult, ranOutOfTime || base.Model.OutOfTurns, EndCombat);
	}

	private void EndCombat()
	{
		PlayerInputManager.Instance.Stop();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAll();
		if (combatEndResult == ECombatResult.Successful)
		{
			Helpers.ExecuteCommand(new ResetAttackedTileCommand(base.Model.manager.Player.MapContainerModel));
		}
		combatStartUpTime = 0L;

		if (!OfflineManager.IsLoadDataManager)
		{
			bool inCombatTutorial = TutorialView.Instance.InCombatTutorial;
			TutorialView.Instance.InCombatTutorial = false;
			DisableCombatAudioListener();
			if (!inCombatTutorial || TutorialView.Instance.ShowCombatEndScreen)
			{
				OpenCombatEndFlowPopup();
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.EndCombatMusicSync();
					SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("combat_ui/timer_warning");
				}
			}
			else
			{
				DestroyCombat();
				GameManager.Instance.ReturnFromVisit();
			}
		}
		else
		{
			OpenCombatEndFlowPopup();
		}
	}

	private void ConfirmExit()
	{
		DestroyCombat();
		List<ActorModel> list = new List<ActorModel>();
		if (combatEndResult == ECombatResult.Successful)
		{
			for (int i = 0; i < base.Model.ExtraSurvivors.Count; i++)
			{
				SurvivorModel survivorModel = base.Model.ExtraSurvivors[i] as SurvivorModel;
				if (!survivorModel.IsDead && !survivorModel.IsNotGivenToPlayer)
				{
					list.Add(survivorModel);
				}
			}
		}
		endScreenHandler.BeginEndScreen(combatEndResult, list);
	}

	private void OpenCombatEndFlowPopup()
	{
		if (missionCompletedOnReload && TutorialView.Instance.Model.HasCompletedPart("VictoryScreen"))
		{
			missionCompletedOnReload = false;
			ConfirmExit();
			return;
		}
		PopupCombatEnd popupCombatEnd = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PopupCombatEnd) as PopupCombatEnd;
		if (popupCombatEnd != null)
		{
			if (base.Model.CombatRetryChoicePendingState == MissionRetryState.Pending)
			{
				popupCombatEnd.RetryMissionCallback = RetryMission;
				popupCombatEnd.ConfirmedCallback = ProceedEndMission;
			}
			else
			{
				popupCombatEnd.ConfirmedCallback = ConfirmExit;
			}
			popupCombatEnd.OpenForModel(base.Model);
		}
		else
		{
			Debug.LogError("PopupCombatEnd: Can't open popup. Going straight to EndCombat");
			ConfirmExit();
		}
	}

	private void DestroyCombat()
	{
		if (combatHUD != null)
		{
			combatHUD.SetSpeedUpState(enabled: false);
		}
		VisualizationQueue.Instance.StopAllTasks();
		PlayerInputManager.Instance.ResetAllHandlers();
		FogOfWarVisualization fogOfWarVisualization = UnityEngine.Object.FindObjectOfType<FogOfWarVisualization>();
		if (fogOfWarVisualization != null)
		{
			UnityEngine.Object.Destroy(fogOfWarVisualization.gameObject);
		}
		UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<Scenario>().gameObject);
		if (ThreatMeter != null)
		{
			UnityEngine.Object.Destroy(ThreatMeter.gameObject);
			ThreatMeter = null;
		}
		if (ChargeMeter != null)
		{
			UnityEngine.Object.Destroy(ChargeMeter.gameObject);
			ChargeMeter = null;
		}
		if (TurnPanel != null)
		{
			UnityEngine.Object.Destroy(TurnPanel.gameObject);
			TurnPanel = null;
		}
		if (fadeOut != null)
		{
			UnityEngine.Object.Destroy(fadeOut.gameObject);
			fadeOut = null;
		}
		UnityEngine.Object.Destroy(GameObject.Find("Background"));
		Helpers.ClearUnusedMemory();
		traitActiveVisualizationsManager.Dispose();
	}

	private void ShowSuggestedInteractionTarget()
	{
		if (base.Model.SuggestedInteractionTargetCoordinate != GridCoordinate.Invalid)
		{
			if (showHandDelayedTask != null)
			{
				VisualizationQueue.Instance.RemoveFromQueue(showHandDelayedTask);
				showHandDelayedTask = null;
			}
			showHandDelayedTask = new DelayedNotificationVisualizationTask(null, delegate
			{
				if (base.Model.TurnManager.ActiveActor != null && !base.Model.TurnManager.ActiveActor.TurnComplete)
				{
					Vector3 dragStart = GridView.Instance.GetPosition(base.Model.TurnManager.ActiveActor.GridCoordinate).ToVector3();
					Vector3 dragEnd = GridView.Instance.GetPosition(base.Model.SuggestedInteractionTargetCoordinate).ToVector3();
					TutorialView.Instance.ShowHandDrag(dragStart, dragEnd);
					showHandDelayedTask = null;
				}
			});
			VisualizationQueue.Instance.Add(showHandDelayedTask);
		}
		else
		{
			TutorialView.Instance.HideHand();
			if (showHandDelayedTask != null)
			{
				VisualizationQueue.Instance.RemoveFromQueue(showHandDelayedTask);
				showHandDelayedTask = null;
			}
		}
	}

	private IEnumerator DelayedCreateActorView(ActorModel actor)
	{
		while (GameManager.Instance.modelManager.IsExecutingCommand)
		{
			yield return null;
		}
		CreateActorView(actor);
	}

	private void VisualPropsStartNotification()
	{
		List<ConditionalNode> list = new List<ConditionalNode>();
		list.AddRange(ConditionalNode.GetAllStartedInstances());
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && list[i].isActiveAndEnabled)
			{
				list[i].OnCombatStarted();
			}
		}
	}

	protected void OnModelChange(ModelObject m, string changed, object args)
	{
		if (string.IsNullOrEmpty(changed))
		{
			return;
		}
		switch (changed)
		{
		case "turnEnded":
			break;
		case "damageDealt":
			break;
		case "exitEnabled":
			break;
		case "collidersUpdated":
			break;
		case "suggestedInteractionTargetChanged":
			ShowSuggestedInteractionTarget();
			break;
		case "missionCompleted":
			combatEndResult = (ECombatResult)args;
			if (base.Model.CombatRetryChoicePendingState != MissionRetryState.Resolved)
			{
				missionEndedPending = true;
			}
			else
			{
				ConfirmExit();
			}
			SendMissionCompletedAnalytics();
			break;
		case "actorCreated":
		{
			ActorModel actorModel3 = (ActorModel)args;
			if (GameManager.Instance.GetViewForModel(actorModel3) as ActorView == null)
			{
				if (!HasModularCharacter(actorModel3))
				{
					CreateActorView(actorModel3);
				}
				else
				{
					StartCoroutine(DelayedCreateActorView(actorModel3));
				}
			}
			CheckForExplosiveWalkerSmartTutorial(actorModel3);
			CheckForBossWalkerSmartTutorial(actorModel3);
			break;
		}
		case "MagazineAreasUpdate":
		{
			List<MagazineArea> magazineAreas = GetMagazineAreas();
			RemoveClapPrefabInstance(magazineAreas);
			break;
		}
		case "missionLoadedEvent":
		{
			VisualPropsStartNotification();
			EnableCombatAudioListener();
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.LoadAudio("CombatSfx");
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/combat_start");
			}
			Helpers.ClearUnusedMemory();
			CheckForExplosiveWalkerSmartTutorial();
			ActorModel activeActor = base.Model.TurnManager.ActiveActor;
			if (activeActor != null)
			{
				ActorView actorView = GameManager.Instance.GetViewForModel(activeActor) as ActorView;
				if (actorView != null)
				{
					actorView.SetActiveActor(active: true);
				}
			}
			if (CombatWasResumed)
			{
				if (!OfflineManager.IsLoadDataManager)
				{
					GameManager.Instance.GuildInviteFlow?.StartJoinGuildInCombat();
				}
			}
			else
			{
				VisualizationQueue.Instance.Add(new StartCombatVisualizationTask());
			}
			break;
		}
		case "turnSkipped":
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/turn_skip");
			}
			break;
		case "redactEndEvent":
			TurnPanel.SetRedactCount(base.Model.RedactTimedEffect?.Layers ?? 0);
			break;
		case "survivorTurnEnd":
		{
			DelayedNotificationVisualizationTask task = new DelayedNotificationVisualizationTask(null, delegate
			{
				TurnPanel.ChangeTurnCount(base.Model.TurnsToWave);
				TurnPanel.SetEndTurnButtonHighlight(enabled: false);
			});
			VisualizationQueue.Instance.Add(task);
			if (GameManager.Instance.gameEconomyData.GetFeature("CombatOfflineModeFix").Enabled && !GameManager.Instance.CheckConnectionReachability(showPopup: true, "PlayerEndedTurn"))
			{
				DebugTWD.LogError("GameDisconnected - проверить случаи появления", DebugType.Error);
				VisualizationQueue.Instance.GameDisconnected();
			}
			break;
		}
		case "threatMeterValueChanged":
			if (!base.Model.IsEndlessBattleMission)
			{
				int num = (int)args;
				if (num > 0)
				{
					TurnPanel.AddToCloset(num, base.Model.ThreatMeter.ThreatLevel);
				}
				else if (num < 0)
				{
					TurnPanel.RemoveFromCloset(base.Model.ThreatMeter.ThreatLevel);
				}
				SingularityMonoBehaviour<AudioManager>.Instance.SetMusicThreat(base.Model.ThreatMeter.ThreatLevel);
			}
			break;
		case "EndlessModeWaveSpawned":
		{
			int spawnCount = (int)args;
			OnEndlessWaveSpawned(spawnCount);
			break;
		}
		case "EndlessModeScoreChanged":
		case "EndlessModeMultiplierReduced":
			CombatHUD.RefreshEndlessModeScores(playAnimation: true);
			break;
		case "PvPMissonObjectiveCompleted":
		{
			DelayedNotificationVisualizationTask task2 = new DelayedNotificationVisualizationTask(null, delegate
			{
				ECombatResult currentResult = (ECombatResult)args;
				combatHUD.ChangeMissionButtonState(enabled: true, currentResult);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/exit_ready");
			});
			VisualizationQueue.Instance.Add(task2);
			break;
		}
		case "TurnTimerActivated":
			if (base.Model.HasPvPRules)
			{
				ClearOverwatchIndicators(Faction.Raider);
			}
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				TurnPanel.SetMaxTurnCount(base.Model.AfterAlarmTurns);
				TurnPanel.SetMaxTurnCountEnabled(enabled: true);
				TurnPanel.PlayTurnWarning(LocalizationManager.GetText("Popup.TurnWarning.Title.Start"), LocalizationManager.GetText("Popup.TurnWarning.Body{parameter}", base.Model.TurnsToFlee));
			}));
			break;
		case "actorBecameVisible":
		{
			ActorModel actor = args as ActorModel;
			CheckForExplosiveWalkerSmartTutorial(actor);
			CheckForBossWalkerSmartTutorial(actor);
			break;
		}
		case "MuteStateChanged":
			if (args.ToString().ToLower() == "music")
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
				{
					SingularityMonoBehaviour<AudioManager>.Instance.SetForcedMusicMuteState(base.Model.MusicMuteForced);
				}));
			}
			break;
		case "actorTransformed":
		{
			ActorModel actorModel2 = args as ActorModel;
			ActorView actorViewFromModel = GetActorViewFromModel(actorModel2);
			actorViewFromModel.CanUpdateVisibility = true;
			actorViewFromModel.SetVisible(visible: false);
			actorViewFromModel.CanUpdateVisibility = false;
			break;
		}
		case "modelAdded":
			SpawnModelView((TWDModelObject)args);
			break;
		case "modelRemoved":
			KillModelView((TWDModelObject)args);
			break;
		case "turnManagerActorChangedEvent":
			if (base.Model.IsEndlessBattleMission)
			{
				float survivorsHpPercentage = CalculateSurvivorsHpPercentage();
				SingularityMonoBehaviour<AudioManager>.Instance.UpdateEndlessModeAudioTrack(survivorsHpPercentage);
			}
			break;
		case "BattlePassCurrencyEarned":
		{
			object[] obj = (object[])args;
			int amount = (int)obj[0];
			ActorModel actorModel = obj[1] as ActorModel;
			OnBattlePassCurrencyEarned(amount, actorModel);
			break;
		}
		case "FlushthreatTurn":
			TurnPanel.ChangeTurnCount(base.Model.TurnsToWave);
			break;
		case "DebuffDamagePerRound":
			CombatHUD.ShowDebuffDamagePerRoundTips();
			break;
		}
	}

	private float CalculateSurvivorsHpPercentage()
	{
		int num = 0;
		int num2 = 0;
		foreach (ActorModel survivor in base.Model.Survivors)
		{
			num += survivor.MaxHitPoints * 2;
			num2 += survivor.Hitpoints;
			if (!survivor.OnRedHealthBar)
			{
				num2 += survivor.MaxHitPoints;
			}
		}
		return (float)num2 / (float)num;
	}

	private void RetryMission()
	{
		DebugTWD.Log("RetryMission", DebugType.Wars);

		FixedPoint fixedPoint = GameManager.Instance.gameEconomyData.GuildWarConfig.RetryMissionPenalty + 0.0001;
		bool isPvPCombat = GuildWarHelper.GetGuildWarPlayer().GuildBattleModel.AttackTargetMission.IsPvPCombat;
		GuildBattleMapMissionModel guildBattleMapMissionModel = GameManager.Instance.playerModel.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
		int guildBattleMissionVictoryPoints = GuildWarHelper.GetCurrentBattle().GetGuildBattleMissionVictoryPoints(guildBattleMapMissionModel.SectorIdOwner, isPvPCombat, guildBattleMapMissionModel.AreaIndex);
		int num = (int)FixedPoint.Round(guildBattleMissionVictoryPoints * fixedPoint);
		string text = LocalizationManager.GetText("Popup.Defeat.RetryWarning{Reduction}{NewAmount}{OldAmount}", (int)(fixedPoint * 100L), guildBattleMissionVictoryPoints - num, guildBattleMissionVictoryPoints);
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Defeat.RetryWarning.Title"), text, LocalizationManager.GetText("Button.Retry"), delegate
		{
			if (OfflineManager.IsLoadDataManager && OfflineManager.IsFakeExecuteCommands)
			{
				DebugTWD.LogWarning("RetryWarning - проверить выполнение ExecuteCommand", DebugType.Wars);
				OnGoToCombatCallback();
			}
			else
			{
				if (Helpers.ExecuteCommand(new RetryGuildBattleMissionCommand()) == TWDModelResult.OK)
				{
					OnGoToCombatCallback();
				}
			}
		}, LocalizationManager.GetText("Button.Cancel"), delegate
		{
			PopupCombatEnd popupCombatEnd = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.PopupCombatEnd) as PopupCombatEnd;
			if (popupCombatEnd != null)
			{
				popupCombatEnd.RetryMissionCallback = RetryMission;
				popupCombatEnd.ConfirmedCallback = ProceedEndMission;
			}
		});
	}

	private void ProceedEndMission()
	{
		Helpers.ExecuteCommand(new ProceedEndMissionCommand());
	}

	private void OnGoToCombatCallback()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("map/start_mission");
		EventManager.NotifyClick("StartMission");
		EventManager.NotifyEvent(EventManager.EventType.StartMission);
		MapMissionParameters missionInfo = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel.ToMissionParameters();
		GameManager.Instance.LoadVisitModel(VisitMode.PVE, missionInfo, base.gameObject.scene.name);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
	}

	private void FocusCameraOnActiveActor(bool forceFocus)
	{
		if (base.Model.ActiveActor != null && base.Model.ActiveActor.Faction == Faction.Survivor && (forceFocus || PlayerInputManager.Instance.GetHandler<CameraInputHandler>().IsTargetFarFromCameraCenter(GridView.Instance.GetPosition(base.Model.ActiveActor.GridCoordinate).ToVector3())))
		{
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FrameActorToView(base.Model.ActiveActor, immediateMove: false);
		}
	}

	private bool IsGridCellVisibleByAnySurvivor(GridCoordinate targetLocation)
	{
		GridModel grid = base.Model.Grid;
		for (int i = 0; i < actorViews.Count; i++)
		{
			ActorView actorView = actorViews[i];
			if (actorView.Model.Faction == Faction.Survivor || actorView.Model.IsFlare)
			{
				GridCoordinate coordinate = grid.GetCoordinate(new FixedVec3(actorView.transform.position.x, actorView.transform.position.y, actorView.transform.position.z));
				if (base.Model.IsGridCellVisible(coordinate, targetLocation))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void RefreshVisibilityStatus()
	{
		CombatModel model = base.Model;
		if (base.Model.GridColliderData != null)
		{
			for (int i = 0; i < actorViews.Count; i++)
			{
				ActorView actorView = actorViews[i];
				GridCoordinate coordinate = model.Grid.GetCoordinate(new FixedVec3(actorView.transform.position.x, actorView.transform.position.y, actorView.transform.position.z));
				bool visible = actorView.Model.IsFriendlyHuman || actorView.Model.IsFlare || IsGridCellVisibleByAnySurvivor(coordinate);
				actorView.SetVisible(visible);
			}
			List<InteractiveObjectView> views = GameManager.Instance.GetViews<InteractiveObjectView>();
			for (int j = 0; j < views.Count; j++)
			{
				InteractiveObjectView interactiveObjectView = views[j];
				GridCoordinate coordinate2 = model.Grid.GetCoordinate(new FixedVec3(interactiveObjectView.transform.position.x, interactiveObjectView.transform.position.y, interactiveObjectView.transform.position.z));
				bool visible2 = model.IsGridCellVisibleByAnySurvivor(coordinate2) || interactiveObjectView.Model.Location.Edge >= 0 || interactiveObjectView.Model.VisibleInFog;
				interactiveObjectView.SetVisible(visible2);
			}
		}
	}

	public void AddDeadbody(ActorView view)
	{
		deadBodies.Add(view);
		if (deadBodies.Count > MaxDeadBodies)
		{
			ActorView actorView = deadBodies[0];
			deadBodies.RemoveAt(0);
			actorView.FadeAndDestroy();
		}
	}

	public void Update()
	{
		if (!base.IsInitialized)
		{
			return;
		}
		if (base.Model.TurnManager.CanSwitchActiveActor && base.Model.TurnManager.AllActorsTurnCompleted && !base.Model.MissionCompleted && base.Model.CombatRetryChoicePendingState == MissionRetryState.None && VisualizationQueue.Instance.IsQueueEmpty)
		{
			Helpers.ExecuteCommand(new EndSurvivorTurnCommand());
		}
		if (missionEndedPending)
		{
			if (!OfflineManager.IsLoadDataManager)
			{
				MusicState resultMusic = MusicState.Defeat;
				if (combatEndResult == ECombatResult.Successful)
				{
					resultMusic = MusicState.Victory;
				}
				if (!TutorialView.Instance.InCombatTutorial)
				{
					VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
					{
						SingularityMonoBehaviour<AudioManager>.Instance.RequestMusicStateChange(resultMusic);
					}));
				}
				LevelLoopingSoundView[] array = UnityEngine.Object.FindObjectsOfType<LevelLoopingSoundView>();
				for (int num = 0; num < array.Length; num++)
				{
					array[num].StopSound();
				}
			}
			if (actorViews.Count > 0 && base.Model.IsActorInValidExit(actorViews[0].Model) && combatEndResult == ECombatResult.Successful)
			{
				VisualizationQueue.Instance.Add(new ActionCameraVisualizationTask(actorViews[0].Model, actorViews[0].Model));
			}
			VisualizationQueue.Instance.Add(new EndCombatVisualizationTask(combatEndResult));
			missionEndedPending = false;
		}
		if (combatEndRequested)
		{
			EndCombat();
			combatEndRequested = false;
			TooltipManager.HideAll();
		}
		else if (IsPlayerInputEnabledPending && VisualizationQueue.Instance.TotalTaskCount == 0)
		{
			IsPlayerInputEnabledPending = false;
			IsPlayerInputEnabled = true;
		}
		if (base.Model.HasPvPRules)
		{
			long combatTimeSinceStartup = GetCombatTimeSinceStartup();
			if (combatTimeSinceStartup > 0)
			{
				long combatTimeLeft = base.Model.MaxTime - combatTimeSinceStartup;
				if (missionObjectiveView != null)
				{
					missionObjectiveView.SetCombatTimeLeft(combatTimeLeft);
				}
				if (combatTimeSinceStartup >= base.Model.MaxTime && combatStartUpTime != 0L)
				{
					combatStartUpTime = 0L;
					if (base.Model.IsPVPMission)
					{
						EndCombatTimer(!base.Model.IsPvPFlagCollected && !base.Model.IsPvPLootCollected && !base.Model.IsPvpDefendersKilled);
					}
					else
					{
						AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.Tutorial.OutOfTime.Title"), LocalizationManager.GetText("Popup.Outpost.Tutorial.OutOfTime.Body"), LocalizationManager.GetText("Button.Ok"), delegate
						{
						});
					}
				}
			}
		}
		visibilityRefreshTimer -= Time.deltaTime;
		if (visibilityRefreshTimer <= 0f)
		{
			RefreshVisibilityStatus();
			visibilityRefreshTimer = visibilityRefreshInterval;
		}
	}

	private void OnFactionChanging(Faction currentFaction, Faction newFaction)
	{
		if (currentFaction == Faction.Survivor)
		{
			combatHUD.HideSurvivorTurnHUD();
		}
		if (newFaction == Faction.Survivor)
		{
			IsPlayerInputEnabledPending = true;
		}
		else
		{
			IsPlayerInputEnabled = false;
		}
		VisualizationQueue.Instance.Add(new FactionChangeVisualizationTask());
		VisualizationQueue.Instance.AddTaskBlocker();
	}

	public void OnFactionChanged(Faction previousFaction, Faction newFaction)
	{
		if (newFaction == Faction.Lure)
		{
			return;
		}
		string factionName = base.Model.GetFactionName(newFaction);
		string textId = "Turn." + factionName + ".Title";
		string turnTitle = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId);
		switch (newFaction)
		{
		case Faction.Survivor:
		{
			DelayedNotificationVisualizationTask task2 = new DelayedNotificationVisualizationTask(null, delegate
			{
				CurrentViewFaction = Faction.Survivor;
				combatHUD.HideEnemyTurnHUD();
				combatHUD.SetSurvivorTurnHUD(base.Model.ActiveActor);
				combatHUD.EnablePlayerInfoContainer();
				combatHUD.ShowSurvivorStatusTooltips();
				for (int i = 0; i < actorViews.Count; i++)
				{
					ActorView actorView = actorViews[i];
					actorView?.ClearIndicatorKnockKnockMark();
					actorView?.UpdateIndicatorPhonePortrait(isActive: false);
					actorView?.UpdateIndicatorUpdateABtestB(isActive: false);
				}
				if (base.Model.TurnsToWave == 0 && !base.Model.IsEndlessBattleMission)
				{
					TurnPanel.ChangeTurnCount(base.Model.ThreatMeter.InitialTurnCountToWave + 1);
					TurnPanel.SetMonsterCloset(base.Model.ThreatMeter.ThreatLevel);
					TurnPanel.UpdateThreatOverCount(base.Model.ThreatMeter.ThreatLevel);
				}
				TurnPanel.ChangeTurnsLeft(base.Model.TurnsToFlee);
				if (base.Model.HasPvPRules && !base.Model.IsPVPMission && base.Model.OutOfTurns && !outOfTurnsPopupShown)
				{
					outOfTurnsPopupShown = true;
					AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.Tutorial.OutOfTurns.Title"), LocalizationManager.GetText("Popup.Outpost.Tutorial.OutOfTurns.Body"), LocalizationManager.GetText("Button.Ok"), delegate
					{
					});
				}
				UIEventListener uIEventListener = UIEventListener.Get(TurnPanel.SurvivorButtonLabel.transform.parent.gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(CombatHUD.OnSkipTurn));
				TurnPanel.SetSurvivorTurn(base.Model.TurnsToWave, base.Model.ThreatMeter.ThreatLevel);
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/turn_survivors");
				}
			});
			VisualizationQueue.Instance.Add(task2);
			break;
		}
		case Faction.Walker:
		{
			DelayedNotificationVisualizationTask task3 = new DelayedNotificationVisualizationTask(null, delegate
			{
				CurrentViewFaction = Faction.Walker;
				combatHUD.HideSurvivorTurnHUD();
				combatHUD.SetEnemyTurnHUD(Faction.Walker);
				UIEventListener uIEventListener = UIEventListener.Get(TurnPanel.SurvivorButtonLabel.transform.parent.gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(CombatHUD.OnSkipTurn));
				TurnPanel.WalkerButtonLabel.text = turnTitle;
				if (!base.Model.IsEndlessBattleMission)
				{
					TurnPanel.SetWalkerTurn(base.Model.TurnsToWave, base.Model.ThreatMeter.ThreatLevel);
				}
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/turn_walkers");
				}
			});
			VisualizationQueue.Instance.Add(task3);
			break;
		}
		case Faction.Raider:
		{
			if (base.Model.GetFactionActors(newFaction).Count <= 0)
			{
				break;
			}
			DelayedNotificationVisualizationTask task = new DelayedNotificationVisualizationTask(null, delegate
			{
				CurrentViewFaction = Faction.Raider;
				combatHUD.HideSurvivorTurnHUD();
				combatHUD.SetEnemyTurnHUD(Faction.Raider);
				UIEventListener uIEventListener = UIEventListener.Get(TurnPanel.SurvivorButtonLabel.transform.parent.gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(CombatHUD.OnSkipTurn));
				TurnPanel.WalkerButtonLabel.text = turnTitle;
				TurnPanel.SetRaiderTurn(base.Model.TurnsToWave, base.Model.ThreatMeter.ThreatLevel);
				ClearOverwatchIndicators(Faction.Raider);
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/turn_walkers");
				}
			});
			VisualizationQueue.Instance.Add(task);
			break;
		}
		}
		if (previousFaction == Faction.Raider)
		{
			for (int num = 0; num < base.Model.Raiders.Count; num++)
			{
				ActorModel actorModel = base.Model.Raiders[num];
				ActorView raiderView = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
				if (raiderView != null && !actorModel.IsDead && actorModel.TurnComplete && actorModel.HadActionPointsAtEndOfTurn)
				{
					VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(actorModel, delegate
					{
						raiderView.SetAIOverwatchIndicator(enabled: true);
					}));
				}
			}
		}
		tutorialStarting = false;
		PlayerInputManager.Instance.GetHandler<ObjectInfoInputHandler>()?.ClearInfoPopups();
	}

	private void ClearOverwatchIndicators(Faction faction)
	{
		List<ActorModel> factionActors = base.Model.GetFactionActors(faction);
		for (int i = 0; i < factionActors.Count; i++)
		{
			ActorModel model = factionActors[i];
			ActorView actorView = GameManager.Instance.GetViewForModel(model) as ActorView;
			if (actorView != null)
			{
				actorView.SetAIOverwatchIndicator(enabled: false);
			}
		}
	}

	public void CheckForExplosiveWalkerSmartTutorial(ActorModel actor = null)
	{
		if (OfflineManager.IsLoadDataManager && OfflineManager.IsTutorialDisable) return;
		if (tutorialStarting || GameManager.Instance.SmartTutorialData.HasShown(SmartTutorialType.ExplosiveWalker))
		{
			return;
		}
		if (actor is WalkerModel { IsVisibleToSurvivors: not false } walkerModel && walkerModel.Definition.Class.ToLower().StartsWith("walkerexplosive"))
		{
			DelayedNotificationVisualizationTask task = new DelayedNotificationVisualizationTask(null, delegate
			{
				GameManager.Instance.SmartTutorialData.StartSmartTutorial(SmartTutorialType.ExplosiveWalker);
			});
			VisualizationQueue.Instance.Add(task);
			tutorialStarting = true;
			return;
		}
		List<ActorModel> factionActors = base.Model.GetFactionActors(Faction.Walker);
		for (int num = 0; num < factionActors.Count; num++)
		{
			WalkerModel walkerModel2 = factionActors[num] as WalkerModel;
			if (walkerModel2.IsVisibleToSurvivors && walkerModel2.Definition.Class.ToLower().StartsWith("walkerexplosive"))
			{
				DelayedNotificationVisualizationTask task2 = new DelayedNotificationVisualizationTask(null, delegate
				{
					GameManager.Instance.SmartTutorialData.StartSmartTutorial(SmartTutorialType.ExplosiveWalker);
				});
				VisualizationQueue.Instance.Add(task2);
				tutorialStarting = true;
				break;
			}
		}
	}

	private void CheckForBossWalkerSmartTutorial(ActorModel actor)
	{
		if (OfflineManager.IsLoadDataManager && OfflineManager.IsTutorialDisable) return;
		if (!tutorialStarting && !GameManager.Instance.SmartTutorialData.HasShown(SmartTutorialType.BossWalker) && actor != null && actor.IsVisibleToSurvivors && actor.IsBossWalker)
		{
			DelayedNotificationVisualizationTask task = new DelayedNotificationVisualizationTask(null, delegate
			{
				GameManager.Instance.SmartTutorialData.StartSmartTutorial(SmartTutorialType.BossWalker);
			});
			VisualizationQueue.Instance.Add(task);
			tutorialStarting = true;
		}
	}

	private void OnAbilityPerformed(ActorModel actor)
	{
		combatHUD.ResetEquipmentSelectionIndicator(actor);
	}

	private void EnableCombatAudioListener()
	{
		AudioListener componentInChildren = GetComponentInChildren<AudioListener>();
		if (componentInChildren != null)
		{
			AudioListener uIAudioListener = SingularityMonoBehaviour<HUDManager>.Instance.GetUIAudioListener();
			if (uIAudioListener != null)
			{
				uIAudioListener.enabled = false;
			}
			componentInChildren.enabled = true;
		}
	}

	private void DisableCombatAudioListener()
	{
		AudioListener componentInChildren = GetComponentInChildren<AudioListener>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = false;
		}
		AudioListener uIAudioListener = SingularityMonoBehaviour<HUDManager>.Instance.GetUIAudioListener();
		if (uIAudioListener != null)
		{
			uIAudioListener.enabled = true;
		}
	}

	private void StartCombatTimer()
	{
		combatStartUpTime = (long)Time.realtimeSinceStartup;
	}

	private void EndCombatTimer(bool failed)
	{
		ranOutOfTime = true;
		Helpers.ExecuteCommand(new EndCombatCommand());
	}

	private void OnWalkerTurnNotificationCompleted()
	{
	}

	[Conditional("UNITY_ANDROID")]
	public static void Toggle(bool visibility)
	{
		if (Instance != null)
		{
			Instance.transform.Find("CombatCamera").gameObject.GetComponent<Camera>().enabled = visibility;
		}
	}

	private void SpawnModelView(TWDModelObject model)
	{
		CombatModelViewResourceEntry resources = modelViewResources.GetResources(model.GetType().FullName);
		if (resources != null && GameManager.Instance.GetViewForModel(model) == null)
		{
			UnityEngine.Object.Instantiate(UnityUtils.LoadFromAssetBundle<PrefabResource>(resources.ResourceAddress, "scriptableobjects").GetPrefab()).GetComponent<CombatModelView>().Initialize(model);
		}
	}

	private void KillModelView(TWDModelObject model)
	{
		(GameManager.Instance.GetViewForModel(model) as CombatModelView)?.Kill();
	}

	private void InitWaveIndicators()
	{
		WaveIndicators = UnityEngine.Object.FindObjectsOfType<WaveIndicatorGroup>().ToList();
		foreach (WaveIndicatorGroup waveIndicator in WaveIndicators)
		{
			for (int i = 0; i < base.Model.OrderedSpawnPoints.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = base.Model.OrderedSpawnPoints[i];
				if (waveIndicator.ConnectedSpawnPointView.ViewId == actorSpawnPointModel.ViewId)
				{
					waveIndicator.SpawnPointIndex = i;
					waveIndicator.gameObject.SetActive(base.Model.EndlessModeCombatModel.NextSpawnPointIndices.Contains(i));
					break;
				}
			}
		}
	}

	private void EnableCurrentWaveIndicators()
	{
		foreach (WaveIndicatorGroup waveIndicator in WaveIndicators)
		{
			waveIndicator.gameObject.SetActive(value: false);
		}
		foreach (int index in base.Model.EndlessModeCombatModel.NextSpawnPointIndices)
		{
			int index2 = WaveIndicators.FindIndex((WaveIndicatorGroup x) => x.SpawnPointIndex == index);
			WaveIndicators?[index2].gameObject.SetActive(value: true);
		}
	}

	private void SendMissionCompletedAnalytics()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.Log("SendMissionCompletedAnalytics disabled", DebugType.Wars);
			return;
		}
		MapMissionModel attackTargetMissionModel = GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel;
		if (combatEndResult == ECombatResult.Successful && attackTargetMissionModel != null && attackTargetMissionModel.MissionSpawnPointGroup.Category == MapCategory.Story)
		{
			int.TryParse(attackTargetMissionModel.MissionSpawnPointGroup.DisplayName.Split(' ')[^1], out var result);
			int missionNumber = attackTargetMissionModel.MissionSpawnPointGroup.MissionSpawnPoints.IndexOf(attackTargetMissionModel.MissionSpawnPoint) + 1;
			int episodeDifficultyLevel = attackTargetMissionModel.MissionSpawnPointGroup.EpisodeDifficultyLevel;
			SingularityMonoBehaviour<SDKManager>.Instance.StoryMissionCompleted(result, missionNumber, episodeDifficultyLevel);
		}
	}

	private void OnEndlessWaveSpawned(int spawnCount)
	{
		CombatHUD.RefreshEndlessModeWaveCount(playAnimation: true);
		CombatHUD.RefreshEndlessModeScores(playAnimation: true);
		TurnPanel.ShowNextWaveComposition(spawnCount);
		TurnPanel.SetWaveCount(base.Model.EndlessModeCombatModel.GetCurrentOverAllWaveIndex + 1);
		string text = LocalizationManager.GetText("Endless.Combat.Wave{WaveNumber}", EndlessModeHelpers.OverAllWaveCount);
		string getFormattedWaveNotificationBody = EndlessModeHelpers.GetFormattedWaveNotificationBody;
		Color color = (EndlessModeHelpers.IsEndlessExpertMode() ? CombatHUD.ExpertModeColor : CombatHUD.NormalModeColor);
		CombatHUD.DisplayWaveNotification(text, getFormattedWaveNotificationBody, color, color);
		EnableCurrentWaveIndicators();
	}

	private void OnBattlePassCurrencyEarned(int amount, ActorModel actorModel)
	{
		ActorView walkerView = GetActorViewFromModel(actorModel);
		VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(actorModel, delegate
		{
			string currencyIconName = HelpersGfx.GetCurrencyIconName(CurrencyType.BattlePassPoints);
			ActorNotificationMessage message = new ActorNotificationMessage(amount.ToString(), currencyIconName, NotificationSound.None, ActorNotificationType.BattlePassCurrencyNotification);
			walkerView.AddNotification(message);
		}, addDependencyToOtherActors: true));
	}
}
