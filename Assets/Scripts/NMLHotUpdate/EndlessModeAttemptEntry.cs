using System.Collections.Generic;
using System.Linq;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessModeAttemptEntry : MonoBehaviour
{
	[SerializeField]
	private UILabel rankLabel;

	[SerializeField]
	private UILabel waveLabel;

	[SerializeField]
	private UILabel timeStampLabel;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel walkersKilledLabel;

	[SerializeField]
	private UILabel multiplierReachedLabel;

	[SerializeField]
	private List<EndlessModeAttemptTeamEntry> endlessModeAttemptTeamEntries;

	[SerializeField]
	private List<GameObject> survivorSupportEmptyContainers;

	[SerializeField]
	private List<GameObject> survivorSupportActiveContainers;

	[SerializeField]
	private GameObject expertModeTag;

	[SerializeField]
	private Color expertModeColor;

	[SerializeField]
	private Color normalModeColor;

	[SerializeField]
	private GameObject TeamGroupGO;

	[SerializeField]
	private GameObject ScoreDetailsGO;

	[SerializeField]
	private UILabel ScanLabel;

	public void UpdateEntryDetails(int index, EndlessModeGameModeType state)
	{
		EndlessModeAttemptData endlessModeAttemptData = ((state == EndlessModeGameModeType.Normal) ? EndlessModeHelpers.GetOrderedNormalAttemptDataByScore()[index] : EndlessModeHelpers.GetOrderedExpertAttemptDataByScore()[index]);
		if (endlessModeAttemptData != null)
		{
			string content = (index + 1).ToString();
			HelpersUI.SetContentToLabel(rankLabel, content);
			string text = LocalizationManager.GetText("Endless.Combat.Wave{WaveNumber}", endlessModeAttemptData.WaveCount);
			HelpersUI.SetContentToLabel(waveLabel, text);
			string text2 = Helpers.FormatTime(GameManager.Instance.playerModel.UtcTimeStamp - endlessModeAttemptData.TimeStamp);
			HelpersUI.SetContentToLabel(timeStampLabel, LocalizationManager.GetText("Generic.Time.PostedAgo{TimeAgo}", text2));
			string formattedScoreText = EndlessModeHelpers.GetFormattedScoreText(endlessModeAttemptData.Score);
			HelpersUI.SetContentToLabel(scoreLabel, formattedScoreText);
			HelpersUI.SetColor(scoreLabel, (endlessModeAttemptData.GameModeType == EndlessModeGameModeType.Expert) ? expertModeColor : normalModeColor);
			string content2 = endlessModeAttemptData.WalkersKilled.ToString();
			HelpersUI.SetContentToLabel(walkersKilledLabel, content2);
			string formattedScoreMultiplier = EndlessModeHelpers.GetFormattedScoreMultiplier(endlessModeAttemptData.MaxMultiplier);
			HelpersUI.SetContentToLabel(multiplierReachedLabel, formattedScoreMultiplier);
			Helpers.GameObjectSetActive(expertModeTag, endlessModeAttemptData.GameModeType == EndlessModeGameModeType.Expert);
			SetupTeamEntryDetails(endlessModeAttemptData.SurvivorMockData);
			SetupSupportEntryDetails(endlessModeAttemptData.SurvivorSupportData);
			UpdateScan(endlessModeAttemptData.IsScan);
		}
	}

	private void SetupTeamEntryDetails(List<SurvivorMockData> survivorMockData)
	{
		for (int i = 0; i < survivorMockData.Count; i++)
		{
			SurvivorMockData survivorMockData2 = survivorMockData[i];
			EndlessModeAttemptTeamEntry endlessModeAttemptTeamEntry = endlessModeAttemptTeamEntries[i];
			if (survivorMockData2 != null)
			{
				endlessModeAttemptTeamEntry.SetTeamContent(survivorMockData2);
			}
		}
	}

	private void SetupSupportEntryDetails(List<SurvivorSupportData> survivorSupportData)
	{
		Transform transform = survivorSupportActiveContainers.FirstOrDefault()?.transform.parent;
		Transform transform2 = survivorSupportEmptyContainers.FirstOrDefault()?.transform.parent;
		if (transform != null && transform2 != null)
		{
			NGUITools.SetActiveChildren(transform.gameObject, state: false);
			NGUITools.SetActiveChildren(transform2.gameObject, state: true);
		}
		for (int i = 0; i < survivorSupportData.Count; i++)
		{
			SurvivorSupportData survivorSupportData2 = survivorSupportData[i];
			if (survivorSupportData2 != null)
			{
				int supportIndex = survivorSupportData2.SupportIndex;
				EndlessModeAttemptTeamEntry endlessModeAttemptTeamEntry = endlessModeAttemptTeamEntries[supportIndex];
				if (endlessModeAttemptTeamEntry != null)
				{
					Helpers.GameObjectSetActive(survivorSupportEmptyContainers[supportIndex], value: false);
					Helpers.GameObjectSetActive(survivorSupportActiveContainers[supportIndex], value: true);
					endlessModeAttemptTeamEntry.SetSupportContent(survivorSupportData2);
				}
			}
		}
	}

	private void UpdateScan(bool IsScan)
	{
		Helpers.GameObjectSetActive(TeamGroupGO, !IsScan);
		Helpers.GameObjectSetActive(ScoreDetailsGO, !IsScan);
		Helpers.GameObjectSetActive(ScanLabel, IsScan);
	}
}
