using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class BuyConsumableCommand : ConsumeCurrencyCommand
	{
		[JsonProperty]
		private readonly string consumableId;

		public BuyConsumableCommand()
		{
		}

		public BuyConsumableCommand(string consumableId)
		{
			this.consumableId = consumableId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ConsumablesData consumablesData = tWDModelManager.GameEconomyData.ConsumablesData.ToList().FirstOrDefault((ConsumablesData x) => x.ConsumableId == consumableId);
			if (consumablesData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			CurrencyType currencyType = CurrencyType.Diamonds;
			int priceGold = consumablesData.PriceGold;
			Cashier cashier = Cashier.CreateOneItemCashier(tWDModelManager, PurchaseType.Consumable, currencyType, priceGold);
			if (!cashier.CanAfford() || !cashier.CanPay(currencyType))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EquipmentItemModel equipmentItemModel = tWDModelManager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(consumableId, 1, 1);
			if (equipmentItemModel == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (cashier.Pay() != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.Equipment.Consumables.Add(equipmentItemModel);
			foreach (CombatBackup combatBackup in tWDModelManager.Player.CombatBackups)
			{
				combatBackup.Consumables.Add(equipmentItemModel);
			}
			tWDModelManager.Metrics.AddSpend().AddResources(cashier).AddBuy()
				.AddEquipment(equipmentItemModel)
				.AddSurvivorsHealth()
				.AddMission()
				.AddMissionType();
			if (tWDModelManager.Metrics.GetMissionKind() == "gvg")
			{
				tWDModelManager.Metrics.AddGvGBattle();
			}
			tWDModelManager.Metrics.Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
