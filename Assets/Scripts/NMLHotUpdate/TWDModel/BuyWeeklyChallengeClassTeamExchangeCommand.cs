using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class BuyWeeklyChallengeClassTeamExchangeCommand : ModelCommand
	{
		[JsonIgnore]
		public Rewards Rewards;

		public int ExchangeId;

		public BuyWeeklyChallengeClassTeamExchangeCommand()
		{
		}

		public BuyWeeklyChallengeClassTeamExchangeCommand(int exchangeId)
		{
			ExchangeId = exchangeId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivity = tWDModelManager.Player.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivity == null || !weeklyChallengeClassTeamActivity.IsActive)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			WeeklyChallengeClassTeamShopModel shop = weeklyChallengeClassTeamActivity.Shop;
			if (shop == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ClassTeamExchangeDefinition exchangeDefinition = shop.GetExchangeDefinition(ExchangeId);
			if (exchangeDefinition == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult tWDModelResult = shop.Exchange(ExchangeId);
			if (tWDModelResult == TWDModelResult.OK)
			{
				Rewards = exchangeDefinition.ContentRewards;
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
