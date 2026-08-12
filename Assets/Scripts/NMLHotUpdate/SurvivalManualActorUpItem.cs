using TWDModel;
using UnityEngine;

public class SurvivalManualActorUpItem : MonoBehaviour
{
	[SerializeField]
	private UISprite HeroIcon;

	[SerializeField]
	private UILabel actorLabel;

	[SerializeField]
	private UILabel oldLevel;

	[SerializeField]
	private UILabel newLevel;

	[SerializeField]
	private SurvivorRarityAndClassPanel RarityAndClass;

	[SerializeField]
	private GameObject SelectedState;

	[SerializeField]
	private UISprite TokenIcon;

	[SerializeField]
	private UILabel TokenNum;

	[SerializeField]
	private GameObject ConditionUpgradable;

	[SerializeField]
	private GameObject ConditionStarLevelTooLow;

	[SerializeField]
	private GameObject ConditionMaxLevelReached;

	[SerializeField]
	private GameObject ConditionNotObtained;

	private SurvivalManualModel survivalManualModel;

	private string storyActorID;

	private SurvivorModel survivorModel;

	private bool Selected;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private SurvivalManualActorLevel curSurvivalManualActorLevel => playerModel.gameEconomyData.GetSurvivalManualActorLevel(survivalManualModel.SurvivalManualDefinition.ActorLevelAttrUpgrade, survivalManualModel.GetActorLevel(storyActorID));

	private SurvivalManualActorLevel curSurvivalManualActorNextLevel => playerModel.gameEconomyData.GetSurvivalManualActorLevel(survivalManualModel.SurvivalManualDefinition.ActorLevelAttrUpgrade, survivalManualModel.GetActorLevel(storyActorID) + 1);

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
		if (type == "SurvivalManualStoryHeroUpgrade")
		{
			if (!CanSelected())
			{
				Selected = false;
			}
			UpdateUI();
		}
	}

	public void Setup(SurvivalManualModel survivalManualModel, string storyActorID)
	{
		this.storyActorID = storyActorID;
		this.survivalManualModel = survivalManualModel;
		survivorModel = survivalManualModel.GetSurvivorByStoryActorId(storyActorID);
		SetSelectState(newSet: true);
	}

	private void UpdateUI()
	{
		StoryActorType storyActorCanUpgradeState = survivalManualModel.GetStoryActorCanUpgradeState(storyActorID);
		HelpersUI.SetContentToLabel(actorLabel, curActorDefinition.Name);
		Helpers.GameObjectSetActive(HeroIcon, value: true);
		Helpers.GameObjectSetActive(actorLabel, value: true);
		HeroIcon.spriteName = curActorDefinition.NormalHead;
		Helpers.GameObjectSetActive(TokenIcon, value: false);
		Helpers.GameObjectSetActive(oldLevel, value: false);
		Helpers.GameObjectSetActive(RarityAndClass, value: false);
		if (survivorModel != null)
		{
			Helpers.GameObjectSetActive(TokenIcon, value: true);
			Helpers.GameObjectSetActive(oldLevel, value: true);
			Helpers.GameObjectSetActive(RarityAndClass, value: true);
			CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivorModel);
			TokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType);
			HelpersUI.SetContentToLabel(actorLabel, survivorModel.Name);
			HelpersUI.SetContentToLabel(oldLevel, "Lv." + survivalManualModel.GetActorLevel(storyActorID));
			RarityAndClass?.UpdateWithSurvivor(survivorModel);
		}
		Helpers.GameObjectSetActive(ConditionUpgradable, value: false);
		Helpers.GameObjectSetActive(ConditionStarLevelTooLow, value: false);
		Helpers.GameObjectSetActive(ConditionMaxLevelReached, value: false);
		Helpers.GameObjectSetActive(ConditionNotObtained, value: false);
		switch (storyActorCanUpgradeState)
		{
		case StoryActorType.Upgradable:
		{
			Helpers.GameObjectSetActive(ConditionUpgradable, value: true);
			int num2 = survivalManualModel.CalculateMaxLevelByFragments(storyActorID);
			if (newLevel != null)
			{
				HelpersUI.SetContentToLabel(newLevel, "Lv." + num2);
				newLevel.color = Color.green;
			}
			TokenNum.text = survivalManualModel.CalculateTotalFragmentsToMaxLevel(storyActorID).ToString();
			TokenNum.color = Color.green;
			break;
		}
		case StoryActorType.NotEnoughFragments:
		{
			Helpers.GameObjectSetActive(ConditionUpgradable, value: true);
			int num = survivalManualModel.CalculateMaxLevelByFragments(storyActorID);
			if (newLevel != null)
			{
				HelpersUI.SetContentToLabel(newLevel, "Lv." + num);
				HelpersUI.SetContentToLabel(TokenNum, curSurvivalManualActorNextLevel.CostToken.ToString());
				TokenNum.color = Color.red;
			}
			break;
		}
		case StoryActorType.StarLevelTooLow:
		{
			UILabel component = ConditionStarLevelTooLow.transform.Find("txt").GetComponent<UILabel>();
			component.text = LocalizationManager.GetText("SurvivalManual_SystemNotice_StarUnlock", curSurvivalManualActorNextLevel.UnlockActorStarLevel);
			component.color = Color.red;
			Helpers.GameObjectSetActive(ConditionStarLevelTooLow, value: true);
			break;
		}
		case StoryActorType.MaxLevelReached:
			Helpers.GameObjectSetActive(ConditionMaxLevelReached, value: true);
			break;
		case StoryActorType.NotObtained:
			Helpers.GameObjectSetActive(ConditionNotObtained, value: true);
			break;
		}
		Helpers.GameObjectSetActive(SelectedState, CanSelected() && Selected);
	}

	public void OnClickSwitchSelect()
	{
		SetSelectState(!Selected);
	}

	public void SetSelectState(bool newSet)
	{
		if (newSet && CanSelected())
		{
			Selected = true;
		}
		else
		{
			Selected = false;
		}
		UpdateUI();
		UIEvent.Send("SurvivalManualStoryUpgradeSelecteddEvent");
	}

	private bool CanSelected()
	{
		return survivalManualModel.GetStoryActorCanUpgradeState(storyActorID) == StoryActorType.Upgradable;
	}

	public bool GetSelectedState()
	{
		return Selected;
	}

	public string GetSelectedStoryActorID()
	{
		return storyActorID;
	}

	public int GetSelectedLevel()
	{
		if (Selected)
		{
			return survivalManualModel.CalculateMaxLevelByFragments(storyActorID);
		}
		return survivalManualModel.GetActorLevel(storyActorID);
	}

	public int GetSelectedAttack()
	{
		return playerModel.gameEconomyData.GetSurvivalManualActorLevel(survivalManualModel.SurvivalManualDefinition.ActorLevelAttrUpgrade, GetSelectedLevel()).Attribute_attack_add;
	}

	public int GetSelectedHP()
	{
		return playerModel.gameEconomyData.GetSurvivalManualActorLevel(survivalManualModel.SurvivalManualDefinition.ActorLevelAttrUpgrade, GetSelectedLevel()).Attribute_hp_add;
	}
}
