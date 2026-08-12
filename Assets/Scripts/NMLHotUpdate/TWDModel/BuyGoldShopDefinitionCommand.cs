using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class BuyGoldShopDefinitionCommand : ConsumeCurrencyCommand
	{
		public string ItemId;

		public BuyGoldShopDefinitionCommand()
		{
		}

		public BuyGoldShopDefinitionCommand(string itemId)
		{
			ItemId = itemId;
		}

		public static Cashier GetCashierForItem(GoldShopDefinition definition, TWDModelManager manager)
		{
			CashierItem cashierItem = new CashierItem(PurchaseType.GoldShopDefinition);
			cashierItem.SetCost(CurrencyType.Diamonds, definition.Price);
			Cashier cashier = new Cashier(manager);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			bool isActivityOpen = tWDModelManager.Player.ActivityManager.IsActivityOpen(ActivityType.FreeBadgeUnequip);
			GoldShopDefinition goldShopDefinition = tWDModelManager.GameEconomyData.GetGoldShopDefinition(ItemId, isActivityOpen);
			int buildingLevel = tWDModelManager.Player.Camp.GetBuildingLevel("Scavenger");
			if (goldShopDefinition != null && buildingLevel > 0)
			{
				if (goldShopDefinition.IsNewVersion && !tWDModelManager.Player.GoldShopDefinitionManager.CanBuyBundle(goldShopDefinition))
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Cashier cashierForItem = GetCashierForItem(goldShopDefinition, tWDModelManager);
				cashierForItem.UsedReason = "GoldShopDefinition";
				tWDModelResult = cashierForItem.Pay(goldShopDefinition);
				if (tWDModelResult == TWDModelResult.OK)
				{
					if (goldShopDefinition.GuaranteedComponents != null && goldShopDefinition.GuaranteedComponents.Count > 0)
					{
						tWDModelManager.Player.LootManager.GiveGoldShopDefinition(goldShopDefinition);
						List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
						for (int i = 0; i < goldShopDefinition.SubItems.Count; i++)
						{
							ComponentCrateItem componentCrateItem = goldShopDefinition.SubItems[i];
							list.Add(new Dictionary<string, object>
							{
								{ "resource_name", componentCrateItem.Type },
								{ "resource_num", componentCrateItem.Count }
							});
						}
						tWDModelManager.TdMetrics.SetEventType("goldshop_redeem").AddProperty("resource_id", CurrencyType.Diamonds.ToString()).AddProperty("currency_used_num", goldShopDefinition.Price)
							.AddProperty("bundle_id", goldShopDefinition.ItemId)
							.AddProperty("product_detail", list)
							.Send();
					}
					else
					{
						tWDModelManager.Player.GoldShopDefinitionManager.BuyBundle(goldShopDefinition);
						tWDModelManager.TdMetrics.SetEventType("goldshop_redeem").AddProperty("resource_id", CurrencyType.Diamonds.ToString()).AddProperty("currency_used_num", goldShopDefinition.Price)
							.AddProperty("bundle_id", goldShopDefinition.ItemId)
							.AddProperty("product_detail", goldShopDefinition.RewardEntries.RewardResources)
							.Send();
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
