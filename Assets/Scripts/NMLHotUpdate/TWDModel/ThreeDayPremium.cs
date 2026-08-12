namespace TWDModel
{
	public class ThreeDayPremium : IReward
	{
		public RewardType Type => RewardType.ThreeDayPremium;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return manager.Player.ThreeDayModel.OnBuyBundle();
		}
	}
}
