using BaseModel;

namespace TWDModel
{
	public class RefreshReturnExchangeStoreCommand : ModelCommand
	{
		public int ExchangeId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnQuestAndExchange == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = tWDModelManager.Player.ReturnActivityManager.ReturnQuestAndExchange.TryRefreshExchangeStore(ExchangeId);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
