using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivalManualMainPopup : HUDElement
{
	[SerializeField]
	private UILabel SystemLV;

	[SerializeField]
	private UILabel SkillLevel;

	[SerializeField]
	private UILabel SkillName;

	[SerializeField]
	private GameObject SystemAttack;

	[SerializeField]
	private GameObject SystemHP;

	[SerializeField]
	private GameObject AttributeAttackRatio;

	[SerializeField]
	private GameObject AttributeHpRatio;

	[SerializeField]
	private GameObject AttributeHitrateMelee;

	[SerializeField]
	private GameObject AttributeHitrateRange;

	[SerializeField]
	private GameObject AttributeCriticalRef;

	[SerializeField]
	private GameObject AttributeDmgCriticalRatioRef;

	[SerializeField]
	private GameObject NextContainer;

	[SerializeField]
	private GameObject UpgradeCondition;

	[SerializeField]
	private GameObject UpgradeNotLevelCondition;

	[SerializeField]
	private GameObject UpgradeMaxCondition;

	[SerializeField]
	private PayButton UpgradeButton;

	[SerializeField]
	private GameObject NoticeIconUpgrade;

	[SerializeField]
	private GameObject NoticeIconEnter;

	private SurvivalManualType curUpgradeType;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private SurvivalManualManager survivalManualManager => playerModel.SurvivalManualManager;

	private Cashier cashier
	{
		get
		{
			Cashier cashier = new Cashier(GameManager.Instance.modelManager);
			foreach (KeyValuePair<CurrencyType, int> item in survivalManualManager.SkillDefinition.GetUpgradCostInfo())
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualSkill);
				CurrencyType key = item.Key;
				int value = item.Value;
				cashierItem.SetCost(key, value);
				cashier.AddItem(cashierItem);
			}
			return cashier;
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

	public override void Open()
	{
		base.Open();
		UpdateUI();
		CampHUD.SetSurvivalManualEXTokensCurrencyVisibility(visibility: true);
		CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: false);
	}

	public override void Close()
	{
		base.Close();
		CampHUD.SetSurvivalManualEXTokensCurrencyVisibility(visibility: false);
		CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: true);
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SurvivalManualSkillUpgrade":
		case "SurvivalManualStorySkillUpgrade":
		case "SurvivalManualStoryHeroUpgrade":
			UpdateUI();
			break;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		SkillName.text = LocalizationManager.GetText(survivalManualManager.SkillDefinition.SkillName);
		SystemLV.text = "Lv." + survivalManualManager.GetSystemLV();
		SkillLevel.text = "Lv." + survivalManualManager.SurvivalManualSkillLevel;
		SystemAttack.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetSystemAttack().ToString();
		SystemHP.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetSystemHP().ToString();
		AttributeAttackRatio.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeAttackRatioClient() + "%";
		AttributeHpRatio.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeHpRatioClient() + "%";
		AttributeHitrateMelee.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeHitrateMeleeClient() + "%";
		AttributeHitrateRange.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeHitrateRangeClient() + "%";
		AttributeCriticalRef.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeCriticalRefClient() + "%";
		AttributeDmgCriticalRatioRef.transform.Find("val").GetComponent<UILabel>().text = "+" + survivalManualManager.GetAttributeDmgCriticalRatioRefClient() + "%";
		curUpgradeType = survivalManualManager.CanUpgradeSurvivalManualAttributeLeve();
		UpdateButtonState();
		Helpers.GameObjectSetActive(NoticeIconUpgrade, Helpers.CanUpgradeSurvivalManualAttribute());
		Helpers.GameObjectSetActive(NoticeIconEnter, Helpers.IsRedSurvivalManual_stories());
	}

	private void UpdateButtonState()
	{
		Helpers.GameObjectSetActive(UpgradeCondition, value: false);
		Helpers.GameObjectSetActive(UpgradeNotLevelCondition, value: false);
		Helpers.GameObjectSetActive(UpgradeMaxCondition, value: false);
		if (survivalManualManager.SkillDefinition == null)
		{
			return;
		}
		switch (curUpgradeType)
		{
		case SurvivalManualType.UpgradeCondition:
			Helpers.GameObjectSetActive(UpgradeCondition, value: true);
			UpgradeButton.UpdateUI(cashier, LocalizationManager.GetText("SurvivalManual_Button_LvUp"));
			break;
		case SurvivalManualType.UpgradeNotLevelCondition:
			Helpers.GameObjectSetActive(UpgradeNotLevelCondition, value: true);
			UpgradeNotLevelCondition.transform.Find("val").GetComponent<UILabel>().text = LocalizationManager.GetText("SurvivalManual_SystemNotice_Tips_LvLimit", survivalManualManager.SkillDefinition.UnlockLevel);
			break;
		case SurvivalManualType.UpgradeMaxCondition:
			Helpers.GameObjectSetActive(UpgradeMaxCondition, value: true);
			UpgradeMaxCondition.transform.Find("val").GetComponent<UILabel>().text = LocalizationManager.GetText("SurvivalManual_SystemNotice_Tips_LvMax");
			break;
		}
		Helpers.GameObjectSetActive(NextContainer, value: false);
		string nextLevelValue = survivalManualManager.SkillDefinition.NextLevelValue;
		if (!string.IsNullOrEmpty(nextLevelValue))
		{
			string[] array = nextLevelValue.Split(';');
			if (array != null && array.Length > 2)
			{
				Helpers.GameObjectSetActive(NextContainer, value: true);
				NextContainer.transform.FindInChildren("img").GetComponent<UISprite>().spriteName = array[0];
				NextContainer.transform.FindInChildren("name").GetComponent<UILabel>().text = LocalizationManager.GetText(array[1]);
				NextContainer.transform.FindInChildren("val").GetComponent<UILabel>().text = array[2];
			}
		}
	}

	public void OnclickTipsTop()
	{
		SurvivalManualHelpPopup survivalManualHelpPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualHelpPopup) as SurvivalManualHelpPopup;
		if (survivalManualHelpPopup != null)
		{
			survivalManualHelpPopup.Open(SurvivalManualHelpPopup.HelpType.MainTopHelp);
		}
	}

	public void OnclickTipsBottom()
	{
		SurvivalManualHelpPopup survivalManualHelpPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualHelpPopup) as SurvivalManualHelpPopup;
		if (survivalManualHelpPopup != null)
		{
			survivalManualHelpPopup.Open(SurvivalManualHelpPopup.HelpType.MainBottomHelp);
		}
	}

	public void OnClickUpgradeBtn()
	{
		if (curUpgradeType == SurvivalManualType.UpgradeCondition && cashier.CanAfford())
		{
			if (Helpers.ExecuteCommand(new UpgradeSurvivalManualAttributeCommand()) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("Achievement.CooperationLvup.Title"));
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
			}
			UIEvent.Send("SurvivalManualSkillUpgrade");
		}
	}

	public void OnClickLevelRank()
	{
		SurvivalManualRankPopup survivalManualRankPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualRankPopup) as SurvivalManualRankPopup;
		if (survivalManualRankPopup != null)
		{
			survivalManualRankPopup.Open();
		}
	}

	public void OnClickEnter()
	{
		SurvivalManualStoriesPopup survivalManualStoriesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualStoriesPopup) as SurvivalManualStoriesPopup;
		if (survivalManualStoriesPopup != null)
		{
			survivalManualStoriesPopup.Open();
		}
	}

	public void OnClickBackCamp()
	{
		Close();
	}
}
