using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class MakeModSkillCommand : ConsumeCurrencyCommand
	{
		public string ID { get; set; }

		public SurvivorClass SurvivorClass { get; set; }

		public string GroupID { get; set; }

		public MakeModSkillCommand(string id, string groupID)
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
			Dictionary<CurrencyType, int> makingCost = modSkillManager.GetMakingCost(ID);
			if (makingCost == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!modSkillManager.CanMakeModSkill(ID))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (modSkillManager.HasModSkillMode(GroupID))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.MakeModSkill);
			foreach (KeyValuePair<CurrencyType, int> item in makingCost)
			{
				cashierItem.SetCost(item.Key, item.Value);
			}
			cashier.AddItem(cashierItem);
			if (cashier.Pay(tWDModelManager) == TWDModelResult.OK && modSkillManager.MakeModSkill(ID, GroupID, SurvivorClass) == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
