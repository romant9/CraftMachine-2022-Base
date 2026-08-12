using BaseModel;

namespace TWDModel
{
	public class BattlePassClaimRewardCommand : ModelCommand
	{
		public int TierNo;

		public bool IsPremium;

		public int RewardIndex;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player.BattlePass.ClaimReward(TierNo, IsPremium, RewardIndex) != null)
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
