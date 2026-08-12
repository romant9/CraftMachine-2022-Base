using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ClaimNormalProgressRewardCommand : ModelCommand
	{
		[JsonIgnore]
		public Rewards Rewards;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EndlessModeManagerModel endlessModeManager = tWDModelManager.Player.EndlessModeManager;
			if (endlessModeManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (endlessModeManager.GiveAttemptNormalProgressRewards(out var progressRewards) != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Rewards = new Rewards();
			if (progressRewards != null && progressRewards.RewardsList.Count > 0)
			{
				Rewards.RewardsList.AddRange(progressRewards.RewardsList);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
