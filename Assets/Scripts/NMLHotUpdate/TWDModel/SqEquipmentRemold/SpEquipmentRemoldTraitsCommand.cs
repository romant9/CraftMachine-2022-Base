using System.Collections.Generic;
using BaseModel;

namespace TWDModel.SqEquipmentRemold
{
	public class SpEquipmentRemoldTraitsCommand : ConsumeCurrencyCommand
	{
		public SpEquipmentRemoldTraitsCommand()
		{
		}

		public SpEquipmentRemoldTraitsCommand(int modelId)
			: base(modelId)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			int lockedNoForceTraitCount = model.SpEquipmentRemoldModel.GetLockedNoForceTraitCount();
			Dictionary<CurrencyType, int> dictionary = model.SpEquipmentRemoldModel.CalculateRemoldLockedCost(lockedNoForceTraitCount);
			Dictionary<CurrencyType, int> remoldBaseCost = model.SpEquipmentRemoldModel.GetRemoldBaseCost();
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SPEquipmentRemoldTraits);
			Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
			foreach (KeyValuePair<CurrencyType, int> item in dictionary)
			{
				if (!dictionary2.ContainsKey(item.Key))
				{
					dictionary2[item.Key] = 0;
				}
				dictionary2[item.Key] += item.Value;
			}
			foreach (KeyValuePair<CurrencyType, int> item2 in remoldBaseCost)
			{
				if (!dictionary2.ContainsKey(item2.Key))
				{
					dictionary2[item2.Key] = 0;
				}
				dictionary2[item2.Key] += item2.Value;
			}
			foreach (KeyValuePair<CurrencyType, int> item3 in dictionary2)
			{
				cashierItem.SetCost(item3.Key, item3.Value);
			}
			cashier.AddItem(cashierItem);
			if (cashier.Pay(model) == TWDModelResult.OK)
			{
				if (model.SpEquipmentRemoldModel.RemoldTraits(model.Definition))
				{
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			else
			{
				manager.Debug.LogError("SpEquipmentRemoldTraitsCommand Execute failed  EquipmentId : " + model.Definition.ID);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
