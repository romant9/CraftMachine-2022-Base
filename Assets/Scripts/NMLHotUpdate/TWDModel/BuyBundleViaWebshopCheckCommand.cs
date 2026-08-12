using BaseModel;

namespace TWDModel
{
	public class BuyBundleViaWebshopCheckCommand : ModelCommand
	{
		public string BundleId { get; private set; }

		public BuyBundleViaWebshopCheckCommand()
		{
		}

		public BuyBundleViaWebshopCheckCommand(string bundleId)
		{
			BundleId = bundleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel player = (manager as TWDModelManager).Player;
			BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(BundleId);
			BundleContentDefinition bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(BundleId);
			if (bundleStoreDefinition == null || bundleContentDefinition == null)
			{
				manager.Debug.LogError("BuyBundleViaWebshopCheckCommand Failed. item definition not found with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (bundleContentDefinition.IsNormalBundle() && !player.BundleManager.CanBuyBundle(bundleStoreDefinition))
			{
				manager.Debug.LogError("BuyBundleViaWebshopCheckCommand Failed. NomarlBundle item definition can not buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if ((bundleContentDefinition.BundleType == BundleType.NormalBP && player.BattlePass.PremiumActive) || (bundleContentDefinition.BundleType == BundleType.BeginerBP && player.BattlePass.PremiumActive))
			{
				manager.Debug.LogError("BuyBundleViaWebshopCheckCommand Failed. BattlePass item definition Had buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (bundleContentDefinition.BundleType == BundleType.SevenDayPremium)
			{
				if (player.SevenDayLoginManager.CurrentPeriodModel == null)
				{
					manager.Debug.LogError("BuyBundleViaWebshopCheckCommand Failed. SevenDayLogin is null");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (player.SevenDayLoginManager.CurrentPeriodModel.IsUnlockPremium)
				{
					manager.Debug.LogError("BuyBundleViaWebshopCheckCommand Failed. SevenDayLogin had unlock premium");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
