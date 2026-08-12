namespace TWDModel
{
	public class SupportGiveRewardsLoadQueueMessage : SupportLoadQueueMessage
	{
		public string Rewards { get; set; }

		public SupportGiveRewardsLoadQueueMessage()
		{
		}

		public SupportGiveRewardsLoadQueueMessage(string rewards)
		{
			Rewards = rewards;
		}

		public override bool Execute(TWDModelManager manager)
		{
			if (manager.Player != null && manager.Player.BundleManager != null && Rewards != null)
			{
				return manager.Player.BundleManager.GiveRewardsGivenBySupport(Rewards, base.SupportGivenTimestamp, base.SupportEntityGUID);
			}
			manager.Debug.LogError("Reward failed, missing rewards string or invalid player. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
			return true;
		}
	}
}
