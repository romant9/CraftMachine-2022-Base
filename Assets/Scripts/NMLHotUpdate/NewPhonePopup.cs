using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class NewPhonePopup : HUDElement
{
	private enum DrawCardMode
	{
		Survivor = 0,
		Weapon = 1,
		GoldRadio = 2
	}

	[Header("Top")]
	[SerializeField]
	public UILabel RadiophonesAmountLabel;

	[SerializeField]
	public UILabel GoldRadiophonesAmountLabel;

	[SerializeField]
	private GameObject RadiophonesTop;

	[SerializeField]
	private GameObject GoldRadiophonesTop;

	[SerializeField]
	private PhoneBundlePanel BundlePanel;

	[SerializeField]
	[Header("Bottom")]
	public PhoneMaxNotificationPanel MaxLevelPanel;

	[SerializeField]
	private PhoneClassUnlockPanel ClassUnlockPanel;

	[SerializeField]
	private FeaturedHeroBanner FeaturedHeroBanner;

	[SerializeField]
	[Header("Dynamically Loaded")]
	public Transform ButtonParentTarget;

	[SerializeField]
	private GameObject CallButtonPrefab;

	[SerializeField]
	private PhoneWeaponContainer phoneWeaponContainer;

	[SerializeField]
	private GoldRadioWeaponContainer goldRadioWeaponContainer;

	[SerializeField]
	private GameObject newGoldRadioBannerNotificationContainer;

	private DropType[] DropTypeToIndexArray = new DropType[3]
	{
		DropType.Regular,
		DropType.Silver,
		DropType.Gold
	};

	public List<RadioCallButton> CallButtonsList { get; set; } = new List<RadioCallButton>();

	private string LastClickedTutorialArrowId = "";

	private bool openBlackMarketOnNextClose;

	[SerializeField]
	private UIWidget survivorContainer;

	[SerializeField]
	private UIWidget weaponContainer;

	[SerializeField]
	private UIWidget goldRadioContainer;

	[SerializeField]
	public UIButtonToggle survivorToggle;

	[SerializeField]
	public UIButtonToggle weaponToggle;

	[SerializeField]
	public UIButtonToggle goldRadioToggle;

	public static void OpenRadiophoneFeaturePopup()
	{
		if (!TutorialView.Allowed("PhoneButton"))
		{
			return;
		}
		if (!TutorialView.Instance.Running)
		{
			CampModel camp = GameManager.Instance.playerModel.Camp;
			BuildingsAmountsDefinition buildingsAmountsAtCouncilLevel = GameManager.Instance.gameEconomyData.GetBuildingsAmountsAtCouncilLevel(camp.GetCouncilLevel() - 1);
			if (buildingsAmountsAtCouncilLevel != null && buildingsAmountsAtCouncilLevel.RadioTentAmount > 0 && camp.GetBuilding("RadioTent") == null)
			{
				AlertPopup.ShowPopupGetText("Popup.Alert.NeedToBuildRadioTentTitle", "Popup.Alert.NeedToBuildRadioTentMessage", "Button.Ok", null);
				return;
			}
		}
		if (GameManager.Instance.playerModel.PhoneCall.HasPendingSurvivor)
		{
			SelectSurvivorsPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectSurvivorPopup) as SelectSurvivorsPopup;
			obj.SkipClickIntro = true;
			obj.Open();
		}
		else
		{
			NewPhonePopup obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup;
			obj2.Open();
			obj2.ResetScroll();
		}
		EventManager.NotifyClick("PhoneButton");
	}

	public void Awake()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) Instance = this");
			if (Instance != null)
			{
				Debug.LogError("Multiple SocialPopupGuild!");
				return;
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		DebugClassString = "NewPhonePopup";
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		if (ButtonDelete)
		{
			DebugTWD.LogMycode("if (ButtonDelete)");
			ButtonDelete.isEnabled = CallCraft.Instance.SelectedCall != null;
		}
		CraftSettings.Instance.ShowMeters(new int[] { 1, 3 });
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		CraftSettings.Instance.ShowMeters(new int[] { 0, 1 });
	}

	public override void Open()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (!gameObject.activeSelf) return;
			IsInitDone = true;
			DebugTWD.Log("NewPhonePopup Open");
		}
		base.Open();
		if (GameManager.Instance == null || GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.PhoneCall == null || GameManager.Instance.playerModel.Camp == null)
		{
			DebugLogError("Could not open Data was NULL!");
			return;
		}
		if (!TutorialView.Instance.Running)
		{
			CampModel camp = GameManager.Instance.playerModel.Camp;
			BuildingsAmountsDefinition buildingsAmountsAtCouncilLevel = GameManager.Instance.gameEconomyData.GetBuildingsAmountsAtCouncilLevel(camp.GetCouncilLevel() - 1);
			if (buildingsAmountsAtCouncilLevel != null && buildingsAmountsAtCouncilLevel.RadioTentAmount > 0 && camp.GetBuilding("RadioTent") == null)
			{
				AlertPopup.ShowPopupGetText("Popup.Alert.NeedToBuildRadioTentTitle", "Popup.Alert.NeedToBuildRadioTentMessage", "Button.Ok", null);
				return;
			}
		}
		InitButtons();
		if (!IsLoadDataManager)
		{
			if (BundlePanel != null)
			{
				BundlePanel.SetClickCallback(OnClickedBundle);
			}
			if (ClassUnlockPanel != null)
			{
				ClassUnlockPanel.AddClickListener(GotoClassUnlockMap);
			}
		}
		UpdateUI();
		EnableAllCallButtons(value: true);
		CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: IsLoadDataManager);
		phoneWeaponContainer.Open();
		if (goldRadioWeaponContainer) goldRadioWeaponContainer.Open();
		SetToggle(DrawCardMode.Survivor);
	}

	private void InitButtons()
	{
		if (IsLoadDataManager && buttonParentTargetOriginalPosition == Vector3.zero)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && buttonParentTargetOriginalPosition == Vector3.zero)");
			buttonParentTargetOriginalPosition = ButtonParentTarget.localPosition;
		}
		InstantiateButtons();
		PositionButtons();
		if (IsLoadDataManager)
		{
			if (CallButtonsList != null)
			{
				//int maxNumber = CallButtonsList.Select(x => x.SlotNumber).Max();
				var list = CallButtonsList.Select(x => x.GetCallPrice());
				int maxNumber = list.Max();
				int cheapNumber = list.FirstOrDefault(x => x == 40);
				int regularNumber = list.FirstOrDefault(x => x == 15);

				CallCraft.Instance.GetComponent<CallAuto>()?.SetStartCallNumbers(maxNumber, cheapNumber, regularNumber);
			}
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			UIScrollView componentInChildren = ButtonParentTarget.GetComponentInChildren<UIScrollView>();
			if (componentInChildren != null)
			{
				ButtonParentTarget.localPosition = buttonParentTargetOriginalPosition;
				componentInChildren.ResetPosition();
			}
		}
	}

	private BundleStoreDefinition GetRadioBundleOffer()
	{
		List<string> radioCallBundlesToShow = GameManager.Instance.playerModel.gameEconomyData.ConfigData.RadioCallBundlesToShow;
		if (radioCallBundlesToShow != null)
		{
			for (int i = 0; i < radioCallBundlesToShow.Count; i++)
			{
				if (radioCallBundlesToShow[i] != null)
				{
					BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(radioCallBundlesToShow[i]);
					if (bundleStoreDefinition != null && GameManager.Instance.playerModel.BundleManager.CanBuyBundle(bundleStoreDefinition))
					{
						return bundleStoreDefinition;
					}
				}
			}
		}
		return null;
	}

	private void GotoClassUnlockMap(UIButtonExtended button)
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_accept");
		}
		if (ClassUnlockPanel != null)
		{
			ClassUnlockPanel.RemoveListeners();
		}
		QuestDefinition nextUnlockSurvivorClassQuest = QuestUtils.GetNextUnlockSurvivorClassQuest(GameManager.Instance.modelManager);
		MapMissionGroupModel mapMissionGroupModel = null;
		if (nextUnlockSurvivorClassQuest != null)
		{
			mapMissionGroupModel = nextUnlockSurvivorClassQuest.GetUnlockedEpisode(GameManager.Instance.modelManager);
		}
		if (mapMissionGroupModel != null)
		{
			CampManager.Instance.GoToMap(mapMissionGroupModel);
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBundleBought":
			UpdateUI();
			break;
		case "OnRadioPopupCardExpired":
			InitButtons();
			break;
		case "PhoneWeaponDrawCardDone":
			UpdatePhoneUI();
			break;
		case "PhoneWeaponSelected":
			//if (parameter is EquipPrizeWheelDefinition equipPrizeWheelDefinition)
			//{
			//	Debug.LogError(equipPrizeWheelDefinition.RadioType.ToString() + "radio type =========");
			//}
			break;
		case "UpdateTokenInstance":
			DebugTWD.LogMycode("case \"UpdateTokenInstance\":");
			var item = parameter as TokenSelectorButton;
			var currencyID = item.currencyType.ToString();
			tokenButton.Initialize(currencyID, null);
			CallCraft.Instance.SetPriorityHeroToken(currencyID);
			break;
		}
		if (type == "MarkGoldRadioBanner")
		{
			UpdateGoldRadioBannerNotification();
		}
		if ((parameter is QuestsPopup || parameter is ShopPopup) && type == "OnPopUpOpen")
		{
			Close();
		}
	}

	public override void Close()
	{
		Clear();
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (ButtonParentTarget.gameObject.TryGetComponent<UIScrollView>(out var component)) component.ResetPosition();
		}
		EventManager.NotifyClick("CloseRadio");
		base.Close();
		if (!IsLoadDataManager)
		{
			if (openBlackMarketOnNextClose)
			{
				openBlackMarketOnNextClose = false;
				ShopPopupHelper.OpenWithIndex(4);
			}
			CampHUD.SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: true);
		}
	}

	private void UpdatePhoneUI()
	{
		if (RadiophonesAmountLabel != null)
		{
			bool currencyScientificNotation = GameManager.Instance.gameEconomyData.ConfigData.CurrencyScientificNotation;
			int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value;
			if (currencyScientificNotation)
			{
				RadiophonesAmountLabel.text = Helpers.FormatNumber(value);
			}
			else
			{
				RadiophonesAmountLabel.text = value.ToString();
			}
		}
		if (GoldRadiophonesAmountLabel != null)
		{
			bool currencyScientificNotation2 = GameManager.Instance.gameEconomyData.ConfigData.CurrencyScientificNotation;
			int value2 = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GoldRadio).Value;
			if (currencyScientificNotation2)
			{
				GoldRadiophonesAmountLabel.text = Helpers.FormatNumber(value2);
			}
			else
			{
				GoldRadiophonesAmountLabel.text = value2.ToString();
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (RadiophonesAmountLabel != null)
		{
			bool currencyScientificNotation = GameManager.Instance.gameEconomyData.ConfigData.CurrencyScientificNotation;
			int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value;
			if (currencyScientificNotation)
			{
				RadiophonesAmountLabel.text = Helpers.FormatNumber(value);
			}
			else
			{
				RadiophonesAmountLabel.text = value.ToString();
			}
		}
		if (IsLoadDataManager && GoldMeterAmountLabel)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && GoldMeterAmountLabel)");
			GoldMeterAmountLabel.text = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value.ToString();
		}
		else
		{
			if (GoldRadiophonesAmountLabel != null)
			{
				bool currencyScientificNotation2 = GameManager.Instance.gameEconomyData.ConfigData.CurrencyScientificNotation;
				int value2 = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GoldRadio).Value;
				if (currencyScientificNotation2)
				{
					GoldRadiophonesAmountLabel.text = Helpers.FormatNumber(value2);
				}
				else
				{
					GoldRadiophonesAmountLabel.text = value2.ToString();
				}
			}
			if (BundlePanel != null)
			{
				BundlePanel.UpdateUI(GetRadioBundleOffer());
			}
			if (ClassUnlockPanel != null && BundlePanel != null)
			{
				if (BundlePanel.IsActive())
				{
					ClassUnlockPanel.gameObject.SetActive(value: false);
				}
				else
				{
					ClassUnlockPanel.UpdateUI();
				}
			}
			if (MaxLevelPanel != null)
			{
				MaxLevelPanel.UpdateUI();
			}
			if (FeaturedHeroBanner != null)
			{
				FeaturedHeroBanner.UpdateUI();
			}
			UpdateGoldRadioBannerNotification();
		}
		UpdateAllButtons();
	}

	public override void OnClickClose()
	{
		if (TutorialView.Allowed("CloseRadio"))
		{
			Close();
		}
	}

	public void EnableAllCallButtons(bool value)
	{
		if (OfflineManager.IsFreeAll && value == false) return;
		if (CallButtonsList == null)
		{
			return;
		}
		for (int i = 0; i < CallButtonsList.Count; i++)
		{
			if (CallButtonsList[i] != null)
			{
				CallButtonsList[i].SetIsEnabled(value);
			}
		}
	}

	private void InstantiateButtons()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (CallButtonsList != null && CallButtonsList.Count == 0)
		{
			for (int num = 2; num >= 0; num--)
			{
				PhoneCallDefinition phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(GameManager.Instance.playerModel.UtcTimeStamp, num);
				CreateNewButton(DropTypeToIndexArray[num], num, phoneCallDefinition);
			}
		}
		if (!(TutorialView.Instance == null) && TutorialView.Instance.Running)
		{
			return;
		}
		int phoneCallDefinitionMaxSlotNumber = gameEconomyData.GetPhoneCallDefinitionMaxSlotNumber();
		int count = 0;
		for (int i = 3; i <= phoneCallDefinitionMaxSlotNumber; i++)
		{
			PhoneCallDefinition phoneCallDefinition2 = gameEconomyData.GetPhoneCallDefinition(GameManager.Instance.playerModel.UtcTimeStamp, i);
			if (phoneCallDefinition2 != null)
			{
				if (phoneCallDefinition2.CanCallOwnedHeroes)
				{
					CreateNewButton(phoneCallDefinition2.DropType, i, phoneCallDefinition2);
					count++;
				}
				else if (!GameManager.Instance.playerModel.SurvivorContainer.HasUnLockedHero(phoneCallDefinition2))
				{
					CreateNewButton(phoneCallDefinition2.DropType, i, phoneCallDefinition2);
					count++;
				}
				else
				{
					DestroyButton(i);
				}
			}
			else
			{
				DestroyButton(i);
			}
		}
		DebugTWD.Log("Setup Additional Calls: " + count.ToString(), DebugType.Call);
		CallButtonsList = CallButtonsList?.OrderByDescending((RadioCallButton x) => x.SlotNumber).ToList();
	}

	private void DestroyButton(int slotNumber)
	{
		RadioCallButton buttonBySlotNumber = GetButtonBySlotNumber(slotNumber);
		if (buttonBySlotNumber != null)
		{
			Object.Destroy(buttonBySlotNumber.gameObject);
			CallButtonsList.Remove(buttonBySlotNumber);
		}
	}

	private void CreateNewButton(DropType dropType, int slotNumber, PhoneCallDefinition phoneCallDefinition = null)
	{
		if (ButtonParentTarget == null || ButtonParentTarget.gameObject == null)
		{
			DebugLogError("Could not instantiate target is NULL!");
			return;
		}
		if (phoneCallDefinition != null)
		{
			RadioCallButton buttonBySlotNumber = GetButtonBySlotNumber(slotNumber);
			if (buttonBySlotNumber != null)
			{
				if (buttonBySlotNumber.PhoneCallDefinition == phoneCallDefinition)
				{
					return;
				}
				DestroyButton(slotNumber);
			}
		}
		GameObject gameObject = Helpers.InstantiateToParent(CallButtonPrefab, ButtonParentTarget.gameObject);
		if (gameObject != null)
		{
			RadioCallButton component = gameObject.GetComponent<RadioCallButton>();
			if (component != null)
			{
				component.SetData(slotNumber, dropType, phoneCallDefinition);
				component.AddClickListener(RadioButtonClicked, slotNumber.ToString());
				CallButtonsList.Add(component);
			}
			else
			{
				DebugLogError("Could not find Component: RadioCallButton in prefab with name: " + CallButtonPrefab.name);
			}
		}
		else
		{
			DebugLogError("Could not instantiate prefab with name: " + CallButtonPrefab.name);
		}
	}

	private void PositionButtons()
	{
		float num = 0f;
		if (ButtonParentTarget != null)
		{
			UIPanel component = ButtonParentTarget.GetComponent<UIPanel>();
			if (component != null)
			{
				component.Update();
				num = component.width / 2f;
			}
		}
		RadioCallButton radioCallButton = null;
		if (CallButtonsList != null && CallButtonsList.Count > 0)
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < CallButtonsList.Count; i++)
			{
				radioCallButton = CallButtonsList[i];
				if (radioCallButton != null)
				{
					zero.x = (float)i * radioCallButton.localSize.x - num + radioCallButton.localSize.x / 2f;
					radioCallButton.SetPosition(zero);
					radioCallButton.UpdateUI();
					radioCallButton.gameObject.name = "Radio_Phone_Button_Slot_Number_" + radioCallButton.SlotNumber;
				}
			}
		}
		else
		{
			DebugLogError("CallButtonsList is NULL or Empty!");
		}
	}

	private void UpdateAllButtons()
	{
		RadioCallButton radioCallButton = null;
		if (CallButtonsList == null || CallButtonsList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < CallButtonsList.Count; i++)
		{
			radioCallButton = CallButtonsList[i];
			if (radioCallButton != null)
			{
				radioCallButton.UpdateUI();
			}
			radioCallButton = null;
		}
	}

	public void RadioButtonClicked(UIButtonExtended button)
	{
		DebugTWD.Log("OnClick RadioButtonClicked", DebugType.Call);

		int result = -1;
		if (button != null && int.TryParse(button.id, out result))
		{
			RadioCallButton buttonBySlotNumber = GetButtonBySlotNumber(result);
			if (buttonBySlotNumber != null)
			{
				DebugLog("Start call with dropType: " + buttonBySlotNumber.dropType);
				EnableAllCallButtons(value: false);
				if (IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (IsLoadDataManager)");
					int price = buttonBySlotNumber.GetCallPrice();
					RadioButtonClickedExecute(price, buttonBySlotNumber);
				}
				else
				{
					LastClickedTutorialArrowId = buttonBySlotNumber.GetTutorialArrowID();
					GameManager.Instance.PhoneCallResponseReceived = false;
					ConsumeCurrencyCommandUtils.Execute(new StartPhoneCallCommand
					{
						DropType = buttonBySlotNumber.dropType,
						CallSlotNumber = buttonBySlotNumber.SlotNumber,
						Cashier = GameManager.Instance.playerModel.PhoneCall.GetCashier(buttonBySlotNumber.dropType, buttonBySlotNumber.SlotNumber)
					}, OnPhoneCallMade);
				}
			}
		}
	}

	public void OnPhoneCallMade(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			if (!IsLoadDataManager)
			{
				GameManager.Instance.CheckConnectionReachability(showPopup: true, "StartPhoneCallCommand");
				if (!string.IsNullOrEmpty(LastClickedTutorialArrowId))
				{
					EventManager.NotifyClick(LastClickedTutorialArrowId);
				}
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallStarted();
				}
				PlayerModel playerModel = GameManager.Instance.playerModel;
				if (playerModel.PhoneCall.LootsList != null)
				{
					for (int i = 0; i < playerModel.PhoneCall.LootsList.Count; i++)
					{
						if (playerModel.PhoneCall.LootsList[i] != null && playerModel.PhoneCall.LootsList[i].GeneratedSurvivor != null)
						{
							ActorView.PrepareActor(GameManager.Instance.playerModel.PhoneCall.LootsList[i].GeneratedSurvivor);
						}
					}
				}
				Close();
				SelectSurvivorsPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectSurvivorPopup) as SelectSurvivorsPopup;
				obj.SkipClickIntro = false;
				obj.Open();
			}
			else
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				_CallListPanel.SetActive(false);
				survivorToggle.gameObject.SetActive(false);
				_SelectSurvivorsPopup.gameObject.SetActive(true);
				_SelectSurvivorsPopup.SkipClickIntro = false;
				_SelectSurvivorsPopup.Open();

				if (RadiophonesAmountLabel != null)
				{
					DebugTWD.Log("Update RadiophonesAmountLabel");
					RadiophonesAmountLabel.text = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value.ToString();
				}
				CallCraft.Instance.CalculateHeroTokenQueue();
			}
		}
		else
		{
			EnableAllCallButtons(value: true);
		}
	}

	private void OnClickedBundle(UIButtonExtended button)
	{
		if (BundlePanel != null && BundlePanel.CurrentDefinition != null)
		{
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				DataManager.Instance.BundleSource = Metrics.BundleSource.PhoneScreen;
			}
			else
			{
				GameManager.Instance.BundleSource = Metrics.BundleSource.PhoneScreen;
			}
			BundleCardPopup.OpenBundle(BundlePanel.CurrentDefinition.BundleIdentifier);
		}
		else
		{
			DebugLogError("Could not open bundle data was NULL");
		}
	}

	private void Clear()
	{
		if (BundlePanel != null)
		{
			BundlePanel.Clear();
		}
	}

	public RadioCallButton GetButtonBySlotNumber(int slotNumber)
	{
		if (CallButtonsList != null)
		{
			for (int i = 0; i < CallButtonsList.Count; i++)
			{
				if (CallButtonsList[i].SlotNumber == slotNumber)
				{
					return CallButtonsList[i];
				}
			}
		}
		return null;
	}

	public void OpenBlackMarketOnNextClose()
	{
		openBlackMarketOnNextClose = true;
	}

	public void OnClickSurvivor()
	{
		if (!IsLoadDataManager)
		{
			SetToggle(DrawCardMode.Survivor);
		}
		else
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			IsWeaponMode = !IsWeaponMode;
			SetToggle(IsWeaponMode ? (IsWeaponSkillMode ? DrawCardMode.GoldRadio : DrawCardMode.Weapon) : DrawCardMode.Survivor);
			if (!IsWeaponMode && _SelectWeaponPopup)
			{
				_SelectWeaponPopup.OnClickClose();
			}
		}
	}

	public void ShowPhone(bool flag)
	{
		Helpers.GameObjectSetActive(RadiophonesTop, flag);
		Helpers.GameObjectSetActive(GoldRadiophonesTop, !flag);
	}

	public void OnClickWeapon()
	{
		SetToggle(DrawCardMode.Weapon);
	}

	public void OnClickGoldRadio()
	{
		if (!IsLoadDataManager)
		{
			SetToggle(DrawCardMode.GoldRadio);
		}
		else
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			IsWeaponSkillMode = !IsWeaponSkillMode;
			SetToggle(IsWeaponSkillMode ? DrawCardMode.GoldRadio : DrawCardMode.Weapon);
		}
	}

	private void SetToggle(DrawCardMode drawCardMode)
	{
		string localizationKey = "";
		survivorContainer.alpha = 0f;
		weaponContainer.alpha = 0f;
        goldRadioContainer.alpha = 0f;
		survivorToggle.SetToggled(toggled: false);
		if (weaponToggle) weaponToggle.SetToggled(toggled: false);
		goldRadioToggle.SetToggled(toggled: false);
		goldRadioToggle.gameObject.SetActive(IsWeaponMode);
		WeaponAdvancedContent.SetActive(IsWeaponMode);
		Helpers.GameObjectSetActive(GoldRadiophonesTop, false);
		if (IsLoadDataManager) 
		{
			Helpers.GameObjectSetActive(GoldRadioWeaponContainer.RewardTypeSkillList.gameObject, false);
			Helpers.GameObjectSetActive(GoldRadioWeaponContainer.RewardStarsList.gameObject, false);
			Helpers.GameObjectSetActive(GoldRadioWeaponContainer.MaxCallsInput.transform.parent.gameObject, false);

			Helpers.GameObjectSetActive(PhoneWeaponContainer.RewardTypeList.gameObject, false);
			Helpers.GameObjectSetActive(PhoneWeaponContainer.MaxCallsInput.transform.parent.gameObject, false);
		}

		switch (drawCardMode)
		{
		case DrawCardMode.Survivor:
			survivorContainer.alpha = 1f;
			survivorToggle.SetToggled(toggled: true);
			if (!IsLoadDataManager) ShowPhone(flag: true);
			localizationKey = "NewPhonePopup.PhoneWeapon.Weapon";
			CallCraft.Instance.CalculateHeroTokenQueue();
			CallCraft.Instance.SetSatetButtonToCall();
			break;
		case DrawCardMode.Weapon:
			weaponContainer.alpha = 1f;
			if (weaponToggle) weaponToggle.SetToggled(toggled: true);
			if (IsLoadDataManager)
			{
				PhoneWeaponContainer.SetRewardTypeListValue();
				if (PhoneWeaponContainer.IsAuto) 
				{
					Helpers.GameObjectSetActive(PhoneWeaponContainer.RewardTypeList.gameObject, true);
					Helpers.GameObjectSetActive(PhoneWeaponContainer.MaxCallsInput.transform.parent.gameObject, true);
				}
			}
			UIEvent.Send("PhoneWeaponChanged");
			localizationKey = "NewPhonePopup.PhoneWeapon.Survivor";
			break;
		case DrawCardMode.GoldRadio:
			goldRadioContainer.alpha = 1f;
            goldRadioToggle.SetToggled(toggled: true);
			if (IsLoadDataManager) 
			{
				GoldRadioWeaponContainer.SetRewardTypeListValue();
				GoldRadioWeaponContainer.TweenFavoriteSign(true);
				if (GoldRadioWeaponContainer.IsAuto)
				{
					Helpers.GameObjectSetActive(GoldRadioWeaponContainer.RewardTypeSkillList.gameObject, true);
					Helpers.GameObjectSetActive(GoldRadioWeaponContainer.MaxCallsInput.transform.parent.gameObject, true);
				}
				Helpers.GameObjectSetActive(GoldRadioWeaponContainer.RewardStarsList.gameObject, true);
			}
			Helpers.GameObjectSetActive(GoldRadiophonesTop, true);
			UIEvent.Send("PhoneGoldRadioChanged");
			localizationKey = "GoldRadio_EquipSkill";
			break;
		}
		if (IsLoadDataManager)
		{
			var toggle = IsWeaponMode ? (IsWeaponSkillMode ? goldRadioToggle : survivorToggle) : survivorToggle;
			LocalizationUIUpdater local = toggle.GetComponentInChildren<LocalizationUIUpdater>();
			if (local)
			{
				local.LocalizationKey = localizationKey;
				local.UpdateContent();
			}
		}
	}

	public void ResetScroll()
	{
		UIScrollView componentInChildren = ButtonParentTarget.GetComponentInChildren<UIScrollView>();
		if (componentInChildren != null)
		{
			componentInChildren.ResetPosition();
		}
	}

	public void OnClickPhonePlus()
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.OpenDialogByItem("Phone");
		}
	}

	public void OnClickGoldPhonePlus()
	{
		ShopPopupHelper.OpenWithIndex(2);
	}

	public void UpdateGoldRadioBannerNotification()
	{
		if (GameManager.Instance != null)
		{
			GameManager instance = GameManager.Instance;
			if (instance.playerModel != null && instance.playerModel.EquipPrizeWheelModel != null)
			{
				bool value = instance.playerModel.EquipPrizeWheelModel.ShouldShowGoldRadioPoolRedDot();
				Helpers.GameObjectSetActive(newGoldRadioBannerNotificationContainer, value);
			}
		}
	}



	#region myparams
	public static NewPhonePopup Instance;
	public UILabel GoldMeterAmountLabel;
	[SerializeField]
	private UILabel ZoomLabel;
	public bool IsWeaponMode { get; private set; }
	public bool IsWeaponSkillMode { get; private set; }

	public List<string> FavoriteModSkillList { get; set; } = new List<string>();
	//public List<string> EquipRewardsID { get; set; } = new List<string>();
	private Vector3 buttonParentTargetOriginalPosition = Vector3.zero;
	public UIButtonExtended ButtonDelete;
	public bool IsInitDone;

	public PhoneWeaponContainer PhoneWeaponContainer => phoneWeaponContainer;

	public GoldRadioWeaponContainer GoldRadioWeaponContainer => goldRadioWeaponContainer;
	public SelectSurvivorsPopup _SelectSurvivorsPopup;
	public SelectWeaponsPopup _SelectWeaponPopup { get; private set; }
	public GameObject _CallListPanel;

	public UILabel CallChangeLabel;
	public TokenSelectorButton tokenButton;
	public bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public GameObject WeaponAdvancedContent;

	public float widthMult = 1;
	public float heightMult = 1;
	#endregion

	#region mycode
	public int CallPrice(PayButton payButton = null)
	{
		int price = 0;
		if (payButton != null)
		{
			price = payButton.radioPrice;
			DebugTWD.Log("PayButton.radioPrice : " + price);
		}
		return price;
	}

	public RadioCallButton GetButtonByPrice(int price)
	{
		if (CallButtonsList != null)
		{
			foreach (var bt in CallButtonsList)
			{
				if (bt.GetCallPrice() == price) return bt;
			}
		}
		return null;
	}

	public void NextCall()
	{
		DebugTWD.Log("OnClick NextCall", DebugType.Call);

		RadioCallButton buttonBySlotNumber;
		int price;

		if (CallCraft.Instance.SelectedCall != null && _SelectSurvivorsPopup.gameObject.activeSelf)
		{
			buttonBySlotNumber = CallCraft.Instance.SelectedCall.CallButton;
			price = CallCraft.Instance.SelectedCall.CallPrice;
			if (buttonBySlotNumber == null) return;

			CallCraft.Instance.CurrentCallButton = buttonBySlotNumber;
		}
		else
		{
			buttonBySlotNumber = CallCraft.Instance.CurrentCallButton;

			if (buttonBySlotNumber == null)
			{
				buttonBySlotNumber = CallCraft.Instance.CallDataList.Last().ButtonBySlotNumber;
			}
			if (buttonBySlotNumber == null) return;

			price = buttonBySlotNumber.GetCallPrice();
		}
		CallCraft.Instance.InitCallData = new CallCraft.CallInfo(buttonBySlotNumber);

		if (GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value - price < 0 && !OfflineManager.IsFreeAll)
		{
			DebugTWD.LogWarning("Not enough money to call");
			MyTools.OpenAlert("Not enough radios to call");
			return;
		}

		_SelectSurvivorsPopup.Clear();
		_SelectSurvivorsPopup.FinishCall(true);
		Helpers.DestroyAllChildren(_SelectSurvivorsPopup.ButtonParentTarget.gameObject);

		CallCraft.Instance.StopAllCoroutines();

		CallCraft.Instance.OnClickCall();

		TWDModelResult resultCall = GameManager.Instance.playerModel.PhoneCall.Call(buttonBySlotNumber.dropType, buttonBySlotNumber.SlotNumber);
		OnPhoneCallMade(resultCall);
	}

	public void SetWeaponPopup(SelectWeaponsPopup popup)
	{
		_SelectWeaponPopup = popup;
	}

	public void RadioButtonClickedExecute(int price, RadioCallButton buttonBySlotNumber)
	{
		bool isAnotherSlot = CallCraft.Instance.CurrentCallButton != buttonBySlotNumber;
		CallCraft.Instance.StopAllCoroutines();
		CallCraft.Instance.CurrentCallButton = buttonBySlotNumber;
		CallCraft.Instance.InitCallData = new CallCraft.CallInfo(buttonBySlotNumber);

		if (CallCraft.Instance.IsMultiCallMode)
		{
			StartCoroutine(CallCraft.Instance.OnClickMultiCall());
		}
		else
		{
			if (GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value - price < 0 && !OfflineManager.IsFreeAll)
			{
				DebugTWD.LogWarning("Not enough money to call");
				return;
			}

			if (isAnotherSlot)
			{
				NextCall();
				return;
			}
			CallCraft.Instance.OnClickCall();
			if (!OfflineManager.IsFakeExecuteCommands)
			{
				ConsumeCurrencyCommandUtils.Execute(new StartPhoneCallCommand
				{
					DropType = buttonBySlotNumber.dropType,
					CallSlotNumber = buttonBySlotNumber.SlotNumber,
					Cashier = GameManager.Instance.playerModel.PhoneCall.GetCashier(buttonBySlotNumber.dropType, buttonBySlotNumber.SlotNumber)
				}, OnPhoneCallMade);
			}
			else
			{
				TWDModelResult resultCall = GameManager.Instance.playerModel.PhoneCall.Call(buttonBySlotNumber.dropType, buttonBySlotNumber.SlotNumber);
				OnPhoneCallMade(resultCall);
			}
		}
	}

	public void EnableAllCallButtonsByPrice()
	{
		if (CallButtonsList == null || CallButtonsList.Count == 0)
		{
			return;
		}
		int radios = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value;
		for (int i = 0; i < CallButtonsList.Count; i++)
		{
			if (CallButtonsList[i] != null)
			{
				int price = CallPrice(CallButtonsList[i].GetComponent<PayButton>());
				CallButtonsList[i].SetIsEnabled(radios - price > 0);
			}
		}
	}

	public void ZoomCards(UIScrollBar scrollBar)
	{
		float scale = scrollBar.value / 2 * widthMult + .75f;
		ZoomLabel.text = Mathf.Round(scale * 100).ToString();

		ScaleGrid(scale);
	}

	public void ScaleGrid(float scale)
	{
		if (_SelectWeaponPopup)
		{
			var tweenScale = _SelectWeaponPopup.ButtonParentTarget.GetComponent<TweenScale>();
			tweenScale.to = Vector3.one * scale;
			tweenScale.PlayForward();
		}
	}
	#endregion
}
