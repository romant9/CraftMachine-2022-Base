using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class CraftBadgeCommand : ConsumeCurrencyCommand
	{
		public List<CurrencyType> Currencies;

		public CraftBadgeCommand()
		{
			Currencies = new List<CurrencyType>();
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.Error;
			if (!(tWDModelManager.Player.Camp.GetBuilding("Residence") is ResidenceBuildingModel residenceBuildingModel))
			{
				tWDModelManager.Debug.LogError("Residence building not found in camp");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (residenceBuildingModel.IsUpgrading)
			{
				tWDModelManager.Debug.LogError("Residence building is upgrading");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier badgeCraftCashier = tWDModelManager.Player.LootManager.GetBadgeCraftCashier(Currencies);
			if (badgeCraftCashier == null || !LootManagerModel.IsFirstBadgeSlotBadgeComponent(Currencies))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			int num = tWDModelManager.Player.Equipment.Badges.Count + tWDModelManager.Player.SurvivorContainer.Survivors.Sum((SurvivorModel x) => x.BadgeContainer.Badges.Count);
			int maximumBadgeCount = tWDModelManager.Player.SurvivorContainer.MaximumBadgeCount;
			if (num >= maximumBadgeCount)
			{
				tWDModelManager.Debug.LogError("Max badge inventory size reached");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			string text = Guid.NewGuid().ToString();
			TWDModelResult tWDModelResult = badgeCraftCashier.Pay(null, text);
			if (tWDModelResult == TWDModelResult.OK)
			{
				if (tWDModelManager.Player.LootManager.CraftBadge(Currencies, text))
				{
					result = TWDModelResult.OK;
				}
			}
			else
			{
				tWDModelManager.Debug.LogError("Cashier payment failed");
				result = tWDModelResult;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
