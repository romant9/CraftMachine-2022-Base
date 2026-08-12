namespace TWDModel
{
	public class RewardWeeklySubscription : IReward
	{
		public RewardType Type => RewardType.WeeklySubscription;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return manager.Player.SubscriptionManager?.UpdateWeeklySubscriptionStatus(SubscriptionSyncStatus.WaitSync);
		}
	}
}
