using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivorSurvivalManualPanel : MonoBehaviour
{
	public enum SkillExpandType
	{
		none = 0,
		one = 1,
		twoShowLeft = 2,
		twoShowRight = 3
	}

	[SerializeField]
	private UILabel SurvivalManualLevel;

	[SerializeField]
	private GameObject AttributeAttack;

	[SerializeField]
	private GameObject AttributeHp;

	[SerializeField]
	private GameObject PrivateAttackRatio;

	[SerializeField]
	private GameObject PrivateHpRatio;

	[SerializeField]
	private GameObject AttributeHitrateMelee;

	[SerializeField]
	private GameObject AttributeHitrateRange;

	[SerializeField]
	private GameObject AttributeCriticalRef;

	[SerializeField]
	private GameObject AttributeCriticalRatioRef;

	[SerializeField]
	private GameObject conditionOne;

	[SerializeField]
	private GameObject conditionShowLeft;

	[SerializeField]
	private GameObject conditionShowRight;

	[SerializeField]
	private SurvivorRarityAndClassPanel RarityAndClass;

	[SerializeField]
	private GameObject NoticeIcon;

	private SkillExpandType curentExpandType;

	private List<int> storyIds = new List<int>();

	private SurvivalManualManager survivalManualManager
	{
		get
		{
			if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
			{
				return GameManager.Instance.playerModel.SurvivalManualManager;
			}
			return null;
		}
	}

	private List<SurvivalManualStorySkill> StorySkills
	{
		get
		{
			List<SurvivalManualStorySkill> list = new List<SurvivalManualStorySkill>();
			if (storyIds == null || storyIds.Count <= 0)
			{
				return list;
			}
			for (int i = 0; i < storyIds.Count; i++)
			{
				SurvivalManualModel survivalManualModel = survivalManualManager.GetSurvivalManualModel(storyIds[i]);
				if (survivalManualModel != null)
				{
					list.Add(survivalManualModel.SkillDefinition);
				}
			}
			return list;
		}
	}

	public void UpdateUI(SurvivorModel survivorModel)
	{
		if (RarityAndClass != null)
		{
			RarityAndClass.UpdateWithSurvivor(survivorModel);
		}
		SurvivalManualLevel.text = "Lv." + survivalManualManager.GetSystemLV();
		AttributeAttack.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttackClinet(survivorModel);
		AttributeHp.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetHPClinet(survivorModel);
		PrivateAttackRatio.transform.Find("val").GetComponent<UILabel>().text = "+" + (survivalManualManager.GetPrivateAttackRatioClient(survivorModel) + survivalManualManager.GetAttributeAttackRatioClient()) + "%";
		PrivateHpRatio.transform.Find("val").GetComponent<UILabel>().text = "+" + (survivalManualManager.GetPrivateHpRatioClient(survivorModel) + survivalManualManager.GetAttributeHpRatioClient()) + "%";
		AttributeHitrateMelee.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeHitrateMeleeClient() + "%";
		AttributeHitrateRange.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeHitrateRangeClient() + "%";
		AttributeCriticalRef.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeCriticalRefClient() + "%";
		AttributeCriticalRatioRef.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeDmgCriticalRatioRefClient() + "%";
		storyIds = Helpers.GetSurvivalManualStorySkillList(survivorModel.ActorDefinitionID);
		if (storyIds != null)
		{
			storyIds.RemoveAll((int t) => t <= 0);
		}
		if (storyIds == null || storyIds.Count <= 0)
		{
			curentExpandType = SkillExpandType.none;
		}
		if (storyIds != null && storyIds.Count == 1)
		{
			curentExpandType = SkillExpandType.one;
		}
		if (storyIds != null && storyIds.Count == 2)
		{
			curentExpandType = SkillExpandType.twoShowLeft;
		}
		UpdateSkillList();
		Helpers.GameObjectSetActive(NoticeIcon, Helpers.IsRedSurvivalManual_Hero(survivorModel));
	}

	private void UpdateSkillList()
	{
		Helpers.GameObjectSetActive(conditionOne, value: false);
		Helpers.GameObjectSetActive(conditionShowLeft, value: false);
		Helpers.GameObjectSetActive(conditionShowRight, value: false);
		switch (curentExpandType)
		{
		case SkillExpandType.one:
			Helpers.GameObjectSetActive(conditionOne, value: true);
			conditionOne.transform.FindInChildren("level").GetComponent<UILabel>().text = "Lv." + StorySkills[0].Level;
			conditionOne.transform.FindInChildren("name").GetComponent<UILabel>().text = LocalizationManager.GetText(StorySkills[0].SkillName);
			conditionOne.transform.FindInChildren("icon").GetComponent<UISprite>().spriteName = StorySkills[0].Icon;
			break;
		case SkillExpandType.twoShowLeft:
			Helpers.GameObjectSetActive(conditionShowLeft, value: true);
			conditionShowLeft.transform.Find("left").FindInChildren("level").GetComponent<UILabel>()
				.text = "Lv." + StorySkills[0].Level;
			conditionShowLeft.transform.Find("left").FindInChildren("name").GetComponent<UILabel>()
				.text = LocalizationManager.GetText(StorySkills[0].SkillName);
			conditionShowLeft.transform.Find("left").FindInChildren("icon").GetComponent<UISprite>()
				.spriteName = StorySkills[0].Icon;
			conditionShowLeft.transform.Find("right").FindInChildren("level").GetComponent<UILabel>()
				.text = "Lv." + StorySkills[1].Level;
			conditionShowLeft.transform.Find("right").FindInChildren("icon").GetComponent<UISprite>()
				.spriteName = StorySkills[1].Icon;
			break;
		case SkillExpandType.twoShowRight:
			Helpers.GameObjectSetActive(conditionShowRight, value: true);
			conditionShowRight.transform.Find("left").FindInChildren("level").GetComponent<UILabel>()
				.text = "Lv." + StorySkills[0].Level;
			conditionShowRight.transform.Find("left").FindInChildren("icon").GetComponent<UISprite>()
				.spriteName = StorySkills[0].Icon;
			conditionShowRight.transform.Find("right").FindInChildren("level").GetComponent<UILabel>()
				.text = "Lv." + StorySkills[1].Level;
			conditionShowRight.transform.Find("right").FindInChildren("name").GetComponent<UILabel>()
				.text = LocalizationManager.GetText(StorySkills[1].SkillName);
			conditionShowRight.transform.Find("right").FindInChildren("icon").GetComponent<UISprite>()
				.spriteName = StorySkills[1].Icon;
			break;
		case SkillExpandType.none:
			break;
		}
	}

	public void OnclickSwitch()
	{
		switch (curentExpandType)
		{
		case SkillExpandType.twoShowLeft:
			curentExpandType = SkillExpandType.twoShowRight;
			break;
		case SkillExpandType.twoShowRight:
			curentExpandType = SkillExpandType.twoShowLeft;
			break;
		}
		UpdateSkillList();
	}

	public void OnclickOpenActivePop()
	{
		int num = -1;
		switch (curentExpandType)
		{
		case SkillExpandType.one:
			num = storyIds[0];
			break;
		case SkillExpandType.twoShowLeft:
			num = storyIds[0];
			break;
		case SkillExpandType.twoShowRight:
			num = storyIds[1];
			break;
		}
		if (num >= 0)
		{
			SurvivalManualSkillDetailsPopup survivalManualSkillDetailsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualSkillDetailsPopup) as SurvivalManualSkillDetailsPopup;
			if (survivalManualSkillDetailsPopup != null)
			{
				survivalManualSkillDetailsPopup.Open(SurvivalManualSkillDetailsPopup.OpenType.ReadType, num);
			}
		}
	}



	#region myparams
	public UIButton JumpButton;
    #endregion

    #region mycode
    private void Start()
    {
        if (OfflineManager.IsLoadDataManager && JumpButton)
		{
			JumpButton.gameObject.SetActive(true);
        }
    }
    #endregion
}
