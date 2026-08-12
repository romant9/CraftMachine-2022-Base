using BaseModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class SurvivorManagementPopUp : HUDElement
{
	public SurvivorsListPanel survivorsList;

	public SurvivorClassFilter survivorClassFilter;

	[Header("Tab Stuff")]
	[SerializeField]
	private GameObject supportsTabObject;

	[SerializeField]
	private UITabs tabsObject;

	[SerializeField]
	private GameObject survivorsTabContentObject;

	[SerializeField]
	private GameObject supportTabContentObject;

	[SerializeField]
	private GameObject teamsTabButton;

	[SerializeField]
	private GameObject particleEffectsMask;

	[SerializeField]
	private GameObject survivalManualTabButton;

	[SerializeField]
	private UILabel survivalManualNoticeNum;

	[Header("Buy More Slots Stuff")]
	[SerializeField]
	private UILabel slotsLabel;

	[SerializeField]
	private PayButton slotBuyButton;

	[SerializeField]
	private GameObject slotBuyTooltipButton;

	[Header("Badges")]
	[SerializeField]
	private GameObject survivorBadge;

	[SerializeField]
	private UILabel survivorBadgeLabel;

	[SerializeField]
	private GameObject supportBadge;

	[SerializeField]
	private UILabel supportBadgeLabel;

	private IInterceptor interceptor;

	private int previousTab;

	public SurvivorModel SurvivorModel { get; set; }

	public bool IsAcceptingSurvivor { get; set; }

	public override void Open()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (!gameObject.activeSelf) { return; }
			IsInitDone = true;
			DebugTWD.Log("SurvivorsPopup Open");
		}
		base.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_trainingground");
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player != null)
		{
			if (survivorClassFilter != null)
			{
				for (int i = 0; i < 6; i++)
				{
					survivorClassFilter.EnableButtonForClass((SurvivorClass)i, player.SurvivorContainer.GetSurvivorsOfClass((SurvivorClass)i).Count > 0);
				}
				survivorClassFilter.OnClassFilterSelected += OnClassFilterButtonClicked;
				survivorClassFilter.UpdatePositionAndState();
			}
			RefreshSlots();
		}
		tabsObject.gameObject.SetActive(!IsAcceptingSurvivor);
		if (IsAcceptingSurvivor)
		{
			tabsObject.SelectTab(0);
		}
		else if (tabsObject.CurrentTabIndex != previousTab)
		{
			tabsObject.SelectTab(previousTab);
		}
		UpdateUI();
		Helpers.GameObjectSetActive(particleEffectsMask, value: !OfflineManager.IsNoEffects);
	}

	public override async void Close()
	{
		bool flag = interceptor == null;
		if (!flag)
		{
			flag = await interceptor.Intercept();
		}
		if (flag)
		{
			UIEvent.Send("OnSurvivorInfoClosed");
			base.Close();
			if (!IsAcceptingSurvivor)
			{
				previousTab = tabsObject.CurrentTabIndex;
			}
			IsAcceptingSurvivor = false;
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		if (survivorsList != null && survivorClassFilter != null)
		{
			survivorClassFilter.SurvivorList = survivorsList;
		}
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player != null)
		{
			player.Changed += OnPlayerModelChanged;
		}
		if (player != null && player.Camp != null)
		{
			player.Camp.Changed += CampModelChanged;
		}
	}

	public void OnPlayerModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "currencyChangedEvent")
		{
			if (args is CurrencyModel { Type: CurrencyType.Diamonds })
			{
				UpdateUI();
			}
			else if (changed == "addSurvivor" || changed == "survivorDemoted")
			{
				RecalculateSurvivorsToUpgradeCount();
			}
		}
	}

	private void CampModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "EventLevelUpBuilding" || changed == "EventUpgradeBuilding")
		{
			RecalculateSurvivorsToUpgradeCount();
		}
	}

	private void RecalculateSurvivorsToUpgradeCount()
	{
		GameObject gameObject = null;
		if (!(survivorClassFilter != null))
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			gameObject = survivorClassFilter.GetButtonForClass((SurvivorClass)i);
			if (gameObject != null)
			{
				SurvivorClassButton component = gameObject.GetComponent<SurvivorClassButton>();
				if (component != null && i != 6)
				{
					component.NotificationCount = GameManager.Instance.playerModel.SurvivorContainer.GetUpgradeableSurvivorsOfClass((SurvivorClass)i).Count;
				}
			}
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player != null)
		{
			player.Changed -= OnPlayerModelChanged;
		}
		if (player != null && player.Camp != null)
		{
			player.Camp.Changed -= CampModelChanged;
		}
		if (this.survivorClassFilter != null)
		{
			SurvivorClassFilter survivorClassFilter = this.survivorClassFilter;
			if (survivorClassFilter != null)
			{
				survivorClassFilter.OnClassFilterSelected -= OnClassFilterButtonClicked;
			}
		}
	}

	private void OnClassFilterButtonClicked(SurvivorClass selectedClass)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnNewSurvivorSelected":
			if (tabsObject.CurrentTabIndex == 0 && parameter is SurvivorModel survivorModel)
			{
				SurvivorModel = survivorModel;
                SurvivorInfoPopup survivorInfoPopup2;

                if (!IsLoadDataManager)
                {
                    survivorInfoPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
                }
                else
                {
					DebugTWD.LogMycode("if (IsLoadDataManager)");
                    if (SurvivorInfoPopupCurrent == null)
                    {
						//SurvivorInfoPopupCurrent = Instantiate(_SurvivorInfoPopupPrefab, ResidencePopup.Instance.transform);
						SurvivorInfoPopupCurrent = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup, ResidencePopup.Instance.gameObject, prefabVariant: _SurvivorInfoPopupPrefab.gameObject) as SurvivorInfoPopup;
                    }
                    survivorInfoPopup2 = SurvivorInfoPopupCurrent;
                    if (AdjustedLevel != null)
                    {
                        AdjustedLevel.text = "GW Level : " + GWTeamUtils.GetAdjustedLevel(SurvivorModel).ToString();
                    }
                    string isHero = "Survivor is Hero {isHero} : " + (SurvivorModel.IsHero ? 1 : 0);
                    string SurvivorLevel = "Survivor Level {level}: " + SurvivorModel.Level;
                    string SurvivorRarityLevel = "Survivor Rarity Level {rarlevel} : " + SurvivorModel.SurvivorRarityLevel;
                    string text = isHero + "\n" + SurvivorLevel + "\n" + SurvivorRarityLevel + "\n" + "AdjustedLevel = " +
                            "{level} + {isHero} * " + GameManager.Instance.gameEconomyData.GuildWarConfig.HeroLevelEq + " + Max(0, {rarlevel} - 4) * "
                        + GameManager.Instance.gameEconomyData.GuildWarConfig.PinkLevelEq;
                    TooltipLevel.EnCustomText = text + "\nResult is round to integer.";
                    TooltipLevel.RuCustomText = text + "\nРезультат округляем до целого.";
                }
                if (IsAcceptingSurvivor)
				{
					survivorInfoPopup2.currentStateMachineState = SurvivorInfoStateBase.States.SurvivoreRejectOnly;
				}
				else
				{
					survivorInfoPopup2.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
				}
				SurvivorFilterList currentSurvivorFilterList2 = new SurvivorFilterList(SurvivorInfoPopup.GetSurvivorsFromCards(survivorsList.GetCards()));
				survivorInfoPopup2.OpenForModel(SurvivorModel, currentSurvivorFilterList2);
                if (IsLoadDataManager)
				{
                    survivorInfoPopup2.ShowBadges();
                    survivorInfoPopup2.survivorRightSidePanel.GetButton(0).ForceClick();
                    survivorInfoPopup2.shareButton.transform.parent.gameObject.SetActive(false);                   
                }
            }
			return;
		case "SurvivorHeroPreviewSelected":
			if (parameter != null)
			{
				SurvivorModel = parameter as SurvivorModel;
				SurvivorInfoPopup survivorInfoPopup;

                if (!IsLoadDataManager)
                {
                    survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
                }
                else
                {
                    DebugTWD.LogMycode("if (IsLoadDataManager)");
                    if (SurvivorInfoPopupCurrent == null)
                    {
                        SurvivorInfoPopupCurrent = Instantiate(_SurvivorInfoPopupPrefab, ResidencePopup.Instance.transform);
                    }
                    survivorInfoPopup = SurvivorInfoPopupCurrent;
				}
				survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorHeroPreview;
				List<UIListCard<SurvivorModel>> cards = survivorsList.GetCards();
				List<SurvivorCardHeroLocked> list = new List<SurvivorCardHeroLocked>(cards.Count);
				for (int i = 0; i < cards.Count; i++)
				{
					list.Add(cards[i] as SurvivorCardHeroLocked);
				}
				LockedHeroSurvivorFilterList currentSurvivorFilterList = new LockedHeroSurvivorFilterList(SurvivorInfoPopup.GetSurvivorsFromCards(list));
				survivorInfoPopup.OpenForModel(SurvivorModel, currentSurvivorFilterList);
				if (IsLoadDataManager)
				{
                    survivorInfoPopup.ShowBadges();
                    survivorInfoPopup.survivorRightSidePanel.GetButton(0).ForceClick();
                }
            }
			return;
		case "OnSurvivorUpgradeStarted":
			Close();
			return;
		case "SurvivorDeleted":
			UpdateUI();
			return;
		case "SurvivorCardEquipmentClicked":
		{
			if (IsLoadDataManager)
			{
				if (SurvivorInfoPopupCurrent != null && SurvivorInfoPopupCurrent.gameObject.activeSelf) return;
			}
			else
			{
				if (SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup).IsOpen) return;
			}
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton.GetEquipment() != null)
			{
				SurvivorCard cardFromSurvivor = survivorsList.GetCardFromSurvivor(equipmentButton.GetOwningSurvivor());
				if (cardFromSurvivor != null)
				{
                    DebugTWD.Log("SurvivorCardEquipmentClicked ManagementPopup", DebugType.OnClick);
                    base.gameObject.GetComponent<EquipmentSelectionContainerView>().OpenForSurvivorCard(cardFromSurvivor, equipmentButton);
				}
				UpdateUI();
			}
			return;
		}
		case "OnSurvivorInstantUpgraded":
		case "OnSurvivorRenamed":
			if (survivorsList != null)
			{
				survivorsList.RefreshCards();
			}
			RecalculateSurvivorsToUpgradeCount();
			return;
		case "OnSurvivorUpgradeComplete":
			RecalculateSurvivorsToUpgradeCount();
			return;
		case "SurvivorExtraSlotBought":
			if (survivorsList != null)
			{
				survivorsList.RefreshCards();
			}
			return;
		case "SurvivorListRefreshed":
			UpdateUI();
			return;
		case "SurvivorPortraitUpdated":
			if (survivorsList != null)
			{
				survivorsList.RefreshCards();
			}
			return;
		case "OnBattlePassOpened":
			Close();
			return;
		case "OnPopUpOpen":
			if (!(parameter is SurvivorManagementPopUp))
			{
				Helpers.GameObjectSetActive(particleEffectsMask, value: false);
				return;
			}
			break;
		}
		if (type == "OnPopUpClose" && !(parameter is SurvivorManagementPopUp) && SingularityMonoBehaviour<HUDManager>.Instance.OpenPopups.Count <= 2)
		{
			Helpers.GameObjectSetActive(particleEffectsMask, value: !OfflineManager.IsNoEffects);
		}
	}

	public override void UpdateUI()
	{
		if (!IsLoadDataManager)
		{
            supportsTabObject.SetActive(GameManager.Instance.playerModel.CouncilLevel >= GameManager.Instance.gameEconomyData.EndlessModeConfig.CouncilLockLevel);
        }
        else
        {
			DebugTWD.LogMycode("if (IsLoadDataManager)");
            supportsTabObject.SetActive(false);
        }
        foreach (UIListCard<SurvivorModel> card in survivorsList.GetCards())
		{
			SurvivorCard survivorCard = card as SurvivorCard;
			if (survivorCard != null)
			{
				survivorCard.Type = ((!IsAcceptingSurvivor) ? SurvivorCard.CardType.TrainingGround : SurvivorCard.CardType.TrainingGroundAcceptingSurvivor);
				survivorCard.UpdateUI();
				survivorCard.ShowTrainingGroundsInfo(IsAcceptingSurvivor);
			}
		}
		RecalculateSurvivorsToUpgradeCount();
		RefreshBadges();
		if (!IsLoadDataManager)
		{
			Helpers.GameObjectSetActive(teamsTabButton, TeamPresetHelpers.IsFeatureUnlocked(GameManager.Instance.playerModel));
			Helpers.GameObjectSetActive(survivalManualTabButton, Helpers.IsSurvivalManualShow());
			Helpers.GameObjectSetActive(survivalManualTabButton.transform.Find("lock").gameObject, !Helpers.IsSurvivalManualOpen());
			GameObject obj = survivalManualTabButton.transform.Find("notification").gameObject;
			if (Helpers.IsSurvivalManualOpen() && Helpers.IsSurvivalManualShow())
			{
				Helpers.GameObjectSetActive(obj, Helpers.GetRedSurvivalManualNum() > 0);
				survivalManualNoticeNum.text = Helpers.GetRedSurvivalManualNum().ToString();
			}
			else
			{
				Helpers.GameObjectSetActive(obj, value: false);
			}
		}
		else
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			teamsTabButton.SetActive(false);
		}
	}

	public void SetSurvivorListPanelVisibility(bool visibility)
	{
		if ((bool)survivorsList.gameObject)
		{
			survivorsList.gameObject.SetActive(visibility);
		}
	}

	private void RefreshSlots()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		slotsLabel.text = LocalizationManager.GetText("Popup.TrainingGround.CharacterSlots{Parameters}", playerModel.SurvivorContainer.Survivors.Count, playerModel.SurvivorContainer.SurvivorSlotsCount);
		bool flag = playerModel.SurvivorContainer.CanPurchaseMoreSlots();
		slotBuyButton.GetComponent<UIButton>().isEnabled = flag;
		slotBuyTooltipButton.SetActive(!flag);
		if (flag)
		{
			slotBuyButton.UpdateUI(playerModel.SurvivorContainer.GetPurchaseNextSlotsLevelCashier(), LocalizationManager.GetText("Popup.TrainingGround.BuyMoreSurvivorSlots"));
		}
	}

	public void MoreSlotsClick()
	{
		Cashier purchaseNextSlotsLevelCashier = GameManager.Instance.playerModel.SurvivorContainer.GetPurchaseNextSlotsLevelCashier();
		if (purchaseNextSlotsLevelCashier == null)
		{
			return;
		}
		if (purchaseNextSlotsLevelCashier.CanAfford())
		{
			ConsumeCurrencyCommandUtils.Execute(new BuyMoreSurvivorSlotsCommand
			{
				Cashier = purchaseNextSlotsLevelCashier
			}, delegate
			{
				RefreshSlots();
			});
		}
		else
		{
			ShopPopupHelper.OpenForMissingCurrencyWithMissingAmount(purchaseNextSlotsLevelCashier.GetMissing(CurrencyType.Diamonds));
		}
	}

	public void MoreSlotsTooltipClick()
	{
		TooltipManager.OpenTextBoxWithText(slotBuyTooltipButton, LocalizationManager.GetText("Popup.TrainingGround.BuyMoreSurvivorSlots.Button.DisabledTooltip"));
	}

	public void RefreshBadges()
	{
		int count = GameManager.Instance.playerModel.SurvivorContainer.GetUpgradeableSurvivors().Count;
		int upgradableSupportCount = GameManager.Instance.playerModel.GetUpgradableSupportCount();
		survivorBadge.SetActive(count > 0);
		supportBadge.SetActive(upgradableSupportCount > 0);
		survivorBadgeLabel.text = count.ToString();
		supportBadgeLabel.text = upgradableSupportCount.ToString();
	}

	public void SetInterceptor(IInterceptor i)
	{
		interceptor = i;
	}

	public void UpdateSurvivalManual()
	{
		if (survivalManualTabButton != null)
		{
			Helpers.GameObjectSetActive(survivalManualTabButton.transform.Find("lock").gameObject, Helpers.IsSurvivalManualOpen());
		}
	}

	public void OnClickSurvivalManualButton()
	{
		if (!Helpers.IsSurvivalManualOpen())
		{
			AlertPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
			obj.SetContent("", Helpers.GetSurvivalManualNotOpenTips());
			obj.Open();
			return;
		}
		HUDElement hUDElement = null;
		hUDElement = ((!Helpers.IsSurvivalManualPlotGuidenOpened()) ? ((HUDElement)(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualPlotGuidePopup) as SurvivalManualPlotGuidePopup)) : ((HUDElement)(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualMainPopup) as SurvivalManualMainPopup)));
		if (hUDElement != null)
		{
			hUDElement.Open();
			OnClickClose();
			if (CampManager.Instance != null)
			{
				CampManager.Instance.FullscreenPopupShowCamp(SingularityMonoBehaviour<HUDManager>.Instance.CanEnableCamp(UIType.SurvivalManualMainPopup));
			}
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
    public bool IsInitDone;

    public SurvivorInfoPopup _SurvivorInfoPopupPrefab;

    public SurvivorInfoPopup SurvivorInfoPopupCurrent { get; set; }
    public EquipmentUpgradePopup EquipmentUpgradePopupCurrent { get; set; }
    public int remodelTraitIndexCurrent { get; set; }

    public SurvivorCard SurvivorCardCurrent { get; set; }
    public UILabel AdjustedLevel;
    public ShowTooltip TooltipLevel;
    public Transform SurvivorCardParent;
    public TraitInfoPopup traitInfoPopup;
    public GameObject survivorCardSelected { get; set; }

    public bool IsTraitRerollFree;
    public bool IsOpenTraitsTree { get; set; }

    public SurvivorTraitTree survivorTraitTree;
    public RemodelTraitsTree remodelTraitsTree;

    public Dictionary<string, SurvivorTraits> survivorTraitsList = new Dictionary<string, SurvivorTraits>();

    public Dictionary<string, SurvivorTraits> survivorTraitsListTree = new Dictionary<string, SurvivorTraits>();

    public Dictionary<string, SurvivorTraits> EqipmentTraitsList = new Dictionary<string, SurvivorTraits>();

    public int rerollTraitIndexCurrent { get; set; }

    public bool EquipmentButtonClicked { get; set; }
    #endregion

    #region mycode
    public void ResetTraitsData(bool isTree = false)
    {
        var list = isTree ? survivorTraitsListTree : survivorTraitsList;

        if (list.Count > 0)
        {
            rerollTraitIndexCurrent = -1;

            DebugTWD.Log("Reset all rerolls");
            var survivorTraitsListValues = list.Values.ToList();

            for (int i = 0; i < survivorTraitsListValues.Count; i++)
            {
                if (survivorTraitsListValues[i].UpgradeTraits != null)
                {
                    List<UpgradeTraitsData> originUpgradeTraits = new List<UpgradeTraitsData>();

                    foreach (var trait in survivorTraitsListValues[i].UpgradeTraits)
                    {
                        originUpgradeTraits.Add(trait);
                    }
                    survivorTraitsListValues[i].Survivor.UpgradeTraits = originUpgradeTraits;
                    survivorTraitsListValues[i].Survivor.TraitRandom = new ModelRandom(survivorTraitsListValues[i].random);

                    survivorTraitsListValues[i].Survivor.RandomTraitsFromReroll = survivorTraitsListValues[i].RandomTraitsFromReroll;
                    survivorTraitsListValues[i].Survivor.PreviousRandomRolledTraits = survivorTraitsListValues[i].PreviousRandomRolledTraits;

                    survivorTraitsListValues[i].Survivor.TraitRandom = new ModelRandom(survivorTraitsListValues[i].random);

                    survivorTraitsListValues[i].Survivor.RandomTraitsFromReroll = survivorTraitsListValues[i].RandomTraitsFromReroll;
                    survivorTraitsListValues[i].Survivor.PreviousRandomRolledTraits = survivorTraitsListValues[i].PreviousRandomRolledTraits;
                }
            }
            list.Clear();
            SurvivorInfoPopupCurrent.survivorTraitsList.UpdateWith(SurvivorInfoPopupCurrent.survivorModel);
            SurvivorInfoPopupCurrent.survivorModel.TraitToBeRerolledCandidate = null;
        }

        var listEquip = EqipmentTraitsList;

        if (listEquip.Count > 0)
        {
            foreach (var equipment in listEquip)
            {
                var eqipmentDe = OfflineManager.JsonSerializer.Deserialize<EquipmentItemModel>(equipment.Value.equipmentItemModel);
                var equipmentOrigin = GameManager.Instance.playerModel.Equipment.ChangeEqupmentModel(eqipmentDe, out bool isWeapon);
                var survivor = equipment.Value.Survivor;

                if (survivor != null)
                {
                    survivor.EquipmentItems.Models[isWeapon ? 1 : 0] = equipmentOrigin;
                }
            }
            listEquip.Clear();

            GameManager.Instance.playerModel.PlayerRandom = new ModelRandom(PlayerRandomValues.Instance.PlayerRandomInit);
            StartCoroutine(UpdateTraits());
        }
    }
    public IEnumerator UpdateTraits()
    {
        yield return new WaitForEndOfFrame();

        UpdateUI();
        if (survivorCardSelected != null)
            survivorCardSelected.GetComponent<SurvivorCard>().OnCardClicked();
    }
    public void BackupTraitsData(SurvivorModel survivorModel, TraitDefinition traitDefinition, int traitIndex, bool isTree = false)
    {
        Dictionary<string, SurvivorTraits> list = isTree ? survivorTraitsListTree : survivorTraitsList;

        string name = survivorModel.IsHero ? survivorModel.FullName : survivorModel.SurvivorName;

        List<UpgradeTraitsData> originUpgradeTraits = new List<UpgradeTraitsData>();

        if (!list.ContainsKey(name))
        {
            DebugTWD.Log("Backup survivor TraitData All : " + name + " , " + traitDefinition.Identifier + " " + traitIndex);

            foreach (var trait in survivorModel.UpgradeTraits)
            {
                originUpgradeTraits.Add(trait);
            }
            list.Add(name, new SurvivorTraits()
            {
                Survivor = survivorModel,
                UpgradeTraits = originUpgradeTraits,

                random = new ModelRandom(survivorModel.TraitRandom),
                traitDefinitionCurrent = traitDefinition,

                RandomTraitsFromReroll = survivorModel.RandomTraitsFromReroll,
                PreviousRandomRolledTraits = survivorModel.PreviousRandomRolledTraits
            });
        }
        else
        {
            list.TryGetValue(name, out SurvivorTraits item);
            if (item.UpgradeTraits == null)
            {
                DebugTWD.Log("Backup survivor TraitData : " + name + " , " + traitDefinition.Identifier + " " + traitIndex);

                item.UpgradeTraits = originUpgradeTraits;
                item.random = new ModelRandom(survivorModel.TraitRandom);
                item.traitDefinitionCurrent = traitDefinition;
                item.RandomTraitsFromReroll = survivorModel.RandomTraitsFromReroll;
                item.PreviousRandomRolledTraits = survivorModel.PreviousRandomRolledTraits;
            }
        }
        StartCoroutine(WaitForBackup(traitIndex, name, isTree));
    }
    public void BackupTraitsData(EquipmentItemModel model, SurvivorModel survivor = null)
    {
        Dictionary<string, SurvivorTraits> list = EqipmentTraitsList;

        if (list == null) { return; }

        string name = model.IdForAnalytics;
        if (!list.ContainsKey(name))
        {
            DebugTWD.Log("Backup Equipment TraitData All : " + model.EquipmentDefinitionIdentifier);
            var eqipmentTraitsString = OfflineManager.JsonSerializer.Serialize(model);
            DebugTWD.Log("OriginModelSerialised size is: " + (Encoding.Default.GetBytes(eqipmentTraitsString).Length * 1024) + "kb");

            if (list.Count == 0)
            {
                list.Add(name, new SurvivorTraits()
                {
                    equipmentItemModel = eqipmentTraitsString,
                    equiRandom = new ModelRandom(GameManager.Instance.playerModel.PlayerRandom),
                });
            }
            else
            {
                list.Add(name, new SurvivorTraits()
                {
                    equipmentItemModel = eqipmentTraitsString,
                });
            }
            var owner = model.Owner;
            if (survivor != null) list.Last().Value.Survivor = survivor;
            else if (owner != null) list.Last().Value.Survivor = (SurvivorModel)owner;
        }
    }

    private IEnumerator WaitForBackup(int traitIndex, string name, bool isTree = false)
    {
        yield return new WaitForEndOfFrame();

        Dictionary<string, SurvivorTraits> list = isTree ? survivorTraitsListTree : survivorTraitsList;

        list[name].TraitRerolledList[traitIndex] = true;
    }

    public void SetFree(UIToggle toggle)
    {
        IsTraitRerollFree = toggle.value;
    }
    #endregion
}
