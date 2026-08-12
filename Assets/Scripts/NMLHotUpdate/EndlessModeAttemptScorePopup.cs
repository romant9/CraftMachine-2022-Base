using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessModeAttemptScorePopup : HUDElement
{
	[Header("General")]
	public UIButton closeButtton;

	[Header("My Scores")]
	[SerializeField]
	private UILabel totalScoreLabel;

	[SerializeField]
	private GameObject attemptScoreContainer;

	[SerializeField]
	private UILabel attemptsToFinalScoreLabel;

	[SerializeField]
	private EndlessModeAttemptScoreList endlessModeAttemptScoreList;

	public override void Open()
	{
		base.Open();
		SetupOnClickCloseButton();
	}

	public void OpenWithIndex(int index, EndlessModeGameModeType state)
	{
		Open();
		string content = ((state == EndlessModeGameModeType.Normal) ? EndlessModeHelpers.GetFormattedOverAllAttemptsScoreNormal() : EndlessModeHelpers.GetFormattedOverAllAttemptsScoreExpert());
		HelpersUI.SetContentToLabel(totalScoreLabel, content);
		HelpersUI.SetContentToLabel(attemptsToFinalScoreLabel, LocalizationManager.GetText("Endless.Hub.TotalScore.Tooltip{Parameter}", (state == EndlessModeGameModeType.Normal) ? EndlessModeHelpers.EndlessModeConfig.AttemptsToSumForFinalScoreNormal : EndlessModeHelpers.EndlessModeConfig.AttemptsToSumForFinalScoreExpert));
		endlessModeAttemptScoreList.UpdateUI(state);
		EndlessModeAttemptEntry endlessModeAttemptEntry = Object.FindObjectOfType<EndlessModeAttemptEntry>();
		if (endlessModeAttemptEntry != null && endlessModeAttemptScoreList != null)
		{
			endlessModeAttemptEntry.UpdateEntryDetails(index, state);
			endlessModeAttemptScoreList.CenterToSelectedAttemptEntry(index);
		}
	}

	private void SetupOnClickCloseButton()
	{
		EventDelegate.Set(closeButtton.onClick, OnClickCloseButton);
	}

	private void OnClickCloseButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		Close();
	}
}
