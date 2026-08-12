using BaseModel;

namespace TWDModel
{
	public class ConsumeDeepLinkCommand : ModelCommand
	{
		public string DeepLink;

		public ConsumeDeepLinkCommand(string deepLink)
		{
			DeepLink = deepLink;
		}

		public ConsumeDeepLinkCommand()
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				GameEconomyData gameEconomyData = tWDModelManager.GameEconomyData;
				PlayerModel player = tWDModelManager.Player;
				if (gameEconomyData.TryGetDeepLinkDefinition(DeepLink, out var deepLinkDefinition) && deepLinkDefinition.CheckValidity(player) == RedeemValidity.Valid)
				{
					deepLinkDefinition.Rewards?.Give(tWDModelManager);
					player.RedeemedDeeplinks.Add(deepLinkDefinition.Identifier);
					SendAnalyticEvent(deepLinkDefinition, tWDModelManager);
					result = TWDModelResult.OK;
				}
			}
			return new NGModelCommandRespond(this, result);
		}

		private void SendAnalyticEvent(DeepLinkDefinition deepLinkDefinition, TWDModelManager manager)
		{
			if (manager?.Player == null)
			{
				return;
			}
			Metrics metrics = manager.Metrics;
			metrics.Reset();
			foreach (IReward rewards in deepLinkDefinition.Rewards.RewardsList)
			{
				if (!(rewards is RewardCurrency rewardCurrency))
				{
					if (!(rewards is RewardEquipment rewardEquipment))
					{
						if (!(rewards is RewardMissingTokens rewardMissingTokens))
						{
							if (!(rewards is RewardOutfit rewardOutfit))
							{
								if (!(rewards is RewardRandomEquipment rewardRandomEquipment))
								{
									if (rewards is RewardTimedBonus rewardTimedBonus)
									{
										metrics.AddFind().AddTimedBonus(rewardTimedBonus);
									}
								}
								else
								{
									metrics.AddFind().AddEquipment(rewardRandomEquipment.GivenEquipment);
								}
							}
							else
							{
								metrics.AddFind().AddOutfit(manager.GameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]));
							}
						}
						else
						{
							int lastAmountMissingTokensGiven = manager.Player.BlackMarket.LastAmountMissingTokensGiven;
							metrics.AddFind().AddResources(rewardMissingTokens.RewardCurrencyType, lastAmountMissingTokensGiven, lastAmountMissingTokensGiven);
						}
					}
					else
					{
						metrics.AddFind().AddEquipment(rewardEquipment.GivenEquipment, "Equipment", rewardEquipment.Amount);
					}
				}
				else
				{
					metrics.AddFind().AddResources(rewardCurrency.CurrencyType, rewardCurrency.Amount, rewardCurrency.AmountActuallyAdded);
				}
			}
		}
	}
}
