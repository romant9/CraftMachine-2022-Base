namespace TWDModel
{
	public class ReturnEndlessDealPremium : IReward
	{
		public string TargetBundleId { get; set; }

		public RewardType Type => RewardType.ReturnEndlessDealPremium;

		public ReturnEndlessDealPremium(string bundleId)
		{
			TargetBundleId = bundleId;
		}

		public object Give(TWDModelManager manager, object[] param = null)
		{
			manager.Debug.LogInfo("[ReturnEndlessDeal] Premium reward received for bundleId: " + TargetBundleId);
			if (manager.Player?.ReturnActivityManager != null)
			{
				manager.Player.ReturnActivityManager.OnEndlessDealBundleBought(TargetBundleId);
			}
			return null;
		}
	}
}
