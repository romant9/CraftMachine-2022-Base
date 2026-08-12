using System.Collections.Generic;
using TWDModel;

public class EndlessModePostMissionTutorial : IHardCodedTutorial
{
	private static bool HubReturnTutorialCompleted => GameManager.Instance.playerModel.Blackboard.IsToggleOn("TootleEndlessModeFTUEHubReturnTutorial");

	public static bool CanStartTutorial()
	{
		if (!HubReturnTutorialCompleted)
		{
			return GameManager.Instance.playerModel.EndlessModeManager.EndlessAttemptData.Count > 0;
		}
		return false;
	}

	public EndlessModePostMissionTutorial()
	{
		RegisterListeners();
	}

	public void RegisterListeners()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.IAPConfirmPopupNew);
		if (noCreation != null)
		{
			noCreation.OnClose += OnPopupClosed;
		}
		else
		{
			StartTutorial();
		}
	}

	private void OnPopupClosed(HUDElement element, HUDElementConfig hudElementConfig)
	{
		StartTutorial();
	}

	public void DeregisterListeners()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.IAPConfirmPopupNew);
		if (noCreation != null)
		{
			noCreation.OnClose -= OnPopupClosed;
		}
	}

	public void StartTutorial()
	{
		DeregisterListeners();
		HardCodedTutorialData hardCodedTutorialData = new HardCodedTutorialData
		{
			PortraitId = "Portrait_Info",
			Localizations = new List<string> { "Endless.FTUE.HubReturn.1", "Endless.FTUE.HubReturn.2{Parameter}", "Endless.FTUE.HubReturn.3" },
			LocalizationArguments = new List<object>
			{
				null,
				GameManager.Instance.gameEconomyData.EndlessModeConfig.AttemptsToSumForFinalScoreNormal,
				null
			}
		};
		TutorialView.Instance.ShowDialogWithHighlightedObjects(hardCodedTutorialData, TutorialView.Instance.StartNextTutorial);
		Helpers.ExecuteCommandDelayed(new SetBlackboardToggleCommand("TootleEndlessModeFTUEHubReturnTutorial"));
	}
}
