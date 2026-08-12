using TWDModel;
using UnityEngine;

public class SurvivalInfoPanel : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject TitlesAndTextParent;

	[SerializeField]
	private UIPlayerNameLabel playerName;

	[SerializeField]
	private GameObject survivalCountdownParent;

	[SerializeField]
	private NUICountdownTimer survivalCountdownTimer;

	[Header("Progress Content")]
	[SerializeField]
	private UILabel playerCompletionCountLabel;

	[Header("Title Parts")]
	[SerializeField]
	private GameObject titleParent;

	[SerializeField]
	private UIButtonWithLabelAndIcon survivalNameButton;

	[SerializeField]
	private GameObject missionCostContainer;

	[SerializeField]
	private UISprite missionCostSprite;

	[SerializeField]
	private UILabel missionCostLabel;

	[SerializeField]
	private UILabel survivalTimeLeftLabel;

	[SerializeField]
	private UILabel survivalRestartCostLabel;

	[SerializeField]
	private UILabel missionProgressLabel;

	[SerializeField]
	private UILabel survivorsLeftLabel;

	[SerializeField]
	private UIButtonExtended survivalRestartButton;

	[SerializeField]
	private UIButtonExtended survivalBoosterButton;

	[SerializeField]
	private GameObject noResetNoBoosterState;

	[SerializeField]
	private GameObject boosterActivatedState;

	[SerializeField]
	private GameObject distanceResetState;

	[SerializeField]
	private WeeklySurvival nextSurvival;

	private long survivalTime;

	private void Awake()
	{
		DebugIdString = "SurvivalInfoPanel";
	}

	private void OnEnable()
	{
		UpdateUI();
		Helpers.GameObjectSetActive(survivalCountdownTimer, value: false);
		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.WeeklySurvivalEnd);
	}

	private void Update()
	{
		if (survivalCountdownTimer != null && nextSurvival != null)
		{
			survivalTime = nextSurvival.StartTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
			survivalCountdownTimer.SetCurrentMilliseconds(survivalTime);
			Helpers.GameObjectSetActive(survivalCountdownTimer, value: true);
			if (survivalTime < 0)
			{
				nextSurvival = null;
				DetailMapPopUp.ReloadSurvivalMap();
			}
		}
		UpdateTimeleftUI(calledFromUpdateUI: false);
		WeeklySurvivalHelper.GetWeeklySurvivalModel();
	}

	public virtual void UpdateUI()
	{
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null && WeeklySurvivalHelper.IsSurvivalOngoing())
		{
			if (playerName != null)
			{
				playerName.UpdateUI();
			}
			Helpers.GameObjectSetActive(survivalCountdownTimer, value: false);
		}
		else
		{
			nextSurvival = WeeklySurvivalHelper.GetNextSurvival();
			Helpers.GameObjectSetActive(playerName, value: false);
			Helpers.GameObjectSetActive(survivalCountdownTimer, value: true);
		}
		if (weeklySurvivalModel != null && (weeklySurvivalModel.Finished || weeklySurvivalModel.IsOutOfSurvivors || weeklySurvivalModel.IsCompleted))
		{
			if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.WeeklySurvivalEnd) && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.MissionBriefing) && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.MapTeamSelection))
			{
				WeeklySurvivalEndPopup.OpenWithModel(weeklySurvivalModel);
			}
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.WeeklySurvivalEnd);
			if (weeklySurvivalModel != null)
			{
				if (!weeklySurvivalModel.IsDifficultySelected)
				{
					WeeklySurvivalDifficultyPopup.OpenWithModel(weeklySurvivalModel);
				}
				else
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.WeeklySurvivalDifficulty);
				}
			}
		}
		int survivalRestartCost = GameManager.Instance.gameEconomyData.ConfigData.SurvivalRestartCost;
		HelpersUI.SetContentToLabel(survivalRestartCostLabel, survivalRestartCost.ToString());
		UpdateProgressLabels();
		UpdateTitleParts();
		UpdateTimeleftUI(calledFromUpdateUI: true);
		UpdateBoosterState();
	}

	public void UpdateProgressLabels()
	{
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null && weeklySurvivalModel.CurrentDefinition != null)
		{
			HelpersUI.SetContentToLabel(missionProgressLabel, weeklySurvivalModel.NumberCompleted + "/" + weeklySurvivalModel.CurrentDefinition.TotalMissionCount);
			SurvivalCharacterContainerModel survivalCharacters = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters;
			HelpersUI.SetContentToLabel(survivorsLeftLabel, survivalCharacters.GetNumSurvivorsAvailableForAction() + "/" + survivalCharacters.SurvivalModeSurvivors.Count);
		}
	}

	public void UpdateTitleParts()
	{
		bool flag = WeeklySurvivalHelper.IsSurvivalOngoing();
		Helpers.GameObjectSetActive(titleParent, value: true);
		if (survivalNameButton != null)
		{
			if (flag)
			{
				survivalNameButton.SetContentToLabelOne(WeeklySurvivalHelper.GetCurrentSurvivalName());
			}
			else
			{
				survivalNameButton.SetContentToLabelOne(WeeklySurvivalHelper.GetNextSurvivalName());
			}
		}
		if (flag)
		{
			Helpers.GameObjectSetActive(missionCostContainer, value: true);
			HelpersUI.SetContentToLabel(missionCostLabel, WeeklySurvivalHelper.GetGasCost().ToString());
			HelpersUI.SetSprite(missionCostSprite, HelpersGfx.GetCurrencyIconName(CurrencyType.ReplayToken));
		}
		else
		{
			Helpers.GameObjectSetActive(missionCostContainer, value: false);
		}
	}

	public void AddListeners()
	{
		if ((bool)survivalNameButton)
		{
			survivalNameButton.SetClickCallback(OnClickSurvivalName);
		}
		if ((bool)survivalRestartButton)
		{
			survivalRestartButton.SetClickCallback(OnClickSurvivalRestart);
		}
		if ((bool)survivalBoosterButton)
		{
			survivalBoosterButton.SetClickCallback(OnClickRewardBooster);
		}
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void RemoveListeners()
	{
		if ((bool)survivalNameButton)
		{
			survivalNameButton.Clear();
		}
		if ((bool)survivalRestartButton)
		{
			survivalRestartButton.Clear();
		}
		if ((bool)survivalBoosterButton)
		{
			survivalBoosterButton.Clear();
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void UpdateTimeleftUI(bool calledFromUpdateUI)
	{
		if (WeeklySurvivalHelper.IsSurvivalOngoing())
		{
			string text = LocalizationManager.GetText("Map.WeeklySurvival.EndsIn{Time}", WeeklySurvivalHelper.GetFormatedTimeLeftToCurrentSurvivalEnd());
			HelpersUI.SetContentToLabel(survivalTimeLeftLabel, text);
			return;
		}
		Helpers.GameObjectSetActive(survivalTimeLeftLabel, value: false);
		if (!calledFromUpdateUI && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.WeeklySurvivalEnd))
		{
			UpdateUI();
		}
	}

	private void UpdateBoosterState()
	{
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null)
		{
			bool active = weeklySurvivalModel.CanRestartMapOrDoubleRewards() && !weeklySurvivalModel.Finished && weeklySurvivalModel.IsDifficultySelected;
			noResetNoBoosterState.SetActive(active);
			boosterActivatedState.SetActive(weeklySurvivalModel.DoubleRewardsEnabled);
			distanceResetState.SetActive(weeklySurvivalModel.CurrentMapRestarts > 0);
		}
		else
		{
			noResetNoBoosterState.SetActive(value: false);
			distanceResetState.SetActive(value: false);
			boosterActivatedState.SetActive(value: false);
		}
	}

	private void OnClickSurvivalName(UIButtonExtended button)
	{
		if (WeeklySurvivalHelper.GetWeeklySurvivalModel() != null)
		{
			WeeklySurvivalInfoPopup.TryOpenFromClick();
		}
	}

	private void OnClickSurvivalRestart(UIButtonExtended button)
	{
		ResetSurvivalMapCommand resetSurvivalMapCommand = new ResetSurvivalMapCommand();
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null)
		{
			resetSurvivalMapCommand.Cashier = weeklySurvivalModel.GetRestartCashier();
			ConsumeCurrencyCommandUtils.Execute(resetSurvivalMapCommand, OnRestartCompleted);
		}
	}

	public void OnClickRewardBooster(UIButtonExtended button)
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalRewardBoosterPopup);
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
	}

	private void OnRestartCompleted(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			DetailMapPopUp.ReloadSurvivalMap();
			UpdateUI();
		}
	}

	private void OnUIEvent(string type, object parameter = null)
	{
		if (type == "OnPopUpClose" && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp).IsOpen && !SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp).IsClosing)
		{
			UpdateUI();
		}
		else if (type == "SocialGuildPlayerChanged" || type == "SocialMembershipAccepted")
		{
			UpdateUI();
		}
	}
}
