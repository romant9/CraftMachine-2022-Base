using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;
using UnityEngine.Rendering;

public class LoadingScreenCombat : MonoBehaviour
{
	public enum ActorPositions
	{
		Front = 0,
		Left = 1,
		Right = 2,
		Attack = 3,
		Defend = 4
	}

	private Callback animationInCallback;

	[SerializeField]
	[Tooltip("Positions of survivors that will be added to the screen.")]
	private Transform[] survivorPositions;

	[SerializeField]
	[Tooltip("Position of the survivor info cards.")]
	private Transform[] survivorUIPositions;

	[SerializeField]
	[Tooltip("Prefab for survivor info card.")]
	private GameObject surviviorInfoCardPrefab;

	[SerializeField]
	private GameObject loadingScreenUIPrefab;

	private float loadingContinueTimer;

	public float PvPAutoContinueTime = 5f;

	private bool isDeadly;

	private bool loadingOver;

	private GameObject uiContainer;

	private LoadingScreenCombatUI loadingScreenCombatUi;

	private int currentLoadingStep;

	private const int totalLoadingSteps = 5;

	private static GameObject combatMainGameObject;

	private List<GameObject> actors = new List<GameObject>();

	private static LoadingScreenCombat loadingScreenInstance;

	private bool exiting;

	public static bool Active { get; private set; }

	private void DrawSurvivor(SurvivorModel combatSurvivor, GameObject parent, Vector3 uiPosition, int placementIndex, bool isTransient = false, CharacterBuildType characterBuildType = CharacterBuildType.CombatLoading)
	{
		if (characterBuildType != CharacterBuildType.GuildBattleLoadingDefeated)
		{
			ActorView.PrepareActor(combatSurvivor, isTransient);
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(combatSurvivor);
			ModularCharacter prefabOverrideForActor = ActorView.GetPrefabOverrideForActor(combatSurvivor);
			GameObject gameObject = Helpers.InstantiateToParent(GameManager.Instance.CharacterTemplate, parent);
			gameObject.GetComponent<ModularCharacterCombiner>().GenerateCharacter(prefabForActor, prefabOverrideForActor, characterBuildType);
			ActorView component = gameObject.GetComponent<ActorView>();
			component.Initialize(combatSurvivor);
			component.SetMirrored(prefabForActor.Mirrored);
			component.EnableModelChangeListener(enabled: false);
			EquipmentItemModel weaponEquipment = combatSurvivor.GetWeaponEquipment();
			if (weaponEquipment != null)
			{
				component.RequestSwitchEquipment(weaponEquipment);
			}
			gameObject.GetComponent<ShadowBlobOrient>().enabled = false;
			SurvivorAnimationController component2 = gameObject.GetComponent<SurvivorAnimationController>();
			component2.NotifyWeaponSwitch();
			component2.ForceIdle();
			actors.Add(gameObject);
			SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].lightProbeUsage = LightProbeUsage.Off;
			}
			MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].lightProbeUsage = LightProbeUsage.Off;
			}
			ActorPositions actorAnimationPositionFromIndex = GetActorAnimationPositionFromIndex(placementIndex, characterBuildType);
			AnimationClip loadingAnimationForWeapon = GetLoadingAnimationForWeapon(actorAnimationPositionFromIndex, component.CurrentWeapon);
			if (loadingAnimationForWeapon != null)
			{
				component2.StartCustomAnimation(loadingAnimationForWeapon);
				component2.ForceIdle();
			}
			Object.Destroy(gameObject.GetComponent<SurvivorAnimationController>());
			UnityUtils.StripPhysicsFromHierarchy(gameObject);
			component.SetVisible(visible: true);
			component.gameObject.SetLayerRecursively(18);
		}
		LoadingScreenSurvivorInfo component3 = uiContainer.AddChild(surviviorInfoCardPrefab).GetComponent<LoadingScreenSurvivorInfo>();
		component3.transform.OverlayPosition(uiPosition, base.gameObject.GetComponentInChildren<Camera>());
		Vector3 localPosition = component3.transform.localPosition;
		localPosition.z = 0f;
		component3.transform.localPosition = localPosition;
		component3.UpdateUI(combatSurvivor, placementIndex);
	}

	public void ShowDefenders()
	{
		List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Raider);
		if (factionActors != null && survivorPositions.Length > 5)
		{
			for (int i = 0; i < factionActors.Count; i++)
			{
				SurvivorModel combatSurvivor = factionActors[i] as SurvivorModel;
				DrawSurvivor(combatSurvivor, survivorPositions[i + 3].gameObject, survivorUIPositions[i + 3].position, i);
			}
		}
	}

	private void SetupWorldBossPVPDefenders()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (!(playerModel.GetAttackTargetMissionModel() is WorldBossMissionModel { WorldBossMissionType: WorldBossMissionType.PVP }) || survivorPositions == null || survivorPositions.Length <= 5 || survivorUIPositions == null || survivorUIPositions.Length <= 5)
		{
			return;
		}
		GuildBattlePvpTeam guildBattlePvpTeam = ((playerModel.WorldBossModelManager != null) ? playerModel.WorldBossModelManager.GetCurrentDefenderTeam() : null);
		if (guildBattlePvpTeam?.Survivors != null && guildBattlePvpTeam.Survivors.Count != 0)
		{
			int num = Mathf.Min(3, guildBattlePvpTeam.Survivors.Count);
			for (int i = 0; i < num; i++)
			{
				SurvivorMockData survivorMockData = guildBattlePvpTeam.Survivors[i];
				SurvivorModel combatSurvivor = playerModel.SurvivorContainer.CreateSurvivorFromSurvivorMockData(survivorMockData, survivorMockData.Level, preview: true);
				DrawSurvivor(combatSurvivor, survivorPositions[i + 3].gameObject, survivorUIPositions[i + 3].position, i, isTransient: true);
			}
		}
	}

	private void Start()
	{
		exiting = false;
		loadingContinueTimer = PvPAutoContinueTime;
		Active = true;
		loadingScreenInstance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
		loadingOver = false;
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAll();
		uiContainer = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Transition).transform.parent.gameObject.AddChild(undo: false);
		uiContainer.name = "Load_Screen_Combat_UI";
		base.gameObject.SetLayerRecursively(18);
		bool flag = false;
		GuildBattleMapMissionModel guildBattleMapMissionModel = GameManager.Instance.playerModel.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
		flag = guildBattleMapMissionModel?.IsEnemyUnlocked() ?? false;
		if (flag)
		{
			SetupGuildBattlePVPMissionSurvivors();
		}
		else
		{
			SetupMissionSurvivors();
			SetupWorldBossPVPDefenders();
		}
		loadingScreenCombatUi = uiContainer.AddChild(loadingScreenUIPrefab).GetComponent<LoadingScreenCombatUI>();
		if (loadingScreenCombatUi != null && loadingScreenCombatUi.GetComponent<UIWidget>() != null)
		{
			loadingScreenCombatUi.GetComponent<UIWidget>().SetAnchor(uiContainer.transform.root.gameObject, 0, 0, 0, 0);
			if (flag)
			{
				loadingScreenCombatUi.UpdatePlayersInfo();
			}
			loadingScreenCombatUi.ShowGuildWarsLabel(guildBattleMapMissionModel != null && !flag);
		}
		currentLoadingStep = 0;
		UpdateProgressBar();
		StartCoroutine(PauseAnimation());
		base.gameObject.SetLayerRecursively(18);
	}

	private IEnumerator PauseAnimation()
	{
		yield return new WaitForSeconds(0.5f);
		foreach (GameObject actor in actors)
		{
			actor.GetComponent<Animator>().enabled = false;
		}
	}

	private void OnDestroy()
	{
		EventManager.OnEvent -= OnEvent;
		Active = false;
		loadingScreenInstance = null;
	}

	private void Update()
	{
		if (!loadingOver)
		{
			return;
		}
		if (GameManager.Instance.playerModel.Combat.HasPvPRules)
		{
			loadingContinueTimer -= Time.deltaTime;
			if (loadingScreenCombatUi != null)
			{
				loadingScreenCombatUi.UpdateAutoContinueTimer((int)Mathf.Round(loadingContinueTimer));
			}
		}
		if (Input.GetMouseButtonDown(0) && loadingScreenCombatUi != null)
		{
			TweenManager.PlayTweenGroup(loadingScreenCombatUi.gameObject, 0);
		}
		if (!exiting && (loadingContinueTimer <= 0f || Input.GetMouseButtonUp(0)))
		{
			exiting = true;
			if (loadingScreenCombatUi != null)
			{
				loadingScreenCombatUi.Cleanup();
			}
			TransitionScreenHUD obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Transition) as TransitionScreenHUD;
			obj.AnimationInCallback = ShowCombat;
			obj.Open();
			if (loadingScreenCombatUi != null)
			{
				loadingScreenCombatUi.gameObject.SetActive(value: false);
			}
			GameManager.Instance.SurvivorsFromMission = null;
			if (Input.GetMouseButtonUp(0))
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			}
		}
	}

	private void UpdateProgressBar()
	{
		if (loadingScreenCombatUi != null)
		{
			loadingScreenCombatUi.UpdateProgress((float)currentLoadingStep / 5f);
		}
	}

	private void ShowCombat()
	{
		ShowCombatUI(show: true);
		RemoveUI();
		GameManager.Instance.StartCoroutine(DelayedStartCombat());
		Object.Destroy(base.gameObject);
		loadingScreenInstance = null;
	}

	private void RemoveUI()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.Transition);
		NGUITools.Destroy(uiContainer);
	}

	private IEnumerator DelayedStartCombat()
	{
		while (this != null)
		{
			yield return null;
		}
		yield return null;
		EventManager.NotifyEvent(EventManager.EventType.CombatStart);
	}

	public static void HideCombatScene()
	{
		if (Active)
		{
			ShowCombatUI(show: false);
		}
	}

	private static void ShowCombatUI(bool show)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD).gameObject.SetActive(show);
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatQuickTipPopup, null, createIfNotExist: false);
		if (hUDElement != null)
		{
			hUDElement.gameObject.SetActive(show);
		}
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		switch (eventType)
		{
		case EventManager.EventType.StateTransitionCompleted:
			StateTransitionCompleted();
			break;
		case EventManager.EventType.LoadingStepComplete:
			LoadingStepComplete();
			break;
		}
	}

	private void StateTransitionCompleted()
	{
		loadingOver = true;
		foreach (GameObject actor in actors)
		{
			actor.GetComponent<Animator>().enabled = true;
		}
		if (loadingScreenCombatUi != null)
		{
			loadingScreenCombatUi.LoadingOver();
		}
	}

	private void LoadingStepComplete()
	{
		currentLoadingStep++;
		UpdateProgressBar();
	}

	private AnimationClip GetLoadingAnimationForWeapon(ActorPositions actorPosition, EquipmentItemModel selectedWeapon)
	{
		AnimationClip result = null;
		EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(selectedWeapon);
		if (equipmentResourceEntry != null)
		{
			string text = "Loading_Screen_" + equipmentResourceEntry.AnimationId + "_" + actorPosition;
			result = Resources.Load<AnimationClip>("Loading_Screen_Animations/" + text);
		}
		return result;
	}

	private ActorPositions GetActorAnimationPositionFromIndex(int placementIndex, CharacterBuildType characterBuildType = CharacterBuildType.CombatLoading)
	{
		ActorPositions result = ActorPositions.Front;
		if (characterBuildType == CharacterBuildType.GuildBattleLoading)
		{
			result = ((placementIndex >= 3) ? ActorPositions.Defend : ActorPositions.Attack);
		}
		else if (placementIndex < 3)
		{
			result = (ActorPositions)placementIndex;
		}
		return result;
	}

	public static void Remove()
	{
		if (loadingScreenInstance != null)
		{
			loadingScreenInstance.RemoveUI();
			Object.Destroy(loadingScreenInstance.gameObject);
			loadingScreenInstance = null;
		}
	}

	private void SetupGuildBattlePVPMissionSurvivors()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		SurvivorContainerModel survivorContainer = playerModel.SurvivorContainer;
		for (int i = 0; i < survivorContainer.CombatSurvivors.Count; i++)
		{
			SurvivorModel combatSurvivor = survivorContainer.CombatSurvivors[i];
			DrawSurvivor(combatSurvivor, survivorPositions[i].gameObject, survivorUIPositions[i].position, i, isTransient: false, CharacterBuildType.GuildBattleLoading);
		}
		if (!(playerModel.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel))
		{
			DebugTWD.LogError("SetupGuildBattlePVPMissionSurvivors error", DebugType.Error);
			return;
		}
		GuildBattlePvpTeam pvpTeamForMission = playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMapMissionModel.Id);
		if (pvpTeamForMission != null)
		{
			playerModel.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);
			for (int j = 0; j < 3; j++)
			{
				int num = j + 3;
				SurvivorMockData survivorModel = pvpTeamForMission.Survivors[j];
				SurvivorModel survivorModel2 = playerModel.SurvivorContainer.CreateSurvivorFromSurvivorMockData(survivorModel, GvGModelHelper.GetPlayerSpecificDifficulty(playerModel), preview: true);
				ActorView.PrepareActor(survivorModel2, isTransient: true);
				DrawSurvivor(survivorModel2, survivorPositions[num].gameObject, survivorUIPositions[num].position, num, isTransient: true, guildBattleMapMissionModel.SavedData.Contains(j) ? CharacterBuildType.GuildBattleLoadingDefeated : CharacterBuildType.GuildBattleLoading);
			}
		}
	}

	private void SetupMissionSurvivors()
	{
		List<SurvivorModel> survivorsFromMission = GameManager.Instance.SurvivorsFromMission;
		int num = 0;
		List<int> list = new List<int> { 0, 1, 2 };
		for (int i = 0; i < (survivorsFromMission?.Count ?? 0); i++)
		{
			SurvivorModel survivorModel = survivorsFromMission[i];
			DrawSurvivor(survivorModel, survivorPositions[survivorModel.PvPDefenderIndex].gameObject, survivorUIPositions[survivorModel.PvPDefenderIndex].position, survivorModel.PvPDefenderIndex, isTransient: true);
			list.Remove(survivorModel.PvPDefenderIndex);
			num++;
		}
		SurvivorContainerModel survivorContainer = GameManager.Instance.playerModel.SurvivorContainer;
		for (int j = 0; j < survivorContainer.CombatSurvivors.Count; j++)
		{
			int num2 = list[0];
			SurvivorModel combatSurvivor = survivorContainer.CombatSurvivors[j];
			DrawSurvivor(combatSurvivor, survivorPositions[num2].gameObject, survivorUIPositions[num2].position, num2);
			list.Remove(num2);
		}
	}
}
