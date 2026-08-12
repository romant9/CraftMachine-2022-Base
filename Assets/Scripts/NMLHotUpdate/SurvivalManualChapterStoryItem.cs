using TWDModel;
using UnityEngine;

public class SurvivalManualChapterStoryItem : MonoBehaviour
{
	[SerializeField]
	private UILabel storyName;

	[SerializeField]
	private UILabel storyLevel;

	[SerializeField]
	private GameObject NoticeIconUpgradHero;

	[SerializeField]
	private GameObject SelectedBox;

	private SurvivalManualModel storyModel;

	private bool isSelected;

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
		if (type == "SurvivalManualStorySelected" && parameter != null && parameter is int)
		{
			SetSelectState(storyModel.ID == (int)parameter);
		}
	}

	public void Setup(SurvivalManualModel storyModel)
	{
		this.storyModel = storyModel;
		isSelected = false;
		UpdateUI();
	}

	public void OnClickSelected()
	{
		UIEvent.Send("SurvivalManualStorySelected", storyModel.ID);
	}

	public bool GetSelectState()
	{
		return isSelected;
	}

	public void SetSelectState(bool newSet)
	{
		isSelected = newSet;
		UpdateUI();
	}

	public void UpdateUI()
	{
		Helpers.GameObjectSetActive(NoticeIconUpgradHero, Helpers.IsRedSurvivalManual_StoryId(storyModel.ID));
		Helpers.GameObjectSetActive(SelectedBox, isSelected);
		storyName.text = LocalizationManager.GetText(storyModel.SurvivalManualDefinition.StoryQueueName);
		storyLevel.text = "Lv." + storyModel.GetTotalActorsAllLevel();
	}
}
