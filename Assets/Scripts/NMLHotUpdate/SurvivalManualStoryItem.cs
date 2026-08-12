using TWDModel;
using UnityEngine;

public class SurvivalManualStoryItem : MonoBehaviour
{
	[SerializeField]
	private UILabel storyName;

	[SerializeField]
	private UILabel storyDesc;

	[SerializeField]
	private UILabel storyLevel;

	[SerializeField]
	private UILabel storyDmg;

	[SerializeField]
	private UILabel storyHP;

	[SerializeField]
	private GameObject storyLevelMax;

	[SerializeField]
	private UILabel storySkillName;

	[SerializeField]
	private UILabel storySkillLevel;

	[SerializeField]
	private UISprite storySkillIcon;

	[SerializeField]
	private GameObject NoticeIconSkill;

	[SerializeField]
	private GameObject NoticeIconLevel;

	[SerializeField]
	private GameObject NoticeIconDetails;

	[SerializeField]
	private UISprite storyMainIcon;

	[SerializeField]
	private UISprite medalIcon;

	[SerializeField]
	private GameObject TimeContainer;

	[SerializeField]
	private UILabel TimeTitle;

	[SerializeField]
	private UILabel TimeLeftNum;

	[SerializeField]
	private UILabel TimeLeftType;

	[SerializeField]
	private UILabel TimeRightNum;

	[SerializeField]
	private UILabel TimeRightType;

	private int storyId = -1;

	private SurvivalManualModel storyModel => playerModel.SurvivalManualManager.GetSurvivalManualModel(storyId);

	private PlayerModel playerModel => GameManager.Instance.playerModel;

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
		switch (type)
		{
		case "SurvivalManualStorySkillUpgrade":
		case "SurvivalManualStoryHeroUpgrade":
		case "SurvivalManualStoryHeroChapterUnlockEvent":
			UpdateUI();
			break;
		}
	}

	public void Setup(int storyId)
	{
		this.storyId = storyId;
		UpdateUI();
	}

	public void OnClickDetails()
	{
		SurvivalManualStoriesPopup survivalManualStoriesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualStoriesPopup) as SurvivalManualStoriesPopup;
		if (survivalManualStoriesPopup != null)
		{
			survivalManualStoriesPopup.OnClickStoryDetails(storyModel.ID);
		}
	}

	public void OnClickUpgradeAllBtn()
	{
		SurvivalManualUpgradePopup survivalManualUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualUpgradePopup) as SurvivalManualUpgradePopup;
		if (survivalManualUpgradePopup != null)
		{
			survivalManualUpgradePopup.Open(storyModel.ID);
		}
	}

	public void OnClickUpgradeBtn()
	{
		SurvivalManualSkillDetailsPopup survivalManualSkillDetailsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualSkillDetailsPopup) as SurvivalManualSkillDetailsPopup;
		if (survivalManualSkillDetailsPopup != null && storyModel != null)
		{
			survivalManualSkillDetailsPopup.Open(SurvivalManualSkillDetailsPopup.OpenType.UpgradeType, storyModel.ID);
		}
	}

	public void UpdateUI()
	{
		Helpers.GameObjectSetActive(storyLevelMax, storyModel.IsSurvivalInMaxLevel());
		storyName.text = LocalizationManager.GetText(storyModel.SurvivalManualDefinition.StoryQueueName);
		storyDesc.text = LocalizationManager.GetText(storyModel.SurvivalManualDefinition.StoryQueueDesc);
		storyDmg.text = "+" + storyModel.GetSurvivalManualAttack().ToString();
		storyHP.text = "+" + storyModel.GetSurvivalManualHp().ToString();
		storyLevel.text = "Lv." + storyModel.GetTotalActorsAllLevel();
		storyMainIcon.spriteName = storyModel.SurvivalManualDefinition.StoryQueueImage;
		Helpers.GameObjectSetActive(medalIcon, !string.IsNullOrEmpty(storyModel.SurvivalManualDefinition.SouvenirMedalIcon));
		medalIcon.spriteName = storyModel.SurvivalManualDefinition.SouvenirMedalIcon;
		storySkillName.text = LocalizationManager.GetText(storyModel.SkillDefinition.SkillName);
		storySkillLevel.text = "Lv." + storyModel.SkillDefinition.Level;
		storySkillIcon.spriteName = storyModel.SkillDefinition.Icon;
		Helpers.GameObjectSetActive(TimeContainer, value: false);
		if (storyModel.SurvivalManualDefinition.HasDateLimit && storyModel.Timer > 0)
		{
			Helpers.GameObjectSetActive(TimeContainer, value: true);
			TimeTitle.text = LocalizationManager.GetText(storyModel.SurvivalManualDefinition.ActiveDesc);
			int num = (int)(storyModel.Timer / 1000);
			int num2 = num / 86400;
			int num3 = num - num2 * 24 * 60 * 60;
			int num4 = num3 / 3600;
			int num5 = (num3 - num4 * 60 * 60) / 60 + 1;
			if (num2 > 0)
			{
				TimeLeftNum.text = num2.ToString();
				TimeLeftType.text = LocalizationManager.GetText("SurvivalManual_Active_Timer_DD");
				TimeRightNum.text = num4.ToString();
				TimeRightType.text = LocalizationManager.GetText("SurvivalManual_Active_Timer_HH");
			}
			else
			{
				TimeLeftNum.text = num4.ToString();
				TimeLeftType.text = LocalizationManager.GetText("SurvivalManual_Active_Timer_HH");
				TimeRightNum.text = num5.ToString();
				TimeRightType.text = LocalizationManager.GetText("SurvivalManual_Active_Timer_MM");
			}
		}
		Helpers.GameObjectSetActive(NoticeIconDetails, Helpers.IsRedSurvivalManual_StoryId(storyModel.ID));
		Helpers.GameObjectSetActive(NoticeIconLevel, Helpers.IsRedSurvivalManual_StoryUpgradeLevel(storyModel.ID));
		Helpers.GameObjectSetActive(NoticeIconSkill, Helpers.CanSurvivalManualStorySkillUpgrade(storyModel.ID));
	}
}
