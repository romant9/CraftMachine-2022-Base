namespace TWDModel
{
	public class RewardActiveFoundationPremium : IReward
	{
		public RewardType Type => RewardType.ActiveFoundationPremium;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return manager.Player.ActiveFoundationManager.ActivatePremium();
		}
	}
}
