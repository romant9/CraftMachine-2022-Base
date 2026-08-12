using BaseModel;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using System;
using UnityEngine;
using TwdCustomMod;

public class BadgeDetailsPopup : HUDElement
{
	[Header("Effect")]
	[SerializeField]
	private UILabel[] effect;

	[SerializeField]
	private UISprite[] effectIcon;

	[SerializeField]
	private UILabel effectAmount;

	[Header("Rarity")]
	[SerializeField]
	private GameObject[] rarityStars;

	[SerializeField]
	private UILabel[] rarityLabel;

	[SerializeField]
	private UISprite rarityColor;

	[Header("Set")]
	[SerializeField]
	private UISprite[] setIcon;

	[SerializeField]
	private UILabel setLetter;

	[Header("Slot")]
	[SerializeField]
	private GameObject[] slots;

	[SerializeField]
	private GameObject mainSlot;

	[SerializeField]
	private float[] mainSlotRotations;

	[Header("Bonus")]
	[SerializeField]
	private GameObject bonusContainer;

	[SerializeField]
	private UILabel bonus;

	[SerializeField]
	private GameObject noBonus;

	[Header("Reroll area")]
	[SerializeField]
	private GameObject equippedOnSurvivor;

	[SerializeField]
	private UILabel survivorName;

	[SerializeField]
	private UILabel rerollSetCost;

	[SerializeField]
	private UILabel rerollSlotCost;

	[SerializeField]
	private UILabel rerollBonusCost;

	[SerializeField]
	private UIButton rerollSetButton;

	[SerializeField]
	private UIButton rerollSlotButton;

	[SerializeField]
	private UIButton rerollBonusButton;

	[Header("Scrap")]
	[SerializeField]
	private UIButton scrapButton;

	[Header("Unequip")]
	[SerializeField]
	private UIButton unequipButton;

	[Header("Trait reroll meter")]
	[SerializeField]
	private HUDMeter traitRerollMeter;

	private BadgeModel badgeModel;

	private string[] badgeSpriteNames = new string[5] { "Ui_Bagde_Bg_Common", "Ui_Bagde_Bg_Uncommon", "Ui_Bagde_Bg_Rare", "Ui_Bagde_Bg_Epic", "Ui_Bagde_Bg_Legendary" };

	[SerializeField]
	private Color enoughCurrency = Color.white;

	[SerializeField]
	private Color notEnoughCurrency = new Color(0.6313726f, 0.18431373f, 0.101960786f);

	private readonly Dictionary<BadgeType, string> setLetterCollection = new Dictionary<BadgeType, string>
	{
		{
			BadgeType.Rugged,
			"A"
		},
		{
			BadgeType.Shiny,
			"B"
		},
		{
			BadgeType.Bold,
			"C"
		},
		{
			BadgeType.Wellworn,
			"D"
		},
		{
			BadgeType.Jagged,
			"E"
		}
	};

	public override void OpenForModel(ModelObject model)
	{
		badgeModel = model as BadgeModel;
		if (FavoriteToggle) FavoriteToggle.value = badgeModel.IsFavorite;
		ColorEntry rarityColorData = GameManager.Instance.GetRarityColorData(badgeModel.Rarity);
		string badgeEffectTitle = HelpersLocalization.GetBadgeEffectTitle(badgeModel);
		UILabel[] array = effect;
		foreach (UILabel obj in array)
		{
			obj.text = badgeEffectTitle;
			obj.gradientTop = rarityColorData.GradientColorTop;
			obj.gradientBottom = rarityColorData.GradientColorBottom;
		}
		string badgeEffectSprite = HelpersGfx.GetBadgeEffectSprite(badgeModel.EffectId);
		UISprite[] array2 = effectIcon;
		for (int i = 0; i < array2.Length; i++)
		{
			HelpersUI.SetSprite(array2[i], badgeEffectSprite);
		}
		for (int j = 0; j < rarityStars.Length; j++)
		{
			HelpersUI.SetColor(rarityStars[j].GetComponent<UISprite>(), rarityColorData.GradientColorTop);
			rarityStars[j].SetActive(j <= badgeModel.Rarity);
		}
		array = rarityLabel;
		foreach (UILabel obj2 in array)
		{
			obj2.text = HelpersLocalization.GetRarityLevel(badgeModel.Rarity);
			HelpersUI.SetColor(obj2, rarityColorData.GradientColorTop);
		}
		HelpersUI.SetColor(rarityColor, rarityColorData.BackgroundColor);
		string badgeTypeSprite = HelpersGfx.GetBadgeTypeSprite(badgeModel.Type);
		array2 = setIcon;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].spriteName = badgeTypeSprite;
		}
		setLetter.text = setLetterCollection[badgeModel.Type];
		for (int k = 0; k < slots.Length; k++)
		{
			slots[k].SetActive(k == badgeModel.SlotIndex);
		}
		Vector3 eulerAngles = mainSlot.transform.eulerAngles;
		eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, mainSlotRotations[badgeModel.SlotIndex]);
		mainSlot.transform.eulerAngles = eulerAngles;
		mainSlot.GetComponent<UISprite>().spriteName = badgeSpriteNames[badgeModel.Rarity];
		if (badgeModel.BonusCondition is ConstantBonusCondition constantBonusCondition)
		{
			FixedPoint increment = badgeModel.Increment;
			increment += FixedPoint.Max(1L, FixedPoint.Round(increment * (constantBonusCondition.BonusValue / 100.0)));
			HelpersUI.SetContentToLabel(effectAmount, HelpersLocalization.GetBadgeEffectDescription(badgeModel, increment));
			HelpersUI.SetContentToLabel(bonus, "");
			noBonus.SetActive(value: true);
		}
		else
		{
			FixedPoint increment2 = badgeModel.Increment;
			HelpersUI.SetContentToLabel(effectAmount, HelpersLocalization.GetBadgeEffectDescription(badgeModel, increment2));
			HelpersUI.SetContentToLabel(bonus, HelpersLocalization.GetBadgeBonusDescription(badgeModel));
			noBonus.SetActive(value: false);
		}
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.TraitRerollToken);

		traitRerollMeter = CraftSettings.Instance.TraitRerollMeter;
        traitRerollMeter.gameObject.SetActive(true);
		traitRerollMeter.SetCurrencyType(CurrencyType.TraitRerollToken);
		traitRerollMeter.SetValue(currencyAmount);

		bool IsNoEquipped = true;
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			BadgeInfo survivorModel = DataManager.Instance.PlayerBadges.FirstOrDefault(badgeInfo => badgeInfo.Model.ModelId == badgeModel.ModelId);
			bool IsEquipped = survivorModel != null && !string.IsNullOrEmpty(survivorModel.OwnerName) && !IsUnEquip;
			if (IsEquipped)
			{
				equippedOnSurvivor.SetActive(value: true);
				survivorName.text = LocalizationManager.GetText("Popup.Badges.Details.EquippedOn{Survivor}", survivorModel.OwnerName);
				DebugTWD.Log("SurvivorModel is " + survivorModel.OwnerName, DebugType.Craft);
				IsNoEquipped = false;
			}
		}
		else
		{
			SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.Survivors.FirstOrDefault((SurvivorModel survivor) => survivor.BadgeContainer.Badges.Any((BadgeModel badge) => badge.ModelId == badgeModel.ModelId));
			if (survivorModel != null)
			{
				equippedOnSurvivor.SetActive(value: true);
				survivorName.text = LocalizationManager.GetText("Popup.Badges.Details.EquippedOn{Survivor}", survivorModel.SurvivorName);
				IsNoEquipped = false;
			}
		}

		if (IsNoEquipped)
		{
			DebugTWD.Log("SurvivorModel is null ", DebugType.Craft);
			equippedOnSurvivor.SetActive(value: false);
			LootManagerModel lootManager = GameManager.Instance.playerModel.LootManager;
			int badgeReRollCost = lootManager.GetBadgeReRollCost(badgeModel.ModelId, BadgeReroll.Slot);
			int badgeReRollCost2 = lootManager.GetBadgeReRollCost(badgeModel.ModelId, BadgeReroll.Set);
			int badgeReRollCost3 = lootManager.GetBadgeReRollCost(badgeModel.ModelId, BadgeReroll.Bonus);
			rerollSlotCost.text = badgeReRollCost.ToString();
			rerollSetCost.text = badgeReRollCost2.ToString();
			rerollBonusCost.text = badgeReRollCost3.ToString();
			rerollSlotCost.color = ((currencyAmount >= badgeReRollCost) ? enoughCurrency : notEnoughCurrency);
			HelpersUI.SetButtonState(rerollSlotButton, (currencyAmount < badgeReRollCost) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
			rerollSetCost.color = ((currencyAmount >= badgeReRollCost2) ? enoughCurrency : notEnoughCurrency);
			HelpersUI.SetButtonState(rerollSetButton, (currencyAmount < badgeReRollCost2) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
			rerollBonusCost.color = ((currencyAmount >= badgeReRollCost3) ? enoughCurrency : notEnoughCurrency);
			HelpersUI.SetButtonState(rerollBonusButton, (badgeModel.BonusCondition is ConstantBonusCondition || currencyAmount < badgeReRollCost3) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		}
		HelpersUI.SetButtonState(scrapButton, !IsNoEquipped ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		HelpersUI.SetButtonState(unequipButton, IsNoEquipped ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		base.OpenForModel(model);
	}

	public void OnScrapButtonClicked()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			return;
		}
		List<BadgeModel> badgeItems = new List<BadgeModel> { model as BadgeModel };
		Cashier badgeListScrapCashier = GameManager.Instance.playerModel.Equipment.GetBadgeListScrapCashier(badgeItems);
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		obj.SetContent(LocalizationManager.GetText("Popup.ScrapConfirmationList.Badges.Title"), LocalizationManager.GetText("Popup.ScrapConfirmationList.Badges.Message"));
		obj.SetCurrencies(badgeListScrapCashier);
		obj.SetCallbacks(OnScrapBadgesConfirmed, delegate
		{
		});
		obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
		obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
		obj.Open();
	}

	private void OnScrapBadgesConfirmed()
	{
		List<int> modelIds = new List<int> { model.ModelId };
		if (Helpers.ExecuteCommand(new ScrapBadgesCommand
		{
			modelIds = modelIds
		}) == TWDModelResult.OK)
		{
			UIEvent.Send("OnBadgeScraped");
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/workshop_equipment_scrap");
		Close();
	}

	public void OnUnequipBadgeButtonClicked()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			IsUnEquip = true;
			OnBadgeUnequiped(TWDModelResult.OK);
			return;
		}
		SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.Survivors.First((SurvivorModel survivor) => survivor.BadgeContainer.Badges.Any((BadgeModel badge) => badge.ModelId == model.ModelId));
		int num = -1;
		for (int num2 = 0; num2 < 6; num2++)
		{
			if (survivorModel.GetBadgeWithSlotIndex(num2)?.ModelId == model.ModelId)
			{
				num = num2;
				break;
			}
		}
		if (num != -1)
		{
			ConsumeCurrencyCommandUtils.Execute(new ReclaimBadgeCommand(survivorModel, num)
			{
				Cashier = survivorModel.GetBadgeReclaimCashier()
			}, OnBadgeUnequiped);
		}
	}

	private void OnBadgeUnequiped(TWDModelResult result)
	{
		UIEvent.Send("OnBadgeUnequipped");
		OpenForModel(model);
	}

	public void OnRerollSetButtonClicked()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("OnClick reroll Set", DebugType.Craft);
			RerollBadge(BadgeReroll.Set);
		}
		else
		{
			OpenConfirmationPopup(BadgeReroll.Set);
		}
	}

	public void OnRerollSlotButtonClicked()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("OnClick reroll Slot", DebugType.Craft);
			RerollBadge(BadgeReroll.Slot);
		}
		else
		{
			OpenConfirmationPopup(BadgeReroll.Slot);
		}
	}

	public void OnRerollBonusButtonClicked()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("OnClick reroll Bonus", DebugType.Craft);
			RerollBadge(BadgeReroll.Bonus);
		}
		else
		{
			OpenConfirmationPopup(BadgeReroll.Bonus);
		}
	}

	public void RerollBadge(BadgeReroll reroll)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			int badgeReRollCost = RerollCost(reroll);
			if (badgeReRollCost < 0)
			{
				DebugTWD.LogError("Error calculating badge reroll cost");
			}
			Cashier cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.BadgeReroll, CurrencyType.TraitRerollToken, badgeReRollCost);
			if (cashier.Pay() != 0)
			{
				DebugTWD.LogError("Cashier for badge reroll payment failed");
				return;
			}
			if (!cashier.CanAfford())
			{
				DebugTWD.LogError("Cant afford badge reroll");
				return;
			}
			BadgeModel badgeModel2 = RerollBadge(badgeModel, reroll);
			if (badgeModel2 == null)
			{
				DebugTWD.LogError("Error rerolling badge");
				return;
			}

			_BadgeRerollPopupList.gameObject.SetActive(true);
			_BadgeRerollPopupList.SetRerollType(reroll);
			_BadgeRerollPopupList.Open(badgeModel2, RerollCost(reroll));
			OpenForModel(badgeModel2);
			DebugTWD.Log("OnBadgeRerolled: " + reroll, DebugType.Craft);
		}
		else
		{
			if (Helpers.ExecuteCommand(new RerollBadgeCommand
			{
				RerollType = reroll,
				BadgeModelId = model.ModelId
			}) != TWDModelResult.OK)
			{
				Close();
				return;
			}
			GameManager.Instance.CheckConnectionReachability(showPopup: true, "RerollBadgeCommand");
			Close();
			BadgeRerollPopup badgeRerollPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeRerollPopup) as BadgeRerollPopup;
			badgeRerollPopup.SetRerollType(reroll);
			if (badgeRerollPopup != null)
			{
				badgeRerollPopup.OpenForModel(GameManager.Instance.playerModel.LastCraftedBadge);
			}
		}
		UIEvent.Send("OnBadgeRerolled");
	}

	private void OpenConfirmationPopup(BadgeReroll reroll)
	{
		int badgeReRollCost = GameManager.Instance.playerModel.LootManager.GetBadgeReRollCost(badgeModel.ModelId, reroll);
		CurrencyType currencyType = CurrencyType.TraitRerollToken;
		BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
		obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), LocalizationManager.GetText("Popup.Badges.Details.RerollTitle"), badgeReRollCost, currencyType);
		obj.SetCallbacks(delegate
		{
			RerollBadge(reroll);
		});
		obj.Open();
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public UIToggle FavoriteToggle;
	private ResidenceBadgeInventoryTab _ResidenceBadgeInventoryTab;
	private BadgeRerollPopupCustom _BadgeRerollPopupList;

	private int SlotRerollCount = 0;
	private int SetRerollCount = 0;
	private int BonusRerollCount = 0;

	private bool IsUnEquip;
	#endregion

	#region mycode
	public BadgeModel RerollBadge(BadgeModel badgeToReroll, BadgeReroll reroll)
	{
		bool IsUseRerollPool = BadgeCraft.Instance.IsUseRerollPool;
		GameEconomyData data = DataManager.Instance.GameData;

		int analyticsId = ++DataManager.Instance.Player.LootManager.CurrentBadgeAnalyticsId;

		ModelRandom dedicatedRandom = BadgeCraft.Instance.modelRandomLast;

		int num = badgeToReroll.SlotIndex;
		BadgeType badgeType = badgeToReroll.Type;
		int num2 = badgeToReroll.RerollsSlot;
		int num3 = badgeToReroll.RerollsSet;
		int num4 = badgeToReroll.RerollsBonus;
		switch (reroll)
		{
			case BadgeReroll.Slot:
				SlotRerollCount++;
				num2++;
				if (IsUseRerollPool)
				{
					badgeToReroll.AddSlotToHistory(num);
					while (badgeToReroll.HistorySlots.Contains(num))
					{
						num = dedicatedRandom.GetRandomInRange(0, 5);
					}
				}
				else
				{
					num = dedicatedRandom.GetRandomInRange(0, 5);
				}
				break;
			case BadgeReroll.Set:
				SetRerollCount++;
				num3++;
				if (IsUseRerollPool)
				{
					badgeToReroll.AddSetToHistory(badgeType);
					while (badgeToReroll.HistorySet.Contains(badgeType))
					{
						badgeType = (BadgeType)dedicatedRandom.GetRandomInRange(0, 4);
					}
				}
				else
				{
					badgeType = (BadgeType)dedicatedRandom.GetRandomInRange(0, 4);
				}
				break;
		}
		BadgeModel badgeModel = new BadgeModel(analyticsId, num, badgeToReroll.Rarity, badgeType, badgeToReroll.EffectId, badgeToReroll.EffectRoll, badgeToReroll.Level);
		if (reroll == BadgeReroll.Bonus)
		{
			if (badgeToReroll.BonusId == "Constant")
			{
				return null;
			}
			BonusRerollCount++;
			num4++;
			List<string> list = (from x in data.BadgeBonusDefinitions where x.ID != "Constant" select x.ID).ToList();
			if (IsUseRerollPool)
			{
				badgeToReroll.AddBonusToHistory();
			}
			string id = badgeModel.BonusId = dedicatedRandom.GetRandomElement(list, remove: false);

			BadgeBonusDefinition badgeBonusDefinition = data.GetBadgeBonusDefinition(id);
			CreateBonusCondition(badgeBonusDefinition, dedicatedRandom, ref badgeModel);
			if (IsUseRerollPool)
			{
				while (badgeToReroll.BonusHistoryContain(badgeModel))
				{
					id = badgeModel.BonusId = dedicatedRandom.GetRandomElement(list, remove: false);
					badgeBonusDefinition = data.GetBadgeBonusDefinition(id);
					CreateBonusCondition(badgeBonusDefinition, dedicatedRandom, ref badgeModel);
				}
			}
		}
		else
		{
			BadgeBonusDefinition badgeBonusDefinition2 = data.GetBadgeBonusDefinition(badgeToReroll.BonusId);
			badgeModel.BonusId = badgeToReroll.BonusId;
			if (badgeBonusDefinition2 != null)
			{
				CreateCopyOfBonusCondition(badgeBonusDefinition2, ref badgeModel, badgeToReroll);
			}
		}
		badgeModel.RerollsSlot = num2;
		badgeModel.RerollsSet = num3;
		badgeModel.RerollsBonus = num4;

		badgeModel.HistorySlots = badgeToReroll.HistorySlots;
		badgeModel.HistorySet = badgeToReroll.HistorySet;
		badgeModel.HistoryBonus = badgeToReroll.HistoryBonus;
		return badgeModel;
	}

	private void CreateBonusCondition(BadgeBonusDefinition bonusDef, ModelRandom random, ref BadgeModel badgeModel)
	{
		Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
		if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
		{
			DebugTWD.LogError("Failed to instantiate condition class " + bonusDef.ConditionClassName);
		}
		List<string> list = new List<string> { bonusDef.ConstructionParameters[0] };
		if (bonusDef.ConstructionParameters.Count > 1)
		{
			list.Add(random.GetRandomElement(bonusDef.ConstructionParameters.GetRange(1, bonusDef.ConstructionParameters.Count - 1), remove: false));
		}
		badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, list) as BaseBonusCondition) : null);
		badgeModel.BonusParameters = list;
	}

	private void CreateCopyOfBonusCondition(BadgeBonusDefinition bonusDef, ref BadgeModel badgeModel, BadgeModel oldBadgeModel)
	{
		Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
		if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
		{
			DebugTWD.LogError("Failed to instantiate condition class " + bonusDef.ConditionClassName);
		}
		badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, oldBadgeModel.BonusParameters) as BaseBonusCondition) : null);
		badgeModel.BonusParameters = oldBadgeModel.BonusParameters;
	}

	public void SetFavourite(UIToggle tg)
	{
		badgeModel.IsFavorite = tg.value;
		BadgeCraft.Instance.InvokeOnFavorite(badgeModel.ModelId);
	}

	public override void OnClickClose()
	{
		//_ResidenceBadgeInventoryTab.traitRerollMeter.hasBeenInitialised = false;
		_ResidenceBadgeInventoryTab.SetCurrency();
		_BadgeRerollPopupList.gameObject.SetActive(false);

		if (BadgeCraft.Instance.modelRandomReroll != null)
		{
			BadgeCraft.Instance.modelRandomLast = new ModelRandom(BadgeCraft.Instance.modelRandomReroll);
			DebugTWD.Log("Change Random to Craft " + BadgeCraft.Instance.modelRandomLast.State);
			BadgeCraft.Instance.GetOriginBadgeData();
		}

		if (traitRerollMeter != null) traitRerollMeter.gameObject.SetActive(false);
        base.OnClickClose();
	}

	public void SetData(ResidenceBadgeInventoryTab root)
	{
		_ResidenceBadgeInventoryTab = root;
		_BadgeRerollPopupList = root.BadgeRerollPopupList;
	}

	private int RerollCost(BadgeReroll reroll)
	{
		int count = -1;
		switch (reroll)
		{
			case BadgeReroll.Slot:
				count = SlotRerollCount;
				break;
			case BadgeReroll.Set:
				count = SetRerollCount;
				break;
			case BadgeReroll.Bonus:
				count = BonusRerollCount;
				break;
		}

		if (count > -1)
		{
			if (count < 2)
				return 15;
			else if (count == 2)
				return 20;
			else return 25;
		}
		else return 0;
	}
	#endregion
}
