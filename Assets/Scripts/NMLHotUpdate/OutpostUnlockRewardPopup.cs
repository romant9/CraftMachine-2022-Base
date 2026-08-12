using System.Collections.Generic;
using TWDModel;

public class OutpostUnlockRewardPopup : HUDElement
{
	public void OnConfirmClicked()
	{
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.OutpostGiftSurvivorsGiven"))
		{
			Helpers.ExecuteCommand(new GiveOutpostSurvivorsCommand());
			CampView.Instance.ShowDialog("Portrait_Daryl", new List<string> { "Tutorial.OutpostTutorial.GotSurvivors.1" }, CampView.Instance.OutpostTutorialSurvivorsGivenDialogOver);
		}
		base.Close();
	}
}
