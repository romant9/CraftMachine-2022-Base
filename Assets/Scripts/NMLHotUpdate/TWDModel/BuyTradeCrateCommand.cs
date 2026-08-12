using BaseModel;

namespace TWDModel
{
	public class BuyTradeCrateCommand : ConsumeCurrencyCommand
	{
		public int TradeSlotId { get; private set; }

		public BuyTradeCrateCommand()
		{
		}

		public BuyTradeCrateCommand(int tradeSlotId)
		{
			TradeSlotId = tradeSlotId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			if (tWDModelManager.Player.gameEconomyData.ConfigData.TradeCratesEnabled)
			{
				TradeSlotInfo currentTradeSlotDefinitionById = tWDModelManager.Player.GetCurrentTradeSlotDefinitionById(TradeSlotId);
				if (currentTradeSlotDefinitionById != null && !currentTradeSlotDefinitionById.Bought)
				{
					Cashier cashier = new Cashier(tWDModelManager);
					CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrate);
					cashierItem.SetCost(cost: currentTradeSlotDefinitionById.GetPurchasePrice(out var currencyType), currencyType: currencyType);
					cashier.AddItem(cashierItem);
					cashier.UsedReason = "TradeCrate";
					cashier.UseDiamondsAmount = base.UseDiamondsAmount;
					tWDModelResult = cashier.Pay(currentTradeSlotDefinitionById);
					if (tWDModelResult == TWDModelResult.OK)
					{
						if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardTradeCrate)
						{
							if ((currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardTradeCrate).Give(tWDModelManager) != null)
							{
								currentTradeSlotDefinitionById.PurchaseCount++;
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
						else if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardCurrency)
						{
							RewardCurrency rewardCurrency = currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardCurrency;
							LootEntry lootEntry = tWDModelManager.Player.LootManager.CreateCurrencyLoot(rewardCurrency.CurrencyType, rewardCurrency.Amount, DropType.None, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency);
							if (lootEntry != null)
							{
								tWDModelManager.Player.LootManager.GiveLoot(lootEntry);
								currentTradeSlotDefinitionById.PurchaseCount++;
								tWDModelManager.Metrics.ResourceChangeIsByCharging = "1";
								tWDModelManager.Metrics.ResourceChangeUsedReason = "BuyTradeCrate";
								tWDModelManager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, lootEntry.RewardedAmount, lootEntry.ActualAmountAdded).AddTradeCrate(currentTradeSlotDefinitionById)
									.Send();
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
						else if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipment)
						{
							RewardEquipment rewardEquipment = currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardEquipment;
							rewardEquipment.EquipmentSource = EquipmentSource.TradeGoodsShop;
							ModelRandom modelRandom = new ModelRandom((int)tWDModelManager.Player.LastTradeShopRefreshTime + currentTradeSlotDefinitionById.CurrentTradeDefinition.UniqueId);
							if (rewardEquipment.Give(tWDModelManager, new object[1] { modelRandom }) is EquipmentItemModel equipmentItemModel)
							{
								tWDModelManager.Player.LootManager.LastTradedEquipment = equipmentItemModel;
								currentTradeSlotDefinitionById.PurchaseCount++;
								tWDModelManager.Metrics.AddFind().AddEquipment(equipmentItemModel, "Equipment", rewardEquipment?.Amount ?? 1).AddTradeCrate(currentTradeSlotDefinitionById)
									.Send();
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
						else if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardRandomEquipment)
						{
							RewardRandomEquipment obj = currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardRandomEquipment;
							obj.EquipmentSource = EquipmentSource.TradeGoodsShop;
							ModelRandom modelRandom2 = new ModelRandom((int)tWDModelManager.Player.LastTradeShopRefreshTime + currentTradeSlotDefinitionById.CurrentTradeDefinition.UniqueId);
							if (obj.Give(tWDModelManager, new object[1] { modelRandom2 }) is EquipmentItemModel equipmentItemModel2)
							{
								tWDModelManager.Player.LootManager.LastTradedEquipment = equipmentItemModel2;
								tWDModelManager.Metrics.AddFind().AddEquipment(equipmentItemModel2).AddTradeCrate(currentTradeSlotDefinitionById)
									.Send();
								currentTradeSlotDefinitionById.PurchaseCount++;
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
						else if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardOutfit)
						{
							if (!string.IsNullOrEmpty((currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardOutfit).Give(tWDModelManager) as string))
							{
								currentTradeSlotDefinitionById.PurchaseCount++;
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
						else if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardTimedBonus)
						{
							RewardTimedBonus rewardTimedBonus = currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardTimedBonus;
							if (rewardTimedBonus.Give(tWDModelManager) != null)
							{
								currentTradeSlotDefinitionById.PurchaseCount++;
								tWDModelManager.Metrics.AddFind().AddTimedBonus(rewardTimedBonus).AddTradeCrate(currentTradeSlotDefinitionById)
									.Send();
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
						else if (currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] is RewardEquipToken)
						{
							RewardEquipToken rewardEquipToken = currentTradeSlotDefinitionById.CurrentTradeDefinition.SoldItems.RewardsList[0] as RewardEquipToken;
							if (rewardEquipToken.Give(tWDModelManager) != null)
							{
								currentTradeSlotDefinitionById.PurchaseCount++;
								tWDModelManager.Metrics.AddFind().AddEquipToken(rewardEquipToken).AddTradeCrate(currentTradeSlotDefinitionById)
									.Send();
							}
							else
							{
								tWDModelResult = TWDModelResult.Error;
							}
						}
					}
					if (tWDModelResult == TWDModelResult.OK)
					{
						if (currentTradeSlotDefinitionById.CurrentTradeDefinition.HasDateLimit)
						{
							tWDModelManager.Player.AddBoughtTimeLimitedTradeOffer(currentTradeSlotDefinitionById.CurrentTradeDefinition.UniqueId);
						}
						tWDModelManager.Player.NotifyChange("TradeShopItemBought");
						tWDModelManager.Player.DailyQuestManager.StartAction("Purchase").ShopType = "TradeGoods";
						tWDModelManager.Player.DailyQuestManager.CommitAction();
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
