using TWDModel;

public class RewardMissingTokens : IReward
{
	public int MaxTokensGiven { get; set; }

	public CurrencyType RewardCurrencyType { get; set; }

	public RewardType Type => RewardType.MissingTokens;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		int tokenAmount = GetTokenAmount(manager);
		if (tokenAmount == -1)
		{
			return null;
		}
		manager.Player.GetCurrency(RewardCurrencyType).Add(tokenAmount);
		manager.Player.BlackMarket.LastAmountMissingTokensGiven = tokenAmount;
		return RewardCurrencyType;
	}

	public int GetTokenAmount(TWDModelManager manager)
	{
		ActorDefinition actorDefinitionForToken = manager.GameEconomyData.GetActorDefinitionForToken(RewardCurrencyType);
		if (actorDefinitionForToken == null)
		{
			return -1;
		}
		bool num = manager.Player.SurvivorContainer.HasHero(actorDefinitionForToken.ID);
		int num2 = 0;
		if (num)
		{
			SurvivorModel heroById = manager.Player.SurvivorContainer.GetHeroById(actorDefinitionForToken.ID);
			CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(heroById);
			int totalCost = heroById.GetUpgradeTraitCashier().GetTotalCost(survivorTraitUpgradeCurrencyType);
			int value = manager.Player.GetCurrency(survivorTraitUpgradeCurrencyType).Value;
			num2 = totalCost - value;
		}
		else
		{
			num2 = actorDefinitionForToken.TokensToUnlock - manager.Player.GetCurrency(RewardCurrencyType).Value;
		}
		if (num2 > MaxTokensGiven)
		{
			num2 = MaxTokensGiven;
		}
		if (num2 <= 0)
		{
			return -1;
		}
		return num2;
	}
}
