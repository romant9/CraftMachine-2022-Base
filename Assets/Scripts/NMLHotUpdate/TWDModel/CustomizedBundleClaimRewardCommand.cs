using BaseModel;

namespace TWDModel
{
	public class CustomizedBundleClaimRewardCommand : ConsumeCurrencyCommand
	{
		public string Identifier;

		public int index;

		public IReward reward;

		public CustomizedBundleClaimRewardCommand()
		{
		}

		public CustomizedBundleClaimRewardCommand(string id, int index, IReward reward)
		{
			Identifier = id;
			this.index = index;
			this.reward = reward;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!tWDModelManager.Player.gameEconomyData.ConfigData.CustomBundleSwitch)
			{
				return new NGModelCommandRespond(this, TWDModelResult.CustomBundleClosed);
			}
			if (!tWDModelManager.Player.CustomizedBundleManager.UpgradeCustomRewards(Identifier, index, reward))
			{
				return new NGModelCommandRespond(this, TWDModelResult.CustomBundleError);
			}
			TWDModelResult result = TWDModelResult.OK;
			return new NGModelCommandRespond(this, result);
		}
	}
}
