namespace TWDModel
{
	public class RewardBattlePassPremium : IReward
	{
		public RewardType Type => RewardType.BattlePassPremium;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return manager.Player.BattlePass.ActivatePremium() != null;
		}
	}
}
