using BaseModel;

namespace TWDModel
{
	public class RerollPhoneCallCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelManager twdModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.Error;
			if (playerModel != null)
			{
				if (playerModel.PhoneCall.NumRerolls > 0)
				{
					SendRerollAnalyticsEvents(twdModelManager, playerModel);
					result = playerModel.PhoneCall.RerollCall();
				}
				else
				{
					result = TWDModelResult.Error;
				}
			}
			return new NGModelCommandRespond(this, result);
		}

		private void SendRerollAnalyticsEvents(TWDModelManager twdModelManager, PlayerModel playerModel)
		{
			for (int i = 0; i < playerModel.PhoneCall.LootsList.Count; i++)
			{
				if (playerModel.PhoneCall.IsLootLockedForReroll(i))
				{
					continue;
				}
				LootEntry lootEntry = playerModel.PhoneCall.LootsList[i];
				switch (lootEntry.DropCurrencyType)
				{
				case DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor:
				{
					SurvivorModel generatedSurvivor = lootEntry.GeneratedSurvivor;
					if (generatedSurvivor != null)
					{
						twdModelManager.Metrics.AddIgnore().AddSurvivor(generatedSurvivor).AddFromSurvivorSource(NewSurvivorSource.Phone)
							.Send();
					}
					break;
				}
				case DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken:
					twdModelManager.Metrics.AddIgnore().AddHeroToken(lootEntry.RewardedCurrency, lootEntry.RewardedAmount).AddFromSurvivorSource(NewSurvivorSource.Phone)
						.Send();
					break;
				}
			}
		}
	}
}
