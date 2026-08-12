using TWDModel;
using UnityEngine;

public class SurvivalManualActorMemoryItem : MonoBehaviour
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private GameObject selectedState;

	[SerializeField]
	private GameObject lockGo;

	private int survivalManualModelId = -1;

	private string storyActorID;

	private int memoryId = -1;

	private bool Selected;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SurvivalManualMemorySelected" && parameter != null && parameter is int)
		{
			SetSelectState(memoryId == (int)parameter);
		}
	}

	public void Setup(int survivalManualModelId, string storyActorID, int memoryId)
	{
		this.survivalManualModelId = survivalManualModelId;
		this.storyActorID = storyActorID;
		this.memoryId = memoryId;
		Selected = false;
	}

	private void UpdateUI()
	{
		StoryUnlockStatus survivalManualStoryUnlockStatus = GameManager.Instance.playerModel.SurvivalManualManager.GetSurvivalManualModel(survivalManualModelId).GetSurvivalManualStoryUnlockStatus(storyActorID, memoryId);
		GameManager.Instance.playerModel.gameEconomyData.GetSurvivalManualActorStory(storyActorID, memoryId);
		title.text = LocalizationManager.GetText("SurvivalManual_Title_4", memoryId);
		Helpers.GameObjectSetActive(selectedState, Selected);
		Helpers.GameObjectSetActive(lockGo, survivalManualStoryUnlockStatus != StoryUnlockStatus.Unlocked);
	}

	public void OnClickSelected()
	{
		UIEvent.Send("SurvivalManualMemorySelected", memoryId);
	}

	public void SetSelectState(bool newSet)
	{
		Selected = newSet;
		UpdateUI();
	}

	public bool GetSelectedState()
	{
		return Selected;
	}
}
