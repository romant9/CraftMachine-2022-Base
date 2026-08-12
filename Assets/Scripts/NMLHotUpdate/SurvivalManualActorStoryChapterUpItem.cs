using TWDModel;
using UnityEngine;

public class SurvivalManualActorStoryChapterUpItem : MonoBehaviour
{
	[SerializeField]
	private UISprite HeroIcon;

	[SerializeField]
	private UILabel actorLabel;

	[SerializeField]
	private UILabel Level;

	[SerializeField]
	private SurvivorRarityAndClassPanel RarityAndClass;

	[SerializeField]
	private GameObject SelectedState;

	[SerializeField]
	private GameObject UpgradeNotice;

	private SurvivalManualModel survivalManualModel;

	private string storyActorID;

	private SurvivorModel survivorModel;

	private bool Selected;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private ActorDefinition curActorDefinition
	{
		get
		{
			string survivalManualActorId = playerModel.gameEconomyData.GetSurvivalManualActorId(storyActorID);
			return playerModel.gameEconomyData.GetActorDefinition(survivalManualActorId);
		}
	}

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
		if (type == "SurvivalManualHeroSelected" && parameter != null && parameter is string)
		{
			SetSelectState(storyActorID == (string)parameter);
		}
	}

	public void Setup(SurvivalManualModel survivalManualModel, string storyActorID)
	{
		this.storyActorID = storyActorID;
		this.survivalManualModel = survivalManualModel;
		survivorModel = survivalManualModel.GetSurvivorByStoryActorId(storyActorID);
	}

	private void UpdateUI()
	{
		Helpers.GameObjectSetActive(RarityAndClass, value: false);
		Helpers.GameObjectSetActive(actorLabel, value: true);
		HelpersUI.SetContentToLabel(actorLabel, curActorDefinition.Name);
		HeroIcon.spriteName = curActorDefinition.NormalHead;
		StoryActorType storyActorCanUpgradeState = survivalManualModel.GetStoryActorCanUpgradeState(storyActorID);
		if (survivorModel != null)
		{
			HelpersUI.SetContentToLabel(actorLabel, survivorModel.Name);
			Helpers.GameObjectSetActive(RarityAndClass, value: true);
			RarityAndClass?.UpdateWithSurvivor(survivorModel);
			HelpersUI.SetContentToLabel(Level, "Lv." + survivalManualModel.GetActorLevel(storyActorID));
		}
		if (storyActorCanUpgradeState == StoryActorType.NotObtained)
		{
			HelpersUI.SetContentToLabel(Level, LocalizationManager.GetText("SurvivalManual_SystemNotice_Tips_8"));
		}
		Helpers.GameObjectSetActive(UpgradeNotice, Helpers.IsRedSurvivalManual_Hero(survivalManualModel.ID, storyActorID));
		Helpers.GameObjectSetActive(SelectedState, Selected);
	}

	public void OnClickSelectState()
	{
		UIEvent.Send("SurvivalManualHeroSelected", storyActorID);
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
