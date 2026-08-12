using BaseModel;

namespace TWDModel
{
	public class EnableDoubleSurvivalRewardsCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			WeeklySurvival currentDefinition = player.WeeklySurvival.CurrentDefinition;
			tWDModelManager.Debug.Log("ResetSurvivalMapCommand: " + tWDModelManager.GameEconomyData.WeeklySurvivals.Count + " survivals.");
			if (currentDefinition == null)
			{
				tWDModelManager.Debug.Log("ResetSurvivalMapCommand Current Survival is null");
			}
			else
			{
				tWDModelManager.Debug.Log("ResetSurvivalMapCommand Current Survival start " + currentDefinition.StartTimeMilliseconds + " end " + currentDefinition.EndTimeMilliseconds);
			}
			if (player.WeeklySurvival.CanRestartMapOrDoubleRewards())
			{
				Cashier doubleRewardsCashier = player.WeeklySurvival.GetDoubleRewardsCashier();
				doubleRewardsCashier.UsedReason = "DistanceDoubleReward";
				TWDModelResult tWDModelResult = doubleRewardsCashier.Pay();
				if (tWDModelResult == TWDModelResult.OK)
				{
					player.WeeklySurvival.EnableDoubleRewards();
				}
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
