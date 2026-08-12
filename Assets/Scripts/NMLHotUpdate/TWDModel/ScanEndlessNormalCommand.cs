using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ScanEndlessNormalCommand : ModelCommand
	{
		[JsonIgnore]
		public Rewards Rewards;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				EndlessModeManagerModel endlessModeManager = tWDModelManager.Player.EndlessModeManager;
				if (endlessModeManager == null || !endlessModeManager.CheckCanScanNormal())
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				int missionBaseCost = tWDModelManager.GameEconomyData.EndlessModeConfig.MissionBaseCost;
				Cashier cashier = Cashier.CreateOneItemCashier(tWDModelManager, PurchaseType.EndlessPass, CurrencyType.EndlessPassToken, missionBaseCost);
				cashier.UseDiamondsAmount = -2;
				TWDModelResult tWDModelResult = cashier.Pay();
				if (tWDModelResult != TWDModelResult.OK)
				{
					return new NGModelCommandRespond(this, tWDModelResult);
				}
				if (cashier.ExchangedDiamonds > 0)
				{
					endlessModeManager.CurrentGoldAttemptCount++;
				}
				tWDModelManager.Metrics.ResourceChangeUsedReason = "LastStandAutoClear";
				tWDModelManager.Metrics.AddResources(cashier);
				endlessModeManager.ScanNormal();
				Rewards regularRewards;
				TWDModelResult num = endlessModeManager.GiveAttemptRegularRewards(out regularRewards);
				Rewards rewards;
				TWDModelResult tWDModelResult2 = endlessModeManager.GiveAttemptRewards(out rewards);
				TWDModelResult tWDModelResult3 = ((num != TWDModelResult.OK && tWDModelResult2 != TWDModelResult.OK) ? TWDModelResult.Error : TWDModelResult.OK);
				if (tWDModelResult3 != TWDModelResult.OK)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Rewards = new Rewards();
				if (regularRewards != null && regularRewards.RewardsList.Count > 0)
				{
					Rewards.RewardsList.AddRange(regularRewards.RewardsList);
				}
				if (rewards != null && rewards.RewardsList.Count > 0)
				{
					Rewards.RewardsList.AddRange(rewards.RewardsList);
				}
				return new NGModelCommandRespond(this, tWDModelResult3);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
