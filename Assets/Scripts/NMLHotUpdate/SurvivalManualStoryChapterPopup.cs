using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class SurvivalManualStoryChapterPopup : HUDElement
{
	[SerializeField]
	private GameObject StoryEntryPrefab;

	[SerializeField]
	private GameObject StoryEntryContainer;

	[SerializeField]
	private GameObject HeroEntryPrefab;

	[SerializeField]
	private GameObject HeroEntryContainer;

	[SerializeField]
	private GameObject ChapterEntryPrefab;

	[SerializeField]
	private GameObject ChapterEntryContainer;

	[SerializeField]
	private UILabel heroName;

	[SerializeField]
	private UILabel heroLevel;

	[SerializeField]
	private UILabel oldAttackVal;

	[SerializeField]
	private GameObject NextAttackContainer;

	[SerializeField]
	private UILabel oldHPkVal;

	[SerializeField]
	private GameObject NextHPContainer;

	[SerializeField]
	private UISprite TokenIcon;

	[SerializeField]
	private UILabel UpgradeCostNum;

	[SerializeField]
	private GameObject UpgradeNotice;

	[SerializeField]
	private GameObject HeroConditionUpgradable;

	[SerializeField]
	private GameObject HeroConditionStarLevelTooLow;

	[SerializeField]
	private GameObject HeroConditionNotObtained;

	[SerializeField]
	private GameObject HeroConditionMaxLevelReached;

	[SerializeField]
	private UILabel MemoryLockedDec;

	[SerializeField]
	private UILabel MemoryLockedAttr;

	[SerializeField]
	private UILabel MemoryDec;

	[SerializeField]
	private UITexture MemoryTexture;

	[SerializeField]
	private GameObject MemoryConditionUnlocked;

	[SerializeField]
	private GameObject MemoryConditionNotOpen;

	[SerializeField]
	private GameObject MemoryConditionUnlockable;

	[SerializeField]
	private GameObject MemoryConditionLocked;

	[SerializeField]
	private UILabel HeroLevelLimitLabel;

	[SerializeField]
	private UISprite MedalIcon;

	[SerializeField]
	private UILabel MedalClaimLevel;

	private readonly List<GameObject> StoryEntries = new List<GameObject>();

	private readonly List<GameObject> HeroEntries = new List<GameObject>();

	private readonly List<GameObject> ChapterEntries = new List<GameObject>();

	[SerializeField]
	private UIScrollView heroUIScrollView;

	[SerializeField]
	private UIScrollView storyUIScrollView;

	private int curSurvivalManualId;

	private string curStoryActorID;

	private int curMemoryId;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private SurvivalManualManager survivalManualManager => playerModel.SurvivalManualManager;

	private int FirstSurvivalManualId => survivalManualManager.SurvivalManualModels.FirstOrDefault().ID;

	private SurvivalManualModel curSurvivalManualModel => survivalManualManager.GetSurvivalManualModel(curSurvivalManualId);

	private string FirstStoryActorID => curSurvivalManualModel.SurvivalManualDefinition.ActorList.FirstOrDefault();

	private SurvivorModel curSurvivorModel => curSurvivalManualModel.GetSurvivorByStoryActorId(curStoryActorID);

	private ActorDefinition curActorDefinition
	{
		get
		{
			string survivalManualActorId = playerModel.gameEconomyData.GetSurvivalManualActorId(curStoryActorID);
			return playerModel.gameEconomyData.GetActorDefinition(survivalManualActorId);
		}
	}

	private SurvivalManualActorLevel curSurvivalManualActorLevel => playerModel.gameEconomyData.GetSurvivalManualActorLevel(curSurvivalManualModel.SurvivalManualDefinition.ActorLevelAttrUpgrade, curSurvivalManualModel.GetActorLevel(curStoryActorID));

	private SurvivalManualActorLevel curSurvivalManualActorNextLevel => playerModel.gameEconomyData.GetSurvivalManualActorLevel(curSurvivalManualModel.SurvivalManualDefinition.ActorLevelAttrUpgrade, curSurvivalManualModel.GetActorLevel(curStoryActorID) + 1);

	private int FirstMemoryId => playerModel.gameEconomyData.GetSurvivalManualActorStories(curStoryActorID).FirstOrDefault().MemoryID;

	private SurvivalManualActorStory curMemoryDefinition => playerModel.gameEconomyData.GetSurvivalManualActorStory(curStoryActorID, curMemoryId);

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public override void Open()
	{
		base.Open();
		UpdateStoriesList();
	}

	public void InitSelect(int storyId)
	{
		if (storyId > 0)
		{
			UIEvent.Send("SurvivalManualStorySelected", storyId);
		}
		else
		{
			UIEvent.Send("SurvivalManualStorySelected", FirstSurvivalManualId);
		}
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SurvivalManualStorySelected":
			if (parameter != null && parameter is int)
			{
				curSurvivalManualId = (int)parameter;
				if (!string.IsNullOrEmpty(curSurvivalManualModel.SurvivalManualDefinition.SouvenirMedalIcon))
				{
					Helpers.GameObjectSetActive(MedalIcon, value: true);
					Helpers.GameObjectSetActive(MedalClaimLevel, value: true);
					MedalIcon.spriteName = curSurvivalManualModel.SurvivalManualDefinition.SouvenirMedalIcon;
					MedalClaimLevel.text = "Lv." + curSurvivalManualModel.SurvivalManualDefinition.SouvenirMedalLevel;
				}
				else
				{
					Helpers.GameObjectSetActive(MedalIcon, value: false);
					Helpers.GameObjectSetActive(MedalClaimLevel, value: false);
				}
				UpdateHeroList();
				UIEvent.Send("SurvivalManualHeroSelected", FirstStoryActorID);
			}
			break;
		case "SurvivalManualHeroSelected":
			if (parameter != null && parameter is string)
			{
				curStoryActorID = (string)parameter;
				UpdateHeroUpgradeUI();
				UpdateChapterList();
				UIEvent.Send("SurvivalManualMemorySelected", FirstMemoryId);
			}
			break;
		case "SurvivalManualMemorySelected":
			if (parameter != null && parameter is int)
			{
				curMemoryId = (int)parameter;
				UpdateChapterMainUI();
			}
			break;
		}
	}

	public void UpdateStoriesList()
	{
		UITable component = StoryEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = StoryEntryContainer.GetComponentInParent<UIScrollView>();
		FreshListDataStory();
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void FreshListDataStory()
	{
		ClearStoryEntries();
		ModelList<SurvivalManualModel> survivalManualModels = playerModel.SurvivalManualManager.SurvivalManualModels;
		if (survivalManualModels == null || survivalManualModels.Count <= 0)
		{
			return;
		}
		foreach (SurvivalManualModel item in survivalManualModels)
		{
			if (item != null)
			{
				GameObject gameObject = StoryEntryContainer.AddChild(StoryEntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<SurvivalManualChapterStoryItem>(out var component))
				{
					component.Setup(item);
				}
				StoryEntries.Add(gameObject);
			}
		}
	}

	public void UpdateHeroList()
	{
		UITable component = HeroEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = HeroEntryContainer.GetComponentInParent<UIScrollView>();
		FreshListDataHero();
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void FreshListDataHero()
	{
		ClearHeroEntries();
		List<string> actorList = curSurvivalManualModel.SurvivalManualDefinition.ActorList;
		if (actorList == null || actorList.Count <= 0)
		{
			return;
		}
		foreach (string item in actorList)
		{
			GameObject gameObject = HeroEntryContainer.AddChild(HeroEntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<SurvivalManualActorStoryChapterUpItem>(out var component))
			{
				component.Setup(curSurvivalManualModel, item);
			}
			HeroEntries.Add(gameObject);
		}
	}

	private void UpdateHeroUpgradeUI()
	{
		if (curSurvivorModel != null)
		{
			CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(curSurvivorModel);
			TokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType);
		}
		heroName.text = curActorDefinition.FullName;
		heroLevel.text = curSurvivalManualModel.GetActorLevel(curStoryActorID).ToString();
		oldAttackVal.text = curSurvivalManualActorLevel.Attribute_attack_add.ToString();
		oldHPkVal.text = curSurvivalManualActorLevel.Attribute_hp_add.ToString();
		if (curSurvivalManualActorNextLevel != null)
		{
			Helpers.GameObjectSetActive(NextAttackContainer, value: true);
			Helpers.GameObjectSetActive(NextAttackContainer, value: true);
			NextAttackContainer.transform.Find("val").GetComponent<UILabel>().text = "+" + curSurvivalManualActorNextLevel.Attribute_attack_add;
			NextHPContainer.transform.Find("val").GetComponent<UILabel>().text = "+" + curSurvivalManualActorNextLevel.Attribute_hp_add;
		}
		else
		{
			Helpers.GameObjectSetActive(NextAttackContainer, value: false);
			Helpers.GameObjectSetActive(NextHPContainer, value: false);
		}
		Helpers.GameObjectSetActive(HeroConditionUpgradable, value: false);
		Helpers.GameObjectSetActive(HeroConditionStarLevelTooLow, value: false);
		Helpers.GameObjectSetActive(HeroConditionMaxLevelReached, value: false);
		Helpers.GameObjectSetActive(HeroConditionNotObtained, value: false);
		Helpers.GameObjectSetActive(UpgradeNotice, value: false);
		switch (curSurvivalManualModel.GetStoryActorCanUpgradeState(curStoryActorID))
		{
		case StoryActorType.Upgradable:
			Helpers.GameObjectSetActive(HeroConditionUpgradable, value: true);
			UpgradeCostNum.text = curSurvivalManualActorLevel.CostToken.ToString();
			UpgradeCostNum.color = Color.green;
			Helpers.GameObjectSetActive(UpgradeNotice, value: true);
			break;
		case StoryActorType.NotEnoughFragments:
			Helpers.GameObjectSetActive(HeroConditionUpgradable, value: true);
			UpgradeCostNum.text = curSurvivalManualActorLevel.CostToken.ToString();
			UpgradeCostNum.color = Color.red;
			break;
		case StoryActorType.StarLevelTooLow:
			HeroLevelLimitLabel.text = LocalizationManager.GetText("SurvivalManual_SystemNotice_StarUnlock", curSurvivalManualActorNextLevel.UnlockActorStarLevel);
			Helpers.GameObjectSetActive(HeroConditionStarLevelTooLow, value: true);
			break;
		case StoryActorType.MaxLevelReached:
			Helpers.GameObjectSetActive(HeroConditionMaxLevelReached, value: true);
			break;
		case StoryActorType.NotObtained:
			Helpers.GameObjectSetActive(HeroConditionNotObtained, value: true);
			break;
		}
	}

	public void UpdateChapterList()
	{
		UITable component = ChapterEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = ChapterEntryContainer.GetComponentInParent<UIScrollView>();
		FreshListDataChapter();
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void FreshListDataChapter()
	{
		ClearChapterEntries();
		List<SurvivalManualActorStory> survivalManualActorStories = playerModel.gameEconomyData.GetSurvivalManualActorStories(curStoryActorID);
		if (survivalManualActorStories == null || survivalManualActorStories.Count <= 0)
		{
			return;
		}
		foreach (SurvivalManualActorStory item in survivalManualActorStories)
		{
			GameObject gameObject = ChapterEntryContainer.AddChild(ChapterEntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<SurvivalManualActorMemoryItem>(out var component))
			{
				component.Setup(curSurvivalManualId, curStoryActorID, item.MemoryID);
			}
			ChapterEntries.Add(gameObject);
		}
	}

	private void UpdateChapterMainUI()
	{
		Helpers.GameObjectSetActive(MemoryConditionUnlocked, value: false);
		Helpers.GameObjectSetActive(MemoryConditionNotOpen, value: false);
		Helpers.GameObjectSetActive(MemoryConditionUnlockable, value: false);
		Helpers.GameObjectSetActive(MemoryConditionLocked, value: false);
		switch (curSurvivalManualModel.GetSurvivalManualStoryUnlockStatus(curStoryActorID, curMemoryId))
		{
		case StoryUnlockStatus.Unlockable:
			Helpers.GameObjectSetActive(MemoryConditionLocked, value: true);
			Helpers.GameObjectSetActive(MemoryConditionUnlockable, value: true);
			break;
		case StoryUnlockStatus.Unlocked:
			Helpers.GameObjectSetActive(MemoryConditionUnlocked, value: true);
			break;
		case StoryUnlockStatus.Locked:
			Helpers.GameObjectSetActive(MemoryConditionLocked, value: true);
			break;
		case StoryUnlockStatus.NotOpen:
			Helpers.GameObjectSetActive(MemoryConditionNotOpen, value: true);
			break;
		}
		if (!string.IsNullOrEmpty(curMemoryDefinition.MemoryLockedTips))
		{
			string[] array = curMemoryDefinition.MemoryLockedTips.Split(";");
			if (array != null && array.Length > 1)
			{
				MemoryLockedDec.text = LocalizationManager.GetText(array[0], curActorDefinition.Name, curMemoryDefinition.MemoryUnlockLevel);
				MemoryLockedAttr.text = LocalizationManager.GetText(array[1]);
			}
		}
		MemoryTexture.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle(curMemoryDefinition.MemoryImage, "itemgraphics");
		MemoryDec.text = LocalizationManager.GetText(curMemoryDefinition.MemoryAttrUpgradeDesc, curActorDefinition.Name);
	}

	private void ClearStoryEntries()
	{
		for (int i = 0; i < StoryEntries.Count; i++)
		{
			NGUITools.Destroy(StoryEntries[i]);
		}
		StoryEntries.Clear();
	}

	private void ClearHeroEntries()
	{
		for (int i = 0; i < HeroEntries.Count; i++)
		{
			NGUITools.Destroy(HeroEntries[i]);
		}
		HeroEntries.Clear();
	}

	private void ClearChapterEntries()
	{
		for (int i = 0; i < ChapterEntries.Count; i++)
		{
			NGUITools.Destroy(ChapterEntries[i]);
		}
		ChapterEntries.Clear();
	}

	public void OnclickTips()
	{
		SurvivalManualHelpPopup survivalManualHelpPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualHelpPopup) as SurvivalManualHelpPopup;
		if (survivalManualHelpPopup != null)
		{
			survivalManualHelpPopup.Open(SurvivalManualHelpPopup.HelpType.StoriesHelp);
		}
	}

	public void OnclickUpgrade()
	{
		if (curSurvivalManualModel.GetStoryActorCanUpgradeState(curStoryActorID) == StoryActorType.Upgradable)
		{
			List<string> list = new List<string>();
			list.Add(curStoryActorID);
			if (Helpers.ExecuteCommand(new SurvivalManualActorUpgradeCommand(curSurvivalManualModel.ModelId, list, SurvivalManualActorUpgradeCommand.UpgradeType.ActorUpgrade)) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("Achievement.CooperationLvup.Title"));
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
			}
			UIEvent.Send("SurvivalManualStoryHeroUpgrade");
			Vector3 localPosition = heroUIScrollView.panel.cachedTransform.localPosition;
			Vector2 clipOffset = heroUIScrollView.panel.clipOffset;
			Vector3 localPosition2 = storyUIScrollView.panel.cachedTransform.localPosition;
			Vector2 clipOffset2 = storyUIScrollView.panel.clipOffset;
			FreshAllUI();
			heroUIScrollView.panel.cachedTransform.localPosition = localPosition;
			heroUIScrollView.panel.clipOffset = clipOffset;
			storyUIScrollView.panel.cachedTransform.localPosition = localPosition2;
			storyUIScrollView.panel.clipOffset = clipOffset2;
		}
	}

	public void OnclickChapterUnlock()
	{
		if (Helpers.ExecuteCommand(new UnlockSurvivalManualActorStoryCommand(curSurvivalManualModel.ModelId, curStoryActorID, curMemoryId)) == TWDModelResult.OK)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
		}
		UIEvent.Send("SurvivalManualStoryHeroChapterUnlockEvent");
		Vector3 localPosition = heroUIScrollView.panel.cachedTransform.localPosition;
		Vector2 clipOffset = heroUIScrollView.panel.clipOffset;
		Vector3 localPosition2 = storyUIScrollView.panel.cachedTransform.localPosition;
		Vector2 clipOffset2 = storyUIScrollView.panel.clipOffset;
		FreshAllUI();
		heroUIScrollView.panel.cachedTransform.localPosition = localPosition;
		heroUIScrollView.panel.clipOffset = clipOffset;
		storyUIScrollView.panel.cachedTransform.localPosition = localPosition2;
		storyUIScrollView.panel.clipOffset = clipOffset2;
	}

	private void FreshAllUI()
	{
		UpdateStoriesList();
		UpdateHeroList();
		UpdateHeroUpgradeUI();
		UpdateChapterList();
		UpdateChapterMainUI();
		int num = curSurvivalManualId;
		string parameter = curStoryActorID;
		int num2 = curMemoryId;
		UIEvent.Send("SurvivalManualStorySelected", num);
		UIEvent.Send("SurvivalManualHeroSelected", parameter);
		UIEvent.Send("SurvivalManualMemorySelected", num2);
	}
}
