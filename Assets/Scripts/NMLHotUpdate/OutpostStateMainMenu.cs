public class OutpostStateMainMenu : OutpostStateBase
{
	public void OnEditTeam()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		obj.SurvivorType = SurvivorContainerModel.SurvivorType.Outpost;
		obj.Open();
	}
}
