using System.Collections.Generic;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessModeAttemptScoreList : MonoBehaviour
{
	[SerializeField]
	private GameObject attemptScorePrefab;

	[SerializeField]
	private GameObject attemptScoreContainer;

	[SerializeField]
	private bool isScorePopup;

	[SerializeField]
	private UIScrollBar attemptUiScrollBar;

	[SerializeField]
	private int scrollItemThreshold;

	[SerializeField]
	private float scoreitemOffSet;

	private List<GameObject> attemptEntries = new List<GameObject>();

	private EndlessModeGameModeType _state;

	private void ClearAttemptScores()
	{
		for (int i = 0; i < attemptEntries.Count; i++)
		{
			NGUITools.Destroy(attemptEntries[i]);
		}
		attemptEntries.Clear();
	}

	private void OpenAttemptScorePopup(int index)
	{
		EndlessModeAttemptScorePopup endlessModeAttemptScorePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeAttemptScorePopup) as EndlessModeAttemptScorePopup;
		if (endlessModeAttemptScorePopup != null)
		{
			endlessModeAttemptScorePopup.OpenWithIndex(index, _state);
		}
	}

	private void OnClickAttemptScore(int index)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (!isScorePopup)
		{
			OpenAttemptScorePopup(index);
			return;
		}
		EndlessModeAttemptEntry endlessModeAttemptEntry = Object.FindObjectOfType<EndlessModeAttemptEntry>();
		if (endlessModeAttemptEntry != null)
		{
			CenterToSelectedAttemptEntry(index);
			endlessModeAttemptEntry.UpdateEntryDetails(index, _state);
		}
	}

	public void CenterToSelectedAttemptEntry(int index)
	{
		float num = ((index < scrollItemThreshold) ? Mathf.Epsilon : ((float)index / (float)(attemptEntries.Count - 1)));
		attemptUiScrollBar.value = Mathf.Clamp01(num + scoreitemOffSet);
	}

	public void UpdateUI(EndlessModeGameModeType state, SurvivorClass survivorClass = SurvivorClass.None)
	{
		_state = state;
		List<EndlessModeAttemptData> list = null;
		switch (state)
		{
		case EndlessModeGameModeType.Normal:
			list = EndlessModeHelpers.GetOrderedNormalAttemptDataByScore();
			break;
		case EndlessModeGameModeType.Expert:
			list = ((survivorClass == SurvivorClass.None) ? EndlessModeHelpers.GetOrderedExpertAttemptDataByScore() : EndlessModeHelpers.GetEndlessExpertModeAttemptDataListBySurvivorClass(survivorClass));
			break;
		}
		if (list == null || list.Count == 0)
		{
			return;
		}
		ClearAttemptScores();
		UITable component = attemptScoreContainer.GetComponent<UITable>();
		UIScrollView componentInParent = attemptScoreContainer.GetComponentInParent<UIScrollView>();
		int count = list.Count;
		int num = 0;
		num = ((state != EndlessModeGameModeType.Normal) ? EndlessModeHelpers.EndlessModeConfig.AttemptsToSumForFinalScoreExpert : EndlessModeHelpers.EndlessModeConfig.AttemptsToSumForFinalScoreNormal);
		for (int i = 0; i < count; i++)
		{
			EndlessModeAttemptData endlessModeAttemptData = list[i];
			if (endlessModeAttemptData == null)
			{
				continue;
			}
			GameObject gameObject = attemptScoreContainer.AddChild(attemptScorePrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<EndlessModeAttemptScoreEntry>(out var component2))
			{
				component2.SetContent(endlessModeAttemptData.Score, i + 1, i < num, endlessModeAttemptData.GameModeType == EndlessModeGameModeType.Expert, endlessModeAttemptData.SurvivorMockData[0].ActorDefinitionId);
			}
			if (gameObject.TryGetComponent<UIButton>(out var component3))
			{
				int selectedIndex = i;
				EventDelegate.Set(component3.onClick, delegate
				{
					OnClickAttemptScore(selectedIndex);
				});
			}
			attemptEntries.Add(gameObject);
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}
}
