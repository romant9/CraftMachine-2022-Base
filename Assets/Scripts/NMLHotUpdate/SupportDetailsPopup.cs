using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SupportDetailsPopup : HUDElement
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UISprite enhancedSprite;

	[SerializeField]
	private UILabel skillNameLabel;

	[SerializeField]
	private UILabel skillDescriptionLabel;

	[SerializeField]
	private UILabel cooldownLabel;

	[SerializeField]
	private UILabel currencyLabel;

	[SerializeField]
	private UILabel levelRankLabel;

	[SerializeField]
	private UITexture[] supportIconTextures;

	[SerializeField]
	private UISprite skillIconSprite;

	[SerializeField]
	private UISprite currencyIconSprite;

	[SerializeField]
	private UISprite[] levelRepresentations;

	[SerializeField]
	private GameObject promoteHolder;

	[SerializeField]
	private LocalizationUIUpdater promoteLabel;

	[SerializeField]
	private GameObject unlockHolder;

	[SerializeField]
	private LocalizationUIUpdater unlockLabel;

	[SerializeField]
	private GameObject needMoreTokensHolder;

	[SerializeField]
	private UIProgressBar tokenProgressBar;

	[SerializeField]
	private GameObject currency2Go;

	[SerializeField]
	private UISprite currency2Sprite;

	[SerializeField]
	private UIProgressBar currency2ProgressBar;

	[SerializeField]
	private UILabel currency2Label;

	[SerializeField]
	private Color[] rarityColors;

	[SerializeField]
	private TweenColor colorTween;

	[SerializeField]
	private int upgradeColorTweenGroup;

	[SerializeField]
	private int upgradeStarTweenGroup;

	[SerializeField]
	private GameObject upgradeHolder;

	[SerializeField]
	private UIScrollBar infoScrollBar;

	[SerializeField]
	private UIScrollBar promotionInfoScrollbar;

	[SerializeField]
	private GameObject supportEffectGo;

	[SerializeField]
	private GameObject promotePreview;

	[SerializeField]
	private SupportStatPreviewEntry[] statPreviews;

	[SerializeField]
	private GameObject levelUpGo;

	[SerializeField]
	private GameObject talentGo;

	[SerializeField]
	private UIButtonToggleSet toggleSet;

	[Header("Talent")]
	[SerializeField]
	private UILabel attackNumLabel;

	[SerializeField]
	private UILabel defenseNumLabel;

	[SerializeField]
	private UIButton trait1Button;

	[SerializeField]
	private UIButton trait2Button;

	[SerializeField]
	private UIButton trait3Button;

	[SerializeField]
	private SupportTraitButton supportTraitButton1;

	[SerializeField]
	private SupportTraitButton supportTraitButton2;

	[SerializeField]
	private SupportTraitButton supportTraitButton3;

	[SerializeField]
	private UILabel tokenNormalNumLabel;

	[SerializeField]
	private UILabel tokenAdvancedNumLabel;

	[SerializeField]
	private UILabel tokenFragmentNumLabel;

	[SerializeField]
	private UISprite tokenFragmentSprite;

	[SerializeField]
	private UIButton promoteButton;

	[Header("TalentTrait")]
	[SerializeField]
	private GameObject supportTraitGo;

	[SerializeField]
	private SupportTraitCardList supportTraitCardList;

	private SupportModel supportModel;

	private bool isUpgradable;

	private bool canRedirectToTokens;

	private SupportTraitButton[] _traitButtons;

	private Action upgraded;

	private bool IsInPrePromoteState => promotePreview.activeSelf;

	public void Awake()
	{
		_traitButtons = new SupportTraitButton[3] { supportTraitButton1, supportTraitButton2, supportTraitButton3 };
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SupportDetailSelectedEvent")
		{
			RefreshTalent();
		}
	}

	public void Show(SupportModel model, bool canUpgrade, Action onUpgrade, bool shouldRedirectToTokens = true)
	{
		supportModel = model;
		upgraded = onUpgrade;
		isUpgradable = canUpgrade;
		canRedirectToTokens = shouldRedirectToTokens;
		infoScrollBar.value = 0f;
		promotionInfoScrollbar.value = 0f;
		RefreshTab();
		OnClickLevelUpButton();
	}

	private void RefreshLevelUp()
	{
		string supportId = supportModel.SupportId;
		if (supportModel.definition.Category == 1)
		{
			Helpers.GameObjectSetActive(enhancedSprite.gameObject, value: true);
			nameLabel.leftAnchor.Set(0f, 50f);
		}
		else
		{
			Helpers.GameObjectSetActive(enhancedSprite.gameObject, value: false);
			nameLabel.leftAnchor.Set(0f, 0f);
		}
		nameLabel.text = HelpersLocalization.GetSupportName(supportId);
		UITexture[] array = supportIconTextures;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mainTexture = HelpersGfx.LoadSupportIcon(supportId);
		}
		skillIconSprite.spriteName = HelpersGfx.GetSupportSkillIconName(supportId);
		currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(supportModel.Currency);
		upgradeHolder.SetActive(isUpgradable);
		RefreshLevel(instant: true);
	}

	public void RefreshTalent()
	{
		HelpersUI.SetContentToLabel(attackNumLabel, supportModel.GetAttack().ToString());
		HelpersUI.SetContentToLabel(defenseNumLabel, supportModel.GetHP().ToString());
		HelpersUI.SetContentToLabel(tokenNormalNumLabel, GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.PrimarySupportTalentToken).ToString());
		HelpersUI.SetContentToLabel(tokenAdvancedNumLabel, GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.AdvancedSupportTalentToken).ToString());
		int supportTalentSlot = supportModel.SupportTalentSlot;
		for (int i = 0; i < _traitButtons.Length; i++)
		{
			int value;
			bool flag = supportModel.SlotAssembledTalentIds.TryGetValue(i, out value);
			SupportTraitType type = ((i <= supportTalentSlot - 1) ? ((!flag || value == 0) ? SupportTraitType.Empty : SupportTraitType.Trait) : SupportTraitType.Lock);
			SupportTalentDefinition supportTalentDefinitionById = GameManager.Instance.gameEconomyData.GetSupportTalentDefinitionById(value);
			if (supportTalentDefinitionById != null)
			{
				SupportTalentTreeBranchDefinition supportTalentTreeBranchDefinitionByBranchId = GameManager.Instance.gameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(supportTalentDefinitionById.SupportTalentId);
				_traitButtons[i].SetContent(type, supportTalentTreeBranchDefinitionByBranchId.Icon);
			}
			else
			{
				_traitButtons[i].SetContent(type);
			}
		}
		tokenFragmentSprite.spriteName = HelpersGfx.GetCurrencyIconName(supportModel.Currency);
		CurrencyModel currency = supportModel.manager.Player.GetCurrency(supportModel.Currency);
		tokenFragmentNumLabel.text = currency.Value.ToString();
	}

	private void RefreshLevel(bool instant)
	{
		SetPromoteActive(active: false);
		if (isUpgradable)
		{
			UpdateButtonTexts();
		}
		skillDescriptionLabel.text = HelpersLocalization.GetSupportSkillDescription(supportModel);
		if (supportModel.definition.Category == 1)
		{
			cooldownLabel.text = LocalizationManager.GetText("Support.Cooldown_Type1");
		}
		else if (supportModel.definition.Category == 0)
		{
			cooldownLabel.text = HelpersLocalization.GetSupportCooldownText(supportModel.Cooldown);
		}
		CurrencyModel currency = supportModel.manager.Player.GetCurrency(supportModel.Currency);
		KeyValuePair<CurrencyType, int>? currencyKeyValue = GetCurrencyKeyValue();
		bool flag = supportModel.Level >= supportModel.MaxLevel;
		if (currencyKeyValue.HasValue)
		{
			Helpers.GameObjectSetActive(currency2Go, value: true);
			CurrencyModel currency2 = supportModel.manager.Player.GetCurrency(currencyKeyValue.Value.Key);
			currency2ProgressBar.value = (flag ? 1f : Mathf.Clamp01((float)currency2.Value / (float)currencyKeyValue.Value.Value));
			currency2Sprite.spriteName = HelpersGfx.GetCurrencyIconName(currency2.Type);
			currency2Label.text = (flag ? LocalizationManager.GetText("Popup.UpgradeBuilding.MaxLevelReachedMessage") : $"{currency2.Value}/{currencyKeyValue.Value.Value}");
		}
		else
		{
			Helpers.GameObjectSetActive(currency2Go, value: false);
		}
		int supportTokenValue = GetSupportTokenValue();
		if (supportTokenValue != 0)
		{
			currencyLabel.text = (flag ? LocalizationManager.GetText("Popup.UpgradeBuilding.MaxLevelReachedMessage") : $"{currency.Value}/{supportTokenValue}");
			tokenProgressBar.value = (flag ? 1f : Mathf.Clamp01((float)currency.Value / (float)supportTokenValue));
		}
		else if (flag)
		{
			if (supportModel.definition.Category == 1)
			{
				Helpers.GameObjectSetActive(currency2Go, value: true);
				currency2Label.text = LocalizationManager.GetText("Popup.UpgradeBuilding.MaxLevelReachedMessage");
				currency2ProgressBar.value = 1f;
			}
			currencyLabel.text = LocalizationManager.GetText("Popup.UpgradeBuilding.MaxLevelReachedMessage");
			tokenProgressBar.value = 1f;
		}
		levelRankLabel.text = HelpersLocalization.GetSupportLevelRankLabel(supportModel.Level);
		for (int i = 0; i < levelRepresentations.Length; i++)
		{
			levelRepresentations[i].gameObject.SetActive(i < supportModel.Level);
			levelRepresentations[i].spriteName = "Ui_Rarity_Star";
		}
		for (int j = 0; j < supportModel.Level - levelRepresentations.Length; j++)
		{
			levelRepresentations[j].spriteName = "Ui_Rarity_Star_Tier3";
		}
		bool canUpgrade = supportModel.CanUpgrade;
		promoteHolder.SetActive(canUpgrade && supportModel.Level > 0);
		unlockHolder.SetActive(canUpgrade && supportModel.Level <= 0);
		needMoreTokensHolder.SetActive(!canUpgrade && supportModel.Level < supportModel.MaxLevel && EndlessModeHelpers.IsEndlessModeActive() && canRedirectToTokens);
		TweenManager.StopTweenGroup(base.gameObject, upgradeStarTweenGroup);
		TweenManager.FinishTweenGroup(base.gameObject, upgradeStarTweenGroup);
		Helpers.GameObjectSetActive(supportEffectGo, supportModel.Level > levelRepresentations.Length);
		TweenManager.FinishTweenGroup(base.gameObject, upgradeColorTweenGroup);
		colorTween.from = colorTween.to;
		colorTween.to = GetRarityColor();
		TweenManager.PlayTweenGroup(base.gameObject, upgradeColorTweenGroup);
		if (instant)
		{
			TweenManager.FinishTweenGroup(base.gameObject, upgradeColorTweenGroup);
		}
	}

	private void RefreshTab()
	{
		UIButtonToggle[] getUIButtonToggleList = toggleSet.GetUIButtonToggleList;
		toggleSet.SetSelectedIndex(0);
		if (getUIButtonToggleList != null && getUIButtonToggleList.Length > 1 && getUIButtonToggleList[1] != null)
		{
			if (GameManager.Instance.gameEconomyData.ConfigData.SupportTalentUnlockToggle && supportModel.Level > 0 && GameManager.Instance.playerModel.CouncilLevel >= GameManager.Instance.gameEconomyData.ConfigData.SupportTalentUnlockAtCouncilLevel)
			{
				Helpers.GameObjectSetActive(toggleSet.GetUIButtonToggleList[1].gameObject, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(toggleSet.GetUIButtonToggleList[1].gameObject, value: false);
			}
		}
	}

	private Color GetRarityColor()
	{
		return rarityColors[Mathf.Clamp(supportModel.Level - 1, 0, rarityColors.Length - 1)];
	}

	public void PromoteClick()
	{
		if (!supportModel.CanUpgrade)
		{
			return;
		}
		if (!IsInPrePromoteState)
		{
			SetPromoteActive(active: true);
			UpdateButtonTexts();
			UISprite uISprite;
			if (supportModel.Level < levelRepresentations.Length)
			{
				uISprite = levelRepresentations[supportModel.Level];
				uISprite.gameObject.SetActive(value: true);
			}
			else
			{
				uISprite = levelRepresentations[supportModel.Level - levelRepresentations.Length];
				uISprite.spriteName = "Ui_Rarity_Star_Tier3";
			}
			TweenManager.PlayTweenGroup(uISprite.gameObject, upgradeStarTweenGroup);
		}
		else
		{
			Helpers.ExecuteCommand(new UpgradeSupportCommand(supportModel.SupportId));
			RefreshLevel(instant: false);
			upgraded?.Invoke();
		}
	}

	public void OnClickPromoteTree()
	{
		((SupportTalentPopup)HUDManager.TryOpenPopup(UIType.SupportTalentPopup)).Show(supportModel);
	}

	private void SetPromoteActive(bool active)
	{
		promotePreview.SetActive(active);
		if (active)
		{
			for (int i = 0; i < statPreviews.Length; i++)
			{
				statPreviews[i].Set(supportModel, i);
			}
		}
	}

	private void UpdateButtonTexts()
	{
		if (promoteHolder.activeSelf)
		{
			promoteLabel.LocalizationKey = (IsInPrePromoteState ? "Button.Confirm" : "Popup.SurvivorInfo.Button.Promote");
			promoteLabel.UpdateContent();
		}
		else if (unlockHolder.activeSelf)
		{
			unlockLabel.LocalizationKey = (IsInPrePromoteState ? "Button.Confirm" : "Button.Unlock");
			unlockLabel.UpdateContent();
		}
	}

	public void GetMoreTokensClick()
	{
		MissionHubNavigation.TryOpenEndlessMode();
	}

	public void OnClickTrait1Button()
	{
		if (CheckTraitSlotUnlocked(1))
		{
			Helpers.GameObjectSetActive(supportTraitGo, value: true);
			supportTraitCardList.UpdateContent(supportModel, 0);
		}
	}

	public void OnClickTrait2Button()
	{
		if (CheckTraitSlotUnlocked(2))
		{
			Helpers.GameObjectSetActive(supportTraitGo, value: true);
			supportTraitCardList.UpdateContent(supportModel, 1);
		}
	}

	public void OnClickTrait3Button()
	{
		if (CheckTraitSlotUnlocked(3))
		{
			Helpers.GameObjectSetActive(supportTraitGo, value: true);
			supportTraitCardList.UpdateContent(supportModel, 2);
		}
	}

	private bool CheckTraitSlotUnlocked(int requiredSlot)
	{
		if (supportModel.SupportTalentSlot < requiredSlot)
		{
			int[] supportTalentSlots = supportModel.definition.GetSupportTalentSlots();
			for (int i = 0; i < supportTalentSlots.Length; i++)
			{
				if (supportTalentSlots[i] >= requiredSlot)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
					HUDNotification.Info(LocalizationManager.GetText("SupportTalentUI_Tips", i + 1));
					return false;
				}
			}
		}
		return true;
	}

	public void OnClickLevelUpButton()
	{
		Helpers.GameObjectSetActive(levelUpGo, value: true);
		Helpers.GameObjectSetActive(talentGo, value: false);
		UpdateButtonTexts();
		RefreshLevelUp();
	}

	public void OnClickTalentButton()
	{
		Helpers.GameObjectSetActive(levelUpGo, value: false);
		Helpers.GameObjectSetActive(talentGo, value: true);
		if (IsInPrePromoteState)
		{
			SetPromoteActive(active: false);
		}
		RefreshLevelUp();
		RefreshTalent();
	}

	public void OnClickSupportTraitCloseButton()
	{
		Helpers.GameObjectSetActive(supportTraitGo, value: false);
	}

	private KeyValuePair<CurrencyType, int>? GetCurrencyKeyValue()
	{
		foreach (KeyValuePair<CurrencyType, int> item in supportModel.definition.GetUpgradCostInfo(supportModel.Level))
		{
			if (item.Key != supportModel.Currency)
			{
				return item;
			}
		}
		return null;
	}

	private int GetSupportTokenValue()
	{
		foreach (KeyValuePair<CurrencyType, int> item in supportModel.definition.GetUpgradCostInfo(supportModel.Level))
		{
			if (item.Key == supportModel.Currency)
			{
				return item.Value;
			}
		}
		return 0;
	}
}
