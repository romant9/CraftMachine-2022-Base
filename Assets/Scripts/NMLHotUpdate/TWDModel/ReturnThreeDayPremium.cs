namespace TWDModel
{
	public class ReturnThreeDayPremium : IReward
	{
		public RewardType Type => RewardType.ReturnThreeDayPremium;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			if (manager?.Player?.ReturnActivityManager == null)
			{
				return false;
			}
			return manager.Player.ReturnActivityManager.OnThreeDayBundleBought();
		}
	}
}
