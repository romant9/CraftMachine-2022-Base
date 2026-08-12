using System;
using TWDModel;
using UnityEngine;

public class WeeklySurvivalEndPopup : HUDElement
{
	[SerializeField]
	private UILabel missionsCompletedLabel;

	[SerializeField]
	private UILabel survivorsRemainingLabel;

	[SerializeField]
	private UILabel loseTitleLabel;

	[SerializeField]
	private UILabel difficultyLabel;

	[SerializeField]
	private GameObject timerBGNormalContainer;

	[SerializeField]
	private GameObject timerBGHardContainer;

	[SerializeField]
	private GameObject timerBGNightmareContainer;

	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private UIButton survivalRestartButton;

	[SerializeField]
	private NUICountdownTimer roundTimer;

	[SerializeField]
	private GameObject completionContainer;

	[SerializeField]
	private GameObject loseContainer;

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	private void UpdateNextSurvivalTimer(WeeklySurvivalModel currentSurvivalModel)
	{
		if (currentSurvivalModel != null && !(roundTimer == null))
		{
			WeeklySurvival nextWeeklySurvival = currentSurvivalModel.NextWeeklySurvival;
			if (nextWeeklySurvival != null)
			{
				roundTimer.SetCurrentMilliseconds(nextWeeklySurvival.StartTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp);
			}
			else
			{
				roundTimer.SetCurrentMilliseconds(8639999000L);
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WeeklySurvivalModel weeklySurvivalModel = GetModel<WeeklySurvivalModel>();
		if (weeklySurvivalModel != null && weeklySurvivalModel.CurrentDefinition != null)
		{
			SurvivalCharacterContainerModel survivalCharacters = GameManager.Instance.playerModel.SurvivorContainer.SurvivalCharacters;
			HelpersUI.SetContentToLabel(survivorsRemainingLabel, survivalCharacters.GetNumSurvivorsAvailableForAction() + "/" + survivalCharacters.SurvivalModeSurvivors.Count);
			HelpersUI.SetContentToLabel(missionsCompletedLabel, weeklySurvivalModel.NumberCompleted + "/" + weeklySurvivalModel.CurrentDefinition.TotalMissionCount);
			SurvivalDifficulty currentDifficulty = weeklySurvivalModel.CurrentDifficulty;
			HelpersUI.SetContentToLabel(difficultyLabel, LocalizationManager.GetText("Survival.Difficulty." + Enum.GetName(typeof(SurvivalDifficulty), currentDifficulty)));
			Helpers.GameObjectSetActive(timerBGNormalContainer, currentDifficulty <= SurvivalDifficulty.Normal);
			Helpers.GameObjectSetActive(timerBGHardContainer, currentDifficulty == SurvivalDifficulty.Hard);
			Helpers.GameObjectSetActive(timerBGNightmareContainer, currentDifficulty == SurvivalDifficulty.Nightmare);
			bool isCompleted = weeklySurvivalModel.IsCompleted;
			bool flag = survivalCharacters.GetNumSurvivorsAvailableForAction() == 0;
			Helpers.GameObjectSetActive(completionContainer, isCompleted);
			Helpers.GameObjectSetActive(loseContainer, !isCompleted);
			Helpers.GameObjectSetActive(survivalRestartButton, !weeklySurvivalModel.Finished && !isCompleted && flag && weeklySurvivalModel.CanRestartMapOrDoubleRewards());
			if (!isCompleted)
			{
				if (flag)
				{
					HelpersUI.SetContentToLabel(loseTitleLabel, LocalizationManager.GetText("Popup.SurvivalEnded.Title.OutOfSurvivors"));
				}
				else
				{
					HelpersUI.SetContentToLabel(loseTitleLabel, LocalizationManager.GetText("Popup.SurvivalEnded.Title.OutOfTime"));
				}
			}
			UpdateNextSurvivalTimer(weeklySurvivalModel);
		}
		else
		{
			HelpersUI.SetContentToLabel(loseTitleLabel, LocalizationManager.GetText("Generic.ComingSoon"));
			Helpers.GameObjectSetActive(survivalRestartButton, value: false);
			UpdateNextSurvivalTimer(weeklySurvivalModel);
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateNextSurvivalTimer(GameManager.Instance.playerModel.WeeklySurvival);
	}

	public override void Close()
	{
		base.Close();
	}

	public override void OnBackButtonClicked()
	{
		OnClickClose();
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp, null, createIfNotExist: false);
		if (hUDElement != null)
		{
			hUDElement.OnClickClose();
		}
	}

	public static void OpenWithModel(WeeklySurvivalModel model)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklySurvivalEnd).OpenForModel(model);
	}

	public void OnClickSurvivalRestart()
	{
		ResetSurvivalMapCommand resetSurvivalMapCommand = new ResetSurvivalMapCommand();
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null)
		{
			resetSurvivalMapCommand.Cashier = weeklySurvivalModel.GetRestartCashier();
			ConsumeCurrencyCommandUtils.Execute(resetSurvivalMapCommand, OnRestartCompleted);
		}
	}

	private void OnRestartCompleted(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			DetailMapPopUp.ReloadSurvivalMap();
		}
	}
}
