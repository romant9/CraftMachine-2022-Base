using OrbCreationExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class EquipmentButton : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UITexture icon;

	[SerializeField]
	private UISprite chargeBackground;

	[SerializeField]
	private UISprite chargeIcon;

	[SerializeField]
	private UISprite raritySprite;

	[SerializeField]
	private UISprite colorOverlay;

	[SerializeField]
	private UISprite statIcon;

	[SerializeField]
	private UILabel statLabel;

	[SerializeField]
	private UISprite remoldIcon;

	[SerializeField]
	private GameObject upgradeTimeBox;

	[SerializeField]
	private UILabel upgradeTimeBoxLabel;

	[SerializeField]
	private UIProgressBar upgradeProgressBar;

	[SerializeField]
	private UILabel ownerLabel;

	[SerializeField]
	private UISprite ownerBoxBackground;

	[SerializeField]
	private GameObject ownerBox;

	[SerializeField]
	private UILabel currentWeaponLevelLabel;

	[SerializeField]
	private GameObject swapNotAvailableOverlay;

	[SerializeField]
	private GameObject upgradeIndicator;

	[SerializeField]
	private UISprite[] traitsSprites;

	[SerializeField]
	private UISprite breakThroughTraitsSprite;

	[SerializeField]
	private Color traitsLockedColor;

	[SerializeField]
	private UISprite[] rarityStarsSprites;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private GameObject lockedParent;

	[SerializeField]
	private UILabel lockedLabel;

	[SerializeField]
	private GameObject selectionHighlight;

	[SerializeField]
	private GameObject scrapBg;

	[SerializeField]
	private GameObject scrapCostObject;

	[SerializeField]
	private UILabel scrapCostLabel;

	[SerializeField]
	private GameObject scrapCostTokenObject;

	[SerializeField]
	private UILabel scrapCostTokenLabel;

	[SerializeField]
	private GameObject equippedLabel;

	[SerializeField]
	private UISprite classIconSprite;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private EquipmentFavouriteVisibility equipmentFavouriteVisibility;

	[SerializeField]
	private GameObject equipmentFavoriteIcon;

	[SerializeField]
	[Header("Functionality indicators")]
	private GameObject indicatorInfusedEquipment;

	[SerializeField]
	private GameObject indicatorSpecialFunctionalityEquipment;

	[SerializeField]
	private GameObject indicatorRemoldEquipment;

	[SerializeField]
	private GameObject indicatorSpecialFunctionalityAndInfused;

	[SerializeField]
	private GameObject apocalypticEffect;

	[SerializeField]
	private GameObject apocalypticPlus;

	[SerializeField]
	private UILabel BTLevelLabel;

	[SerializeField]
	private UISprite scrapSprite;

	private bool canEquipItem = true;

	private string onClickUIEvent;

	private EquipmentItemModel equipment;

	private EquipmentItemModel equipmentToCompare;

	private SurvivorModel owningSurvivor;

	private bool owningseStatAddedToSurvivor;

	private bool showOwnerAndUpgradeIndicator;

	private bool allowUpgradeIndicator;

	private int equipmentMaxUpgradeLevels = -1;

	private TutorialArrowParent tutorialArrow;

	private EquipScrapMode scrapMode;

	private bool setToScrap;

	private RewardEquipment EquipmentRewardToOpenOnClick;

	private bool attemptedToSwap;

	private bool isSwapCooldownOver = true;

	private bool canSwap = true;

	public bool isDisableScrap = true;

	public bool OpenEquipmentReceivedOnClick { get; set; }

	private void ClearPrivateVariables()
	{
		canEquipItem = true;
		canSwap = true;
		onClickUIEvent = null;
		equipment = null;
		equipmentToCompare = null;
		owningSurvivor = null;
		showOwnerAndUpgradeIndicator = false;
		allowUpgradeIndicator = false;
		equipmentMaxUpgradeLevels = -1;
		tutorialArrow = null;
		scrapMode = EquipScrapMode.Normal;
		setToScrap = false;
		EquipmentRewardToOpenOnClick = null;
	}

	public void Start()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
		setToScrap = false;
		if (isDisableScrap)
		{
			DisableScrapMenu();
		}
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		setToScrap = false;
		DisableScrapMenu();
	}

	public void OnDestroy()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void SetupWeapon(SurvivorModel survivor, bool useStatAddedToSurvivor = false, string clickUIEvent = "SurvivorCardEquipmentClicked", bool showLockedState = true)
	{
		ClearPrivateVariables();
		Setup(survivor.GetWeaponEquipment(), null, survivor, clickUIEvent, showOwnerAndUpgradeIndicator, useStatAddedToSurvivor, showLockedState);
	}

	public void SetupArmor(SurvivorModel survivor, bool useStatAddedToSurvivor = false, string clickUIEvent = "SurvivorCardEquipmentClicked", bool showLockedState = true)
	{
		ClearPrivateVariables();
		Setup(survivor.GetEquipmentOfCategory(EquipmentCategory.Armor), null, survivor, clickUIEvent, showOwnerAndUpgradeIndicator, useStatAddedToSurvivor, showLockedState);
	}

	public void Setup(EquipmentItemModel targetEquipment, EquipmentItemModel compareEquipment, SurvivorModel equipmentOwningSurvivor, string clickUIEvent, bool showOwnerAndUpgradeIndicator, bool useStatAddedToSurvivor = false, bool showLockedState = true)
	{
		if (IsLoadDataManager && RewardHighlight != null) RewardHighlight.SetActive(false);

		onClickUIEvent = clickUIEvent;
		EquipmentRewardToOpenOnClick = null;
		OpenEquipmentReceivedOnClick = false;
		equipment = targetEquipment;
		equipmentToCompare = compareEquipment;
		owningSurvivor = equipmentOwningSurvivor;
		owningseStatAddedToSurvivor = useStatAddedToSurvivor;
		this.showOwnerAndUpgradeIndicator = showOwnerAndUpgradeIndicator;
		allowUpgradeIndicator = showOwnerAndUpgradeIndicator;
		Helpers.GameObjectSetActive(equipmentFavoriteIcon, value: false);
		Update();
		if (equipment != null)
		{
			if (!IsLoadDataManager) checkLockedState(showLockedState);
			Show(icon, show: true);
			Show(raritySprite, show: true);
			Show(statLabel, show: true);
			if (tutorialArrow == null)
			{
				tutorialArrow = GetComponentInChildren<TutorialArrowParent>();
			}
			if (tutorialArrow != null)
			{
				tutorialArrow.Id = ((equipmentToCompare == null) ? "Equipment_Equiped" : "Equipment_Owned");
			}
			if (nameLabel != null)
			{
				nameLabel.text = HelpersLocalization.GetEquipmentName(equipment);
				nameLabel.gradientTop = GameManager.Instance.GetRarityColorData(equipment.RarityLevel).GradientColorTop;
				nameLabel.gradientBottom = GameManager.Instance.GetRarityColorData(equipment.RarityLevel).GradientColorBottom;
			}
			Helpers.GameObjectSetActive(remoldIcon, value: false);
			if (equipment.Definition != null && equipment.Definition.SwitchRemoldMode)
			{
				Helpers.GameObjectSetActive(remoldIcon, value: true);
			}
			if (icon != null)
			{
				icon.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipment);
				if (equipment.Definition.UseSpecialMaterial)
				{
					Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(equipment).specialMaterial;
					icon.material = specialMaterial ?? icon.material;
				}
				else
				{
					icon.material = null;
				}
			}
			HelpersUI.SetSprite(classIconSprite, HelpersGfx.GetSurvivorClassSmallIconName(equipment.EquipmentSurvivorClass));
			int num = equipment.MainStat;
			if (equipmentToCompare != null)
			{
				num = equipment.MainStat - equipmentToCompare.MainStat;
			}
			if (statLabel != null)
			{
				if (useStatAddedToSurvivor)
				{
					if (owningSurvivor != null)
					{
						if (equipment.Definition.Category == EquipmentCategory.Armor)
						{
							statLabel.text = owningSurvivor.GetHitpoints().ToString();
						}
						else
						{
							statLabel.text = owningSurvivor.GetDamageForPreferredWeapon().ToString();
						}
					}
					else
					{
						Debug.LogError("owningSurvivor is null!");
					}
				}
				else if (equipmentToCompare != null)
				{
					statLabel.color = HelpersGfx.GetBrightColorForDamageDifference(num);
					statLabel.text = HelpersString.FormatNumberWithSign(num);
				}
				else
				{
					statLabel.color = Color.white;
					statLabel.text = num.ToString() ?? "";
				}
			}
			if (statIcon != null)
			{
				HelpersUI.SetSprite(statIcon, HelpersGfx.GetEquipmentCategoryIconNameSmall(equipment.Definition.Category));
			}
			if (colorOverlay != null)
			{
				colorOverlay.color = HelpersGfx.GetColorForDamageDifference(num);
				colorOverlay.gameObject.SetActive(equipmentToCompare != null && num != 0);
			}
			if (raritySprite != null)
			{
				HelpersGfx.UpdateSpriteAndKeepScale(raritySprite, HelpersGfx.GetEquipmentRaritySprite(equipment.RarityLevel, equipment.Definition != null && equipment.Definition.SwitchRemoldMode));
			}
			SetApocalypticEffect(equipment.RarityLevel);
			EquipmentItemModel chargeEquipment = equipment.ChargeEquipment;
			if (chargeBackground != null)
			{
				chargeBackground.gameObject.SetActive(chargeEquipment != null);
			}
			if (chargeIcon != null)
			{
				chargeIcon.gameObject.SetActive(chargeEquipment != null);
				if (equipment.ChargeEquipment != null)
				{
					HelpersGfx.UpdateSpriteAndKeepScale(chargeIcon, HelpersGfx.GetChargeEquipmentIconName(equipment.ChargeEquipment));
				}
			}
			if (ownerBox != null && ownerLabel != null)
			{
				if (owningSurvivor == null)
				{
					if (equipment.Owner != null)
					{
						Helpers.GameObjectSetActive(ownerBox, value: true);
						ownerLabel.text = equipment.Owner.Name;
					}
					else
					{
						ownerLabel.text = "";
						Helpers.GameObjectSetActive(ownerBox, value: false);
					}
				}
				else
				{
					EquipmentItemModel equipmentByType = owningSurvivor.GetEquipmentByType(equipment.IsWeaponEquipment);
					if (equipmentByType.Owner == equipment.Owner)
					{
						Helpers.GameObjectSetActive(ownerBox, value: false);
					}
					else if (equipmentByType.Owner != null)
					{
						Helpers.GameObjectSetActive(ownerBox, value: true);
						ownerLabel.text = equipment.Owner?.Name;
						if (currentWeaponLevelLabel != null)
						{
							currentWeaponLevelLabel.text = $"LVL {equipmentByType.StartingLevel.ToString()}";
						}
					}
					else
					{
						Helpers.GameObjectSetActive(ownerBox, value: true);
					}
				}
			}
			if (upgradeIndicator != null)
			{
				upgradeIndicator.SetActive(value: false);
			}
			updateRarityRating(rarityStarsSprites, equipment.RarityLevel);
			if (IsLoadDataManager) UpdateBreakthroughUI();
			updateTraits(traitsSprites, equipment.UpgradeTraits, equipment.Level);
			UpdateLevelLabel(equipment.Level);
			if (scrapCostLabel != null)
			{
				int num2 = Math.Abs(equipment.GetScrapCashier.GetTotalCost(CurrencyType.SurvivalPoints));
				scrapCostLabel.text = num2.ToString();
			}
			SetFavoriteIconVisibility(show: true);
			UpdateTraitIndicators(equipment.Definition);
			Show(amountLabel, show: false);
			if (IsLoadDataManager) UpdateStatLabel();
		}
		else
		{
			Show(nameLabel, show: false);
			Show(statIcon, show: false);
			Show(icon, show: false);
			Show(chargeBackground, show: false);
			Show(chargeIcon, show: false);
			Show(raritySprite, show: false);
			Show(statLabel, show: false);
			Show(colorOverlay, show: false);
			Show(amountLabel, show: false);
			if (upgradeProgressBar != null)
			{
				upgradeProgressBar.gameObject.SetActive(value: false);
			}
		}
	}

	public void Setup(RewardEquipment rewardEquipment, bool allowClick = true, bool traitsUnknown = false)
	{
		Setup(rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager), rewardEquipment.RarityLevel, rewardEquipment.StartingLevel, rewardEquipment.Amount);
		if (!traitsUnknown)
		{
			List<UpgradeTraitsData> upgradeTraitsDataList = new List<UpgradeTraitsData>();
			rewardEquipment.PreviewUpgradeTraitsDataForEquipment(GameManager.Instance.modelManager, out upgradeTraitsDataList);
			updateTraits(traitsSprites, upgradeTraitsDataList, rewardEquipment.StartingLevel);
		}
		if (allowClick)
		{
			EquipmentRewardToOpenOnClick = rewardEquipment;
		}
		SetFavoriteIconVisibility(show: false);
	}

	public void Setup(EquipmentDefinition equipmentDefinition, int rarityLevel, int level, int amount = 1)
	{
		ClearPrivateVariables();
		Update();
		OpenEquipmentReceivedOnClick = false;
		EquipmentRewardToOpenOnClick = null;
		Show(icon, show: true);
		Show(raritySprite, show: true);
		Show(statLabel, show: false);
		Show(statIcon, show: false);
		Show(chargeBackground, show: false);
		Show(chargeIcon, show: false);
		SetScrapMode(scrapMode);
		if (amount > 1)
		{
			amountLabel.text = amount.ToString();
			Show(amountLabel, show: true);
		}
		else
		{
			Show(amountLabel, show: false);
		}
		if (upgradeIndicator != null)
		{
			upgradeIndicator.SetActive(value: false);
		}
		if (upgradeProgressBar != null)
		{
			upgradeProgressBar.gameObject.SetActive(value: false);
		}
		if (ownerBox != null)
		{
			ownerBox.gameObject.SetActive(value: false);
		}
		if (nameLabel != null)
		{
			nameLabel.text = HelpersLocalization.GetEquipmentName(equipmentDefinition.ID);
			nameLabel.gradientTop = GameManager.Instance.GetRarityColorData(rarityLevel).GradientColorTop;
			nameLabel.gradientBottom = GameManager.Instance.GetRarityColorData(rarityLevel).GradientColorBottom;
		}
		Helpers.GameObjectSetActive(remoldIcon, value: false);
		if (equipmentDefinition != null && equipmentDefinition.SwitchRemoldMode)
		{
			Helpers.GameObjectSetActive(remoldIcon, value: true);
		}
		if (icon != null)
		{
			icon.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			if (equipmentDefinition.UseSpecialMaterial)
			{
				Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(equipmentDefinition).specialMaterial;
				icon.material = specialMaterial ?? icon.material;
			}
			else
			{
				icon.material = null;
			}
		}
		HelpersUI.SetSprite(classIconSprite, HelpersGfx.GetSurvivorClassSmallIconName(equipmentDefinition.SurvivorClass));
		UpdateLevelLabel(level);
		if (raritySprite != null)
		{
			HelpersGfx.UpdateSpriteAndKeepScale(raritySprite, HelpersGfx.GetEquipmentRaritySprite(rarityLevel, equipmentDefinition?.SwitchRemoldMode ?? false));
		}
		SetApocalypticEffect(rarityLevel);
		updateRarityRating(rarityStarsSprites, rarityLevel);
		updateTraits(traitsSprites, null, -1);
		SetFavoriteIconVisibility(show: false);
		UpdateTraitIndicators(equipmentDefinition);
	}

	private IEnumerator UpdateFavoriteTagPosition(bool show)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Helpers.GameObjectSetActive(equipmentFavouriteVisibility, show);
		if (show && equipmentFavouriteVisibility != null)
		{
			equipmentFavouriteVisibility.UpdateVisibility(equipment);
		}
	}

	public void UpdateLevelLabel(int level)
	{
		if (Helpers.GameObjectSetActive(levelLabel, level > 0))
		{
			levelLabel.text = level.ToString() ?? "";
		}
	}

	public void RefreshEquipmentToCompare(EquipmentItemModel compareEquipment)
	{
		equipmentToCompare = compareEquipment;
	}

	private void Show(UIWidget widget, bool show)
	{
		if (widget != null)
		{
			widget.gameObject.SetActive(show);
		}
	}

	public void OnEquipmentButtonClicked()
	{
		if (IsLoadDataManager && IsForProtectors)
		{
			SurvivorStatisticsPanel panel = transform.GetFirstComponentInParents<SurvivorStatisticsPanel>();
			if (equipment.IsWeaponEquipment)
			{
				panel.OpenWeaponInfoPopup();
			}
			else
			{
				panel.OpenArmorInfoPopup();
			}
			return;
		}
		if (EquipmentRewardToOpenOnClick != null)
		{
			OpenForReward();
		}
		else if (OpenEquipmentReceivedOnClick)
		{
			OpenEquipmentReceived();
		}
		else if (scrapMode != EquipScrapMode.Normal)
		{
			if (!setToScrap)
			{
				bool flag = false;
				if (scrapMode == EquipScrapMode.Scrap)
				{
					bool flag2 = equipment.Definition.SwitchRemoldMode && !GameManager.Instance.gameEconomyData.ConfigData.RemoldEquipCanBeBreakDown;
					flag = equipment.Owner == null && !equipment.IsUpgrading() && !equipment.IsFavourite && !flag2;
				}
				if (flag)
				{
					UIEvent.Send("EquipmentAddToScrapList", equipment);
					setToScrap = true;
				}
			}
			else
			{
				UIEvent.Send("EquipmentRemoveFromScrapList", equipment);
				setToScrap = false;
			}
			if (selectionHighlight != null)
			{
				selectionHighlight.SetActive(setToScrap);
			}
		}
		else if ((!attemptedToSwap && canEquipItem) || (attemptedToSwap && canSwap))
		{
			UIEvent.Send(onClickUIEvent, this);
			EventManager.NotifyClick("Click_Equipment");
		}
		else if (!canSwap && attemptedToSwap && isSwapCooldownOver)
		{
			TweenAlpha component = swapNotAvailableOverlay.GetComponent<TweenAlpha>();
			Helpers.GameObjectSetActive(swapNotAvailableOverlay, value: true);
			component.ResetToBeginning();
			component.PlayForward();
			isSwapCooldownOver = false;
		}
	}

	public void OnEquipmentSelect(string starNum)
	{
		int num = 0;
		switch (starNum)
		{
		case "Popup.Workshop.AutoScrap.Off":
			num = 0;
			break;
		case "Popup.Workshop.AutoScrap.ThreeStars":
			num = 3;
			break;
		case "Popup.Workshop.AutoScrap.FourStars":
			num = 4;
			break;
		case "Popup.Workshop.AutoScrap.FiveStars":
			num = 5;
			break;
		}
		if (scrapMode != EquipScrapMode.Normal)
		{
			if (equipment.Owner == null && !equipment.IsUpgrading() && !equipment.IsFavourite && equipment.RarityLevel + 1 <= num && num != 0 && string.IsNullOrEmpty(HelpersLocalization.GetEquipmentSpecialDescription(equipment.Definition)) && string.IsNullOrEmpty(equipment.Definition.SpecialTrait) && string.IsNullOrEmpty(equipment.Definition.InfusedTrait))
			{
				UIEvent.Send("EquipmentAddToScrapList", equipment);
				setToScrap = true;
			}
			else
			{
				UIEvent.Send("EquipmentRemoveFromScrapList", equipment);
				setToScrap = false;
			}
			if (selectionHighlight != null)
			{
				selectionHighlight.SetActive(setToScrap);
			}
		}
	}

	public void OnSelectionHighlight(bool isEnable)
	{
		Helpers.GameObjectSetActive(selectionHighlight, isEnable);
	}

	public void OnSwapNotAvailableAnimationFinished()
	{
		Helpers.GameObjectSetActive(swapNotAvailableOverlay, value: false);
		isSwapCooldownOver = true;
	}

	public EquipmentItemModel GetEquipment()
	{
		return equipment;
	}

	public SurvivorModel GetOwningSurvivor()
	{
		return owningSurvivor;
	}

	public void AllowUpgradeIndicator(bool value)
	{
		allowUpgradeIndicator = value;
	}

	protected void Update()
	{
		if (equipment != null)
		{
			if (equipment.IsUpgrading())
			{
				HelpersUI.SetContentToLabel(upgradeTimeBoxLabel, Helpers.FormatTimeNoZero(equipment.TimedActionModel.MillisecondsTillCompletion));
			}
			Helpers.GameObjectSetActive(upgradeTimeBox, equipment.IsUpgrading());
			Helpers.GameObjectSetActive(upgradeTimeBoxLabel, equipment.IsUpgrading());
			if (allowUpgradeIndicator && !equipment.IsUpgrading())
			{
				Helpers.GameObjectSetActive(upgradeIndicator, equipment.CanUpgrade && equipment.GetUpgradeCashier(instantUpgrade: false).CanAfford());
			}
			else
			{
				Helpers.GameObjectSetActive(upgradeIndicator, value: false);
			}
			if (upgradeProgressBar != null)
			{
				if (equipmentMaxUpgradeLevels == -1)
				{
					equipmentMaxUpgradeLevels = equipment.manager.Player.gameEconomyData.GetMaxNumberOfUpgrades(TWDModel.UpgradeType.EquipmentUpgrade);
				}
				upgradeProgressBar.value = (float)(equipment.Level - equipment.StartingLevel) / (float)equipmentMaxUpgradeLevels;
			}
		}
		else
		{
			Helpers.GameObjectSetActive(upgradeTimeBox, value: false);
			Helpers.GameObjectSetActive(upgradeTimeBoxLabel, value: false);
		}
		UpdateBreakthroughUI();
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "WorkshopPopupUnSelectEvent":
			break;
		case "SPRemoldLockChanged":
			break;
		case "SPRemoldRandomChanged":
			break;
		case "SPRemoldUpgradeChanged":
			break;
		case "SetEquipmentScrapMode":
			if (!IsLoadDataManager) SetScrapMode((EquipScrapMode)parameter);
			break;
		case "BreakThroughed":
			if (equipment != null)
			{
				UpdateBreakthroughUI();
				updateTraits(traitsSprites, equipment.UpgradeTraits, equipment.Level);
				UpdateStatLabel();
			}
			break;
		case "EquipmentRemodelSelectioned":
			if (equipment != null)
			{
				UpdateBreakthroughUI();
				updateTraits(traitsSprites, equipment.UpgradeTraits, equipment.Level);
				UpdateStatLabel();
			}
			break;
		case "WorkshopPopupSelectEvent":
		{
			string starNum = (string)parameter;
			OnEquipmentSelect(starNum);
			break;
		}
		case "EquipmentInstantUpgraded":
			if (equipment != null)
			{
				UpdateStatLabel();
			}
			break;
		case "NewRecommendEquipmentSelected":
			if (parameter is EquipmentButton equipmentButton)
			{
				OnSelectionHighlight(this == equipmentButton);
			}
			break;
		}
	}

	private void SetScrapMode(EquipScrapMode enabled)
	{
		if (scrapBg == null)
		{
			return;
		}
		scrapMode = enabled;
		scrapBg.SetActive(scrapMode != EquipScrapMode.Normal);
		scrapCostObject.SetActive(scrapMode != EquipScrapMode.Normal && equipment != null && equipment.Owner == null && !equipment.IsUpgrading() && !equipment.IsFavourite);
		equippedLabel.SetActive(scrapMode != EquipScrapMode.Normal && equipment != null && (equipment.Owner != null || equipment.IsUpgrading() || equipment.IsFavourite));
		UILabel componentInChildren = equippedLabel.GetComponentInChildren<UILabel>();
		if (componentInChildren != null && equipment != null)
		{
			string text = "";
			if (equipment.IsUpgrading())
			{
				text = "EquipmentCard.Upgrading";
			}
			else if (equipment.Owner != null)
			{
				text = "EquipmentCard.Equipped";
			}
			else if (equipment.IsFavourite)
			{
				text = "EquipmentCard.Favourite";
			}
			else if (equipment.Definition.SwitchRemoldMode && !GameManager.Instance.gameEconomyData.ConfigData.RemoldEquipCanBeBreakDown)
			{
				text = "System.EquipmentScrap.FuncInfo3";
			}
			if (!string.IsNullOrEmpty(text))
			{
				componentInChildren.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text);
				equippedLabel.SetActive(value: true);
				scrapCostObject.SetActive(value: false);
			}
			if (enabled == EquipScrapMode.Scrap && scrapCostLabel != null && equipment != null)
			{
				int num = Math.Abs(equipment.GetScrapCashier.GetTotalCost(CurrencyType.SurvivalPoints));
				scrapCostLabel.text = num.ToString();
				bool flag = equipment.Definition.SwitchRemoldMode && !GameManager.Instance.gameEconomyData.ConfigData.RemoldEquipCanBeBreakDown;
				Helpers.GameObjectSetActive(scrapCostTokenObject, equipment.IsCanBreak && equipment.Owner == null && !equipment.IsFavourite && !flag);
				if (equipment.IsCanBreak && equipment.Owner == null && !equipment.IsFavourite)
				{
					int num2 = 0;
					if (equipment.BreakthroughLevel > 0)
					{
						List<EquipmentItemModel> list = new List<EquipmentItemModel>();
						list.Add(equipment);
						foreach (IReward rewards in GameManager.Instance.playerModel.Equipment.GetEquipmentListScrapReward(list).RewardsList)
						{
							if (rewards.Type == RewardType.Currency && rewards is RewardCurrency { CurrencyType: CurrencyType.ApocalypticEquipToken } rewardCurrency)
							{
								num2 = rewardCurrency.Amount;
							}
						}
					}
					else
					{
						num2 = Math.Abs(equipment.GetScrapCashier.GetTotalCost(CurrencyType.ApocalypticEquipToken));
					}
					scrapCostTokenLabel.text = num2.ToString();
				}
			}
		}
		if (enabled == EquipScrapMode.Normal)
		{
			DisableScrapMenu();
		}
	}

	private void OnLanguageChanged()
	{
		SetScrapMode(scrapMode);
	}

	private void DisableScrapMenu()
	{
		setToScrap = false;
		Helpers.GameObjectSetActive(selectionHighlight, value: false);
		Helpers.GameObjectSetActive(scrapBg, value: false);
		Helpers.GameObjectSetActive(scrapCostTokenObject, value: false);
		Helpers.GameObjectSetActive(equippedLabel, value: false);
		Helpers.GameObjectSetActive(equipmentFavouriteVisibility, value: false);
	}

	private void checkLockedState(bool showLockedState)
	{
		if (!(lockedParent != null) || !(lockedLabel != null))
		{
			return;
		}
		lockedParent.SetActive(value: false);
		if (equipment != null && owningSurvivor != null && equipment != null)
		{
			canEquipItem = equipment.StartingLevel <= owningSurvivor.Level;
			if (equipment.Owner != null)
			{
				canSwap = owningSurvivor.GetEquipmentByType(equipment.IsWeaponEquipment)?.StartingLevel <= equipment.Owner.Level && canEquipItem;
				attemptedToSwap = true;
			}
			if (!canEquipItem && lockedParent != null && lockedLabel != null && showLockedState)
			{
				lockedParent.SetActive(value: true);
				lockedLabel.text = LocalizationManager.GetText("Popup.EquipmentButton.Locked{Parameter}", equipment.StartingLevel);
			}
			else if (lockedParent != null)
			{
				lockedParent.SetActive(value: false);
			}
		}
	}

	private void updateTraits(UISprite[] traitsArray, List<UpgradeTraitsData> upgradeTraitsDataList, int equipmentLevel)
	{
		if (traitsArray != null && traitsArray.Length != 0 && upgradeTraitsDataList == null)
		{
			for (int i = 0; i < traitsArray.Length; i++)
			{
				traitsArray[i].gameObject.SetActive(value: true);
				traitsArray[i].spriteName = HelpersGfx.GetEquipmentTraitIconName(null);
				traitsArray[i].color = traitsLockedColor;
			}
			return;
		}
		if (traitsArray != null && traitsArray.Length != 0 && upgradeTraitsDataList == null)
		{
			for (int j = 0; j < traitsArray.Length; j++)
			{
				traitsArray[j].gameObject.SetActive(value: false);
			}
		}
		if (traitsArray == null || traitsArray.Length == 0 || upgradeTraitsDataList == null)
		{
			return;
		}
		for (int k = 0; k < traitsArray.Length; k++)
		{
			if (!(traitsArray[k] != null))
			{
				continue;
			}
			if (upgradeTraitsDataList.Count > k + 1)
			{
				traitsArray[k].gameObject.SetActive(value: true);
				traitsArray[k].spriteName = HelpersGfx.GetEquipmentTraitIconName(upgradeTraitsDataList[k + 1]);
				if (upgradeTraitsDataList[k + 1].UnlockingLevel > equipmentLevel)
				{
					traitsArray[k].color = traitsLockedColor;
				}
				else
				{
					traitsArray[k].color = Color.white;
				}
			}
			else
			{
				traitsArray[k].gameObject.SetActive(value: false);
			}
		}
	}

	private void updateRarityRating(UISprite[] starsArray, int rarityLevel)
	{
		for (int i = 0; i < starsArray.Length; i++)
		{
			if (starsArray[i] != null && starsArray[i].gameObject != null)
			{
				if (rarityLevel >= i && starsArray[i] != null && starsArray[i].gameObject != null)
				{
					starsArray[i].gameObject.SetActive(value: true);
				}
				else
				{
					starsArray[i].gameObject.SetActive(value: false);
				}
			}
		}
	}

	private void OpenForReward()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Helpers.OpenEquipmentUpgradePopupPreview(EquipmentRewardToOpenOnClick.EquipmentDefinition(GameManager.Instance.modelManager), EquipmentRewardToOpenOnClick.RarityLevel).ShowNextLevel = false;
		if (!IsLoadDataManager) CampHUD.Get().PauseCurrencyMeters = false;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
	}

	private void OpenEquipmentReceived()
	{
		EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		obj.ShowNextLevel = false;
		obj.OpenForModel(GetEquipment());
		obj.ShowEquipmentReceivedVersion();
		if (!IsLoadDataManager) CampHUD.Get().PauseCurrencyMeters = false;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
	}

	private void SetFavoriteIconVisibility(bool show)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(UpdateFavoriteTagPosition(show));
			return;
		}
		Helpers.GameObjectSetActive(equipmentFavouriteVisibility, show);
		if (show && equipmentFavouriteVisibility != null)
		{
			equipmentFavouriteVisibility.UpdateVisibility(equipment);
		}
	}

	private void UpdateTraitIndicators(EquipmentDefinition equipmentDefinition)
	{
		bool flag = !string.IsNullOrEmpty(equipmentDefinition.InfusedTrait);
		bool flag2 = !string.IsNullOrEmpty(equipmentDefinition.SpecialTrait) || !string.IsNullOrEmpty(HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition));
		Helpers.GameObjectSetActive(indicatorSpecialFunctionalityAndInfused, flag && flag2);
		Helpers.GameObjectSetActive(indicatorInfusedEquipment, flag && !flag2);
		Helpers.GameObjectSetActive(indicatorSpecialFunctionalityEquipment, flag2 && !flag);
		Helpers.GameObjectSetActive(indicatorRemoldEquipment, equipmentDefinition.SwitchRemoldMode);
		if (equipmentDefinition.SwitchRemoldMode)
		{
			Helpers.GameObjectSetActive(indicatorSpecialFunctionalityEquipment, value: false);
		}
	}

	private void SetApocalypticEffect(int rarityLevel)
	{
		HelpersGfx.SetApocalypticEffectActive(apocalypticEffect, rarityLevel);
		HelpersGfx.SetApocalypticEffectActive(apocalypticPlus, rarityLevel);
	}

	private void UpdateBreakthroughUI()
	{
		Helpers.GameObjectSetActive(breakThroughTraitsSprite, value: false);
		Helpers.GameObjectSetActive(BTLevelLabel, value: false);
		if (equipment != null)
		{
			if (equipment.BreakthroughLevel > 0)
			{
				BTLevelLabel.text = LocalizationManager.GetText("Popup.Workshop.BreakthroughsLevel", equipment.BreakthroughLevel);
				Helpers.GameObjectSetActive(BTLevelLabel, value: true);
			}
			UpgradeTraitsData breakThroughUpgradeTraitsData = equipment.GetBreakThroughUpgradeTraitsData();
			if (breakThroughUpgradeTraitsData != null)
			{
				Helpers.GameObjectSetActive(breakThroughTraitsSprite, value: true);
				breakThroughTraitsSprite.spriteName = HelpersGfx.GetEquipmentTraitIconName(breakThroughUpgradeTraitsData);
			}
		}
	}

	private void UpdateStatLabel()
	{
		if (!OfflineManager.IsNoAddRewards && NewPhonePopup.Instance)
		{
			//вычисление купленного за рации снаряжения
			List<int> counts = NewPhonePopup.Instance.PhoneWeaponContainer != null ? NewPhonePopup.Instance.PhoneWeaponContainer.equipCount : null;
			if (counts != null && counts.Count > 0 && RewardHighlight != null)
			{
				var equipments = DataManager.Instance.Player.Equipment;

				if (equipment.Definition.Category == EquipmentCategory.Armor)
				{
					if (equipments.Armors.IndexOf(equipment) > counts[0] - 1)
					{
						RewardHighlight.SetActive(true);
					}
				}
				else if (equipment.Definition.Category == EquipmentCategory.MeleeWeapon)
				{
					if (equipments.MeleeWeapons.IndexOf(equipment) > counts[1] - 1)
					{
						RewardHighlight.SetActive(true);
					}
				}
				else if (equipment.Definition.Category == EquipmentCategory.RangeWeapon)
				{
					if (equipments.RangeWeapons.IndexOf(equipment) > counts[2] - 1)
					{
						//UnityEngine.Debug.LogError("YES " + equipment.EquipmentDefinitionIdentifier);
						RewardHighlight.SetActive(true);
					}
				}
			}
		}
		int num = equipment.MainStat;
		if (equipmentToCompare != null)
		{
			num = equipment.MainStat - equipmentToCompare.MainStat;
		}
		if (!(statLabel != null))
		{
			return;
		}
		if (owningseStatAddedToSurvivor)
		{
			if (owningSurvivor != null)
			{
				if (equipment.Definition.Category == EquipmentCategory.Armor)
				{
					statLabel.text = owningSurvivor.GetHitpoints().ToString();
				}
				else
				{
					statLabel.text = owningSurvivor.GetDamageForPreferredWeapon().ToString();
				}
			}
			else
			{
				Debug.LogError("owningSurvivor is null!");
			}
		}
		else if (equipmentToCompare != null)
		{
			statLabel.color = HelpersGfx.GetBrightColorForDamageDifference(num);
			statLabel.text = HelpersString.FormatNumberWithSign(num);
		}
		else
		{
			statLabel.color = Color.white;
			statLabel.text = num.ToString() ?? "";
		}
	}


	#region myparams
	public bool IsForProtectors = false;

	public GameObject RewardHighlight;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion
}
