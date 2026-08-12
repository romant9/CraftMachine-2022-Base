using BaseModel;

namespace TWDModel
{
	public class BuyGuildShopItemCommand : ConsumeCurrencyCommand
	{
		public int GuildShopItemId { get; private set; }

		public BuyGuildShopItemCommand()
		{
		}

		public BuyGuildShopItemCommand(int guildShopItemId)
		{
			GuildShopItemId = guildShopItemId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			GuildShopItemInfo value = null;
			if (!tWDModelManager.Player.GuildShopModel.GuildShopAvailableItems.TryGetValue(GuildShopItemId, out value))
			{
				manager.Debug.LogError("BuyGuildShopItemCommand Failed. item definition not found with ID: " + GuildShopItemId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GuildShopDefinition itemDefinition = value.ItemDefinition;
			if (itemDefinition != null)
			{
				if (itemDefinition.LimitedPurchases && value.AvailableAmount <= 0)
				{
					manager.Debug.LogError("BuyGuildShopItemCommand Failed. item has no available stock: " + GuildShopItemId);
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Cashier cashier = new Cashier(tWDModelManager);
				CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrate);
				CurrencyType priceCurrency = itemDefinition.PriceCurrency;
				int priceAmount = itemDefinition.PriceAmount;
				cashierItem.SetCost(priceCurrency, priceAmount);
				cashier.AddItem(cashierItem);
				cashier.UseDiamondsAmount = base.UseDiamondsAmount;
				tWDModelResult = cashier.Pay(value);
				if (tWDModelResult == TWDModelResult.OK)
				{
					if (value.ItemDefinition.LimitedPurchases)
					{
						value.AvailableAmount--;
					}
					if (value.ItemDefinition.ContentRewards.RewardsList[0] is RewardTradeCrate)
					{
						if ((value.ItemDefinition.ContentRewards.RewardsList[0] as RewardTradeCrate).Give(tWDModelManager) == null)
						{
							tWDModelResult = TWDModelResult.Error;
						}
					}
					else if (value.ItemDefinition.ContentRewards.RewardsList[0] is RewardCurrency)
					{
						RewardCurrency rewardCurrency = value.ItemDefinition.ContentRewards.RewardsList[0] as RewardCurrency;
						LootEntry lootEntry = tWDModelManager.Player.LootManager.CreateCurrencyLoot(rewardCurrency.CurrencyType, rewardCurrency.Amount, DropType.None, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency);
						if (lootEntry != null)
						{
							tWDModelManager.Player.LootManager.GiveLoot(lootEntry);
							tWDModelManager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, lootEntry.RewardedAmount, lootEntry.ActualAmountAdded).AddGvGCrate(value)
								.AddGvG()
								.Send();
						}
						else
						{
							tWDModelResult = TWDModelResult.Error;
						}
					}
					else if (value.ItemDefinition.ContentRewards.RewardsList[0] is RewardEquipment)
					{
						RewardEquipment rewardEquipment = value.ItemDefinition.ContentRewards.RewardsList[0] as RewardEquipment;
						rewardEquipment.EquipmentSource = EquipmentSource.GuildShop;
						ModelRandom modelRandom = new ModelRandom(tWDModelManager.Player.GuildShopModel.RandomSeed + value.ItemDefinition.ID);
						if (rewardEquipment.Give(tWDModelManager, new object[1] { modelRandom }) is EquipmentItemModel equipmentItemModel)
						{
							tWDModelManager.Player.LootManager.LastTradedEquipment = equipmentItemModel;
							tWDModelManager.Metrics.AddFind().AddEquipment(equipmentItemModel, "Equipment", rewardEquipment?.Amount ?? 1).AddGvGCrate(value)
								.AddGvG()
								.Send();
						}
						else
						{
							tWDModelResult = TWDModelResult.Error;
						}
					}
					else if (value.ItemDefinition.ContentRewards.RewardsList[0] is RewardRandomEquipment)
					{
						RewardRandomEquipment obj = value.ItemDefinition.ContentRewards.RewardsList[0] as RewardRandomEquipment;
						obj.EquipmentSource = EquipmentSource.GuildShop;
						ModelRandom modelRandom2 = new ModelRandom(tWDModelManager.Player.GuildShopModel.RandomSeed + value.ItemDefinition.ID);
						if (obj.Give(tWDModelManager, new object[1] { modelRandom2 }) is EquipmentItemModel equipmentItemModel2)
						{
							tWDModelManager.Player.LootManager.LastTradedEquipment = equipmentItemModel2;
							tWDModelManager.Metrics.AddFind().AddEquipment(equipmentItemModel2).AddGvGCrate(value)
								.AddGvG()
								.Send();
						}
						else
						{
							tWDModelResult = TWDModelResult.Error;
						}
					}
					else if (value.ItemDefinition.ContentRewards.RewardsList[0] is RewardOutfit)
					{
						if (string.IsNullOrEmpty((value.ItemDefinition.ContentRewards.RewardsList[0] as RewardOutfit).Give(tWDModelManager) as string))
						{
							tWDModelResult = TWDModelResult.Error;
						}
					}
					else if (value.ItemDefinition.ContentRewards.RewardsList[0] is RewardTimedBonus)
					{
						RewardTimedBonus rewardTimedBonus = value.ItemDefinition.ContentRewards.RewardsList[0] as RewardTimedBonus;
						if (rewardTimedBonus.Give(tWDModelManager) != null)
						{
							tWDModelManager.Metrics.AddFind().AddTimedBonus(rewardTimedBonus).AddGvGCrate(value)
								.AddGvG()
								.Send();
						}
					}
				}
				if (tWDModelResult != TWDModelResult.OK && value.ItemDefinition.LimitedPurchases)
				{
					value.AvailableAmount++;
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
