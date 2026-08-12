using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class WorkshopPopup : HUDElement
{
	[SerializeField]
	private Transform classFilterPosition;

	[SerializeField]
	private GameObject classFilterPrefab;

	[SerializeField]
	private UISprite selectedClassSprite;

	[SerializeField]
	private UILabel selectedClassName;

	[SerializeField]
	private UILabel selectedClassDescription;

	[SerializeField]
	private UILabel equipmentLevelIncreaseTokenAmount;

	[SerializeField]
	private UITexture selectedClassArt;

	[SerializeField]
	private WorkshopClassListPanel equipmentListPanel;

	[SerializeField]
	private WorkshopTokenClassListPanel equipmentTokenListPanel;

	[SerializeField]
	private GameObject scrollbar;

	[SerializeField]
	private GameObject scrollbarToken;

	[SerializeField]
	private UIButton scrapModeButton;

	[SerializeField]
	private UIButton scrapOKButton;

	[SerializeField]
	private GameObject scrapMenu;

	[SerializeField]
	private GameObject scrapEntryContainer;

	[SerializeField]
	private GameObject scrapEntryPrefab;

	[SerializeField]
	private UILabel scrapTotalCurrencyLabel1;

	[SerializeField]
	private UILabel scrapTotalCurrencyLabel2;

	[SerializeField]
	private GameObject inventoryCountParent;

	[SerializeField]
	private UILabel inventoryCount;

	[SerializeField]
	private UITexture switchIcon;

	private EquipScrapMode scrapModeActive;

	private bool isEquipment;

	private int scrapTotalCurrency1;

	private int scrapTotalCurrency2;

	private List<EquipmentItemModel> scrapEquipmentItems = new List<EquipmentItemModel>();

	private GameObject classFilterInstance;

	private Dictionary<SurvivorClass, int> availableUpgradesCount = new Dictionary<SurvivorClass, int>();

	[SerializeField]
	private GameObject autoScrapPrefab;

	[SerializeField]
	private GameObject autoScrapScrollContainer;

	[SerializeField]
	private UIScrollView autoScrapScrollView;

	[SerializeField]
	private UITable autoScrapTable;

	[SerializeField]
	private UILabel currentAutoScrap;

	[SerializeField]
	private GameObject autoScrapGameObjest;

	[SerializeField]
	private GameObject switchButton;

	private List<GameObject> autoScrapS;

	private List<string> availableautoScrapS = new List<string> { "Popup.Workshop.AutoScrap.Off", "Popup.Workshop.AutoScrap.ThreeStars", "Popup.Workshop.AutoScrap.FourStars", "Popup.Workshop.AutoScrap.FiveStars" };

	private string currentAutoScraps;

	private SurvivorClass currentSurvivorClass;

	private readonly List<GameObject> ScrapEntries = new List<GameObject>();

	public override void Open()
	{
		base.Open();
		autoScrapS = new List<GameObject>();
		if (equipmentListPanel != null)
		{
			SurvivorClass survivorClass = SurvivorClass.None;
			if (classFilterInstance != null)
			{
				SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
				if (component != null)
				{
					survivorClass = component.CurrentSelectedFilter.ClassFilter;
				}
				if (survivorClass == SurvivorClass.None)
				{
					survivorClass = component.GetFirstAvailableClass();
				}
				SurvivorClassFilter component2 = classFilterInstance.GetComponent<SurvivorClassFilter>();
				SurvivorListFilter currentSelectedFilter = component2.CurrentSelectedFilter;
				currentSelectedFilter.ClassFilter = survivorClass;
				component2.SetSelectedClass(currentSelectedFilter);
			}
		}
		SetScrapMode(EquipScrapMode.Normal);
		UpdateUI();
		RecalculateEquipmentsToUpgradeCount();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_workshop");
		isEquipment = true;
		SwitchEquipment(isEquipment);
		if (!IsLoadDataManager) CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: false);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (Helpers.GameObjectSetActive(inventoryCountParent, scrapModeActive == EquipScrapMode.Normal))
		{
			int count = GameManager.Instance.playerModel.Equipment.GetAllEquipments().Count;
			int maxItemCount = GameManager.Instance.playerModel.gameEconomyData.ConfigData.MaxItemCount;
			HelpersUI.SetContentToLabel(inventoryCount, count + " / " + maxItemCount);
		}
	}

	private void OnLanguageChanged()
	{
		currentAutoScrap.text = LocalizationManager.GetText(currentAutoScraps);
		selectedClassName.text = HelpersLocalization.GetSurvivorClassName(currentSurvivorClass);
	}

	public void SetAutoScrap(string key)
	{
		autoScrapScrollContainer.SetActive(value: false);
		UIEvent.Send("WorkshopPopupUnSelectEvent", currentAutoScraps);
		currentAutoScraps = key;
		currentAutoScrap.text = LocalizationManager.GetText(currentAutoScraps);
		UIEvent.Send("WorkshopPopupSelectEvent", key);
	}

	public override void Close()
	{
		SetScrapMode(EquipScrapMode.Normal);
		base.Close();
		if (!IsLoadDataManager) CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: true);
		ClearScrapEntries();
	}

	public void OnScrapModeEnabled()
	{
		SetScrapMode(EquipScrapMode.Scrap);
		ClearScrapEntries();
	}

	public void OnScrapEquipmentItemsClicked()
	{
		if (scrapEquipmentItems.Count > 0)
		{
			Cashier cashier = null;
			if (scrapModeActive == EquipScrapMode.Scrap)
			{
				cashier = GameManager.Instance.playerModel.Equipment.GetEquipmentListScrapCashier(scrapEquipmentItems);
			}
			if (cashier == null)
			{
				DebugLogError("scrapCashier NULL");
				return;
			}
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent(LocalizationManager.GetText("Popup.ScrapConfirmationList.Title"), LocalizationManager.GetText("System.scrap.confirmtips"));
			obj.SetCallbacks(OnScrapEquipmentItemsConfirmed, OnScrapEquipmentItemsCancelled);
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
	}

	public void OnScrapEquipmentItemsCancelled()
	{
		SetScrapMode(EquipScrapMode.Normal);
	}

	private void OnEnable()
	{
		if (IsLoadDataManager)
		{
			Instance = this;
			PlayerRandomValues.Instance.On_Call_Reset += ResetTraitsData;
		}
		UIEvent.OnUIEvent += OnUIEvent;
		if (classFilterInstance == null)
		{
			classFilterInstance = Helpers.InstantiateToParent(classFilterPrefab, base.gameObject);
			classFilterInstance.transform.localPosition = classFilterPosition.localPosition;
		}
		if (equipmentListPanel != null)
		{
			SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
			equipmentListPanel.SetClassFilter(component);
			if (component != null)
			{
				component.OnClassFilterSelected += OnClassFilterButtonClicked;
			}
		}
		if (equipmentTokenListPanel != null)
		{
			SurvivorClassFilter component2 = classFilterInstance.GetComponent<SurvivorClassFilter>();
			equipmentTokenListPanel.SetClassFilter(component2);
			if (component2 != null)
			{
				component2.OnClassFilterSelected += OnClassFilterButtonClicked;
			}
		}
		if (classFilterInstance != null)
		{
			SurvivorClassFilter component3 = classFilterInstance.GetComponent<SurvivorClassFilter>();
			if (component3 != null)
			{
				component3.SetGenericFilterButtonsEnabled(active: false);
				component3.UpdatePositionAndState();
			}
		}
		if (IsLoadDataManager)
		{
			equipmentListPanel.tabs.SetSelectedIndex(0);
			equipmentListPanel.tabs.GetUIButtonToggleList[0].ForceClick();
		}
	}

	private void OnDisable()
	{
		if (IsLoadDataManager)
		{
			PlayerRandomValues.Instance.On_Call_Reset -= ResetTraitsData;
			Instance = null;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
		if (classFilterInstance != null)
		{
			SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
			if (component != null)
			{
				component.OnClassFilterSelected -= OnClassFilterButtonClicked;
			}
		}
	}

	private void OnClassFilterButtonClicked(SurvivorClass selectedClass)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		currentSurvivorClass = selectedClass;
		if (selectedClassName != null)
		{
			selectedClassName.text = HelpersLocalization.GetSurvivorClassName(selectedClass);
		}
		if (selectedClassDescription != null)
		{
			selectedClassDescription.text = HelpersLocalization.GetSurvivorClassDescription(selectedClass);
		}
		if (selectedClassSprite != null)
		{
			selectedClassSprite.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(selectedClass);
		}
		if (selectedClassArt != null)
		{
			HelpersGfx.SetSurvivorClassMaterial(selectedClassArt, selectedClass);
		}
		SetScrapMode(EquipScrapMode.Normal);
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "EquipmentStartUpgrade":
			RecalculateEquipmentsToUpgradeCount();
			if (!IsLoadDataManager) Close();
			break;
		case "EquipmentInstantUpgraded":
		case "EquipmentUpgraded":
		case "OnEquipmentUpdated":
			RecalculateEquipmentsToUpgradeCount();
			break;
		case "OnEquipTokenUnlockEvent":
		{
			RecalculateEquipmentsToUpgradeCount();
			SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
			if (component != null)
			{
				component.SetSelectedClass(component.CurrentSelectedFilter);
			}
			break;
		}
		case "EquipmentAddToScrapList":
			if (parameter is EquipmentItemModel { Owner: null } equipmentItemModel && !scrapEquipmentItems.Contains(equipmentItemModel))
			{
				scrapEquipmentItems.Add(equipmentItemModel);
			}
			FreshScrapListData();
			SetScrapOKButton(scrapEquipmentItems.Count > 0);
			break;
		case "EquipmentRemoveFromScrapList":
			if (parameter is EquipmentItemModel item && scrapEquipmentItems.Contains(item))
			{
				scrapEquipmentItems.Remove(item);
			}
			FreshScrapListData();
			SetScrapOKButton(scrapEquipmentItems.Count > 0);
			break;
		case "EquipmentScrapped":
			UpdateUI();
			break;
		}
	}

	private void OnScrapEquipmentItemsConfirmed()
	{
		if (scrapModeActive == EquipScrapMode.Scrap)
		{
			ScrapEquipmentItemsCommand scrapEquipmentItemsCommand = new ScrapEquipmentItemsCommand(scrapEquipmentItems);
			if (Helpers.ExecuteCommand(scrapEquipmentItemsCommand) == TWDModelResult.OK)
			{
				SPRemoldScrapRewardsPopup sPRemoldScrapRewardsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldScrapRewardsPopup) as SPRemoldScrapRewardsPopup;
				if (sPRemoldScrapRewardsPopup != null && scrapEquipmentItemsCommand.Rewards != null && scrapEquipmentItemsCommand.Rewards.RewardsList != null && scrapEquipmentItemsCommand.Rewards.RewardsList.Count > 0)
				{
					sPRemoldScrapRewardsPopup.SetupRewards(scrapEquipmentItemsCommand.Rewards);
					sPRemoldScrapRewardsPopup.Open();
				}
				SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
				component.SetSelectedClass(component.CurrentSelectedFilter);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/workshop_equipment_scrap");
			}
		}
		SetScrapMode(EquipScrapMode.Normal);
	}

	private void SetScrapMode(EquipScrapMode enabled)
	{
		scrapModeActive = enabled;
		if (!IsLoadDataManager)
			{
			if (scrapMenu != null)
			{
				scrapMenu.SetActive(enabled != EquipScrapMode.Normal);
				if (scrapModeButton != null)
				{
					scrapModeButton.gameObject.SetActive(enabled == EquipScrapMode.Normal && isEquipment);
				}
				SetScrapOKButton(scrapEquipmentItems.Count > 0);
				if (autoScrapScrollView != null)
				{
					if (autoScrapS != null)
					{
						for (int i = 0; i < autoScrapS.Count; i++)
						{
							UnityEngine.Object.Destroy(autoScrapS[i]);
						}
						autoScrapS.Clear();
					}
					autoScrapScrollView.ResetPosition();
				}
				autoScrapGameObjest.SetActive(enabled == EquipScrapMode.Scrap);
				autoScrapScrollContainer.SetActive(value: false);
			}
			if (enabled == EquipScrapMode.Scrap && autoScrapScrollView != null)
			{
				for (int j = 0; j < autoScrapS.Count; j++)
				{
					UnityEngine.Object.Destroy(autoScrapS[j]);
				}
				autoScrapS.Clear();
				autoScrapScrollView.ResetPosition();
				for (int k = 0; k < availableautoScrapS.Count; k++)
				{
					GameObject gameObject = Helpers.InstantiateToParent(autoScrapPrefab, autoScrapScrollView.gameObject);
					gameObject.GetComponent<AutoScrapItem2>().SetKey(this, availableautoScrapS[k]);
					autoScrapS.Add(gameObject);
				}
				autoScrapScrollView.ResetPosition();
				autoScrapTable.repositionNow = true;
				currentAutoScraps = availableautoScrapS[0];
				currentAutoScrap.text = LocalizationManager.GetText(currentAutoScraps);
			}
			switchButton.SetActive(enabled == EquipScrapMode.Normal);
		}
		if (enabled == EquipScrapMode.Normal)
		{
			scrapTotalCurrency1 = 0;
			scrapTotalCurrencyLabel1.text = "0";
			scrapTotalCurrency2 = 0;
			scrapTotalCurrencyLabel2.text = "0";
			scrapEquipmentItems.Clear();
		}
		UpdateUI();
		UIEvent.Send("SetEquipmentScrapMode", enabled);
	}

	public void ToggleAutoScrapScroll()
	{
		autoScrapScrollContainer.SetActive(!autoScrapScrollContainer.activeSelf);
		autoScrapScrollView.ResetPosition();
	}

	private void SetScrapOKButton(bool enabled)
	{
		if (scrapOKButton != null)
		{
			scrapOKButton.SetState((!enabled) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, true);
			Color color = (enabled ? scrapOKButton.defaultColor : scrapOKButton.disabledColor);
			UILabel componentInChildren = scrapOKButton.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.color = color;
			}
		}
	}

	private void RecalculateEquipmentsToUpgradeCount()
	{
		SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
		for (int i = 0; i < 6; i++)
		{
			availableUpgradesCount[(SurvivorClass)i] = GetTotalAvailableEquipmentUpgradesForClass((SurvivorClass)i);
			int equipTokenCountBySurvivorClass = GameManager.Instance.playerModel.EquipTokenContainer.GetEquipTokenCountBySurvivorClass((SurvivorClass)i);
			if (!(component != null))
			{
				continue;
			}
			GameObject buttonForClass = component.GetButtonForClass((SurvivorClass)i);
			if (buttonForClass != null)
			{
				SurvivorClassButton component2 = buttonForClass.GetComponent<SurvivorClassButton>();
				if (component2 != null)
				{
					component2.NotificationCount = availableUpgradesCount[(SurvivorClass)i];
					component2.NotificationTokenCount = equipTokenCountBySurvivorClass;
				}
			}
		}
		HelpersUI.SetContentToLabel(equipmentLevelIncreaseTokenAmount, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Workshop.TokensAmount{parameter}", GameManager.Instance.playerModel.GetCurrency(CurrencyType.EquipmentUpgradeToken).Value.ToString()));
	}

	public static int GetTotalAvailableEquipmentUpgradesForClass(SurvivorClass survivorClass)
	{
		int num = 0;
		ModelList<EquipmentItemModel> meleeWeapons = GameManager.Instance.playerModel.Equipment.MeleeWeapons;
		int count = meleeWeapons.Count;
		for (int i = 0; i < count; i++)
		{
			EquipmentItemModel equipmentModel = meleeWeapons[i];
			if (CanEquipmentBeUpgraded(survivorClass, equipmentModel))
			{
				num++;
			}
		}
		ModelList<EquipmentItemModel> rangeWeapons = GameManager.Instance.playerModel.Equipment.RangeWeapons;
		count = rangeWeapons.Count;
		for (int j = 0; j < count; j++)
		{
			EquipmentItemModel equipmentModel2 = rangeWeapons[j];
			if (CanEquipmentBeUpgraded(survivorClass, equipmentModel2))
			{
				num++;
			}
		}
		ModelList<EquipmentItemModel> armors = GameManager.Instance.playerModel.Equipment.Armors;
		count = armors.Count;
		for (int k = 0; k < count; k++)
		{
			EquipmentItemModel equipmentModel3 = armors[k];
			if (CanEquipmentBeUpgraded(survivorClass, equipmentModel3))
			{
				num++;
			}
		}
		return num;
	}

	private static bool CanEquipmentBeUpgraded(SurvivorClass survivorClass, EquipmentItemModel equipmentModel)
	{
		if (equipmentModel != null)
		{
			if (equipmentModel.Definition.CanBeEquippedBySurvivorClass(survivorClass) && equipmentModel.CanUpgrade && equipmentModel.CanBeManipulated())
			{
				return equipmentModel.GetUpgradeCashier(instantUpgrade: false).CanAfford();
			}
			return false;
		}
		return false;
	}

	public void OnClickSwitchButton()
	{
		isEquipment = !isEquipment;
		if (IsLoadDataManager) BundlePromo.SetActive(!isEquipment);
		SwitchEquipment(isEquipment);
	}

	private void SwitchEquipment(bool isEquip)
	{
		Helpers.GameObjectSetActive(equipmentListPanel, isEquip);
		Helpers.GameObjectSetActive(equipmentTokenListPanel, !isEquip);
		Helpers.GameObjectSetActive(scrollbar, isEquip);
		Helpers.GameObjectSetActive(scrollbarToken, !isEquip);
		Helpers.GameObjectSetActive(scrapModeButton, isEquip);
		if (switchIcon != null)
		{
			UnityEngine.Object obj = UnityUtils.LoadFromAssetBundle(isEquip ? "Icon_Equipment_Token_Close" : "Icon_Equipment_Token_Open", "itemgraphics");
			if (obj != null)
			{
				switchIcon.mainTexture = (Texture)obj;
			}
		}
	}

	private void FreshScrapListData()
	{
		ClearScrapEntries();
		if (scrapEquipmentItems.Count <= 0)
		{
			return;
		}
		Rewards equipmentListScrapReward = GameManager.Instance.modelManager.Player.Equipment.GetEquipmentListScrapReward(scrapEquipmentItems);
		UITable component = scrapEntryContainer.GetComponent<UITable>();
		int count = equipmentListScrapReward.RewardsList.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = scrapEntryContainer.AddChild(scrapEntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldScrapItemPreview>(out var component2))
			{
				component2.Setup(equipmentListScrapReward.RewardsList[i]);
			}
			ScrapEntries.Add(gameObject);
		}
		component.Reposition();
	}

	private void ClearScrapEntries()
	{
		for (int i = 0; i < ScrapEntries.Count; i++)
		{
			NGUITools.Destroy(ScrapEntries[i]);
		}
		ScrapEntries.Clear();
	}



	#region myparams
	public static WorkshopPopup Instance;
	public GameObject BundlePromo; //Add tokens
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion

	#region mycode
	public void ResetTraitsData(bool isResetCounters)
	{
		var listEquip = DataManager.Instance.SurvivorManagementPopUp.EqipmentTraitsList;

		if (listEquip.Count > 0)
		{
			foreach (var equipment in listEquip)
			{
				var eqipmentDe = OfflineManager.JsonSerializer.Deserialize<EquipmentItemModel>(equipment.Value.equipmentItemModel);
				var equipmentOrigin = DataManager.Instance.Player.Equipment.ChangeEqupmentModel(eqipmentDe, out bool isWeapon);
				var survivor = equipment.Value.Survivor;

				if (survivor != null)
				{
					survivor.EquipmentItems.Models[isWeapon ? 1 : 0] = equipmentOrigin;
				}
			}
			listEquip.Clear();
			if (gameObject.activeSelf)
			{
				var index = equipmentListPanel.tabs.GetSelectedIndex();
				equipmentListPanel.getCardAt(index).UpdateUI();
			}
		}
	}

	public void ResetTraitsDataButton()
	{
		PlayerRandomValues.Instance.ResetAll(true);
	}
	#endregion
}
