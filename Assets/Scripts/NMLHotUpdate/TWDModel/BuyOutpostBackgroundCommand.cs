using BaseModel;

namespace TWDModel
{
	public class BuyOutpostBackgroundCommand : ConsumeCurrencyCommand
	{
		public string OutpostTemplateId { get; set; }

		public BuyOutpostBackgroundCommand()
		{
		}

		public BuyOutpostBackgroundCommand(string outpostTemplateId)
		{
			OutpostTemplateId = outpostTemplateId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			OutpostTemplateDefinition outpostTemplateDefinition = tWDModelManager.GameEconomyData.GetOutpostTemplateDefinition(OutpostTemplateId);
			if (outpostTemplateDefinition != null && !tWDModelManager.Player.OutpostModel.IsBackgroundUnlocked(OutpostTemplateId))
			{
				Cashier cashier = new Cashier(tWDModelManager);
				CashierItem cashierItem = new CashierItem(PurchaseType.OutpostBackground);
				cashierItem.SetCost(outpostTemplateDefinition.GetCostCurrencyType(), outpostTemplateDefinition.GetCostAmount());
				cashier.AddItem(cashierItem);
				tWDModelResult = cashier.Pay();
				if (tWDModelResult == TWDModelResult.OK)
				{
					tWDModelResult = ((!tWDModelManager.Player.OutpostModel.AddPurchasedBackground(OutpostTemplateId)) ? TWDModelResult.Error : TWDModelResult.OK);
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
