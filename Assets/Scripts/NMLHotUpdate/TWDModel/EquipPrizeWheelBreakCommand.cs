using System.Collections.Generic;
using BaseModel;
using TWDModel.ResponsClass;

namespace TWDModel
{
	public class EquipPrizeWheelBreakCommand : ModelCommand
	{
		public List<int> modelIds { get; set; }

		public EquipPrizeWheelBreakCommand()
		{
			modelIds = new List<int>();
		}

		public EquipPrizeWheelBreakCommand(List<EquipmentItemModel> equipmentItems)
		{
			modelIds = new List<int>();
			for (int i = 0; i < equipmentItems.Count; i++)
			{
				modelIds.Add(equipmentItems[i].ModelId);
			}
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			List<EquipmentItemModel> list = new List<EquipmentItemModel>();
			for (int i = 0; i < modelIds.Count; i++)
			{
				EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(modelIds[i]);
				if (model.Owner == null && model.RarityLevel >= 4 && model.RarityLevel >= tWDModelManager.GameEconomyData.ConfigData.EquipmentDecompositionRarity)
				{
					list.Add(model);
				}
			}
			bool flag = true;
			for (int j = 0; j < list.Count; j++)
			{
				EquipmentItemModel equipmentItemModel = list[j];
				Cashier cashier = new Cashier(tWDModelManager);
				CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
				int cost = ((equipmentItemModel.RarityLevel > 4) ? tWDModelManager.GameEconomyData.ConfigData.ApocalypticWeaponsBreakDownFragmentsNumber : tWDModelManager.GameEconomyData.ConfigData.GoldWeaponsBreakDownFragmentsNumber);
				cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, cost);
				cashier.AddItem(cashierItem);
				ResponsScrapEquipmentItem responsScrapEquipmentItem = playerModel.Equipment.ScrapEquipmentItem(equipmentItemModel, deletedBySupport: false, cashier);
				flag = flag && responsScrapEquipmentItem.Result == TWDModelResult.OK;
			}
			TWDModelResult result = ((!flag) ? TWDModelResult.Error : TWDModelResult.OK);
			return new NGModelCommandRespond(this, result);
		}
	}
}
