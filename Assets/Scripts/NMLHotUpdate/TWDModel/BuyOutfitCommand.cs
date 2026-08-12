using BaseModel;

namespace TWDModel
{
	public class BuyOutfitCommand : ConsumeCurrencyCommand
	{
		public string OutfitID { get; private set; }

		public BuyOutfitCommand()
		{
		}

		public BuyOutfitCommand(string outfitID)
		{
			OutfitID = outfitID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			if (tWDModelManager.Player.gameEconomyData.ConfigData.BetaFlag_Outfits)
			{
				OutfitDefinition outfitDefinition = tWDModelManager.Player.gameEconomyData.GetOutfitDefinition(OutfitID);
				if (outfitDefinition != null && !tWDModelManager.Player.SurvivorContainer.HasOutfit(OutfitID))
				{
					Cashier cashier = new Cashier(tWDModelManager);
					CashierItem cashierItem = new CashierItem(PurchaseType.Outfit);
					cashierItem.SetCost(CurrencyType.Diamonds, outfitDefinition.Cost);
					cashier.AddItem(cashierItem);
					tWDModelResult = cashier.Pay(outfitDefinition);
					if (tWDModelResult == TWDModelResult.OK)
					{
						tWDModelManager.Player.SurvivorContainer.AddOutfit(OutfitID);
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
