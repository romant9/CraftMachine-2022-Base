using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class UpgradeModSkillCommand : ConsumeCurrencyCommand
	{
		public string ID { get; set; }

		public string GroupID { get; set; }

		public UpgradeModSkillCommand(string id, string groupID)
		{
			ID = id;
			GroupID = groupID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ModSkillManager modSkillManager = tWDModelManager.Player.ModSkillManager;
			if (modSkillManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Dictionary<CurrencyType, int> upgradeModSkillCost = modSkillManager.GetUpgradeModSkillCost(ID);
			if (upgradeModSkillCost == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!modSkillManager.CanUpgradeModSkill(ID))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeModSkill);
			foreach (KeyValuePair<CurrencyType, int> item in upgradeModSkillCost)
			{
				cashierItem.SetCost(item.Key, item.Value);
			}
			cashier.AddItem(cashierItem);
			TWDModelResult tWDModelResult = cashier.Pay(tWDModelManager);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			TWDModelResult tWDModelResult2 = modSkillManager.UpgradeModSkill(ID, GroupID);
			if (tWDModelResult2 != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult2);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
