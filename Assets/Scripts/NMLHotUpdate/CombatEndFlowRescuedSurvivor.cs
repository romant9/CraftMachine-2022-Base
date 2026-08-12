using System.Collections.Generic;
using TWDModel;

public class CombatEndFlowRescuedSurvivor : CombatEndFlowStep
{
	public List<ActorModel> RescuedSurvivors { get; set; }

	public CombatVictoryScreen VictoryScreen { get; set; }

	public override void StartFlow()
	{
		base.StartFlow();
		EventManager.OnEvent += OnAcceptSurvivorChange;
		SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
		if (survivorInfoPopup != null)
		{
			survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivoreMissionAccept;
			survivorInfoPopup.OpenForModel(RescuedSurvivors[0] as SurvivorModel);
		}
	}

	private void OnAcceptSurvivorChange(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.AcceptSurvivor || eventType == EventManager.EventType.RejectSurvivor)
		{
			EventManager.OnEvent -= OnAcceptSurvivorChange;
			VictoryScreen.OnReturnToCampButtonButton(null);
		}
	}
}
