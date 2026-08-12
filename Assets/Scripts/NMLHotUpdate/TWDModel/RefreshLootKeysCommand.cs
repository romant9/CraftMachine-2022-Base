using BaseModel;

namespace TWDModel
{
	public class RefreshLootKeysCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			CurrencyModel currency = player.GetCurrency(CurrencyType.LootKeys);
			int lootKeySoftCap = player.ActivityManager.GetLootKeySoftCap(tWDModelManager.GameEconomyData.ConfigData);
			if (player.UtcTimeStamp - player.LootKeysFirstSpentTime >= player.gameEconomyData.ConfigData.LootKeyRefreshRate && currency.Value < lootKeySoftCap)
			{
				int amount = lootKeySoftCap - currency.Value;
				RewardCurrency rewardCurrency = new RewardCurrency();
				rewardCurrency.CurrencyType = CurrencyType.LootKeys;
				rewardCurrency.Amount = amount;
				rewardCurrency.Give(tWDModelManager);
				tWDModelManager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, rewardCurrency.Amount, rewardCurrency.AmountActuallyAdded).AddLootKeysRefresh()
					.Send();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
