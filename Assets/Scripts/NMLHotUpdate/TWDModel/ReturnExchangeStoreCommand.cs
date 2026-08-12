using BaseModel;

namespace TWDModel
{
	public class ReturnExchangeStoreCommand : ModelCommand
	{
		public int ExchangeId { get; set; }

		public ReturnExchangeStoreCommand(int exchangeId)
		{
			ExchangeId = exchangeId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnQuestAndExchange == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult result = tWDModelManager.Player.ReturnActivityManager.ReturnQuestAndExchange.TryExchange(ExchangeId);
			return new NGModelCommandRespond(this, result);
		}
	}
}
