using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class UpgradeBounsLevelCommand : ConsumeCurrencyCommand
	{
		public int ItemId { get; private set; }

		public UpgradeBounsLevelCommand()
		{
		}

		public UpgradeBounsLevelCommand(int itemId)
		{
			ItemId = itemId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			BounsModel bounsModel = tWDModelManager.Player.Equipment.GetBounsModelWithItemId(ItemId);
			TWDModelResult result = TWDModelResult.Error;
			BounsInfoDefinition bounsInfo = tWDModelManager.GameEconomyData.GetBounsInfo(ItemId);
			if (bounsInfo == null || !tWDModelManager.Player.SurvivorContainer.HasHero(bounsInfo.Owner))
			{
				return new NGModelCommandRespond(this, result);
			}
			BounsLevelDefinition bounsLevelDefinition = tWDModelManager.GameEconomyData.GetBounsLevelDefinition(ItemId, (bounsModel == null) ? 1 : (bounsModel.Level + 1));
			if (bounsLevelDefinition == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			Cashier cashier = new Cashier(tWDModelManager);
			foreach (KeyValuePair<CurrencyType, int> item in bounsLevelDefinition.GetCostInfo())
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeBouns);
				CurrencyType key = item.Key;
				int value = item.Value;
				cashierItem.SetCost(key, value);
				cashier.AddItem(cashierItem);
			}
			cashier.UseDiamondsAmount = base.UseDiamondsAmount;
			result = cashier.Pay(bounsModel);
			if (result == TWDModelResult.OK)
			{
				if (bounsModel == null)
				{
					bounsModel = new BounsModel(ItemId);
					bounsModel.SetManager(manager);
					bounsModel.Initialize();
					bounsModel.Start();
					tWDModelManager.Player.Equipment.AddBounsModel(bounsModel);
				}
				bounsModel.UpgradeLevel();
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
