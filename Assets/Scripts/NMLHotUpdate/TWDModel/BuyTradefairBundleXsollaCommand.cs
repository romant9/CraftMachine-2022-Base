using BaseModel;

namespace TWDModel
{
	public class BuyTradefairBundleXsollaCommand : ConsumeCurrencyCommand
	{
		public string BundleId { get; private set; }

		public BuyTradefairBundleXsollaCommand()
		{
		}

		public BuyTradefairBundleXsollaCommand(string bundleId)
		{
			BundleId = bundleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel player = (manager as TWDModelManager).Player;
			TradefairBundleStoreDefinition bundleTradefairDefinition = player.gameEconomyData.GetBundleTradefairDefinition(BundleId);
			TradefairBundleContentDefinition tradefairBundleContentDefinition = player.gameEconomyData.GetTradefairBundleContentDefinition(BundleId);
			if (bundleTradefairDefinition == null || tradefairBundleContentDefinition == null)
			{
				manager.Debug.LogError("BuyTradefairBundleXsollaCommand Failed. item definition not found with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tradefairBundleContentDefinition.IsNormalBundle() && !player.TradefairManager.CanBuyBundle(bundleTradefairDefinition))
			{
				manager.Debug.LogError("BuyTradefairBundleXsollaCommand Failed. NomarlBundle item definition can not buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if ((tradefairBundleContentDefinition.BundleType == BundleType.NormalBP && player.BattlePass.PremiumActive) || (tradefairBundleContentDefinition.BundleType == BundleType.BeginerBP && player.BattlePass.PremiumActive))
			{
				manager.Debug.LogError("BuyTradefairBundleXsollaCommand Failed. BattlePass item definition Had buy with ID: " + BundleId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tradefairBundleContentDefinition.BundleType == BundleType.SevenDayPremium)
			{
				if (player.SevenDayLoginManager.CurrentPeriodModel == null)
				{
					manager.Debug.LogError("BuyTradefairBundleXsollaCommand Failed. SevenDayLogin is null");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (player.SevenDayLoginManager.CurrentPeriodModel.IsUnlockPremium)
				{
					manager.Debug.LogError("BuyTradefairBundleXsollaCommand Failed. SevenDayLogin had unlock premium");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
			}
			manager.Debug.LogInfo("BuyTradefairBundleXsollaCommand Success. NomarlBundle item definition can buy with ID: " + BundleId + "  type" + tradefairBundleContentDefinition.IsNormalBundle());
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
