using System.Collections.Generic;
using System.Text;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TutorialModel : TWDModelObject
	{
		public const string NextStepEvent = "NextStepEvent";

		public const string NoExtraKeyPurchase = "S01E01M03OutOfTheWoods/S01E01M02CheckTheCamp/S01E01M04AlongTheTracks";

		public const string LastMainTutorialId = "RewardsScreen3";

		public const string WalkerTappingTutorial = "WalkerTapping";

		public const string RewardScreen2 = "RewardsScreen2";

		public const string TeachRewardScreenBuyMore = "RewardsScreen3";

		public const string TrainingGroundTutorial = "Tutorial_Training_Ground";

		public const int FirstSurvivorTrainedStageId = 6;

		public const string FirstCampTutorial = "Tutorial";

		public const string PhoneTutorial = "Phone";

		public const string TeachRewardScreenBuyMoreMission = "S01E01M04AlongTheTracks";

		public const string BruiserTutorialMission = "S01E01M04AlongTheTracks";

		public const string HeroUnlock = "HeroUnlock";

		public const string HeroTrait = "HeroTrait";

		public const string TeachHeroTraitMission = "E02M03";

		public const string HeroPromote = "HeroPromote";

		public const string ScavengeMode = "ScavengeMode";

		public const string SeasonsMode = "SeasonsMode";

		public const string ChallengeMode = "ChallengeMode";

		public const string SurvivalMode = "SurvivalMode";

		public const string SurvivalHardMode = "SurvivalHardMode";

		public const string SurvivalNightmareMode = "SurvivalNightmareMode";

		public const string EndlessMode = "EndlessMode";

		public const string EndlessExpertMode = "EndlessExpertMode";

		public const string GuildBattleMode = "GuildBattleMode";

		public const string OutpostMode = "OutpostMode";

		public string CurrentPartId { get; set; }

		public int CurrentStep { get; set; }

		public bool ShowSuppliesHud { get; set; }

		public bool ShowGasHud { get; set; }

		public bool ShowDiamondsHud { get; set; }

		public bool ShowDailyQuestHud { get; set; }

		[JsonIgnore]
		public bool StaticTutorialComplete => HasCompletedPart("RewardsScreen3");

		[JsonIgnore]
		public bool Completed => CurrentPartDefinition == null;

		[JsonIgnore]
		public int GetNumberParts => base.manager.GameEconomyData.Tutorial.Parts.Count;

		[JsonIgnore]
		public int GetNumberSteps
		{
			get
			{
				if (CurrentPartDefinition == null)
				{
					return 0;
				}
				return CurrentPartDefinition.Steps.Count;
			}
		}

		[JsonIgnore]
		public TutorialStepDefinition GetCurrentStepDefinition
		{
			get
			{
				if (CurrentPartDefinition == null)
				{
					return null;
				}
				return CurrentPartDefinition.Steps[CurrentStep];
			}
		}

		[JsonIgnore]
		public List<string> GetCurrentActions
		{
			get
			{
				if (CurrentPartDefinition == null)
				{
					return null;
				}
				return CurrentPartDefinition.Steps[CurrentStep].Actions;
			}
		}

		public List<string> completedParts { get; set; }

		[JsonIgnore]
		public TutorialPartDefinition CurrentPartDefinition => GetPartDefinition(CurrentPartId);

		public override void Start()
		{
			base.Start();
			if (CurrentPartId != null)
			{
				RegisterListeners();
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			completedParts = new List<string>();
		}

		public override bool IsValid()
		{
			return CurrentStep >= -1;
		}

		public void SetPart(string partId)
		{
			if (CurrentPartId != partId)
			{
				CurrentPartId = partId;
				if (CurrentPartDefinition != null)
				{
					SetStep(0);
				}
				RegisterListeners();
				NewPartStarted();
			}
		}

		private void NewPartStarted()
		{
			if (!(CurrentPartId == "WalkerTapping"))
			{
				return;
			}
			CampDefenseModel campDefenseModel = base.manager.CampModel.CampDefenseModel;
			if (campDefenseModel != null && campDefenseModel.Walkers.Count == 0 && CurrentStep == 0)
			{
				for (int i = 0; i < 4; i++)
				{
					campDefenseModel.CreateWalker();
				}
			}
		}

		public void RecordTdEvent()
		{
			base.manager.TdMetrics.SetEventType("tutorial").AddProperty("part_id", CurrentPartId).AddProperty("step", CurrentStep.ToString())
				.Send();
		}

		public void NextStep()
		{
			SetStep(CurrentStep + 1);
		}

		private void SetStep(int step)
		{
			if (step < 0 || step >= GetNumberSteps)
			{
				CurrentStep = -1;
			}
			else
			{
				CurrentStep = step;
			}
			if (CurrentStep == -1)
			{
				base.manager.Metrics.AddEnd().AddTutorial(step - 1).Send();
				SetCompleted(step);
			}
			if (CurrentPartId != null && base.manager.Player != null)
			{
				base.manager.Metrics.AddStart().AddTutorial().Send();
			}
		}

		public bool HasCompletedPart(string id)
		{
			return completedParts.Contains(id);
		}

		public bool PartExists(string id)
		{
			return GetPartDefinition(id) != null;
		}

		public void SetPartCompleted(string id)
		{
			if (!completedParts.Contains(id))
			{
				completedParts.Add(id);
			}
			if (id == "InitialCombat")
			{
				base.manager.Player.CombatTutorialCompleted = true;
			}
			CurrentPartId = null;
		}

		public void SetAllPartsCompleted()
		{
			foreach (TutorialPartDefinition part in base.manager.GameEconomyData.Tutorial.Parts)
			{
				if (!completedParts.Contains(part.Id) && part.Id != "PvPTutorial" && part.Id != "OutpostUnlock" && part.Id != "OutpostEditUnlocked" && part.Id != "OutpostEditExplain")
				{
					completedParts.Add(part.Id);
				}
			}
			base.manager.Player.CombatTutorialCompleted = true;
			CurrentPartId = null;
			ShowDiamondsHud = true;
			ShowGasHud = true;
			ShowSuppliesHud = true;
			ShowDailyQuestHud = true;
		}

		private void SetCompleted(int step)
		{
			TutorialPartDefinition currentPartDefinition = CurrentPartDefinition;
			if (currentPartDefinition == null)
			{
				StringBuilder stringBuilder = new StringBuilder("Tutorial:SetCompleted. currentPartDefinition is null");
				if (base.manager.Player != null)
				{
					stringBuilder.Append(" player level " + base.manager.Player.Level);
				}
				stringBuilder.Append(" completed ");
				if (completedParts != null)
				{
					foreach (string completedPart in completedParts)
					{
						stringBuilder.Append(completedPart + ",");
					}
				}
				stringBuilder.Append(" step that was set " + step);
				base.manager.Debug.LogWarning(stringBuilder.ToString());
				CurrentPartId = null;
			}
			else
			{
				if (!completedParts.Contains(currentPartDefinition.Id))
				{
					completedParts.Add(currentPartDefinition.Id);
				}
				if (CurrentPartId == "InitialCombat")
				{
					base.manager.Player.CombatTutorialCompleted = true;
				}
				CurrentPartId = null;
			}
		}

		private TutorialPartDefinition GetPartDefinition(string partId)
		{
			foreach (TutorialPartDefinition part in base.manager.GameEconomyData.Tutorial.Parts)
			{
				if (part.Id == partId)
				{
					return part;
				}
			}
			return null;
		}

		public bool MissionHasFakeMissionRewards()
		{
			MapMissionModel attackTargetMissionModel = base.manager.Player.MapContainerModel.AttackTargetMissionModel;
			if (!StaticTutorialComplete && attackTargetMissionModel != null)
			{
				return "S01E01M03OutOfTheWoods/S01E01M02CheckTheCamp/S01E01M04AlongTheTracks".Contains(attackTargetMissionModel.MissionData.DisplayTextID);
			}
			return false;
		}

		private void RegisterListeners()
		{
			CampModel campModel = base.manager.CampModel;
			if (campModel != null)
			{
				campModel.Changed -= OnCampChanged;
				campModel.Changed += OnCampChanged;
				if (campModel.CampDefenseModel != null)
				{
					campModel.CampDefenseModel.Changed -= OnCampDefenseChanged;
					campModel.CampDefenseModel.Changed += OnCampDefenseChanged;
				}
				else
				{
					base.Debug.LogWarning("Tutorial:RegisterListeners. CampDefenseModel is null");
				}
			}
			else
			{
				base.Debug.LogWarning("Tutorial:RegisterListeners. CampModel is null");
			}
			SurvivorContainerModel survivorContainer = base.manager.Player.SurvivorContainer;
			if (survivorContainer != null)
			{
				survivorContainer.Changed -= OnSurvivorContainerChanged;
				survivorContainer.Changed += OnSurvivorContainerChanged;
				StoryTellerModel storyTeller = survivorContainer.StoryTeller;
				if (storyTeller != null)
				{
					storyTeller.Changed -= OnStoryTellerChanged;
					storyTeller.Changed += OnStoryTellerChanged;
				}
				else
				{
					base.Debug.LogWarning("Tutorial:RegisterListeners. StoryTellerModel is null");
				}
				if (survivorContainer.Survivors != null)
				{
					foreach (SurvivorModel survivor in survivorContainer.Survivors)
					{
						survivor.Changed -= OnSurvivorChanged;
						survivor.Changed += OnSurvivorChanged;
					}
				}
				else
				{
					base.Debug.LogWarning("Tutorial:RegisterListeners. SurvivorContainerModel.Survivors is null");
				}
			}
			else
			{
				base.Debug.LogWarning("Tutorial:RegisterListeners. SurvivorContainerModel is null");
			}
			PhoneCallModel phoneCall = base.manager.Player.PhoneCall;
			if (phoneCall != null)
			{
				phoneCall.Changed -= OnPhoneCallChanged;
				phoneCall.Changed += OnPhoneCallChanged;
			}
			else
			{
				base.Debug.LogWarning("Tutorial:RegisterListeners. phoneCallModel is null");
			}
		}

		private void OnPhoneCallChanged(ModelObject model, string changed, object args)
		{
		}

		private void OnCampDefenseChanged(ModelObject model, string changed, object args)
		{
			if (changed == "CampDefenseWalkerKilled")
			{
				NextStepIfIncludeAction("DefenseWalkerTap");
			}
		}

		private void OnSurvivorContainerChanged(ModelObject model, string changed, object args)
		{
			if (changed == "addSurvivor")
			{
				NextStepIfIncludeAction("AcceptSurvivor");
			}
		}

		private void OnStoryTellerChanged(ModelObject model, string changed, object args)
		{
			if (changed == "QuestAccepted")
			{
				NextStepIfIncludeAction("StartQuest");
			}
		}

		private void OnCampChanged(ModelObject model, string changed, object args)
		{
			switch (changed)
			{
			case "EventAddBuilding":
				NextStepIfIncludeAction("Build");
				break;
			case "EventUpgradeBuilding":
				NextStepIfIncludeAction("Upgrade");
				break;
			case "EventLevelUpBuilding":
				NextStepIfIncludeAction("SpeedUp");
				break;
			case "EventBuildingCollected":
				NextStepIfIncludeAction("CollectSupplies");
				break;
			}
		}

		private void OnSurvivorChanged(ModelObject modelObject, string changed, object args)
		{
			if (changed == "ActionStartEvent")
			{
				NextStepIfIncludeAction("UpgradeSurvivor");
			}
			else if (changed == "ActionFinishedEvent")
			{
				NextStepIfIncludeAction("SpeedUp");
			}
		}

		private void NextStepIfIncludeAction(string action)
		{
			if (CurrentPartId == null)
			{
				return;
			}
			List<string> getCurrentActions = GetCurrentActions;
			for (int i = 0; i < getCurrentActions.Count; i++)
			{
				string[] array = getCurrentActions[i].Split(',');
				if (array != null && array.Length != 0 && array[0] == action)
				{
					NextStep();
					NotifyChange("NextStepEvent");
					break;
				}
			}
		}
	}
}
