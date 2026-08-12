using System.Collections.Generic;
using TWDModel;

public class CombatEndScreenHandler
{
	private ECombatResult combatEndResult;

	private CombatModel combatModel;

	public CombatEndScreenHandler(CombatModel combat)
	{
		combatModel = combat;
	}

	public void BeginEndScreen(ECombatResult result, List<ActorModel> rescuedSurvivors)
	{
		combatEndResult = result;
		CombatVictoryScreen combatVictoryScreen = null;
		if (combatEndResult == ECombatResult.Successful)
		{
			combatVictoryScreen = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatVictoryScreen) as CombatVictoryScreen;
			combatVictoryScreen.Setup(combatModel.MissionRoster, rescuedSurvivors, combatModel.MissionStatistics.CollectedLoot);
		}
		else
		{
			DebugTWD.Log("TODO: сделать повтор миссии", DebugType.Wars);
			OfflineManager.Instance.IsReturnToResidence = true;
			GameManager.Instance.ReturnFromVisit();
		}
		if (combatVictoryScreen != null)
		{
			combatVictoryScreen.Open();
		}
	}
}
