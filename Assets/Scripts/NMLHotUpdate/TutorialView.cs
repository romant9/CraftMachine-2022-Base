using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using BaseModel;
using TWDModel;
using UnityEngine;

public class TutorialView : ModelView<TutorialModel>
{
	public enum StartupSettingType
	{
		Normal = 0,
		SkipCombatTutorial = 1,
		SkipEntireTutorial = 2,
		SkipToEarlyGame = 3,
		SkipToLateGame = 4,
		SkipToMaxCouncil = 5
	}

	public const string StartWalkerTappingAfterMission = "S01E01M05CandleRoom";

	public const string MainTutorialId = "InitialCombat";

	public StartupSettingType StartupSetting;

	private TutorialUi tutorialUi;

	private CampModel camp;

	private CampView campView;

	private CampHUD hud;

	private string waitingClickType;

	private string clearSuggestionOnClickType;

	private string clickedType;

	private EventManager.EventType waitingEvent;

	private EventManager.EventType lastTriggeredEvent;

	private string allowActionId;

	private int resumedAtStep;

	private bool showDiamondsHud;

	private bool showGasHud;

	private bool showSuppliesHud;

	private bool showDailyQuestHud;

	private bool sendBasicTutorialCompleteEvent;

	private string currentAction;

	private List<string> suggestions;

	private List<GameObject> suggestedGameObjects;

	public static TutorialView Instance { get; protected set; }

	public bool Running { get; private set; }

	public static bool WasStartedWithSkipCheat { get; private set; }

	public GridPosition BuildingGridPosition { get; private set; }

	public bool IsSuggesting
	{
		get
		{
			if (suggestions != null)
			{
				return suggestions.Count > 0;
			}
			return false;
		}
	}

	public bool IsWaitingForClick => waitingClickType != null;

	public bool RunningButNotSuggesting
	{
		get
		{
			if (Running)
			{
				return !IsSuggesting;
			}
			return false;
		}
	}

	public bool IsDialogPlaying
	{
		get
		{
			if (tutorialUi != null)
			{
				return tutorialUi.IsActorTalking;
			}
			return false;
		}
	}

	public bool PerformingActions { get; private set; }

	public bool IsInInitialPart => !base.Model.HasCompletedPart("InitialCombat");

	public bool InCombatTutorial { get; set; }

	public bool ShowCombatEndScreen { get; set; }

	public bool IsArrowActive
	{
		get
		{
			if (tutorialUi != null)
			{
				return tutorialUi.IsArrowActive();
			}
			return false;
		}
	}

	public bool IsHandActive
	{
		get
		{
			if (tutorialUi != null)
			{
				return tutorialUi.IsHandActive();
			}
			return false;
		}
	}

	public string CurentPerformingAction { get; private set; }

	private void Awake()
	{
		EventManager.OnEvent += OnEvent;
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		WasStartedWithSkipCheat = StartupSetting != StartupSettingType.Normal;
	}

	public void InitializeForCamp()
	{
		campView = CampView.Instance;
		camp = campView.Model;
		hud = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		hud.Open();
		if (StartupSetting >= StartupSettingType.SkipEntireTutorial)
		{
			base.Model.SetPart(null);
		}
		else if (ValidateResumePartId(base.Model.CurrentPartId))
		{
			resumedAtStep = base.Model.CurrentStep;
			ActivateTutorial();
			StartStep();
		}
		else
		{
			StartPart("Tutorial");
		}
		InitUi();
	}

	public bool ValidateResumePartId(string partId)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(partId))
		{
			result = !(partId == "HeroTrait") && (!(partId == "HeroPromote") || CanPlayPromoteDarylTutorial());
		}
		return result;
	}

	public void InitializeForCombat()
	{
		if (base.Model.CurrentPartId != null)
		{
			StartPart(base.Model.CurrentPartId);
		}
		else
		{
			StartPart("InitialCombat");
		}
	}

	public bool IsTutorialUIEnabled()
	{
		if (tutorialUi != null)
		{
			return tutorialUi.IsEnabled();
		}
		return false;
	}

	private void OnDestroy()
	{
		EventManager.OnClick -= OnClick;
		EventManager.OnEvent -= OnEvent;
	}

	private void InitUi()
	{
		tutorialUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial) as TutorialUi;
		if (!tutorialUi.IsOpen)
		{
			tutorialUi.OpenForModel(base.Model);
		}
	}

	public bool Allow(string actionId)
	{
		if (!Running || IsSuggesting)
		{
			return true;
		}
		if (!(actionId == waitingClickType))
		{
			return actionId == allowActionId;
		}
		return true;
	}

	public static bool Allowed(string actionId)
	{
		if (Instance != null && Instance.Running && !Instance.Allow(actionId) && !Instance.IsSuggesting)
		{
			return false;
		}
		return true;
	}

	public bool MoveBuildingAllowed()
	{
		if (Instance.Running)
		{
			if (currentAction == "Upgrade" || currentAction == "UpgradeSurvivor" || currentAction == "SpeedUp")
			{
				return false;
			}
			if (waitingClickType == "TentsButton" || waitingClickType == "BuildingMenuUpgrade" || waitingClickType == "Council" || waitingClickType == "TrainingGround" || waitingClickType == "BuildingProduceSupplies")
			{
				return false;
			}
		}
		return true;
	}

	private void ActivateTutorial()
	{
		InitUi();
		EventManager.OnClick += OnClick;
		base.Model.Changed += OnTutorialChanged;
	}

	private void OnTutorialChanged(ModelObject model, string changed, object args)
	{
		if (changed == "NextStepEvent")
		{
			OnClick("NextStepEvent");
		}
	}

	public void Stop()
	{
		Running = false;
		PerformingActions = false;
		InCombatTutorial = false;
		StopAllCoroutines();
		HideArrow();
		SetEnabledAllButtons(enabled: true);
		if (suggestedGameObjects != null)
		{
			suggestedGameObjects.Clear();
		}
		currentAction = null;
		CurentPerformingAction = null;
	}

	public bool StartPart(string partId)
	{
		if (base.Model != null && base.Model.PartExists(partId) && !base.Model.HasCompletedPart(partId))
		{
			resumedAtStep = -1;
			ActivateTutorial();
			Helpers.ExecuteCommand(new SetTutorialPartCommand(base.Model)
			{
				PartId = partId
			});
			StartStep();
			return true;
		}
		return false;
	}

	public bool ResumeCurrentPart()
	{
		if (base.Model != null && ValidateResumePartId(base.Model.CurrentPartId))
		{
			resumedAtStep = base.Model.CurrentStep;
			ActivateTutorial();
			StartStep();
			return true;
		}
		return false;
	}

	private void StartStep()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.BundleCardPopup);
		if (base.Model.CurrentPartId == "RewardsScreen3")
		{
			sendBasicTutorialCompleteEvent = true;
		}
		Running = true;
		StopAllCoroutines();
		StartCoroutine(DoStepActions(base.Model.GetCurrentActions, base.Model.GetCurrentStepDefinition));
	}

	public void StartCutScene(List<string> actions, Callback callback = null)
	{
		Running = true;
		if (tutorialUi == null)
		{
			tutorialUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial) as TutorialUi;
		}
		if (!tutorialUi.IsOpen)
		{
			tutorialUi.Open();
		}
		StartCoroutine(DoStepActions(actions, null, delegate
		{
			EndCutScene(callback);
		}));
	}

	public void ShowDialogWithHighlightedObjects(HardCodedTutorialData hardCodedTutorialData, Callback callback = null)
	{
		Running = true;
		if (tutorialUi == null)
		{
			tutorialUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial) as TutorialUi;
		}
		if (!tutorialUi.IsOpen)
		{
			tutorialUi.Open();
		}
		StartCoroutine(StartHardCodedTutorialCutScene(hardCodedTutorialData, delegate
		{
			EndCutScene(callback);
		}));
	}

	private IEnumerator StartHardCodedTutorialCutScene(HardCodedTutorialData hardCodedTutorialData, Callback callback)
	{
		yield return new WaitForSeconds(hardCodedTutorialData.TutorialStartDelay);
		if (tutorialUi != null && tutorialUi.isActiveAndEnabled)
		{
			VisualizationQueue.Instance.PauseVisualizations(pause: true);
			for (int i = 0; i < hardCodedTutorialData.Localizations.Count; i++)
			{
				List<string> uIElementsToHighlight = hardCodedTutorialData.UIElementsToHighlight;
				if (uIElementsToHighlight != null && uIElementsToHighlight.Count > i && !string.IsNullOrEmpty(hardCodedTutorialData.UIElementsToHighlight[i]))
				{
					tutorialUi.ClearHighlightedObjects();
					string[] array = hardCodedTutorialData.UIElementsToHighlight[i].Split('-');
					foreach (string text in array)
					{
						GameObject gameObject = GameObject.Find(text);
						if (gameObject != null)
						{
							GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, tutorialUi.HighlightedObjectContainer);
							gameObject2.SetLayerRecursively(tutorialUi.gameObject.layer);
							gameObject2.transform.position = gameObject.transform.position;
							gameObject2.transform.localPosition = new Vector3(gameObject2.transform.localPosition.x, gameObject2.transform.localPosition.y, 0f);
							UIPanel component2;
							if (gameObject2.TryGetComponent<UIWidget>(out var component))
							{
								component.SetAnchor((Transform)null);
								component.ResetAndUpdateAnchors();
								component.depth = 10;
							}
							else if (gameObject2.TryGetComponent<UIPanel>(out component2))
							{
								component2.SetAnchor((Transform)null);
								component2.ResetAndUpdateAnchors();
								component2.depth += 100;
							}
						}
						else
						{
							Debug.LogError("Gameobject " + text + " not found to be highlighted");
						}
					}
				}
				TutorialUi obj = tutorialUi;
				string portraitId = hardCodedTutorialData.PortraitId;
				string textId = hardCodedTutorialData.Localizations[i];
				bool showDialogOnCenter = hardCodedTutorialData.ShowDialogOnCenter;
				List<object> localizationArguments = hardCodedTutorialData.LocalizationArguments;
				yield return obj.Say(portraitId, textId, waitForClick: true, showDialogOnCenter, (localizationArguments != null && localizationArguments.Count > i) ? hardCodedTutorialData.LocalizationArguments[i] : null);
				if (i == hardCodedTutorialData.Localizations.Count - 1)
				{
					VisualizationQueue.Instance.PauseVisualizations(pause: false);
					tutorialUi.ClearHighlightedObjects();
					yield return tutorialUi.HideCharacter();
				}
			}
		}
		OnTutorialStepCompleted(null, callback, sendNextStepCommand: false);
	}

	private void EndCutScene(Callback callback)
	{
		if (base.Model.CurrentPartId == null)
		{
			Running = false;
		}
		callback?.Invoke();
	}

	private float ParseFloat(string value)
	{
		return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
	}

	private IEnumerator DoStepActions(List<string> actions, TutorialStepDefinition tutorialStepDefinition = null, Callback callback = null)
	{
		if (tutorialStepDefinition != null && !tutorialStepDefinition.IsForCombat && campView != null)
		{
			hud.SetupTutorialHUD(tutorialStepDefinition);
			EnableCampControls(tutorialStepDefinition.AllowCampSelect);
			SetEnabledAllButtons(enabled: false);
		}
		clearSuggestionOnClickType = null;
		showDiamondsHud = false;
		showGasHud = false;
		showSuppliesHud = false;
		showDailyQuestHud = false;
		yield return null;
		PerformingActions = true;
		bool forceCompleteAllActions = false;
		ClearSuggestions();
		bool sendNextStepCommand = true;
		foreach (string action in actions)
		{
			string[] actionToExecuteParts = action.Split(',');
			currentAction = actionToExecuteParts[0];
			CurentPerformingAction = action;
			DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp, null, createIfNotExist: false) as DetailMapPopUp;
			if (tutorialStepDefinition != null)
			{
				SingularityMonoBehaviour<SDKManager>.Instance.TutorialClient(base.Model, currentAction);
			}
			switch (actionToExecuteParts[0])
			{
			case "Video":
				GameManager.Instance.EnableAssetLoaderUI(enable: false);
				break;
			case "Combat":
			{
				string missionId = actionToExecuteParts[1];
				ShowCombatEndScreen = actionToExecuteParts[2].ToLower() == "true";
				GameManager.Instance.LoadVisitModel(missionId);
				InCombatTutorial = true;
				while (InCombatTutorial)
				{
					yield return null;
				}
				SingularityMonoBehaviour<AudioManager>.Instance.EndCombatMusicSync();
				break;
			}
			case "Wait":
			{
				float time = ParseFloat(actionToExecuteParts[1]);
				string eventThatEndsWait = null;
				if (actionToExecuteParts.Length > 2)
				{
					eventThatEndsWait = actionToExecuteParts[2];
				}
				yield return StartCoroutine(Wait(time, eventThatEndsWait));
				break;
			}
			case "Dialog":
			{
				if (actionToExecuteParts[1].ToLower() == "hide")
				{
					yield return tutorialUi.HideCharacter();
					break;
				}
				if (SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampBuildMenu) != null && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampBuildMenu).IsOpen)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampBuildMenu).Close();
				}
				if (CampView.Instance != null && CampView.Instance.CampViewBuildings != null)
				{
					CampView.Instance.CancelBuildingPlacement();
				}
				bool waitForClick = ((actionToExecuteParts.Length <= 3 || !(actionToExecuteParts[3] == "noclick")) ? true : false);
				if (tutorialUi != null && tutorialUi.isActiveAndEnabled)
				{
					yield return tutorialUi.Say(actionToExecuteParts[1], actionToExecuteParts[2], waitForClick);
					if (actionToExecuteParts.Length > 3 && actionToExecuteParts[3] == "hide")
					{
						yield return tutorialUi.HideCharacter();
					}
				}
				break;
			}
			case "CameraPan":
				Pan(new Vector3(ParseFloat(actionToExecuteParts[1]), ParseFloat(actionToExecuteParts[2]), ParseFloat(actionToExecuteParts[3])), ParseFloat(actionToExecuteParts[4]), ParseFloat(actionToExecuteParts[5]));
				break;
			case "CameraPanToBuilding":
			{
				BuildingModel building = camp.GetBuilding(actionToExecuteParts[1]);
				if (building != null)
				{
					Pan(building, ParseFloat(actionToExecuteParts[2]), ParseFloat(actionToExecuteParts[3]));
				}
				break;
			}
			case "CameraPanToStoryTeller":
				if (campView.CampViewActors.StoryTellerViews[0] != null && campView.CampViewActors.StoryTellerViews != null)
				{
					Vector3 vector = new Vector3(ParseFloat(actionToExecuteParts[1]), 0f, ParseFloat(actionToExecuteParts[2]));
					Pan(campView.CampViewActors.StoryTellerViews[0].transform.position + vector, ParseFloat(actionToExecuteParts[3]), ParseFloat(actionToExecuteParts[4]));
				}
				if (CampView.Instance != null && CampView.Instance.CampViewBuildings != null)
				{
					CampView.Instance.CancelBuildingPlacement();
				}
				break;
			case "AddRunningSurvivors":
				CampView.Instance.CampViewActors.InitTutorialRunningCharacters();
				break;
			case "CameraSet":
				campView.CameraController.Reset(new Vector3(ParseFloat(actionToExecuteParts[1]), ParseFloat(actionToExecuteParts[2]), ParseFloat(actionToExecuteParts[3])), campView.CameraController.Distance);
				break;
			case "Allow":
				yield return StartCoroutine(AllowAction(actionToExecuteParts[1]));
				break;
			case "Arrow":
				ShowArrow(actionToExecuteParts[1]);
				break;
			case "DisableAllButtons":
				SetEnabledAllButtons(enabled: false);
				break;
			case "EnableAllButtons":
				SetEnabledAllButtons(enabled: true);
				break;
			case "EnableButtons":
				SetEnabledButtons(actionToExecuteParts[1], actionToExecuteParts[2], enable: true);
				break;
			case "WaitClick":
				SetEnabledAllButtons(enabled: false);
				yield return StartCoroutine(WaitClick(actionToExecuteParts[1], hideArrow: true));
				break;
			case "WaitClickDontDisableButtons":
				yield return StartCoroutine(WaitClick(actionToExecuteParts[1], hideArrow: true, disableAllButtonsAfterClick: true));
				break;
			case "WaitEvent":
				yield return StartCoroutine(WaitEvent(actionToExecuteParts[1]));
				break;
			case "WaitClickButton":
			{
				string text = actionToExecuteParts[1];
				string clickEvent = ((actionToExecuteParts.Length == 2) ? actionToExecuteParts[1] : actionToExecuteParts[2]);
				if (text.Equals("MissionHub") && (base.Model.CurrentPartId.Equals("ScavengeMode") || base.Model.CurrentPartId.Equals("SeasonsMode") || base.Model.CurrentPartId.Equals("ChallengeMode") || base.Model.CurrentPartId.Equals("SurvivalMode") || base.Model.CurrentPartId.Equals("EndlessMode") || base.Model.CurrentPartId.Equals("GuildBattleMode") || base.Model.CurrentPartId.Equals("OutpostMode")))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs(new List<UIType> { UIType.MissionHubPopup });
				}
				yield return StartCoroutine(WaitClickButton(text, clickEvent, disableAllButtonsAfterClick: true));
				break;
			}
			case "Build":
			{
				hud.SetupTutorialHUD(tutorialStepDefinition);
				yield return StartCoroutine(WaitClickButton("BuildMenu", "BuildMenu"));
				yield return tutorialUi.HideCharacter();
				string buildingName = actionToExecuteParts[1];
				if (actionToExecuteParts.Length < 3 || string.IsNullOrEmpty(actionToExecuteParts[2]))
				{
					BuildingGridPosition = null;
				}
				else
				{
					FixedVec2 fixedVec = GameManager.Instance.playerModel.Camp.TransformGroundToGridPosition(new FixedVec2(ParseFloat(actionToExecuteParts[2]), ParseFloat(actionToExecuteParts[3])));
					BuildingGridPosition = new GridPosition(fixedVec.X, fixedVec.Y);
				}
				yield return null;
				SetButtonToClick(buildingName);
				SetEnablePopupButtons(UIType.CampBuildMenu, enable: true);
				yield return StartCoroutine(WaitClickButton(buildingName));
				yield return StartCoroutine(WaitClickButton("BuildOk", "NextStepEvent"));
				sendNextStepCommand = false;
				yield return null;
				break;
			}
			case "ClickBuilding":
			{
				string buildingName = actionToExecuteParts[1];
				yield return StartCoroutine(WaitClickBuilding(buildingName));
				break;
			}
			case "Upgrade":
			{
				string buildingName = actionToExecuteParts[1];
				yield return StartCoroutine(WaitClickBuilding(buildingName));
				yield return tutorialUi.HideCharacter();
				yield return StartCoroutine(WaitClickButton("BuildingMenuUpgrade", "BuildingMenuUpgrade"));
				yield return StartCoroutine(WaitClickButton("Buy", "NextStepEvent"));
				sendNextStepCommand = false;
				break;
			}
			case "SpeedUp":
			{
				int num = 0;
				for (int i = 0; i < campView.CampViewBuildings.Buildings.Count; i++)
				{
					if (campView.CampViewBuildings.Buildings[i].Model.IsUpgrading)
					{
						num = (int)campView.CampViewBuildings.Buildings[i].Model.UpgradeTimer / 1000;
					}
				}
				if (num == 0)
				{
					TrainingGroundBuildingModel trainingGroundBuildingModel = (TrainingGroundBuildingModel)GameManager.Instance.playerModel.Camp.GetBuilding("TrainingGround");
					if (trainingGroundBuildingModel != null)
					{
						SurvivorModel upgradingSurvivor = trainingGroundBuildingModel.UpgradingSurvivor;
						if (upgradingSurvivor != null)
						{
							num = (int)upgradingSurvivor.TimedActionModel.MillisecondsTillCompletion / 1000;
						}
					}
				}
				if (num > 0)
				{
					if (actionToExecuteParts.Length > 1 && actionToExecuteParts[1] == "Force")
					{
						SetButtonToClick("BuildingMenuSpeedUp");
					}
					else
					{
						EnabledButton("BuildingMenuSpeedUp", enabled: true);
					}
					yield return StartCoroutine(Wait((float)num + 0.5f, "NextStepEvent"));
				}
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
				}
				sendNextStepCommand = false;
				HideArrow();
				hud.SetupTutorialHUD(tutorialStepDefinition);
				break;
			}
			case "UpgradeSurvivor":
				yield return StartCoroutine(WaitClickBuilding("TrainingGround"));
				yield return tutorialUi.HideCharacter();
				yield return StartCoroutine(WaitClickButton("TentsButton", "TentsButton"));
				CampManager.Instance.CampBackground.ApplyCampBounds();
				AddSuggestion("CampTrainingGrounds");
				AddSuggestion("SurvivorCard_Ken");
				AddSuggestion("SurvivorCard_Ann");
				AddSuggestion("SurvivorCard_Joel");
				AddSuggestion("Open-train");
				AddSuggestion("Train");
				clearSuggestionOnClickType = "Open-Train";
				clearSuggestionOnClickType = "Train";
				yield return StartCoroutine(WaitClick("NextStepEvent", hideArrow: false));
				sendNextStepCommand = false;
				showDailyQuestHud = true;
				break;
			case "AcceptSurvivor":
				allowActionId = "AcceptSurvivor";
				yield return StartCoroutine(WaitClickButton("AcceptSurvivor", "NextStepEvent"));
				sendNextStepCommand = false;
				break;
			case "SwitchWeapon":
				yield return StartCoroutine(WaitClickBuilding("TrainingGround"));
				yield return StartCoroutine(WaitClickButton("TentsButton", "TentsButton"));
				yield return StartCoroutine(WaitClickButton("Equipment_Equiped", "Click_Equipment"));
				yield return StartCoroutine(WaitClickButton("Equipment_Owned", "Equipment_Owned"));
				yield return StartCoroutine(WaitClickButton("Equipment_Equiped", "Click_Equipment"));
				yield return StartCoroutine(WaitClickButton("Close", "Close"));
				break;
			case "PlayMission":
				if (detailMapPopUp == null || !detailMapPopUp.IsOpen)
				{
					yield return StartCoroutine(WaitClickButton("MapButton", "MapButton"));
				}
				yield return StartCoroutine(WaitClickButton(actionToExecuteParts[2], actionToExecuteParts[2]));
				yield return StartCoroutine(WaitClickButton("SelectTeam", "SelectTeam"));
				yield return StartCoroutine(WaitClickButton("StartMission", "StartMission"));
				yield return StartCoroutine(WaitEvent("CombatStartTutorial"));
				break;
			case "CollectSupplies":
				yield return StartCoroutine(WaitClickButton("Collect_Supplies_Indicator", "NextStepEvent"));
				sendNextStepCommand = false;
				break;
			case "StartQuest":
				yield return StartCoroutine(AllowAction("StoryTeller"));
				ShowArrow("StoryTeller");
				yield return StartCoroutine(WaitClickButton("StoryTeller", "StoryTeller"));
				yield return tutorialUi.HideCharacter();
				SetEnabledAllButtons(enabled: false);
				yield return StartCoroutine(WaitClickButton("PositiveButton", "NextStepEvent"));
				sendNextStepCommand = false;
				break;
			case "GiveCurrency":
			{
				CurrencyType currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), actionToExecuteParts[1]);
				CampView.Instance.BuildingsHud.CreateCollectAnim(currencyType, campView.CampViewActors.StoryTellerViews[0].gameObject, GameManager.Instance.playerModel.GetCurrency(currencyType).Value, OnCurrencyFlyingDone);
				ShowCurrency(actionToExecuteParts[1]);
				break;
			}
			case "ShowDailyQuestHud":
				showDailyQuestHud = true;
				break;
			case "InitWalkersTapping":
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.InCampDefenseTutorial = true;
				}
				break;
			case "EndWalkersTapping":
				EnabledButton("DefenseWalker", enabled: true);
				while (CampDefenseView.Instance.Model.Walkers.Count != 0)
				{
					yield return 0;
				}
				break;
			case "DefenseWalkerTap":
				yield return null;
				allowActionId = "DefenseWalker";
				yield return StartCoroutine(WaitClickButton("DefenseWalker", "NextStepEvent"));
				sendNextStepCommand = false;
				if (CampDefenseView.Instance != null && SingularityMonoBehaviour<AudioManager>.Instance != null && CampDefenseView.Instance.Model.Walkers.Count == 0)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.InCampDefenseTutorial = false;
				}
				break;
			case "CameraToWalker":
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.OnCampDefenseAdded(1);
				}
				yield return null;
				if (CampDefenseView.Instance.Model.Walkers != null && CampDefenseView.Instance.Model.Walkers.Count > 0)
				{
					ActorView actorView = GameManager.Instance.GetViewForModel((ActorModel)CampDefenseView.Instance.Model.Walkers[0]) as ActorView;
					Pan(actorView.transform.position, ParseFloat(actionToExecuteParts[1]), ParseFloat(actionToExecuteParts[2]));
				}
				break;
			case "HideButton":
				ShowButton(actionToExecuteParts[1], show: false);
				break;
			case "ForceMap":
				if (detailMapPopUp == null || !detailMapPopUp.IsOpen)
				{
					CampManager.Instance.GoToMap();
					yield return null;
				}
				break;
			case "ForceCamp":
				if (detailMapPopUp != null && detailMapPopUp.IsOpen)
				{
					CampManager.Instance.GoToCamp();
					yield return null;
				}
				break;
			case "SkipIfRestoredHere":
				if (resumedAtStep == base.Model.CurrentStep)
				{
					forceCompleteAllActions = true;
				}
				break;
			case "RewardScreenCanReturnToCamp":
				if (RewardScreenHandler.Instance != null && RewardScreenHandler.Instance.GetKeysLeft() != 0)
				{
					yield return StartCoroutine(WaitClick("CanReturnToCamp", hideArrow: false));
				}
				break;
			case "ShowCurrency":
				ShowCurrency(actionToExecuteParts[1]);
				break;
			case "QuickTip":
			{
				PopupQuickTip popupQuickTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatQuickTipPopup) as PopupQuickTip;
				popupQuickTip.TipId = actionToExecuteParts[1];
				popupQuickTip.Open();
				while (popupQuickTip.IsOpen)
				{
					yield return null;
				}
				break;
			}
			case "OutpostHelp":
			{
				OutpostEditPopup outpostEditPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupEdit) as OutpostEditPopup;
				if (outpostEditPopup != null)
				{
					outpostEditPopup.ShowOutpostTutorialFromClick();
				}
				break;
			}
			case "SendEvent":
				EventManager.NotifyEvent(EventManager.EventType.TutorialEvent, actionToExecuteParts[1]);
				break;
			case "Suggest":
				if (actionToExecuteParts.Length > 1)
				{
					AddSuggestion(actionToExecuteParts[1]);
				}
				break;
			case "VoiceOver":
				if (actionToExecuteParts.Length > 1)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayVoiceOver(int.Parse(actionToExecuteParts[1]));
				}
				break;
			default:
				Debug.LogError("Unknown action '" + actionToExecuteParts[0] + "' configured in GED.");
				break;
			case "WaitReturnToCamp":
				break;
			}
			if (forceCompleteAllActions)
			{
				break;
			}
		}
		OnTutorialStepCompleted(tutorialStepDefinition, callback, sendNextStepCommand);
	}

	private void AddSuggestion(string suggestion)
	{
		if (suggestions == null)
		{
			suggestions = new List<string>();
		}
		suggestions.Add(suggestion);
		SetEnabledAllButtons(enabled: true);
		UpdateSuggestion();
	}

	private void OnTutorialStepCompleted(TutorialStepDefinition tutorialStepDefinition, Callback callback, bool sendNextStepCommand)
	{
		PerformingActions = false;
		HideSuggestion();
		currentAction = null;
		CurentPerformingAction = null;
		clickedType = null;
		if (tutorialStepDefinition != null)
		{
			CompleteStep(sendNextStepCommand);
		}
		callback?.Invoke();
		EventManager.NotifyEvent(EventManager.EventType.TutorialPartOver);
	}

	private void ShowCurrency(string currency)
	{
		switch (currency)
		{
		case "Supplies":
			showSuppliesHud = true;
			break;
		case "Diamonds":
			showDiamondsHud = true;
			break;
		case "Gas":
			showGasHud = true;
			break;
		}
	}

	private void OnCurrencyFlyingDone(bool iscomplete, CurrencyType currency)
	{
		hud.SetupTutorialHUD(base.Model.GetCurrentStepDefinition);
	}

	private IEnumerator AllowAction(string actionId)
	{
		allowActionId = actionId;
		yield return 0;
	}

	private bool SetButtonToClick(string buttonName)
	{
		SetEnabledAllButtons(enabled: false);
		EnabledButton(buttonName, enabled: true);
		return ShowArrow(buttonName);
	}

	private IEnumerator WaitClickBuilding(string buildingName)
	{
		ShowArrow(buildingName);
		SetEnabledAllButtons(enabled: false);
		yield return StartCoroutine(WaitClick(buildingName, hideArrow: true));
		yield return null;
		SetEnabledAllButtons(enabled: false);
	}

	private IEnumerator WaitClickButton(string buttonName, string clickEvent, bool disableAllButtonsAfterClick = false)
	{
		if (SetButtonToClick(buttonName))
		{
			yield return StartCoroutine(WaitClick(clickEvent, hideArrow: true, disableAllButtonsAfterClick));
		}
		yield return null;
		SetEnabledAllButtons(enabled: false);
	}

	private IEnumerator WaitClickButton(string clickEvent)
	{
		yield return StartCoroutine(WaitClick(clickEvent, hideArrow: true));
		yield return null;
		SetEnabledAllButtons(enabled: false);
	}

	private void CompleteStep(bool sendNextStepCommand)
	{
		if (sendNextStepCommand)
		{
			Helpers.ExecuteCommand(new NextTutorialStepCommand(base.Model)
			{
				ShowDiamondsHud = showDiamondsHud,
				ShowGasHud = showGasHud,
				ShowSuppliesHud = showSuppliesHud,
				ShowDailyQuestHud = showDailyQuestHud
			});
		}
		if (base.Model.Completed)
		{
			TutorialComplete();
		}
		else
		{
			StartStep();
		}
	}

	private void TutorialComplete()
	{
		if (base.Model.CurrentPartId == "InitialCombat")
		{
			GameManager.Instance.GameCenterManager.ReportProgress("CP_TWD_TUTORIAL_COMPLETED", 1, 1);
		}
		if (base.Model.completedParts.Count != 0)
		{
			_ = base.Model.completedParts[base.Model.completedParts.Count - 1];
		}
		Running = false;
		if (base.Model.StaticTutorialComplete && sendBasicTutorialCompleteEvent)
		{
			sendBasicTutorialCompleteEvent = false;
			SingularityMonoBehaviour<SDKManager>.Instance.ExternalAnalytics.TutorialCompleted();
		}
		if (base.Model.CurrentPartDefinition == null || !base.Model.CurrentPartDefinition.IsForCombat)
		{
			EnableCampControls(enable: true);
			SetEnabledAllButtons(enabled: true);
			if (hud != null)
			{
				hud.SetupTutorialHUD();
			}
		}
		GameManager.Instance.RequestPltv();
	}

	private void EnableCampControls(bool enable)
	{
		if (campView != null)
		{
			campView.EnableCampControls(enable);
		}
	}

	private void EnableMapControls(bool enable)
	{
	}

	public void SetEnabledAllButtons(bool enabled)
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in array)
		{
			hashSet.Add(gameObject);
			foreach (Transform componentInChild in gameObject.GetComponentInChildren<Transform>(includeInactive: true))
			{
				hashSet.Add(componentInChild.gameObject);
			}
		}
		foreach (GameObject item in hashSet)
		{
			if (item.TryGetComponent<UIButton>(out var component))
			{
				component.isEnabled = enabled;
			}
		}
		if (enabled)
		{
			SetEnablePopupButtons(UIType.DefaultPopup, enable: true);
			SetEnablePopupButtons(UIType.CampBuildingMenu, enable: true);
			SetEnablePopupButtons(UIType.CampCampMapHud, enable: true);
			SetEnablePopupButtons(UIType.CampTrainingGrounds, enable: true);
		}
		else
		{
			SetEnablePopupButtons(UIType.ConfirmationPopup, enable: true);
		}
	}

	private void SetEnablePopupButtons(UIType uiType, bool enable)
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType, null, createIfNotExist: false);
		if (!(hUDElement != null))
		{
			return;
		}
		List<UIButton> list = ListPool<UIButton>.Get();
		hUDElement.gameObject.GetComponentsInChildren(includeInactive: true, list);
		if (list != null)
		{
			foreach (UIButton item in list)
			{
				item.isEnabled = enable;
			}
		}
		ListPool<UIButton>.Release(list);
	}

	public void SetEnabledButtons(string popupName, string gameobjectName, bool enable)
	{
		UIType uiType = UIType.None;
		try
		{
			uiType = (UIType)Enum.Parse(typeof(UIType), popupName);
		}
		catch (Exception)
		{
		}
		SetEnabledButtons(uiType, gameobjectName, enable);
	}

	public void SetEnabledButtons(UIType uiType, string gameobjectName, bool enable)
	{
		if (uiType == UIType.None)
		{
			return;
		}
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType, null, createIfNotExist: false);
		if (!(hUDElement != null))
		{
			return;
		}
		Transform obj = hUDElement.transform.Find(gameobjectName);
		List<UIButton> list = ListPool<UIButton>.Get();
		obj.gameObject.GetComponentsInChildren(list);
		if (list != null)
		{
			foreach (UIButton item in list)
			{
				item.isEnabled = enable;
			}
		}
		ListPool<UIButton>.Release(list);
	}

	private void EnabledButton(string id, bool enabled)
	{
		UIButton button = GetButton(id);
		if (button != null)
		{
			button.isEnabled = enabled;
		}
	}

	private void ShowButton(string id, bool show)
	{
		UIButton button = GetButton(id);
		if (button != null)
		{
			button.gameObject.SetActive(show);
		}
	}

	private UIButton GetButton(string id)
	{
		UIButton uIButton = null;
		TutorialArrowParent tutorialArrowParent = GetTutorialArrowParent(id);
		if (tutorialArrowParent != null)
		{
			uIButton = tutorialArrowParent.gameObject.GetComponent<UIButton>();
			if (uIButton == null)
			{
				uIButton = tutorialArrowParent.transform.parent.gameObject.GetComponent<UIButton>();
				if (uIButton == null)
				{
					uIButton = tutorialArrowParent.transform.parent.GetComponentInParent<UIButton>();
				}
			}
		}
		return uIButton;
	}

	public void OnClick(string clickType)
	{
		if (clearSuggestionOnClickType != null && clearSuggestionOnClickType == clickType)
		{
			HideSuggestion();
			ClearSuggestions();
			clearSuggestionOnClickType = null;
		}
		if (waitingClickType == null || waitingClickType != clickedType)
		{
			clickedType = clickType;
			if (waitingClickType == clickedType)
			{
				SetEnabledAllButtons(enabled: true);
			}
		}
	}

	private IEnumerator WaitClick(string type, bool hideArrow, bool disableAllButtonsAfterClick = false)
	{
		waitingClickType = type;
		while (waitingClickType != clickedType)
		{
			yield return null;
		}
		if (disableAllButtonsAfterClick)
		{
			SetEnabledAllButtons(enabled: false);
		}
		clickedType = null;
		if (hideArrow)
		{
			tutorialUi.HideArrow();
		}
		waitingClickType = null;
		allowActionId = null;
		yield return null;
		yield return null;
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype == EventManager.EventType.CombatStartTutorial && base.Model != null && base.Model.GetCurrentStepDefinition != null && !base.Model.GetCurrentStepDefinition.IsForCombat && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Combat != null && !GameManager.Instance.playerModel.Combat.MissionCompleted)
		{
			StopAllCoroutines();
			HideArrow();
			OnTutorialStepCompleted(base.Model.GetCurrentStepDefinition, null, sendNextStepCommand: true);
		}
		lastTriggeredEvent = eventtype;
	}

	private IEnumerator WaitEvent(string eventName)
	{
		EventManager.EventType eventType = (EventManager.EventType)Enum.Parse(typeof(EventManager.EventType), eventName);
		waitingEvent = eventType;
		while (waitingEvent != lastTriggeredEvent)
		{
			yield return 0;
		}
		waitingEvent = EventManager.EventType.None;
		lastTriggeredEvent = EventManager.EventType.None;
		yield return 0;
		yield return null;
	}

	private IEnumerator Wait(float time, string eventThatEndsWait = null)
	{
		waitingClickType = eventThatEndsWait;
		time *= 1000f;
		Stopwatch stopwatch = Stopwatch.StartNew();
		while ((float)stopwatch.ElapsedMilliseconds < time && (waitingClickType == null || waitingClickType != clickedType))
		{
			yield return 0;
		}
		waitingClickType = null;
		yield return null;
		yield return null;
	}

	private TutorialArrowParent GetTutorialArrowParent(string id)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("TutorialArrowParent");
		for (int i = 0; i < array.Length; i++)
		{
			TutorialArrowParent component = array[i].GetComponent<TutorialArrowParent>();
			if (component.Id == id)
			{
				return component;
			}
		}
		return null;
	}

	public void ShowHandDrag(Vector3 dragStart, Vector3 dragEnd)
	{
		if (tutorialUi == null)
		{
			tutorialUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial) as TutorialUi;
		}
		if (!tutorialUi.IsOpen)
		{
			tutorialUi.Open();
		}
		tutorialUi.ShowHand(dragStart, dragEnd);
	}

	public void ShowHandTap(Vector3 tapTarget)
	{
		if (tutorialUi == null)
		{
			tutorialUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial) as TutorialUi;
			tutorialUi.Open();
		}
		tutorialUi.ShowHand(tapTarget);
	}

	public void HideHand()
	{
		if (tutorialUi != null)
		{
			tutorialUi.HideHand();
		}
	}

	public void ShowHandGameObject(bool show)
	{
		if (tutorialUi != null)
		{
			tutorialUi.ShowHandGameObject(show);
		}
	}

	public void ShowArrow(GameObject parent)
	{
		if (tutorialUi == null)
		{
			tutorialUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial) as TutorialUi;
			tutorialUi.Open();
		}
		tutorialUi.ShowArrow(parent);
	}

	public void HideArrow()
	{
		if (tutorialUi != null)
		{
			tutorialUi.HideArrow();
		}
	}

	private bool ShowArrow(string id)
	{
		TutorialArrowParent tutorialArrowParent = GetTutorialArrowParent(id);
		if (tutorialArrowParent != null)
		{
			tutorialUi.ShowArrow(tutorialArrowParent.gameObject, tutorialArrowParent.downwards);
		}
		return tutorialArrowParent != null;
	}

	private void Pan(BuildingModel targetBuilding, float distance, float time)
	{
		Vector3 worldPosition = campView.TransformGridToWorldPosition(targetBuilding.GridPosition);
		if (distance == -1f)
		{
			distance = campView.CameraController.Distance;
		}
		Pan(worldPosition, distance, time);
	}

	private void Pan(Vector3 worldPosition, float distance, float time)
	{
		if (distance <= 0f)
		{
			distance = campView.CameraController.Distance;
		}
		campView.CameraController.StartPan(worldPosition, distance, time);
	}

	public void Say(string character, string textId, Callback callback = null)
	{
		StartCoroutine(SayCoroutine(character, textId, callback));
	}

	private IEnumerator SayCoroutine(string character, string textId, Callback callback = null)
	{
		EnableCampControls(enable: false);
		EnableMapControls(enable: false);
		yield return null;
		yield return tutorialUi.Say(character, textId);
		tutorialUi.HideCharacter();
		EnableCampControls(enable: true);
		EnableMapControls(enable: true);
		callback?.Invoke();
	}

	public void StartNextTutorial()
	{
		if (Running || GameManager.Instance == null)
		{
			return;
		}
		bool flag = false;
		if (GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.CurrentQuest is MissionQuest missionQuest)
		{
			MapMissionGroupModel unlockedEpisode = missionQuest.GetUnlockedEpisode();
			if (unlockedEpisode != null)
			{
				for (int i = 0; i < unlockedEpisode.Missions.Count; i++)
				{
					if (unlockedEpisode.Missions[i].IsCompleted && unlockedEpisode.Missions[i].MissionData != null && unlockedEpisode.Missions[i].MissionData.DisplayTextID == "S01E01M05CandleRoom")
					{
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			flag = Instance.StartPart("WalkerTapping");
		}
		else
		{
			if (flag || Instance.StartPart("Tutorial_Training_Ground") || Instance.StartPart("Phone") || Instance.StartPart("EndTutorial"))
			{
				return;
			}
			if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("HeroUnlock"))
			{
				string heroId = SurvivorToken.GetHeroId(CurrencyType.DarylToken);
				SurvivorModel heroById = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(heroId);
				Cashier heroUnlockCashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(CurrencyType.DarylToken);
				if (GameManager.Instance.playerModel.GetCurrency(CurrencyType.DarylToken).Value >= heroUnlockCashier.GetTotalCost(CurrencyType.DarylToken) && heroById == null)
				{
					Instance.StartPart("HeroUnlock");
				}
			}
			else if (SingularityMonoBehaviour<HUDManager>.Instance.OpenPopups.Count == 0 && CanPlayPromoteDarylTutorial())
			{
				Instance.StartPart("HeroPromote");
			}
		}
	}

	public void UpdateSuggestion()
	{
		if (Running && IsSuggesting)
		{
			ShowSuggestions();
		}
	}

	private void ClearSuggestions()
	{
		if (suggestions != null)
		{
			suggestions.Clear();
		}
	}

	private void HideSuggestion()
	{
		if (suggestedGameObjects == null)
		{
			return;
		}
		for (int i = 0; i < suggestedGameObjects.Count; i++)
		{
			if (suggestedGameObjects[i] != null)
			{
				suggestedGameObjects[i].SetActive(value: false);
			}
		}
		suggestedGameObjects.Clear();
	}

	public void ShowSuggestions()
	{
		HideSuggestion();
		if (suggestions == null)
		{
			return;
		}
		if (suggestedGameObjects == null)
		{
			suggestedGameObjects = new List<GameObject>();
		}
		for (int i = 0; i < suggestions.Count; i++)
		{
			Transform transform = null;
			if (!suggestions[i].StartsWith("Episode"))
			{
				UIButton button = GetButton(suggestions[i]);
				if (button != null)
				{
					transform = button.transform.Find("TutorialSuggest");
				}
				else
				{
					UIType uIType = UIType.None;
					try
					{
						uIType = (UIType)Enum.Parse(typeof(UIType), suggestions[i]);
					}
					catch (Exception)
					{
					}
					if (uIType != UIType.None)
					{
						HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(uIType, null, createIfNotExist: false);
						if (hUDElement != null)
						{
							transform = hUDElement.transform.Find("TutorialSuggest");
						}
					}
				}
			}
			if (transform != null)
			{
				transform.gameObject.SetActive(value: true);
				suggestedGameObjects.Add(transform.gameObject);
			}
		}
	}

	public void ShowButtonSuggest(string buttonId, bool show)
	{
		UIButton button = GetButton(buttonId);
		if (button != null)
		{
			Transform transform = button.transform.Find("TutorialSuggest");
			if (transform != null)
			{
				transform.gameObject.SetActive(show);
			}
		}
	}

	private bool CanPlayPromoteDarylTutorial()
	{
		List<SurvivorModel> promotableSurvivors = GameManager.Instance.playerModel.SurvivorContainer.GetPromotableSurvivors(herosOnly: true);
		if (promotableSurvivors.Count > 0)
		{
			string heroId = SurvivorToken.GetHeroId(CurrencyType.DarylToken);
			SurvivorModel heroById = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(heroId);
			for (int i = 0; i < promotableSurvivors.Count; i++)
			{
				if (promotableSurvivors[i] == heroById)
				{
					return true;
				}
			}
		}
		return false;
	}
}
