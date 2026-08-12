namespace TWDModel
{
	public class RewardMonthlySubscription : IReward
	{
		public RewardType Type => RewardType.MonthlySubscription;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return manager.Player.SubscriptionManager?.UpdateMonthlySubscriptionStatus(SubscriptionSyncStatus.WaitSync);
		}
	}
}
