using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivalManualSkillDetailsPopup : HUDElement
{
	public enum OpenType
	{
		ReadType = 0,
		UpgradeType = 1
	}

	[SerializeField]
	private UISprite SkillIcon;

	[SerializeField]
	private UILabel SkillName;

	[SerializeField]
	private UILabel SkillLevel;

	[SerializeField]
	private UILabel skillTips;

	[SerializeField]
	private UILabel skillDesc;

	[SerializeField]
	private GameObject UpgradeCondition;

	[SerializeField]
	private GameObject UpgradeNotLevelCondition;

	[SerializeField]
	private UILabel levelLimit;

	[SerializeField]
	private GameObject UpgradeMaxCondition;

	[SerializeField]
	private PayButton UpgradeButton;

	[SerializeField]
	private GameObject UpgradeNotice;

	private OpenType openType;

	private int storyId = -1;

	private SurvivalManualStorySkill SkillDefinition => storyModel.SkillDefinition;

	private SurvivalManualModel storyModel => playerModel.SurvivalManualManager.GetSurvivalManualModel(storyId);

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private Cashier cashier
	{
		get
		{
			Cashier cashier = new Cashier(GameManager.Instance.modelManager);
			foreach (KeyValuePair<CurrencyType, int> item in SkillDefinition.GetUpgradCostInfo())
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualStorySkill);
				cashierItem.SetCost(item.Key, item.Value);
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

	public void Open(OpenType openType, int storyId)
	{
		base.Open();
		this.openType = openType;
		this.storyId = storyId;
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SurvivalManualStorySkillUpgrade" && parameter != null && parameter is int)
		{
			storyId = (int)parameter;
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		SkillIcon.spriteName = SkillDefinition.Icon;
		SkillName.text = LocalizationManager.GetText(SkillDefinition.SkillName);
		SkillLevel.text = "Lv." + SkillDefinition.Level;
		skillTips.text = LocalizationManager.GetText("SurvivalManual_SystemNotice_Tips_3", LocalizationManager.GetText(storyModel.SurvivalManualDefinition.StoryQueueName));
		string[] array = SkillDefinition.TraitsDesc.Split(";");
		string text = "";
		int num = 0;
		while (array != null && num < array.Length)
		{
			text = text + HelpersLocalization.GetTraitDescription(array[num]) + "\n";
			num++;
		}
		skillDesc.text = text;
		Helpers.GameObjectSetActive(UpgradeCondition, value: false);
		Helpers.GameObjectSetActive(UpgradeNotLevelCondition, value: false);
		Helpers.GameObjectSetActive(UpgradeMaxCondition, value: false);
		if (openType == OpenType.UpgradeType)
		{
			switch (playerModel.SurvivalManualManager.GetSurvivalManualStorySkillCanUpgradeState(storyModel.ID))
			{
			case SurvivalManualType.UpgradeCondition:
				UpgradeButton.UpdateUI(cashier, LocalizationManager.GetText("SurvivalManual_Button_LvUp"));
				Helpers.GameObjectSetActive(UpgradeCondition, value: true);
				break;
			case SurvivalManualType.UpgradeNotLevelCondition:
				Helpers.GameObjectSetActive(UpgradeNotLevelCondition, value: true);
				levelLimit.text = LocalizationManager.GetText("SurvivalManual_SystemNotice_Tips_StorySkillLimit", LocalizationManager.GetText(storyModel.SurvivalManualDefinition.StoryQueueName), SkillDefinition.UnlockLevel);
				break;
			case SurvivalManualType.UpgradeMaxCondition:
				Helpers.GameObjectSetActive(UpgradeMaxCondition, value: true);
				break;
			}
		}
		Helpers.GameObjectSetActive(UpgradeNotice, Helpers.CanSurvivalManualStorySkillUpgrade(storyModel.ID));
	}

	public void OnClickUpgradeBtn()
	{
		if (cashier.CanAfford())
		{
			if (Helpers.ExecuteCommand(new SurvivalManualStorSkillCommand(storyModel.ID)) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("Achievement.CooperationLvup.Title"));
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
			}
			UIEvent.Send("SurvivalManualStorySkillUpgrade", storyModel.ID);
		}
	}
}
