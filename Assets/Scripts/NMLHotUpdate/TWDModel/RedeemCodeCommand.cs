using BaseModel;

namespace TWDModel
{
	public class RedeemCodeCommand : ModelCommand
	{
		public string RedeemCode;

		public RedeemCodeCommand(string code)
		{
			RedeemCode = code;
		}

		public RedeemCodeCommand()
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				GameEconomyData gameEconomyData = tWDModelManager.GameEconomyData;
				PlayerModel player = tWDModelManager.Player;
				if (gameEconomyData.TryGetGiftCodeDefinition(RedeemCode, out var giftCodeDefinition) && giftCodeDefinition.CheckValidity(player) == RedeemValidity.Valid)
				{
					giftCodeDefinition.Rewards.Give(tWDModelManager);
					player.RedeemedCodes.Add(giftCodeDefinition.Identifier);
					SendAnalyticEvent(giftCodeDefinition, tWDModelManager);
					result = TWDModelResult.OK;
				}
			}
			return new NGModelCommandRespond(this, result);
		}

		private void SendAnalyticEvent(GiftCodeDefinition giftCodeDefinition, TWDModelManager manager)
		{
			if (manager?.Player == null)
			{
				return;
			}
			Metrics metrics = manager.Metrics;
			metrics.Reset();
			foreach (IReward rewards in giftCodeDefinition.Rewards.RewardsList)
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
									if (!(rewards is RewardTimedBonus rewardTimedBonus))
									{
										continue;
									}
									metrics.AddFind().AddTimedBonus(rewardTimedBonus);
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
				metrics.AddGiftCodeRedeem(giftCodeDefinition).Send();
			}
		}
	}
}
