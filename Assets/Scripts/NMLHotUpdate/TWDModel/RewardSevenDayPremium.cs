namespace TWDModel
{
	public class RewardSevenDayPremium : IReward
	{
		public RewardType Type => RewardType.SevenDayPremium;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return manager.Player.SevenDayLoginManager.ActivatePremium();
		}
	}
}
