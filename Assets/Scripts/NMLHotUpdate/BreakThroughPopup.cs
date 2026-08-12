using System;
using System.Collections.Generic;
using BaseModel;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class BreakThroughPopup : HUDElement
{
	[SerializeField]
	private UILabel BTLevelLabel;

	[SerializeField]
	private UITexture EquipIcon;

	[SerializeField]
	private BreakThroughEquipmentList EquipmentList;

	[SerializeField]
	private BreakThroughLevelList LevelList;

	[SerializeField]
	private UILabel ApocalypticEquipAmountLable;

	[SerializeField]
	private BreakThroughMarkContainer[] BreakThroughMarkContainers;

	[SerializeField]
	private GameObject Content_Bottom;

	[SerializeField]
	private GameObject Content_Left;

	[SerializeField]
	private GameObject BreakThroughBtn;

	[SerializeField]
	private GameObject NormalContainer;

	[SerializeField]
	private GameObject RemoldContainer;

	[SerializeField]
	private UISprite RemoldIcon;

	[SerializeField]
	private UILabel RemoldNum;

	private EquipmentItemModel equipmentItemModel;

	private int NeedApocalypticNum;

	private List<string> SelectedApocalypticIds = new List<string>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		if (IsLoadDataManager)
		{
			this.transform.localScale = Vector3.one * (this.gameObject.layer == 5 ?  1.4f : 1.1f);
		}
		UIEvent.OnUIEvent += OnUiEvent;
		this.transform.SetAsLastSibling();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public override void Close()
	{
		if (IsLoadDataManager)
		{
			ResidencePopup.Instance.UpadateEquipUIData();
		}
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (!(type == "BreakThroughSelected"))
		{
			if (type == "BreakThroughUnSelected" && parameter != null && parameter is string)
			{
				string item = parameter as string;
				SelectedApocalypticIds.Remove(item);
				UpdateApocalypticNumUI();
			}
		}
		else if (parameter != null && parameter is string)
		{
			string item2 = parameter as string;
			SelectedApocalypticIds.Add(item2);
			UpdateApocalypticNumUI();
		}
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		equipmentItemModel = model as EquipmentItemModel;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (equipmentItemModel != null)
		{
			NeedApocalypticNum = equipmentItemModel.GetBreakThroughWeaponApocalypticNumber();
			SelectedApocalypticIds.Clear();
			LevelList.InitData(equipmentItemModel);
			EquipIcon.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentItemModel);
			int num = 0;
			if (equipmentItemModel.EquipmentBreakthrough != null)
			{
				num = equipmentItemModel.EquipmentBreakthrough.Level;
			}
			int maxBreakThroughLevel = equipmentItemModel.GetMaxBreakThroughLevel();
			BTLevelLabel.text = LocalizationManager.GetText("Popup.EquipBreakthroughs.Label1{Parameter}", num, maxBreakThroughLevel);
			int value = playerModel.GetCurrency(CurrencyType.ApocalypticEquipToken).Value;
			ApocalypticEquipAmountLable.text = value + " / " + equipmentItemModel.GetBreakThroughWeaponFragmentsNumber();
			ApocalypticEquipAmountLable.color = Color.white;
			if (value < equipmentItemModel.GetBreakThroughWeaponFragmentsNumber())
			{
				ApocalypticEquipAmountLable.color = Color.red;
			}
			Helpers.GameObjectSetActive(NormalContainer, value: false);
			Helpers.GameObjectSetActive(RemoldContainer, value: false);
			if (equipmentItemModel.Definition.SwitchRemoldMode)
			{
				EquipmentList.ClearData();
				Helpers.GameObjectSetActive(RemoldContainer, value: true);
				UpdateRemoldUI();
			}
			else
			{
				Helpers.GameObjectSetActive(NormalContainer, value: true);
				EquipmentList.InitData(equipmentItemModel);
				UpdateNormalUI();
			}
			if (Helpers.IsBreakthroughMaxed(equipmentItemModel))
			{
				Helpers.GameObjectSetActive(Content_Bottom, value: false);
				Helpers.GameObjectSetActive(Content_Left, value: false);
			}
		}
	}

	private bool CheckRemold()
	{
		CurrencyType survivorClassCurrencyType = Helpers.GetSurvivorClassCurrencyType(equipmentItemModel.Definition.SurvivorClass);
		return playerModel.GetCurrency(survivorClassCurrencyType).Value >= NeedApocalypticNum;
	}

	private void UpdateRemoldUI()
	{
		CurrencyType survivorClassCurrencyType = Helpers.GetSurvivorClassCurrencyType(equipmentItemModel.Definition.SurvivorClass);
		RemoldIcon.spriteName = HelpersGfx.GetCurrencyIconName(survivorClassCurrencyType);
		int value = playerModel.GetCurrency(survivorClassCurrencyType).Value;
		RemoldNum.text = value + " / " + NeedApocalypticNum;
		RemoldNum.color = Color.white;
		if (value < NeedApocalypticNum)
		{
			RemoldNum.color = Color.red;
		}
	}

	private void UpdateNormalUI()
	{
		UpdateApocalypticNumUI();
	}

	public void ClickBreakThroughBtn()
	{
		if (!equipmentItemModel.CanBreakthrough)
		{
			return;
		}
		if (!((!equipmentItemModel.Definition.SwitchRemoldMode) ? IsEnoughSelected() : CheckRemold()))
		{
			TooltipManager.OpenTextBoxWithText(BreakThroughBtn, LocalizationManager.GetText("Popup.EquipBreakthroughs.tips1"));
			if (!IsLoadDataManager) return;
		}
		int value = playerModel.GetCurrency(CurrencyType.ApocalypticEquipToken).Value;
		int breakThroughWeaponFragmentsNumber = equipmentItemModel.GetBreakThroughWeaponFragmentsNumber();
		if (value < breakThroughWeaponFragmentsNumber)
		{
			TooltipManager.OpenTextBoxWithText(BreakThroughBtn, LocalizationManager.GetText("Popup.EquipBreakthroughs.tips1"));
			return;
		}
		if (IsLoadDataManager || OfflineManager.IsFakeExecuteCommands)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager || OfflineManager.IsFakeExecuteCommands)");
			SurvivorModel survivor = DataManager.Instance.SurvivorManagementPopUp != null ? DataManager.Instance.SurvivorManagementPopUp.SurvivorModel : null;
			if (!DataManager.Instance.SurvivorManagementPopUp.IsInitDone)
			{
				string ru = "Пожалуйста активируйте сперва панель Выжившие (однократно зайдите туда)";
				string eng = "Please activate a Survivors Panel first (go there once)";
				string text = DataManager.Instance.language == DataManager.Language.Ru ? ru : eng;

				AlertPopup.ShowPopup("", text, LocalizationManager.GetText("Button.Ok"));
				return;
			}
			DataManager.Instance.SurvivorManagementPopUp.BackupTraitsData(equipmentItemModel, survivor);

			TWDModelResult result = equipmentItemModel.BreakthroughLevelUp(SelectedApocalypticIds, equipmentItemModel.GetBreakThroughWeaponFragmentsNumber());

			if (result == TWDModelResult.OK)
			{
				DebugTWD.Log("Прорыв успешен");

				UpdateUI();
				UIEvent.Send("BreakThroughed");
			}
			else
			{
				DebugTWD.Log("что-то пошло не так");
			}
		}
		else
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent("", LocalizationManager.GetText("Popup.EquipBreakthroughsConfirmationPopup.Label1"));
			obj.SetCallbacks(delegate
			{
				if (Helpers.ExecuteCommand(new EquipBreakthroughCommand(equipmentItemModel)
				{
					ConsumeEquipTokenIdList = SelectedApocalypticIds
				}) == TWDModelResult.OK)
				{
					UpdateUI();
					UIEvent.Send("BreakThroughed");
				}
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
	}

	public void ClickAutoSelectBtn()
	{
		EquipmentList.AutoSelect();
	}

	public void UpdateApocalypticNumUI()
	{
		for (int i = 0; i < BreakThroughMarkContainers.Length; i++)
		{
			Helpers.GameObjectSetActive(BreakThroughMarkContainers[i], value: false);
			BreakThroughMarkContainers[i].GetComponent<UIButtonExtended>().SetClickCallback(OnclickMarkContainer);
			if (IsEnoughSelected())
			{
				if (i < NeedApocalypticNum)
				{
					Helpers.GameObjectSetActive(BreakThroughMarkContainers[i], value: true);
					BreakThroughMarkContainers[i].SetFill();
				}
			}
			else if (i < SelectedApocalypticIds.Count)
			{
				Helpers.GameObjectSetActive(BreakThroughMarkContainers[i], value: true);
				BreakThroughMarkContainers[i].SetFill();
			}
			else if (i >= SelectedApocalypticIds.Count && i < NeedApocalypticNum)
			{
				Helpers.GameObjectSetActive(BreakThroughMarkContainers[i], value: true);
				BreakThroughMarkContainers[i].SetEmpty();
			}
		}
	}

	public bool IsEnoughSelected()
	{
		if (NeedApocalypticNum <= SelectedApocalypticIds.Count)
		{
			return true;
		}
		return false;
	}

	public void OnclickMarkContainer(UIButtonExtended button)
	{
		if (equipmentItemModel != null)
		{
			int breakthroughLevel = equipmentItemModel.BreakthroughLevel;
			EquipBreakthroughDefinition equipBreakthroughDefinitionByRarityAndLevel = GameManager.Instance.gameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(equipmentItemModel.RarityLevel, breakthroughLevel + 1);
			if (equipBreakthroughDefinitionByRarityAndLevel != null)
			{
				TooltipManager.OpenTextBoxWithText(button.gameObject, LocalizationManager.GetText(equipBreakthroughDefinitionByRarityAndLevel.MaterialsDescribe));
			}
		}
	}

	public void OnclickTips()
	{
		CommonInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CommonInfoPopup) as CommonInfoPopup;
		obj.SetContent(LocalizationManager.GetText("Popup.EquipBreakthroughs.TipsLabel1"), LocalizationManager.GetText("Popup.EquipBreakthroughs.TipsLabel2"));
		obj.Open();
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public UILabel TokenAmountLabel;
	#endregion

	#region mycode
	public void SetCurrencyAmount()
	{
		int tokenAmount;
		try
		{
			tokenAmount = Convert.ToInt32(TokenAmountLabel.text.ToString());
		}
		catch
		{
			tokenAmount = 100;
		}
		playerModel.SetCurrency(CurrencyType.ApocalypticEquipToken, tokenAmount);
		int tokens = playerModel.GetCurrency(CurrencyType.ApocalypticEquipToken).Value;
		ApocalypticEquipAmountLable.text = tokens + " / " + equipmentItemModel.GetBreakThroughWeaponFragmentsNumber();
	}
	#endregion
}
