using BaseModel;

namespace TWDModel
{
	public class SetWeeklyChallengeClassTeamExchangeReminderCommand : ModelCommand
	{
		public int ExchangeId;

		public bool Enabled;

		public SetWeeklyChallengeClassTeamExchangeReminderCommand()
		{
		}

		public SetWeeklyChallengeClassTeamExchangeReminderCommand(int exchangeId, bool enabled)
		{
			ExchangeId = exchangeId;
			Enabled = enabled;
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
			if (!shop.SetExchangeReminderEnabled(ExchangeId, Enabled))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
