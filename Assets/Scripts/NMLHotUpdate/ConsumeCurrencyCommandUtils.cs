using System.Collections.Generic;
using System.Linq;
using TWDModel;

public class ConsumeCurrencyCommandUtils
{
	public delegate void ConfirmationCallback(TWDModelResult result);

	public static Dictionary<PurchaseType, CurrencyType> purchaseToCurrencyTypeMap = new Dictionary<PurchaseType, CurrencyType>
	{
		{
			PurchaseType.SpeedUp,
			CurrencyType.BuildingTokenBP
		},
		{
			PurchaseType.SpeedUpSurvivorUpgrade,
			CurrencyType.TrainingTokenBP
		},
		{
			PurchaseType.SpeedUpEquipmentUpgrade,
			CurrencyType.EquipmentTokenBP
		},
		{
			PurchaseType.SpeedUpCuringSurvivor,
			CurrencyType.HealingTokenBP
		},
		{
			PurchaseType.InstantBuildingUpgrade,
			CurrencyType.SuperBuildingTokenBP
		},
		{
			PurchaseType.InstantSurvivorUpgrade,
			CurrencyType.SuperTrainingTokenBP
		},
		{
			PurchaseType.InstantEquipmentUpgrade,
			CurrencyType.SuperEquipmentTokenBP
		}
	};

	private static string FormatMissingCurrencies(Cashier cashier)
	{
		string text = "";
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			CurrencyType currencyType = (CurrencyType)i;
			if (cashier.GetMissing(currencyType) > 0)
			{
				text = text + cashier.GetMissing(currencyType) + " " + currencyType;
			}
		}
		return text + "?";
	}

	public static TWDModelResult ShowSpeedupPopup(ConsumeCurrencyCommand command, Cashier cashier, int askDiamondsNumber, int diamondsUsedForExchange, string title, string message, ConfirmationCallback callback = null)
	{
		SpeedupPopupTwo speedupPopupTwo = null;
		PurchaseType currentPurchaseType = PurchaseType.None;
		CurrencyType currentCurrencyType = CurrencyType.Diamonds;
		speedupPopupTwo = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SpeedupPopupTwo) as SpeedupPopupTwo;
		if (speedupPopupTwo != null)
		{
			speedupPopupTwo.consumeCurrencyCommand = command;
			message = LocalizationManager.GetText("SpeedupToken.Popup.Text");
			title = LocalizationManager.GetText("Popup.BuyResources." + title);
			CashierItem cashierItem = cashier.GetCashierItems().First((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUp || x.PurchaseType == PurchaseType.SpeedUpSurvivorUpgrade || x.PurchaseType == PurchaseType.SpeedUpEquipmentUpgrade || x.PurchaseType == PurchaseType.SpeedUpCuringSurvivor);
			currentPurchaseType = cashierItem.PurchaseType;
			if (!purchaseToCurrencyTypeMap.TryGetValue(currentPurchaseType, out currentCurrencyType))
			{
				return TWDModelResult.Error;
			}
			speedupPopupTwo.SetContent(title, message, askDiamondsNumber, currentCurrencyType);
			TWDModelResult result;
			speedupPopupTwo.SetSpeedupCallbacks(delegate
			{
				if (GameManager.Instance.playerModel.GetCurrency(currentCurrencyType).Value < 1)
				{
					result = TWDModelResult.NotEnoughCurrency;
				}
				else
				{
					SetSpeedUpCommand(currentPurchaseType, command, callback);
				}
				if (callback != null)
				{
					callback(TWDModelResult.Cancelled);
				}
			}, delegate
			{
				if (askDiamondsNumber > 0 && askDiamondsNumber > GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value)
				{
					result = TWDModelResult.NotEnoughCurrency;
				}
				else if (command != null)
				{
					command.UseDiamondsAmount = diamondsUsedForExchange;
					result = Helpers.ExecuteCommand(command);
				}
				else
				{
					result = TWDModelResult.OK;
				}
				if (result == TWDModelResult.OK)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
				}
				else if (result == TWDModelResult.NotEnoughCapacity)
				{
					HUDNotification.Error("Not Enough Capacity");
				}
				if (callback != null)
				{
					callback(result);
				}
			}, delegate
			{
				if (callback != null)
				{
					callback(TWDModelResult.Cancelled);
				}
			});
			speedupPopupTwo.Open();
			return TWDModelResult.OK;
		}
		return TWDModelResult.Error;
	}

	private static void SetSpeedUpCommand(PurchaseType currentPurchaseType, ConsumeCurrencyCommand command, ConfirmationCallback confirmationCallback)
	{
		TWDModelResult result = TWDModelResult.Error;
		switch (currentPurchaseType)
		{
		case PurchaseType.SpeedUp:
		{
			BuildingModel model4 = GameManager.Instance.modelManager.GetModel<BuildingModel>(command.ModelId);
			command.Cashier = model4.GetSpeedUpUpgradeCashierWithTokens();
			result = Helpers.ExecuteCommand(command);
			break;
		}
		case PurchaseType.SpeedUpSurvivorUpgrade:
		{
			SurvivorModel model3 = GameManager.Instance.modelManager.GetModel<SurvivorModel>(command.ModelId);
			command.Cashier = model3.TimedActionModel.GetSpeedUpCashierWithTokens(purchaseToCurrencyTypeMap[currentPurchaseType]);
			result = Helpers.ExecuteCommand(command);
			break;
		}
		case PurchaseType.SpeedUpEquipmentUpgrade:
		{
			EquipmentItemModel model2 = GameManager.Instance.modelManager.GetModel<EquipmentItemModel>(command.ModelId);
			command.Cashier = model2.TimedActionModel.GetSpeedUpCashierWithTokens(purchaseToCurrencyTypeMap[currentPurchaseType]);
			result = Helpers.ExecuteCommand(command);
			break;
		}
		case PurchaseType.SpeedUpCuringSurvivor:
		{
			SurvivorModel model = GameManager.Instance.modelManager.GetModel<SurvivorModel>(command.ModelId);
			command.Cashier = model.TimedActionModel.GetSpeedUpCashierWithTokens(purchaseToCurrencyTypeMap[currentPurchaseType]);
			result = Helpers.ExecuteCommand(command);
			break;
		}
		}
		confirmationCallback?.Invoke(result);
	}

	public static TWDModelResult ExecuteForSocialCommands(Cashier cashier, ConfirmationCallback callback = null)
	{
		return Execute(null, cashier, callback);
	}

	public static TWDModelResult Execute(ConsumeCurrencyCommand command, ConfirmationCallback callback = null)
	{
		return Execute(command, command.Cashier, callback);
	}

	public static TWDModelResult Execute(ConsumeCurrencyCommand command, Cashier cashier, ConfirmationCallback callback = null)
	{
		bool flag = cashier != null && !cashier.CanAfford();
		bool flag2 = cashier != null && cashier.useTokensForPayment;
		int askDiamondsNumber = 0;
		int diamondsUsedForExchange = 0;
		if (cashier != null)
		{
			askDiamondsNumber = cashier.GetTotalCost(CurrencyType.Diamonds);
		}
		if (askDiamondsNumber > 0 || flag || flag2)
		{
			string text = "";
			string text2;
			if (askDiamondsNumber > 0)
			{
				text2 = "SpendDiamonds";
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUp || x.PurchaseType == PurchaseType.SpeedupAndBuildingUpgrade))
				{
					text += "CompleteCurrentBuilding";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.ReclaimBadge))
				{
					text += "ReclaimBadge";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SevenDayLogin))
				{
					text += "RemedySevenDayReward";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpEquipmentUpgrade))
				{
					text += "UpgradeEquipment";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpEquipmentTypeUpgrade))
				{
					text += "CompleteEquipmentUpgrade";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpSurvivorUpgrade))
				{
					text += "CompleteSurvivorUpgrade";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpWalkerUpgrade))
				{
					text += "CompleteWalkerUpgrade";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpSearchSurvivor))
				{
					text += "CompleteSearchSurvivor";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpCuringSurvivor))
				{
					text += "CompleteCuringSurvivor";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpCuringAllSurvivors))
				{
					text += "CureAllSurvivors";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUpCreatingAllItems))
				{
					text += "CreateAllItems";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.AdditionalSurvivorSlots))
				{
					text += "BuyMoreSlots";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.BuildingUpgrade) || cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.InstantBuildingUpgrade))
				{
					text += "UpgradeBuilding";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.InstantSurvivorUpgrade))
				{
					text += "UpgradeSurvivor";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.InstantWalkerUpgrade))
				{
					text += "UpgradeWalker";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.InstantEquipmentUpgrade))
				{
					text += "UpgradeEquipment";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.PhoneCall))
				{
					text += "PhoneCall";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.UnlockWalker))
				{
					text += "UnlockWalker";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.UpgradeWalkerAmount))
				{
					text += "UpgradeWalkerAmount";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.GuildGift))
				{
					text += "GuildGift";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.TradeCrate))
				{
					text += "TradeCrate";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.OutpostBackground))
				{
					text += "OutpostBackground";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.GuildAd))
				{
					text += "CreateGuildAd";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.GoldShopDefinition))
				{
					text += "GoldShopDefinition";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SurvivalRest))
				{
					text += "SurvivalRest";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SurvivalRestart))
				{
					text += "SurvivalRestart";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SurvivalDoubleRewards))
				{
					text += "SurvivalDoubleRewards";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.GuildBattleAttackMission))
				{
					text += "GuildBattleAttackMission";
				}
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.RemedyActiveFoundation))
				{
					text += "RemedyActiveFoundationReward";
				}
				int num = 0;
				for (int num2 = 0; num2 < (int)CurrencyType.Count; num2++)
				{
					CurrencyType currencyType = (CurrencyType)num2;
					if (cashier.GetMissing(currencyType) > 0 && currencyType != CurrencyType.Diamonds)
					{
						if (!GameManager.Instance.gameEconomyData.CanConvertToDiamonds((CurrencyType)num2))
						{
							return TWDModelResult.NotEnoughCurrency;
						}
						num += GameManager.Instance.gameEconomyData.CurrencyToDiamonds((CurrencyType)num2, cashier.GetMissing((CurrencyType)num2), GameManager.Instance.playerModel);
					}
				}
				if (num > 0)
				{
					diamondsUsedForExchange = num;
					askDiamondsNumber += num;
					text += "AndBuyMissingResources";
				}
			}
			else
			{
				text2 = "NotEnoughCurrency";
				text = "BuyMissingResources";
				askDiamondsNumber = 0;
				int missing = cashier.GetMissing(CurrencyType.GvGGas);
				if (flag && missing > 0)
				{
					ShopPopupHelper.OpenForMissingCurrencyWithMissingAmount(missing, CurrencyType.GvGGas);
					return TWDModelResult.NotEnoughCurrency;
				}
				int missing2 = cashier.GetMissing(CurrencyType.GuildBattleRP);
				if (flag && missing2 > 0)
				{
					bool num3 = GameManager.Instance.gameEconomyData.FindNextGuildWarWithinSeason(GameManager.Instance.playerModel.UtcTimeStamp, GuildWarHelper.GetCurrentSeasonDefinitionId(), includeCurrentWar: true) != null;
					bool flag3 = GameManager.Instance.modelManager.Player.GvGSeasonModel?.FindNextSeason(GameManager.Instance.playerModel.UtcTimeStamp) != null;
					if (num3 || flag3)
					{
						SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NotEnoughRpPopup).Open();
					}
					else
					{
						AlertPopup.ShowPopup("", SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.NotEnoughRP"), LocalizationManager.GetText("Button.Ok"));
					}
					return TWDModelResult.NotEnoughCurrency;
				}
				for (int num4 = 0; num4 < (int)CurrencyType.Count; num4++)
				{
					int missing3 = cashier.GetMissing((CurrencyType)num4);
					if (missing3 > 0)
					{
						if (!GameManager.Instance.gameEconomyData.CanConvertToDiamonds((CurrencyType)num4))
						{
							return TWDModelResult.NotEnoughCurrency;
						}
						askDiamondsNumber += GameManager.Instance.gameEconomyData.CurrencyToDiamonds((CurrencyType)num4, missing3, GameManager.Instance.playerModel);
					}
				}
				diamondsUsedForExchange = askDiamondsNumber;
			}
			if (!flag && cashier.GetMissing(CurrencyType.Diamonds) > 0)
			{
				ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(askDiamondsNumber);
				return TWDModelResult.NotEnoughCurrency;
			}
			BuyResourcesPopup buyResourcesPopup = null;
			if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.RechargeCurrency))
			{
				buyResourcesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyEnergyPopup) as BuyResourcesPopup;
			}
			else
			{
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.SpeedUp || x.PurchaseType == PurchaseType.SpeedUpSurvivorUpgrade || x.PurchaseType == PurchaseType.SpeedUpEquipmentUpgrade || x.PurchaseType == PurchaseType.SpeedUpCuringSurvivor))
				{
					return ShowSpeedupPopup(command, cashier, askDiamondsNumber, diamondsUsedForExchange, text, text2, callback);
				}
				buyResourcesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup) as BuyResourcesPopup;
			}
			int depth = BuyResourcesPopup.DefaultDepth;
			if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.TradeCrate))
			{
				EquipmentUpgradePopup equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				if (equipmentUpgradePopup == null || !equipmentUpgradePopup.IsOpen)
				{
					depth = BuyResourcesPopup.TradeShopDepth;
				}
			}
			buyResourcesPopup.Open();
			if (text != "")
			{
				text = LocalizationManager.GetText("Popup.BuyResources." + text);
			}
			if (cashier.useTokensForPayment && cashier.CanAfford())
			{
				string text3 = LocalizationManager.GetText("SuperToken.DefaultPopup.InstantUpgrade");
				CashierItem cashierItem = cashier.GetCashierItems().First((CashierItem x) => x.PurchaseType == PurchaseType.InstantSurvivorUpgrade || x.PurchaseType == PurchaseType.InstantEquipmentUpgrade || x.PurchaseType == PurchaseType.InstantBuildingUpgrade);
				string text4 = "Popup.BuyResources.";
				if (cashierItem.PurchaseType == PurchaseType.InstantSurvivorUpgrade)
				{
					text4 += "UpgradeSurvivor";
				}
				else if (cashierItem.PurchaseType == PurchaseType.InstantEquipmentUpgrade)
				{
					text4 += "UpgradeEquipment";
				}
				else if (cashierItem.PurchaseType == PurchaseType.InstantBuildingUpgrade)
				{
					text4 += "UpgradeBuilding";
				}
				if (purchaseToCurrencyTypeMap.TryGetValue(cashierItem.PurchaseType, out var value))
				{
					buyResourcesPopup.SetConfirmContent(text3, LocalizationManager.GetText(text4), 1, value);
				}
			}
			else
			{
				buyResourcesPopup.SetContent(LocalizationManager.GetText("Popup.BuyResources." + text2), text, askDiamondsNumber);
				buyResourcesPopup.SetMissingCurrencies(cashier, showDiamonds: false);
			}
			buyResourcesPopup.SetCallbacks(delegate
			{
				if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.RechargeCurrency))
				{
					if (GameManager.Instance.playerModel.Blackboard.IsToggleOn("BuyJustEnoughGasForMission"))
					{
						askDiamondsNumber = BuyEnergyPopup.MissionCostGold;
					}
					else
					{
						askDiamondsNumber = diamondsUsedForExchange;
					}
				}
				TWDModelResult tWDModelResult;
				if (askDiamondsNumber > 0 && askDiamondsNumber > GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value)
				{
					tWDModelResult = TWDModelResult.NotEnoughCurrency;
				}
				else if (command != null)
				{
					command.UseDiamondsAmount = diamondsUsedForExchange;
					tWDModelResult = Helpers.ExecuteCommand(command);
				}
				else
				{
					tWDModelResult = TWDModelResult.OK;
				}
				switch (tWDModelResult)
				{
				case TWDModelResult.OK:
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
					break;
				case TWDModelResult.NotEnoughCapacity:
					HUDNotification.Error("Not Enough Capacity");
					break;
				case TWDModelResult.NotEnoughCurrency:
					ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(askDiamondsNumber);
					if (cashier.GetCashierItems().Exists((CashierItem x) => x.PurchaseType == PurchaseType.TradeCrate))
					{
						EquipmentUpgradePopup equipmentUpgradePopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
						if ((equipmentUpgradePopup2 == null || !equipmentUpgradePopup2.IsOpen) && buyResourcesPopup.GetComponent<UIPanel>() != null)
						{
							buyResourcesPopup.GetComponent<UIPanel>().depth = BuyResourcesPopup.DefaultDepth;
						}
					}
					break;
				}
				if (callback != null)
				{
					callback(tWDModelResult);
				}
			}, delegate
			{
				if (callback != null)
				{
					callback(TWDModelResult.Cancelled);
				}
			});
			buyResourcesPopup.Open();
			if (buyResourcesPopup.GetComponent<UIPanel>() != null)
			{
				buyResourcesPopup.GetComponent<UIPanel>().depth = depth;
			}
			return TWDModelResult.Pending;
		}
		TWDModelResult result;
		if (command != null)
		{
			command.UseDiamondsAmount = 0;
			result = Helpers.ExecuteCommand(command);
		}
		else
		{
			result = TWDModelResult.OK;
		}
		if (callback != null)
		{
			callback(result);
		}
		return result;
	}
}
