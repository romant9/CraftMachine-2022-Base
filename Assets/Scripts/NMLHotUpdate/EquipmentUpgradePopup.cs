using BaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class EquipmentUpgradePopup : HUDElement
{
	[SerializeField]
	private UITexture weaponIcon;

	[SerializeField]
	private UITexture armorIcon;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel survivorRequiredLabel;

	[SerializeField]
	private UILabel equipmentRarityLabel;

	[SerializeField]
	private UISprite statIcon;

	[SerializeField]
	private GameObject StateSPRemoldContainer;

	[SerializeField]
	private UILabel SPtraitRateLabel;

	[SerializeField]
	private UILabel statAmount;

	[SerializeField]
	private UILabel firstTraitLabel;

	[SerializeField]
	private UISprite firstTraitIcon;

	[SerializeField]
	private UISprite firstTraitBG;

	[SerializeField]
	private TooltipButton firstTraitButton;

	[SerializeField]
	private TraitsPanel traitsPanel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UIWidget background;

	[SerializeField]
	private UISprite[] starsSpriteArray;

	[SerializeField]
	private GameObject statsContainer;

	[SerializeField]
	private GameObject chargeContainer;

	[SerializeField]
	private UIButton specialDescriptionButton;

	[SerializeField]
	private UIButton spRemoldDescriptionButton;

	[SerializeField]
	private UIButton spRemoldButton;

	[SerializeField]
	private GameObject apocalypticButton;

	[SerializeField]
	private GameObject apocalypticPlus;

	[SerializeField]
	private UITexture apocalypticIcon;

	[SerializeField]
	private UIButton gvgInfusedDescriptionButton;

	[SerializeField]
	private GameObject favoriteButton;

	[Header("Share screen")]
	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	[SerializeField]
	private UISprite closeAreaSprite;

	[SerializeField]
	private UISprite closeAreaBackground;

	[SerializeField]
	private GameObject levelUpEffect;

	[SerializeField]
	private EquipmentFavouriteButton equipmentFavouriteButton;

	[SerializeField]
	private GameObject apocalypticEffect;

	[SerializeField]
	private GameObject remoldOperateContainer;

	[SerializeField]
	private SPRemoldSkillLeft SPRemoldSkillLeft;

	[SerializeField]
	private SPRemoldSkillMain SPRemoldSkillMain;

	[SerializeField]
	private SPRemoldSkillOperate SPRemoldSkillOperate;

	[HideInInspector]
	public EquipmentItemModel equipmentItemModel;

	private EquipmentDefinition equipmentDefinition;

	private Popup_LevelUp_Base popupLevelUpBase;

	private ColorEntry rarityColorEntry;

	private bool equipmentIsNotAcquired;

	private TradeSlotInfo tradeSlotDefinition;

	private GuildShopItemInfo guildShopItemInfo;

	private int equipmentRarityLevel;

	public static int BundlePopUpDepth = 57;

	private int equipmentStartingLevel;

	private bool traitsKnown = true;

	private bool tradeShopRefreshTimer;

	private bool tradeShopItemRefreshTimer;

	private Action blackMarketBuyCallback;

	private BlackMarketDefinition blackMarketDefinition;

	private Action hillCoinBuyCallback;

	private HillTopStoreDefinition hillTopStoreDefinition;

	private bool isRemoldOperateContainerOpen;

	private int currentOperateSlotIndex = -1;

	public bool ShowNextLevel { get; set; }

	public bool ShowThisLevelUnlocks { get; set; }

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "BreakThroughed":
			UpdateUI();
			break;
		case "EquipmentRemodelSelectioned":
			UpdateUI();
			break;
		case "SPRemoldLockChanged":
		case "SPRemoldRandomChanged":
		case "SPRemoldUpgradeChanged":
			UpdateUI();
			break;
		case "SPRemoldOperateCloseClick":
			isRemoldOperateContainerOpen = false;
			UpdateRemoldOperateContainer();
			break;
		case "SPRemoldOperateItemClick":
		{
			int num = (int)parameter;
			isRemoldOperateContainerOpen = true;
			currentOperateSlotIndex = num;
			UpdateRemoldOperateContainer();
			break;
		}
		case "SPRemoldEquipModSkill":
		case "SPRemoldUnEquipModSkill":
			if (equipmentItemModel == null)
			{
				break;
			}
			if (isRemoldOperateContainerOpen)
			{
				UpdateRemoldOperateContainer();
				break;
			}
			if (SPRemoldSkillMain != null)
			{
				SPRemoldSkillMain.Setup(equipmentItemModel);
			}
			if (SPRemoldSkillLeft != null)
			{
				SPRemoldSkillLeft.Setup(equipmentItemModel);
			}
			break;
		}
	}

	private void Awake()
	{
		popupLevelUpBase = GetComponent<Popup_LevelUp_Base>();
	}

	public override void Start()
	{
		base.Start();
		GameManager.Instance.playerModel.Changed += GameManager_Instance_playerModel_Changed;
	}

	private void GameManager_Instance_playerModel_Changed(ModelObject model, string changed, object args)
	{
		if (changed == "TradeShopRefreshed" && tradeSlotDefinition != null && tradeShopRefreshTimer)
		{
			tradeShopRefreshTimer = false;
			defaultPopup.SetOfferTime(LocalizationManager.GetText("Popup.BuildMenu.NoTimeLeft"));
			defaultPopup.HideAllPayButtons();
		}
	}

	public void OnDestroy()
	{
		GameManager.Instance.playerModel.Changed -= GameManager_Instance_playerModel_Changed;
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		equipmentIsNotAcquired = false;
		isRemoldOperateContainerOpen = false;
		equipmentItemModel = model as EquipmentItemModel;
		equipmentDefinition = equipmentItemModel.Definition;
		equipmentStartingLevel = equipmentItemModel.StartingLevel;
		equipmentRarityLevel = equipmentItemModel.RarityLevel;
		EnableOwnCloseArea(enable: false);
		if (equipmentItemModel.Definition.Category == EquipmentCategory.Armor)
		{
			weaponIcon.gameObject.SetActive(value: false);
			armorIcon.gameObject.SetActive(value: true);
			armorIcon.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentItemModel);
			if (equipmentDefinition.UseSpecialMaterial)
			{
				Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(equipmentItemModel).specialMaterial;
				armorIcon.material = specialMaterial ?? armorIcon.material;
			}
		}
		else
		{
			weaponIcon.gameObject.SetActive(value: true);
			armorIcon.gameObject.SetActive(value: false);
			weaponIcon.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentItemModel);
			if (equipmentDefinition.UseSpecialMaterial)
			{
				Material specialMaterial2 = HelpersGfx.GetEquipmentResourceEntry(equipmentItemModel).specialMaterial;
				weaponIcon.material = specialMaterial2 ?? weaponIcon.material;
			}
		}
		if (defaultPopup != null)
		{
			//defaultPopup.transform.SetChildLayer(20);
			if (OfflineManager.IsLoadDataManager)
			{
				closeAreaBackground.gameObject.SetActive(false);
				defaultPopup.transform.parent = this.transform;
				defaultPopup.transform.SetChildLayer(this.gameObject.layer);
				defaultPopup.transform.localScale = Vector3.one;
			}
		}
		nameLabel.text = HelpersLocalization.GetEquipmentName(equipmentItemModel);
		rarityColorEntry = GameManager.Instance.GetRarityColorData(equipmentItemModel.RarityLevel);
		nameLabel.gradientTop = rarityColorEntry.GradientColorTop;
		nameLabel.gradientBottom = rarityColorEntry.GradientColorBottom;
		survivorRequiredLabel.text = LocalizationManager.GetText("Popup.EquipmentLevelUp.SurvivorLevelNeeded{ClassName}{Level}", HelpersLocalization.GetSurvivorClassName(equipmentItemModel.Definition.SurvivorClass), equipmentItemModel.StartingLevel);
		equipmentRarityLabel.text = LocalizationManager.GetText("Popup.SurvivorLevelUp.Rarity{Name}", HelpersLocalization.GetRarityLevel(equipmentItemModel.RarityLevel));
		SetApocalypticEffect(equipmentItemModel.RarityLevel);
		statIcon.spriteName = HelpersGfx.GetEquipmentCategoryIconName(equipmentItemModel.Definition.Category);
		if (background != null)
		{
			background.color = rarityColorEntry.BackgroundColor;
		}
		popupLevelUpBase.ShowNextLevel = ShowNextLevel;
		popupLevelUpBase.Init(equipmentItemModel.RarityLevel);
		UpgradePathData upgradePathData = new UpgradePathData();
		upgradePathData.StartLevel = equipmentItemModel.StartingLevel;
		upgradePathData.CurrentLevel = equipmentItemModel.Level;
		upgradePathData.MaxLevel = equipmentItemModel.MaxLevel;
		upgradePathData.Equipment = equipmentItemModel;
		popupLevelUpBase.InitUpgradePath(upgradePathData);
		ShowUiForScreenshot(show: false);
		if (SPRemoldSkillMain != null)
		{
			SPRemoldSkillMain.Setup(equipmentItemModel);
		}
		if (SPRemoldSkillLeft != null)
		{
			SPRemoldSkillLeft.Setup(equipmentItemModel);
		}
		UpdateUI();
	}

	public void OpenForEquipmentTradeItem(TradeSlotInfo tradeSlot)
	{
		base.Open();
		EnableOwnCloseArea(enable: false);
		tradeSlotDefinition = tradeSlot;
		if (tradeSlot.CurrentTradeDefinition.HasDateLimit)
		{
			tradeShopItemRefreshTimer = true;
			tradeShopRefreshTimer = false;
		}
		else
		{
			tradeShopItemRefreshTimer = false;
			tradeShopRefreshTimer = true;
		}
		equipmentIsNotAcquired = true;
		IReward reward = tradeSlot.CurrentTradeDefinition.SoldItems.RewardsList[0];
		FillEquipmentData(reward, out var rarityLevel, out var startingLevel, out var definition, (int)GameManager.Instance.playerModel.LastTradeShopRefreshTime + tradeSlot.CurrentTradeDefinition.UniqueId);
		OpenForEquipmentDefinition(definition, rarityLevel, startingLevel);
	}

	public void OpenForEquipmentInBlackMarket(BlackMarketDefinition blackMarketDefinition, IReward reward, Action buyCallback)
	{
		base.Open();
		EnableOwnCloseArea(enable: false);
		this.blackMarketDefinition = blackMarketDefinition;
		blackMarketBuyCallback = buyCallback;
		tradeShopItemRefreshTimer = false;
		tradeShopRefreshTimer = false;
		equipmentIsNotAcquired = true;
		FillEquipmentData(reward, out var rarityLevel, out var startingLevel, out var definition, this.blackMarketDefinition.UniqueId);
		OpenForEquipmentDefinition(definition, rarityLevel, startingLevel);
	}

	public void OpenForEquipmentInHillCoin(HillTopStoreDefinition hillTopStoreDefinition, IReward reward, Action buyCallback)
	{
		base.Open();
		EnableOwnCloseArea(enable: false);
		this.hillTopStoreDefinition = hillTopStoreDefinition;
		hillCoinBuyCallback = buyCallback;
		tradeShopItemRefreshTimer = false;
		tradeShopRefreshTimer = false;
		equipmentIsNotAcquired = true;
		FillEquipmentData(reward, out var rarityLevel, out var startingLevel, out var definition, this.hillTopStoreDefinition.UniqueId);
		OpenForEquipmentDefinition(definition, rarityLevel, startingLevel);
	}

	public void OpenForGuildShopItem(GuildShopItemInfo itemInfo)
	{
		base.Open();
		EnableOwnCloseArea(enable: false);
		guildShopItemInfo = itemInfo;
		IReward reward = itemInfo.ItemDefinition.ContentRewards.RewardsList[0];
		tradeShopItemRefreshTimer = false;
		tradeShopRefreshTimer = false;
		equipmentIsNotAcquired = true;
		FillEquipmentData(reward, out var rarityLevel, out var startingLevel, out var definition, GameManager.Instance.playerModel.GuildShopModel.RandomSeed + itemInfo.ItemDefinition.ID);
		OpenForEquipmentDefinition(definition, rarityLevel, startingLevel);
	}

	private void FillEquipmentData(IReward reward, out int rarityLevel, out int startingLevel, out EquipmentDefinition definition, int randomSeed)
	{
		rarityLevel = 0;
		startingLevel = 0;
		definition = null;
		if (!(reward is RewardEquipment rewardEquipment))
		{
			if (reward is RewardRandomEquipment rewardRandomEquipment)
			{
				definition = rewardRandomEquipment.GetRandomEquipmentDefinition(GameManager.Instance.modelManager, new ModelRandom(randomSeed), out var levelOut);
				rarityLevel = rewardRandomEquipment.RarityLevel;
				startingLevel = levelOut;
				traitsKnown = false;
			}
		}
		else
		{
			rarityLevel = rewardEquipment.RarityLevel;
			startingLevel = rewardEquipment.StartingLevel;
			definition = GameManager.Instance.playerModel.gameEconomyData.GetEquipmentDefinition(rewardEquipment.EquipmentId);
		}
	}

	public override void Update()
	{
		base.Update();
		if (tradeSlotDefinition == null)
		{
			return;
		}
		if (tradeShopItemRefreshTimer)
		{
			long timeLeft = tradeSlotDefinition.CurrentTradeDefinition.GetTimeLeft(GameManager.Instance.playerModel.UtcTimeStamp);
			if (timeLeft >= 0)
			{
				defaultPopup.SetOfferTime(LocalizationManager.GetText("Popup.BuildMenu.OfferEndsIn{timeToEnd}", Helpers.FormatTime(timeLeft)));
			}
			else
			{
				defaultPopup.SetOfferTime(LocalizationManager.GetText("Popup.BuildMenu.NoTimeLeft"));
				defaultPopup.HideAllPayButtons();
			}
		}
		else if (tradeShopRefreshTimer)
		{
			defaultPopup.SetOfferTime(LocalizationManager.GetText("Popup.BuildMenu.OfferEndsIn{timeToEnd}", Helpers.FormatTime(GameManager.Instance.playerModel.GetTimeLeftToTradeShopRefresh())));
		}
	}

	public void OpenForBundleReward(RewardEquipment reward)
	{
		base.Open();
		EnableOwnCloseArea(enable: true);
		string equipmentId = reward.EquipmentId;
		DebugTWD.Log("Try open " + equipmentId);

		int rarityLevel = reward.RarityLevel;
		EquipmentDefinition definition = GameManager.Instance.playerModel.gameEconomyData.GetEquipmentDefinition(equipmentId);
		OpenForEquipmentDefinition(definition, rarityLevel, reward.StartingLevel);
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			component.depth = BundlePopUpDepth;
		}
		if (defaultPopup != null)
		{
			defaultPopup.gameObject.SetActive(value: false);
		}
		ShowUiForScreenshot(show: false);
	}

	public void OpenForRewardEquipTokenApocalyptic(RewardEquipToken rewardEquipToken)
	{
		base.Open();
		EnableOwnCloseArea(enable: true);
		string equipTokenId = rewardEquipToken.EquipTokenId;
		EquipTokenDefinition equipTokenDefinition = GameManager.Instance.playerModel.gameEconomyData.GetEquipTokenDefinition(equipTokenId);
		if (equipTokenDefinition != null)
		{
			EquipmentDefinition definition = GameManager.Instance.playerModel.gameEconomyData.GetEquipmentDefinition(equipTokenDefinition.RelateEquipId);
			OpenForEquipmentDefinition(definition, equipTokenDefinition.Star);
			UIPanel component = base.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				component.depth = BundlePopUpDepth;
			}
			if (defaultPopup != null)
			{
				defaultPopup.gameObject.SetActive(value: false);
			}
			ShowUiForScreenshot(show: false);
		}
	}

	public void OpenForPreview(string equipId, int rarityLevel, int startingLevel = 0)
	{
		if (equipId != null)
		{
			base.Open();
			EnableOwnCloseArea(enable: true);
			EquipmentDefinition definition = GameManager.Instance.playerModel.gameEconomyData.GetEquipmentDefinition(equipId);
			OpenForEquipmentDefinition(definition, rarityLevel, startingLevel);
			UIPanel component = base.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				component.depth = BundlePopUpDepth;
			}
			if (defaultPopup != null)
			{
				defaultPopup.gameObject.SetActive(value: false);
			}
			ShowUiForScreenshot(show: false);
		}
	}

	private void OpenForEquipmentDefinition(EquipmentDefinition definition, int rarityLevel, int level = 0)
	{
		equipmentIsNotAcquired = true;
		equipmentDefinition = definition;
		equipmentRarityLevel = rarityLevel;
		equipmentStartingLevel = ((level == 0) ? GameManager.Instance.playerModel.Equipment.GetHighestLevelForEquipmentImmediateEquip(equipmentDefinition) : level);
		if (equipmentDefinition == null)
		{
			Close();
		}
		if (equipmentDefinition.Category == EquipmentCategory.Armor)
		{
			weaponIcon.gameObject.SetActive(value: false);
			armorIcon.gameObject.SetActive(value: true);
			armorIcon.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(equipmentDefinition.ID);
			if (equipmentDefinition.UseSpecialMaterial)
			{
				Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(equipmentDefinition).specialMaterial;
				armorIcon.material = specialMaterial ?? armorIcon.material;
			}
		}
		else
		{
			weaponIcon.gameObject.SetActive(value: true);
			armorIcon.gameObject.SetActive(value: false);
			weaponIcon.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(equipmentDefinition.ID);
			if (equipmentDefinition.UseSpecialMaterial)
			{
				Material specialMaterial2 = HelpersGfx.GetEquipmentResourceEntry(equipmentDefinition).specialMaterial;
				weaponIcon.material = specialMaterial2 ?? weaponIcon.material;
			}
		}
		nameLabel.text = HelpersLocalization.GetEquipmentName(equipmentDefinition.ID);
		rarityColorEntry = GameManager.Instance.GetRarityColorData(rarityLevel);
		nameLabel.gradientTop = rarityColorEntry.GradientColorTop;
		nameLabel.gradientBottom = rarityColorEntry.GradientColorBottom;
		survivorRequiredLabel.text = LocalizationManager.GetText("Popup.EquipmentLevelUp.SurvivorLevelNeeded{ClassName}{Level}", HelpersLocalization.GetSurvivorClassName(equipmentDefinition.SurvivorClass), equipmentStartingLevel);
		equipmentRarityLabel.text = LocalizationManager.GetText("Popup.SurvivorLevelUp.Rarity{Name}", HelpersLocalization.GetRarityLevel(rarityLevel));
		SetApocalypticEffect(rarityLevel);
		statIcon.spriteName = HelpersGfx.GetEquipmentCategoryIconName(equipmentDefinition.Category);
		if (background != null)
		{
			background.color = rarityColorEntry.BackgroundColor;
		}
		popupLevelUpBase.ShowNextLevel = ShowNextLevel;
		popupLevelUpBase.Init(rarityLevel);
		ShowUiForScreenshot(show: false);
		UpdateUI();
		if (SPRemoldSkillMain != null)
		{
			SPRemoldSkillMain.Setup(equipmentDefinition, equipmentRarityLevel, equipmentStartingLevel);
		}
		if (SPRemoldSkillLeft != null)
		{
			SPRemoldSkillLeft.Setup(equipmentDefinition, equipmentRarityLevel, equipmentStartingLevel);
		}
		if (levelLabel != null)
		{
			levelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", equipmentStartingLevel.ToString() ?? "");
		}
	}

	private void SetApocalypticEffect(int rarityLevel)
	{
		HelpersGfx.SetApocalypticEffectActive(apocalypticEffect, rarityLevel);
		HelpersGfx.SetApocalypticEffectActive(apocalypticButton, rarityLevel);
		HelpersGfx.SetApocalypticEffectActive(apocalypticPlus, rarityLevel);
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		if (equipmentItemModel != null)
		{
			list = equipmentItemModel?.GetEquipmentActiveTraits();
			list2 = equipmentItemModel?.GetEquipmentPassiveTraits();
		}
		list = equipmentDefinition?.ActiveTraits;
		list2 = equipmentDefinition?.PassiveTraits;
		HelpersGfx.SetApocalypticEffectSprite(apocalypticIcon, list, list2, rarityLevel);
	}

	public void EnableOwnCloseArea(bool enable)
	{
		if (closeAreaSprite != null)
		{
			closeAreaSprite.gameObject.SetActive(enable);
		}
		if (closeAreaBackground != null)
		{
			if (closeAreaBackground.TryGetComponent<AnimatedAlpha>(out AnimatedAlpha alpha))
			{
				closeAreaBackground.gameObject.SetActive(true);
				alpha.alpha = enable ? .5f : 0.01f;
			}
			else
			{
				closeAreaBackground.gameObject.SetActive(enable);
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (equipmentIsNotAcquired)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			Cashier cashier = new Cashier(GameManager.Instance.modelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrate);
			if (tradeSlotDefinition != null || guildShopItemInfo != null || blackMarketDefinition != null || hillTopStoreDefinition != null)
			{
				if (tradeSlotDefinition != null)
				{
					if (tradeSlotDefinition.SlotDefinition.PriceCategory == PriceCategory.Normal)
					{
						cashierItem.SetCost(tradeSlotDefinition.CurrentTradeDefinition.PriceNormalType, tradeSlotDefinition.CurrentTradeDefinition.PriceNormalAmount);
					}
					else
					{
						cashierItem.SetCost(tradeSlotDefinition.CurrentTradeDefinition.PriceDiscountType, tradeSlotDefinition.CurrentTradeDefinition.PriceDiscountAmount);
					}
				}
				else if (guildShopItemInfo != null)
				{
					cashierItem.SetCost(guildShopItemInfo.ItemDefinition.PriceCurrency, guildShopItemInfo.ItemDefinition.PriceAmount);
				}
				else if (blackMarketDefinition != null)
				{
					cashierItem.SetCost(blackMarketDefinition.GetCurrencyType(), blackMarketDefinition.GetPrice(GameManager.Instance.modelManager));
				}
				else if (hillTopStoreDefinition != null)
				{
					cashierItem.SetCost(CurrencyType.HillTopCoin, hillTopStoreDefinition.Score);
				}
				cashier.AddItem(cashierItem);
				string text = "";
				text = ((!cashier.IsFree()) ? LocalizationManager.GetText("Popup.Workshop.Button.Trade") : LocalizationManager.GetText("Generic.Free"));
				if (defaultPopup != null)
				{
					defaultPopup.SetPayButton(text, cashier);
					defaultPopup.SetPayButtonClickCallback(OnTrade);
					defaultPopup.SetInstantPayPanel(active: false);
				}
			}
			else if (traitsPanel != null && this.equipmentDefinition != null && defaultPopup != null)
			{
				defaultPopup.HideAllPayButtons();
			}
			if (defaultPopup != null)
			{
				if (this.equipmentDefinition != null && playerModel.Equipment.CanAcquireEquipment(this.equipmentDefinition))
				{
					defaultPopup.ShowPayButtons();
					defaultPopup.SetInstantPayPanel(active: false);
				}
				else
				{
					defaultPopup.HideAllPayButtons();
					defaultPopup.ShowLockedPanel(LocalizationManager.GetText("Popup.UpgradeEquipment.EquipmentAlreadyAcquired"));
				}
			}
			List<UpgradeTraitsData> list = new List<UpgradeTraitsData>();
			if (!traitsKnown)
			{
				traitsPanel.fillTraitsAsUnknown(0);
			}
			else if (this.equipmentDefinition != null && this.equipmentDefinition.TraitsOverride != null && this.equipmentDefinition.TraitsOverride.Count > 0)
			{
				Dictionary<int, TraitBucketsDefinition> levelsThatUnlockATrait = playerModel.gameEconomyData.GetLevelsThatUnlockATrait(equipmentRarityLevel, TWDModel.UpgradeType.EquipmentUpgrade, equipmentStartingLevel, replaceTacticalWithLowLevel: false);
				int num = 0;
				foreach (KeyValuePair<int, TraitBucketsDefinition> item in levelsThatUnlockATrait)
				{
					if (num < this.equipmentDefinition.TraitsOverride.Count)
					{
						string text2 = this.equipmentDefinition.TraitsOverride[num];
						if (item.Value.IsTactical)
						{
							text2 = "ChargeEquipment";
						}
						else
						{
							num++;
						}
						UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
						TraitDefinition traitDefinition = playerModel.gameEconomyData.GetTraitDefinition(text2);
						if (traitDefinition == null)
						{
							Debug.LogError("Could not find trait definition for EquipmentItemModel:" + this.equipmentDefinition.ID + " - " + text2 + "," + this.equipmentDefinition.Type.ToString() + "," + this.equipmentDefinition.Category);
						}
						else
						{
							upgradeTraitsData.Identifier = traitDefinition.Identifier;
							upgradeTraitsData.UnlockingLevel = item.Key;
							upgradeTraitsData.RarityLevel = item.Value.RarityLevel;
							upgradeTraitsData.IsLocked = item.Value.IsLocked;
							upgradeTraitsData.IsTactical = item.Value.IsTactical;
							list.Add(upgradeTraitsData);
						}
					}
				}
				traitsPanel.setInfo(equipmentItemModel, list, equipmentStartingLevel, 1, ShowThisLevelUnlocks);
			}
			else if (this.equipmentDefinition != null)
			{
				traitsPanel.fillTraitsAsUnknown(0);
			}
			popupLevelUpBase.UpdateUpgradePath(equipmentStartingLevel);
			if (levelLabel != null)
			{
				levelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", equipmentStartingLevel.ToString() ?? "");
			}
			updateRarityRating(starsSpriteArray, equipmentRarityLevel);
			EquipmentLevelDefinition equipmentLevelDefinition = GameManager.Instance.playerModel.gameEconomyData.GetEquipmentLevelDefinition(equipmentStartingLevel);
			bool num2 = this.equipmentDefinition.Category == EquipmentCategory.Armor;
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GameManager.Instance.playerModel.gameEconomyData.GetRarityBasedUpgradeDefinition(equipmentRarityLevel, TWDModel.UpgradeType.EquipmentUpgrade);
			FixedPoint fixedPoint = (num2 ? equipmentLevelDefinition.ArmorBase : equipmentLevelDefinition.DamageBase);
			FixedPoint fixedPoint2 = (float)(num2 ? this.equipmentDefinition.ArmorMultiplier : this.equipmentDefinition.DamageMultiplier) / 100f;
			FixedPoint fixedPoint3 = (float)(num2 ? rarityBasedUpgradeDefinition.ArmorMultiplier : rarityBasedUpgradeDefinition.DamageMultiplier) / 100f;
			FixedPoint fixedPoint4 = fixedPoint * (fixedPoint2 + fixedPoint3);
			statAmount.text = ((int)fixedPoint4).ToString();
			if (firstTraitBG != null && firstTraitLabel != null && firstTraitIcon != null && firstTraitButton != null && !string.IsNullOrEmpty(this.equipmentDefinition.ChargeEquipmentIdentifier))
			{
				firstTraitBG.gameObject.SetActive(value: true);
				firstTraitLabel.gameObject.SetActive(value: true);
				firstTraitLabel.gameObject.SetActive(value: true);
				EquipmentDefinition equipmentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetEquipmentDefinition(this.equipmentDefinition.ChargeEquipmentIdentifier);
				firstTraitIcon.spriteName = HelpersGfx.GetEquipmentResourceEntry(equipmentDefinition).IconSprite;
				firstTraitLabel.text = LocalizationManager.GetText("Equipment.ChargeLabel." + equipmentDefinition.ID);
				string text3 = LocalizationManager.GetText("Traits." + equipmentDefinition.ID + ".Description");
				text3 = text3.Substring(text3.IndexOf(":") + 1);
				firstTraitButton.SetText(text3);
			}
			equipmentFavouriteButton.gameObject.SetActive(value: false);
		}
		else
		{
			int maxNumberOfUpgrades = equipmentItemModel.manager.Player.gameEconomyData.GetMaxNumberOfUpgrades(TWDModel.UpgradeType.EquipmentUpgrade);
			bool canUpgradeWithEquipmentUpgradeToken = equipmentItemModel.CanUpgradeWithEquipmentUpgradeToken;
			if (defaultPopup != null)
			{
				defaultPopup.SetInstantPayButton(equipmentItemModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true));
				defaultPopup.SetInstantPayWithTokensButton(equipmentItemModel.GetUpgradeCashierWithTokens());
				defaultPopup.SetInstantPayWithTokensButtonClickCallback(OnUpgradeInstantWithTokens);
				int equipmentBaseLevelUpgradeCost = equipmentItemModel.GetEquipmentBaseLevelUpgradeCost();
				bool twoCurrenciesPayment = canUpgradeWithEquipmentUpgradeToken && equipmentBaseLevelUpgradeCost > 0;
				defaultPopup.SetPayButton(LocalizationManager.GetText("Popup.Workshop.Button.Upgrade"), equipmentItemModel.GetUpgradeCashier(instantUpgrade: false, addInitialSurvivorPoints: false, canUpgradeWithEquipmentUpgradeToken), 0, twoCurrenciesPayment);
				defaultPopup.SetInstantPayButtonClickCallback(OnUpgradeInstant);
				if (!equipmentItemModel.CanUpgrade)
				{
					defaultPopup.SetInstantPayPanel(active: false);
				}
			}
			EventDelegate.Callback payButtonClickCallback = ((!canUpgradeWithEquipmentUpgradeToken) ? new EventDelegate.Callback(OnUpgrade) : new EventDelegate.Callback(ShowConfirmationBeforeUpdating));
			if (defaultPopup != null)
			{
				defaultPopup.SetPayButtonClickCallback(payButtonClickCallback);
			}
			int startingLevel = equipmentItemModel.StartingLevel;
			int getTotalUpgrades = equipmentItemModel.GetTotalUpgrades;
			int mainStat = equipmentItemModel.MainStat;
			int mainStatForLevel = equipmentItemModel.GetMainStatForLevel(equipmentItemModel.MaxLevel);
			popupLevelUpBase.SetDamagePanel(mainStat, mainStatForLevel, equipmentItemModel.Level, startingLevel, getTotalUpgrades, maxNumberOfUpgrades);
			if (SPRemoldSkillLeft != null)
			{
				statAmount.text = LocalizationManager.GetText("System.EquipInfo.Value") + mainStat;
			}
			WorkshopBuildingModel workshopBuildingModel = GameManager.Instance.playerModel.Camp.GetBuilding("Workshop") as WorkshopBuildingModel;
			bool flag = workshopBuildingModel?.IsUpgrading ?? false;
			bool flag2 = workshopBuildingModel != null && (workshopBuildingModel.UpgradingEquipment != null || workshopBuildingModel.UpgradedUnseenModel != null);
			bool flag3 = workshopBuildingModel != null && workshopBuildingModel.UpgradingEquipment == equipmentItemModel;
			bool flag4 = !flag2 && !flag && equipmentItemModel.CanUpgrade;
			bool flag5 = !flag2 && !flag && canUpgradeWithEquipmentUpgradeToken;
			bool active = false;
			bool showMax = false;
			if (flag4)
			{
				if (defaultPopup != null)
				{
					defaultPopup.ShowPayButtons();
				}
			}
			else if (flag5)
			{
				if (defaultPopup != null)
				{
					bool flag6 = equipmentItemModel.GetUpgradeCashier(instantUpgrade: false, addInitialSurvivorPoints: false, canUpgradeWithEquipmentUpgradeToken).CanPay(CurrencyType.EquipmentUpgradeToken);
					defaultPopup.ShowPayOnlyWithCurrencyButton(flag6);
					if (!flag6)
					{
						defaultPopup.SetCannotPayClickCallback(ShowCannotAffordAlert);
						defaultPopup.ShowCannotPayButton();
					}
					active = true;
				}
			}
			else
			{
				string text4 = null;
				if (equipmentItemModel.CanUpgrade || canUpgradeWithEquipmentUpgradeToken)
				{
					if (flag2)
					{
						text4 = LocalizationManager.GetText("Popup.UpgradeEquipment.EquipmentUpgrading");
					}
					else if (flag)
					{
						text4 = LocalizationManager.GetText("Popup.UpgradeEquipment.WorkshopUpgrading");
					}
				}
				else if (!equipmentItemModel.HasWorkshopLevelRequired)
				{
					text4 = LocalizationManager.GetText("Popup.UpgradeEquipment.WorkshopLevelRequired{Level}", equipmentItemModel.EquipmentLevelDefinition.WorkshopLevelRequired);
				}
				else
				{
					text4 = LocalizationManager.GetText("Popup.UpgradeEquipment.EquipmentMaxLevel");
					popupLevelUpBase.HideNextDamage();
					popupLevelUpBase.HideNextHealth();
					active = true;
					showMax = true;
				}
				if (defaultPopup != null)
				{
					defaultPopup.HideAllPayButtons();
					defaultPopup.ShowLockedPanel(text4);
				}
			}
			if (defaultPopup != null)
			{
				bool star7CanBeBreakDown = GameManager.Instance.gameEconomyData.ConfigData.Star7CanBeBreakDown;
				bool flag7 = equipmentItemModel.Is7StarEquipment && !star7CanBeBreakDown;
				bool flag8 = equipmentItemModel.Definition.SwitchRemoldMode && !GameManager.Instance.gameEconomyData.ConfigData.RemoldEquipCanBeBreakDown;
				bool available = !flag3 && !equipmentItemModel.IsFavourite && !flag7 && !flag8;
				defaultPopup.SetActionButton(available, LocalizationManager.GetText("Popup.Workshop.Button.Scrap"), OnScrap);
				defaultPopup.SetBreakthtroughInfoActive(active, showMax, equipmentItemModel);
			}
			if (ShowNextLevel && !equipmentItemModel.HasReachedMaxLevel)
			{
				int nextDamageValue = equipmentItemModel.GetMainStatForLevel(equipmentItemModel.Level + 1) - equipmentItemModel.MainStat;
				popupLevelUpBase.SetNextDamageValue(nextDamageValue);
			}
			popupLevelUpBase.UpdateUpgradePath(equipmentItemModel.Level);
			if (traitsPanel != null && equipmentItemModel != null)
			{
				traitsPanel.setInfo(equipmentItemModel, equipmentItemModel.UpgradeTraits, equipmentItemModel.Level, 1, ShowThisLevelUnlocks);
			}
			updateRarityRating(starsSpriteArray, equipmentItemModel.RarityLevel);
			if (firstTraitLabel != null && firstTraitBG != null && firstTraitButton != null && equipmentItemModel != null && equipmentItemModel.UpgradeTraits[0] != null)
			{
				if (equipmentItemModel.ChargeEquipment != null)
				{
					firstTraitBG.gameObject.SetActive(value: true);
					firstTraitLabel.gameObject.SetActive(value: true);
					firstTraitLabel.gameObject.SetActive(value: true);
					firstTraitLabel.text = LocalizationManager.GetText("Equipment.ChargeLabel." + equipmentItemModel.ChargeEquipment.Definition.ID);
					string text5 = LocalizationManager.GetText("Traits." + equipmentItemModel.ChargeEquipment.Definition.ID + ".Description");
					text5 = text5.Substring(text5.IndexOf(":") + 1);
					firstTraitButton.SetText(text5);
				}
				else
				{
					firstTraitBG.gameObject.SetActive(value: false);
					firstTraitLabel.gameObject.SetActive(value: false);
				}
			}
			Helpers.GameObjectSetActive(firstTraitIcon, value: false);
			if (equipmentItemModel.ChargeEquipment != null)
			{
				Helpers.GameObjectSetActive(firstTraitIcon, value: true);
				firstTraitIcon.spriteName = HelpersGfx.GetChargeEquipmentIconName(equipmentItemModel.ChargeEquipment);
			}
			if (levelLabel != null)
			{
				string text6 = ((equipmentItemModel.EquipmentUpgradeTokenLevelUpgrades > 0) ? (" + " + equipmentItemModel.EquipmentUpgradeTokenLevelUpgrades) : "");
				levelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", equipmentItemModel.Level - equipmentItemModel.EquipmentUpgradeTokenLevelUpgrades + text6);
			}
			if (equipmentFavouriteButton != null)
			{
				equipmentFavouriteButton.gameObject.SetActive(value: true);
				equipmentFavouriteButton.UpdateVisibility();
			}
		}
		if (statsContainer != null)
		{
			statsContainer.SetActive(value: true);
		}
		if (chargeContainer != null)
		{
			chargeContainer.SetActive(!string.IsNullOrEmpty(this.equipmentDefinition.ChargeEquipmentIdentifier));
		}
		UpdateSpecialDescriptionButton();
		UpdateGvGSpecialWeapon();
		UpdateRemoldOperateContainer();
	}

	private void updateRarityRating(UISprite[] starsArray, int rarityLevel)
	{
		for (int i = 0; i < starsArray.Length; i++)
		{
			if (!(starsArray[i] != null) || !(starsArray[i].gameObject != null))
			{
				continue;
			}
			if (rarityLevel >= i && starsArray[i] != null && starsArray[i].gameObject != null)
			{
				starsArray[i].gameObject.SetActive(value: true);
				if (starsArray[i].GetComponent<UIWidget>() != null && rarityColorEntry != null)
				{
					starsArray[i].GetComponent<UIWidget>().color = rarityColorEntry.GradientColorTop;
				}
			}
			else
			{
				starsArray[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void ShowEquipmentReceivedVersion()
	{
		if (defaultPopup != null)
		{
			defaultPopup.HideAllPayButtons();
		}
		TweenManager.PlayTweenGroup(base.gameObject, 10);
	}

	public void OnUpgradeInstant()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(UIType.ConfirmationPopup);
		if (equipmentItemModel.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeEquipmentCommand(equipmentItemModel)
			{
				Instant = true,
				Cashier = equipmentItemModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true)
			}, InstantUpgradeCallback);
		}
	}

	public void OnUpgradeInstantWithTokens()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(UIType.ConfirmationPopup);
		if (equipmentItemModel.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeEquipmentCommand(equipmentItemModel)
			{
				Instant = true,
				Cashier = equipmentItemModel.GetUpgradeCashierWithTokens()
			}, InstantUpgradeCallback);
		}
	}

	public void InstantUpgradeCallback(TWDModelResult result)
	{
		if (result != TWDModelResult.Cancelled)
		{
			StartCoroutine(DelayedUpgradeUpdate());
		}
	}

	private IEnumerator DelayedUpgradeUpdate()
	{
		yield return new WaitForSeconds(0.5f);
		UpdateUI();
		UIEvent.Send("EquipmentInstantUpgraded", equipmentItemModel);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/workshop_equipment_upgrade");
		Helpers.InstantiateToParentAndLayer(levelUpEffect, statAmount.gameObject);
		yield return null;
	}

	public void OnUpgrade()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(UIType.ConfirmationPopup);
		bool canUpgradeWithEquipmentUpgradeToken = equipmentItemModel.CanUpgradeWithEquipmentUpgradeToken;
		if (equipmentItemModel.CanUpgrade || canUpgradeWithEquipmentUpgradeToken)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeEquipmentCommand(equipmentItemModel)
			{
				Instant = false,
				Cashier = equipmentItemModel.GetUpgradeCashier(instantUpgrade: false, addInitialSurvivorPoints: false, canUpgradeWithEquipmentUpgradeToken)
			}, InstantUpgradeCallback);
		}
		UIEvent.Send("EquipmentStartUpgrade", equipmentItemModel);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/workshop_equipment_upgrade");
		if (OfflineManager.IsLoadDataManager) Close();
	}

	private void ShowConfirmationBeforeUpdating()
	{
		ConfirmationPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.AccountConfirmation.AdditionalCheck.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Workshop.UseTokensConfirmation"), LocalizationManager.GetText("Button.Yes"), OnUpgrade, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void ShowCannotAffordAlert()
	{
		AlertPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Workshop.InsufficientTokens.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Workshop.InsufficientTokens.Message"), LocalizationManager.GetText("Button.Ok"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnTrade()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null)
		{
			if (tradeSlotDefinition != null)
			{
				ConsumeCurrencyCommandUtils.Execute(new BuyTradeCrateCommand(tradeSlotDefinition.SlotDefinition.SlotId)
				{
					Cashier = playerModel.LootManager.GetCashierForTradeCrate(tradeSlotDefinition)
				}, tradePurchaseCallback);
			}
			else if (guildShopItemInfo != null)
			{
				ConsumeCurrencyCommandUtils.Execute(new BuyGuildShopItemCommand(guildShopItemInfo.ItemDefinition.ID)
				{
					Cashier = playerModel.GuildShopModel.GetCashierForItem(guildShopItemInfo.ItemDefinition)
				}, tradePurchaseCallback);
			}
			else if (blackMarketBuyCallback != null)
			{
				blackMarketBuyCallback();
			}
			else if (hillCoinBuyCallback != null)
			{
				hillCoinBuyCallback();
			}
		}
	}

	private void tradePurchaseCallback(TWDModelResult result)
	{
		if (result != TWDModelResult.OK)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && (tradeSlotDefinition != null || guildShopItemInfo != null))
		{
			if (tradeSlotDefinition != null)
			{
				UIEvent.Send("OnTradeEquipmentPurchased", tradeSlotDefinition.SlotDefinition);
			}
			else if (guildShopItemInfo != null)
			{
				UIEvent.Send("OnGuildShopItemPurchased", guildShopItemInfo.ItemDefinition.ID);
			}
			Close();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			iAPConfirmPopupNew.ShowShopWhenClosed = true;
			if (playerModel.LootManager.LastTradedEquipment != null)
			{
				iAPConfirmPopupNew.OpenForEquipment(playerModel.LootManager.LastTradedEquipment);
			}
		}
	}

	public void OnScrap()
	{
		if (!equipmentItemModel.Definition.SwitchRemoldMode || GameManager.Instance.gameEconomyData.ConfigData.RemoldEquipCanBeBreakDown)
		{
			ActorModel owner = equipmentItemModel.Owner;
			if (owner != null)
			{
				AlertPopup alertPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
				alertPopup.SetContent(LocalizationManager.GetText("Popup.ScrapConfirmation.InvalidEquipmentScrapTitle"), LocalizationManager.GetText("Popup.ScrapConfirmation.InvalidEquipmentScrapMessage{survivorName}", owner.Name));
				alertPopup.SetCallbacks(OnScrapInvalidOkClicked);
				alertPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
				alertPopup.Open();
			}
			else
			{
				SPRemoldScrapConfirmAgainPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldScrapConfirmAgainPopup) as SPRemoldScrapConfirmAgainPopup;
				obj.SetCallbacks(OnScrapConfirmed);
				obj.Setup(equipmentItemModel);
				obj.Open();
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void OnScrapConfirmed()
	{
		Cashier getScrapCashier = equipmentItemModel.GetScrapCashier;
		BuildingsHUD.Get().CreateCollectAnim(getScrapCashier);
		ScrapEquipmentItemCommand scrapEquipmentItemCommand = new ScrapEquipmentItemCommand(equipmentItemModel);
		if (Helpers.ExecuteCommand(scrapEquipmentItemCommand) == TWDModelResult.OK)
		{
			SPRemoldScrapRewardsPopup sPRemoldScrapRewardsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldScrapRewardsPopup) as SPRemoldScrapRewardsPopup;
			if (sPRemoldScrapRewardsPopup != null && scrapEquipmentItemCommand.Rewards != null && scrapEquipmentItemCommand.Rewards.RewardsList != null && scrapEquipmentItemCommand.Rewards.RewardsList.Count > 0)
			{
				sPRemoldScrapRewardsPopup.SetupRewards(scrapEquipmentItemCommand.Rewards);
				sPRemoldScrapRewardsPopup.Open();
			}
			UIEvent.Send("OnEquipmentUpdated");
			Close();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/workshop_equipment_scrap");
			UIEvent.Send("EquipmentScrapped");
		}
	}

	private void OnScrapInvalidOkClicked()
	{
		Close();
	}

	private void ShowUiForScreenshot(bool show)
	{
		sharePanel.SetActive(show);
		if ((bool)shareButton)
		{
			shareButton.gameObject.SetActive(!show);
		}
		if ((bool)favoriteButton)
		{
			favoriteButton.SetActive(!show && !equipmentIsNotAcquired);
		}
	}

	public void OnClickShare()
	{
		StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot("Equipment", shareButton, shareBadge, ShowUiForScreenshot));
	}

	public void HideShareButton()
	{
		if ((bool)shareButton)
		{
			shareButton.gameObject.SetActive(value: false);
		}
	}

	private void UpdateSpecialDescriptionButton()
	{
		bool num = !string.IsNullOrEmpty(HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition));
		bool flag = !string.IsNullOrEmpty(equipmentDefinition.SpecialTrait);
		bool num2 = num || flag;
		Helpers.GameObjectSetActive(spRemoldDescriptionButton, value: false);
		Helpers.GameObjectSetActive(specialDescriptionButton, value: false);
		if (num2)
		{
			if (spRemoldDescriptionButton != null && equipmentDefinition != null && equipmentDefinition.SwitchRemoldMode)
			{
				Helpers.GameObjectSetActive(spRemoldDescriptionButton, value: true);
				SPRemoldUpgradeDesc component = spRemoldDescriptionButton.GetComponent<SPRemoldUpgradeDesc>();
				component.SetToolBtnActive(active: false);
				if (equipmentItemModel != null && equipmentItemModel.SpEquipmentRemoldModel != null)
				{
					component.SetToolBtnActive(active: true);
				}
			}
			else
			{
				Helpers.GameObjectSetActive(specialDescriptionButton, value: true);
			}
		}
		UpdateSPRemoldUIRaity();
	}

	private void UpdateGvGSpecialWeapon()
	{
		Helpers.GameObjectSetActive(gvgInfusedDescriptionButton, !string.IsNullOrEmpty(equipmentDefinition.InfusedTrait));
	}

	public void OnClickSpecialDescription()
	{
		if (!(specialDescriptionButton != null))
		{
			return;
		}
		if (equipmentItemModel != null)
		{
			if (equipmentItemModel.IsWeaponEquipment)
			{
				TooltipManager.OpenTextBoxWithText(specialDescriptionButton.gameObject, HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition));
			}
			else if (!string.IsNullOrEmpty(equipmentDefinition.SpecialTrait))
			{
				TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(equipmentDefinition.SpecialTrait);
				if (traitDefinition != null)
				{
					TooltipManager.OpenTextBoxWithText(specialDescriptionButton.gameObject, HelpersLocalization.GetTraitDescription(traitDefinition));
				}
				else
				{
					TooltipManager.OpenTextBoxWithText(specialDescriptionButton.gameObject, LocalizationManager.GetText(equipmentDefinition.SpecialTrait));
				}
			}
		}
		else
		{
			if (equipmentDefinition == null)
			{
				return;
			}
			if (equipmentDefinition.Category == EquipmentCategory.Armor && !string.IsNullOrEmpty(equipmentDefinition.SpecialTrait))
			{
				TraitDefinition traitDefinition2 = GameManager.Instance.gameEconomyData.GetTraitDefinition(equipmentDefinition.SpecialTrait);
				if (traitDefinition2 != null)
				{
					TooltipManager.OpenTextBoxWithText(specialDescriptionButton.gameObject, HelpersLocalization.GetTraitDescription(traitDefinition2));
				}
				else
				{
					TooltipManager.OpenTextBoxWithText(specialDescriptionButton.gameObject, LocalizationManager.GetText(equipmentDefinition.SpecialTrait));
				}
			}
			else if (!string.IsNullOrEmpty(HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition)))
			{
				TooltipManager.OpenTextBoxWithText(specialDescriptionButton.gameObject, HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition));
			}
		}
	}

	public void OnClickGvGWeaponInfusedDescription()
	{
		if (!(gvgInfusedDescriptionButton != null))
		{
			return;
		}
		if (equipmentDefinition.Category == EquipmentCategory.Armor && !string.IsNullOrEmpty(equipmentDefinition.InfusedTrait))
		{
			if (GameManager.Instance.gameEconomyData.GetTraitDefinition(equipmentDefinition.InfusedTrait) != null)
			{
				TooltipManager.OpenTextBoxWithText(gvgInfusedDescriptionButton.gameObject, HelpersLocalization.GetGvGInfusedWeaponSpecialDescription(equipmentDefinition));
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(gvgInfusedDescriptionButton.gameObject, LocalizationManager.GetText(equipmentDefinition.InfusedTrait));
			}
		}
		else
		{
			TooltipManager.OpenTextBoxWithText(gvgInfusedDescriptionButton.gameObject, HelpersLocalization.GetGvGInfusedWeaponSpecialDescription(equipmentDefinition));
		}
	}

	public void OnClickApocalypticDescription()
	{
		List<string> list = equipmentItemModel?.GetEquipmentActiveTraits();
		List<string> list2 = equipmentItemModel?.GetEquipmentPassiveTraits();
		List<string> list3 = new List<string>();
		if (list != null)
		{
			list3.AddRange(list);
		}
		else if (equipmentDefinition?.ActiveTraits != null)
		{
			list3.AddRange(equipmentDefinition.ActiveTraits);
		}
		if (list2 != null)
		{
			list3.AddRange(list2);
		}
		else if (equipmentDefinition?.PassiveTraits != null)
		{
			list3.AddRange(equipmentDefinition.PassiveTraits);
		}
		for (int i = 0; i < list3.Count; i++)
		{
			string text = list3[i];
			if (text.Contains("Equipment_Apocalyptic_DMG") || text.Contains("Equipment_Apocalyptic_BS") || text.Contains("Equipment_Apocalyptic_DEF"))
			{
				TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(text);
				TooltipManager.OpenTextBoxWithText(apocalypticButton, HelpersLocalization.GetTraitDescription(traitDefinition));
			}
		}
	}

	public void OpenSPTraitList()
	{
		SPRemoldTraiListPopup sPRemoldTraiListPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraiListPopup) as SPRemoldTraiListPopup;
		if (!(sPRemoldTraiListPopup == null) && equipmentDefinition != null)
		{
			if (equipmentItemModel != null && equipmentItemModel.SpEquipmentRemoldModel != null)
			{
				sPRemoldTraiListPopup.OpenForModel(equipmentItemModel);
			}
			else
			{
				sPRemoldTraiListPopup.OpenForPreview(equipmentDefinition.ID);
			}
		}
	}

	public Vector3 GetSPRemoldDescriptionButtonV()
	{
		if (spRemoldDescriptionButton == null)
		{
			return Vector3.zero;
		}
		return new Vector3(spRemoldDescriptionButton.transform.position.x, spRemoldDescriptionButton.transform.position.y, spRemoldDescriptionButton.transform.position.z);
	}

	private void UpdateSPRemoldUIRaity()
	{
		Helpers.GameObjectSetActive(StateSPRemoldContainer, value: false);
		if (equipmentDefinition != null && !(SPtraitRateLabel == null) && equipmentRarityLevel >= 6)
		{
			Helpers.GameObjectSetActive(StateSPRemoldContainer, value: true);
			SPtraitRateLabel.text = Helpers.GetRateStrForPreviewMin(equipmentDefinition.ID);
			if (equipmentItemModel != null && equipmentItemModel.SpEquipmentRemoldModel != null)
			{
				SPtraitRateLabel.text = equipmentItemModel.SpEquipmentRemoldModel.GetRateStr();
			}
		}
	}

	public void OpenSPRemold()
	{
		SPRemoldMainPopup sPRemoldMainPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldMainPopup) as SPRemoldMainPopup;
		if (sPRemoldMainPopup != null)
		{
			sPRemoldMainPopup.BindData(equipmentItemModel);
			sPRemoldMainPopup.Open();
		}
	}

	private void UpdateRemoldOperateContainer()
	{
		Helpers.GameObjectSetActive(remoldOperateContainer, value: false);
		if (!(remoldOperateContainer == null) && equipmentDefinition != null && equipmentItemModel != null)
		{
			if (isRemoldOperateContainerOpen)
			{
				Helpers.GameObjectSetActive(remoldOperateContainer, value: true);
				SPRemoldSkillMain.Setup(equipmentItemModel, currentOperateSlotIndex);
				SPRemoldSkillOperate.Setup(currentOperateSlotIndex, equipmentItemModel);
			}
			else
			{
				SPRemoldSkillMain.Setup(equipmentItemModel);
			}
			if (!Helpers.IsSkillWeaponBagOpened())
			{
				Helpers.SetSkillWeaponBagOpened(on: true);
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillHelp1);
			}
		}
	}

	public void OnClickSkillInfoPopup()
	{
		SPRemoldTraitsSkillInfoPopup sPRemoldTraitsSkillInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillInfoPopup) as SPRemoldTraitsSkillInfoPopup;
		if (!(sPRemoldTraitsSkillInfoPopup == null))
		{
			sPRemoldTraitsSkillInfoPopup.InitData(equipmentDefinition.ID);
			sPRemoldTraitsSkillInfoPopup.Open();
		}
	}


	#region mycode
	public void CloseWindow()
	{
		ResidencePopup.Instance.UpadateEquipUIData();
		DataManager.Instance.SurvivorManagementPopUp.EquipmentUpgradePopupCurrent = null;
		DataManager.Instance.SurvivorManagementPopUp.remodelTraitIndexCurrent = -1;

		if (defaultPopup) defaultPopup.Close();
		base.Close();
		Destroy(this.gameObject);
	}

	public void SetTraitsLocked(bool isLocked)
	{
		traitsPanel.SetStateImmediate(isLocked);
	}

	public void OnClickBreakthrough()
	{
		var parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainer : null;
		BreakThroughPopup breakThroughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BreakThroughPopup, parent) as BreakThroughPopup;
		if (breakThroughPopup != null)
		{
			breakThroughPopup.OnClose += OnCloseBreakthrough;
			breakThroughPopup.OpenForModel(equipmentItemModel);
		}
	}

	private void OnCloseBreakthrough(HUDElement element, HUDElementConfig hudElementConfig)
	{
		BreakThroughPopup breakThroughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BreakThroughPopup) as BreakThroughPopup;
		if (breakThroughPopup != null)
		{
			breakThroughPopup.OnClose -= OnCloseBreakthrough;
		}
	}
	#endregion
}
