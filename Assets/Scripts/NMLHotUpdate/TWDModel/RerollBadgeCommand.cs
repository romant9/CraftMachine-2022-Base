using BaseModel;

namespace TWDModel
{
	public class RerollBadgeCommand : ModelCommand
	{
		public int BadgeModelId;

		public BadgeReroll RerollType;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
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
			int badgeReRollCost = player.LootManager.GetBadgeReRollCost(BadgeModelId, RerollType);
			if (badgeReRollCost < 0)
			{
				tWDModelManager.Debug.LogError("Error calculating badge reroll cost");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Cashier cashier = Cashier.CreateOneItemCashier(tWDModelManager, PurchaseType.BadgeReroll, CurrencyType.TraitRerollToken, badgeReRollCost);
			if (!cashier.CanAfford())
			{
				tWDModelManager.Debug.LogError("Cant afford badge reroll");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			BadgeModel badgeModel = player.Equipment.Badges.Get(BadgeModelId);
			if (badgeModel == null)
			{
				tWDModelManager.Debug.LogError("No badge found with model id " + BadgeModelId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			BadgeModel badgeModel2 = player.LootManager.RerollBadge(badgeModel, RerollType);
			if (badgeModel2 == null)
			{
				tWDModelManager.Debug.LogError("Error rerolling badge");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (cashier.Pay() != TWDModelResult.OK)
			{
				tWDModelManager.Debug.LogError("Cashier for badge reroll payment failed");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			badgeModel2.Initialize();
			badgeModel2.SetManager(manager);
			badgeModel2.Start();
			player.Equipment.RemoveBadge(badgeModel);
			player.Equipment.AddBadge(badgeModel2);
			player.LastCraftedBadge = badgeModel2;
			player.NotifyChange(LootManagerModel.BadgeCreatedEvent);
			string text = string.Empty;
			switch (RerollType)
			{
			case BadgeReroll.Slot:
				text = badgeModel.SlotIndex.ToString();
				break;
			case BadgeReroll.Set:
				text = badgeModel.Type.ToString();
				break;
			case BadgeReroll.Bonus:
				text = badgeModel.BonusId;
				text += $"-{badgeModel.BonusCondition}";
				foreach (string bonusParameter in badgeModel.BonusParameters)
				{
					text = text + "_" + bonusParameter;
				}
				break;
			}
			tWDModelManager.Metrics.AddFind().AddBadge(badgeModel2).AddCrafting(CraftingType.Badge, player.LootManager.CurrentBadgeAnalyticsId.ToString())
				.AddBadgeReroll(RerollType, badgeReRollCost, text)
				.Send();
			tWDModelManager.Metrics.AddSpend().AddResources(cashier).AddBadge(badgeModel2)
				.AddBadgeReroll(RerollType, badgeReRollCost, text)
				.Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
