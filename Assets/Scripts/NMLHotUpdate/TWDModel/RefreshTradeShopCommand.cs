using BaseModel;

namespace TWDModel
{
	public class RefreshTradeShopCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.Player.GetTimeLeftToTradeShopRefresh() <= 0)
			{
				tWDModelManager.Player.RefreshTradeSlotsAndItems();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
