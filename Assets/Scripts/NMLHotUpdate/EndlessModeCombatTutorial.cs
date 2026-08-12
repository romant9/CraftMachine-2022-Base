using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class EndlessModeCombatTutorial : IHardCodedTutorial
{
	private bool tutorialIsRunning;

	private static bool FirstBloodTutorialCompleted => GameManager.Instance.playerModel.Blackboard.IsToggleOn("ToggleEndlessModeFTUEFirstBloodTutorial");

	private static bool SpecialWalkerTutorialCompleted => GameManager.Instance.playerModel.Blackboard.IsToggleOn("ToggleEndlessModeFTUESpecialWalkerTutorial");

	private static bool WavesTutorialCompleted => GameManager.Instance.playerModel.Blackboard.IsToggleOn("ToggleEndlessModeFTUEWavesTutorial");

	public static bool CanStartTutorial()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null && combat.IsEndlessBattleMission)
		{
			if (FirstBloodTutorialCompleted && SpecialWalkerTutorialCompleted)
			{
				return !WavesTutorialCompleted;
			}
			return true;
		}
		return false;
	}

	public EndlessModeCombatTutorial()
	{
		StartTutorial();
	}

	public void StartTutorial()
	{
		RegisterListeners();
	}

	public void RegisterListeners()
	{
		GameManager.Instance.playerModel.Combat.Changed += OnCombatModelChanged;
	}

	public void DeregisterListeners()
	{
		GameManager.Instance.playerModel.Combat.Changed -= OnCombatModelChanged;
	}

	private void OnCombatModelChanged(ModelObject model, string changed, object args)
	{
		if (tutorialIsRunning || ((CombatModel)model).MissionCompleted)
		{
			return;
		}
		if (changed == "actorKilled" && args is WalkerModel walkerModel)
		{
			if (!FirstBloodTutorialCompleted)
			{
				tutorialIsRunning = true;
				DelayedNotificationVisualizationTask task = new DelayedNotificationVisualizationTask(walkerModel, delegate
				{
					if (!((CombatModel)model).MissionCompleted)
					{
						List<string> uIElementsToHighlight = new List<string> { "CombatHUD_EndlessMode_Container/Contents/Multiplier_Container/Multiplier_Number" };
						HardCodedTutorialData hardCodedTutorialData = new HardCodedTutorialData
						{
							PortraitId = "Portrait_Info",
							Localizations = new List<string> { "Endless.FTUE.FirstBlood.1", "Endless.FTUE.FirstBlood.2" },
							UIElementsToHighlight = uIElementsToHighlight
						};
						TutorialView.Instance.ShowDialogWithHighlightedObjects(hardCodedTutorialData, delegate
						{
							TutorialCompleted("ToggleEndlessModeFTUEFirstBloodTutorial");
						});
					}
				});
				VisualizationQueue.Instance.Add(task);
			}
			else if (!SpecialWalkerTutorialCompleted && walkerModel.Definition.IsSpecial)
			{
				tutorialIsRunning = true;
				DelayedNotificationVisualizationTask task2 = new DelayedNotificationVisualizationTask(walkerModel, delegate
				{
					if (!((CombatModel)model).MissionCompleted)
					{
						HardCodedTutorialData hardCodedTutorialData = new HardCodedTutorialData
						{
							PortraitId = "Portrait_Info",
							Localizations = new List<string> { "Endless.FTUE.SpecialWalker" }
						};
						TutorialView.Instance.ShowDialogWithHighlightedObjects(hardCodedTutorialData, delegate
						{
							TutorialCompleted("ToggleEndlessModeFTUESpecialWalkerTutorial");
						});
					}
				});
				VisualizationQueue.Instance.Add(task2);
			}
			else
			{
				if (WavesTutorialCompleted || ((CombatModel)model).Walkers.Count != 1 || !((CombatModel)model).Walkers.Contains(walkerModel))
				{
					return;
				}
				tutorialIsRunning = true;
				DelayedNotificationVisualizationTask task3 = new DelayedNotificationVisualizationTask(walkerModel, delegate
				{
					if (!((CombatModel)model).MissionCompleted)
					{
						List<string> uIElementsToHighlight = new List<string> { "Turn_Indicator_EndlessMode(Clone)/Container/Turn_Counter-Turn_Indicator_EndlessMode(Clone)/Container/Threat_Icons_Container-Turn_Indicator_EndlessMode(Clone)/Container/Bg_Middle-Turn_Indicator_EndlessMode(Clone)/Container/Wave_Container" };
						HardCodedTutorialData hardCodedTutorialData = new HardCodedTutorialData
						{
							PortraitId = "Portrait_Info",
							Localizations = new List<string> { "Endless.FTUE.Waves.1", "Endless.FTUE.Waves.2" },
							UIElementsToHighlight = uIElementsToHighlight,
							ShowDialogOnCenter = true
						};
						TutorialView.Instance.ShowDialogWithHighlightedObjects(hardCodedTutorialData, delegate
						{
							TutorialCompleted("ToggleEndlessModeFTUEWavesTutorial");
						});
					}
				});
				VisualizationQueue.Instance.Add(task3);
			}
		}
		else
		{
			if (!(changed == "turnEnded") || WavesTutorialCompleted)
			{
				return;
			}
			EndlessModeCombatModel endlessModeCombatModel = ((CombatModel)model).EndlessModeCombatModel;
			if (endlessModeCombatModel.CurrentWaveIndex <= 0 || endlessModeCombatModel.CurrentTurnCount != endlessModeCombatModel.CurrentWaveDuration - 1)
			{
				return;
			}
			tutorialIsRunning = true;
			DelayedNotificationVisualizationTask task4 = new DelayedNotificationVisualizationTask(null, delegate
			{
				if (!((CombatModel)model).MissionCompleted)
				{
					List<string> uIElementsToHighlight = new List<string> { "Turn_Indicator_EndlessMode(Clone)/Container/Turn_Counter-Turn_Indicator_EndlessMode(Clone)/Container/Threat_Icons_Container-Turn_Indicator_EndlessMode(Clone)/Container/Bg_Middle-Turn_Indicator_EndlessMode(Clone)/Container/Wave_Container" };
					HardCodedTutorialData hardCodedTutorialData = new HardCodedTutorialData
					{
						PortraitId = "Portrait_Info",
						Localizations = new List<string> { "Endless.FTUE.Waves.1", "Endless.FTUE.Waves.2" },
						UIElementsToHighlight = uIElementsToHighlight,
						ShowDialogOnCenter = true
					};
					TutorialView.Instance.ShowDialogWithHighlightedObjects(hardCodedTutorialData, delegate
					{
						TutorialCompleted("ToggleEndlessModeFTUEWavesTutorial");
					});
				}
			});
			VisualizationQueue.Instance.Add(task4);
		}
	}

	private void TutorialCompleted(string tutorial)
	{
		Helpers.ExecuteCommandDelayed(new SetBlackboardToggleCommand(tutorial));
		tutorialIsRunning = false;
	}
}
