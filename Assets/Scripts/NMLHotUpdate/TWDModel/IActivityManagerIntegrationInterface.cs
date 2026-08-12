namespace TWDModel
{
	public interface IActivityManagerIntegrationInterface
	{
		string GetIntegrationEventId();

		bool CanShowInActivityList();

		[ActivityIntegrationInvokeOrder(int.MaxValue, InvokeOrder = 0)]
		bool AreThereAnyUnclaimedReward();

		[ActivityIntegrationInvokeOrder(int.MaxValue, InvokeOrder = 1)]
		bool AreThereCanCompleteTask();

		[ActivityIntegrationInvokeOrder(int.MaxValue, InvokeOrder = 2)]
		bool IsActivityOpen();
	}
}
